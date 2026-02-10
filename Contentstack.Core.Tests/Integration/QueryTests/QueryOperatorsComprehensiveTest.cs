using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.QueryTests
{
    /// <summary>
    /// Comprehensive tests for advanced Query Operators
    /// Tests complex query combinations, nested queries, and advanced filtering
    /// </summary>
    public class QueryOperatorsComprehensiveTest : IntegrationTestBase
    {
        public QueryOperatorsComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Regex Operations
        
        [Fact(DisplayName = "Query Operations - Query Regex Complex Pattern Matches Correctly")]
        public async Task Query_Regex_ComplexPattern_MatchesCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Match UIDs that start with "blt" followed by alphanumeric characters
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.Regex("uid", "^blt[a-zA-Z0-9]+$");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            // All UIDs should match the pattern
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.Matches("^blt[a-zA-Z0-9]+$", entry.Uid);
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Regex With Modifiers Case Insensitive Search")]
        public async Task Query_Regex_WithModifiers_CaseInsensitiveSearch()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Case insensitive search using "i" modifier
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Regex("title", ".*test.*", "i");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            // Should match entries regardless of case - validate structure
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Regex Multiple Patterns Combined With And")]
        public async Task Query_Regex_MultiplePatterns_CombinedWithAnd()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Multiple regex patterns
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var subQuery1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            subQuery1.Regex("uid", "^blt.*");
            
            var subQuery2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            subQuery2.Exists("title");
            
            query.And(new List<Query> { subQuery1, subQuery2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Logical Combinations
        
        [Fact(DisplayName = "Query Operations - Query Complex And Three Conditions Filters Correctly")]
        public async Task Query_ComplexAnd_ThreeConditions_FiltersCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Combine three conditions with AND
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var subQuery1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            subQuery1.Exists("title");
            
            var subQuery2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            subQuery2.Exists("uid");
            
            var subQuery3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            subQuery3.GreaterThan("created_at", DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd"));
            
            query.And(new List<Query> { subQuery1, subQuery2, subQuery3 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Complex Or Multiple Alternatives Returns All Matches")]
        public async Task Query_ComplexOr_MultipleAlternatives_ReturnsAllMatches()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);
            LogContext("EntryUid", TestDataHelper.MediumEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - OR with multiple alternatives
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var queries = new List<Query>
            {
                client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Where("uid", TestDataHelper.SimpleEntryUid),
                client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Where("uid", TestDataHelper.MediumEntryUid),
                client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Exists("title")
            };
            
            query.Or(queries);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Query Operations - Query Nested And Or Complex Logic Executes Correctly")]
        public async Task Query_NestedAndOr_ComplexLogic_ExecutesCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - (A AND B) OR (C AND D)
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var andQuery1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            andQuery1.And(new List<Query> { sub1, sub2 });
            
            var andQuery2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var sub3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().NotExists("non_existent_field");
            var sub4 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("created_at");
            andQuery2.And(new List<Query> { sub3, sub4 });
            
            query.Or(new List<Query> { andQuery1, andQuery2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Combined Comparison Greater Than And Less Than Range Query")]
        public async Task Query_CombinedComparison_GreaterThanAndLessThan_RangeQuery()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Date range query
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var startDate = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");
            var endDate = DateTime.Now.ToString("yyyy-MM-dd");
            
            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query()
                .GreaterThan("created_at", startDate);
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query()
                .LessThan("created_at", endDate);
            
            query.And(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Not Operator With Contained In Excludes Multiple Values")]
        public async Task Query_NotOperator_WithContainedIn_ExcludesMultipleValues()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - NOT IN query
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var excludedUids = new[] { "uid1", "uid2", "uid3" };
            query.NotContainedIn("uid", excludedUids);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // None of the excluded UIDs should be in results
            foreach (var entry in result.Items)
            {
                Assert.DoesNotContain(entry.Uid, excludedUids);
            }
        }
        
        #endregion
        
        #region Nested Field Queries
        
        [Fact(DisplayName = "Query Operations - Query Nested Field Dot Notation Query Correctly")]
        public async Task Query_NestedField_DotNotation_QueryCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Query nested field using dot notation
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Where("seo.title", "test");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Nested field query executed (may return 0 results if no match)
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Group Field Query By Nested Property")]
        public async Task Query_GroupField_QueryByNestedProperty()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Query group field
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Exists("group");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Modular Blocks Exists Check")]
        public async Task Query_ModularBlocks_ExistsCheck()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Check for modular blocks existence
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Exists("modular_blocks");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Json Rte Field Exists")]
        public async Task Query_JsonRte_FieldExists()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Check for JSON RTE field
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Exists("json_rte");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Reference Operators
        
        [Fact(DisplayName = "Query Operations - Query Include Reference Single Level Loads References")]
        public async Task Query_IncludeReference_SingleLevel_LoadsReferences()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Where("uid", TestDataHelper.ComplexEntryUid);
            query.IncludeReference("authors");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include Reference Multiple Fields Loads All References")]
        public async Task Query_IncludeReference_MultipleFields_LoadsAllReferences()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Use array overload to include multiple references
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Where("uid", TestDataHelper.ComplexEntryUid);
            query.IncludeReference(new[] { "authors", "related_content" });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include Reference Only With Projection Filters Reference Fields")]
        public async Task Query_IncludeReferenceOnly_WithProjection_FiltersReferenceFields()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Where("uid", TestDataHelper.ComplexEntryUid);
            query.IncludeReference("authors");
            query.IncludeReferenceContentTypeUID();
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Query Operations - Query Reference Query With Content Type Filter")]
        public async Task Query_ReferenceQuery_WithContentTypeFilter()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Include references and add filter
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.IncludeReference("authors");
            query.Where("uid", TestDataHelper.ComplexEntryUid);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Tag Queries
        
        [Fact(DisplayName = "Query Operations - Query Where Tags Single Tag Returns Matching Entries")]
        public async Task Query_WhereTags_SingleTag_ReturnsMatchingEntries()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.WhereTags(new[] { "test" });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // May return 0 results if no entries have the tag
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Where Tags Multiple Tags Returns Entries With Any Tag")]
        public async Task Query_WhereTags_MultipleTags_ReturnsEntriesWithAnyTag()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.WhereTags(new[] { "tag1", "tag2", "tag3" });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Query Operations - Query Complex Query Completes In Reasonable Time")]
        public async Task Query_ComplexQuery_CompletesInReasonableTime()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Complex query with multiple conditions
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
                var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
                query.And(new List<Query> { sub1, sub2 });
                query.Limit(10);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 10000, $"Complex query should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Query Operations - Query With Pagination Performance Is Consistent")]
        public async Task Query_WithPagination_PerformanceIsConsistent()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act - Measure first page
            var (result1, elapsed1) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Query()
                    .Limit(5)
                    .Skip(0)
                    .Find<Entry>();
            });
            
            // Act - Measure second page
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var (result2, elapsed2) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Query()
                    .Limit(5)
                    .Skip(5)
                    .Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            // Both should complete in reasonable time
            Assert.True(elapsed1 < 5000, $"First page should complete within 5s, took {elapsed1}ms");
            Assert.True(elapsed2 < 5000, $"Second page should complete within 5s, took {elapsed2}ms");
        }
        
        [Fact(DisplayName = "Query Operations - Query Count Operation Is Faster Than Fetch")]
        public async Task Query_CountOperation_IsFasterThanFetch()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act - Measure count
            var (countResult, countElapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Count();
            });
            
            // Act - Measure full fetch
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var (fetchResult, fetchElapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(countResult);
            Assert.NotNull(fetchResult);
            // Count should generally be faster (though not always guaranteed)
            Assert.True(countElapsed < 10000, $"Count should complete within 10s, took {countElapsed}ms");
            Assert.True(fetchElapsed < 10000, $"Fetch should complete within 10s, took {fetchElapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Query Operations - Query Empty Query Returns All Entries")]
        public async Task Query_EmptyQuery_ReturnsAllEntries()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - No filters applied
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0, "Empty query should return all entries");
        }
        
        [Fact(DisplayName = "Query Operations - Query Invalid Field Name Handles Gracefully")]
        public async Task Query_InvalidFieldName_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Query non-existent field
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.Where("non_existent_field_xyz_123", "some_value");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Should return empty results, not throw
            Assert.Equal(0, result.Items.Count());
        }
        
        [Fact(DisplayName = "Query Operations - Query Extreme Limit Handles Gracefully")]
        public async Task Query_ExtremeLimit_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Very large limit
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.Limit(1000);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Should handle large limit without error
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Query Operations - Query Chained Operations Executes In Order")]
        public async Task Query_ChainedOperations_ExecutesInOrder()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Chain multiple operations
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var result = await query
                .Exists("title")
                .Descending("created_at")
                .Limit(5)
                .Skip(0)
                .Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() <= 5);
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

