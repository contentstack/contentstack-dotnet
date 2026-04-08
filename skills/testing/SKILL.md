---
name: testing
description: Testing patterns for the Contentstack .NET SDK — unit tests using AutoFixture and private field reflection (no network), and integration tests using IntegrationTestBase, TestDataHelper, LogArrange/LogAct/LogAssert, xUnit traits, and RequestLoggingPlugin. Use when writing new tests, adding coverage for a new feature, debugging integration test failures, or understanding the test structure.
---

# Testing

## When to use

- New unit tests for `Query` or models (reflection on `QueryValueJson` / `UrlQueries`).
- New or failing integration tests against the live API.

## Projects

| Project | Framework | Purpose |
|---------|-----------|---------|
| `Contentstack.Core.Unit.Tests` | xUnit + AutoFixture | No network; assert internal state via reflection |
| `Contentstack.Core.Tests` | xUnit, net7.0 | Live API; requires `app.config` (or equivalent) credentials |

## Invariants

- Unit tests: **no real network** — use AutoFixture for options, reflection for private dictionaries.
- Integration tests: **never commit secrets** — credentials from `app.config` / env locally.
- New `Query` methods: unit test operators + null/invalid cases; integration test when behavior is API-bound.

## Reference

Templates, reflection helpers, `IntegrationTestBase`, `TestDataHelper`, mocks, traits, `app.config` keys, dotnet CLI filters, RequestLoggingPlugin, coverage guidelines: **[references/testing-patterns.md](references/testing-patterns.md)**.
