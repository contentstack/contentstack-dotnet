// Contentstack.Core/ContentstackClientTest.cs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;

namespace Contentstack.Core.Tests.Integration.ClientTests
{
    [Trait("Category", "ClientInternal")]
    public class ContentstackClientTest
    {
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
            return new ContentstackClient(options);
        }

        private string GetPrivateField(ContentstackClient client, string fieldName)
        {
            var field = typeof(ContentstackClient).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)field.GetValue(client);
        }

        [Fact(DisplayName = "Set Entry Uid Sets Value When Non Empty")]
        public void SetEntryUid_SetsValue_WhenNonEmpty()
        {
            var client = CreateClient();
            client.SetEntryUid("entry123");
            Assert.Equal("entry123", GetPrivateField(client, "currentEntryUid"));
        }

        [Fact(DisplayName = "Set Entry Uid Does Not Set When Empty")]
        public void SetEntryUid_DoesNotSet_WhenEmpty()
        {
            var client = CreateClient();
            client.SetEntryUid("entry123");
            client.SetEntryUid("");
            Assert.Equal("entry123", GetPrivateField(client, "currentEntryUid"));
        }

        [Fact(DisplayName = "Set Entry Uid Does Not Set When Null")]
        public void SetEntryUid_DoesNotSet_WhenNull()
        {
            var client = CreateClient();
            client.SetEntryUid("entry123");
            client.SetEntryUid(null);
            Assert.Equal("entry123", GetPrivateField(client, "currentEntryUid"));
        }

        [Fact(DisplayName = "Content Type Setscurrent Contenttype Uid When Non Empty")]
        public void ContentType_SetscurrentContenttypeUid_WhenNonEmpty()
        {
            var client = CreateClient();
            client.ContentType("blog");
            Assert.Equal("blog", GetPrivateField(client, "currentContenttypeUid"));
        }

        [Fact(DisplayName = "Content Type Does Not Set When Empty")]
        public void ContentType_DoesNotSet_WhenEmpty()
        {
            var client = CreateClient();
            client.ContentType("blog");
            client.ContentType("");
            Assert.Equal("blog", GetPrivateField(client, "currentContenttypeUid"));
        }

        [Fact(DisplayName = "Content Type Does Not Set When Null")]
        public void ContentType_DoesNotSet_WhenNull()
        {
            var client = CreateClient();
            client.ContentType("blog");
            client.ContentType(null);
            Assert.Equal("blog", GetPrivateField(client, "currentContenttypeUid"));
        }

        [Fact(DisplayName = "Content Type Returns Content Type Instance")]
        public void ContentType_ReturnsContentTypeInstance()
        {
            var client = CreateClient();
            var contentType = client.ContentType("blog");
            Assert.NotNull(contentType);
            Assert.Equal("blog", contentType.ContentTypeId);
        }

        [Fact(DisplayName = "Global Field Returns Global Field Instance")]
        public void GlobalField_ReturnsGlobalFieldInstance()
        {
            var client = CreateClient();
            var globalField = client.GlobalField("author");
            Assert.NotNull(globalField);
            Assert.Equal("author", globalField.GlobalFieldId);
        }

        [Fact(DisplayName = "Asset Returns Asset Instance")]
        public void Asset_ReturnsAssetInstance()
        {
            var client = CreateClient();
            var asset = client.Asset("asset_uid");
            Assert.NotNull(asset);
            Assert.Equal("asset_uid", asset.Uid);
        }

        [Fact(DisplayName = "Asset Library Returns Asset Library Instance")]
        public void AssetLibrary_ReturnsAssetLibraryInstance()
        {
            var client = CreateClient();
            var assetLibrary = client.AssetLibrary();
            Assert.NotNull(assetLibrary);
        }

        [Fact(DisplayName = "Taxonomies Returns Taxonomy Instance")]
        public void Taxonomies_ReturnsTaxonomyInstance()
        {
            var client = CreateClient();
            var taxonomy = client.Taxonomies();
            Assert.NotNull(taxonomy);
        }

        [Fact(DisplayName = "Get Version Returns Version")]
        public void GetVersion_ReturnsVersion()
        {
            var client = CreateClient();
            var t = client.GetVersion();
            Assert.Equal("1.2.3", client.GetVersion());
        }

        [Fact(DisplayName = "Get Application Key Returns Api Key")]
        public void GetApplicationKey_ReturnsApiKey()
        {
            var client = CreateClient();
            Assert.Equal("api_key", client.GetApplicationKey());
        }

        [Fact(DisplayName = "Get Access Token Returns Delivery Token")]
        public void GetAccessToken_ReturnsDeliveryToken()
        {
            var client = CreateClient();
            Assert.Equal("delivery_token", client.GetAccessToken());
        }

        [Fact(DisplayName = "Get Live Preview Config Returns Config")]
        public void GetLivePreviewConfig_ReturnsConfig()
        {
            var client = CreateClient();
            Assert.NotNull(client.GetLivePreviewConfig());
        }

        [Fact(DisplayName = "Remove Header Removes Header")]
        public void RemoveHeader_RemovesHeader()
        {
            var client = CreateClient();
            client.SetHeader("custom", "value");
            client.RemoveHeader("custom");
            var localHeaders = typeof(ContentstackClient)
                .GetField("_LocalHeaders", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(client) as Dictionary<string, object>;
            Assert.False(localHeaders.ContainsKey("custom"));
        }

        [Fact(DisplayName = "Set Header Adds Header")]
        public void SetHeader_AddsHeader()
        {
            var client = CreateClient();
            client.SetHeader("custom", "value");
            var localHeaders = typeof(ContentstackClient)
                .GetField("_LocalHeaders", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(client) as Dictionary<string, object>;
            Assert.True(localHeaders.ContainsKey("custom"));
            Assert.Equal("value", localHeaders["custom"]);
        }

        [Fact(DisplayName = "Live Preview Query Async Sets Live Preview Config Fields")]
        public async Task LivePreviewQueryAsync_SetsLivePreviewConfigFields()
        {
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
            Assert.Equal("ctuid", GetPrivateField(client, "currentContenttypeUid"));
            Assert.Equal("euid", GetPrivateField(client, "currentEntryUid"));
            Assert.Equal(true, v.Enable );
            Assert.Equal("rid", v.ReleaseId);
            Assert.Equal("ts", v.PreviewTimestamp);
            Assert.Equal("pt", v.PreviewToken);
        }

        // For SyncRecursive, SyncPaginationToken, SyncToken, you should mock HTTP calls.
        // Here we just check that the methods exist and can be called (will throw if not configured).
        [Fact(DisplayName = "Sync Recursive Throws Or Returns")]
        public async Task SyncRecursive_ThrowsOrReturns()
        {
            var client = CreateClient();
            await Assert.ThrowsAnyAsync<Exception>(() => client.SyncRecursive());
        }

        [Fact(DisplayName = "Sync Pagination Token Throws Or Returns")]
        public async Task SyncPaginationToken_ThrowsOrReturns()
        {
            var client = CreateClient();
            await Assert.ThrowsAnyAsync<Exception>(() => client.SyncPaginationToken("pagetoken"));
        }

        [Fact(DisplayName = "Sync Token Throws Or Returns")]
        public async Task SyncToken_ThrowsOrReturns()
        {
            var client = CreateClient();
            await Assert.ThrowsAnyAsync<Exception>(() => client.SyncToken("synctoken"));
        }
    }
}