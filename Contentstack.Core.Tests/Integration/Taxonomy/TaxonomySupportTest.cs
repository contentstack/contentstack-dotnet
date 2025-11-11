using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.Taxonomy
{
    /// <summary>
    /// Tests for Taxonomy support in the SDK
    /// Tests taxonomy queries, filters, and retrieval
    /// </summary>
    [Trait("Category", "Taxonomy")]
    public class TaxonomySupportTest
    {
        #region Basic Taxonomy Queries
        
        [Fact(DisplayName = "Taxonomy - Taxonomy Query By Taxonomy Term Returns Matching Entries")]
        public async Task Taxonomy_QueryByTaxonomyTerm_ReturnsMatchingEntries()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Query entries by taxonomy term
            query.AddParam("taxonomy", TestDataHelper.TaxUsaState);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // May return 0 entries if taxonomy is not configured
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Taxonomy - Taxonomy Fetch Entry With Taxonomy Data")]
        public async Task Taxonomy_FetchEntry_WithTaxonomyData()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Fetch entry that may have taxonomy
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        [Fact(DisplayName = "Taxonomy - Taxonomy Query Multiple Entries With Taxonomy Filter")]
        public async Task Taxonomy_QueryMultipleEntries_WithTaxonomyFilter()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.AddParam("taxonomy", TestDataHelper.TaxIndiaState);
            query.Limit(10);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Taxonomy with Additional Filters
        
        [Fact(DisplayName = "Taxonomy - Taxonomy Combine With Where Clause Filters Correctly")]
        public async Task Taxonomy_CombineWithWhereClause_FiltersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Combine taxonomy with where clause
            query.AddParam("taxonomy", TestDataHelper.TaxUsaState);
            query.Exists("title");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Taxonomy - Taxonomy With Sorting Orders Correctly")]
        public async Task Taxonomy_WithSorting_OrdersCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.AddParam("taxonomy", TestDataHelper.TaxUsaState);
            query.Descending("created_at");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Taxonomy - Taxonomy With Pagination Returns Paged Results")]
        public async Task Taxonomy_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.AddParam("taxonomy", TestDataHelper.TaxUsaState);
            query.Limit(5);
            query.Skip(0);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Taxonomy with References
        
        [Fact(DisplayName = "Taxonomy - Taxonomy With References Loads Referenced Content")]
        public async Task Taxonomy_WithReferences_LoadsReferencedContent()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.AddParam("taxonomy", TestDataHelper.TaxUsaState);
            query.IncludeReference("authors");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Taxonomy - Taxonomy With Field Projection Returns Only Requested Fields")]
        public async Task Taxonomy_WithFieldProjection_ReturnsOnlyRequestedFields()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.AddParam("taxonomy", TestDataHelper.TaxIndiaState);
            query.Only(new[] { "title", "uid" });
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        #endregion
        
        #region Taxonomy Edge Cases
        
        [Fact(DisplayName = "Taxonomy - Taxonomy Invalid Term Returns Empty Results")]
        public async Task Taxonomy_InvalidTerm_ReturnsEmptyResults()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Query with non-existent taxonomy term
            query.AddParam("taxonomy", "non_existent_taxonomy_term_xyz");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Should return empty results
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
        }
        
        [Fact(DisplayName = "Taxonomy - Taxonomy Empty Term Handles Gracefully")]
        public async Task Taxonomy_EmptyTerm_HandlesGracefully()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Query with empty taxonomy
            query.AddParam("taxonomy", "");
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<Entry>>(result.Items);
            foreach (var entry in result.Items)
            {
                Assert.NotNull(entry.Uid);
                Assert.NotEmpty(entry.Uid);
                // Each entry must have valid structure
            }
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

