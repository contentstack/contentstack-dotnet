using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.RetryTests
{
    /// <summary>
    /// Tests for Retry Integration and Error Handling
    /// Tests retry logic, network resilience, and error recovery
    /// </summary>
    [Trait("Category", "RetryIntegration")]
    public class RetryIntegrationTest : IntegrationTestBase
    {
        public RetryIntegrationTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Successful Retries
        
        [Fact(DisplayName = "Retry Successful Request No Retry Needed")]
        public async Task Retry_SuccessfulRequest_NoRetryNeeded()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Retry Multiple Successful Requests Consistent")]
        public async Task Retry_MultipleSuccessfulRequests_Consistent()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);
            LogContext("EntryUid", TestDataHelper.MediumEntryUid);

            var client = CreateClient();
            
            // Act - Multiple requests should all succeed
            LogAct("Fetching entry from API");

            var task1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Entry(TestDataHelper.SimpleEntryUid).Fetch<Entry>();
            var task2 = client.ContentType(TestDataHelper.MediumContentTypeUid).Entry(TestDataHelper.MediumEntryUid).Fetch<Entry>();
            var task3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Entry(TestDataHelper.ComplexEntryUid).Fetch<Entry>();
            
            await Task.WhenAll(task1, task2, task3);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(task1.Result);
            TestAssert.NotNull(task2.Result);
            TestAssert.NotNull(task3.Result);
        }
        
        #endregion
        
        #region Timeout Scenarios
        
        [Fact(DisplayName = "Retry Within Timeout Succeeds")]
        public async Task Retry_WithinTimeout_Succeeds()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithTimeout(30000); // 30s timeout
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Retry Reasonable Timeout Works For Complex Queries")]
        public async Task Retry_ReasonableTimeout_WorksForComplexQueries()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClientWithTimeout(30000);
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.IncludeReference(new[] { "authors", "authors.reference" });
            query.Limit(10);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Network Resilience
        
        [Fact(DisplayName = "Retry Parallel Requests Handles Load")]
        public async Task Retry_ParallelRequests_HandlesLoad()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            var tasks = new List<Task<Entry>>();
            
            // Act - 5 parallel requests
            LogAct("Fetching entry from API");

            for (int i = 0; i < 5; i++)
            {
                tasks.Add(client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>());
            }
            
            await Task.WhenAll(tasks);
            
            // Assert - All should succeed
            LogAssert("Verifying response");

            TestAssert.True(tasks.All(t => t.Result != null));
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
        
        private ContentstackClient CreateClientWithTimeout(int timeoutMs)
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Timeout = timeoutMs
            };
            
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }
        
        #endregion
    }
}

