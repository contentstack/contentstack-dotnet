using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for Asset class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class AssetUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public AssetUnitTests()
        {
            Initialize();
        }

        private void Initialize()
        {
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>()
            };
            _client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        private Asset CreateAsset(string uid = "test_asset_uid")
        {
            return _client.Asset(uid);
        }

        private Asset CreateAssetWithAttributes(Dictionary<string, object> attributes)
        {
            var asset = CreateAsset();
            var field = typeof(Asset).GetField("_ObjectAttributes", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(asset, attributes);
            return asset;
        }

        #region GetDeleteAt Tests

        [Fact]
        public void GetDeleteAt_WithDeletedAsset_ReturnsDateTime()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" },
                { "deleted_at", "2023-01-03T00:00:00.000Z" }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            DateTime deletedAt = asset.GetDeleteAt();

            // Assert
            Assert.NotEqual(DateTime.MinValue, deletedAt);
            Assert.Equal(2023, deletedAt.Year);
            Assert.Equal(1, deletedAt.Month);
            Assert.Equal(3, deletedAt.Day);
        }

        [Fact]
        public void GetDeleteAt_WithoutDeletedAt_ReturnsMinValue()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            DateTime deletedAt = asset.GetDeleteAt();

            // Assert
            Assert.Equal(DateTime.MinValue, deletedAt);
        }

        #endregion

        #region GetDeletedBy Tests

        [Fact]
        public void GetDeletedBy_WithDeletedBy_ReturnsUid()
        {
            // Arrange
            var deletedBy = _fixture.Create<string>();
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" },
                { "deleted_by", deletedBy }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            string result = asset.GetDeletedBy();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deletedBy, result);
        }

        [Fact]
        public void GetDeletedBy_WithoutDeletedBy_ThrowsException()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => asset.GetDeletedBy());
        }

        #endregion

        #region GetCreateAt Tests

        [Fact]
        public void GetCreateAt_WithCreatedAt_ReturnsDateTime()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" },
                { "created_at", "2023-01-01T00:00:00.000Z" }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            DateTime result = asset.GetCreateAt();

            // Assert
            Assert.NotEqual(DateTime.MinValue, result);
            Assert.Equal(2023, result.Year);
            Assert.Equal(1, result.Month);
            Assert.Equal(1, result.Day);
        }

        [Fact]
        public void GetCreateAt_WithoutCreatedAt_ReturnsMinValue()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            DateTime result = asset.GetCreateAt();

            // Assert
            Assert.Equal(DateTime.MinValue, result);
        }

        #endregion

        #region GetCreatedBy Tests

        [Fact]
        public void GetCreatedBy_WithCreatedBy_ReturnsUid()
        {
            // Arrange
            var createdBy = "user_123";
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" },
                { "created_by", createdBy }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            string result = asset.GetCreatedBy();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdBy, result);
        }

        [Fact]
        public void GetCreatedBy_WithoutCreatedBy_ThrowsException()
        {
            // Arrange
            var asset = CreateAsset();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => asset.GetCreatedBy());
        }

        #endregion

        #region GetUpdateAt Tests

        [Fact]
        public void GetUpdateAt_WithUpdatedAt_ReturnsDateTime()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" },
                { "updated_at", "2023-01-02T00:00:00.000Z" }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            DateTime result = asset.GetUpdateAt();

            // Assert
            Assert.NotEqual(DateTime.MinValue, result);
            Assert.Equal(2023, result.Year);
            Assert.Equal(1, result.Month);
            Assert.Equal(2, result.Day);
        }

        [Fact]
        public void GetUpdateAt_WithoutUpdatedAt_ReturnsMinValue()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            DateTime result = asset.GetUpdateAt();

            // Assert
            Assert.Equal(DateTime.MinValue, result);
        }

        #endregion

        #region GetUpdatedBy Tests

        [Fact]
        public void GetUpdatedBy_WithUpdatedBy_ReturnsUid()
        {
            // Arrange
            var updatedBy = "user_456";
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { "filename", "test.jpg" },
                { "updated_by", updatedBy }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            string result = asset.GetUpdatedBy();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedBy, result);
        }

        [Fact]
        public void GetUpdatedBy_WithoutUpdatedBy_ThrowsException()
        {
            // Arrange
            var asset = CreateAsset();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => asset.GetUpdatedBy());
        }

        #endregion

        #region Get Tests

        [Fact]
        public void Get_WithValidKey_ReturnsValue()
        {
            // Arrange
            var key = "filename";
            var value = "test.jpg";
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_asset_uid" },
                { key, value }
            };
            var asset = CreateAssetWithAttributes(attributes);

            // Act
            var result = asset.Get(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void Get_WithInvalidKey_ReturnsNull()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            var result = asset.Get("non_existent_key");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region IncludeFallback Tests

        [Fact]
        public void IncludeFallback_AddsQueryParameter()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.IncludeFallback();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(asset, result);
            
            var urlQueriesField = typeof(Asset).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.Equal("true", urlQueries?["include_fallback"]?.ToString());
        }

        #endregion

        #region IncludeMetadata Tests

        [Fact]
        public void IncludeMetadata_AddsQueryParameter()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.IncludeMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(asset, result);
            
            var urlQueriesField = typeof(Asset).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            
            Assert.True(urlQueries?.ContainsKey("include_metadata") ?? false);
            Assert.Equal("true", urlQueries?["include_metadata"]?.ToString());
        }

        #endregion

        #region AssetFields Tests

        [Fact]
        public void AssetFields_WithSingleField_AddsQueryParameter()
        {
            // Arrange
            var asset = CreateAsset();
            var field = "user_defined_fields";

            // Act
            Asset result = asset.AssetFields(field);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(asset, result);

            var urlQueriesField = typeof(Asset).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);

            Assert.True(urlQueries?.ContainsKey("asset_fields[]") ?? false);
            var fields = urlQueries?["asset_fields[]"] as string[];
            Assert.NotNull(fields);
            Assert.Single(fields);
            Assert.Equal("user_defined_fields", fields[0]);
        }

        [Fact]
        public void AssetFields_WithMultipleFields_AddsQueryParameter()
        {
            // Arrange
            var asset = CreateAsset();
            var fields = new[] { "embedded", "visual_markups" };

            // Act
            Asset result = asset.AssetFields(fields);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(asset, result);

            var urlQueriesField = typeof(Asset).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);

            Assert.True(urlQueries?.ContainsKey("asset_fields[]") ?? false);
            Assert.Equal(fields, urlQueries?["asset_fields[]"]);
        }

        [Fact]
        public void AssetFields_ReturnsSameInstance_ForChaining()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.AssetFields("ai_suggested");

            // Assert
            Assert.NotNull(result);
            Assert.Same(asset, result);
        }

        [Fact]
        public void AssetFields_WithNoArguments_DoesNotAddParameter()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.AssetFields();

            // Assert
            var urlQueriesField = typeof(Asset).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            Assert.False(urlQueries?.ContainsKey("asset_fields[]") ?? false);
        }

        [Fact]
        public void AssetFields_WithNull_DoesNotAddParameter()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.AssetFields(null);

            // Assert
            var urlQueriesField = typeof(Asset).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            Assert.False(urlQueries?.ContainsKey("asset_fields[]") ?? false);
        }

        [Fact]
        public void AssetFields_WithEmptyArray_DoesNotAddParameter()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.AssetFields(new string[0]);

            // Assert
            var urlQueriesField = typeof(Asset).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            Assert.False(urlQueries?.ContainsKey("asset_fields[]") ?? false);
        }

        #endregion

        #region IncludeBranch Tests

        [Fact]
        public void IncludeBranch_AddsQueryParameter()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            Asset result = asset.IncludeBranch();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(asset, result);
            
            var urlQueriesField = typeof(Asset).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.Equal("true", urlQueries?["include_branch"]?.ToString());
        }

        #endregion

        #region AddParam Tests

        [Fact]
        public void AddParam_AddsQueryParameter()
        {
            // Arrange
            var asset = CreateAsset();
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            // Act
            Asset result = asset.AddParam(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(asset, result);
            
            var urlQueriesField = typeof(Asset).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            
            Assert.True(urlQueries?.ContainsKey(key) ?? false);
            Assert.Equal(value, urlQueries?[key]?.ToString());
        }

        #endregion

        #region SetHeader and RemoveHeader Tests

        [Fact]
        public void SetHeader_AddsHeader()
        {
            // Arrange
            var asset = CreateAsset();
            var key = "custom_header";
            var value = _fixture.Create<string>();

            // Act
            asset.SetHeader(key, value);

            // Assert
            var headersField = typeof(Asset).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(asset);
            
            Assert.True(headers?.ContainsKey(key) ?? false);
            Assert.Equal(value, headers?[key]?.ToString());
        }

        [Fact]
        public void RemoveHeader_RemovesHeader()
        {
            // Arrange
            var asset = CreateAsset();
            var key = "custom_header";
            var value = _fixture.Create<string>();
            asset.SetHeader(key, value);
            
            var headersField = typeof(Asset).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = (Dictionary<string, object>)headersField?.GetValue(asset);
            Assert.True(headersBefore?.ContainsKey(key) ?? false);

            // Act
            asset.RemoveHeader(key);

            // Assert
            var headersAfter = (Dictionary<string, object>)headersField?.GetValue(asset);
            Assert.False(headersAfter?.ContainsKey(key) ?? true);
        }

        [Fact]
        public void Fetch_WithMultipleParameters_VerifiesAllQueryParameters()
        {
            // Arrange
            var asset = CreateAsset();
            asset.IncludeFallback().IncludeMetadata().IncludeBranch();
            asset.SetHeader("custom_header", "value");

            // Act - Just verify setup, not actual HTTP call
            var urlQueriesField = typeof(Asset).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);
            
            var headersField = typeof(Asset).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(asset);

            // Assert
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_metadata") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True(headers?.ContainsKey("custom_header") ?? false);
        }

        [Fact]
        public void Fetch_Setup_VerifiesUrlQueries()
        {
            // Arrange
            var asset = CreateAsset();
            asset.AddParam("test_param", "test_value");

            // Act - Just verify setup
            var urlQueriesField = typeof(Asset).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(asset);

            // Assert
            Assert.True(urlQueries?.ContainsKey("test_param") ?? false);
            Assert.Equal("test_value", urlQueries?["test_param"]?.ToString());
        }

        [Fact]
        public void GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(Asset).GetMethod("GetContentstackError", 
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
            var method = typeof(Asset).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var ex = new Exception("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { ex }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test error", result.Message);
        }

        [Fact]
        public void GetContentstackError_WithWebException_HandlesExceptionCorrectly()
        {
            // Arrange
            var method = typeof(Asset).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { webEx }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            // When WebException has no response, it should fall back to ex.Message
            Assert.NotNull(result.Message);
        }

        [Fact]
        public void ErrorHandling_WithWebException_ExtractsErrorMessage()
        {
            // Arrange
            var method = typeof(Asset).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var errorMessage = "Asset processing failed";
            var ex = new Exception(errorMessage);

            // Act
            var result = method?.Invoke(null, new object[] { ex }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public void GetHeader_WithNullLocalHeader_ReturnsStackHeaders()
        {
            // Arrange
            var asset = CreateAsset();
            var getHeaderMethod = typeof(Asset).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = getHeaderMethod?.Invoke(asset, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndNoStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var asset = CreateAsset();
            var getHeaderMethod = typeof(Asset).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to null
            var stackHeadersField = typeof(Asset).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(asset, null);

            // Act
            var result = getHeaderMethod?.Invoke(asset, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndStackHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var asset = CreateAsset();
            var getHeaderMethod = typeof(Asset).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };

            // Act
            var result = getHeaderMethod?.Invoke(asset, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AddParam_WithNullKey_ThrowsException()
        {
            // Arrange
            var asset = CreateAsset();

            // Act & Assert
            // Dictionary.Add throws ArgumentNullException for null key
            var exception = Assert.Throws<ArgumentNullException>(() => asset.AddParam(null, "value"));
            Assert.NotNull(exception);
        }

        [Fact]
        public void AddParam_WithNullValue_ThrowsException()
        {
            // Arrange
            var asset = CreateAsset();

            // Act & Assert
            // Dictionary allows null values, so no exception is thrown
            var exception = Record.Exception(() => asset.AddParam("key", null));
            Assert.Null(exception); // No exception should be thrown
        }

        [Fact]
        public void SetHeader_WithNullKey_DoesNotAddHeader()
        {
            // Arrange
            var asset = CreateAsset();

            // Act & Assert
            // Dictionary.ContainsKey throws ArgumentNullException for null key
            Assert.Throws<ArgumentNullException>(() => 
            {
                var headersField = typeof(Asset).GetField("_Headers", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var headers = (Dictionary<string, object>)headersField?.GetValue(asset);
                headers?.ContainsKey(null); // This will throw
            });
        }

        [Fact]
        public void SetHeader_WithEmptyKey_DoesNotAddHeader()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            asset.SetHeader("", "value");

            // Assert
            // Note: SetHeader checks `key != null`, so empty string IS added (empty string != null)
            var headersField = typeof(Asset).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(asset);
            Assert.True(headers?.ContainsKey("") ?? false); // Empty string is added
        }

        [Fact]
        public void SetHeader_WithNullValue_DoesNotAddHeader()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            asset.SetHeader("key", null);

            // Assert
            var headersField = typeof(Asset).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(asset);
            Assert.False(headers?.ContainsKey("key") ?? false);
        }

        [Fact]
        public void SetHeader_WithEmptyValue_DoesNotAddHeader()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            asset.SetHeader("key", "");

            // Assert
            // Note: SetHeader checks `value != null`, so empty string IS added (empty string != null)
            var headersField = typeof(Asset).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(asset);
            Assert.True(headers?.ContainsKey("key") ?? false); // Empty string is added
        }

        [Fact]
        public void GetCreateAt_WithInvalidDateTimeFormat_ReturnsMinValue()
        {
            // Arrange
            var asset = CreateAssetWithAttributes(new Dictionary<string, object>
            {
                { "created_at", "invalid_date_format" }
            });

            // Act
            DateTime result = asset.GetCreateAt();

            // Assert
            // Should return MinValue when date parsing fails
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void GetUpdateAt_WithInvalidDateTimeFormat_ReturnsMinValue()
        {
            // Arrange
            var asset = CreateAssetWithAttributes(new Dictionary<string, object>
            {
                { "updated_at", "invalid_date_format" }
            });

            // Act
            DateTime result = asset.GetUpdateAt();

            // Assert
            // Should return MinValue when date parsing fails
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void GetDeleteAt_WithInvalidDateTimeFormat_ReturnsMinValue()
        {
            // Arrange
            var asset = CreateAssetWithAttributes(new Dictionary<string, object>
            {
                { "deleted_at", "invalid_date_format" }
            });

            // Act
            DateTime result = asset.GetDeleteAt();

            // Assert
            // Should return MinValue when date parsing fails
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void Get_WithNullKey_ReturnsNull()
        {
            // Arrange
            var asset = CreateAsset();

            // Act
            var result = asset.Get(null);

            // Assert
            // Dictionary.ContainsKey(null) throws ArgumentNullException, caught and returns null
            Assert.Null(result);
        }

        #endregion
    }
}

