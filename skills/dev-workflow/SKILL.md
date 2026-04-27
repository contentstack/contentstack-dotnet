---
name: dev-workflow
description: Local development and CI workflow for the Contentstack .NET SDK — solution layout, dotnet build/test commands, package versioning in Directory.Build.props, GitHub Actions (unit tests, CodeQL, NuGet), and integration test credentials. Use when onboarding, running builds, preparing a PR, or finding where CI is defined.
---

# Dev workflow – Contentstack .NET SDK

## When to use

- Onboarding, building, or running tests locally.
- Finding which workflow applies to PRs, releases, or security scans.
- Maintainer tasks: DocFX, NuGet publish expectations.

## Invariants

- Version in **`Directory.Build.props`** only for package version.
- Integration tests need **`app.config`** (or equivalent) — do not commit secrets.

## Repo tooling — CI, release, docs, security

### Local commands

| Action | Command |
|--------|---------|
| Build | `dotnet build Contentstack.Net.sln` |
| Unit tests | `dotnet test Contentstack.Core.Unit.Tests/Contentstack.Core.Unit.Tests.csproj` |
| Integration tests | `dotnet test Contentstack.Core.Tests/Contentstack.Core.Tests.csproj` (credentials) |

Package version: [`Directory.Build.props`](../../../Directory.Build.props) (`<Version>`).

### CI — unit tests

[`.github/workflows/unit-test.yml`](../../../.github/workflows/unit-test.yml): on `pull_request` and `push`, Windows runner, .NET 7, `dotnet restore` → `dotnet build Contentstack.Net.sln` → `dotnet test` on `Contentstack.Core.Unit.Tests`.

### CI — branch policy

[`.github/workflows/check-branch.yml`](../../../.github/workflows/check-branch.yml): on `pull_request`, if base is `master` and head is not `staging`, the job fails with a message to open PRs from `staging` toward `master` per team policy.

### CI — CodeQL

[`.github/workflows/codeql-analysis.yml`](../../../.github/workflows/codeql-analysis.yml): static analysis on PRs (language matrix includes C# as configured in the workflow).

### Release — NuGet

[`.github/workflows/nuget-publish.yml`](../../../.github/workflows/nuget-publish.yml): triggered on **`release` `created`**.

- `dotnet pack -c Release -o out`
- Push `contentstack.csharp.*.nupkg` to NuGet.org (`NUGET_API_KEY` / `NUGET_AUTH_TOKEN` secrets — names appear in workflow; values are org secrets).
- Secondary job may push to GitHub Packages (`nuget.pkg.github.com`).

Maintainers: ensure `Directory.Build.props` version matches the release tag policy before publishing.

### DocFX API docs

Configuration: [`docfx_project/docfx.json`](../../../docfx_project/docfx.json). Output site: `_site/` under the docfx project.

Prerequisite: [DocFX](https://dotnet.github.io/docfx/) CLI installed.

```bash
cd docfx_project
docfx docfx.json
# or: docfx build docfx.json
```

If metadata `src` paths do not resolve (e.g. `src/**.csproj`), adjust `docfx.json` or project layout so metadata generation matches this repository’s structure.

Related: [`filterRules.yml`](../../../docfx_project/filterRules.yml) for API filter rules.

### Security / policy automation

**SCA (dependencies):** [`.github/workflows/sca-scan.yml`](../../../.github/workflows/sca-scan.yml) — on PR, `dotnet restore`, then Snyk Dotnet action against `Contentstack.Core/obj/project.assets.json` (`SNYK_TOKEN` secret). Failures indicate vulnerable packages or scan misconfiguration.

**Policy (repository hygiene):** [`.github/workflows/policy-scan.yml`](../../../.github/workflows/policy-scan.yml) — for public repos, checks presence of `SECURITY.md` (or `.github/SECURITY.md`) and a license file. Adjust repo contents if these jobs fail.

When a PR fails these jobs, inspect the workflow log and fix dependencies or policy items as required by your team.

### Projects (layout)

| Path | Role |
|------|------|
| `Contentstack.Net.sln` | Main solution |
| `Contentstack.Core/` | Delivery SDK package |
| `Contentstack.AspNetCore/` | DI package |
| `Contentstack.Core.Unit.Tests/` | Unit tests |
| `Contentstack.Core.Tests/` | Integration tests |

## Related skills

- [SDK core patterns](../sdk-core-patterns/SKILL.md) — architecture and HTTP.
- [Testing](../testing/SKILL.md) — unit vs integration patterns and credentials.
