using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.ConfigurationTests
{
    /// <summary>
    /// Tests for Timeout Configuration
    /// Tests different timeout values and timeout handling
    /// </summary>
    [Trait("Category", "TimeoutConfiguration")]
    public class TimeoutConfigurationTest
    {
        #region Basic Timeout Configuration
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Default Timeout Works Correctly")]
        public async Task Timeout_DefaultTimeout_WorksCorrectly()
        {
            // Arrange
            var client = CreateClientWithTimeout(30000); // 30s
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Long Timeout Allows Complex Operations")]
        public async Task Timeout_LongTimeout_AllowsComplexOperations()
        {
            // Arrange
            var client = CreateClientWithTimeout(60000); // 60s
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { "authors", "authors.reference" })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Standard Timeout Query Operations")]
        public async Task Timeout_StandardTimeout_QueryOperations()
        {
            // Arrange
            var client = CreateClientWithTimeout(30000);
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Limit(10);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Operations within Timeout
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Fast Operation Completes Quickly")]
        public async Task Timeout_FastOperation_CompletesQuickly()
        {
            // Arrange
            var client = CreateClientWithTimeout(10000); // 10s
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 10000);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Asset Fetch Within Timeout")]
        public async Task Timeout_AssetFetch_WithinTimeout()
        {
            // Arrange
            var client = CreateClientWithTimeout(15000);
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
        }
        
        #endregion
        
        #region Different Timeout Values
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Short Timeout Simple Request")]
        public async Task Timeout_ShortTimeout_SimpleRequest()
        {
            // Arrange
            var client = CreateClientWithTimeout(5000); // 5s
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Timeout Configuration - Timeout Medium Timeout Medium Complexity")]
        public async Task Timeout_MediumTimeout_MediumComplexity()
        {
            // Arrange
            var client = CreateClientWithTimeout(20000); // 20s
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.MediumContentTypeUid)
                .Entry(TestDataHelper.MediumEntryUid)
                .IncludeReference("reference")
                .Fetch<Entry>();
            
            // Assert
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
            var client = CreateClientWithTimeout(30000);
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
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

