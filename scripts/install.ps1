#requires -Version 7.4
# Prepare the repository's pinned .NET SDK and runtimes without administrator access.
[CmdletBinding()]
param(
    [string] $RepositoryRoot = (Split-Path $PSScriptRoot -Parent),
    [string] $WorkspaceRoot = (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$RepositoryRoot = [System.IO.Path]::GetFullPath($RepositoryRoot)
$WorkspaceRoot = [System.IO.Path]::GetFullPath($WorkspaceRoot)
$dotnetRoot = Join-Path $WorkspaceRoot '.dotnet11'
$setupRoot = Join-Path $WorkspaceRoot 'setup'
$cache = Join-Path $setupRoot 'dotnet-upgrade'
$architecture = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString().ToLowerInvariant()
$platform = if ($IsWindows) { 'win' } elseif ($IsMacOS) { 'osx' } elseif ($IsLinux) { 'linux' } else { throw 'Unsupported operating system.' }
if ($architecture -notin @('x64', 'arm64')) { throw "Unsupported architecture: $architecture" }
$rid = "$platform-$architecture"
$extension = if ($IsWindows) { 'zip' } else { 'tar.gz' }
$dotnet = Join-Path $dotnetRoot $(if ($IsWindows) { 'dotnet.exe' } else { 'dotnet' })
$sdkVersion = (Get-Content (Join-Path $RepositoryRoot 'global.json') -Raw | ConvertFrom-Json).sdk.version
$runtimeVersion = '10.0.15'

foreach ($directory in @($dotnetRoot, $cache, (Join-Path $WorkspaceRoot '.nuget/packages'))) {
    [void] [System.IO.Directory]::CreateDirectory($directory)
}

function Expand-VerifiedArchive([string] $Archive, [string] $Destination) {
    if ($Archive.EndsWith('.zip')) {
        [System.IO.Compression.ZipFile]::ExtractToDirectory($Archive, $Destination, $true)
    }
    else {
        $stream = [System.IO.File]::OpenRead($Archive)
        $gzip = [System.IO.Compression.GZipStream]::new($stream, [System.IO.Compression.CompressionMode]::Decompress)
        try { [System.Formats.Tar.TarFile]::ExtractToDirectory($gzip, $Destination, $true) }
        finally { $gzip.Dispose(); $stream.Dispose() }
    }
}

$jobs = @(
    @{ Channel = '11.0'; Kind = 'sdk'; Version = $sdkVersion; Marker = "sdk/$sdkVersion/dotnet.dll" },
    @{ Channel = '10.0'; Kind = 'aspnetcore-runtime'; Version = $runtimeVersion; Marker = "shared/Microsoft.AspNetCore.App/$runtimeVersion/Microsoft.AspNetCore.Hosting.dll" }
)
foreach ($job in $jobs) {
    $installed = (Test-Path (Join-Path $dotnetRoot $job.Marker) -PathType Leaf) -and (Test-Path $dotnet -PathType Leaf)
    if ($job.Kind -eq 'aspnetcore-runtime') {
        $installed = $installed -and (Test-Path (Join-Path $dotnetRoot "shared/Microsoft.NETCore.App/$runtimeVersion/Microsoft.NETCore.App.runtimeconfig.json") -PathType Leaf)
    }
    if ($installed) { continue }

    $metadata = Invoke-RestMethod "https://builds.dotnet.microsoft.com/dotnet/release-metadata/$($job.Channel)/releases.json"
    $component = if ($job.Kind -eq 'sdk') {
        $metadata.releases | ForEach-Object { $_.sdks } | Where-Object version -EQ $job.Version | Select-Object -First 1
    }
    else {
        $metadata.releases | ForEach-Object { $_.'aspnetcore-runtime' } | Where-Object version -EQ $job.Version | Select-Object -First 1
    }
    $filename = if ($job.Kind -eq 'sdk') { "dotnet-sdk-$rid.$extension" } else { "aspnetcore-runtime-$rid.$extension" }
    $artifact = $component.files | Where-Object { $_.rid -eq $rid -and $_.name -eq $filename } | Select-Object -First 1
    if (-not $artifact) { throw "No official $($job.Kind) $($job.Version) artifact for $rid." }
    $archive = Join-Path $cache "$($job.Kind)-$($job.Version)-$rid.$extension"
    Invoke-WebRequest $artifact.url -OutFile $archive
    if ((Get-FileHash $archive -Algorithm SHA512).Hash -ne $artifact.hash) {
        throw "SHA512 mismatch: $archive"
    }
    Write-Host "SHA512 verified: $archive"
    if ($job.Kind -eq 'sdk') {
        Expand-VerifiedArchive $archive $dotnetRoot
    }
    else {
        $staging = Join-Path $cache ([guid]::NewGuid().ToString('N'))
        [void] [System.IO.Directory]::CreateDirectory($staging)
        try {
            Expand-VerifiedArchive $archive $staging
            # Copy only shared runtimes; runtime 10 must not replace SDK 11's host.
            Copy-Item (Join-Path $staging 'shared/*') (Join-Path $dotnetRoot 'shared') -Recurse -Force
        }
        finally { Remove-Item $staging -Recurse -Force }
    }
}

$environment = [ordered]@{
    DOTNET_ROOT = $dotnetRoot
    DOTNET_CLI_HOME = (Join-Path $setupRoot 'dotnet-home')
    NUGET_PACKAGES = (Join-Path $WorkspaceRoot '.nuget/packages')
    XDG_DATA_HOME = (Join-Path $setupRoot 'xdg-data')
    XDG_CONFIG_HOME = (Join-Path $setupRoot 'xdg-config')
    XDG_CACHE_HOME = (Join-Path $setupRoot 'xdg-cache')
    DOTNET_NOLOGO = '1'
    DOTNET_CLI_TELEMETRY_OPTOUT = '1'
}
if (-not $IsWindows) {
    # .NET 11's first-run named mutex also needs a writable, private runtime directory.
    $runtimeDirectory = Join-Path $setupRoot 'runtime'
    $privateMode = [System.IO.UnixFileMode]::UserRead -bor [System.IO.UnixFileMode]::UserWrite -bor [System.IO.UnixFileMode]::UserExecute
    [void] [System.IO.Directory]::CreateDirectory($runtimeDirectory, $privateMode)
    $environment.Add('XDG_RUNTIME_DIR', $runtimeDirectory)
}
$activation = foreach ($entry in $environment.GetEnumerator()) {
    [System.Environment]::SetEnvironmentVariable($entry.Key, $entry.Value, 'Process')
    '$env:' + $entry.Key + " = '" + $entry.Value.Replace("'", "''") + "'"
}
$env:PATH = $dotnetRoot + [System.IO.Path]::PathSeparator + $env:PATH
$activation += '$env:PATH = $env:DOTNET_ROOT + [System.IO.Path]::PathSeparator + $env:PATH'
Set-Content (Join-Path $setupRoot 'activate.ps1') $activation -Encoding utf8

function Invoke-DotNet([string[]] $Arguments) {
    & $dotnet @Arguments
    if ($LASTEXITCODE -ne 0) { throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE." }
}

Push-Location $RepositoryRoot
try {
    Invoke-DotNet -Arguments @('--version')
    Invoke-DotNet -Arguments @('--list-runtimes')
    Invoke-DotNet -Arguments @('restore', 'FakeAuth.sln')
    Invoke-DotNet -Arguments @('build', 'FakeAuth.sln', '--configuration', 'Release', '--no-restore', '--maxcpucount:2')
}
finally { Pop-Location }
