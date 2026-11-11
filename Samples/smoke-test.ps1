#requires -Version 7.4
# Build the Release solution first. No extra PowerShell modules are required.
[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path $PSScriptRoot -Parent
$dotnet = (Get-Command dotnet -CommandType Application).Source
$nameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
$emailClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
$roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

function Assert-Condition([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Invoke-SampleRequest([System.Net.Http.HttpClient] $Client, [string] $Uri, [string] $RequestHost) {
    $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::Get, $Uri)
    try {
        if ($RequestHost) { $request.Headers.Host = $RequestHost }
        $response = $Client.Send($request)
        try {
            [pscustomobject]@{
                Status = [int] $response.StatusCode
                Body = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            }
        }
        finally { $response.Dispose() }
    }
    finally { $request.Dispose() }
}

function Test-Sample([int] $Version, [hashtable] $Case) {
    $sampleName = "FakeAuth.SampleWeb$Version.0"
    $projectDirectory = Join-Path $repositoryRoot "Samples/$sampleName"
    $assembly = Join-Path $projectDirectory "bin/Release/net$Version.0/$sampleName.dll"
    Assert-Condition (Test-Path $assembly -PathType Leaf) "Build the Release solution first: missing $assembly"

    $listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Loopback, 0)
    $listener.Start()
    try { $port = $listener.LocalEndpoint.Port }
    finally { $listener.Stop() }
    $url = "http://localhost:$port"

    $handler = [System.Net.Http.HttpClientHandler]::new()
    $handler.UseProxy = $false
    $client = [System.Net.Http.HttpClient]::new($handler)
    $client.Timeout = [TimeSpan]::FromSeconds(2)
    $process = [System.Diagnostics.Process]::new()
    $process.StartInfo = [System.Diagnostics.ProcessStartInfo]::new($dotnet)
    $process.StartInfo.UseShellExecute = $false
    $process.StartInfo.WorkingDirectory = $projectDirectory
    $process.StartInfo.RedirectStandardOutput = $true
    $process.StartInfo.RedirectStandardError = $true
    $process.StartInfo.Environment['ASPNETCORE_ENVIRONMENT'] = 'Development'
    foreach ($argument in @($assembly, '--urls', $url, '--profile', $Case.Profile)) {
        $process.StartInfo.ArgumentList.Add($argument)
    }
    $started = $false
    $failed = $false
    try {
        $started = $process.Start()
        $stdout = $process.StandardOutput.ReadToEndAsync()
        $stderr = $process.StandardError.ReadToEndAsync()
        $timer = [System.Diagnostics.Stopwatch]::StartNew()
        while ($true) {
            Assert-Condition (-not $process.HasExited) 'Sample exited before becoming ready.'
            try {
                $response = Invoke-SampleRequest $client "$url/api/claims"
                break
            }
            catch [System.Net.Http.HttpRequestException], [System.OperationCanceledException] {
                if ($timer.Elapsed.TotalSeconds -ge 30) { throw 'Sample did not become ready within 30 seconds.' }
                Start-Sleep -Milliseconds 100
            }
        }
        Assert-Condition ($response.Status -eq 200) "Claims endpoint returned $($response.Status)."
        $claims = @{}
        foreach ($claim in (ConvertFrom-Json $response.Body)) { $claims[$claim.type] = $claim.value }
        Assert-Condition ($claims[$nameClaim] -eq $Case.Name) 'Unexpected name claim.'
        Assert-Condition ($claims[$emailClaim] -eq $Case.Email) 'Unexpected email claim.'
        Assert-Condition (($claims[$roleClaim] -eq 'Manager') -eq $Case.Manager) 'Unexpected Manager role.'
        $response = Invoke-SampleRequest $client "$url/api/manager"
        $expectedStatus = if ($Case.Manager) { 200 } else { 403 }
        Assert-Condition ($response.Status -eq $expectedStatus) "Manager endpoint returned $($response.Status)."
        if ($Case.Manager) {
            Assert-Condition ((ConvertFrom-Json $response.Body).name -eq $Case.Name) 'Unexpected authorized user.'
        }
        $response = Invoke-SampleRequest $client "$url/api/claims" 'example.com'
        Assert-Condition ($response.Status -eq 401) "Unexpected host accepted: HTTP $($response.Status)."
        Write-Host "PASS .NET ${Version}: $($Case.Profile) claims, role authorization, host restriction"
    }
    catch { $failed = $true; throw }
    finally {
        if ($started) {
            if (-not $process.HasExited) { $process.Kill($true) }
            $process.WaitForExit()
            if ($failed) {
                Write-Host $stdout.GetAwaiter().GetResult()
                Write-Host $stderr.GetAwaiter().GetResult()
            }
        }
        $process.Dispose()
        $client.Dispose()
    }
}

foreach ($version in @(10, 11)) {
    foreach ($case in @(
        @{ Profile = 'default'; Name = 'Fake User'; Email = 'fake@fakeuser.com'; Manager = $false },
        @{ Profile = 'manager'; Name = 'Manager User'; Email = 'manager@example.test'; Manager = $true },
        @{ Profile = 'inline'; Name = 'Inline User'; Email = 'inline@example.test'; Manager = $true }
    )) {
        Test-Sample $version $case
    }
}
Write-Host 'All 6 sample configurations passed.'
