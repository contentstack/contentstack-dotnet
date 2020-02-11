l[![Contentstack](https://www.contentstack.com/docs/static/images/contentstack.png)](https://www.contentstack.com/)
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
ContentstackClient stack = new ContentstackClient("api_key", "access_token", "enviroment_name");
```
or:

``` cs
var options = new ContentstackOptions()
{
    ApiKey = "<api_key>",
    AccessToken = "<access_token>"
    Environment = "<environment>"
}
ContentstackClient stack = new ContentstackClient(options);
```

Once you have initialized the SDK, you can start getting content in your app.

## Basic Queries

### Get a Single Entry

To retrieve a single entry from a content type, use the code snippet given below:
``` cs
Entry entry = client.ContentType("blog").Entry("blta464e9fbd048668c");
entry.Fetch<Blog>().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
        Console.WriteLine("entry:" + t.Result);  
    } 
});
```

### Get Multiple Entries

To retrieve multiple entries of a particular content type, use the code snippet given below:

``` cs
Query query = client.ContentType("blog").Query(); 
query.Where("title", "welcome"); 
query.IncludeSchema(); 
query.IncludeCount(); 
query.ToJSON(); 
query.Find<Blog>().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
         ContentstackCollection<Blog> result = t.Result; 
         Console.WriteLine("result" + result.items); 
    } 
});
```
These were example of some of the basic queries of the SDK. For advanced queries, refer to our API reference documentation by visiting the link given below.

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
