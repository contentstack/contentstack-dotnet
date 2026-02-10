using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.PerformanceTests
{
    /// <summary>
    /// Comprehensive tests for Performance with Large Datasets
    /// Tests query performance, pagination, and large result handling
    /// </summary>
    [Trait("Category", "PerformanceLargeDatasets")]
    public class PerformanceLargeDatasetsTest : IntegrationTestBase
    {
        public PerformanceLargeDatasetsTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Large Query Results
        
        [Fact(DisplayName = "Performance - Performance Large Limit Handles Efficiently")]
        public async Task Performance_LargeLimit_HandlesEfficiently()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.Limit(100);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Large query should complete within 15s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Multiple Pages Sequential")]
        public async Task Performance_MultiplePages_Sequential()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var startTime = DateTime.Now;
            
            // Act - Fetch 3 pages sequentially
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            for (int i = 0; i < 3; i++)
            {
                var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
                query.Limit(20);
                query.Skip(i * 20);
                await query.Find<Entry>();
            }
            
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            // Assert
            LogAssert("Verifying response");

            Assert.True(elapsed < 20000, $"3 pages should fetch within 20s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Complex Query Large Results")]
        public async Task Performance_ComplexQuery_LargeResults()
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
                query.Exists("title");
                query.Limit(50);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Complex query should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region References with Large Datasets
        
        [Fact(DisplayName = "Performance - Performance References In Large Query Efficient")]
        public async Task Performance_ReferencesInLargeQuery_Efficient()
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
                query.IncludeReference("authors");
                query.Limit(30);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 20000, $"Large query with refs should complete within 20s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Deep References Large Dataset")]
        public async Task Performance_DeepReferences_LargeDataset()
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
                query.IncludeReference(new[] { "authors", "authors.reference" });
                query.Limit(20);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 25000, $"Deep refs with large dataset should complete within 25s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Asset Queries
        
        [Fact(DisplayName = "Performance - Performance Many Assets Query Efficiently")]
        public async Task Performance_ManyAssets_QueryEfficiently()
        {
            // Arrange
            LogArrange("Setting up fetch all operation");

            var client = CreateClient();
            var assetLibrary = client.AssetLibrary();
            
            // Act
            LogAct("Fetching all items");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/assets");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                assetLibrary.Limit(50);
                return await assetLibrary.FetchAll();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Large asset query should complete within 15s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Assets Pagination Sequential")]
        public async Task Performance_AssetsPagination_Sequential()
        {
            // Arrange
            LogArrange("Setting up fetch all operation");

            var client = CreateClient();
            var startTime = DateTime.Now;
            
            // Act - Fetch 3 pages of assets
            LogAct("Fetching all items");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/assets");

            for (int i = 0; i < 3; i++)
            {
                var assetLibrary = client.AssetLibrary();
                assetLibrary.Limit(20);
                assetLibrary.Skip(i * 20);
                await assetLibrary.FetchAll();
            }
            
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            // Assert
            LogAssert("Verifying response");

            Assert.True(elapsed < 20000, $"3 asset pages should fetch within 20s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Complex Operations
        
        [Fact(DisplayName = "Performance - Performance Complex Filters Large Dataset")]
        public async Task Performance_ComplexFilters_LargeDataset()
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
                var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
                var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
                query.And(new List<Query> { sub1, sub2 });
                query.Limit(40);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 20000, $"Complex filters should complete within 20s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Sorting Large Dataset")]
        public async Task Performance_Sorting_LargeDataset()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.Descending("created_at");
                query.Limit(50);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Sorted query should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Concurrent Requests
        
        [Fact(DisplayName = "Performance - Performance Parallel Queries Handle Concurrency")]
        public async Task Performance_ParallelQueries_HandleConcurrency()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);

            var client = CreateClient();
            var startTime = DateTime.Now;
            
            // Act - Execute 3 queries in parallel
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var task1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Limit(10).Find<Entry>();
            var task2 = client.ContentType(TestDataHelper.MediumContentTypeUid).Query().Limit(10).Find<Entry>();
            var task3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Limit(10).Find<Entry>();
            
            await Task.WhenAll(task1, task2, task3);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            // Assert - Parallel should be faster than sequential
            LogAssert("Verifying response");

            Assert.True(elapsed < 15000, $"3 parallel queries should complete within 15s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Parallel Asset Queries Concurrent")]
        public async Task Performance_ParallelAssetQueries_Concurrent()
        {
            // Arrange
            LogArrange("Setting up fetch operation");
            LogContext("AssetUid", TestDataHelper.ImageAssetUid);

            var client = CreateClient();
            var startTime = DateTime.Now;
            
            // Act - Fetch assets in parallel
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/assets/{TestDataHelper.ImageAssetUid}");

            var task1 = client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            var task2 = client.AssetLibrary().Limit(10).FetchAll();
            
            await Task.WhenAll(task1, task2);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            // Assert
            LogAssert("Verifying response");

            Assert.True(elapsed < 10000, $"Parallel asset queries should complete within 10s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Memory and Efficiency
        
        [Fact(DisplayName = "Performance - Performance Large Entry Content Handles Efficiently")]
        public async Task Performance_LargeEntryContent_HandlesEfficiently()
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
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Large entry should fetch within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Projection Reduces Payload Faster")]
        public async Task Performance_ProjectionReducesPayload_Faster()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - With projection should be faster/equal to full fetch
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .Only(new[] { "title", "uid" })
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 8000, $"Projection query should be fast, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Performance - Performance Cached Vs Uncached Consistency")]
        public async Task Performance_CachedVsUncached_Consistency()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act - Fetch same entry twice
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var (entry1, elapsed1) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            var (entry2, elapsed2) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert - Both should complete reasonably
            LogAssert("Verifying response");

            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
            Assert.True(elapsed1 < 10000);
            Assert.True(elapsed2 < 10000);
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

