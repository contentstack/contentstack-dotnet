// Contentstack.Core/ContentstackClientTest.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Xunit.Abstractions;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.ClientTests
{
    [Trait("Category", "ClientInternal")]
    public class ContentstackClientTest : IntegrationTestBase
    {
        public ContentstackClientTest(ITestOutputHelper output) : base(output)
        {
        }

        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                ApiKey = "api_key",
                DeliveryToken = "delivery_token",
                Environment = "environment",
                Version = "1.2.3",
                LivePreview = new LivePreviewConfig()
                {
                    Enable = true,
                    Host = "https://preview.contentstack.com",
                    ReleaseId = "rid",
                    PreviewTimestamp = "ts",
                    PreviewToken = "pt"

                }
            };
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }

        private string GetPrivateField(ContentstackClient client, string fieldName)
        {
            var field = typeof(ContentstackClient).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)field.GetValue(client);
        }

        [Fact(DisplayName = "Set Entry Uid Sets Value When Non Empty")]
        public void SetEntryUid_SetsValue_WhenNonEmpty()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.SetEntryUid("entry123");
            TestAssert.Equal("entry123", GetPrivateField(client, "currentEntryUid"));
        }

        [Fact(DisplayName = "Set Entry Uid Does Not Set When Empty")]
        public void SetEntryUid_DoesNotSet_WhenEmpty()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.SetEntryUid("entry123");
            client.SetEntryUid("");
            TestAssert.Equal("entry123", GetPrivateField(client, "currentEntryUid"));
        }

        [Fact(DisplayName = "Set Entry Uid Does Not Set When Null")]
        public void SetEntryUid_DoesNotSet_WhenNull()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.SetEntryUid("entry123");
            client.SetEntryUid(null);
            TestAssert.Equal("entry123", GetPrivateField(client, "currentEntryUid"));
        }

        [Fact(DisplayName = "Content Type Setscurrent Contenttype Uid When Non Empty")]
        public void ContentType_SetscurrentContenttypeUid_WhenNonEmpty()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.ContentType("blog");
            TestAssert.Equal("blog", GetPrivateField(client, "currentContenttypeUid"));
        }

        [Fact(DisplayName = "Content Type Does Not Set When Empty")]
        public void ContentType_DoesNotSet_WhenEmpty()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.ContentType("blog");
            client.ContentType("");
            TestAssert.Equal("blog", GetPrivateField(client, "currentContenttypeUid"));
        }

        [Fact(DisplayName = "Content Type Does Not Set When Null")]
        public void ContentType_DoesNotSet_WhenNull()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.ContentType("blog");
            client.ContentType(null);
            TestAssert.Equal("blog", GetPrivateField(client, "currentContenttypeUid"));
        }

        [Fact(DisplayName = "Content Type Returns Content Type Instance")]
        public void ContentType_ReturnsContentTypeInstance()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            var contentType = client.ContentType("blog");
            TestAssert.NotNull(contentType);
            TestAssert.Equal("blog", contentType.ContentTypeId);
        }

        [Fact(DisplayName = "Global Field Returns Global Field Instance")]
        public void GlobalField_ReturnsGlobalFieldInstance()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            var globalField = client.GlobalField("author");
            TestAssert.NotNull(globalField);
            TestAssert.Equal("author", globalField.GlobalFieldId);
        }

        [Fact(DisplayName = "Asset Returns Asset Instance")]
        public void Asset_ReturnsAssetInstance()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            var asset = client.Asset("asset_uid");
            TestAssert.NotNull(asset);
            TestAssert.Equal("asset_uid", asset.Uid);
        }

        [Fact(DisplayName = "Asset Library Returns Asset Library Instance")]
        public void AssetLibrary_ReturnsAssetLibraryInstance()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            var assetLibrary = client.AssetLibrary();
            TestAssert.NotNull(assetLibrary);
        }

        [Fact(DisplayName = "Taxonomies Returns Taxonomy Instance")]
        public void Taxonomies_ReturnsTaxonomyInstance()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            var taxonomy = client.Taxonomies();
            TestAssert.NotNull(taxonomy);
        }

        [Fact(DisplayName = "Get Version Returns Version")]
        public void GetVersion_ReturnsVersion()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            var t = client.GetVersion();
            TestAssert.Equal("1.2.3", client.GetVersion());
        }

        [Fact(DisplayName = "Get Application Key Returns Api Key")]
        public void GetApplicationKey_ReturnsApiKey()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            TestAssert.Equal("api_key", client.GetApplicationKey());
        }

        [Fact(DisplayName = "Get Access Token Returns Delivery Token")]
        public void GetAccessToken_ReturnsDeliveryToken()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            TestAssert.Equal("delivery_token", client.GetAccessToken());
        }

        [Fact(DisplayName = "Get Live Preview Config Returns Config")]
        public void GetLivePreviewConfig_ReturnsConfig()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            TestAssert.NotNull(client.GetLivePreviewConfig());
        }

        [Fact(DisplayName = "Remove Header Removes Header")]
        public void RemoveHeader_RemovesHeader()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.SetHeader("custom", "value");
            client.RemoveHeader("custom");
            var localHeaders = typeof(ContentstackClient)
                .GetField("_LocalHeaders", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(client) as Dictionary<string, object>;
            TestAssert.False(localHeaders.ContainsKey("custom"));
        }

        [Fact(DisplayName = "Set Header Adds Header")]
        public void SetHeader_AddsHeader()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.SetHeader("custom", "value");
            var localHeaders = typeof(ContentstackClient)
                .GetField("_LocalHeaders", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(client) as Dictionary<string, object>;
            TestAssert.True(localHeaders.ContainsKey("custom"));
            TestAssert.Equal("value", localHeaders["custom"]);
        }

        [Fact(DisplayName = "Live Preview Query Async Sets Live Preview Config Fields")]
        public async Task LivePreviewQueryAsync_SetsLivePreviewConfigFields()
        {
            LogArrange("Setting up test");

            var client = CreateClient();
            client.ContentType("ctuid");
            client.ContentType("ctuid").Entry("euid");
            // Mock GetLivePreviewData to avoid actual HTTP call
            var method = typeof(ContentstackClient).GetMethod("GetLivePreviewData", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(client, null); // Just to ensure method exists

            var query = new Dictionary<string, string>
            {
                { "live_preview", "hash" },
                { "release_id", "rid" },
                { "preview_timestamp", "ts" }
            };

            // Patch GetLivePreviewData to return a dummy JObject

            // Since GetLivePreviewData is private and does a real HTTP call, 
            // we only test the config fields are set correctly before the call
            await client.LivePreviewQueryAsync(query);
            var v = client.GetLivePreviewConfig();
            TestAssert.Equal("ctuid", GetPrivateField(client, "currentContenttypeUid"));
            TestAssert.Equal("euid", GetPrivateField(client, "currentEntryUid"));
            TestAssert.Equal(true, v.Enable );
            TestAssert.Equal("rid", v.ReleaseId);
            TestAssert.Equal("ts", v.PreviewTimestamp);
            TestAssert.Equal("pt", v.PreviewToken);
        }

        // For SyncRecursive, SyncPaginationToken, SyncToken, you should mock HTTP calls.
        // Here we just check that the methods exist and can be called (will throw if not configured).
        [Fact(DisplayName = "Sync Recursive Throws Or Returns")]
        public async Task SyncRecursive_ThrowsOrReturns()
        {
            LogArrange("Setting up error handling test");

            var client = CreateClient();
            await TestAssert.ThrowsAnyAsync<Exception>(() => client.SyncRecursive());
        }

        [Fact(DisplayName = "Sync Pagination Token Throws Or Returns")]
        public async Task SyncPaginationToken_ThrowsOrReturns()
        {
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            await TestAssert.ThrowsAnyAsync<Exception>(() => client.SyncPaginationToken("pagetoken"));
        }

        [Fact(DisplayName = "Sync Token Throws Or Returns")]
        public async Task SyncToken_ThrowsOrReturns()
        {
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            await TestAssert.ThrowsAnyAsync<Exception>(() => client.SyncToken("synctoken"));
        }
    }
}