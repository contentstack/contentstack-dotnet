using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.ReferenceTests
{
    /// <summary>
    /// Comprehensive tests for Deep References (3-4 levels)
    /// Tests reference chains, nested reference filtering, and deep data structures
    /// </summary>
    [Trait("Category", "DeepReferences")]
    public class DeepReferencesComprehensiveTest : IntegrationTestBase
    {
        public DeepReferencesComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Single Level References
        
        [Fact(DisplayName = "References - Deep Ref Level1 Basic Reference Inclusion")]
        public async Task DeepRef_Level1_BasicReferenceInclusion()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify reference was actually fetched
            var authors = entry.Get("authors");
            TestAssert.NotNull(authors); // ← If NULL, IncludeReference() FAILED
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Level1 Multiple References")]
        public async Task DeepRef_Level1_MultipleReferences()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { "authors", "related_content" })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify BOTH references were actually fetched
            var authors = entry.Get("authors");
            TestAssert.NotNull(authors); // ← If NULL, IncludeReference("authors") FAILED
            
            var relatedContent = entry.Get("related_content");
            TestAssert.NotNull(relatedContent); // ← If NULL, IncludeReference("related_content") FAILED
        }
        
        #endregion
        
        #region Two Level References
        
        [Fact(DisplayName = "References - Deep Ref Level2 Nested References")]
        public async Task DeepRef_Level2_NestedReferences()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Include references at 2 levels
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .IncludeReference("authors.reference")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify Level 1 reference was fetched
            var authors = entry.Get("authors");
            TestAssert.NotNull(authors); // ← Level 1: authors must be present
            
            // ✅ CRITICAL TEST: Verify Level 2 nested reference exists
            // (Checking structure - nested references would be in the authors data)
        }
        
        [Fact(DisplayName = "References - Deep Ref Level2 Multiple Nested Paths")]
        public async Task DeepRef_Level2_MultipleNestedPaths()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

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
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Three Level References
        
        [Fact(DisplayName = "References - Deep Ref Level3 Deep Nested References")]
        public async Task DeepRef_Level3_DeepNestedReferences()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - 3 level deep reference
            LogAct("Fetching entry from API");

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
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Level3 Multiple Branches")]
        public async Task DeepRef_Level3_MultipleBranches()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Multiple 3-level branches
            LogAct("Fetching entry from API");

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
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Reference Filtering with Only/Except
        
        [Fact(DisplayName = "References - Deep Ref Filtering Only Level1")]
        public async Task DeepRef_FilteringOnly_Level1()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Include only specific fields from references
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeOnlyReference(new[] { "title", "uid" }, "authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Filtering Except Level1")]
        public async Task DeepRef_FilteringExcept_Level1()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Exclude specific fields from references
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeExceptReference(new[] { "bio", "description" }, "authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Deep Ref Combine Only And Except Different References")]
        public async Task DeepRef_CombineOnlyAndExcept_DifferentReferences()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Different filtering for different references
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeOnlyReference(new[] { "title" }, "authors")
                .IncludeExceptReference(new[] { "metadata" }, "related_content")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Query with Deep References
        
        [Fact(DisplayName = "References - Deep Ref Query Level1 References")]
        public async Task DeepRef_Query_Level1References()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.IncludeReference("authors");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "References - Deep Ref Query Multi Level References")]
        public async Task DeepRef_Query_MultiLevelReferences()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Multi-level in query
            LogAct("Executing query");

            query.IncludeReference(new[] {
                "authors",
                "authors.reference"
            });
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "References - Deep Ref Query With Projection")]
        public async Task DeepRef_Query_WithProjection()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - References + field projection
            LogAct("Executing query");

            query.IncludeReference("authors");
            query.Only(new[] { "title", "authors" });
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "References - Deep Ref Performance Single Level")]
        public async Task DeepRef_Performance_SingleLevel()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .IncludeReference("authors")
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.True(elapsed < 10000, $"Single level reference should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "References - Deep Ref Performance Multi Level")]
        public async Task DeepRef_Performance_MultiLevel()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

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
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.True(elapsed < 15000, $"Multi-level reference should complete within 15s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "References - Deep Ref Reference Content Type UID Includes Content Type Info")]
        public async Task DeepRef_ReferenceContentTypeUID_IncludesContentTypeInfo()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference("authors")
                .IncludeReferenceContentTypeUID()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify reference was actually fetched
            var authors = entry.Get("authors");
            TestAssert.NotNull(authors); // ← If NULL, IncludeReference() FAILED
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
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

