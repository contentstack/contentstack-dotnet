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
using Contentstack.core; // ContentstackClient 
using Contentstack.Core.Models; // Stack, Query, Entry, Asset, ContentType
```

## Initialize SDK

You will need to specify the API key, Access token, and Environment Name of your stack to initialize the SDK:

``` cs
// Initialize the Contentstack 
Stack Stack stack = ContentstackClient.Stack("api_key", "access_token", "enviroment_name");
```

Once you have initialized the SDK, you can start getting content in your app.

## Basic Queries

### Get a Single Entry

To retrieve a single entry from a content type, use the code snippet given below:
``` cs
Entry entry = stack.ContentType("blog").Entry("blta464e9fbd048668c");
entry.Fetch().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
        Console.WriteLine("entry:" + t.Result);  
    } 
});
```

### Get Multiple Entries

To retrieve multiple entries of a particular content type, use the code snippet given below:

``` cs

Query query = stack.ContentType("blog").Query(); 
query.Where("title", "welcome"); 
query.IncludeSchema(); 
query.IncludeCount(); 
query.ToJSON(); 
query.Find().ContinueWith((t) => { 
    if (!t.IsFaulted) { 
         Entry[] result = t.Result.Result; Console.WriteLine("result" + result); 
    } 
});
```
These were example of some of the basic queries of the SDK. For advanced queries, refer to our API reference documentation by visiting the link given below.

## API Reference
Go through our .NET SDK API Reference guide to know about the methods that can be used to query your content in Contentstack.
[Read .NET API Reference Guide](www.contentstack.com)

## Example
To help you get started, we have created a sample application that is powered by Contentstack .NET SDK. Click on the link below to read the tutorial of the app.

[.NET News Console App](https://stag-www.contentstack.com/docs/example-apps/build-a-news-app-using-contentstack-dot-net-sdk)


### Helpful Links

- [Contentstack Website](https://www.contentstack.com) 
- [Official Documentation](https://contentstack.com/docs) 
- [Content Delivery API Docs](https://contentstack.com/docs/apis/content-delivery-api/) 

### The MIT License (MIT)

Copyright © 2012-2018 [Contentstack](https://www.contentstack.com/). All Rights Reserved

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.