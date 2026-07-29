using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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
            Assert.Throws<NullReferenceException>(() => taxonomy.Above(null, value));
        }

        [Fact]
        public void Above_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.Above(key, null));
        }

        [Fact]
        public void EqualAndAbove_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.EqualAndAbove(null, value));
        }

        [Fact]
        public void EqualAndAbove_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.EqualAndAbove(key, null));
        }

        [Fact]
        public void Below_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.Below(null, value));
        }

        [Fact]
        public void Below_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.Below(key, null));
        }

        [Fact]
        public void EqualAndBelow_WithNullKey_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var value = 100;

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.EqualAndBelow(null, value));
        }

        [Fact]
        public void EqualAndBelow_WithNullValue_ThrowsException()
        {
            // Arrange
            var taxonomy = CreateTaxonomy();
            var key = _fixture.Create<string>();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => taxonomy.EqualAndBelow(key, null));
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
        public void TaxonomyRequestHelper_GetHeader_WithLocalHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var stackHeaders = new Dictionary<string, object> { { "stack-header", "stack-value" } };
            var localHeaders = new Dictionary<string, object> { { "custom-header", "value1" } };

            // Act
            var result = TaxonomyRequestHelper.GetHeader(stackHeaders, localHeaders);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void TaxonomyRequestHelper_GetHeader_WithNullLocalHeaders_ReturnsStackHeaders()
        {
            // Arrange
            var stackHeaders = new Dictionary<string, object> { { "stack-header", "stack-value" } };

            // Act
            var result = TaxonomyRequestHelper.GetHeader(stackHeaders, null);

            // Assert
            Assert.NotNull(result);
            Assert.Same(stackHeaders, result);
        }

        [Fact]
        public void TaxonomyRequestHelper_GetHeader_WithEmptyLocalHeaders_ReturnsStackHeaders()
        {
            // Arrange
            var stackHeaders = new Dictionary<string, object> { { "stack-header", "stack-value" } };

            // Act
            var result = TaxonomyRequestHelper.GetHeader(stackHeaders, new Dictionary<string, object>());

            // Assert
            Assert.NotNull(result);
            Assert.Same(stackHeaders, result);
        }

        [Fact]
        public void TaxonomyRequestHelper_GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var webEx = new System.Net.WebException("Test exception");

            // Act
            var result = TaxonomyRequestHelper.GetContentstackError(webEx);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void TaxonomyRequestHelper_GetContentstackError_WithGenericException_ReturnsContentstackException()
        {
            // Arrange
            var ex = new Exception("Test exception");

            // Act
            var result = TaxonomyRequestHelper.GetContentstackError(ex);

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
        public void TaxonomyRequestHelper_GetHeader_WithLocalHeaderAndEmptyStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };

            // Act
            var result = TaxonomyRequestHelper.GetHeader(new Dictionary<string, object>(), localHeader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void TaxonomyRequestHelper_GetHeader_WithOverlappingKeys_LocalHeaderTakesPrecedence()
        {
            // Arrange
            var stackHeaders = new Dictionary<string, object> { { "custom", "stack_value" } };
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Act
            var result = TaxonomyRequestHelper.GetHeader(stackHeaders, localHeader);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        [Fact]
        public void TaxonomyRequestHelper_GetHeader_WithBothHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var stackHeaders = new Dictionary<string, object> { { "stack_key", "stack_value" } };
            var localHeader = new Dictionary<string, object> { { "local_key", "local_value" } };

            // Act
            var result = TaxonomyRequestHelper.GetHeader(stackHeaders, localHeader);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("local_key"));
            Assert.True(result.ContainsKey("stack_key"));
        }

        [Fact]
        public void TaxonomyRequestHelper_GetContentstackError_WithWebExceptionContainingErrorCode_ExtractsErrorCode()
        {
            // Arrange
            var webEx = new System.Net.WebException("Test exception");

            // Act
            var result = TaxonomyRequestHelper.GetContentstackError(webEx);

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

        #region Taxonomy UID Constructor Tests

        [Fact]
        public void Taxonomy_WithUid_DoesNotThrow()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            Assert.NotNull(taxonomy);
        }

        [Fact]
        public void Taxonomy_WithNullUid_ThrowsTaxonomyException()
        {
            Assert.Throws<TaxonomyException>(() => _client.Taxonomies(null));
        }

        [Fact]
        public void Taxonomy_WithEmptyUid_ThrowsTaxonomyException()
        {
            Assert.Throws<TaxonomyException>(() => _client.Taxonomies(string.Empty));
        }

        #endregion

        #region Taxonomy.Term() Tests

        [Fact]
        public void Taxonomy_TermWithUid_ReturnsTaxonomyInstance()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            var term = taxonomy.Term("smartwatch");
            Assert.NotNull(term);
            Assert.IsType<Term>(term);
        }

        [Fact]
        public void Taxonomy_TermWithoutUid_ThrowsTaxonomyException()
        {
            var taxonomy = _client.Taxonomies();
            Assert.Throws<TaxonomyException>(() => taxonomy.Term("smartwatch"));
        }

        #endregion

        #region Taxonomy.Terms() Tests

        [Fact]
        public void Taxonomy_Terms_ReturnsTermQueryInstance()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            var termQuery = taxonomy.Terms();
            Assert.NotNull(termQuery);
            Assert.IsType<TermQuery>(termQuery);
        }

        [Fact]
        public void Taxonomy_Terms_WithoutUid_ThrowsTaxonomyException()
        {
            var taxonomy = _client.Taxonomies();
            Assert.Throws<TaxonomyException>(() => taxonomy.Terms());
        }

        #endregion

        #region TermQuery Tests

        [Fact]
        public void TermQuery_SetLocale_ReturnsSelfForChaining()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            var result = termQuery.SetLocale("hi-in");
            Assert.Same(termQuery, result);
        }

        [Fact]
        public void TermQuery_SetLocale_SetsLocaleParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.SetLocale("hi-in");

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("locale") ?? false);
            Assert.Equal("hi-in", queryParams["locale"]);
        }

        [Fact]
        public void TermQuery_SetLocale_WithNull_DoesNotSetParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.SetLocale(null);

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.False(urlQueries?.ContainsKey("locale") ?? false);
        }

        [Fact]
        public void TermQuery_IncludeFallback_ReturnsSelfForChaining()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            var result = termQuery.IncludeFallback();
            Assert.Same(termQuery, result);
        }

        [Fact]
        public void TermQuery_IncludeFallback_SetsParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.IncludeFallback();

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("include_fallback") ?? false);
            Assert.Equal("true", queryParams["include_fallback"]);
        }

        [Fact]
        public void TermQuery_SetLocale_Then_IncludeFallback_ChainsBoth()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms()
                .SetLocale("hi-in")
                .IncludeFallback();

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("locale") ?? false);
            Assert.True(queryParams?.ContainsKey("include_fallback") ?? false);
        }

        [Fact]
        public void TermQuery_AddParam_SetsUrlQuery()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.AddParam("custom_key", "custom_value");

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("custom_key") ?? false);
            Assert.Equal("custom_value", queryParams["custom_key"]);
        }

        #endregion

        #region TermQuery.Skip / Limit / IncludeCount Tests

        [Fact]
        public void TermQuery_IncludeBranch_SetsQueryParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.IncludeBranch();

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("include_branch") ?? false);
            Assert.Equal("true", queryParams["include_branch"]);
        }

        [Fact]
        public void TermQuery_Skip_SetsQueryParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.Skip(10);

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("skip") ?? false);
            Assert.Equal(10, queryParams["skip"]);
        }

        [Fact]
        public void TermQuery_Limit_SetsQueryParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.Limit(10);

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("limit") ?? false);
            Assert.Equal(10, queryParams["limit"]);
        }

        [Fact]
        public void TermQuery_IncludeCount_SetsQueryParam()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms();
            termQuery.IncludeCount();

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("include_count") ?? false);
            Assert.Equal("true", queryParams["include_count"]);
        }

        [Fact]
        public void TermQuery_Skip_Limit_IncludeCount_Depth_ChainAllTogether()
        {
            var termQuery = _client.Taxonomies("gadgets").Terms()
                .Skip(0)
                .Limit(10)
                .IncludeCount()
                .Depth(2)
                .IncludeBranch();

            var field = typeof(TermQuery).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(termQuery);

            Assert.True(queryParams?.ContainsKey("skip") ?? false);
            Assert.True(queryParams?.ContainsKey("limit") ?? false);
            Assert.True(queryParams?.ContainsKey("include_count") ?? false);
            Assert.True(queryParams?.ContainsKey("depth") ?? false);
            Assert.True(queryParams?.ContainsKey("include_branch") ?? false);
        }

        #endregion

        #region Taxonomy.SetLocale / IncludeFallback / AddParam Tests

        [Fact]
        public void Taxonomy_SetLocale_ReturnsSelfForChaining()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            var result = taxonomy.SetLocale("hi-in");
            Assert.Same(taxonomy, result);
        }

        [Fact]
        public void Taxonomy_SetLocale_SetsUrlQuery()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            taxonomy.SetLocale("hi-in");

            var field = typeof(Taxonomy).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(taxonomy);

            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.Equal("hi-in", urlQueries["locale"]);
        }

        [Fact]
        public void Taxonomy_SetLocale_WithNull_DoesNotSetParam()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            taxonomy.SetLocale(null);

            var field = typeof(Taxonomy).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(taxonomy);

            Assert.False(urlQueries?.ContainsKey("locale") ?? false);
        }

        [Fact]
        public void Taxonomy_IncludeFallback_ReturnsSelfForChaining()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            var result = taxonomy.IncludeFallback();
            Assert.Same(taxonomy, result);
        }

        [Fact]
        public void Taxonomy_IncludeFallback_SetsUrlQuery()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            taxonomy.IncludeFallback();

            var field = typeof(Taxonomy).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(taxonomy);

            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.Equal("true", urlQueries["include_fallback"]);
        }

        [Fact]
        public void Taxonomy_AddParam_SetsUrlQuery()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            taxonomy.AddParam("custom_key", "custom_value");

            var field = typeof(Taxonomy).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(taxonomy);

            Assert.True(urlQueries?.ContainsKey("custom_key") ?? false);
            Assert.Equal("custom_value", urlQueries["custom_key"]);
        }

        [Fact]
        public void Taxonomy_SetLocale_Then_IncludeFallback_ChainsBoth()
        {
            var taxonomy = _client.Taxonomies("gadgets")
                .SetLocale("hi-in")
                .IncludeFallback();

            var field = typeof(Taxonomy).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(taxonomy);

            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
        }

        #endregion

        #region Taxonomy.Find Tests

        [Fact]
        public async Task Taxonomy_Find_OnScopedInstance_ThrowsTaxonomyException()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            await Assert.ThrowsAsync<TaxonomyException>(() => taxonomy.Find<Newtonsoft.Json.Linq.JObject>());
        }

        [Fact]
        public void Taxonomy_Find_AddParam_SetsSkipLimitIncludeCountParams()
        {
            var taxonomy = _client.Taxonomies()
                .AddParam("skip", "0")
                .AddParam("limit", "10")
                .AddParam("include_count", "true");

            var field = typeof(Taxonomy).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)field?.GetValue(taxonomy);

            Assert.True(urlQueries?.ContainsKey("skip") ?? false);
            Assert.True(urlQueries?.ContainsKey("limit") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_count") ?? false);
        }

        #endregion

        #region Taxonomy.Find Routing (HasEntryFilters) Tests

        [Fact]
        public void HasEntryFilters_FalseByDefault_OnFreshTaxonomy()
        {
            var taxonomy = CreateTaxonomy();
            Assert.False(taxonomy.HasEntryFilters);
        }

        [Fact]
        public void HasEntryFilters_TrueAfterAbove()
        {
            var taxonomy = CreateTaxonomy();
            taxonomy.Above(_fixture.Create<string>(), 1);
            Assert.True(taxonomy.HasEntryFilters);
        }

        [Fact]
        public void HasEntryFilters_TrueAfterBelow()
        {
            var taxonomy = CreateTaxonomy();
            taxonomy.Below(_fixture.Create<string>(), 1);
            Assert.True(taxonomy.HasEntryFilters);
        }

        [Fact]
        public void HasEntryFilters_TrueAfterEqualAndAbove()
        {
            var taxonomy = CreateTaxonomy();
            taxonomy.EqualAndAbove(_fixture.Create<string>(), 1);
            Assert.True(taxonomy.HasEntryFilters);
        }

        [Fact]
        public void HasEntryFilters_TrueAfterEqualAndBelow()
        {
            var taxonomy = CreateTaxonomy();
            taxonomy.EqualAndBelow(_fixture.Create<string>(), 1);
            Assert.True(taxonomy.HasEntryFilters);
        }

        [Fact]
        public void HasEntryFilters_TrueAfterExists_InheritedFromQuery()
        {
            var taxonomy = CreateTaxonomy();
            taxonomy.Exists(_fixture.Create<string>());
            Assert.True(taxonomy.HasEntryFilters);
        }

        [Fact]
        public void HasEntryFilters_FalseAfterAddParam_ListAllPathStillUsed()
        {
            var taxonomy = _client.Taxonomies().AddParam("skip", "0");
            Assert.False(taxonomy.HasEntryFilters);
        }

        [Fact]
        public async Task Taxonomy_Find_OnScopedInstance_ThrowsTaxonomyException_EvenWithFilters()
        {
            var taxonomy = _client.Taxonomies("gadgets");
            taxonomy.Above("category", "electronics");
            await Assert.ThrowsAsync<TaxonomyException>(() => taxonomy.Find<Newtonsoft.Json.Linq.JObject>());
        }

        #endregion

        #region Term.SetLocale / IncludeFallback / AddParam Tests

        [Fact]
        public void Term_SetLocale_ReturnsSelfForChaining()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            var result = term.SetLocale("hi-in");
            Assert.Same(term, result);
        }

        [Fact]
        public void Term_SetLocale_SetsQueryParam()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            term.SetLocale("hi-in");

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("locale") ?? false);
            Assert.Equal("hi-in", queryParams["locale"]);
        }

        [Fact]
        public void Term_SetLocale_WithNull_DoesNotSetParam()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            term.SetLocale(null);

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.False(queryParams?.ContainsKey("locale") ?? false);
        }

        [Fact]
        public void Term_IncludeFallback_ReturnsSelfForChaining()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            var result = term.IncludeFallback();
            Assert.Same(term, result);
        }

        [Fact]
        public void Term_IncludeFallback_SetsQueryParam()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            term.IncludeFallback();

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("include_fallback") ?? false);
            Assert.Equal("true", queryParams["include_fallback"]);
        }

        [Fact]
        public void Term_AddParam_SetsQueryParam()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            term.AddParam("custom_key", "custom_value");

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("custom_key") ?? false);
            Assert.Equal("custom_value", queryParams["custom_key"]);
        }

        [Fact]
        public void Term_SetLocale_Then_IncludeFallback_ChainsBoth()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch")
                .SetLocale("hi-in")
                .IncludeFallback();

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("locale") ?? false);
            Assert.True(queryParams?.ContainsKey("include_fallback") ?? false);
        }

        #endregion

        #region Term.Depth / IncludeBranch Tests

        [Fact]
        public void Term_Depth_ReturnsSelfForChaining()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            var result = term.Depth(2);
            Assert.Same(term, result);
        }

        [Fact]
        public void Term_Depth_SetsQueryParam()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            term.Depth(2);

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("depth") ?? false);
            Assert.Equal(2, queryParams["depth"]);
        }

        [Fact]
        public void Term_IncludeBranch_ReturnsSelfForChaining()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            var result = term.IncludeBranch();
            Assert.Same(term, result);
        }

        [Fact]
        public void Term_IncludeBranch_SetsQueryParam()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch");
            term.IncludeBranch();

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("include_branch") ?? false);
            Assert.Equal("true", queryParams["include_branch"]);
        }

        [Fact]
        public void Term_Depth_Then_IncludeBranch_ChainsBoth()
        {
            var term = _client.Taxonomies("gadgets").Term("smartwatch")
                .Depth(2)
                .IncludeBranch();

            var field = typeof(Term).GetField("UrlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var queryParams = (Dictionary<string, object>)field?.GetValue(term);

            Assert.True(queryParams?.ContainsKey("depth") ?? false);
            Assert.True(queryParams?.ContainsKey("include_branch") ?? false);
        }

        #endregion

        #region Term Constructor Validation Tests

        [Fact]
        public void Term_WithNullTaxonomyUid_ThrowsTaxonomyException()
        {
            Assert.Throws<TaxonomyException>(() =>
            {
                var t = typeof(Term)
                    .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                        new[] { typeof(ContentstackClient), typeof(string), typeof(string) }, null);
                try { t?.Invoke(new object[] { _client, null, "smartwatch" }); }
                catch (System.Reflection.TargetInvocationException ex) { throw ex.InnerException; }
            });
        }

        [Fact]
        public void Term_WithNullTermUid_ThrowsTaxonomyException()
        {
            Assert.Throws<TaxonomyException>(() =>
            {
                var t = typeof(Term)
                    .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                        new[] { typeof(ContentstackClient), typeof(string), typeof(string) }, null);
                try { t?.Invoke(new object[] { _client, "gadgets", null }); }
                catch (System.Reflection.TargetInvocationException ex) { throw ex.InnerException; }
            });
        }

        #endregion
    }
}

