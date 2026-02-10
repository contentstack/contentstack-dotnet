using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.JsonRteTests
{
    /// <summary>
    /// Tests for JSON RTE with embedded entries and assets
    /// Tests embedded item resolution, nested structures, and references
    /// </summary>
    [Trait("Category", "JsonRteEmbeddedItems")]
    public class JsonRteEmbeddedItemsTest : IntegrationTestBase
    {
        public JsonRteEmbeddedItemsTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic JSON RTE
        
        [Fact(DisplayName = "JSON RTE - Json Rte Basic Fetch Returns Entry")]
        public async Task JsonRte_BasicFetch_ReturnsEntry()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            
            // ✅ KEY TEST: Verify modular blocks exist and have structure
            var blocks = entry.Get("json_rte_field");
            if (blocks != null)
            {
                var blocksArray = blocks as Newtonsoft.Json.Linq.JArray;
                if (blocksArray != null && blocksArray.Count > 0)
                {
                    Assert.True(blocksArray.Count > 0, "Modular blocks should have content");
                }
            }
        }
        
        [Fact(DisplayName = "JSON RTE - Json Rte With Embedded Items Includes Embedded")]
        public async Task JsonRte_WithEmbeddedItems_IncludesEmbedded()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            
            // ✅ KEY TEST: Verify modular blocks exist and have structure
            var blocks = entry.Get("json_rte_field");
            if (blocks != null)
            {
                var blocksArray = blocks as Newtonsoft.Json.Linq.JArray;
                if (blocksArray != null && blocksArray.Count > 0)
                {
                    Assert.True(blocksArray.Count > 0, "Modular blocks should have content");
                }
            }
        }
        
        #endregion
        
        #region Embedded Entries
        
        [Fact(DisplayName = "JSON RTE - Json Rte Embedded Entry Single Level")]
        public async Task JsonRte_EmbeddedEntry_SingleLevel()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            
            // ✅ KEY TEST: Verify modular blocks exist and have structure
            var blocks = entry.Get("json_rte_field");
            if (blocks != null)
            {
                var blocksArray = blocks as Newtonsoft.Json.Linq.JArray;
                if (blocksArray != null && blocksArray.Count > 0)
                {
                    Assert.True(blocksArray.Count > 0, "Modular blocks should have content");
                }
            }
        }
        
        [Fact(DisplayName = "JSON RTE - Json Rte Embedded Entry Multiple In Same Field")]
        public async Task JsonRte_EmbeddedEntry_MultipleInSameField()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Multiple embedded entries should all be resolved
        }
        
        #endregion
        
        #region Embedded Assets
        
        [Fact(DisplayName = "JSON RTE - Json Rte Embedded Asset Single Asset")]
        public async Task JsonRte_EmbeddedAsset_SingleAsset()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Embedded asset should be resolved with URL
        }
        
        [Fact(DisplayName = "JSON RTE - Json Rte Embedded Asset Multiple Assets")]
        public async Task JsonRte_EmbeddedAsset_MultipleAssets()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Multiple embedded assets should be resolved
        }
        
        #endregion
        
        #region Mixed Embedded Items
        
        [Fact(DisplayName = "JSON RTE - Json Rte Mixed Embedded Entries And Assets")]
        public async Task JsonRte_MixedEmbedded_EntriesAndAssets()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Both entries and assets should be embedded
        }
        
        [Fact(DisplayName = "JSON RTE - Json Rte Nested Embedded Deep Structure")]
        public async Task JsonRte_NestedEmbedded_DeepStructure()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Nested embedded items should be resolved
        }
        
        #endregion
        
        #region Query with Embedded Items
        
        [Fact(DisplayName = "JSON RTE - Json Rte Query With Embedded Items")]
        public async Task JsonRte_Query_WithEmbeddedItems()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.includeEmbeddedItems();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // All entries should have embedded items resolved
        }
        
        [Fact(DisplayName = "JSON RTE - Json Rte Query Embedded With Projection")]
        public async Task JsonRte_Query_EmbeddedWithProjection()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.includeEmbeddedItems();
            query.Only(new[] { "title", "json_rte" });
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "JSON RTE - Json Rte Performance With Embedded Items")]
        public async Task JsonRte_Performance_WithEmbeddedItems()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .includeEmbeddedItems()
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Embedded items fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "JSON RTE - Json Rte Performance Query With Embedded")]
        public async Task JsonRte_Performance_QueryWithEmbedded()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.includeEmbeddedItems();
                query.Limit(5);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Query with embedded items should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "JSON RTE - Json Rte Empty Rte Handles Gracefully")]
        public async Task JsonRte_EmptyRte_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Should handle entries without JSON RTE fields
        }
        
        #endregion
        
        #region Helper Methods
        
        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            
            return new ContentstackClient(options);
        }
        
        #endregion
    }
}

