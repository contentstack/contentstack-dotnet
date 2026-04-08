---
name: code-review
description: SDK-specific PR review checklist for the Contentstack .NET SDK — covers breaking changes, HTTP layer correctness, exception handling, serialization, fluent API patterns, configuration, test coverage, multi-targeting, plugin lifecycle, and internal visibility. Use when reviewing pull requests, examining code changes, or performing code quality assessments on this SDK.
---

# Code Review

## When to use

- Reviewing a PR or diff against SDK conventions.
- Self-review before opening a PR.

## Severity levels

- **Critical** — Must fix before merge (correctness, breaking changes, security)
- **Important** — Should fix (maintainability, SDK patterns, consistency)
- **Suggestion** — Consider improving (style, optimization)

Category-by-category checklists (breaking changes, HTTP, exceptions, serialization, fluent API, config, tests, multi-targeting, plugins, visibility) live in the reference docs below—not duplicated here.

## Reference

| Document | Contents |
|----------|----------|
| [references/code-review-checklist.md](references/code-review-checklist.md) | Full markdown checklists per area |
| [references/red-flags.md](references/red-flags.md) | Short anti-pattern list for quick scan |
