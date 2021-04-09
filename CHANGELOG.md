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
