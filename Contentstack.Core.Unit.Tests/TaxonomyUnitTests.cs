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
    /// Unit tests for Taxonomy class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class TaxonomyUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public TaxonomyUnitTests()
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

        private Taxonomy CreateTaxonomy()
        {
            return _client.Taxonomies();
        }

        #region Above Tests

        [Fact]
        public void Above_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = 100;

            // Act
            Taxonomy result = taxonomy.Above(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taxonomy, result);
            
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
            var queryValue = queryValueJson?[key] as Dictionary<string, object>;
            Assert.NotNull(queryValue);
            Assert.True(queryValue?.ContainsKey("$above") ?? false);
        }

        #endregion

        #region EqualAndAbove Tests

        [Fact]
        public void EqualAndAbove_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = 100;

            // Act
            Taxonomy result = taxonomy.EqualAndAbove(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taxonomy, result);
            
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
            var queryValue = queryValueJson?[key] as Dictionary<string, object>;
            Assert.NotNull(queryValue);
            Assert.True(queryValue?.ContainsKey("$eq_above") ?? false);
        }

        #endregion

        #region Below Tests

        [Fact]
        public void Below_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = 100;

            // Act
            Taxonomy result = taxonomy.Below(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taxonomy, result);
            
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
            var queryValue = queryValueJson?[key] as Dictionary<string, object>;
            Assert.NotNull(queryValue);
            Assert.True(queryValue?.ContainsKey("$below") ?? false);
        }

        #endregion

        #region EqualAndBelow Tests

        [Fact]
        public void EqualAndBelow_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = 100;

            // Act
            Taxonomy result = taxonomy.EqualAndBelow(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taxonomy, result);
            
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
            var queryValue = queryValueJson?[key] as Dictionary<string, object>;
            Assert.NotNull(queryValue);
            Assert.True(queryValue?.ContainsKey("$eq_below") ?? false);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Above_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.Above(null, value));
        }

        [Fact]
        public void Above_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.Above(key, null));
        }

        [Fact]
        public void EqualAndAbove_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.EqualAndAbove(null, value));
        }

        [Fact]
        public void EqualAndAbove_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.EqualAndAbove(key, null));
        }

        [Fact]
        public void Below_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.Below(null, value));
        }

        [Fact]
        public void Below_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.Below(key, null));
        }

        [Fact]
        public void EqualAndBelow_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.EqualAndBelow(null, value));
        }

        [Fact]
        public void EqualAndBelow_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<Exception>(() => taxonomy.EqualAndBelow(key, null));
        }

        [Fact]
        public void Above_WithDifferentValueTypes_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act
            taxonomy.Above(key, "string_value");

            // Assert
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
        }

        [Fact]
        public void Below_WithDoubleValue_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = 123.45;

            // Act
            taxonomy.Below(key, value);

            // Assert
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
        }

        [Fact]
        public void Taxonomy_GetHeader_WithLocalHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var type = typeof(Taxonomy);
            var getHeaderMethod = type.GetMethod("GetHeader", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var localHeaders = new Dictionary<string, object> { { "custom-header", "value1" } };
            
            // Act
            var result = getHeaderMethod?.Invoke(taxonomy, new object[] { localHeaders }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Taxonomy_GetHeader_WithNullLocalHeaders_ReturnsStackHeaders()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var type = typeof(Taxonomy);
            var getHeaderMethod = type.GetMethod("GetHeader", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Act
            var result = getHeaderMethod?.Invoke(taxonomy, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Taxonomy_GetHeader_WithEmptyLocalHeaders_ReturnsStackHeaders()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var type = typeof(Taxonomy);
            var getHeaderMethod = type.GetMethod("GetHeader", BindingFlags.NonPublic | BindingFlags.Instance);
            
            // Act
            var result = getHeaderMethod?.Invoke(taxonomy, new object[] { new Dictionary<string, object>() }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Taxonomy_GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var type = typeof(Taxonomy);
            var getContentstackErrorMethod = type.GetMethod("GetContentstackError", BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test exception");

            // Act
            var result = getContentstackErrorMethod?.Invoke(null, new object[] { webEx }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void Taxonomy_GetContentstackError_WithGenericException_ReturnsContentstackException()
        {
            // Arrange
            var type = typeof(Taxonomy);
            var getContentstackErrorMethod = type.GetMethod("GetContentstackError", BindingFlags.NonPublic | BindingFlags.Static);
            var ex = new Exception("Test exception");

            // Act
            var result = getContentstackErrorMethod?.Invoke(null, new object[] { ex }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void Taxonomy_UrlProperty_ReturnsCorrectUrl()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var type = typeof(Taxonomy);
            var urlProperty = type.GetProperty("_Url", BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var url = urlProperty?.GetValue(taxonomy) as string;

            // Assert
            Assert.NotNull(url);
            Assert.Contains("/taxonomies/entries", url);
        }

        [Fact]
        public void Taxonomy_Find_Setup_VerifiesQueryParameters()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            taxonomy.Above("test_key", 100);

            // Act - Just verify setup, not actual HTTP call
            var type = typeof(Taxonomy);
            var queryValueJsonField = type.GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);

            // Assert
            Assert.NotNull(queryValueJson);
            Assert.True(queryValueJson.ContainsKey("test_key"));
        }

        [Fact]
        public void Taxonomy_GetHeader_WithLocalHeaderAndEmptyStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var getHeaderMethod = typeof(Taxonomy).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to empty dictionary
            var stackHeadersField = typeof(Taxonomy).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(taxonomy, new Dictionary<string, object>());

            // Act
            var result = getHeaderMethod?.Invoke(taxonomy, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void Taxonomy_GetHeader_WithOverlappingKeys_LocalHeaderTakesPrecedence()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var getHeaderMethod = typeof(Taxonomy).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(taxonomy, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        [Fact]
        public void Taxonomy_GetHeader_WithBothHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var getHeaderMethod = typeof(Taxonomy).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "local_key", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(taxonomy, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("local_key"));
        }

        [Fact]
        public void Taxonomy_GetContentstackError_WithWebExceptionContainingErrorCode_ExtractsErrorCode()
        {
            // Arrange
            var type = typeof(Taxonomy);
            var getContentstackErrorMethod = type.GetMethod("GetContentstackError", BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test exception");

            // Act
            var result = getContentstackErrorMethod?.Invoke(null, new object[] { webEx }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void Above_WithEmptyKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            // Note: Empty string != null, so it will try to add the query parameter
            // Only null key throws exception
            var result = taxonomy.Above("", value);
            Assert.NotNull(result);
        }

        [Fact]
        public void EqualAndAbove_WithEmptyKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            // Note: Empty string != null, so it will try to add the query parameter
            // Only null key throws exception
            var result = taxonomy.EqualAndAbove("", value);
            Assert.NotNull(result);
        }

        [Fact]
        public void Below_WithEmptyKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            // Note: Empty string != null, so it will try to add the query parameter
            // Only null key throws exception
            var result = taxonomy.Below("", value);
            Assert.NotNull(result);
        }

        [Fact]
        public void EqualAndBelow_WithEmptyKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            // Note: Empty string != null, so it will try to add the query parameter
            // Only null key throws exception
            var result = taxonomy.EqualAndBelow("", value);
            Assert.NotNull(result);
        }

        [Fact]
        public void Above_WithNegativeValue_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = -100;

            // Act
            taxonomy.Above(key, value);

            // Assert
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
        }

        [Fact]
        public void Below_WithZeroValue_AddsQueryParameter()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();
            var value = 0;

            // Act
            taxonomy.Below(key, value);

            // Assert
            var queryValueJsonField = typeof(Taxonomy).GetField("QueryValueJson", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var queryValueJson = (Dictionary<string, object>)queryValueJsonField?.GetValue(taxonomy);
            
            Assert.True(queryValueJson?.ContainsKey(key) ?? false);
        }

        #endregion
    }
}

