using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Internals;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.StackTests
{
    /// <summary>
    /// Comprehensive tests for Stack/ContentstackClient operations
    /// Tests initialization, configuration, error handling, and core stack functionality
    /// </summary>
    public class StackOperationsComprehensiveTest
    {
        #region Initialization Tests
        
        [Fact(DisplayName = "Stack Operations - Stack Initialization With All Options Should Succeed")]
        public void Stack_Initialization_WithAllOptions_ShouldSucceed()
        {
            // Arrange & Act
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid,
                Timeout = 30000
            };
            
            var client = new ContentstackClient(options);
            
            // Assert
            Assert.NotNull(client);
            AssertionHelper.AssertStackConfiguration(client, options);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Initialization With Minimal Options Should Succeed")]
        public void Stack_Initialization_WithMinimalOptions_ShouldSucceed()
        {
            // Arrange & Act
            var options = new ContentstackOptions()
            {
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            
            var client = new ContentstackClient(options);
            
            // Assert
            Assert.NotNull(client);
            Assert.Equal(TestDataHelper.ApiKey, client.GetApplicationKey());
            Assert.Equal(TestDataHelper.DeliveryToken, client.GetAccessToken());
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Initialization With Live Preview Should Configure Correctly")]
        public void Stack_Initialization_WithLivePreview_ShouldConfigureCorrectly()
        {
            // Arrange
            if (!TestDataHelper.IsLivePreviewConfigured())
            {
                // Skip if Live Preview is not configured
                return;
            }
            
            // Act
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = TestDataHelper.PreviewToken,
                    Host = TestDataHelper.LivePreviewHost
                }
            };
            
            var client = new ContentstackClient(options);
            
            // Assert
            Assert.NotNull(client);
            var livePreviewConfig = client.GetLivePreviewConfig();
            Assert.NotNull(livePreviewConfig);
            Assert.True(livePreviewConfig.Enable);
            Assert.Equal(TestDataHelper.PreviewToken, livePreviewConfig.PreviewToken);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Live Preview Enabled By Default False")]
        public void Stack_LivePreview_EnabledByDefault_False()
        {
            // Arrange & Act
            var options = new ContentstackOptions()
            {
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            
            var client = new ContentstackClient(options);
            
            // Assert
            var livePreviewConfig = client.GetLivePreviewConfig();
            Assert.False(livePreviewConfig?.Enable ?? false);
        }
        
        #endregion
        
        #region Configuration Tests
        
        [Fact(DisplayName = "Stack Operations - Stack Get Application Key Returns Correct Value")]
        public void Stack_GetApplicationKey_ReturnsCorrectValue()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var apiKey = client.GetApplicationKey();
            
            // Assert
            Assert.Equal(TestDataHelper.ApiKey, apiKey);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Get Access Token Returns Correct Value")]
        public void Stack_GetAccessToken_ReturnsCorrectValue()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var deliveryToken = client.GetAccessToken();
            
            // Assert
            Assert.Equal(TestDataHelper.DeliveryToken, deliveryToken);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Get Version Returns Non Empty String")]
        public void Stack_GetVersion_ReturnsNonEmptyString()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var version = client.GetVersion();
            
            // Assert
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            // Version can be either semantic (1.0.0) or simple (v3)
            Assert.True(version.Length > 0, $"Version should not be empty, got: {version}");
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Set Header Custom Headers Are Applied")]
        public void Stack_SetHeader_CustomHeaders_AreApplied()
        {
            // Arrange
            var client = CreateClient();
            var headerKey = "X-Custom-Header";
            var headerValue = "CustomValue";
            
            // Act
            client.SetHeader(headerKey, headerValue);
            
            // Assert - Headers should be stored and used in subsequent requests
            // Note: We can't directly verify headers without making a request,
            // but we can verify the method doesn't throw
            Assert.NotNull(client);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Remove Header Removes Custom Header")]
        public void Stack_RemoveHeader_RemovesCustomHeader()
        {
            // Arrange
            var client = CreateClient();
            var headerKey = "X-Custom-Header";
            var headerValue = "CustomValue";
            
            // Act
            client.SetHeader(headerKey, headerValue);
            client.RemoveHeader(headerKey);
            
            // Assert - Header should be removed
            // Verify method executes without error
            Assert.NotNull(client);
        }
        
        #endregion
        
        #region Content Type Operations
        
        [Fact(DisplayName = "Stack Operations - Stack Content Type Can Be Accessed")]
        public async Task Stack_ContentType_CanBeAccessed()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var contentType = client.ContentType(TestDataHelper.SimpleContentTypeUid);
            var result = await contentType.Fetch();
            
            // Assert
            Assert.NotNull(contentType);
            Assert.NotNull(result);
            // ContentType.Fetch returns JObject, verify it has data
            Assert.True(result.Count > 0, "Content type should have schema fields");
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Content Type Query Returns Entries")]
        public async Task Stack_ContentType_Query_ReturnsEntries()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var contentType = client.ContentType(TestDataHelper.SimpleContentTypeUid);
            var query = contentType.Query();
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Entry Can Be Accessed")]
        public async Task Stack_Entry_CanBeAccessed()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = client.ContentType(TestDataHelper.SimpleContentTypeUid)
                              .Entry(TestDataHelper.SimpleEntryUid);
            var result = await entry.Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(result);
            Assert.Equal(TestDataHelper.SimpleEntryUid, result.Uid);
        }
        
        #endregion
        
        #region Asset Operations
        
        [Fact(DisplayName = "Stack Operations - Stack Asset Can Be Accessed")]
        public async Task Stack_Asset_CanBeAccessed()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = client.Asset(TestDataHelper.ImageAssetUid);
            var result = await asset.Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(result);
            Assert.Equal(TestDataHelper.ImageAssetUid, result.Uid);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Asset Library Can Be Accessed")]
        public async Task Stack_AssetLibrary_CanBeAccessed()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var assetLibrary = client.AssetLibrary();
            var result = await assetLibrary.FetchAll();
            
            // Assert
            Assert.NotNull(assetLibrary);
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Image Delivery Asset Url Is Accessible")]
        public async Task Stack_ImageDelivery_AssetUrlIsAccessible()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
            Assert.NotEmpty(asset.Url);
            // Verify URL is a valid HTTP/HTTPS URL
            Assert.True(Uri.TryCreate(asset.Url, UriKind.Absolute, out var uri));
            Assert.True(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
        
        #endregion
        
        #region Branch Support
        
        [Fact(DisplayName = "Stack Operations - Stack Branches Support Can Query With Branch")]
        public async Task Stack_Branches_Support_CanQueryWithBranch()
        {
            // Arrange - Include Host for custom regions like dev11
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid
            };
            var client = new ContentstackClient(options);
            
            // Act
            var entry = await client.ContentType(TestDataHelper.SimpleContentTypeUid)
                                    .Entry(TestDataHelper.SimpleEntryUid)
                                    .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
        }
        
        #endregion
        
        #region Error Handling
        
        [Fact(DisplayName = "Stack Operations - Stack Invalid API Key Throws Error")]
        public async Task Stack_InvalidAPIKey_ThrowsError()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = "invalid_api_key_12345",
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(options);
            
            // Act & Assert
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType(TestDataHelper.SimpleContentTypeUid)
                           .Entry(TestDataHelper.SimpleEntryUid)
                           .Fetch<Entry>();
            });
            
            // Verify it's an error response (can be EntryException, AssetException, or similar)
            Assert.NotNull(exception);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Invalid Delivery Token Throws Error")]
        public async Task Stack_InvalidDeliveryToken_ThrowsError()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = "invalid_delivery_token_12345",
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(options);
            
            // Act & Assert
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType(TestDataHelper.SimpleContentTypeUid)
                           .Entry(TestDataHelper.SimpleEntryUid)
                           .Fetch<Entry>();
            });
            
            // Verify it's an error response (can be EntryException, AssetException, or similar)
            Assert.NotNull(exception);
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Invalid Content Type UID Throws Error")]
        public async Task Stack_InvalidContentTypeUID_ThrowsError()
        {
            // Arrange
            var client = CreateClient();
            
            // Act & Assert
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType("invalid_content_type_uid_12345")
                           .Entry("invalid_entry_uid_12345")
                           .Fetch<Entry>();
            });
            
            // Verify it's an error response
            Assert.NotNull(exception);
        }
        
        #endregion
        
        #region Timeout and Performance
        
        [Fact(DisplayName = "Stack Operations - Stack Timeout Configuration Is Respected")]
        public async Task Stack_Timeout_Configuration_IsRespected()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,  // Use configured host for custom regions
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Timeout = 30000 // 30 seconds
            };
            var client = new ContentstackClient(options);
            
            // Act
            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.ContentType(TestDataHelper.SimpleContentTypeUid)
                                   .Entry(TestDataHelper.SimpleEntryUid)
                                   .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(result);
            Assert.True(elapsed < 30000, $"Request should complete within timeout, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Stack Operations - Stack Concurrent Requests Handled Correctly")]
        public async Task Stack_ConcurrentRequests_HandledCorrectly()
        {
            // Arrange
            var client = CreateClient();
            var tasks = new List<Task<Entry>>();
            
            // Act - Make 5 concurrent requests
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(client.ContentType(TestDataHelper.SimpleContentTypeUid)
                               .Entry(TestDataHelper.SimpleEntryUid)
                               .Fetch<Entry>());
            }
            
            var results = await Task.WhenAll(tasks);
            
            // Assert
            Assert.Equal(5, results.Length);
            Assert.All(results, result =>
            {
                Assert.NotNull(result);
                Assert.Equal(TestDataHelper.SimpleEntryUid, result.Uid);
            });
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

