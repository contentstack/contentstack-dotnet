# Testing Patterns Reference

## Complete Unit Test Template

```csharp
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Internals;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    public class MyFeatureUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public MyFeatureUnitTests()
        {
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>()
            };
            _client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        private Query CreateQuery(string contentTypeId = "source")
            => _client.ContentType(contentTypeId).Query();

        // Helper: get private QueryValueJson
        private Dictionary<string, object> GetQueryValueJson(Query query)
        {
            var field = typeof(Query).GetField("QueryValueJson",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            return (Dictionary<string, object>)field?.GetValue(query);
        }

        // Helper: get private UrlQueries
        private Dictionary<string, object> GetUrlQueries(Query query)
        {
            var field = typeof(Query).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (Dictionary<string, object>)field?.GetValue(query);
        }

        [Fact]
        public void MyOperator_AddsCorrectQueryParameter()
        {
            var query = CreateQuery();
            var key = _fixture.Create<string>();

            var result = query.MyOperator(key, "value");

            Assert.Equal(query, result); // fluent return
            var qvj = GetQueryValueJson(query);
            Assert.True(qvj.ContainsKey(key));
            var inner = qvj[key] as Dictionary<string, object>;
            Assert.True(inner.ContainsKey("$myop"));
            Assert.Equal("value", inner["$myop"]);
        }

        [Fact]
        public void MyUrlParam_AddsToUrlQueries()
        {
            var query = CreateQuery();

            query.SetLocale("en-us");

            var urlQueries = GetUrlQueries(query);
            Assert.Equal("en-us", urlQueries["locale"]);
        }

        [Fact]
        public void MyOperator_WithNullKey_ThrowsQueryFilterException()
        {
            var query = CreateQuery();
            Assert.Throws<QueryFilterException>(() => query.MyOperator(null, "value"));
        }

        [Fact]
        public void MyOperator_WithEmptyKey_ThrowsQueryFilterException()
        {
            var query = CreateQuery();
            Assert.Throws<QueryFilterException>(() => query.MyOperator(string.Empty, "value"));
        }

        [Fact]
        public void MyOperator_ReturnsQueryForChaining()
        {
            var query = CreateQuery();
            var result = query
                .MyOperator("field1", "value1")
                .MyOperator("field2", "value2");
            Assert.Equal(query, result);
        }
    }
}
```

## Complete Integration Test Template

```csharp
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.MyFeatureTests
{
    public class MyFeatureComprehensiveTest : IntegrationTestBase
    {
        public MyFeatureComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(DisplayName = "MyFeature - BasicOperation ReturnsExpectedResult")]
        public async Task MyFeature_BasicOperation_ReturnsExpectedResult()
        {
            // Arrange
            LogArrange("Setting up basic operation test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();

            // Act
            LogAct("Executing query with my feature");
            query.MyOperator("uid", "someValue");
            var result = await query.Find<Entry>();

            // Assert
            LogAssert("Verifying response structure");
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.True(result.Count >= 0, "Count should be non-negative");
        }

        [Fact(DisplayName = "MyFeature - WithInvalidParams ThrowsException")]
        public async Task MyFeature_WithInvalidParams_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up error scenario");
            var client = CreateClient();

            // Act & Assert
            LogAct("Executing with invalid parameters");
            await Assert.ThrowsAsync<QueryFilterException>(async () =>
            {
                await client.ContentType("nonexistent_type_12345")
                    .Query().Find<Entry>();
            });
        }
    }
}
```

## Existing Unit Test Files — What Each Covers

| File | Covers |
|------|--------|
| `QueryUnitTests.cs` | All `Query` filter/operator methods, UrlQueries params |
| `EntryUnitTests.cs` | `Entry` field access, URL construction, header setting |
| `AssetUnitTests.cs` | `Asset` model fields |
| `AssetLibraryUnitTests.cs` | `AssetLibrary` query params, Skip/Limit |
| `ContentstackClientUnitTests.cs` | Client initialization, header injection, factory methods |
| `ContentstackOptionsUnitTests.cs` | Options defaults, validation |
| `ContentstackExceptionUnitTests.cs` | Exception hierarchy, factory methods, message content |
| `ConfigUnitTests.cs` | BaseUrl composition, region codes |
| `ContentstackRegionUnitTests.cs` | Region enum → URL prefix mapping |
| `GlobalFieldUnitTests.cs` | GlobalField ID validation, URL construction |
| `GlobalFieldQueryUnitTests.cs` | GlobalFieldQuery filter methods |
| `TaxonomyUnitTests.cs` | Taxonomy query path |
| `JsonConverterUnitTests.cs` | CSJsonConverter attribute registration |
| `LivePreviewConfigUnitTests.cs` | LivePreviewConfig validation |

## Existing Integration Test Folders — What Each Covers

| Folder | Covers |
|--------|--------|
| `QueryTests/` | Query operators, complex filters, field queries, includes |
| `EntryTests/` | Entry fetch, field projection, references |
| `GlobalFieldsTests/` | Global field schemas, nested global fields |
| `SyncTests/` | Sync API, pagination tokens, delta sync |
| `AssetTests/` | Asset fetch, asset library queries |
| `ContentTypeTests/` | Content type fetch, schema queries |
| `LocalizationTests/` | Locale filtering, locale fallback chains |
| `PaginationTests/` | Skip/Limit behavior, count accuracy |
| `ErrorHandling/` | API error codes, exception types, invalid params |
| `LivePreview/` | Live preview URL routing, token headers |
| `ModularBlocksTests/` | Modular block field deserialization |
| `MetadataTests/` | Entry metadata fields |
| `TaxonomyTests/` | Taxonomy query path, taxonomy filtering |
| `VariantsTests/` | Entry variant headers, variant content |
| `BranchTests/` | Branch header injection |

## MockHttpHandler Pattern (Unit Tests)

When you need to mock HTTP responses without network:

```csharp
// In Mokes/MockHttpHandler.cs — extend for new mock scenarios
// MockResponse.cs — add JSON fixture strings for new response shapes
// MockInfrastructureTest.cs — base class wiring MockHttpHandler into client
```

## TestAssert Wrappers

Use `TestAssert.*` instead of raw `Assert.*` in integration tests — they log assertion context to `ITestOutputHelper`:

```csharp
TestAssert.NotNull(result);
TestAssert.Equal(expected, actual);
TestAssert.True(condition, "failure message");
TestAssert.False(condition, "failure message");
TestAssert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
TestAssert.Matches("^blt[a-zA-Z0-9]+$", entry.Uid);
```

## app.config Keys for Integration Tests

Integration tests read config from `Contentstack.Core.Tests/app.config`:

```xml
<appSettings>
  <add key="api_key" value="..." />
  <add key="delivery_token" value="..." />
  <add key="environment" value="..." />
  <add key="host" value="..." />
  <add key="branch_uid" value="..." />
  <add key="simple_content_type_uid" value="..." />
  <add key="medium_content_type_uid" value="..." />
  <add key="complex_content_type_uid" value="..." />
</appSettings>
```

Never commit real credentials. Use environment variables or a secrets manager in CI.

## Running Tests

```bash
# Unit tests only (no credentials needed)
dotnet test Contentstack.Core.Unit.Tests/

# Integration tests (requires app.config with valid credentials)
dotnet test Contentstack.Core.Tests/

# Run specific category
dotnet test --filter "Category=RetryIntegration"

# Run specific test class
dotnet test --filter "FullyQualifiedName~QueryOperatorsComprehensiveTest"
```

## Mokes folder (unit tests)

`Contentstack.Core.Unit.Tests/Mokes/`:

- `MockHttpHandler.cs` — intercepts HTTP without network
- `MockResponse.cs` — sample JSON response fixtures
- `MockInfrastructureTest.cs` — base for tests needing mock HTTP
- `Utilities.cs` — test utility helpers

## RequestLoggingPlugin (integration)

`CreateClient()` on `IntegrationTestBase` adds `RequestLoggingPlugin`, which logs HTTP requests and responses via `ITestOutputHelper`. No extra setup required.

Custom plugins for a test:

```csharp
var client = CreateClient();
client.Plugins.Add(new MyTestPlugin());
```

## Test coverage guidelines

- Unit test: every new public `Query` method (operator or URL param)
- Unit test: null/invalid input → expected exception type
- Integration test: happy path with real API response
- Integration test: verify response shape (`Items`, `Count`, fields)
- Place integration tests under the folder that matches the feature area

## Integration test file conventions

- Folders mirror features: `Integration/QueryTests/`, `Integration/EntryTests/`, `Integration/GlobalFieldsTests/`, etc.
- One test class per broad concern when it makes sense; file names often end in `Test.cs` / `Tests.cs`
- Use `LogArrange` / `LogAct` / `LogAssert` / `LogContext` from `IntegrationTestBase` (see templates above)

### xUnit traits (examples)

```csharp
[Trait("Category", "RetryIntegration")]
[Trait("Category", "LivePreview")]
[Trait("Category", "Sync")]
```

### DisplayName convention

```csharp
[Fact(DisplayName = "Query Operations - Regex Complex Pattern Matches Correctly")]
//  FeatureArea    - ComponentAction Outcome
```
