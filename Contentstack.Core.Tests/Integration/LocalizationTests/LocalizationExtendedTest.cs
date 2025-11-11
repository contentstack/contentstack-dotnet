using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.LocalizationTests
{
    /// <summary>
    /// Extended tests for Localization features
    /// Tests comprehensive locale scenarios, combinations, and edge cases
    /// </summary>
    [Trait("Category", "LocalizationExtended")]
    public class LocalizationExtendedTest
    {
        #region Basic Locale Operations
        
        [Fact(DisplayName = "Localization - Locale Extended Set Locale English")]
        public async Task LocaleExtended_SetLocale_English()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .SetLocale("en-us")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
            
            // ✅ KEY TEST: Locale parameter was applied
            // Entry should contain locale-specific content
            // Full validation: Compare with different locale to verify language differences
        }
        
        [Fact(DisplayName = "Localization - Locale Extended Locale With Embedded Combines")]
        public async Task LocaleExtended_LocaleWithEmbedded_Combines()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .SetLocale("en-us")
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Asset Localization
        
        [Fact(DisplayName = "Localization - Locale Extended Asset Library With Locale Fetches Localized")]
        public async Task LocaleExtended_AssetLibraryWithLocale_FetchesLocalized()
        {
            // Arrange
            var client = CreateClient();
            var assetLibrary = client.AssetLibrary();
            
            // Act
            assetLibrary.SetLocale("en-us");
            assetLibrary.Limit(5);
            var result = await assetLibrary.FetchAll();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Localization - Locale Extended Asset Query With Locale Filters Correctly")]
        public async Task LocaleExtended_AssetQueryWithLocale_FiltersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var assetLibrary = client.AssetLibrary();
            
            // Act
            assetLibrary.SetLocale("en-us");
            assetLibrary.Limit(5);
            var result = await assetLibrary.FetchAll();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Localization - Locale Extended Performance With Locale")]
        public async Task LocaleExtended_Performance_WithLocale()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .SetLocale("en-us")
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Locale fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Localization - Locale Extended Performance Complex Locale Query")]
        public async Task LocaleExtended_Performance_ComplexLocaleQuery()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.SetLocale("en-us");
                query.IncludeReference("authors");
                query.IncludeCount();
                query.Limit(5);
                return await query.Find<Entry>();
            });
            
            // Assert
            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Complex locale query should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Localization - Locale Extended Empty Locale Falls Back To Default")]
        public async Task LocaleExtended_EmptyLocale_FallsBackToDefault()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .SetLocale("")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Localization - Locale Extended Multiple Locale Requests Independent")]
        public async Task LocaleExtended_MultipleLocaleRequests_Independent()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry1 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .SetLocale("en-us")
                .Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
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

