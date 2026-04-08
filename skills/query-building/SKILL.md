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

## Query Patterns Reference

### Quick reference: QueryValueJson operators

| Method | Operator | Example JSON shape |
|--------|----------|-------------------|
| `Where(key, value)` | direct equality | `{"field": "value"}` |
| `NotEqualTo(key, value)` | `$ne` | `{"field":{"$ne":"x"}}` |
| `ContainedIn(key, values)` | `$in` | `{"field":{"$in":["a","b"]}}` |
| `NotContainedIn(key, values)` | `$nin` | `{"field":{"$nin":["a"]}}` |
| `Exists(key)` | `$exists: true` | `{"field":{"$exists":true}}` |
| `NotExists(key)` | `$exists: false` | `{"field":{"$exists":false}}` |
| `GreaterThan(key, value)` | `$gt` | `{"field":{"$gt":10}}` |
| `LessThan(key, value)` | `$lt` | `{"field":{"$lt":10}}` |
| `Regex(key, pattern)` | `$regex` | `{"field":{"$regex":"^blt"}}` |
| `And(queries[])` | `$and` | array of query objects |
| `Or(queries[])` | `$or` | array of query objects |

### Quick reference: UrlQueries keys

| Method | Key | Notes |
|--------|-----|-------|
| `SetLocale(locale)` | `locale` | prefer over obsolete `Language` enum |
| `Skip(n)` | `skip` | pagination offset |
| `Limit(n)` | `limit` | pagination page size |
| `IncludeSchema()` | `include_schema` | `true` |
| `IncludeCount()` | `include_count` | `true` |
| `Tags(tags[])` | `tags` | `string[]` → repeated param |
| `Only(fields[])` | `only[BASE][]` | field projection |
| `Except(fields[])` | `except[BASE][]` | field exclusion |
| `IncludeReference(key)` | `include[]` | reference expansion |

### Extending Query.cs (new fluent methods)

#### New Mongo-style filter operator

```csharp
// Pattern used by all existing operators in Query.cs
public Query MyNewOperator(string key, object value)
{
    if (key == null)
        throw QueryFilterException.Create(new ArgumentNullException(nameof(key)));

    if (!QueryValueJson.ContainsKey(key))
        QueryValueJson[key] = new Dictionary<string, object>();

    ((Dictionary<string, object>)QueryValueJson[key])["$myop"] = value;
    return this; // always return this for fluent chaining
}
```

#### New URL-level parameter

```csharp
public Query MyParam(string value)
{
    UrlQueries["my_param"] = value;
    return this;
}
```

### Terminal operations

```csharp
Task<ContentstackCollection<T>> result = await query.Find<T>();
Task<T> result = await query.FindOne<T>();
Task<int> count = await query.Count();
```

### Paginating Find results (entries)

```csharp
// ContentstackCollection<T> response shape
result.Items   // IEnumerable<T>
result.Count   // total count (requires IncludeCount())
result.Skip    // current offset
result.Limit   // current page size

// Paging loop
int skip = 0, limit = 100;
ContentstackCollection<Entry> page;
do {
    page = await query.Skip(skip).Limit(limit).Find<Entry>();
    // process page.Items
    skip += limit;
} while (page.Items.Count() == limit);
```

### SyncStack short reference

When using `SyncRecursive` / sync APIs:

```csharp
SyncStack syncResult = await client.SyncRecursive(parameters);
// SyncStack.PaginationToken — non-null while more pages exist
// SyncStack.SyncToken — final token for next delta sync
```

(See [Sync API Patterns](#sync-api-patterns) below for full sync flows.)

### Complete Operator Categories

#### Comparison Operators

```csharp
query.Where("title", "My Entry")          // direct equality
query.NotEqualTo("status", "draft")       // $ne
query.GreaterThan("price", 100)           // $gt
query.GreaterThanEqualTo("price", 100)    // $gte
query.LessThan("price", 200)             // $lt
query.LessThanEqualTo("price", 200)      // $lte
```

#### Array / Set Operators

```csharp
query.ContainedIn("color", new[] {"red", "blue"})    // $in
query.NotContainedIn("color", new[] {"green"})       // $nin
```

#### Existence Operators

```csharp
query.Exists("field_name")     // $exists: true
query.NotExists("field_name")  // $exists: false
```

#### String Operators

```csharp
query.Regex("uid", "^blt[a-zA-Z0-9]+$")           // $regex
query.Regex("title", "^hello", "i")               // $regex with $options modifier
```

#### Logical Operators

```csharp
// And / Or take Query[] — each sub-query builds its own QueryValueJson
var q1 = client.ContentType("ct").Query().Where("color", "red");
var q2 = client.ContentType("ct").Query().Where("size", "large");
query.And(new[] { q1, q2 });   // $and
query.Or(new[] { q1, q2 });    // $or
```

### Reference / Include Patterns

```csharp
query.IncludeReference("reference_field");           // expand single reference
query.IncludeReference(new[] {"ref1", "ref2"});      // expand multiple
query.IncludeSchema();                               // include content type schema
query.IncludeCount();                                // include total count in response
query.IncludeOwner();                                // include entry owner info
query.IncludeMetadata();                             // include entry metadata
```

### Field Projection

```csharp
query.Only(new[] {"title", "uid", "price"});         // return only these fields
query.Except(new[] {"body", "image"});               // exclude these fields

// For referenced fields
query.OnlyWithReferenceUid(new[] {"title"}, "reference_field");
query.ExceptWithReferenceUid(new[] {"body"}, "reference_field");
```

### Ordering

```csharp
query.OrderByAscending("title");
query.OrderByDescending("created_at");
```

### Locale and Environment

```csharp
query.SetLocale("en-us");        // preferred — string locale code
// Environment is injected automatically from Config in Exec()
// Never set "environment" manually in UrlQueries
```

### Exec() Implementation Detail

The full merge performed in `Query.Exec()` before calling `HttpRequestHandler`:

```csharp
var mainJson = new Dictionary<string, object>();

// 1. Environment (always injected from Config)
mainJson["environment"] = ContentTypeInstance.StackInstance.Config.Environment;

// 2. Live preview headers (if enabled and content type matches)
if (livePreviewActive)
{
    mainJson["live_preview"] = livePreviewConfig.LivePreview;
    mainJson["authorization"] = livePreviewConfig.ManagementToken;
    // or mainJson["preview_token"] = livePreviewConfig.PreviewToken;
}

// 3. Mongo-style query filter (only if non-empty)
if (QueryValueJson.Count > 0)
    mainJson["query"] = QueryValueJson;

// 4. All UrlQueries (locale, skip, limit, includes, projections, etc.)
foreach (var kvp in UrlQueries)
    mainJson[kvp.Key] = kvp.Value;
```

### How QueryValueJson Is Serialized

`Dictionary<string, object>` values are JSON-serialized by `HttpRequestHandler`:

```
QueryValueJson = { "title": {"$ne": "Draft"}, "color": {"$in": ["red","blue"]} }
→ query={"title":{"$ne":"Draft"},"color":{"$in":["red","blue"]}}
```

This becomes a single `query=` URL parameter with the JSON as its value.

### Sync API Patterns

```csharp
// Initial sync (all published content)
SyncStack result = await client.Sync(new SyncStack() { Type = SyncType.entry_published });

// Paginated initial sync
SyncStack result = await client.SyncRecursive(parameters);
// result.PaginationToken — continue paginating if not null
// result.SyncToken — use for next delta sync

// Delta sync (changes since last sync)
SyncStack delta = await client.SyncToken(result.SyncToken);

// Manual pagination loop
SyncStack page = initialResult;
while (page.PaginationToken != null)
{
    page = await client.SyncPaginationToken(page.PaginationToken);
    // process page.Items
}
```

### Taxonomy Query Patterns

```csharp
// Create taxonomy query via client.Taxonomies()
Query taxonomyQuery = client.Taxonomies();

// Same filter methods apply
taxonomyQuery.Where("taxonomies.animals", new Dictionary<string, object> {
    { "$eq", "mammals" }
});

var results = await taxonomyQuery.Find<Entry>();
```

**URL paths**

- Normal content-type query: `{stack.Config.BaseUrl}/content_types/{uid}/entries`
- Taxonomy query (`client.Taxonomies()`): `{stack.Config.BaseUrl}/taxonomies/entries`

Filter and pagination APIs are the same; only the base path differs.

### AssetLibrary Query Patterns

```csharp
AssetLibrary assetLib = client.Assets();
assetLib.Skip(0).Limit(100);
assetLib.IncludeCount();
assetLib.SetLocale("en-us");
ContentstackCollection<Asset> assets = await assetLib.FetchAll();
```

### Common Mistakes to Avoid

- Never set `"environment"` key manually in `UrlQueries` — it is always injected from `Config.Environment` in `Exec()`
- Never call `ProcessRequest` directly — always go through model methods (`Find`, `Fetch`, etc.)
- Never modify `QueryValueJson` from outside `Query` — use the public fluent methods
- `And()` / `Or()` take full `Query` instances, not raw dictionaries
- `string[]` values in `UrlQueries` become repeated URL params, not JSON arrays — use for tags, not Mongo operators
