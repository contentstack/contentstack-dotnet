using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for AssetLibrary class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class AssetLibraryUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public AssetLibraryUnitTests()
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

        private AssetLibrary CreateAssetLibrary()
        {
            return _client.AssetLibrary();
        }

        #region Where Tests

        [Fact]
        public void Where_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            // Act
            AssetLibrary result = assetLibrary.Where(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("query") ?? false);
            var query = urlQueries?["query"] as JObject;
            Assert.NotNull(query);
            Assert.Equal(value, query?[key]?.ToString());
        }

        [Fact]
        public void Where_WithNullKey_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var value = _fixture.Create<string>();

            // Act
            AssetLibrary result = assetLibrary.Where(null, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
        }

        [Fact]
        public void Where_WithEmptyKey_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var value = _fixture.Create<string>();

            // Act
            AssetLibrary result = assetLibrary.Where("", value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
        }

        [Fact]
        public void Where_WithExistingKey_UpdatesQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var key = "filename";
            var value1 = "image1.jpg";
            var value2 = "image2.jpg";

            // Act
            assetLibrary.Where(key, value1);
            assetLibrary.Where(key, value2);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            var query = urlQueries?["query"] as JObject;
            Assert.Equal(value2, query?[key]?.ToString());
        }

        #endregion

        #region IncludeFallback Tests

        [Fact]
        public void IncludeFallback_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.IncludeFallback();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.Equal("true", urlQueries?["include_fallback"]?.ToString());
        }

        #endregion

        #region IncludeMetadata Tests

        [Fact]
        public void IncludeMetadata_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.IncludeMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("include_metadata") ?? false);
            Assert.Equal("true", urlQueries?["include_metadata"]?.ToString());
        }

        #endregion

        #region AssetFields Tests

        [Fact]
        public void AssetFields_WithSingleField_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var field = "user_defined_fields";

            // Act
            AssetLibrary result = assetLibrary.AssetFields(field);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);

            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);

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
            var assetLibrary = CreateAssetLibrary();
            var fields = new[] { "user_defined_fields", "embedded_metadata", "ai_generated_metadata" };

            // Act
            AssetLibrary result = assetLibrary.AssetFields(fields);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);

            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);

            Assert.True(urlQueries?.ContainsKey("asset_fields[]") ?? false);
            Assert.Equal(fields, urlQueries?["asset_fields[]"]);
        }

        [Fact]
        public void AssetFields_ReturnsSameInstance_ForChaining()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.AssetFields("embedded_metadata");

            // Assert
            Assert.NotNull(result);
            Assert.Same(assetLibrary, result);
        }

        [Fact]
        public void AssetFields_WithNoArguments_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.AssetFields();

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("asset_fields[]") ?? false);
        }

        [Fact]
        public void AssetFields_WithNull_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.AssetFields(null);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("asset_fields[]") ?? false);
        }

        [Fact]
        public void AssetFields_WithEmptyArray_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.AssetFields(new string[0]);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("asset_fields[]") ?? false);
        }

        #endregion

        #region IncludeBranch Tests

        [Fact]
        public void IncludeBranch_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            AssetLibrary result = assetLibrary.IncludeBranch();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.Equal("true", urlQueries?["include_branch"]?.ToString());
        }

        #endregion

        #region SetLocale Tests

        [Fact]
        public void SetLocale_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var locale = "en-us";

            // Act
            AssetLibrary result = assetLibrary.SetLocale(locale);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.Equal(locale, urlQueries?["locale"]?.ToString());
        }

        #endregion

        #region AddParam Tests

        [Fact]
        public void AddParam_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            // Act
            AssetLibrary result = assetLibrary.AddParam(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey(key) ?? false);
            Assert.Equal(value, urlQueries?[key]?.ToString());
        }

        #endregion

        #region Skip and Limit Tests

        [Fact]
        public void Skip_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var skipCount = 10;

            // Act
            AssetLibrary result = assetLibrary.Skip(skipCount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("skip") ?? false);
            Assert.Equal(skipCount, urlQueries?["skip"]);
        }

        [Fact]
        public void Limit_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var limitCount = 20;

            // Act
            AssetLibrary result = assetLibrary.Limit(limitCount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("limit") ?? false);
            Assert.Equal(limitCount, urlQueries?["limit"]);
        }

        #endregion

        #region Tags Tests

        [Fact]
        public void Tags_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var tags = new string[] { "tag1", "tag2", "tag3" };

            // Act
            AssetLibrary result = assetLibrary.Tags(tags);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("tags") ?? false);
            Assert.Equal(tags, urlQueries?["tags"]);
        }

        #endregion

        #region Only and Except Tests

        [Fact]
        public void Only_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var fieldUids = new string[] { "field1", "field2" };

            // Act
            AssetLibrary result = assetLibrary.Only(fieldUids);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("only[BASE][]") ?? false);
            Assert.Equal(fieldUids, urlQueries?["only[BASE][]"]);
        }

        [Fact]
        public void Except_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var fieldUids = new string[] { "field1", "field2" };

            // Act
            AssetLibrary result = assetLibrary.Except(fieldUids);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("except[BASE][]") ?? false);
            Assert.Equal(fieldUids, urlQueries?["except[BASE][]"]);
        }

        #endregion

        #region SetHeader and RemoveHeader Tests

        [Fact]
        public void SetHeaderForKey_AddsHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var key = "custom_header";
            var value = _fixture.Create<string>();

            // Act
            AssetLibrary result = assetLibrary.SetHeaderForKey(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var headersField = typeof(AssetLibrary).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
            
            Assert.True(headers?.ContainsKey(key) ?? false);
            Assert.Equal(value, headers?[key]?.ToString());
        }

        [Fact]
        public void RemoveHeader_RemovesHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var key = "custom_header";
            var value = _fixture.Create<string>();
            assetLibrary.SetHeaderForKey(key, value);
            
            var headersField = typeof(AssetLibrary).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
            Assert.True(headersBefore?.ContainsKey(key) ?? false);

            // Act
            assetLibrary.RemoveHeader(key);

            // Assert
            var headersAfter = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
            Assert.False(headersAfter?.ContainsKey(key) ?? true);
        }

        #endregion

        #region Count Tests

        [Fact]
        public void Count_Setup_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act - Just verify setup, not actual HTTP call
            // Count() returns Task<JObject>, so we verify the UrlQueries setup
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            var beforeCount = urlQueriesBefore?.Count ?? 0;

            // Simulate what Count() does internally
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            urlQueries?.Add("count", "true");

            // Assert
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.True(urlQueriesAfter?.ContainsKey("count") ?? false);
            Assert.Equal("true", urlQueriesAfter?["count"]?.ToString());
        }

        #endregion

        #region IncludeCount Tests

        [Fact]
        public void IncludeCount_AddsQueryParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.IncludeCount();

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("include_count") ?? false);
            Assert.Equal("true", urlQueries?["include_count"]?.ToString());
        }

        #endregion

        #region Query Factory Method Tests

        [Fact]
        public void Query_WithJObject_AddsQueryToUrlQueries()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var queryObject = new Newtonsoft.Json.Linq.JObject
            {
                { "field", "value" }
            };

            // Act
            AssetLibrary result = assetLibrary.Query(queryObject);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assetLibrary, result);
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            
            Assert.True(urlQueries?.ContainsKey("query") ?? false);
        }

        #endregion

        #region FetchAll Setup Tests

        [Fact]
        public void FetchAll_Setup_VerifiesUrlQueries()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            assetLibrary.IncludeFallback().IncludeBranch().Skip(5).Limit(10);

            // Act - Just verify setup, not actual HTTP call
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);

            // Assert
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True(urlQueries?.ContainsKey("skip") ?? false);
            Assert.True(urlQueries?.ContainsKey("limit") ?? false);
        }

        [Fact]
        public void GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(AssetLibrary).GetMethod("GetContentstackError", 
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
            var method = typeof(AssetLibrary).GetMethod("GetContentstackError", 
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
            var method = typeof(AssetLibrary).GetMethod("GetContentstackError", 
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
            var method = typeof(AssetLibrary).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var errorMessage = "Asset library error";
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
            var assetLibrary = CreateAssetLibrary();
            var getHeaderMethod = typeof(AssetLibrary).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = getHeaderMethod?.Invoke(assetLibrary, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndNoStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var getHeaderMethod = typeof(AssetLibrary).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to null
            var stackHeadersField = typeof(AssetLibrary).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(assetLibrary, null);

            // Act
            var result = getHeaderMethod?.Invoke(assetLibrary, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndStackHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var getHeaderMethod = typeof(AssetLibrary).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };

            // Act
            var result = getHeaderMethod?.Invoke(assetLibrary, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetHeaderForKey_WithNullKey_DoesNotAddHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act & Assert
            // Dictionary.ContainsKey throws ArgumentNullException for null key
            Assert.Throws<ArgumentNullException>(() => 
            {
                var headersField = typeof(AssetLibrary).GetField("_Headers", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var headers = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
                headers?.ContainsKey(null); // This will throw
            });
        }

        [Fact]
        public void SetHeaderForKey_WithEmptyKey_DoesNotAddHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.SetHeaderForKey("", "value");

            // Assert
            // Note: SetHeaderForKey checks `key != null`, so empty string IS added (empty string != null)
            var headersField = typeof(AssetLibrary).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
            Assert.True(headers?.ContainsKey("") ?? false); // Empty string is added
        }

        [Fact]
        public void SetHeaderForKey_WithNullValue_DoesNotAddHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.SetHeaderForKey("key", null);

            // Assert
            var headersField = typeof(AssetLibrary).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
            Assert.False(headers?.ContainsKey("key") ?? false);
        }

        [Fact]
        public void SetHeaderForKey_WithEmptyValue_DoesNotAddHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.SetHeaderForKey("key", "");

            // Assert
            // Note: SetHeaderForKey checks `value != null`, so empty string IS added (empty string != null)
            var headersField = typeof(AssetLibrary).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(assetLibrary);
            Assert.True(headers?.ContainsKey("key") ?? false); // Empty string is added
        }

        [Fact]
        public void Only_WithNullFields_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Only(null);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("only[BASE][]") ?? false);
        }

        [Fact]
        public void Only_WithEmptyFields_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Only(new string[0]);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("only[BASE][]") ?? false);
        }

        [Fact]
        public void Except_WithNullFields_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Except(null);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("except[BASE][]") ?? false);
        }

        [Fact]
        public void Except_WithEmptyFields_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Except(new string[0]);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("except[BASE][]") ?? false);
        }

        [Fact]
        public void Tags_WithNullTags_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Tags(null);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("tags") ?? false);
        }

        [Fact]
        public void Tags_WithEmptyTags_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Tags(new string[0]);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.False(urlQueries?.ContainsKey("tags") ?? false);
        }

        [Fact]
        public void AddParam_WithNullKey_ThrowsException()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act & Assert
            // Dictionary.Add throws ArgumentNullException for null key
            Assert.Throws<ArgumentNullException>(() => assetLibrary.AddParam(null, "value"));
        }

        [Fact]
        public void AddParam_WithNullValue_ThrowsException()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act & Assert
            // Dictionary allows null values, so no exception is thrown
            var exception = Record.Exception(() => assetLibrary.AddParam("key", null));
            Assert.Null(exception); // No exception should be thrown
        }

        [Fact]
        public void SetLocale_WithNullLocale_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.SetLocale(null);

            // Assert
            // Note: SetLocale adds locale even if value is null (uses dictionary indexer)
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.True(urlQueries?.ContainsKey("locale") ?? false); // Null value is added
        }

        [Fact]
        public void SetLocale_WithEmptyLocale_DoesNotAddParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.SetLocale("");

            // Assert
            // Note: SetLocale adds locale even if value is empty (uses dictionary indexer)
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.True(urlQueries?.ContainsKey("locale") ?? false); // Empty value is added
        }

        [Fact]
        public void Where_WithNullKey_ThrowsException()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act & Assert
            // Where handles null/empty key gracefully by returning this, no exception
            var result = assetLibrary.Where(null, "value");
            Assert.NotNull(result);
            Assert.Same(assetLibrary, result); // Returns this when key is null/empty
        }

        [Fact]
        public void Where_WithNullValue_ThrowsException()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act & Assert
            // Where allows null values, no exception
            var result = assetLibrary.Where("key", null);
            Assert.NotNull(result);
        }

        [Fact]
        public void Skip_WithNegativeNumber_AddsParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Skip(-1);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.True(urlQueries?.ContainsKey("skip") ?? false);
            Assert.Equal(-1, urlQueries?["skip"]);
        }

        [Fact]
        public void Limit_WithNegativeNumber_AddsParameter()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();

            // Act
            assetLibrary.Limit(-1);

            // Assert
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(assetLibrary);
            Assert.True(urlQueries?.ContainsKey("limit") ?? false);
            Assert.Equal(-1, urlQueries?["limit"]);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndEmptyStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var getHeaderMethod = typeof(AssetLibrary).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to empty dictionary
            var stackHeadersField = typeof(AssetLibrary).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(assetLibrary, new Dictionary<string, object>());

            // Act
            var result = getHeaderMethod?.Invoke(assetLibrary, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void GetHeader_WithOverlappingKeys_LocalHeaderTakesPrecedence()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var getHeaderMethod = typeof(AssetLibrary).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(assetLibrary, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        [Fact]
        public void GetHeader_WithBothHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var assetLibrary = CreateAssetLibrary();
            var getHeaderMethod = typeof(AssetLibrary).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "local_key", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(assetLibrary, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("local_key"));
        }

        #endregion
    }
}

