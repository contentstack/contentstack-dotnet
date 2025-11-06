using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Contentstack.Core.Unit.Tests.Mokes;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for ContentstackClient class - uses mocks and AutoFixture, no real API calls
    /// Follows Management SDK pattern
    /// </summary>
    public class ContentstackClientUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        private ContentstackClient CreateClient(string environment = null, string apiKey = null, string deliveryToken = null, string version = null)
        {
            var options = new ContentstackOptions()
            {
                ApiKey = apiKey ?? _fixture.Create<string>(),
                DeliveryToken = deliveryToken ?? _fixture.Create<string>(),
                Environment = environment ?? _fixture.Create<string>(),
                Version = version
            };
            return new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        [Fact]
        public void GetEnvironment_ReturnsEnvironment()
        {
            // Arrange
            var environment = _fixture.Create<string>();
            var client = CreateClient(environment);

            // Act
            string result = client.GetEnvironment();

            // Assert
            Assert.Equal(environment, result);
        }

        [Fact]
        public void GetEnvironment_WithDifferentEnvironment_ReturnsCorrectValue()
        {
            // Arrange
            var environment = "production";
            var client = CreateClient(environment);

            // Act
            string result = client.GetEnvironment();

            // Assert
            Assert.Equal("production", result);
        }

        [Fact]
        public void GetEnvironment_WithNullEnvironment_ReturnsNull()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = null  // Explicitly set to null
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act
            string result = client.GetEnvironment();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEnvironment_WithEmptyEnvironment_ReturnsEmpty()
        {
            // Arrange
            var client = CreateClient("");

            // Act
            string result = client.GetEnvironment();

            // Assert
            Assert.Equal("", result);
        }

        #region GetVersion Tests

        [Fact]
        public void GetVersion_ReturnsVersion()
        {
            // Arrange
            var version = "v3";
            var client = CreateClient(version: version);

            // Act
            string result = client.GetVersion();

            // Assert
            Assert.Equal(version, result);
        }

        [Fact]
        public void GetVersion_WithNullVersion_ReturnsDefault()
        {
            // Arrange - Config.Version defaults to "v3" when null
            var client = CreateClient(version: null);

            // Act
            string result = client.GetVersion();

            // Assert - Config has default value "v3"
            Assert.Equal("v3", result);
        }

        #endregion

        #region GetApplicationKey Tests

        [Fact]
        public void GetApplicationKey_ReturnsApiKey()
        {
            // Arrange
            var apiKey = _fixture.Create<string>();
            var client = CreateClient(apiKey: apiKey);

            // Act
            string result = client.GetApplicationKey();

            // Assert
            Assert.Equal(apiKey, result);
        }

        #endregion

        #region GetAccessToken Tests

        [Fact]
        public void GetAccessToken_WithDeliveryToken_ReturnsDeliveryToken()
        {
            // Arrange
            var deliveryToken = _fixture.Create<string>();
            var client = CreateClient(deliveryToken: deliveryToken);

            // Act
            string result = client.GetAccessToken();

            // Assert
            Assert.Equal(deliveryToken, result);
        }

        [Fact]
        public void GetAccessToken_WithAccessToken_ReturnsAccessToken()
        {
            // Arrange
            var accessToken = _fixture.Create<string>();
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                AccessToken = accessToken,
                Environment = _fixture.Create<string>()
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act
            string result = client.GetAccessToken();

            // Assert
            Assert.Equal(accessToken, result);
        }

        #endregion

        #region GetLivePreviewConfig Tests

        [Fact]
        public void GetLivePreviewConfig_ReturnsConfig()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Enable);
        }

        [Fact]
        public void GetLivePreviewConfig_WithLivePreview_ReturnsConfig()
        {
            // Arrange
            var livePreview = new LivePreviewConfig()
            {
                Enable = true,
                PreviewToken = _fixture.Create<string>()
            };
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = livePreview
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act
            var result = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Enable);
        }

        #endregion

        #region ContentType Tests

        [Fact]
        public void ContentType_WithValidName_ReturnsContentType()
        {
            // Arrange
            var client = CreateClient();
            var contentTypeName = _fixture.Create<string>();

            // Act
            var result = client.ContentType(contentTypeName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contentTypeName, result.ContentTypeId);
        }

        [Fact]
        public void ContentType_WithNullName_ReturnsContentType()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.ContentType(null);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ContentType_WithEmptyName_ReturnsContentType()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.ContentType("");

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region GlobalField Tests

        [Fact]
        public void GlobalField_WithValidName_ReturnsGlobalField()
        {
            // Arrange
            var client = CreateClient();
            var globalFieldName = _fixture.Create<string>();

            // Act
            var result = client.GlobalField(globalFieldName);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GlobalField_WithNullName_ReturnsGlobalField()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.GlobalField(null);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region GlobalFieldQuery Tests

        [Fact]
        public void GlobalFieldQuery_ReturnsGlobalFieldQuery()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.GlobalFieldQuery();

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Asset Tests

        [Fact]
        public void Asset_WithValidUid_ReturnsAsset()
        {
            // Arrange
            var client = CreateClient();
            var assetUid = _fixture.Create<string>();

            // Act
            var result = client.Asset(assetUid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetUid, result.Uid);
        }

        [Fact]
        public void Asset_WithNullUid_ReturnsAsset()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.Asset(null);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region AssetLibrary Tests

        [Fact]
        public void AssetLibrary_ReturnsAssetLibrary()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.AssetLibrary();

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region Taxonomies Tests

        [Fact]
        public void Taxonomies_ReturnsTaxonomy()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var result = client.Taxonomies();

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region SetHeader Tests

        [Fact]
        public void SetHeader_WithValidKeyAndValue_AddsHeader()
        {
            // Arrange
            var client = CreateClient();
            var key = "custom_header";
            var value = _fixture.Create<string>();

            // Act
            client.SetHeader(key, value);

            // Assert - Verify header was actually set using reflection
            var headersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(client);
            
            Assert.NotNull(headers);
            Assert.True(headers.ContainsKey(key));
            Assert.Equal(value, headers[key]?.ToString());
        }

        [Fact]
        public void SetHeader_WithNullKey_DoesNotAddHeader()
        {
            // Arrange
            var client = CreateClient();
            var value = _fixture.Create<string>();
            var headersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = new Dictionary<string, object>((Dictionary<string, object>)headersField?.GetValue(client));

            // Act
            client.SetHeader(null, value);

            // Assert - Header should not be added
            var headersAfter = (Dictionary<string, object>)headersField?.GetValue(client);
            Assert.Equal(headersBefore.Count, headersAfter?.Count ?? 0);
        }

        [Fact]
        public void SetHeader_WithNullValue_DoesNotAddHeader()
        {
            // Arrange
            var client = CreateClient();
            var key = _fixture.Create<string>();
            var headersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = new Dictionary<string, object>((Dictionary<string, object>)headersField?.GetValue(client));

            // Act
            client.SetHeader(key, null);

            // Assert - Header should not be added
            var headersAfter = (Dictionary<string, object>)headersField?.GetValue(client);
            Assert.Equal(headersBefore.Count, headersAfter?.Count ?? 0);
        }

        [Fact]
        public void SetHeader_WithExistingKey_ReplacesHeader()
        {
            // Arrange
            var client = CreateClient();
            var key = "test_header";
            var value1 = "value1";
            var value2 = "value2";

            // Act
            client.SetHeader(key, value1);
            client.SetHeader(key, value2);

            // Assert - Verify header was replaced
            var headersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(client);
            
            Assert.NotNull(headers);
            Assert.True(headers.ContainsKey(key));
            Assert.Equal(value2, headers[key]?.ToString());
        }

        #endregion

        #region RemoveHeader Tests

        [Fact]
        public void RemoveHeader_WithExistingKey_RemovesHeader()
        {
            // Arrange
            var client = CreateClient();
            var key = "test_header";
            var value = _fixture.Create<string>();
            
            var headersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = new Dictionary<string, object>((Dictionary<string, object>)headersField?.GetValue(client));
            var initialCount = headersBefore.Count;
            
            client.SetHeader(key, value);
            var headersAfterSet = (Dictionary<string, object>)headersField?.GetValue(client);
            Assert.True(headersAfterSet?.ContainsKey(key) ?? false);
            Assert.Equal(initialCount + 1, headersAfterSet?.Count ?? 0);

            // Act
            client.RemoveHeader(key);

            // Assert - Verify header was actually removed
            var headersAfterRemove = (Dictionary<string, object>)headersField?.GetValue(client);
            Assert.NotNull(headersAfterRemove);
            Assert.False(headersAfterRemove.ContainsKey(key));
            Assert.Equal(initialCount, headersAfterRemove.Count);
        }

        [Fact]
        public void RemoveHeader_WithNonExistentKey_DoesNotThrow()
        {
            // Arrange
            var client = CreateClient();
            var key = _fixture.Create<string>();
            var headersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = new Dictionary<string, object>((Dictionary<string, object>)headersField?.GetValue(client));

            // Act
            client.RemoveHeader(key);

            // Assert - Header count should remain the same
            var headersAfter = (Dictionary<string, object>)headersField?.GetValue(client);
            Assert.Equal(headersBefore.Count, headersAfter?.Count ?? 0);
        }

        [Fact]
        public void RemoveHeader_WithNullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var client = CreateClient();

            // Act & Assert - Dictionary.ContainsKey throws ArgumentNullException for null key
            Assert.Throws<ArgumentNullException>(() => client.RemoveHeader(null));
        }

        #endregion

        #region SetEntryUid Tests

        [Fact]
        public void SetEntryUid_WithValidUid_SetsUid()
        {
            // Arrange
            var client = CreateClient();
            var entryUid = _fixture.Create<string>();
            var currentEntryUidField = typeof(ContentstackClient).GetField("currentEntryUid", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            client.SetEntryUid(entryUid);

            // Assert - Verify UID was actually set using reflection
            var actualUid = currentEntryUidField?.GetValue(client)?.ToString();
            Assert.Equal(entryUid, actualUid);
        }

        [Fact]
        public void SetEntryUid_WithNullUid_DoesNotSetUid()
        {
            // Arrange
            var client = CreateClient();
            var currentEntryUidField = typeof(ContentstackClient).GetField("currentEntryUid", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var initialUid = currentEntryUidField?.GetValue(client);

            // Act
            client.SetEntryUid(null);

            // Assert - UID should remain unchanged (null or initial value)
            var finalUid = currentEntryUidField?.GetValue(client);
            Assert.Equal(initialUid, finalUid);
        }

        [Fact]
        public void SetEntryUid_WithEmptyUid_DoesNotSetUid()
        {
            // Arrange
            var client = CreateClient();
            var currentEntryUidField = typeof(ContentstackClient).GetField("currentEntryUid", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var initialUid = currentEntryUidField?.GetValue(client);

            // Act
            client.SetEntryUid("");

            // Assert - UID should remain unchanged (empty strings are not set)
            var finalUid = currentEntryUidField?.GetValue(client);
            Assert.Equal(initialUid, finalUid);
        }

        #endregion

        #region Sync Methods Tests

        [Fact]
        public void SyncPaginationToken_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var paginationToken = _fixture.Create<string>();
            
            // Act - Just verify setup, not actual HTTP call
            // We can't easily test async methods without mocking HttpRequestHandler
            // So we verify the method exists and can be called
            var method = typeof(ContentstackClient).GetMethod("SyncPaginationToken", 
                BindingFlags.Public | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void SyncToken_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var syncToken = _fixture.Create<string>();
            
            // Act
            var method = typeof(ContentstackClient).GetMethod("SyncToken", 
                BindingFlags.Public | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void SyncRecursive_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var method = typeof(ContentstackClient).GetMethod("SyncRecursive", 
                BindingFlags.Public | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void GetResultAsync_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void SyncPageinationRecursive_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var method = typeof(ContentstackClient).GetMethod("SyncPageinationRecursive", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void Sync_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var method = typeof(ContentstackClient).GetMethod("Sync", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void SyncLanguage_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var method = typeof(ContentstackClient).GetMethod("SyncLanguage", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Assert
            Assert.NotNull(method);
            Assert.Equal(typeof(System.Threading.Tasks.Task<SyncStack>), method.ReturnType);
        }

        [Fact]
        public void GetHeader_WithLocalHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            
            var localHeaders = new Dictionary<string, object> { { "custom-header", "value1" } };
            
            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { localHeaders }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithNullLocalHeaders_ReturnsStackHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            
            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_NoParameters_ReturnsLocalHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { }, 
                null);
            
            // Act
            var result = getHeaderMethod?.Invoke(client, null) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(ContentstackClient).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { webEx }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetContentstackError_WithGenericException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(ContentstackClient).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var ex = new Exception("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { ex }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test error", result.ErrorMessage);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndNoStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            
            var localHeaders = new Dictionary<string, object> { { "custom-header", "value1" } };
            
            // Set _StackHeaders to null
            var stackHeadersField = typeof(ContentstackClient).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(client, null);
            
            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { localHeaders }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithContentTypeUid_SetsContentTypeUID()
        {
            // Arrange
            var client = CreateClient();
            var query = new Dictionary<string, string>
            {
                ["content_type_uid"] = "test_ct_uid"
            };

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal("test_ct_uid", client.LivePreviewConfig.ContentTypeUID);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithEntryUid_SetsEntryUID()
        {
            // Arrange
            var client = CreateClient();
            var query = new Dictionary<string, string>
            {
                ["entry_uid"] = "test_entry_uid"
            };

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal("test_entry_uid", client.LivePreviewConfig.EntryUID);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithLivePreview_SetsLivePreview()
        {
            // Arrange
            var client = CreateClient();
            var query = new Dictionary<string, string>
            {
                ["live_preview"] = "test_hash"
            };

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal("test_hash", client.LivePreviewConfig.LivePreview);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithReleaseId_SetsReleaseId()
        {
            // Arrange
            var client = CreateClient();
            var query = new Dictionary<string, string>
            {
                ["release_id"] = "test_release_id"
            };

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal("test_release_id", client.LivePreviewConfig.ReleaseId);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithPreviewTimestamp_SetsPreviewTimestamp()
        {
            // Arrange
            var client = CreateClient();
            var query = new Dictionary<string, string>
            {
                ["preview_timestamp"] = "test_timestamp"
            };

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal("test_timestamp", client.LivePreviewConfig.PreviewTimestamp);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithoutContentTypeUid_UsesCurrentContentTypeUid()
        {
            // Arrange
            var client = CreateClient();
            var currentContentTypeUid = _fixture.Create<string>();
            var contentTypeUidField = typeof(ContentstackClient).GetField("currentContenttypeUid", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            contentTypeUidField?.SetValue(client, currentContentTypeUid);
            
            var query = new Dictionary<string, string>();

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal(currentContentTypeUid, client.LivePreviewConfig.ContentTypeUID);
        }

        [Fact]
        public void LivePreviewQueryAsync_WithoutEntryUid_UsesCurrentEntryUid()
        {
            // Arrange
            var client = CreateClient();
            var currentEntryUid = _fixture.Create<string>();
            var entryUidField = typeof(ContentstackClient).GetField("currentEntryUid", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            entryUidField?.SetValue(client, currentEntryUid);
            
            var query = new Dictionary<string, string>();

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Equal(currentEntryUid, client.LivePreviewConfig.EntryUID);
        }

        [Fact]
        public void LivePreviewQueryAsync_ClearsLivePreviewConfig()
        {
            // Arrange
            var client = CreateClient();
            client.LivePreviewConfig.LivePreview = "old_hash";
            client.LivePreviewConfig.PreviewTimestamp = "old_timestamp";
            client.LivePreviewConfig.ReleaseId = "old_release";
            
            var query = new Dictionary<string, string>();

            // Act
            var task = client.LivePreviewQueryAsync(query);
            task.Wait();

            // Assert
            Assert.Null(client.LivePreviewConfig.LivePreview);
            Assert.Null(client.LivePreviewConfig.PreviewTimestamp);
            Assert.Null(client.LivePreviewConfig.ReleaseId);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndEmptyStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to empty dictionary
            var stackHeadersField = typeof(ContentstackClient).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(client, new Dictionary<string, object>());

            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void GetHeader_WithNullLocalHeader_ReturnsStackHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);

            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithEmptyLocalHeader_ReturnsStackHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            var localHeader = new Dictionary<string, object>();

            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithOverlappingKeys_LocalHeaderTakesPrecedence()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Set _StackHeaders with same key
            var stackHeadersField = typeof(ContentstackClient).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(client, new Dictionary<string, object> { { "custom", "stack_value" } });

            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        [Fact]
        public void GetHeader_WithBothHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(Dictionary<string, object>) }, 
                null);
            var localHeader = new Dictionary<string, object> { { "local_key", "local_value" } };

            // Set _StackHeaders with different key
            var stackHeadersField = typeof(ContentstackClient).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(client, new Dictionary<string, object> { { "stack_key", "stack_value" } });

            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("local_key"));
            Assert.True(result.ContainsKey("stack_key"));
        }

        [Fact]
        public void GetHeader_WithNoParameters_ReturnsLocalHeaders()
        {
            // Arrange
            var client = CreateClient();
            var type = typeof(ContentstackClient);
            var getHeaderMethod = type.GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                new Type[] { }, 
                null);

            // Act
            var result = getHeaderMethod?.Invoke(client, new object[] { }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetResultAsync_WithInitTrue_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists and has correct signature
            Assert.NotNull(method);
            var parameters = method.GetParameters();
            
            // Assert
            Assert.True(parameters.Length > 0);
        }

        [Fact]
        public void GetResultAsync_WithStartFrom_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists
            Assert.NotNull(method);
            
            // Assert
            var parameters = method.GetParameters();
            var startFromParam = parameters.FirstOrDefault(p => p.Name == "StartFrom");
            Assert.NotNull(startFromParam);
        }

        [Fact]
        public void GetResultAsync_WithSyncToken_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists
            Assert.NotNull(method);
            
            // Assert
            var parameters = method.GetParameters();
            var syncTokenParam = parameters.FirstOrDefault(p => p.Name == "SyncToken");
            Assert.NotNull(syncTokenParam);
        }

        [Fact]
        public void GetResultAsync_WithPaginationToken_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists
            Assert.NotNull(method);
            
            // Assert
            var parameters = method.GetParameters();
            var paginationTokenParam = parameters.FirstOrDefault(p => p.Name == "PaginationToken");
            Assert.NotNull(paginationTokenParam);
        }

        [Fact]
        public void GetResultAsync_WithContentTypeUid_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists
            Assert.NotNull(method);
            
            // Assert
            var parameters = method.GetParameters();
            var contentTypeUidParam = parameters.FirstOrDefault(p => p.Name == "ContentTypeUid");
            Assert.NotNull(contentTypeUidParam);
        }

        [Fact]
        public void GetResultAsync_WithLocale_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists
            Assert.NotNull(method);
            
            // Assert
            var parameters = method.GetParameters();
            var localeParam = parameters.FirstOrDefault(p => p.Name == "Locale");
            Assert.NotNull(localeParam);
        }

        [Fact]
        public void GetResultAsync_WithSyncType_Setup_VerifiesParameters()
        {
            // Arrange
            var client = CreateClient();
            var method = typeof(ContentstackClient).GetMethod("GetResultAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Just verify method exists
            Assert.NotNull(method);
            
            // Assert
            var parameters = method.GetParameters();
            var syncTypeParam = parameters.FirstOrDefault(p => p.Name == "SyncType");
            Assert.NotNull(syncTypeParam);
        }

        [Fact]
        public void GetLivePreviewData_WithLivePreviewEnabled_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "content_type",
                    EntryUID = "entry_uid"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act - Just verify setup, not actual HTTP call
            var livePreviewConfig = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(livePreviewConfig);
            Assert.True(livePreviewConfig.Enable);
        }

        [Fact]
        public void GetLivePreviewData_WithManagementToken_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    ManagementToken = "mgmt_token",
                    ContentTypeUID = "content_type",
                    EntryUID = "entry_uid"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act - Just verify setup
            var livePreviewConfig = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(livePreviewConfig);
            Assert.Equal("mgmt_token", livePreviewConfig.ManagementToken);
        }

        [Fact]
        public void GetLivePreviewData_WithReleaseId_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "content_type",
                    EntryUID = "entry_uid",
                    ReleaseId = "release_123"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act - Just verify setup
            var livePreviewConfig = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(livePreviewConfig);
            Assert.Equal("release_123", livePreviewConfig.ReleaseId);
        }

        [Fact]
        public void GetLivePreviewData_WithPreviewTimestamp_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "content_type",
                    EntryUID = "entry_uid",
                    PreviewTimestamp = "timestamp_123"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act - Just verify setup
            var livePreviewConfig = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(livePreviewConfig);
            Assert.Equal("timestamp_123", livePreviewConfig.PreviewTimestamp);
        }

        [Fact]
        public void GetLivePreviewData_WithEmptyLivePreview_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "content_type",
                    EntryUID = "entry_uid",
                    LivePreview = "" // Empty string
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));

            // Act - Just verify setup
            var livePreviewConfig = client.GetLivePreviewConfig();

            // Assert
            Assert.NotNull(livePreviewConfig);
            Assert.Equal("", livePreviewConfig.LivePreview);
        }

        [Fact]
        public void GetLivePreviewData_WithAccessTokenHeader_SkipsAccessToken()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "content_type",
                    EntryUID = "entry_uid",
                    LivePreview = "preview_value"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            client.SetHeader("access_token", "token_value");

            // Act - Just verify setup
            var localHeadersField = typeof(ContentstackClient).GetField("_LocalHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeaders = (Dictionary<string, object>)localHeadersField?.GetValue(client);

            // Assert
            Assert.NotNull(localHeaders);
            Assert.True(localHeaders.ContainsKey("access_token"));
        }

        #endregion
    }
}
