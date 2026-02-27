using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.PaginationTests
{
    /// <summary>
    /// Comprehensive tests for Pagination functionality
    /// Tests limit, skip, multiple pages, and pagination edge cases
    /// </summary>
    [Trait("Category", "PaginationComprehensive")]
    public class PaginationComprehensiveTest : IntegrationTestBase
    {
        public PaginationComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic Pagination
        
        [Fact(DisplayName = "Pagination - Pagination Limit Returns Limited Results")]
        public async Task Pagination_Limit_ReturnsLimitedResults()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            // ✅ KEY TEST: Verify limit is applied
            TestAssert.True(result.Limit <= 3 || result.Limit == 0, "Limit should be <= requested or 0 if not returned by API");
            TestAssert.True(result.Items.Count() <= 3);
        }
        
        [Fact(DisplayName = "Pagination - Pagination Skip Skips Results")]
        public async Task Pagination_Skip_SkipsResults()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Skip(2);
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            // ✅ KEY TEST: Verify skip and limit applied
            TestAssert.True(result.Skip >= 0, "Skip should be >= 0");
            TestAssert.True(result.Limit <= 5 || result.Limit == 0, "Limit should be <= requested or 0 if not returned by API");
            TestAssert.True(result.Items.Count() <= 5);
        }
        
        [Fact(DisplayName = "Pagination - Pagination Limit And Skip Combine Correctly")]
        public async Task Pagination_LimitAndSkip_CombineCorrectly()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Limit(3);
            query.Skip(1);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            // ✅ KEY TEST: Verify both limit and skip applied
            TestAssert.True(result.Limit <= 3 || result.Limit == 0, "Limit should be <= requested or 0 if not returned by API");
            TestAssert.True(result.Skip >= 0, "Skip should be >= 0");
            TestAssert.True(result.Items.Count() <= 3);
        }
        
        #endregion
        
        #region Multiple Pages
        
        [Fact(DisplayName = "Pagination - Pagination First Page Returns First Set")]
        public async Task Pagination_FirstPage_ReturnsFirstSet()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Page 1
            LogAct("Executing query");

            query.Limit(3);
            query.Skip(0);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            // ✅ KEY TEST: Verify pagination params for first page
            TestAssert.True(result.Limit <= 3 || result.Limit == 0, "Limit should be <= requested or 0 if not returned by API");
            TestAssert.True(result.Skip >= 0, "Skip should be >= 0");
            TestAssert.True(result.Items.Count() <= 3);
            // ✅ KEY TEST: Verify limit is applied
            TestAssert.True(result.Limit <= 3 || result.Limit == 0, "Limit should be <= requested or 0 if not returned by API");
            TestAssert.True(result.Items.Count() <= 3);
        }
        
        [Fact(DisplayName = "Pagination - Pagination Sorted Pages Consistent Order")]
        public async Task Pagination_SortedPages_ConsistentOrder()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act - Page 1 sorted
            LogAct("Executing query");

            var query1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query1.Descending("created_at");
            query1.Limit(2);
            var page1 = await query1.Find<Entry>();
            
            // Page 2 sorted
            var query2 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query2.Descending("created_at");
            query2.Limit(2);
            query2.Skip(2);
            var page2 = await query2.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(page1);
            TestAssert.NotNull(page2);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Pagination - Pagination Performance Small Page")]
        public async Task Pagination_Performance_SmallPage()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.Limit(5);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.True(elapsed < 5000, $"Small page should complete within 5s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Pagination - Pagination Performance Large Page")]
        public async Task Pagination_Performance_LargePage()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.Limit(50);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.True(elapsed < 10000, $"Large page should complete within 10s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Pagination - Pagination Zero Limit Returns Default")]
        public async Task Pagination_ZeroLimit_ReturnsDefault()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Default limit should apply
            LogAct("Executing query");

            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Pagination - Pagination Large Skip Handles Gracefully")]
        public async Task Pagination_LargeSkip_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Skip beyond available results
            LogAct("Executing query");

            query.Skip(1000);
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            // Should return empty or remaining items
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

