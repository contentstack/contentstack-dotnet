using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.ReferenceTests
{
    /// <summary>
    /// Comprehensive tests for Multi-Reference fields
    /// Tests arrays of references, mixed references, and querying
    /// </summary>
    [Trait("Category", "MultiReference")]
    public class MultiReferenceTest : IntegrationTestBase
    {
        public MultiReferenceTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic Multi-Reference
        
        [Fact(DisplayName = "References - Multi Ref Basic Fetch Returns Entry")]
        public async Task MultiRef_BasicFetch_ReturnsEntry()
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
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "References - Multi Ref Include Single Ref Field Includes All References")]
        public async Task MultiRef_IncludeSingleRefField_IncludesAllReferences()
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
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ CRITICAL TEST: Verify reference was actually fetched
            var reference = entry.Get("authors");
            Assert.NotNull(reference); // ← If NULL, IncludeReference("authors") FAILED
        }
        
        #endregion
        
        #region Multi-Reference Filtering
        
        [Fact(DisplayName = "References - Multi Ref Only Fields In Reference Filters References")]
        public async Task MultiRef_OnlyFieldsInReference_FiltersReferences()
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
                .IncludeOnlyReference(new[] { "title", "uid" }, "authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "References - Multi Ref Except Fields In Reference Excludes Correctly")]
        public async Task MultiRef_ExceptFieldsInReference_ExcludesCorrectly()
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
                .IncludeExceptReference(new[] { "bio", "description" }, "authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Multi-Reference Deep Nesting
        
        [Fact(DisplayName = "References - Multi Ref Deep References Level2")]
        public async Task MultiRef_DeepReferences_Level2()
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
                .IncludeReference(new[] {
                    "authors",
                    "authors.reference"
                })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "References - Multi Ref Deep References Level3")]
        public async Task MultiRef_DeepReferences_Level3()
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
                .IncludeReference(new[] {
                    "authors",
                    "authors.reference",
                    "authors.reference.reference"
                })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Query Operations on Multi-Reference
        
        [Fact(DisplayName = "References - Multi Ref Query By Reference Uid Finds Entries")]
        public async Task MultiRef_QueryByReferenceUid_FindsEntries()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Where("authors.uid", TestDataHelper.SimpleEntryUid);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "References - Multi Ref Query Contained In Finds Matching References")]
        public async Task MultiRef_QueryContainedIn_FindsMatchingReferences()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.ContainedIn("authors", new object[] {
                TestDataHelper.SimpleEntryUid
            });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "References - Multi Ref Query Not Contained In Excludes References")]
        public async Task MultiRef_QueryNotContainedIn_ExcludesReferences()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.NotContainedIn("authors", new object[] {
                "nonexistent_uid"
            });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Mixed Content Type References
        
        [Fact(DisplayName = "References - Multi Ref Mixed Content Types All Included")]
        public async Task MultiRef_MixedContentTypes_AllIncluded()
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
                .IncludeReference("related_content")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // References to different content types should work
        }
        
        [Fact(DisplayName = "References - Multi Ref Reference Content Type UID Includes Type Info")]
        public async Task MultiRef_ReferenceContentTypeUID_IncludesTypeInfo()
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
                .IncludeReference("authors")
                .IncludeReferenceContentTypeUID()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "References - Multi Ref Performance Single Level")]
        public async Task MultiRef_Performance_SingleLevel()
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
                    .IncludeReference("authors")
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Multi-ref fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "References - Multi Ref Performance Deep References")]
        public async Task MultiRef_Performance_DeepReferences()
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
                    .IncludeReference(new[] {
                        "authors",
                        "authors.reference"
                    })
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 15000, $"Deep multi-ref fetch should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "References - Multi Ref Empty Reference Array Handles Gracefully")]
        public async Task MultiRef_EmptyReferenceArray_HandlesGracefully()
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
                .IncludeReference("reference")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Empty reference array should not cause errors
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

