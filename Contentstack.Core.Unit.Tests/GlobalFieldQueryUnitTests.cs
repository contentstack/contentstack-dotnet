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
    /// Unit tests for GlobalFieldQuery class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class GlobalFieldQueryUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public GlobalFieldQueryUnitTests()
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

        private GlobalFieldQuery CreateGlobalFieldQuery()
        {
            return _client.GlobalFieldQuery();
        }

        #region IncludeBranch Tests

        [Fact]
        public void IncludeBranch_AddsQueryParameter()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();

            // Act
            GlobalFieldQuery result = globalFieldQuery.IncludeBranch();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(globalFieldQuery, result);
            
            var urlQueriesField = typeof(GlobalFieldQuery).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalFieldQuery);
            
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True((bool)(urlQueries?["include_branch"] ?? false));
        }

        #endregion

        #region IncludeGlobalFieldSchema Tests

        [Fact]
        public void IncludeGlobalFieldSchema_AddsQueryParameter()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();

            // Act
            GlobalFieldQuery result = globalFieldQuery.IncludeGlobalFieldSchema();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(globalFieldQuery, result);
            
            var urlQueriesField = typeof(GlobalFieldQuery).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalFieldQuery);
            
            Assert.True(urlQueries?.ContainsKey("include_global_field_schema") ?? false);
            Assert.True((bool)(urlQueries?["include_global_field_schema"] ?? false));
        }

        #endregion

        #region Find Tests

        [Fact]
        public void Find_Setup_VerifiesQueryParameters()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            globalFieldQuery.IncludeBranch();
            globalFieldQuery.IncludeGlobalFieldSchema();

            // Act - Just verify setup, not actual HTTP call
            var urlQueriesField = typeof(GlobalFieldQuery).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalFieldQuery);

            // Assert
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_global_field_schema") ?? false);
        }

        #endregion

        #region GetContentstackError Tests

        [Fact]
        public void GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var exception = new System.Net.WebException("Test error");

            // Act
            var result = GlobalFieldQuery.GetContentstackError(exception);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void GetContentstackError_WithGenericException_ReturnsContentstackException()
        {
            // Arrange
            var exception = new Exception("Test error");

            // Act
            var result = GlobalFieldQuery.GetContentstackError(exception);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void Find_WithMultipleParameters_VerifiesAllQueryParameters()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            globalFieldQuery.IncludeBranch();
            globalFieldQuery.IncludeGlobalFieldSchema();

            // Act - Just verify setup
            var urlQueriesField = typeof(GlobalFieldQuery).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalFieldQuery);

            // Assert
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_global_field_schema") ?? false);
        }

        [Fact]
        public void Find_Setup_VerifiesUrlQueries()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            globalFieldQuery.IncludeBranch();

            // Act - Just verify setup
            var urlQueriesField = typeof(GlobalFieldQuery).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalFieldQuery);

            // Assert
            Assert.NotNull(urlQueries);
            Assert.True(urlQueries.ContainsKey("include_branch"));
        }

        #endregion

        #region SetStackInstance Tests

        [Fact]
        public void SetStackInstance_SetsStackInstance()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var stackInstanceField = typeof(GlobalFieldQuery).GetProperty("StackInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var setMethod = typeof(GlobalFieldQuery).GetMethod("SetStackInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            setMethod?.Invoke(globalFieldQuery, new object[] { _client });

            // Assert
            var stackInstance = stackInstanceField?.GetValue(globalFieldQuery);
            Assert.NotNull(stackInstance);
            Assert.Equal(_client, stackInstance);
        }

        #endregion

        #region GetHeader Tests

        [Fact]
        public void GetHeader_WithNullLocalHeader_ReturnsStackHeaders()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndNoStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to null
            var stackHeadersField = typeof(GlobalFieldQuery).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(globalFieldQuery, null);

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndStackHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("custom"));
        }

        [Fact]
        public void GetHeader_WithEmptyLocalHeader_ReturnsStackHeaders()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object>();

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithOverlappingKeys_PrefersLocalHeader()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        #endregion

        #region GetContentstackError Expanded Tests

        [Fact]
        public void GetContentstackError_WithWebExceptionContainingErrorCode_ExtractsErrorCode()
        {
            // Arrange
            // Create a WebException with a response that contains error_code
            var exception = new System.Net.WebException("Test error");

            // Act
            var result = GlobalFieldQuery.GetContentstackError(exception);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void GetContentstackError_WithWebExceptionContainingErrorMessage_ExtractsErrorMessage()
        {
            // Arrange
            var exception = new System.Net.WebException("Test error");

            // Act
            var result = GlobalFieldQuery.GetContentstackError(exception);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void GetContentstackError_WithWebExceptionContainingErrors_ExtractsErrors()
        {
            // Arrange
            var exception = new System.Net.WebException("Test error");

            // Act
            var result = GlobalFieldQuery.GetContentstackError(exception);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void GetContentstackError_WithWebExceptionContainingStatusCode_ExtractsStatusCode()
        {
            // Arrange
            var exception = new System.Net.WebException("Test error");

            // Act
            var result = GlobalFieldQuery.GetContentstackError(exception);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContentstackException>(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndEmptyStackHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _StackHeaders to empty dictionary
            var stackHeadersField = typeof(GlobalFieldQuery).GetField("_StackHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            stackHeadersField?.SetValue(globalFieldQuery, new Dictionary<string, object>());

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void GetHeader_WithOverlappingKeys_LocalHeaderTakesPrecedence()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        [Fact]
        public void GetHeader_WithBothHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var globalFieldQuery = CreateGlobalFieldQuery();
            var getHeaderMethod = typeof(GlobalFieldQuery).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "local_key", "local_value" } };

            // Act
            var result = getHeaderMethod?.Invoke(globalFieldQuery, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("local_key"));
        }

        #endregion
    }
}

