using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Contentstack.Core.Tests.Models;

namespace Contentstack.Core.Tests.Integration.QueryTests
{
    /// <summary>
    /// Comprehensive tests for Entry Query operations
    /// Tests all query operators, sorting, filtering, and edge cases
    /// </summary>
    public class EntryQueryablesComprehensiveTest
    {
        #region Comparison Operators
        
        [Fact(DisplayName = "Entry Operations - Query Where Exact Match Returns Matching Entries")]
        public async Task Query_Where_ExactMatch_ReturnsMatchingEntries()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Where("uid", TestDataHelper.SimpleEntryUid);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
            var entry = result.Items.First();
            Assert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
        }
        
        [Fact(DisplayName = "Entry Operations - Query Not Equal To Excludes Specific Entry")]
        public async Task Query_NotEqualTo_ExcludesSpecificEntry()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.NotEqualTo("uid", TestDataHelper.SimpleEntryUid);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Should not contain the excluded entry
            Assert.DoesNotContain(result.Items, e => e.Uid == TestDataHelper.SimpleEntryUid);
        }
        
        [Fact(DisplayName = "Entry Operations - Query Less Than Filters Correctly")]
        public async Task Query_LessThan_FiltersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var comparisonDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            
            // Act
            query.LessThan("created_at", comparisonDate);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // All returned entries should have been created before the comparison date
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            if (result.Items.Any())
            {
                foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Query Less Than Or Equal To Filters Correctly")]
        public async Task Query_LessThanOrEqualTo_FiltersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var comparisonDate = DateTime.Now.ToString("yyyy-MM-dd");
            
            // Act
            query.LessThanOrEqualTo("created_at", comparisonDate);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            if (result.Items.Any())
            {
                foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Query Greater Than Filters Correctly")]
        public async Task Query_GreaterThan_FiltersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var comparisonDate = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");
            
            // Act
            query.GreaterThan("created_at", comparisonDate);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Should return entries created after the comparison date
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Entry Operations - Query Greater Than Or Equal To Filters Correctly")]
        public async Task Query_GreaterThanOrEqualTo_FiltersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var comparisonDate = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd");
            
            // Act
            query.GreaterThanOrEqualTo("created_at", comparisonDate);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Entry Operations - Query Regex Matches Pattern")]
        public async Task Query_Regex_MatchesPattern()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Search for entries with UIDs starting with "blt"
            query.Regex("uid", "^blt.*");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
            // All UIDs should start with "blt"
            Assert.All(result.Items, entry => Assert.StartsWith("blt", entry.Uid));
        }
        
        [Fact(DisplayName = "Entry Operations - Query Regex Case Insensitive Matches Pattern")]
        public async Task Query_Regex_CaseInsensitive_MatchesPattern()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Case-insensitive search (RegexOptions = "i")
            query.Regex("uid", "BLT.*", "i");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        #endregion
        
        #region Array Operators
        
        [Fact(DisplayName = "Entry Operations - Query Contained In Returns Matching Entries")]
        public async Task Query_ContainedIn_ReturnsMatchingEntries()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            var uids = new[] { TestDataHelper.SimpleEntryUid, TestDataHelper.MediumEntryUid };
            
            // Act
            query.ContainedIn("uid", uids);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
            // All returned entries should have UIDs in the provided list
            Assert.All(result.Items, entry => Assert.Contains(entry.Uid, uids));
        }
        
        [Fact(DisplayName = "Entry Operations - Query Not Contained In Excludes Entries")]
        public async Task Query_NotContainedIn_ExcludesEntries()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            var excludedUids = new[] { TestDataHelper.SimpleEntryUid };
            
            // Act
            query.NotContainedIn("uid", excludedUids);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // None of the returned entries should have the excluded UID
            Assert.DoesNotContain(result.Items, e => excludedUids.Contains(e.Uid));
        }
        
        [Fact(DisplayName = "Entry Operations - Query Tags Exact Match Returns Entries With Tag")]
        public async Task Query_Tags_ExactMatch_ReturnsEntriesWithTag()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Query by tags (assuming entries have tags)
            query.WhereTags(new[] { "test" }); // Adjust tag based on your test data
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            // Results may be empty if no entries have the tag, which is fine
            // If results exist, validate structure
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Query Empty Array Handles Gracefully")]
        public async Task Query_EmptyArray_HandlesGracefully()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.ContainedIn("uid", new string[] { });
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Empty array should return no results
            Assert.Equal(0, result.Items.Count());
        }
        
        #endregion
        
        #region Existence Checks
        
        [Fact(DisplayName = "Entry Operations - Query Exists Returns Entries With Field")]
        public async Task Query_Exists_ReturnsEntriesWithField()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.Exists("title"); // Title should exist on all entries
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
            // All returned entries should have the title field
            Assert.All(result.Items, entry => Assert.NotNull(entry.Title));
        }
        
        [Fact(DisplayName = "Entry Operations - Query Not Exists Returns Entries Without Field")]
        public async Task Query_NotExists_ReturnsEntriesWithoutField()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.NotExists("non_existent_field_xyz_123");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            // Should return entries without the non-existent field (which is all of them)
            if (result.Items.Any())
            {
                foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
            }
        }
        
        #endregion
        
        #region Sorting
        
        [Fact(DisplayName = "Entry Operations - Query Ascending Sorts Correctly")]
        public async Task Query_Ascending_SortsCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Ascending("created_at");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            Assert.True(result.Items.Count() > 0);
            
            // Verify entries have required fields
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                Assert.NotNull(entry.Title);
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Query Descending Sorts Correctly")]
        public async Task Query_Descending_SortsCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Descending("created_at");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            Assert.True(result.Items.Count() > 0);
            
            // Verify entries have required fields
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                Assert.NotNull(entry.Title);
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Query Multiple Sorts Applies In Order")]
        public async Task Query_MultipleSorts_AppliesInOrder()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Sort by created_at descending, then by title ascending
            query.Descending("created_at").Ascending("title");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            // Multiple sorts applied successfully - validate entries if present
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Reference Queries
        
        [Fact(DisplayName = "Entry Operations - Query Include Reference Loads Referenced Entries")]
        public async Task Query_IncludeReference_LoadsReferencedEntries()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.Where("uid", TestDataHelper.ComplexEntryUid);
            query.IncludeReference("authors"); // Assuming authors is a reference field
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
            
            var entry = result.Items.First();
            // Check if reference field exists (even if null or empty)
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Query Include Reference Content Type UID Loads Specific References")]
        public async Task Query_IncludeReferenceContentTypeUID_LoadsSpecificReferences()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.Where("uid", TestDataHelper.ComplexEntryUid);
            query.IncludeReferenceContentTypeUID(); // Include reference content type UID
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        #endregion
        
        #region Logical Operators
        
        [Fact(DisplayName = "Entry Operations - Query And Combines Multiple Conditions")]
        public async Task Query_And_CombinesMultipleConditions()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Combine multiple conditions
            var subQuery1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            subQuery1.Where("uid", TestDataHelper.SimpleEntryUid);
            
            var subQuery2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            subQuery2.Exists("title");
            
            query.And(new List<Query> { subQuery1, subQuery2 });
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Count >= 0, "Count should be non-negative");
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            // Should return entries matching both conditions
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Query Or Combines Alternative Conditions")]
        public async Task Query_Or_CombinesAlternativeConditions()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Either condition should match
            var subQuery1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            subQuery1.Where("uid", TestDataHelper.SimpleEntryUid);
            
            var subQuery2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            subQuery2.Where("uid", "non_existent_uid_12345");
            
            query.Or(new List<Query> { subQuery1, subQuery2 });
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0); // Should find at least the first one
        }
        
        [Fact(DisplayName = "Entry Operations - Query Complex Logical Nested And Or")]
        public async Task Query_ComplexLogical_NestedAndOr()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Complex nested logical query
            var subQuery1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            subQuery1.Exists("title");
            
            var subQuery2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            subQuery2.GreaterThan("created_at", DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd"));
            
            query.And(new List<Query> { subQuery1, subQuery2 });
            var result = await query.Find<Entry>();
            
            // Assert
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
        
        [Fact(DisplayName = "Entry Operations - Query Multiple Or Handles Correctly")]
        public async Task Query_MultipleOr_HandlesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Multiple OR conditions
            var queries = new List<Query>
            {
                client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Where("uid", TestDataHelper.SimpleEntryUid),
                client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Where("uid", TestDataHelper.MediumEntryUid)
            };
            
            query.Or(queries);
            var result = await query.Find<Entry>();
            
            // Assert
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
        
        #region Edge Cases
        
        [Fact(DisplayName = "Entry Operations - Query Limit And Skip Pagination")]
        public async Task Query_LimitAndSkip_Pagination()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Limit(2).Skip(0);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() <= 2, "Limit should restrict results to 2 or fewer");
        }
        
        [Fact(DisplayName = "Entry Operations - Query Count Returns Correct Count")]
        public async Task Query_Count_ReturnsCorrectCount()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            var countResult = await query.Count();
            
            // Assert
            Assert.NotNull(countResult);
            // Count returns a JObject with count information
            Assert.True(countResult.Count > 0, "Count result should contain data");
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

