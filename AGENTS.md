# Contentstack .NET SDK – Agent guide

**Universal entry point** for contributors and AI agents. Each skill is documented in **`skills/*/SKILL.md`** (YAML frontmatter for agent discovery where applicable).

## What this repo is

| Field | Detail |
|-------|--------|
| **Name:** | [contentstack-dotnet](https://github.com/contentstack/contentstack-dotnet) |
| **Purpose:** | .NET SDK for Contentstack’s Content Delivery API (CDA)—fetch entries, assets, and run queries from .NET apps. |
| **Out of scope (if any):** | Do not bypass the SDK HTTP layer with ad-hoc `HttpClient` usage; all requests go through `HttpRequestHandler` (see `skills/sdk-core-patterns/SKILL.md`). |

## Tech stack (at a glance)

| Area | Details |
|------|---------|
| Language | C#; multi-targeting includes `netstandard2.0`, `net47`, `net472` (see project files under `Contentstack.Core/`). |
| Build | .NET SDK — solution [`Contentstack.Net.sln`](Contentstack.Net.sln); packages [`Contentstack.Core/`](Contentstack.Core/) (Delivery SDK), [`Contentstack.AspNetCore/`](Contentstack.AspNetCore/) (DI extensions). |
| Tests | xUnit; unit tests in [`Contentstack.Core.Unit.Tests/`](Contentstack.Core.Unit.Tests/) (no credentials); integration tests in [`Contentstack.Core.Tests/`](Contentstack.Core.Tests/) (requires `app.config` / API credentials). |
| Lint / coverage | No dedicated repo-wide lint/format CLI in CI. Security/static analysis: [CodeQL workflow](.github/workflows/codeql-analysis.yml). |
| Other | JSON: Newtonsoft.Json; package version: single source in [`Directory.Build.props`](Directory.Build.props). |

## Commands (quick reference)

| Command type | Command |
|--------------|---------|
| Build | `dotnet build Contentstack.Net.sln` |
| Test (unit) | `dotnet test Contentstack.Core.Unit.Tests/Contentstack.Core.Unit.Tests.csproj` |
| Test (integration) | `dotnet test Contentstack.Core.Tests/Contentstack.Core.Tests.csproj` (configure credentials locally) |

CI: [`.github/workflows/unit-test.yml`](.github/workflows/unit-test.yml) restores, builds, and runs unit tests on Windows (.NET 7). Other workflows include [NuGet publish](.github/workflows/nuget-publish.yml), [branch checks](.github/workflows/check-branch.yml), [CodeQL](.github/workflows/codeql-analysis.yml), policy/SCA scans.

## Where the documentation lives: skills

| Skill | Path | What it covers |
|-------|------|----------------|
| Dev workflow | [`skills/dev-workflow/SKILL.md`](skills/dev-workflow/SKILL.md) | Solution layout, build/test commands, versioning, CI entry points. |
| SDK core patterns | [`skills/sdk-core-patterns/SKILL.md`](skills/sdk-core-patterns/SKILL.md) | Architecture, `ContentstackClient`, HTTP layer, DI, plugins. |
| Query building | [`skills/query-building/SKILL.md`](skills/query-building/SKILL.md) | Fluent query API, operators, pagination, sync, taxonomy. |
| Models and serialization | [`skills/models-and-serialization/SKILL.md`](skills/models-and-serialization/SKILL.md) | Entry/Asset models, JSON converters, collections. |
| Error handling | [`skills/error-handling/SKILL.md`](skills/error-handling/SKILL.md) | Exception hierarchy, `ErrorMessages`, API error parsing. |
| Testing | [`skills/testing/SKILL.md`](skills/testing/SKILL.md) | Unit vs integration tests, AutoFixture, `IntegrationTestBase`. |
| Code review | [`skills/code-review/SKILL.md`](skills/code-review/SKILL.md) | PR checklist for this SDK. |

An index with “when to use” hints is in [`skills/README.md`](skills/README.md).

## Using Cursor (optional)

If you use **Cursor**, [`.cursor/rules/README.md`](.cursor/rules/README.md) only points to **`AGENTS.md`**—the same conventions as for everyone else. Canonical guidance remains in **`skills/*/SKILL.md`**.
