# FakeAuth

[![Development CI](https://github.com/calebjenkins/FakeAuth/actions/workflows/dev-ci.yml/badge.svg?branch=Develop)](https://github.com/calebjenkins/FakeAuth/actions/workflows/dev-ci.yml)
[![Main CI](https://github.com/calebjenkins/FakeAuth/actions/workflows/main-ci-test-publish.yml/badge.svg?branch=main)](https://github.com/calebjenkins/FakeAuth/actions/workflows/main-ci-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/vpre/fakeauth.svg)](https://www.nuget.org/packages/fakeauth)

![FakeAuth logo](./assets/logo/FakeAuth_med.png "FakeAuth for local OAuth development")

FakeAuth makes local development and testing of ASP.NET Core applications that use OAuth or OIDC easier. Use default claims, custom profiles, or claims set per test client without configuring an identity provider.

**For development and testing only. Do not use FakeAuth in production.** Authentication accepts `localhost` by default; explicitly configure `FakeAuthOptions.AllowedHosts` for other development hosts.

## Supported .NET versions

Version 3.0.0 targets **.NET 10 and .NET 11**, with a runnable sample for each. Microsoft lists these versions as supported as of November 10, 2026; .NET 8 and 9 reached end of support on that date. Samples for .NET Core 3.1, .NET 5, and .NET 6 have been removed.

| Runtime | Library target | Sample |
| --- | --- | --- |
| .NET 10 | `net10.0` | [FakeAuth.SampleWeb10.0](Samples/FakeAuth.SampleWeb10.0) |
| .NET 11 | `net11.0` | [FakeAuth.SampleWeb11.0](Samples/FakeAuth.SampleWeb11.0) |

## Install and use

```sh
dotnet add package FakeAuth
```

The local 3.0.0 source must be built or packaged to use the changes before that version is published. Both samples reference the local library directly.

Register FakeAuth in an ASP.NET Core application's `Program.cs`:

```csharp
using FakeAuth;

builder.Services.AddAuthentication().AddFakeAuth();
builder.Services.AddAuthorization();

// After builder.Build(), before mapping protected endpoints:
app.UseAuthentication();
app.UseAuthorization();
```

The default profile supplies the name `Fake User` and email `fake@fakeuser.com`. To provide inline claims:

```csharp
using System.Security.Claims;

builder.Services.AddAuthentication().AddFakeAuth(options =>
{
    options.Claims.Add(new Claim(ClaimTypes.Name, "Joe Manager"));
    options.Claims.Add(new Claim(ClaimTypes.Role, "Manager"));
});
```

For reusable profiles, implement [IFakeAuthProfile](src/FakeAuth/Profiles/IFakeAuthProfile.cs) and use `AddFakeAuth<MyProfile>()`. See the sample [ManagerProfile](Samples/FakeAuth.SampleWeb11.0/ManagerProfile.cs) and [registration examples](Samples/FakeAuth.SampleWeb11.0/Program.cs).

## Build and test

### VS Code Dev Container

Install Docker and the VS Code **Dev Containers** extension (`ms-vscode-remote.remote-containers`), open this repository in VS Code, and run **Dev Containers: Reopen in Container** from the Command Palette.

The [Dev Container configuration](.devcontainer/devcontainer.json) installs SDK **11.0.100**, matching `global.json`, the **ASP.NET Core 10.0.15 runtime** for the .NET 10 tests and sample, and **PowerShell 7.7.0**. It restores solution dependencies automatically and includes the C# Dev Kit and PowerShell extensions. The default terminal is PowerShell; sample ports 5010 and 5011 are forwarded.

Once the container opens, use the build, test, and sample commands below. No separate host .NET installation or manual `install.ps1` run is needed. After changing the container's SDK or runtime versions, run **Dev Containers: Rebuild Container**; keep the SDK version aligned with `global.json`.

### Local installation

Install the **.NET 11 SDK** selected by [global.json](global.json), plus the **.NET 10 ASP.NET Core runtime**. Installing the .NET 10 SDK alongside SDK 11 also supplies that runtime. The solution's library, test suites, and integration-test host target both frameworks.

Alternatively, with PowerShell 7.4 or later installed, run the user-local installer in a PowerShell terminal:

```powershell
./scripts/install.ps1
```

The [installer](scripts/install.ps1) downloads the pinned SDK and runtime for your operating system and architecture, verifies their official SHA-512 checksums, then restores and builds the solution. It stores tools and caches in the checkout's parent directory. In later PowerShell sessions, run `. ../setup/activate.ps1` to activate those tools. Use `-WorkspaceRoot` to choose another installation directory.

From the repository root:

```sh
dotnet restore FakeAuth.sln
dotnet build FakeAuth.sln --configuration Release --no-restore
dotnet test FakeAuth.sln --configuration Release --no-build --no-restore
pwsh -NoProfile -File Samples/smoke-test.ps1
```

The existing test suites run 20 tests per framework, for 40 executions total. The smoke runner needs PowerShell 7.4 or later (`pwsh`) and checks both samples in all three profile modes, including denied Manager access and rejection of unexpected hosts. It starts and stops its own sample processes. CI runs these same checks using VSTest.

## Run the samples

From the repository root, after building:

```sh
dotnet run --project Samples/FakeAuth.SampleWeb10.0 --configuration Release --no-build --no-launch-profile -- --urls http://localhost:5010 --profile default
dotnet run --project Samples/FakeAuth.SampleWeb11.0 --configuration Release --no-build --no-launch-profile -- --urls http://localhost:5011 --profile manager
```

Run each command in its own terminal. Each sample accepts `--profile default`, `--profile manager`, or `--profile inline`; restart with another value to switch registration styles.

| Mode | Registration | Name | Manager access |
| --- | --- | --- | --- |
| `default` | `AddFakeAuth()` | Fake User | Denied (403) |
| `manager` | `AddFakeAuth<ManagerProfile>()` | Manager User | Allowed (200) |
| `inline` | `AddFakeAuth(options => ...)` | Inline User | Allowed (200) |

Use `localhost` in requests so the host passes FakeAuth's default restriction:

```sh
curl http://localhost:5010/api/claims
curl -i http://localhost:5010/api/manager
curl http://localhost:5011/api/manager
```

`/api/claims` returns the authenticated user's claim type/value pairs. `/api/manager` requires the `Manager` role. The examples need no Azure AD configuration, database, or external authentication service.

## Testing your application

FakeAuth works with ASP.NET Core's testing framework. See [FakeAuth.IntegrationTests](Tests/FakeAuth.IntegrationTests) for role and allowed-host tests. Set claims on a specific `HttpClient`:

```csharp
using FakeAuth.Testing;
using System.Security.Claims;

client.SetFakeAuthClaims(
    new Claim(ClaimTypes.Name, "Joe Manager"),
    new Claim(ClaimTypes.Role, "Manager")
);
```

Reuse a profile with `client.SetFakeAuthClaims<DefaultProfile>()` (from `FakeAuth.Profiles`). This lets tests verify access with and without required roles.

## Migrating to 3.0.0

- The minimum supported framework is .NET 10. Legacy .NET Core 3.1 and .NET 6 library targets are removed.
- The library uses the `Microsoft.AspNetCore.App` shared framework instead of the retired ASP.NET Core 2.2 authentication package.
- `FakeAuthHandler` uses the current three-argument constructor: options monitor, logger factory, and URL encoder. Code constructing or subclassing it must remove the obsolete `ISystemClock` argument.
- `AddFakeAuth`, profiles, test-client claims, and allowed-host behavior are preserved. Older NuGet versions remain separate from the source version built here.

## Contributing

Please target pull requests to the `Develop` branch. Build and run both framework test suites and sample checks before submitting changes.

Background: [why FakeAuth was built](https://developingux.com/introducing-fakeauth/) and [additional articles](https://developingux.com/fakeauth).
