---
name: query-building
description: Query building patterns for the Contentstack .NET SDK — two-dictionary architecture (QueryValueJson + UrlQueries), Mongo-style operators, fluent chaining, Exec() merge order, pagination (skip/limit), sync API pagination tokens, taxonomy query path. Use when adding query operators or filters, modifying Query.cs, working on pagination, debugging query parameter serialization, or building entry/asset queries.
---

# Query Building

## When to use

- Adding or changing operators on `Query`, `AssetLibrary`, or related fluent APIs.
- Debugging why a filter or URL param is wrong.
- Pagination, sync, or taxonomy entry queries.

## Invariants

- Field-level Mongo filters live in **`QueryValueJson`**; top-level API params (locale, skip, limit, includes, etc.) live in **`UrlQueries`**.
- **`Exec()`** merges `environment`, optional live-preview keys, then `query` (from `QueryValueJson`), then each `UrlQueries` entry—later keys override earlier ones.
- New filter methods return **`this`** and validate keys with **`QueryFilterException`** where appropriate.
- Never set **`environment`** manually in `UrlQueries`—`Exec()` injects it from `Config`.

## Exec() merge order (compact)

```csharp
mainJson["environment"] = stack.Config.Environment;
// live preview keys if enabled
if (QueryValueJson.Count > 0)
    mainJson["query"] = QueryValueJson;
foreach (var kvp in UrlQueries)
    mainJson[kvp.Key] = kvp.Value;
// → HttpRequestHandler.ProcessRequest(..., BodyJson)
```

## Reference

All exhaustive patterns, tables, and examples: **[references/query-patterns.md](references/query-patterns.md)** (quick-reference tables, extending `Query.cs`, terminal operations, pagination, sync, taxonomy URL paths, asset library, common mistakes).
