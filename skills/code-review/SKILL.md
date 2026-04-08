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

The sections below provide category-by-category checklists (breaking changes, HTTP, exceptions, serialization, fluent API, config, tests, multi-targeting, plugins, visibility) and a short red-flag list for quick scanning.


## Code Review Checklist

### Breaking Changes Checklist

```markdown
## Breaking Changes Review
- [ ] No public method signatures removed or changed without [Obsolete] deprecation
- [ ] No [JsonProperty] values changed (would break consumer deserialization silently)
- [ ] No ContentstackOptions public property removed
- [ ] New required options have defaults (don't break existing consumers who don't set them)
- [ ] No namespace renames without backward-compatible type aliases
- [ ] No IContentstackPlugin interface signature changed
- [ ] Version bump planned if breaking change is intentional (Directory.Build.props)
```

### HTTP Layer Checklist

```markdown
## HTTP Layer Review
- [ ] All HTTP calls route through HttpRequestHandler.ProcessRequest
- [ ] No HttpClient instantiation anywhere in the PR
- [ ] New query params added to UrlQueries dict (not directly to URL string)
- [ ] New field-level filters added to QueryValueJson dict
- [ ] New headers added via Headers parameter to ProcessRequest
- [ ] Branch header uses "branch" key, passed as separate Branch parameter
- [ ] No hardcoded URLs — BaseUrl comes from Config.BaseUrl
- [ ] Live preview URL resolved via Config.getBaseUrl() — not hardcoded
- [ ] ProcessRequest result (string JSON) parsed, not further HTTP calls made
```

### Exception Handling Checklist

```markdown
## Exception Handling Review
- [ ] Domain-specific exception type used (QueryFilterException, AssetException, etc.)
- [ ] No bare `throw new Exception(...)` or `throw new ContentstackException(...)`
- [ ] All message strings sourced from ErrorMessages.cs constants
- [ ] No string literals in throw statements
- [ ] GetContentstackError(ex) called when catching WebException from HTTP calls
- [ ] ErrorCode, StatusCode, Errors preserved when re-wrapping exceptions
- [ ] New domain area has new exception class with factory methods
- [ ] New error messages added to correct section in ErrorMessages.cs
- [ ] FormatExceptionDetails(innerEx) used in ProcessingError factory methods
```

### Serialization Checklist

```markdown
## Serialization Review
- [ ] All public properties mapping CDA JSON fields have [JsonProperty("snake_case")]
- [ ] No reliance on default Newtonsoft.Json camelCase or PascalCase matching
- [ ] Custom deserialization uses [CSJsonConverter] + JsonConverter subclass
- [ ] JsonConverter placed in Contentstack.Core/Internals/ (internal class)
- [ ] No System.Text.Json usage
- [ ] No JsonConvert.DeserializeObject with hardcoded type outside of converter
- [ ] ContentstackCollection<T> used for list responses (not List<T> directly)
- [ ] "entries" token used for entry collection, "assets" for asset collection
```

### Fluent API Checklist

```markdown
## Fluent API Review
- [ ] Every Query filter/operator method returns `return this;`
- [ ] Null key validated at start of method → QueryFilterException.Create()
- [ ] Empty string key validated → QueryFilterException.Create()
- [ ] Operator value stored in QueryValueJson[key][$operator] nested dict
- [ ] URL-level params stored in UrlQueries[key]
- [ ] Method name follows verb+noun pattern (GreaterThan, ContainedIn, NotExists)
- [ ] No mutation of QueryValueJson or UrlQueries outside of the Query class itself
- [ ] And()/Or() accept Query[] (not raw dictionaries)
```

### Configuration Checklist

```markdown
## Configuration Review
- [ ] New options added to ContentstackOptions (public class), not Config (internal)
- [ ] New property has XML <summary> doc comment
- [ ] Default value set in ContentstackOptions() constructor or property initializer
- [ ] ContentstackClient constructor reads new option and passes to Config
- [ ] Config never exposed as public property
- [ ] New option tested in ContentstackOptionsUnitTests.cs
- [ ] ASP.NET Core binding works (IOptions<ContentstackOptions> path verified)
```

### Test Coverage Checklist

```markdown
## Test Coverage Review
- [ ] Unit test for each new public Query method (QueryValueJson assertion via reflection)
- [ ] Unit test for null key input → QueryFilterException
- [ ] Unit test for empty key input → QueryFilterException
- [ ] Unit test for fluent return (Assert.Equal(query, result))
- [ ] Integration test file in Integration/{FeatureName}Tests/ subfolder
- [ ] Integration test class extends IntegrationTestBase
- [ ] Integration test constructor takes ITestOutputHelper output
- [ ] CreateClient() used (not manual ContentstackClient construction)
- [ ] LogArrange/LogAct/LogAssert used in correct order
- [ ] TestAssert.* used (not raw Assert.*)
- [ ] [Fact(DisplayName = "FeatureArea - Component Description")] present
- [ ] Happy path test (valid params → expected response)
- [ ] Error path test (invalid params or not found → expected exception)
```

### Multi-Targeting Checklist

```markdown
## Multi-Targeting Review
- [ ] No HttpClient (netstandard2.0 HttpClient has behavioural differences from net4x)
- [ ] No System.Text.Json (not available without separate package in netstandard2.0)
- [ ] No record types (C# 9, requires LangVersion setting for net47/net472)
- [ ] No default interface implementations (C# 8, may affect net47)
- [ ] No nullable reference types without #nullable enable guard
- [ ] No top-level statements (not applicable to library projects but worth checking)
- [ ] Tested compile against netstandard2.0 target (or verified via CI)
```

### Plugin Lifecycle Checklist

```markdown
## Plugin Lifecycle Review
- [ ] New feature that makes HTTP calls uses HttpRequestHandler (plugins run automatically)
- [ ] No WebRequest.Create() called directly in new model classes
- [ ] IContentstackPlugin interface not modified (breaking for all plugin consumers)
- [ ] RequestLoggingPlugin still works with any new request/response changes
- [ ] Plugin.OnRequest receives HttpWebRequest before send
- [ ] Plugin.OnResponse receives response string (can mutate/inspect)
```

### Internal Visibility Checklist

```markdown
## Internal Visibility Review
- [ ] New utility/helper classes in Internals/ are marked `internal`
- [ ] New model types intended for consumers are in Models/ and `public`
- [ ] New configuration types are in Configuration/ and `public`
- [ ] No public exposure of Config, HttpRequestHandler, or VersionUtility
- [ ] InternalsVisibleTo not modified (already covers both test projects)
- [ ] New internal methods accessible in unit tests without changes
```

### Common Issues Found in Past PRs

#### Silent Deserialization Failures
`[JsonProperty]` omitted → field is always null at runtime, no exception. Verify all properties that map CDA JSON fields.

#### Exception Message in Throw
```csharp
// Bad
throw new QueryFilterException("Please provide valid params.");

// Good
throw QueryFilterException.Create(innerEx);
// or
throw new QueryFilterException(ErrorMessages.QueryFilterError);
```

#### Hardcoded Environment
```csharp
// Bad — breaks for consumers with different environments
mainJson["environment"] = "production";

// Correct — already done in Exec()
mainJson["environment"] = ContentTypeInstance.StackInstance.Config.Environment;
```

#### Returning void from Query Method
```csharp
// Bad — breaks fluent chaining
public void SetMyParam(string value) { UrlQueries["my_param"] = value; }

// Good
public Query SetMyParam(string value) { UrlQueries["my_param"] = value; return this; }
```

#### Dictionary Not Initialized for QueryValueJson Entry
```csharp
// Bad — throws KeyNotFoundException or InvalidCastException
((Dictionary<string, object>)QueryValueJson[key])["$op"] = value;

// Good — guard with ContainsKey
if (!QueryValueJson.ContainsKey(key))
    QueryValueJson[key] = new Dictionary<string, object>();
((Dictionary<string, object>)QueryValueJson[key])["$op"] = value;
```

## SDK-specific red flags

Quick scan for anti-patterns in PRs:

```
❌ new HttpClient()               — use HttpRequestHandler
❌ throw new Exception("message") — use typed ContentstackException subclass
❌ "hardcoded_field_name"         — use [JsonProperty] or ErrorMessages constant
❌ public Config GetConfig()      — Config is internal by design
❌ return void                    — Query methods return Query (fluent)
❌ [JsonProperty] omitted         — CDA uses snake_case; PascalCase won't deserialize
❌ <Version> in .csproj           — use Directory.Build.props
```

For full category-by-category review, see **Code review checklist** above.
