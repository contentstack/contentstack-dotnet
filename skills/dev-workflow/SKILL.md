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

## Reference

| Document | Contents |
|----------|----------|
| [references/repo-tooling.md](references/repo-tooling.md) | Commands, CI, branch policy, CodeQL, NuGet release workflow, DocFX, SCA/policy scans, project layout |

## Related skills

- [SDK core patterns](../sdk-core-patterns/SKILL.md) — architecture and HTTP.
- [Testing](../testing/SKILL.md) — unit vs integration patterns and credentials.
