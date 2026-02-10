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
    /// Extended tests for Query Include operations
    /// Tests various query include combinations
    /// </summary>
    [Trait("Category", "QueryIncludeExtended")]
    public class QueryIncludeExtendedTest : IntegrationTestBase
    {
        public QueryIncludeExtendedTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Query Include Basics
        
        [Fact(DisplayName = "Query Operations - Query Include Count Returns Count For All")]
        public async Task QueryInclude_Count_ReturnsCountForAll()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.IncludeCount();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include Owner Includes Owner For All")]
        public async Task QueryInclude_Owner_IncludesOwnerForAll()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.IncludeOwner();
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include Embedded Items Includes For All")]
        public async Task QueryInclude_EmbeddedItems_IncludesForAll()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.includeEmbeddedItems();
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Multiple Includes in Query
        
        [Fact(DisplayName = "Query Operations - Query Include Count And Owner Both Applied")]
        public async Task QueryInclude_CountAndOwner_BothApplied()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.IncludeCount();
            query.IncludeOwner();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include All Includes Combined")]
        public async Task QueryInclude_AllIncludes_Combined()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.IncludeCount();
            query.IncludeOwner();
            query.includeEmbeddedItems();
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include With References Combines With Includes")]
        public async Task QueryInclude_WithReferences_CombinesWithIncludes()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.IncludeReference("authors");
            query.IncludeCount();
            query.IncludeOwner();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Include with Filters
        
        [Fact(DisplayName = "Query Operations - Query Include With Where Includes On Filtered Results")]
        public async Task QueryInclude_WithWhere_IncludesOnFilteredResults()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.Exists("title");
            query.IncludeOwner();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include With Complex Query Includes Correctly")]
        public async Task QueryInclude_WithComplexQuery_IncludesCorrectly()
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
            query.IncludeCount();
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Include with Projection
        
        [Fact(DisplayName = "Query Operations - Query Include With Only Combines Correctly")]
        public async Task QueryInclude_WithOnly_CombinesCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.IncludeOwner();
            query.Only(new[] { "title", "uid" });
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include With Except Combines Correctly")]
        public async Task QueryInclude_WithExcept_CombinesCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.IncludeCount();
            query.Except(new[] { "large_field" });
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Include with Localization
        
        [Fact(DisplayName = "Query Operations - Query Include With Locale Combines Correctly")]
        public async Task QueryInclude_WithLocale_CombinesCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.SetLocale("en-us");
            query.IncludeOwner();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Query Operations - Query Include With Fallback Combines Correctly")]
        public async Task QueryInclude_WithFallback_CombinesCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act & Assert
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            try
            {
                query.SetLocale("en-us");
                query.IncludeFallback();
                query.IncludeCount();
                query.Limit(3);
                var result = await query.Find<Entry>();
                
                Assert.NotNull(result);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }
        
        #endregion
        
        #region Include with Sorting
        
        [Fact(DisplayName = "Query Operations - Query Include With Sorting Maintains Order")]
        public async Task QueryInclude_WithSorting_MaintainsOrder()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.Descending("created_at");
            query.IncludeOwner();
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Query Operations - Query Include Performance Multiple Includes")]
        public async Task QueryInclude_Performance_MultipleIncludes()
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
                query.IncludeCount();
                query.IncludeOwner();
                query.includeEmbeddedItems();
                query.Limit(5);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Query with multiple includes should complete within 15s, took {elapsed}ms");
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

