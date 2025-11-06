using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Mocks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests.Mocks
{
    /// <summary>
    /// Helper class for creating Entry objects for unit testing
    /// </summary>
    public static class EntryTestHelper
    {
        /// <summary>
        /// Creates an Entry object from JSON string for testing
        /// </summary>
        public static Entry CreateEntryFromJson(string jsonString, ContentstackClient client, string contentTypeId = "source")
        {
            JObject jsonObj = JObject.Parse(jsonString);
            JToken entryToken = jsonObj["entry"] ?? jsonObj;
            
            // Create Entry using internal constructor
            var entry = CreateEntryInternal(client, contentTypeId);
            
            // Use reflection to call ParseObject
            var parseMethod = typeof(Entry).GetMethod("ParseObject", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            parseMethod?.Invoke(entry, new object[] { entryToken as JObject, null });
            
            return entry;
        }

        /// <summary>
        /// Creates an Entry object with mock data
        /// </summary>
        public static Entry CreateEntry(ContentstackClient client, string contentTypeId = "source", 
            Dictionary<string, object> attributes = null)
        {
            var entry = CreateEntryInternal(client, contentTypeId);
            
            if (attributes != null)
            {
                var field = typeof(Entry).GetField("_ObjectAttributes", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                field?.SetValue(entry, attributes);
            }
            
            return entry;
        }

        /// <summary>
        /// Creates a ContentstackClient with mock response handler
        /// </summary>
        public static ContentstackClient CreateMockClient(string mockResponse = null)
        {
            var fixture = new Fixture();
            var options = new ContentstackOptions()
            {
                ApiKey = fixture.Create<string>(),
                DeliveryToken = fixture.Create<string>(),
                Environment = fixture.Create<string>()
            };
            
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            
            if (!string.IsNullOrEmpty(mockResponse))
            {
                var plugin = new MockResponsePlugin(mockResponse);
                client.Plugins.Add(plugin);
            }
            
            return client;
        }

        private static Entry CreateEntryInternal(ContentstackClient client, string contentTypeId)
        {
            // Create ContentType and Entry
            var contentType = client.ContentType(contentTypeId);
            var entry = contentType.Entry("test_entry_uid");
            return entry;
        }
    }
}



