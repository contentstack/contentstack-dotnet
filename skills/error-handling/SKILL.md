---
name: error-handling
description: Error handling patterns for the Contentstack .NET SDK — ContentstackException hierarchy, domain-specific typed exceptions with static factory methods, GetContentstackError WebException parsing, centralized ErrorMessages.cs strings, and consumer catch order. Use when adding new exception types for a new domain area, modifying error messages, handling API HTTP errors, debugging WebException responses, or catching SDK errors in consumer code.
---

# Error Handling

## When to use

- New exceptions, new `ErrorMessages` strings, or HTTP error parsing.
- Reviewing catches in SDK internals or consumer apps.

## Invariants

- Throw **domain-specific** subclasses of `ContentstackException`, not raw `ContentstackException` for domain cases.
- **All user-facing message strings** live in **`ErrorMessages.cs`** — no inline strings in `throw`.
- On HTTP errors, use **`GetContentstackError`** and preserve **`ErrorCode`**, **`StatusCode`**, **`Errors`** when re-throwing.

## Exception hierarchy (summary)

`ContentstackException` → `QueryFilterException`, `AssetException`, `LivePreviewException`, `GlobalFieldException`, `EntryException`, `TaxonomyException`, `ContentTypeException` (see `Contentstack.Core/Internals/ContentstackExceptions.cs`).

Base properties: `ErrorCode`, `StatusCode`, `Errors`.

## Reference

Exhaustive detail: **[references/error-patterns.md](references/error-patterns.md)** — `ErrorMessages` catalogue, API error JSON shape, HTTP mapping, `GetContentstackError` locations, factories, catch patterns, Live Preview / global field triggers.
