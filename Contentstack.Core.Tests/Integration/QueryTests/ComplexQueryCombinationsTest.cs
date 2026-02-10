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
    /// Tests for Complex Query Combinations (AND, OR, nested queries)
    /// </summary>
    [Trait("Category", "ComplexQueryCombinations")]
    public class ComplexQueryCombinationsTest : IntegrationTestBase
    {
        public ComplexQueryCombinationsTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Triple AND Conditions
        
        [Fact(DisplayName = "Query Operations - Complex Query Triple And All Conditions Met")]
        public async Task ComplexQuery_TripleAnd_AllConditionsMet()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            var sub3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("url");
            query.And(new List<Query> { sub1, sub2, sub3 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Complex Query And With Different Operators Combined")]
        public async Task ComplexQuery_AndWithDifferentOperators_Combined()
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

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Where("uid", TestDataHelper.ComplexEntryUid);
            query.And(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Triple OR Conditions
        
        [Fact(DisplayName = "Query Operations - Complex Query Triple Or Any Condition Met")]
        public async Task ComplexQuery_TripleOr_AnyConditionMet()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);
            LogContext("EntryUid", TestDataHelper.MediumEntryUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Where("uid", TestDataHelper.SimpleEntryUid);
            var sub2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Where("uid", TestDataHelper.MediumEntryUid);
            var sub3 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Exists("title");
            query.Or(new List<Query> { sub1, sub2, sub3 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Complex Query Or With Different Fields Flexible")]
        public async Task ComplexQuery_OrWithDifferentFields_Flexible()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("authors");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("related_content");
            query.Or(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Nested AND/OR Combinations
        
        [Fact(DisplayName = "Query Operations - Complex Query And Inside Or Nested Logic")]
        public async Task ComplexQuery_AndInsideOr_NestedLogic()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - (A AND B) OR (C AND D)
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var and1Sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var and1Sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            var and1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().And(new List<Query> { and1Sub1, and1Sub2 });
            
            var and2Sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("url");
            var and2Sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("authors");
            var and2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().And(new List<Query> { and2Sub1, and2Sub2 });
            
            query.Or(new List<Query> { and1, and2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Complex Query Or Inside And Nested Logic")]
        public async Task ComplexQuery_OrInsideAnd_NestedLogic()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - (A OR B) AND (C OR D)
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var or1Sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var or1Sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("url");
            var or1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Or(new List<Query> { or1Sub1, or1Sub2 });
            
            var or2Sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            var or2Sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("authors");
            var or2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Or(new List<Query> { or2Sub1, or2Sub2 });
            
            query.And(new List<Query> { or1, or2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Complex Filters with References
        
        [Fact(DisplayName = "Query Operations - Complex Query And With References Filters And Includes")]
        public async Task ComplexQuery_AndWithReferences_FiltersAndIncludes()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("authors");
            query.And(new List<Query> { sub1, sub2 });
            query.IncludeReference("authors");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Complex Query Or With Projection Combines Features")]
        public async Task ComplexQuery_OrWithProjection_CombinesFeatures()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("url");
            query.Or(new List<Query> { sub1, sub2 });
            query.Only(new[] { "title", "uid" });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Complex Filters with Pagination
        
        [Fact(DisplayName = "Query Operations - Complex Query And With Pagination Limited Results")]
        public async Task ComplexQuery_AndWithPagination_LimitedResults()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            query.And(new List<Query> { sub1, sub2 });
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(result.Items.Count() <= 5);
        }
        
        [Fact(DisplayName = "Query Operations - Complex Query Or With Sorting Ordered Results")]
        public async Task ComplexQuery_OrWithSorting_OrderedResults()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("url");
            query.Or(new List<Query> { sub1, sub2 });
            query.Descending("created_at");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Query Operations - Complex Query Performance Nested Combinations")]
        public async Task ComplexQuery_Performance_NestedCombinations()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
                var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
                var sub3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("url");
                query.And(new List<Query> { sub1, sub2, sub3 });
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Complex nested query should complete within 15s, took {elapsed}ms");
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

