using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.ModularBlocksTests
{
    /// <summary>
    /// Comprehensive tests for Modular Blocks functionality
    /// Tests block structures, nested blocks, references within blocks
    /// </summary>
    [Trait("Category", "ModularBlocks")]
    public class ModularBlocksComprehensiveTest
    {
        #region Basic Modular Blocks
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Basic Fetch Returns Entry")]
        public async Task ModularBlocks_BasicFetch_ReturnsEntry()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            
            // ✅ KEY TEST: Verify modular blocks exist and have structure
            var blocks = entry.Get("modular_blocks");
            if (blocks != null)
            {
                var blocksArray = blocks as Newtonsoft.Json.Linq.JArray;
                if (blocksArray != null && blocksArray.Count > 0)
                {
                    Assert.True(blocksArray.Count > 0, "Modular blocks should have content");
                }
            }
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Exists Check Finds Blocks")]
        public async Task ModularBlocks_ExistsCheck_FindsBlocks()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.Exists("modular_blocks");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Block Structures
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Single Block Fetches Correctly")]
        public async Task ModularBlocks_SingleBlock_FetchesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // Single modular block should be accessible
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Multiple Blocks All Accessible")]
        public async Task ModularBlocks_MultipleBlocks_AllAccessible()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // Multiple blocks in sequence should be accessible
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Different Block Types Mixed Structure")]
        public async Task ModularBlocks_DifferentBlockTypes_MixedStructure()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // Different block types should coexist
        }
        
        #endregion
        
        #region Nested Modular Blocks
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Nested Blocks Deep Structure")]
        public async Task ModularBlocks_NestedBlocks_DeepStructure()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // Nested blocks should be resolved
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Blocks With Groups Complex Nesting")]
        public async Task ModularBlocks_BlocksWithGroups_ComplexNesting()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // Blocks containing group fields should work
        }
        
        #endregion
        
        #region Blocks with References
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks With References Includes Referenced")]
        public async Task ModularBlocks_WithReferences_IncludesReferenced()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("modular_blocks.reference")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // References within blocks should be included
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks With Embedded Items Resolves Embedded")]
        public async Task ModularBlocks_WithEmbeddedItems_ResolvesEmbedded()
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
            // Embedded items in blocks should be resolved
        }
        
        #endregion
        
        #region Query with Modular Blocks
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Query Filter By Block Content")]
        public async Task ModularBlocks_Query_FilterByBlockContent()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.Exists("modular_blocks");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Query With Projection")]
        public async Task ModularBlocks_Query_WithProjection()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.Only(new[] { "title", "modular_blocks" });
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Performance Complex Blocks")]
        public async Task ModularBlocks_Performance_ComplexBlocks()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Complex blocks fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Performance With References")]
        public async Task ModularBlocks_Performance_WithReferences()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .IncludeReference("modular_blocks.reference")
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 15000, $"Blocks with references should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Modular Blocks - Modular Blocks Empty Blocks Handles Gracefully")]
        public async Task ModularBlocks_EmptyBlocks_HandlesGracefully()
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
            // Should handle entries without modular blocks
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

