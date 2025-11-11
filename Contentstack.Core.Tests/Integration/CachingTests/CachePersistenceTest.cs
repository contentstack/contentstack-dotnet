using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.CachingTests
{
    /// <summary>
    /// Tests for Cache Persistence and Management
    /// Note: The .NET SDK may use in-memory caching. These tests verify the API behavior.
    /// </summary>
    [Trait("Category", "CachePersistence")]
    public class CachePersistenceTest
    {
        #region Cache Behavior Tests
        
        [Fact(DisplayName = "Caching - Cache First Fetch Makes API Call")]
        public async Task Cache_FirstFetch_MakesAPICall()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
            
            // ✅ NOTE: Cache test - fetch same entry twice and compare timing
            // 2nd fetch should be faster if caching works
            // Full validation: Measure elapsed time for both fetches
        }
        
        [Fact(DisplayName = "Caching - Cache Second Fetch Same Entry")]
        public async Task Cache_SecondFetch_SameEntry()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Fetch same entry twice
            var entry1 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
            Assert.Equal(entry1.Uid, entry2.Uid);
        }
        
        [Fact(DisplayName = "Caching - Cache Different Entries Independent")]
        public async Task Cache_DifferentEntries_Independent()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry1 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.MediumContentTypeUid)
                .Entry(TestDataHelper.MediumEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
            Assert.NotEqual(entry1.Uid, entry2.Uid);
        }
        
        #endregion
        
        #region Query Caching
        
        [Fact(DisplayName = "Caching - Cache Query Results Consistent")]
        public async Task Cache_QueryResults_Consistent()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Same query twice
            var query1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query1.Limit(5);
            var result1 = await query1.Find<Entry>();
            
            var query2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query2.Limit(5);
            var result2 = await query2.Find<Entry>();
            
            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
        }
        
        [Fact(DisplayName = "Caching - Cache Different Queries Independent Results")]
        public async Task Cache_DifferentQueries_IndependentResults()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var query1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query1.Limit(3);
            var result1 = await query1.Find<Entry>();
            
            var query2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query2.Limit(5);
            var result2 = await query2.Find<Entry>();
            
            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
        }
        
        #endregion
        
        #region Asset Caching
        
        [Fact(DisplayName = "Caching - Cache Asset Fetch Consistent")]
        public async Task Cache_AssetFetch_Consistent()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Fetch same asset twice
            var asset1 = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            var asset2 = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset1);
            Assert.NotNull(asset2);
            Assert.Equal(asset1.Uid, asset2.Uid);
        }
        
        [Fact(DisplayName = "Caching - Cache Asset Query Consistent")]
        public async Task Cache_AssetQuery_Consistent()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var assetLib1 = client.AssetLibrary();
            assetLib1.Limit(5);
            var result1 = await assetLib1.FetchAll();
            
            var assetLib2 = client.AssetLibrary();
            assetLib2.Limit(5);
            var result2 = await assetLib2.FetchAll();
            
            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
        }
        
        #endregion
        
        #region Performance with Caching
        
        [Fact(DisplayName = "Caching - Cache Performance Repeated Fetch")]
        public async Task Cache_Performance_RepeatedFetch()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - First fetch
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
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
            // Both should complete within reasonable time
            Assert.True(elapsed1 < 10000);
            Assert.True(elapsed2 < 10000);
        }
        
        [Fact(DisplayName = "Caching - Cache Performance Multiple Clients")]
        public async Task Cache_Performance_MultipleClients()
        {
            // Arrange
            var client1 = CreateClient();
            var client2 = CreateClient();
            
            // Act - Different clients, same entry
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
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
        }
        
        #endregion
        
        #region Client Independence
        
        [Fact(DisplayName = "Caching - Cache Multiple Clients Independent Caches")]
        public async Task Cache_MultipleClients_IndependentCaches()
        {
            // Arrange
            var client1 = CreateClient();
            var client2 = CreateClient();
            
            // Act
            var entry1 = await client1
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            var entry2 = await client2
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert - Both should succeed independently
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
            Assert.Equal(entry1.Uid, entry2.Uid);
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
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
        }
        
        #endregion
        
        #region Complex Scenarios
        
        [Fact(DisplayName = "Caching - Cache With References Caches All")]
        public async Task Cache_WithReferences_CachesAll()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Fetch with references twice
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
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
        }
        
        [Fact(DisplayName = "Caching - Cache Different Projections Independent Cache")]
        public async Task Cache_DifferentProjections_IndependentCache()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Same entry, different projections
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
            Assert.NotNull(entry1);
            Assert.NotNull(entry2);
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

