# Supported .NET migration

## Intent and scope

Upgrade FakeAuth to the .NET 11 toolchain while supporting both currently supported runtime versions, .NET 10 and 11. Remove samples targeting retired runtimes. Microsoft's release index retrieved on November 10, 2026 lists 10 and 11 as active and 8 and 9 as end of life.

## Approaches

The user selected support for .NET 10 and 11 rather than an 11-only library. Multi-targeting lets the two version-specific samples reference the same local library and verifies compatibility on each runtime. Keeping the old targets would contradict the requested removal of deprecated versions.

## Proposed implementation

- Target the library, both existing test projects, and the integration-test web host at net10.0 and net11.0. Use SDK 11 with both runtimes installed; pin the SDK in global.json.
- Replace obsolete ASP.NET Core authentication package references with the Microsoft.AspNetCore.App framework reference. Use the current AuthenticationHandler constructor without ISystemClock. Preserve AddFakeAuth, profile, claim-header, and allowed-host behavior.
- Update required test dependencies to compatible stable versions, preserving the existing tests and assertions. Avoid unrelated behavior changes or test-framework migration.
- Remove all four existing Samples projects targeting 3.1, 5, and 6. Add Samples/FakeAuth.SampleWeb10.0 and Samples/FakeAuth.SampleWeb11.0 as independent runnable ASP.NET Core examples using local project references. Each demonstrates default authentication, a custom profile, inline claims, and role authorization through documented routes/configuration. Prefer small HTTP examples without external identity-provider dependencies or bundled frontend assets.
- Keep the existing integration web host and its routes, modernize dependencies as needed, and remove unused identity-provider integration where it only serves template UI. Retain host-rewrite support for allowed-host tests.
- Update FakeAuth.sln, both GitHub Actions workflows, and README links and commands. CI installs SDK 11 and runtime 10, builds and tests both targets, and retains the existing branch and package-publishing conditions. Do not publish packages during this task.
- Refresh the saved cloud install and startup instructions to match the upgraded workflow after validation.

## Compatibility and exclusions

Dropping legacy target frameworks and the obsolete clock-based handler constructor is a breaking compatibility change. Propose package version 3.0.0, with the rationale documented in README. No change to the development-only purpose or localhost restriction. No production identity integration, UI redesign, or extra supported-framework samples beyond 10 and 11.

## Validation

Install official SDK/runtime artifacts with checksum and TLS verification. Restore and Release-build the full solution, run the existing 20 tests on each framework (40 executions total), and verify that no tests are skipped. Smoke-test each new sample for expected claims, successful authorized access, and denied access when the required role is absent. Build the package and inspect its net10.0/net11.0 contents. Check solution membership and documentation for obsolete sample links, and review the final diff. Record any genuine compatibility blockers rather than bypassing checks.

## Builder decisions

The implementer chooses compatible stable dependency versions, minimal sample routing/configuration, concrete SDK patch version, and any additional regression tests justified by migration changes. Preserve user changes and use the existing isolated checkout, without a new worktree.
