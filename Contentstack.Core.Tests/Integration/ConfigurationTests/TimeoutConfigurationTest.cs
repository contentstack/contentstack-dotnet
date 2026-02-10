using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.ConfigurationTests
{
    /// <summary>
    /// Tests for Timeout Configuration
    /// Tests different timeout values and timeout handling
    /// </summary>
    [Trait("Category", "TimeoutConfiguration")]
    public class TimeoutConfigurationTest : IntegrationTestBase
    {
        public TimeoutConfigurationTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic Timeout Configuration
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Default Timeout Works Correctly")]
        public async Task Timeout_DefaultTimeout_WorksCorrectly()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithTimeout(30000); // 30s
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Long Timeout Allows Complex Operations")]
        public async Task Timeout_LongTimeout_AllowsComplexOperations()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClientWithTimeout(60000); // 60s
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { "authors", "authors.reference" })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Standard Timeout Query Operations")]
        public async Task Timeout_StandardTimeout_QueryOperations()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClientWithTimeout(30000);
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries");

            query.Limit(10);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Operations within Timeout
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Fast Operation Completes Quickly")]
        public async Task Timeout_FastOperation_CompletesQuickly()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithTimeout(10000); // 10s
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 10000);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Asset Fetch Within Timeout")]
        public async Task Timeout_AssetFetch_WithinTimeout()
        {
            // Arrange
            LogArrange("Setting up fetch operation");
            LogContext("AssetUid", TestDataHelper.ImageAssetUid);

            var client = CreateClientWithTimeout(15000);
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/assets/{TestDataHelper.ImageAssetUid}");

            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(asset);
        }
        
        #endregion
        
        #region Different Timeout Values
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Short Timeout Simple Request")]
        public async Task Timeout_ShortTimeout_SimpleRequest()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithTimeout(5000); // 5s
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Medium Timeout Medium Complexity")]
        public async Task Timeout_MediumTimeout_MediumComplexity()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);
            LogContext("EntryUid", TestDataHelper.MediumEntryUid);

            var client = CreateClientWithTimeout(20000); // 20s
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.MediumContentTypeUid}/entries/{TestDataHelper.MediumEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.MediumContentTypeUid)
                .Entry(TestDataHelper.MediumEntryUid)
                .IncludeReference("reference")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Performance Monitor Duration")]
        public async Task Timeout_Performance_MonitorDuration()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithTimeout(30000);
            
            // Act
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 30000, $"Should complete within configured timeout, took {elapsed}ms");
        }
        
        #endregion
        
        #region Helper Methods
        
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

