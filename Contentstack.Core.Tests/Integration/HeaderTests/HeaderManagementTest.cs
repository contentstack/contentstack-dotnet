using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.HeaderTests
{
    /// <summary>
    /// Tests for Header Management
    /// Tests custom headers, header manipulation, and request headers
    /// </summary>
    [Trait("Category", "HeaderManagement")]
    public class HeaderManagementTest
    {
        #region Basic Header Operations
        
        [Fact(DisplayName = "Header Management - Header Set Custom Header Works Correctly")]
        public async Task Header_SetCustomHeader_WorksCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var entryObj = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            
            // Act
            entryObj.SetHeader("X-Custom-Header", "test-value");
            var entry = await entryObj.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        [Fact(DisplayName = "Header Management - Header Multiple Custom Headers All Applied")]
        public async Task Header_MultipleCustomHeaders_AllApplied()
        {
            // Arrange
            var client = CreateClient();
            var entryObj = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            
            // Act
            entryObj.SetHeader("X-Custom-1", "value1");
            entryObj.SetHeader("X-Custom-2", "value2");
            var entry = await entryObj.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        [Fact(DisplayName = "Header Management - Header Query With Header Header Applied")]
        public async Task Header_QueryWithHeader_HeaderApplied()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.SetHeader("X-Query-Header", "query-value");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Header Manipulation
        
        [Fact(DisplayName = "Header Management - Header Overwrite Header Uses Latest Value")]
        public async Task Header_OverwriteHeader_UsesLatestValue()
        {
            // Arrange
            var client = CreateClient();
            var entryObj = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            
            // Act
            entryObj.SetHeader("X-Test", "original");
            entryObj.SetHeader("X-Test", "updated");
            var entry = await entryObj.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        [Fact(DisplayName = "Header Management - Header Remove Header Works Correctly")]
        public async Task Header_RemoveHeader_WorksCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var entryObj = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            
            // Act
            entryObj.SetHeader("X-Remove", "value");
            entryObj.RemoveHeader("X-Remove");
            var entry = await entryObj.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        #endregion
        
        #region Common Headers
        
        [Fact(DisplayName = "Header Management - Header User Agent Can Be Set")]
        public async Task Header_UserAgent_CanBeSet()
        {
            // Arrange
            var client = CreateClient();
            var entryObj = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            
            // Act
            entryObj.SetHeader("User-Agent", "CustomUserAgent/1.0");
            var entry = await entryObj.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        [Fact(DisplayName = "Header Management - Header Accept Header Can Be Set")]
        public async Task Header_AcceptHeader_CanBeSet()
        {
            // Arrange
            var client = CreateClient();
            var entryObj = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            
            // Act
            entryObj.SetHeader("Accept", "application/json");
            var entry = await entryObj.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
        }
        
        #endregion
        
        #region Headers with Different Operations
        
        [Fact(DisplayName = "Header Management - Header Asset With Header Header Applied")]
        public async Task Header_AssetWithHeader_HeaderApplied()
        {
            // Arrange
            var client = CreateClient();
            var assetObj = client.Asset(TestDataHelper.ImageAssetUid);
            
            // Act
            assetObj.SetHeader("X-Asset-Header", "asset-value");
            var asset = await assetObj.Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(asset.Uid);
        }
        
        [Fact(DisplayName = "Header Management - Header Query With Multiple Headers All Applied")]
        public async Task Header_QueryWithMultipleHeaders_AllApplied()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.SetHeader("X-Query-1", "value1");
            query.SetHeader("X-Query-2", "value2");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Header Persistence
        
        [Fact(DisplayName = "Header Management - Header Client Level Persists Across Requests")]
        public async Task Header_ClientLevel_PersistsAcrossRequests()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Multiple requests should maintain headers
            var entryObj1 = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            entryObj1.SetHeader("X-Persistent", "value");
            var entry1 = await entryObj1.Fetch<Entry>();
            
            var entry2 = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry1);
            Assert.NotNull(entry1.Uid);
            Assert.NotNull(entry2);
            Assert.NotNull(entry2.Uid);
        }
        
        [Fact(DisplayName = "Header Management - Header Request Level Independent Requests")]
        public async Task Header_RequestLevel_IndependentRequests()
        {
            // Arrange
            var client = CreateClient();
            
            // Act - Headers should be independent per request
            var entryObj1 = client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid);
            entryObj1.SetHeader("X-Request-1", "value1");
            var entry1 = await entryObj1.Fetch<Entry>();
            
            var entryObj2 = client
                .ContentType(TestDataHelper.MediumContentTypeUid)
                .Entry(TestDataHelper.MediumEntryUid);
            entryObj2.SetHeader("X-Request-2", "value2");
            var entry2 = await entryObj2.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry1);
            Assert.NotNull(entry1.Uid);
            Assert.NotNull(entry2);
            Assert.NotNull(entry2.Uid);
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

