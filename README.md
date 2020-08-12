[![Contentstack](https://www.contentstack.com/docs/static/images/contentstack.png)](https://www.contentstack.com/)
# Contentstack dotnet

.NET SDK for Contentstack's Content Delivery API

## Getting Started

This guide will help you get started with our .NET SDK to build apps powered by Contentstack.

## SDK Installation and Setup

To use the .NET SDK, download it from here

Open the terminal and install the contentstack module via ‘Package Manager’ command

``` console
PM> Install-Package contentstack.csharp
```
And via ‘.Net CLI’
``` console
dotnet add package contentstack.csharp
```
To use the module in your application, you need to first Add Namespace to your class

``` cs
using Contentstack.Core; // ContentstackClient 
using Contentstack.Core.Models; // Stack, Query, Entry, Asset, ContentType, ContentstackCollection
using Contentstack.Core.Configuration; // ContentstackOptions
```

## Initialize SDK

You will need to specify the API key, Access token, and Environment Name of your stack to initialize the SDK:

``` cs
ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "enviroment_name");
```
or:

``` cs
var options = new ContentstackOptions()
{
    ApiKey = "<api_key>",
    DeliveryToken = "<delivery_token>"
    Environment = "<environment>"
}
ContentstackClient stack = new ContentstackClient(options);
```

Once you have initialized the SDK, you can start getting content in your app.

## Basic Queries

### Get a Single Entry

To retrieve a single entry from a content type, use the code snippet given below:
``` cs
Entry entry = client.ContentType("product").Entry("blta464e9fbd048668c");
entry.Fetch<Product>().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
        Console.WriteLine("entry:" + t.Result);  
    } 
});
```

### Get Multiple Entries

To retrieve multiple entries of a particular content type, use the code snippet given below:

``` cs
Query query = client.ContentType("product").Query(); 
query.Where("title", "welcome"); 
query.IncludeSchema(); 
query.IncludeCount(); 
query.ToJSON(); 
query.Find<Product>().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
         ContentstackCollection<Product> result = t.Result; 
         Console.WriteLine("result" + result.items); 
    } 
});
```
These were example of some of the basic queries of the SDK. For advanced queries, refer to our API reference documentation by visiting the link given below.


 > ***Note***: Currently, the .NET SDK does not support multiple content types referencing in a single query. For more information on how to query entries and assets, refer the [Queries](https://www.contentstack.com/docs/developers/apis/content-delivery-api/#queries) section of our Content Delivery API documentation.

### Paginating Responses
In a single instance, the [Get Multiple Entries](https://www.contentstack.com/docs/developers/dot-net/get-started-with-dot-net-sdk/#get-multiple-entries) query will retrieve only the first 100 items of the specified content type. You can paginate and retrieve the rest of the items in batches using the [skip](https://www.contentstack.com/docs/platforms/dot-net/api-reference/api/Contentstack.Core.Models.Query.html#Contentstack_Core_Models_Query_Skip_System_Int32_) and [limit](https://www.contentstack.com/docs/platforms/dot-net/api-reference/api/Contentstack.Core.Models.Query.html#Contentstack_Core_Models_Query_Limit_System_Int32_) parameters in subsequent requests.

``` cs
Query query = client.ContentType("product").Query();
query.Skip(20);
query.Limit(20); 
query.Find<Product>().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
         ContentstackCollection<Product> result = t.Result; 
         Console.WriteLine("result" + result); 
    } 
});
```
## API Reference
Go through our .NET SDK API Reference guide to know about the methods that can be used to query your content in Contentstack.

[Read .NET API Reference Guide](https://www.contentstack.com/docs/platforms/dot-net/api-reference/api/index.html)

## Example
To help you get started, we have created a sample application that is powered by Contentstack .NET SDK. Click on the link below to read the tutorial of the app.

[.NET News Console App](https://www.contentstack.com/docs/example-apps/build-a-news-app-using-contentstack-dot-net-sdk)


### Helpful Links

- [Contentstack Website](https://www.contentstack.com) 
- [Official Documentation](https://contentstack.com/docs) 
- [Content Delivery API Docs](https://contentstack.com/docs/apis/content-delivery-api/) 
