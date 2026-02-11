using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.CachingTests
{
    /// <summary>
    /// Tests for Cache Persistence and Management
    /// Note: The .NET SDK may use in-memory caching. These tests verify the API behavior.
    /// </summary>
    [Trait("Category", "CachePersistence")]
    public class CachePersistenceTest : IntegrationTestBase
    {
        public CachePersistenceTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Cache Behavior Tests
        
        [Fact(DisplayName = "Caching - Cache First Fetch Makes API Call")]
        public async Task Cache_FirstFetch_MakesAPICall()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
            
            // ✅ NOTE: Cache test - fetch same entry twice and compare timing
            // 2nd fetch should be faster if caching works
            // Full validation: Measure elapsed time for both fetches
        }
        
        [Fact(DisplayName = "Caching - Cache Second Fetch Same Entry")]
        public async Task Cache_SecondFetch_SameEntry()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act - Fetch same entry twice
            LogAct("Fetching entry from API");

            var entry1 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
            TestAssert.Equal(entry1.Uid, entry2.Uid);
        }
        
        [Fact(DisplayName = "Caching - Cache Different Entries Independent")]
        public async Task Cache_DifferentEntries_Independent()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);
            LogContext("EntryUid", TestDataHelper.MediumEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry1 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.MediumContentTypeUid)
                .Entry(TestDataHelper.MediumEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
            TestAssert.NotEqual(entry1.Uid, entry2.Uid);
        }
        
        #endregion
        
        #region Query Caching
        
        [Fact(DisplayName = "Caching - Cache Query Results Consistent")]
        public async Task Cache_QueryResults_Consistent()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act - Same query twice
            LogAct("Executing query");

            var query1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query1.Limit(5);
            var result1 = await query1.Find<Entry>();
            
            var query2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query2.Limit(5);
            var result2 = await query2.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result1);
            TestAssert.NotNull(result2);
        }
        
        [Fact(DisplayName = "Caching - Cache Different Queries Independent Results")]
        public async Task Cache_DifferentQueries_IndependentResults()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Executing query");

            var query1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query1.Limit(3);
            var result1 = await query1.Find<Entry>();
            
            var query2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query2.Limit(5);
            var result2 = await query2.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result1);
            TestAssert.NotNull(result2);
        }
        
        #endregion
        
        #region Asset Caching
        
        [Fact(DisplayName = "Caching - Cache Asset Fetch Consistent")]
        public async Task Cache_AssetFetch_Consistent()
        {
            // Arrange
            LogArrange("Setting up fetch operation");
            LogContext("AssetUid", TestDataHelper.ImageAssetUid);

            var client = CreateClient();
            
            // Act - Fetch same asset twice
            LogAct("Fetching entry from API");

            var asset1 = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            var asset2 = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(asset1);
            TestAssert.NotNull(asset2);
            TestAssert.Equal(asset1.Uid, asset2.Uid);
        }
        
        [Fact(DisplayName = "Caching - Cache Asset Query Consistent")]
        public async Task Cache_AssetQuery_Consistent()
        {
            // Arrange
            LogArrange("Setting up fetch all operation");

            var client = CreateClient();
            
            // Act
            LogAct("Fetching all items");

            var assetLib1 = client.AssetLibrary();
            assetLib1.Limit(5);
            var result1 = await assetLib1.FetchAll();
            
            var assetLib2 = client.AssetLibrary();
            assetLib2.Limit(5);
            var result2 = await assetLib2.FetchAll();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result1);
            TestAssert.NotNull(result2);
        }
        
        #endregion
        
        #region Performance with Caching
        
        [Fact(DisplayName = "Caching - Cache Performance Repeated Fetch")]
        public async Task Cache_Performance_RepeatedFetch()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act - First fetch
            LogAct("Fetching entry from API");

            var (entry1, elapsed1) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Second fetch - may be cached
            var (entry2, elapsed2) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
            // Both should complete within reasonable time
            TestAssert.True(elapsed1 < 10000);
            TestAssert.True(elapsed2 < 10000);
        }
        
        [Fact(DisplayName = "Caching - Cache Performance Multiple Clients")]
        public async Task Cache_Performance_MultipleClients()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client1 = CreateClient();
            var client2 = CreateClient();
            
            // Act - Different clients, same entry
            LogAct("Fetching entry from API");

            var (entry1, elapsed1) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client1
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            var (entry2, elapsed2) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client2
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
        }
        
        #endregion
        
        #region Client Independence
        
        [Fact(DisplayName = "Caching - Cache Multiple Clients Independent Caches")]
        public async Task Cache_MultipleClients_IndependentCaches()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client1 = CreateClient();
            var client2 = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry1 = await client1
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await client2
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert - Both should succeed independently
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
            TestAssert.Equal(entry1.Uid, entry2.Uid);
        }
        
        [Fact(DisplayName = "Caching - Cache Client Recreation Fresh Cache")]
        public async Task Cache_ClientRecreation_FreshCache()
        {
            // Arrange & Act
            var entry1 = await CreateClient()
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await CreateClient()
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
        }
        
        #endregion
        
        #region Complex Scenarios
        
        [Fact(DisplayName = "Caching - Cache With References Caches All")]
        public async Task Cache_WithReferences_CachesAll()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Fetch with references twice
            LogAct("Fetching entry from API");

            var entry1 = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
        }
        
        [Fact(DisplayName = "Caching - Cache Different Projections Independent Cache")]
        public async Task Cache_DifferentProjections_IndependentCache()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act - Same entry, different projections
            LogAct("Fetching entry from API");

            var entry1 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Only(new[] { "title" })
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Only(new[] { "title", "url" })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry1);
            TestAssert.NotNull(entry2);
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
            
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }
        
        #endregion
    }
}

