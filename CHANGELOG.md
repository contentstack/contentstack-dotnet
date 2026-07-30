### Version: 3.2.0
#### Date: Aug-3-2026

##### Feat:
- Taxonomy Publishing CDA support
  - `Taxonomies(uid)` scopes to a specific taxonomy: `.Fetch<T>()`, `.Term(termUid)`, `.Terms()`
  - `Taxonomies().Find<T>()` — list all published taxonomies (`GET /taxonomies`)
  - New `Term` class: `.Fetch<T>()`, `.Locales<T>()`, `.Ancestors<T>()`, `.Descendants<T>()`
  - New `TermQuery` class: `.Find<T>()` for listing terms within a taxonomy
  - `Depth(int)` and `IncludeBranch()` on `Term`/`TermQuery` for hierarchy traversal control
  - `Skip(int)`, `Limit(int)`, `IncludeCount()` on `TermQuery` for paginated term listing
  - Added `Internals/TaxonomyRequestHelper` — shared request-building, header-merging, and error-parsing for `Taxonomy`/`Term`/`TermQuery`

- Taxonomy / Term / TermQuery — localization support
  - `SetLocale(string)` filters the taxonomy/term response to a specific locale (e.g. `"fr-fr"`)
  - `IncludeFallback()` returns the master-locale (`en-us`) version when a term/taxonomy isn't translated into the requested locale, instead of omitting it
  - Both compose correctly with hierarchy traversal — `Term(uid).SetLocale("fr-fr").IncludeFallback().Depth(2).Descendants<T>()` returns a full localized subtree, with untranslated nodes individually falling back to master locale in the same response
  - Fallback is per-node, not all-or-nothing: a single hierarchy fetch can return some terms translated and others fallen-back simultaneously — safe to use on partially-translated taxonomies

---

### Version: 3.1.0
#### Date: Jul-20-2026

##### Feat:
- Entry Variants Branch Support
  - Added support for passing an optional `branch` parameter to the `.Variant()` method in both `Entry` and `Query` classes.
  - If the branch parameter is null or empty, it automatically falls back to the Stack's configured branch or "main".
  - Added comprehensive unit and integration tests for Entry and Query variant branch logic.

---

### Version: 3.0.0
#### Date: Jul-13-2026

##### Breaking Changes:
- Removed `Newtonsoft.Json` dependency; all JSON serialisation now uses `System.Text.Json` (BCL)
- `Entry.ToJson()`, `Query.Count()`, `AssetLibrary.Count()` now return `JsonObject` instead of `JObject`
- `AssetLibrary.Query(JsonObject)` — parameter type changed from `JObject` to `JsonObject`
- `ContentType.Fetch()`, `GlobalField.Fetch()`, `GlobalFieldQuery.Find()` now return `JsonObject` instead of `JObject`
- `SerializerSettings` → `SerializerOptions`
- Model classes use `[JsonPropertyName]` instead of `[JsonProperty]`
- Requires **.NET 10** or later
- Updated `contentstack.utils` dependency to `2.0.0` (final, non-beta)

##### Feat:
- Added `Endpoint` class for dynamic region-to-URL resolution via CDN-backed `regions.json`
- Added `ContentstackRegionMap` to map `ContentstackRegion` enum to registry region IDs
- Added `GCP_EU` region support
- Added `ApiErrorBodyParser` for consistent API error envelope parsing
- Added `JsonNodeConversion` and `JsonObjectMerge` utilities to replace Newtonsoft equivalents
- Added `ContentstackJsonDefaults` — shared `JsonSerializerOptions` used across all custom converters

##### Enh:
- `Config.BaseUrl` now resolves hosts from the regions registry; removed hardcoded `regionCode()` and `HostURL`
- Replaced `Console.WriteLine` with `Debug.WriteLine` in `ContentstackConvert` to suppress parse warnings from application stdout

##### Chore:
- Replaced `refresh-region.cs` with `refresh-region.py` — avoids MSBuild compiling the script as source
- Added `build/contentstack.csharp.targets` to auto-deliver `refresh-region.py` to consumer projects on first build
- Added `Assets/regions.json` to `.gitignore`
- Added `EndpointTest.cs`
- Updated .NET version in SCA scan CI from `7.0.x` to `10.0.x`

##### Migration Guide:
- See [Migrating from Newtonsoft.Json to System.Text.Json](https://www.contentstack.com/docs/developers/sdks/content-delivery-sdk/dot-net/migrate-dotnet-delivery-sdk-from-newtonsoft.json-to-system.text.json) for the full upgrade path from v2.x.

---

### Version: 3.0.0-beta.2
#### Date: Jun-22-2026

##### Feat:
- Added `Endpoint` class for dynamic region-to-URL resolution via CDN-backed `regions.json`
- Added `ContentstackRegionMap` to map `ContentstackRegion` enum to registry region IDs
- Added `GCP_EU` region support

##### Enh:
- `Config.BaseUrl` now resolves hosts from the regions registry; removed hardcoded `regionCode()` and `HostURL`

##### Chore:
- Replaced `refresh-region.cs` with `refresh-region.py` — avoids MSBuild compiling the script as source
- Added `build/contentstack.csharp.targets` to auto-deliver `refresh-region.py` to consumer projects on first build
- Added `Assets/regions.json` to `.gitignore`
- Added `EndpointTest.cs`

---

### Version: 3.0.0-beta.1
#### Date: May-04-2026

##### Breaking Changes:
- Removed `Newtonsoft.Json` dependency; all JSON serialisation now uses `System.Text.Json` (BCL)
  - `AssetJsonConverter` and `EntryJsonConverter` now implement `System.Text.Json.Serialization.JsonConverter<T>`
  - `ContentstackCollection<T>` no longer carries `[JsonObject]`; direct `JsonSerializer` usage on this type is not supported
- Updated `contentstack.utils` from `1.0.6` to `2.0.0-beta.1` (major version bump)

##### Feat:
- Migrated all internal JSON handling to `System.Text.Json`
- Added `ApiErrorBodyParser` for consistent API error envelope parsing
- Added `JsonNodeConversion` and `JsonObjectMerge` utilities to replace Newtonsoft equivalents
- Added `ContentstackJsonDefaults` — shared `JsonSerializerOptions` used across all custom converters

##### Enh:
- Replaced `Console.WriteLine` with `Debug.WriteLine` in `ContentstackConvert` to suppress parse warnings from application stdout

##### Chore:
- Updated .NET version in SCA scan CI from `7.0.x` to `10.0.x`

---

### Version: 2.28.0
#### Date: Jun-24-2026

##### Fix:
- Register `EmbeddedObjectConverter` in `ContentstackClient` constructor so `.includeEmbeddedItems().Fetch<T>()` deserializes `_embedded_items` correctly when the model implements `IEntryEmbedable`. No changes required in consumer code.
- Upgraded utils dependency from `contentstack.utils 1.3.0` to `contentstack.utils 1.4.0` which ships the concrete `EmbeddedObject` class and `EmbeddedObjectConverter`.

---

### Version: 2.27.0
#### Date: Apr-23-2026

##### Feat:
- Timeline Preview Support
  - Added `ReleaseId` and `PreviewTimestamp` properties to `LivePreviewConfig` for temporal content queries
  - Enhanced `LivePreviewQueryAsync()` to support `preview_timestamp` and `release_id` parameters
  - Implemented Timeline Preview API headers (`preview_timestamp`, `release_id`) in preview requests
  - Added intelligent cache fingerprinting system to prevent stale timeline data
  - New `IsCachedPreviewForCurrentQuery()` method for Timeline-aware cache validation
  - Fork isolation now maintains independent Timeline contexts for concurrent operations
  - Timeline Preview works seamlessly with complex nested content types and group fields
- Integration Test Coverage Enhancement  
  - Added comprehensive Timeline Preview integration test suites (70+ test cases)
  - New test categories: `TimelinePreviewApiTests`, `TimelineAuthenticationTests`, `TimelineCacheValidationTests`
  - Enhanced performance testing with Timeline-specific benchmarking
  - Added authentication flow validation for Management Token vs Preview Token scenarios
  - Comprehensive error handling tests for Timeline Preview edge cases


---

### Version: 2.26.0
#### Date: Feb-10-2026

##### Feat:
- CDA / – AssetFields support
  - Added `AssetFields(params string[] fields)` to request specific asset-related metadata via the CDA `asset_fields[]` query parameter
  - Implemented on: Entry (single entry fetch), Query (entries find), Asset (single asset fetch), AssetLibrary (assets find)
  - Valid parameters: `user_defined_fields`, `embedded_metadata`, `ai_generated_metadata`, `visual_markups`
  - Method is chainable; when called with no arguments, the query parameter is not set
- CDA / – Asset localisation support
  - Added `SetLocale(string locale)` on Asset for single-asset fetch by locale (e.g. `stack.Asset(uid).SetLocale("en-us").Fetch()`)
  - Added `Title` property on Asset for localised title in API response
  - AssetLibrary `SetLocale` continues to support listing assets by locale

### Version: 2.25.2
#### Date: Nov-13-2025

##### Fix:
- Error Handling
  - Fixed error message extraction from Contentstack API responses across all model classes
  - HTTP request errors now properly extract and display actual API error messages instead of generic exception messages
  - Improved error handling in Query, Entry, Asset, GlobalField, ContentType, AssetLibrary, GlobalFieldQuery, and Taxonomy classes
  - Users will now see meaningful error messages (e.g., "Invalid API key", "Entry not found") instead of generic "Exception of type 'ContentstackException' was thrown" messages
  - ErrorCode, StatusCode, and Errors dictionary are now properly populated from API responses

### Version: 2.25.1
#### Date: Nov-10-2025

##### Enh: 
- Improved Error messages
##### Fix: 
- Taxonomy
  - Fixed NullReferenceExceptions 
  - Fixed InvalidCastException in `GetContentstackError` when exception is not a WebException
  - Fixed JsonReaderException in `GetContentstackError` when response is not valid JSON
  - All exceptions now properly throw TaxonomyException (extends ContentstackException) with descriptive error messages

### Version: 2.25.0
#### Date: Jan-07-2025

##### Feat: 
- AssetLibrary
  - Added new `Where` method for simple key-value pair queries
  - Enhanced `Query` method to support multiple calls with intelligent merging
- Improved query handling with better null safety and error handling

### Version: 2.24.0
#### Date: Sep-29-2025

##### Feat: 
- Added Support For AWS-AU Region

### Version: 2.23.0
#### Date: Aug-05-2025

##### Feat: 
- Fetch Assets using tags

### Version: 2.22.2
#### Date: July-14-2025

##### Fix: 
- Fixed token issue for Live Preview

### Version: 2.22.1
#### Date: June-13-2025

##### Fix: 
- Fixed Timeline issue of Entry not getting updated

### Version: 2.22.0
#### Date: March-03-2025

##### Feat: 
- Added Support for Global Fields

### Version: 2.21.0
#### Date: March-03-2025

##### Feat: 
- Added Support for Timeline Preview

### Version: 2.20.0
#### Date: Dec-19-2024

##### Fix: 
- Reset `LivePreviewConfig` to prevent overwriting fetched data with live preview data during regular fetch calls.

### Version: 2.19.0
#### Date: Nov-30-2024

##### Fix: 
- Remove updateLPContent call to resolve incorrect display of unsaved changes

### Version: 2.18.0
#### Date: Nov-18-2024

##### Feat: 
- Added support to access different properties of asset

### Version: 2.17.0
#### Date: Oct-21-2024

##### Feat: 
- Added support for fetch asset by Query 

### Version: 2.16.0
#### Date: Oct-11-2024

##### Feat: 
- Live Preview 2.0 Implementation
##### Fix: 
- Removed exclusion of env when adding headers

### Version: 2.15.0
#### Date: Jul-30-2024

##### New Feature: 
- Taxonomy class added
- Added Early Access Header Support

### Version: 2.14.0
#### Date: May-28-2024

##### New Feature: 
- GCP_NA region support added
- AddParam method added for Entry, Asset, AssetLibrary and Query

### Version: 2.13.0
#### Date: April-02-2024

##### New Feature: 
- Proxy support added

### Version: 2.12.0
#### Date: Feb-01-2024

##### New Feature: 
- Timeout support added

### Version: 2.11.0
#### Date: Sep-27-2023

##### New Feature: 
- Region support added
- IncludeMetadata support added

### Version: 2.8.0
#### Date: Jan-11-2021

##### Bug fix:
- Live preview Query issue
##### New Feature: 
- Entry
  - IncludeOnlyReference function added
  - IncludeExceptReference function added
- Query
  - IncludeOnlyReference function added
  - IncludeExceptReference function added

### Version: 2.7.0
#### Date: Oct-14-2021

##### New Feature:
- Live preview feature support added

### Version: 2.6.0
#### Date: Apr-05-2021

##### New Feature:
- Entry
  - IncludeEmbeddedItems function added
- Query
  - IncludeEmbeddedItems function added

### Version: 2.5.0
#### Date: Dec-05-2020

##### Update API:
 - AssetLibrary
   - IncludeFallback function added
   - SetLocale function added
- Asset
  - IncludeFallback function added
  - SetLocale function added
- Entry
  - IncludeFallback function added
- Query
  - IncludeFallback function added

### Version: 2.4.0
#### Date: Aug-12-2020

##### Update API:
 - AssetLibrary
   - Count function added
   - Limit, Skip functionality added
   - Only, Except function added
 - Query 
   - Count function added
 - CSJsonConverter 
   - Added class CSJsonConverter to allow autoloading of converters
##### Enhancement
 - Stack
  - Sync function to allow multiple SyncType
##### Bug Fixes
 - Entry
  - GetContentType exception resolved
##### Deprecation
 - Stack
  - AccessToken deprecated with support to add DeliveryToken

### Version: 2.3.0
#### Date: Jun-22-2020

##### Update API:
 - GetEnvironment issue resolved
 - GetDeleted at Method addedAssetLibrary
 - SyncType issue resolved

### Version: 2.2.1
#### Date: Feb-17-2020

##### Update API:
- Query
  - update function 'IncludeOwner'
- Entry
  - update function 'IncludeOwner'

##### Update API:
  
### Version: 2.2.0
#### Date: Nov-15-2019

##### Update API:
- Stack
  - update function 'GetContentType:'
- ContentType
  - update function 'Fetch:'
  
### Version: 2.1.1
#### Date: Sept-03-2019

##### New Features:
- Config
  - added property attribute 'region'
- Query
  - added method 'ReferenceIn'
  - added method 'ReferenceNotIn'

### Version: 2.1.0
#### Date: Jul-29-2019

##### New Features:
- Query
  - added method 'includeReferenceContentTypeUid'
- Entry
  - added method 'includeReferenceContentTypeUid'
  
### Version: 
#### Date: Jun-28-2019

##### New Features:
- Query
  - added method 'SetLocale'
  
- Entry
   - added method 'SetLocale'

 ##### Update API
- Query
  - update method 'Fetch'
  - update method 'FindOne'
  
- Entry
  - update method 'Find'
 
##### Deprecated API
- Query
  - deprecated method 'SetLanguage'
  
- Entry
  - deprecated method 'SetLanguage'

### Version: 1.1.0
#### Date: Apr-12-2019

##### New Features:
- ContentstackClient
  - added method 'GetContentTypes'
  - added method 'SyncRecursive' 
  - added method 'SyncPaginationToken'
  - added method 'SyncToken'

- CotentType
   - added method 'Fetch'

 ### Version: 1.0.6
 #### Date: Aug-10-2018

Localization support for Query and Entry is added.
 
### Version: 1.0.0 
#### Date: Jun-1-2018 

- Introduce Contentstack SDK for DOTNET.
