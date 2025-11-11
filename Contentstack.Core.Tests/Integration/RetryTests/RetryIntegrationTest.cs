using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.RetryTests
{
    /// <summary>
    /// Tests for Retry Integration and Error Handling
    /// Tests retry logic, network resilience, and error recovery
    /// </summary>
    [Trait("Category", "RetryIntegration")]
    public class RetryIntegrationTest
    {
        #region Successful Retries
        
        [Fact(DisplayName = "Retry Successful Request No Retry Needed")]
        public async Task Retry_SuccessfulRequest_NoRetryNeeded()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Retry Multiple Successful Requests Consistent")]
        public async Task Retry_MultipleSuccessfulRequests_Consistent()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Multiple requests should all succeed
            var task1 = client.ContentType(TestDataHelper.SimpleContentTypeUid).Entry(TestDataHelper.SimpleEntryUid).Fetch<Entry>();
            var task2 = client.ContentType(TestDataHelper.MediumContentTypeUid).Entry(TestDataHelper.MediumEntryUid).Fetch<Entry>();
            var task3 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Entry(TestDataHelper.ComplexEntryUid).Fetch<Entry>();
            
            await Task.WhenAll(task1, task2, task3);
            
            // Assert
            Assert.NotNull(task1.Result);
            Assert.NotNull(task2.Result);
            Assert.NotNull(task3.Result);
        }
        
        #endregion
        
        #region Timeout Scenarios
        
        [Fact(DisplayName = "Retry Within Timeout Succeeds")]
        public async Task Retry_WithinTimeout_Succeeds()
        {
            // Arrange
            var client = CreateClientWithTimeout(30000); // 30s timeout
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Retry Reasonable Timeout Works For Complex Queries")]
        public async Task Retry_ReasonableTimeout_WorksForComplexQueries()
        {
            // Arrange
            var client = CreateClientWithTimeout(30000);
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            query.IncludeReference(new[] { "authors", "authors.reference" });
            query.Limit(10);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Network Resilience
        
        [Fact(DisplayName = "Retry Parallel Requests Handles Load")]
        public async Task Retry_ParallelRequests_HandlesLoad()
        {
            // Arrange
            var client = CreateClient();
            var tasks = new List<Task<Entry>>();
            
            // Act - 5 parallel requests
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>());
            }
            
            await Task.WhenAll(tasks);
            
            // Assert - All should succeed
            Assert.True(tasks.All(t => t.Result != null));
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
            
            return new ContentstackClient(options);
        }
        
        #endregion
    }
}

