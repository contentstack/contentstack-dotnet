---
name: sdk-core-patterns
description: Core architecture of the Contentstack .NET Delivery SDK — namespaces, ContentstackClient factory, Config/ContentstackOptions, HttpRequestHandler (HttpWebRequest GET), plugin hooks, ASP.NET Core DI, and multi-targeting. Use when working on new SDK features, adding model types, wiring DI, understanding request flow, or onboarding to the codebase.
---

# SDK Core Patterns

## When to use

- Onboarding, request flow, or where types live.
- Changing HTTP behavior, config, plugins, or DI.
- Anything that must go through `HttpRequestHandler`.

## Namespace map

| Namespace | Purpose |
|-----------|---------|
| `Contentstack.Core` | `ContentstackClient` — root entry point |
| `Contentstack.Core.Models` | `Query`, `Entry`, `Asset`, `AssetLibrary`, `ContentType`, `GlobalField`, `GlobalFieldQuery`, `Taxonomy`, `SyncStack`, `ContentstackCollection<T>` |
| `Contentstack.Core.Configuration` | `ContentstackOptions`, `Config` (internal), `LivePreviewConfig` |
| `Contentstack.Core.Internals` | `HttpRequestHandler`, exceptions, enums, converters, constants — all `internal` |
| `Contentstack.Core.Interfaces` | `IContentstackPlugin` |
| `Contentstack.AspNetCore` | `IServiceCollectionExtensions` for DI registration |

## Invariants

- **`ContentstackClient`** is the only public entry point for creating stack operations.
- **All HTTP** goes through **`HttpRequestHandler.ProcessRequest`** — no `HttpClient`, no bypassing for feature work.
- **GET + query string** for requests; plugins run `OnRequest` / `OnResponse` around the call.
- **Multi-target:** `netstandard2.0`, `net47`, `net472` — no `System.Text.Json`; Newtonsoft.Json throughout.
- **Version:** `Directory.Build.props` only — do not set `<Version>` in individual `.csproj` files.

## ContentstackClient (compact)

```csharp
var client = new ContentstackClient(options);

client.ContentType("uid").Query().Find<T>();
client.ContentType("uid").Entry("uid");
client.Assets(); client.Asset("uid");
client.GlobalField("uid"); client.GlobalFields();
client.Taxonomies(); client.Sync(...);
```

## Options → Config → BaseUrl

`ContentstackOptions` → internal `Config` → `BaseUrl` (region + host + version). Required: `ApiKey`, `DeliveryToken`, `Environment`. Region table, live preview host switching, and internal client state are in **SDK architecture reference** below. ASP.NET Core registration is covered under **ASP.NET Core integration** below.


## SDK Architecture Reference

### Full Request Flow

```
ContentstackClient
  └── ContentType("uid") → ContentType
        └── Query() → Query
              └── Find<T>() → Query.Exec()
                    └── HttpRequestHandler.ProcessRequest(url, headers, bodyJson)
                          ├── Serialize BodyJson → query string
                          ├── Create HttpWebRequest (GET)
                          ├── Set headers (api_key, access_token, branch, x-user-agent)
                          ├── foreach plugin: OnRequest(client, request)
                          ├── await request.GetResponseAsync()
                          ├── foreach plugin: OnResponse(client, request, response, body)
                          └── return JSON string → parsed in Query.parseJObject<T>
```

### Config.BaseUrl Composition

```
Protocol    Region Code    Host                 Version
"https://"  ""             "cdn.contentstack.io" "/v3"   → US (default)
"https://"  "eu-"          "cdn.contentstack.com" "/v3"   → EU
"https://"  "azure-na-"    "cdn.contentstack.com" "/v3"   → AZURE_NA
"https://"  "azure-eu-"    "cdn.contentstack.com" "/v3"   → AZURE_EU
"https://"  "gcp-na-"      "cdn.contentstack.com" "/v3"   → GCP_NA
"https://"  "au-"          "cdn.contentstack.com" "/v3"   → AU
```

`HostURL` defaults to `cdn.contentstack.io` for US, `cdn.contentstack.com` for all other regions.

### Live Preview URL Resolution

When `LivePreviewConfig.Enable == true` and `LivePreview != "init"` and `ContentTypeUID` matches the queried content type, `Config.getBaseUrl()` returns the live preview host instead of `BaseUrl`:

```
"https://{livePreviewConfig.Host}/{version}"
```

Additional headers injected: `live_preview`, `authorization` (management token) or `preview_token`, optional `release_id`, `preview_timestamp`.

### Query String Serialization Rules (HttpRequestHandler)

| Value type | Serialization |
|-----------|--------------|
| `string` | `key=value` |
| `string[]` | `key=v1&key=v2` (repeated) |
| `Dictionary<string,object>` | `key={"$in":["a","b"]}` (JSON) |
| Other | `key=value.ToString()` |

### ContentstackClient Internal State

```csharp
internal string StackApiKey                          // from options
internal Dictionary<string,object> _Headers          // api_key, access_token/delivery_token
internal Dictionary<string,object> _StackHeaders     // shared across requests
internal LivePreviewConfig LivePreviewConfig          // null if not configured
public List<IContentstackPlugin> Plugins             // empty by default
public JsonSerializerSettings SerializerSettings     // for Fetch<T>/Find<T>
internal JsonSerializer Serializer                   // created from SerializerSettings
```

### How Models Get Stack Context

All model constructors are `internal`. `ContentstackClient` methods set back-references:

```csharp
// ContentType.cs internal wiring
internal ContentstackClient StackInstance { get; set; }

// Query.cs
private ContentType ContentTypeInstance { get; set; }  // for entry path
private ContentstackClient TaxonomyInstance { get; set; } // for taxonomy path
```

Models build their URL from `ContentTypeInstance.StackInstance.Config.BaseUrl` at call time (lazy).

### Plugin Implementation Pattern

```csharp
public class MyPlugin : IContentstackPlugin
{
    public Task<HttpWebRequest> OnRequest(ContentstackClient stack, HttpWebRequest request)
    {
        // Mutate request (add headers, log, etc.)
        return Task.FromResult(request);
    }

    public Task<string> OnResponse(ContentstackClient stack, HttpWebRequest request,
        HttpWebResponse response, string responseString)
    {
        // Inspect/transform response body string
        return Task.FromResult(responseString);
    }
}

// Register
client.Plugins.Add(new MyPlugin());
```

### ContentstackRegion Enum Values

```csharp
public enum ContentstackRegion { US, EU, AZURE_NA, AZURE_EU, GCP_NA, AU }
```

`ContentstackRegionCode` (internal enum) maps to URL prefixes: `eu`, `azure_na`, `azure_eu`, `gcp_na`, `au`. Underscores are replaced with hyphens in the URL.

### Key NuGet Dependencies (Contentstack.Core.csproj)

| Package | Version | Purpose |
|---------|---------|---------|
| `Newtonsoft.Json` | 13.0.4 | All JSON serialization |
| `Microsoft.Extensions.Options` | 8.0.2 | `IOptions<ContentstackOptions>` |
| `Markdig` | 0.36.2 | Markdown processing in RTE fields |
| `contentstack.utils` | 1.0.6 | RTE embedded item resolution |

### Solution Layout

```
Contentstack.Net.sln
├── Contentstack.Core/           ← Main SDK package (contentstack.csharp on NuGet)
├── Contentstack.AspNetCore/     ← DI extension (contentstack.aspnetcore on NuGet)
├── Contentstack.Core.Tests/     ← Integration tests (net7.0, hits live API)
└── Contentstack.Core.Unit.Tests/ ← Unit tests (no network)
```

Version shared via `Directory.Build.props` → `<Version>2.26.0</Version>` (or current).

### Supporting internals (maintainers)

When debugging HTTP, serialization edges, or multi-target behavior, these types in `Contentstack.Core/Internals/` are often involved. They are not public API.

| Area | Types / files |
|------|----------------|
| Async helpers around `HttpWebRequest` | `WebRequestAsyncExtensions.cs` |
| Version string / user-agent composition | `VersionUtility.cs`, `StackOutput.cs` |
| JSON / value coercion helpers | `ContentstackConvert.cs` |
| Language / locale enums | `LanguageEnums.cs` |

Prefer changing behavior through `HttpRequestHandler`, `Config`, and public models rather than exposing these types.

## ASP.NET Core integration

Source: [`Contentstack.AspNetCore/IServiceCollectionExtensions.cs`](../../../Contentstack.AspNetCore/IServiceCollectionExtensions.cs).

### Registration

Two overloads register the same services:

```csharp
public static IServiceCollection AddContentstack(this IServiceCollection services, IConfigurationRoot configuration)
public static IServiceCollection AddContentstack(this IServiceCollection services, IConfiguration configuration)
```

Both:

1. `services.AddOptions()`
2. `services.Configure<ContentstackOptions>(configuration.GetSection("ContentstackOptions"))`
3. `services.TryAddTransient<ContentstackClient>()`

### Configuration section

Bind options from configuration using section name **`ContentstackOptions`**:

```json
{
  "ContentstackOptions": {
    "ApiKey": "...",
    "DeliveryToken": "...",
    "Environment": "..."
  }
}
```

Adjust property names to match [`ContentstackOptions`](../../../Contentstack.Core/Configuration/ContentstackOptions.cs) public properties.

### Service lifetime

`ContentstackClient` is registered as **transient** (`TryAddTransient`). Each resolution gets a new instance; use this when injecting into short-lived scopes or when the app expects a fresh client per operation.

### Usage in app code

Inject `ContentstackClient` or `IOptions<ContentstackOptions>` as needed after calling `AddContentstack` in `Program.cs` / `Startup.cs`:

```csharp
services.AddContentstack(configuration);
```

Ensure `configuration` includes the `ContentstackOptions` section (e.g. `appsettings.json`, environment variables, user secrets).
