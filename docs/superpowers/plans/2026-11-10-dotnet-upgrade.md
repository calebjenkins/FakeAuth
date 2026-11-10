# .NET 10 and 11 Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox syntax for tracking.

**Goal:** Support .NET 10 and 11 throughout FakeAuth and replace retired-framework samples.

**Architecture:** Multi-target the existing library, test suites and test host. Use two small independent HTTP sample projects referencing the library and matching CI/runtime tooling.

**Tech Stack:** .NET SDK 11, ASP.NET Core 10/11, xUnit v2, VSTest.

**Spec:** docs/superpowers/specs/2026-11-10-dotnet-upgrade-design.md

## Global Constraints

- Support net10.0 and net11.0, removing legacy targets and samples.
- Preserve AddFakeAuth, profile, claim-header, and allowed-host behavior.
- Package version 3.0.0; do not publish packages.
- Use the existing isolated checkout, without a new worktree.
- Keep TLS and artifact checksum verification enabled.

## Review Focus

- A package consumer needs both framework assemblies and the ASP.NET shared-framework dependency; inspect the nupkg and compile sample consumers.
- The runtime-specific test host must match the test runtime; inspect test results separately per framework.
- Localhost restrictions must still reject unexpected hosts; retain the existing host tests on both runtimes.
- Default users must be denied Manager routes while profile/inline users can access them; smoke-test all sample modes.
- Fresh checkouts need working SDK/runtime setup and no dependency on unreleased local packages; use project references, CI setup and saved install instructions.

### Task 1: Library and tests on supported runtimes

**Files:** global.json; src/FakeAuth/FakeAuth.csproj; src/FakeAuth/FakeAuthHandler.cs; Tests/*/*.csproj; Tests/intigrationtests.SampleWeb/Program.cs; Tests/intigrationtests.SampleWeb/Pages/Shared/_LoginPartial.cshtml.

**Interfaces:** Produces FakeAuth for net10.0/net11.0 with the existing registration API and the current three-argument handler constructor; existing test-host endpoints remain /api/open and /api/protected.

- [ ] Install checksum-verified SDK 11 and ASP.NET runtime 10, inspecting official release metadata.
- [ ] Change framework/dependency configuration, then build to expose authentication API incompatibilities before changing handler code.
- [ ] Use the modern handler constructor and remove unused identity-provider dependencies/template links.
- [ ] Run existing test suites on each runtime: expect 3 extension and 17 integration tests per runtime, zero failures or skips.
- [ ] Commit the library/test migration after the checks pass.

### Task 2: Supported samples and solution membership

**Files:** Samples/FakeAuth.SampleWeb10.0/{FakeAuth.SampleWeb10.0.csproj,Program.cs,ManagerProfile.cs}; matching files under Samples/FakeAuth.SampleWeb11.0; Samples/smoke-test.py; FakeAuth.sln; remove all four old Samples project directories.

**Interfaces:** Each sample accepts --profile default|manager|inline (default default); GET /api/claims returns claim type/value pairs, GET /api/manager requires Manager. Ports are provided by --urls; profile mode uses IFakeAuthProfile, inline mode uses AddFakeAuth options.

- [ ] Write a standard-library HTTP smoke runner that starts both samples in each mode and asserts default name/email, custom profile name, inline name, Manager success, default Manager denial and unexpected-host rejection. Run before sample implementation: expect failure because sample projects do not exist.
- [ ] Add the minimal sample projects and implementations, replacing legacy solution membership.
- [ ] Build the complete solution and run python3 Samples/smoke-test.py: expect all six sample configurations pass and child processes stop.
- [ ] Commit the samples and smoke check.

### Task 3: CI, documentation and reusable environment

**Files:** .github/workflows/dev-ci.yml; .github/workflows/main-ci-test-publish.yml; README.md; saved environment install_script and start_skill.

**Interfaces:** Consumes solution/tests/sample smoke runner from tasks 1 and 2. Produces repeatable restore/build/test/sample checks using SDK 11 plus runtime 10.

- [ ] Configure CI installation of SDK 11 and runtime 10; retain existing branch and package-publishing conditions; run sample smoke checks.
- [ ] Rewrite stale sample and framework documentation, explain the breaking 3.0.0 change and local run/test commands.
- [ ] Run restore, Release build, both runtime test suites, sample smoke checks and pack. Inspect nupkg lib/net10.0 and lib/net11.0 plus framework references; expect no old targets or sample links outside history/design documentation.
- [ ] Exercise the saved installation commands and persist complete install_script/start_skill updates.
- [ ] Commit the remaining changes and request one independent whole-branch review. Resolve important findings and run affected checks before reporting completion.
