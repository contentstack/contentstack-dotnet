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

`ContentstackOptions` → internal `Config` → `BaseUrl` (region + host + version). Required: `ApiKey`, `DeliveryToken`, `Environment`. See **[SDK Architecture](references/sdk-architecture.md)** for region table, live preview host switching, and internal client state.

## Reference map

| Topic | Document |
|-------|----------|
| Full HTTP flow, Config/BaseUrl, live preview, query serialization, plugins, regions, NuGet layout | [references/sdk-architecture.md](references/sdk-architecture.md) |
| `AddContentstack`, `ContentstackOptions` section, transient registration | [references/aspnetcore-integration.md](references/aspnetcore-integration.md) |
