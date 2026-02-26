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
    /// Tests for Complex Field Queries (nested fields, groups, modular blocks)
    /// </summary>
    [Trait("Category", "ComplexFieldQueries")]
    public class ComplexFieldQueriesTest : IntegrationTestBase
    {
        public ComplexFieldQueriesTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Group Field Queries
        
        [Fact(DisplayName = "Complex Field Query Group Field By Dot Notation")]
        public async Task ComplexField_QueryGroupField_ByDotNotation()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("group.nested_field");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Query Nested Group Deep Path")]
        public async Task ComplexField_QueryNestedGroup_DeepPath()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("group.nested_group.deep_field");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Where On Group Field Filters Correctly")]
        public async Task ComplexField_WhereOnGroupField_FiltersCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Where("group.title", "Test");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Modular Blocks Queries
        
        [Fact(DisplayName = "Complex Field Query Modular Block Exists Check")]
        public async Task ComplexField_QueryModularBlock_ExistsCheck()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("modular_blocks");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Query Modular Block Field Dot Notation")]
        public async Task ComplexField_QueryModularBlockField_DotNotation()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("modular_blocks.block_title");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region JSON RTE Queries
        
        [Fact(DisplayName = "Complex Field Query Json Rte Exists Check")]
        public async Task ComplexField_QueryJsonRte_ExistsCheck()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("json_rte");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Query Json Rte Embedded Finds Entries")]
        public async Task ComplexField_QueryJsonRteEmbedded_FindsEntries()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.includeEmbeddedItems();
            query.Exists("json_rte");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Array Field Queries
        
        [Fact(DisplayName = "Complex Field Query Array Field Contained In")]
        public async Task ComplexField_QueryArrayField_ContainedIn()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.ContainedIn("multi_select", new object[] { "option1", "option2" });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Query Multi Reference Array Containment")]
        public async Task ComplexField_QueryMultiReference_ArrayContainment()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.ContainedIn("authors", new object[] { TestDataHelper.SimpleEntryUid });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region File/Asset Field Queries
        
        [Fact(DisplayName = "Complex Field Query File Field Exists")]
        public async Task ComplexField_QueryFileField_Exists()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("file");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Query Multiple File Fields And Condition")]
        public async Task ComplexField_QueryMultipleFileFields_AndCondition()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("file");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("image");
            query.And(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Taxonomy Field Queries
        
        [Fact(DisplayName = "Complex Field Query Taxonomy By Term")]
        public async Task ComplexField_QueryTaxonomy_ByTerm()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("TaxonomyTerm", TestDataHelper.TaxUsaState);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Where("taxonomy.usa_states", TestDataHelper.TaxUsaState);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Complex Field Query Multiple Taxonomies Or Condition")]
        public async Task ComplexField_QueryMultipleTaxonomies_OrCondition()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("TaxonomyTerm", TestDataHelper.TaxUsaState);
            LogContext("TaxonomyTerm", TestDataHelper.TaxIndiaState);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Where("taxonomy.usa_states", TestDataHelper.TaxUsaState);
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Where("taxonomy.india_states", TestDataHelper.TaxIndiaState);
            query.Or(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Complex Field Performance Deep Nested Query")]
        public async Task ComplexField_Performance_DeepNestedQuery()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.Exists("group.nested_group.deep_field");
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.True(elapsed < 10000, $"Nested query should complete within 10s, took {elapsed}ms");
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

