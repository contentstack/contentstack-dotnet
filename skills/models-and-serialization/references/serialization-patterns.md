# Serialization Patterns Reference

## CSJsonConverter Registration Flow

At `ContentstackClient` construction time:

```
1. Scan all assemblies in current AppDomain
2. Find all types with [CSJsonConverter("ConverterName")] attribute
3. Find the JsonConverter class with matching name in Contentstack.Core.Internals
4. Instantiate it and add to SerializerSettings.Converters
5. All subsequent Fetch<T>/Find<T> calls use these converters
```

This means **converter registration is automatic** — adding the attribute and converter class is all that's required.

## EntryJsonConverter Internals

`EntryJsonConverter` handles the nested entry JSON structure from the CDA:

```json
{
  "uid": "blt123",
  "title": "My Entry",
  "publish_details": { "environment": "production", "locale": "en-us", "time": "...", "user": "..." },
  "locale": "en-us",
  "_metadata": { ... },
  "custom_field": "value",
  "reference_field": [{ "uid": "blt456", "_content_type_uid": "blog" }]
}
```

The converter:
1. Reads the raw `JObject`
2. Maps known fields to strongly-typed properties (`Uid`, `Title`, `PublishDetails`, etc.)
3. Puts all remaining fields into `entry.Object` (the catch-all dictionary)

## AssetJsonConverter Internals

Similar pattern for assets — maps `uid`, `title`, `url`, `content_type`, `file_size`, `filename`, `tags` to typed properties; remaining fields to `asset.Object`.

## parseJObject\<T\> in Query

After `HttpRequestHandler.ProcessRequest` returns a JSON string, `Query.parseJObject<T>` does:

```csharp
JObject jObject = JObject.Parse(responseString);

// For entry queries
JArray entries = (JArray)jObject["entries"];
collection.Items = entries.ToObject<IEnumerable<T>>(client.Serializer);
collection.Count = jObject["count"]?.Value<int>() ?? 0;
collection.Skip  = (int)UrlQueries.GetValueOrDefault("skip", 0);
collection.Limit = (int)UrlQueries.GetValueOrDefault("limit", 100);
```

The `"entries"` token name is fixed by the CDA response format. Asset library uses `"assets"`.

## JsonProperty Mapping Reference

Key mappings already in the codebase:

| JSON field | C# property | Model |
|-----------|-------------|-------|
| `publish_details` | `PublishDetails` | `Entry` |
| `_metadata` | `Metadata` / `_metadata` | `Entry` |
| `content_type` | `ContentType` | `Asset` |
| `file_size` | `FileSize` | `Asset` |
| `filename` | `FileName` | `Asset` |
| `created_at` | `CreatedAt` | various |
| `updated_at` | `UpdatedAt` | various |
| `created_by` | `CreatedBy` | various |

## Newtonsoft.Json Settings Defaults

The SDK uses default `JsonSerializerSettings` with no special configuration unless consumers override `client.SerializerSettings`. This means:
- `NullValueHandling.Include` (nulls are included)
- No date format override (ISO 8601 by default)
- No contract resolver override (property names as-is, so `[JsonProperty]` is required)
- No type name handling

## Handling Embedded RTE Items

RTE (Rich Text Editor) fields with embedded entries/assets are processed via `contentstack.utils` NuGet package. The `Entry` model surfaces the raw RTE JSON; consumers call the utils library to resolve embedded references:

```csharp
// RTE field value from entry.Object["rte_field"]
// Pass to contentstack.utils for resolution
Utils.RenderContent(content, entryEmbeds);
```

## Deep Reference Deserialization

When `IncludeReference()` is called, the CDA returns nested objects inside the entry JSON. These are deserialized as nested dictionaries in `entry.Object` or as typed sub-objects when the consumer POCO uses `[JsonProperty]` on the reference field:

```csharp
public class BlogPost
{
    [JsonProperty("author")]
    public Author Author { get; set; }  // auto-deserialized if CDA returns expanded ref
}
```

If the reference is not expanded (not included), it will be an array of `{"uid": "...", "_content_type_uid": "..."}` objects.

## Modular Blocks Deserialization

Modular blocks are returned as JSON arrays of objects, each with a `_content_type_uid` discriminator:

```json
"modular_blocks": [
  { "block_a": { "field1": "value" } },
  { "block_b": { "field2": "value" } }
]
```

Map with a `List<Dictionary<string, object>>` or use a custom converter with a discriminator switch on the first key.

## ContentstackCollection Parsing Edge Cases

- `count` field only present when `IncludeCount()` is set on the query
- `entries` array is present even when empty (`[]`), never `null`
- `skip` and `limit` in the response may differ from what was requested if the CDA has its own limits

---

## Entry model shape (quick reference)

```csharp
// Strongly-typed fields (typical)
entry.Uid; entry.Title; entry.Tags; entry.Metadata; entry.PublishDetails;

// Catch-all for arbitrary content type fields
entry.Object  // Dictionary<string, object>
```

```csharp
var price = entry.Object["price"];
var color = entry.Object["color"] as string;
```

## Fetch and Find with typed POCOs

Prefer typed models over `entry.Object` for structured access:

```csharp
public class Product
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("uid")]
    public string Uid { get; set; }
}

var result = await client.ContentType("product").Query().Find<Product>();
var product = await client.ContentType("product").Entry("uid").Fetch<Product>();
```

Deserialization uses `client.Serializer` (from `client.SerializerSettings`).

## JsonProperty — always map snake_case

The CDA uses `snake_case`. Newtonsoft defaults to PascalCase property names. Always annotate:

```csharp
[JsonProperty("publish_details")]
public object PublishDetails { get; set; }
```

Without `[JsonProperty]`, deserialization looks for the wrong JSON keys.

## CSJsonConverter on models

```csharp
[CSJsonConverter("EntryJsonConverter")]
public class Entry { ... }
```

Converters live in `Contentstack.Core/Internals/`. See [CSJsonConverter Registration Flow](#csjsonconverter-registration-flow).

## ContentstackCollection shape

```csharp
public class ContentstackCollection<T>
{
    public IEnumerable<T> Items { get; }
    public int Count { get; }
    public int Skip { get; }
    public int Limit { get; }
}
```

Parsed from `"entries"` or `"assets"` in `Query.parseJObject<T>` (see [parseJObject\<T\> in Query](#parsejobjectt-in-query)).

## Asset model (common properties)

`Uid`, `Title`, `Url`, `ContentType`, `FileSize`, `FileName`, `Tags`, plus `Object` for other fields. `AssetJsonConverter` handles nested JSON; `AssetLibrary.FetchAll()` returns `ContentstackCollection<Asset>`.

## Entry variants

```csharp
entry.SetVariant("variant_uid");
var result = await entry.Fetch<MyModel>();
```

Uses `_variant` and the `x-cs-variant-uid` header path as implemented on `Entry`.

## Adding a new model type (checklist)

1. Add class under `Contentstack.Core/Models/` with `[JsonProperty]` on API fields and optional `Object` catch-all.
2. If custom deserialization is required, add `internal class MyModelJsonConverter : JsonConverter` in `Internals/`, implement `ReadJson` / `WriteJson` as needed (delivery is typically read-heavy).
3. Mark the model with `[CSJsonConverter("MyModelJsonConverter")]`.
4. Expose via `ContentstackClient` factory methods if it is a first-class API surface.

## SerializerSettings customization

Consumers may adjust before calls:

```csharp
client.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
client.SerializerSettings.DateFormatString = "yyyy-MM-dd";
```

`Serializer` is rebuilt from `SerializerSettings` when needed.
