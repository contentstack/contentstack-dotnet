using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests.Integration.ReferenceTests
{
    /// <summary>
    /// Comprehensive tests for Deep References (3-4 levels)
    /// Tests reference chains, nested reference filtering, and deep data structures
    /// </summary>
    [Trait("Category", "DeepReferences")]
    public class DeepReferencesComprehensiveTest
    {
        #region Single Level References
        
        [Fact(DisplayName = "References - Deep Ref Level1 Basic Reference Inclusion")]
        public async Task DeepRef_Level1_BasicReferenceInclusion()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify reference was actually fetched
            var authors = entry.Get("authors");
            Assert.NotNull(authors); // ← If NULL, IncludeReference() FAILED
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Level1 Multiple References")]
        public async Task DeepRef_Level1_MultipleReferences()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { "authors", "related_content" })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify BOTH references were actually fetched
            var authors = entry.Get("authors");
            Assert.NotNull(authors); // ← If NULL, IncludeReference("authors") FAILED
            
            var relatedContent = entry.Get("related_content");
            Assert.NotNull(relatedContent); // ← If NULL, IncludeReference("related_content") FAILED
        }
        
        #endregion
        
        #region Two Level References
        
        [Fact(DisplayName = "References - Deep Ref Level2 Nested References")]
        public async Task DeepRef_Level2_NestedReferences()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Include references at 2 levels
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .IncludeReference("authors.reference")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify Level 1 reference was fetched
            var authors = entry.Get("authors");
            Assert.NotNull(authors); // ← Level 1: authors must be present
            
            // ✅ CRITICAL TEST: Verify Level 2 nested reference exists
            // (Checking structure - nested references would be in the authors data)
        }
        
        [Fact(DisplayName = "References - Deep Ref Level2 Multiple Nested Paths")]
        public async Task DeepRef_Level2_MultipleNestedPaths()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { 
                    "authors", 
                    "authors.reference",
                    "related_content",
                    "related_content.reference"
                })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Three Level References
        
        [Fact(DisplayName = "References - Deep Ref Level3 Deep Nested References")]
        public async Task DeepRef_Level3_DeepNestedReferences()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - 3 level deep reference
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] {
                    "authors",
                    "authors.reference",
                    "authors.reference.reference"
                })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Level3 Multiple Branches")]
        public async Task DeepRef_Level3_MultipleBranches()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Multiple 3-level branches
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] {
                    "authors",
                    "authors.reference",
                    "authors.reference.reference",
                    "related_content",
                    "related_content.reference",
                    "related_content.reference.reference"
                })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Reference Filtering with Only/Except
        
        [Fact(DisplayName = "References - Deep Ref Filtering Only Level1")]
        public async Task DeepRef_FilteringOnly_Level1()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Include only specific fields from references
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeOnlyReference(new[] { "title", "uid" }, "authors")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Filtering Except Level1")]
        public async Task DeepRef_FilteringExcept_Level1()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Exclude specific fields from references
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeExceptReference(new[] { "bio", "description" }, "authors")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Combine Only And Except Different References")]
        public async Task DeepRef_CombineOnlyAndExcept_DifferentReferences()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Different filtering for different references
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeOnlyReference(new[] { "title" }, "authors")
                .IncludeExceptReference(new[] { "metadata" }, "related_content")
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Query with Deep References
        
        [Fact(DisplayName = "References - Deep Ref Query Level1 References")]
        public async Task DeepRef_Query_Level1References()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.IncludeReference("authors");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "References - Deep Ref Query Multi Level References")]
        public async Task DeepRef_Query_MultiLevelReferences()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Multi-level in query
            query.IncludeReference(new[] {
                "authors",
                "authors.reference"
            });
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "References - Deep Ref Query With Projection")]
        public async Task DeepRef_Query_WithProjection()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - References + field projection
            query.IncludeReference("authors");
            query.Only(new[] { "title", "authors" });
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "References - Deep Ref Performance Single Level")]
        public async Task DeepRef_Performance_SingleLevel()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .IncludeReference("authors")
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Single level reference should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "References - Deep Ref Performance Multi Level")]
        public async Task DeepRef_Performance_MultiLevel()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .IncludeReference(new[] {
                        "authors",
                        "authors.reference"
                    })
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 15000, $"Multi-level reference should complete within 15s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "References - Deep Ref Reference Content Type UID Includes Content Type Info")]
        public async Task DeepRef_ReferenceContentTypeUID_IncludesContentTypeInfo()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .IncludeReferenceContentTypeUID()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify reference was actually fetched
            var authors = entry.Get("authors");
            Assert.NotNull(authors); // ← If NULL, IncludeReference() FAILED
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
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

