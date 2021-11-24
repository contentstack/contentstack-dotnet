
# Contentstack - .Net SDK

.NET SDK for Contentstack's Content Delivery API

Contentstack is a headless CMS with an API-first approach. It is a CMS that developers can use to build powerful cross-platform applications in their favorite languages. Build your application frontend, and Contentstack will take care of the rest.  [Read More](https://www.contentstack.com/).

For more details about the namespaces and classes navigate menu to left.

## Prerequisites

To get started with C#, you will need:

.net platform, IDE (Visual Studio) and NuGet.

## SDK installation and setup

The .Net SDK provided by contentstack.io is available for Xamarin, Windows Phone and legacy .Net applications. You can integrate contentstack with your application by following these steps.

Open the terminal and install the contentstack module via 'Package Manager' command

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
// Initialize the Contentstack 
ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "enviroment_name");
```

or:

``` cs
//
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
Entry entry = client.ContentType("blog").Entry("blta464e9fbd048668c");
entry.Fetch<Product>().ContinueWith((t) => { 
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
query.Find<Product>().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
        ContentstackCollection<Product> result = t.Result; 
        Console.WriteLine("result" + result.items); 
    } 
});
```
## Example

To help you get started, we have created a sample console application that is powered by Contentstack .NET SDK. Click on the link below to read the tutorial of the app.

[.NET News Console App](https://contentstack.com/docs/example-apps/build-a-news-app-using-contentstack-dot-net-sdk)

## Helpful Links

-   [Contentstack Website](https://www.contentstack.com/)
-   [Official Documentation](https://contentstack.com/docs)
-   [Content Delivery API Docs](https://contentstack.com/docs/apis/content-delivery-api/)
