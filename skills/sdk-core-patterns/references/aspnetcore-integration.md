# ASP.NET Core integration

Source: [`Contentstack.AspNetCore/IServiceCollectionExtensions.cs`](../../../Contentstack.AspNetCore/IServiceCollectionExtensions.cs).

## Registration

Two overloads register the same services:

```csharp
public static IServiceCollection AddContentstack(this IServiceCollection services, IConfigurationRoot configuration)
public static IServiceCollection AddContentstack(this IServiceCollection services, IConfiguration configuration)
```

Both:

1. `services.AddOptions()`
2. `services.Configure<ContentstackOptions>(configuration.GetSection("ContentstackOptions"))`
3. `services.TryAddTransient<ContentstackClient>()`

## Configuration section

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

## Service lifetime

`ContentstackClient` is registered as **transient** (`TryAddTransient`). Each resolution gets a new instance; use this when injecting into short-lived scopes or when the app expects a fresh client per operation.

## Usage in app code

Inject `ContentstackClient` or `IOptions<ContentstackOptions>` as needed after calling `AddContentstack` in `Program.cs` / `Startup.cs`:

```csharp
services.AddContentstack(configuration);
```

Ensure `configuration` includes the `ContentstackOptions` section (e.g. `appsettings.json`, environment variables, user secrets).
