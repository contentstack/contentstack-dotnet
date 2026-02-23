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
