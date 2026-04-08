# Error Patterns Reference

## Complete ErrorMessages Catalogue

All strings in `Contentstack.Core/Internals/ErrorMessages.cs`:

### Query and Filter
```csharp
QueryFilterError        = "Please provide valid params."
InvalidParamsError      = "Invalid parameters provided. {0}"
```

### Asset
```csharp
AssetJsonConversionError = "Failed to convert asset JSON. Please check the asset format and data integrity."
AssetProcessingError     = "An error occurred while processing the asset. {0}"
AssetLibraryRequestError = "Exception in {0}: {1}\nStackTrace: {2}"
```

### Entry
```csharp
EntryProcessingError = "An error occurred while processing the entry. {0}"
EntryUidRequired     = "Please set entry uid."
EntryNotFoundInCache = "Entry is not present in cache"
```

### Global Field
```csharp
GlobalFieldIdNullError    = "GlobalFieldId required. This value cannot be null or empty, define it in your configuration."
GlobalFieldProcessingError = "An error occurred while processing the globalField. {0}"
GlobalFieldQueryError     = "Global field query failed. Check your query syntax and field schema before retrying."
```

### Live Preview
```csharp
LivePreviewTokenMissing = "Live Preview token missing. Add either a PreviewToken or a ManagementToken in the LivePreviewConfig."
```

### Client Request
```csharp
ContentstackClientRequestError = "Contentstack client request failed. Check your network settings or request parameters and try again: {0}"
ContentstackSyncRequestError   = "An error occurred while processing the Contentstack client request: {0}"
```

### Taxonomy
```csharp
TaxonomyProcessingError = "An error occurred while processing the taxonomy operation: {0}"
```

### Content Type
```csharp
ContentTypeProcessingError = "Content type processing failed. Verify the schema and ensure all required fields are configured."
```

### Authentication and Configuration
```csharp
StackApiKeyRequired       = "Stack api key can not be null."
AccessTokenRequired       = "Access token can not be null."
EnvironmentRequired       = "Environment can not be null."
AuthenticationNotPresent  = "Authentication Not present."
ContentTypeNameRequired   = "Please set contentType name."
```

### JSON and Parsing
```csharp
InvalidJsonFormat = "Please provide valid JSON."
ParsingError      = "Parsing Error."
```

### Network and Server
```csharp
NoConnectionError    = "Connection error"
ServerError          = "Server interaction went wrong, Please try again."
NetworkUnavailable   = "Network not available."
DefaultError         = "Oops! Something went wrong. Please try again."
```

### Cache
```csharp
SavingNetworkCallResponseForCache = "Error while saving network call response."
```

### Initialization
```csharp
ContentstackDefaultMethodNotCalled = "You must called Contentstack.stack() first"
```

## CDA API Error Response Format

The Contentstack CDA returns errors in this JSON format:

```json
{
  "error_message": "The requested entry doesn't exist.",
  "error_code": 141,
  "errors": {
    "field_name": ["validation message"]
  }
}
```

`error_code` is Contentstack-specific (not HTTP status). Common codes:
- `141` — Entry not found
- `141` — Asset not found  
- `109` — API key invalid
- `103` — Access token invalid
- `129` — Invalid query parameters

## HTTP Status to Exception Mapping

| HTTP Status | Typical cause | SDK behavior |
|------------|--------------|-------------|
| 400 | Invalid query params | `QueryFilterException` |
| 401 | Invalid API key / token | `ContentstackException` with StatusCode 401 |
| 404 | Entry / asset not found | `ContentstackException` with StatusCode 404 |
| 422 | Invalid field value | `ContentstackException` with `Errors` dict populated |
| 429 | Rate limit exceeded | `ContentstackException` with StatusCode 429 |
| 500 | Server error | `ContentstackException` with StatusCode 500 |

## Where GetContentstackError Is Called

The same static `GetContentstackError` method is replicated (acknowledged code smell) in:
- `Query` — wraps in `QueryFilterException`
- `Entry` — wraps in `EntryException`  
- `ContentType` — wraps in `ContentTypeException`
- `Asset` — wraps in `AssetException`
- `AssetLibrary` — wraps in `AssetException`

When adding a new model, follow the same pattern — copy `GetContentstackError` into the new class (or call the one from `Query` if same namespace/access level permits).

## Exception with Errors Dictionary

When the API returns field-level validation errors:

```csharp
catch (ContentstackException ex)
{
    if (ex.Errors != null)
    {
        foreach (var field in ex.Errors)
        {
            // field.Key = field name, field.Value = error messages
            Console.WriteLine($"Field '{field.Key}': {field.Value}");
        }
    }
}
```

## LivePreviewException Trigger Conditions

Thrown when `LivePreviewConfig.Enable = true` but neither `ManagementToken` nor `PreviewToken` is configured:

```csharp
if (livePreviewConfig.Enable)
{
    if (string.IsNullOrEmpty(livePreviewConfig.ManagementToken)
        && string.IsNullOrEmpty(livePreviewConfig.PreviewToken))
        throw new LivePreviewException(); // uses LivePreviewTokenMissing message
}
```

## GlobalFieldException.CreateForIdNull Trigger

Thrown when `GlobalField(null)` or `GlobalField("")` is called — validated before any HTTP request:

```csharp
public GlobalField GlobalField(string uid)
{
    if (string.IsNullOrEmpty(uid))
        throw GlobalFieldException.CreateForIdNull();
    // ...
}
```

## Adding Messages for New Features — Checklist

1. Add `public const string` to the appropriate section in `ErrorMessages.cs`
2. Use `{0}` placeholder for `string.Format` when appending exception details
3. Add the factory method on the exception class using `ErrorMessages.FormatExceptionDetails(ex)` for processing errors
4. Never concatenate exception details manually — always use `FormatExceptionDetails()`

---

## Static factory pattern (examples)

Use factories — not `new XyzException("literal")`:

```csharp
throw QueryFilterException.Create(innerException);
throw GlobalFieldException.CreateForIdNull();
throw AssetException.CreateForJsonConversionError();
throw AssetException.CreateForProcessingError(innerException);
throw EntryException.CreateForProcessingError(innerException);
throw TaxonomyException.CreateForProcessingError(innerException);
throw ContentTypeException.CreateForProcessingError(innerException);
```

## Adding a new domain exception

1. Add the class in `ContentstackExceptions.cs` extending `ContentstackException` with static factories.
2. Add `public const string` entries in `ErrorMessages.cs`; use `{0}` when wrapping `FormatExceptionDetails(innerException)`.

Example skeleton:

```csharp
public class MyFeatureException : ContentstackException
{
    public MyFeatureException(string message) : base(message) { }
    public MyFeatureException(string message, Exception innerException) : base(message, innerException) { }

    public static MyFeatureException CreateForProcessingError(Exception innerException)
    {
        return new MyFeatureException(
            string.Format(ErrorMessages.MyFeatureProcessingError,
                ErrorMessages.FormatExceptionDetails(innerException)),
            innerException);
    }
}
```

## GetContentstackError (implementation sketch)

`WebException` responses are parsed into `ContentstackException` with `ErrorCode`, `StatusCode`, and `Errors`. The same helper pattern is duplicated on `Query`, `Entry`, `ContentType`, `Asset`, `AssetLibrary` — follow the existing copy when adding a new HTTP-calling model.

```csharp
internal static ContentstackException GetContentstackError(Exception ex)
{
    var webEx = (WebException)ex;
    using var stream = webEx.Response.GetResponseStream();
    string errorMessage = new StreamReader(stream).ReadToEnd();
    JObject data = JObject.Parse(errorMessage);
    int errorCode = data["error_code"]?.Value<int>() ?? 0;
    HttpStatusCode statusCode = ((HttpWebResponse)webEx.Response).StatusCode;
    var errors = data["errors"]?.ToObject<Dictionary<string, object>>();
    return new ContentstackException(data["error_message"]?.Value<string>())
    {
        ErrorCode = errorCode,
        StatusCode = statusCode,
        Errors = errors
    };
}
```

## Standard catch block (SDK internals)

Preserve `ErrorCode`, `StatusCode`, and `Errors` when re-throwing domain exceptions:

```csharp
catch (Exception ex)
{
    ContentstackException error = GetContentstackError(ex);
    throw new QueryFilterException(error.Message, ex)
    {
        ErrorCode = error.ErrorCode,
        StatusCode = error.StatusCode,
        Errors = error.Errors
    };
}
```

## Consumer catch order (example)

```csharp
try { /* await query.Find<Product>() */ }
catch (QueryFilterException ex) { /* query validation */ }
catch (ContentstackException ex) { /* API / HTTP */ }
catch (Exception ex) { /* network / unknown */ }
```

## ErrorMessages.FormatExceptionDetails

```csharp
ErrorMessages.FormatExceptionDetails(innerException)
```
