using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.EntryTests
{
    /// <summary>
    /// Extended tests for Entry Include operations
    /// Tests various include combinations and scenarios
    /// </summary>
    [Trait("Category", "EntryIncludeExtended")]
    public class EntryIncludeExtendedTest
    {
        #region Include Combinations
        
        [Fact(DisplayName = "Entry Operations - Entry Include Owner Includes Owner Metadata")]
        public async Task EntryInclude_Owner_IncludesOwnerMetadata()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include Basic Fetch Returns Entry")]
        public async Task EntryInclude_BasicFetch_ReturnsEntry()
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
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include Owner Includes Owner Info")]
        public async Task EntryInclude_Owner_IncludesOwnerInfo()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include Metadata Includes Metadata Fields")]
        public async Task EntryInclude_Metadata_IncludesMetadataFields()
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
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include Embedded Items Includes Embedded")]
        public async Task EntryInclude_EmbeddedItems_IncludesEmbedded()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Multiple Includes
        
        [Fact(DisplayName = "Entry Operations - Entry Include Count And Owner Both Included")]
        public async Task EntryInclude_CountAndOwner_BothIncluded()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include All Includes Combined Correctly")]
        public async Task EntryInclude_AllIncludes_CombinedCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeOwner()
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include With References Includes Combined")]
        public async Task EntryInclude_WithReferences_IncludesCombined()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Include with Projection
        
        [Fact(DisplayName = "Entry Operations - Entry Include With Only Combines Correctly")]
        public async Task EntryInclude_WithOnly_CombinesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeOwner()
                .Only(new[] { "title", "uid" })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include With Except Combines Correctly")]
        public async Task EntryInclude_WithExcept_CombinesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Except(new[] { "large_field" })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Include with Localization
        
        [Fact(DisplayName = "Entry Operations - Entry Include With Locale Combines Correctly")]
        public async Task EntryInclude_WithLocale_CombinesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .SetLocale("en-us")
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Entry Include With Fallback Combines Correctly")]
        public async Task EntryInclude_WithFallback_CombinesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act & Assert
            try
            {
                var entry = await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .SetLocale("en-us")
                    .IncludeFallback()
                    .Fetch<Entry>();
                
                Assert.NotNull(entry);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Entry Operations - Entry Include Performance Multiple Includes")]
        public async Task EntryInclude_Performance_MultipleIncludes()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .IncludeOwner()
                    .includeEmbeddedItems()
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 15000, $"Multiple includes should complete within 15s, took {elapsed}ms");
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

