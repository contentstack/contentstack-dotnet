using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for GlobalField class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class GlobalFieldUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public GlobalFieldUnitTests()
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

        private GlobalField CreateGlobalField(string globalFieldUid = "test_global_field")
        {
            return _client.GlobalField(globalFieldUid);
        }

        #region IncludeBranch Tests

        [Fact]
        public void IncludeBranch_AddsQueryParameter()
        {
            // Arrange
            var globalField = CreateGlobalField();

            // Act
            GlobalField result = globalField.IncludeBranch();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(globalField, result);
            
            var urlQueriesField = typeof(GlobalField).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
                    var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalField);
                    
                    Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
                    // Value can be stored as bool "True" or string "true"
                    var branchValue = urlQueries?["include_branch"];
                    Assert.True(branchValue != null && (branchValue.ToString().Equals("True", StringComparison.OrdinalIgnoreCase) || (bool)branchValue));
        }

        #endregion

        #region IncludeGlobalFieldSchema Tests

        [Fact]
        public void IncludeGlobalFieldSchema_AddsQueryParameter()
        {
            // Arrange
            var globalField = CreateGlobalField();

            // Act
            GlobalField result = globalField.IncludeGlobalFieldSchema();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(globalField, result);
            
            var urlQueriesField = typeof(GlobalField).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(globalField);
            
            Assert.True(urlQueries?.ContainsKey("include_global_field_schema") ?? false);
            // Value can be stored as bool "True" or string "true"
            var schemaValue = urlQueries?["include_global_field_schema"];
            Assert.True(schemaValue != null && (schemaValue.ToString().Equals("True", StringComparison.OrdinalIgnoreCase) || (bool)schemaValue));
        }

        #endregion

        #region SetHeader and RemoveHeader Tests

        [Fact]
        public void SetHeader_AddsHeader()
        {
            // Arrange
            var globalField = CreateGlobalField();
            var key = "custom_header";
            var value = _fixture.Create<string>();

            // Act
            globalField.SetHeader(key, value);

            // Assert
            var headersField = typeof(GlobalField).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(globalField);
            
            Assert.True(headers?.ContainsKey(key) ?? false);
            Assert.Equal(value, headers?[key]?.ToString());
        }

        [Fact]
        public void RemoveHeader_RemovesHeader()
        {
            // Arrange
            var globalField = CreateGlobalField();
            var key = "custom_header";
            var value = _fixture.Create<string>();
            globalField.SetHeader(key, value);
            
            var headersField = typeof(GlobalField).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headersBefore = (Dictionary<string, object>)headersField?.GetValue(globalField);
            Assert.True(headersBefore?.ContainsKey(key) ?? false);

            // Act
            globalField.RemoveHeader(key);

            // Assert
            var headersAfter = (Dictionary<string, object>)headersField?.GetValue(globalField);
            Assert.False(headersAfter?.ContainsKey(key) ?? true);
        }

        #endregion

        #region GetContentstackError Tests

        [Fact]
        public void GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(GlobalField).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { webEx }) as Contentstack.Core.Internals.ContentstackException;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetContentstackError_WithGenericException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(GlobalField).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var ex = new Exception("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { ex }) as Contentstack.Core.Internals.ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test error", result.Message);
        }

        [Fact]
        public void GetContentstackError_WithWebException_HandlesExceptionCorrectly()
        {
            // Arrange
            var method = typeof(GlobalField).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { webEx }) as Contentstack.Core.Internals.ContentstackException;

            // Assert
            Assert.NotNull(result);
            // When WebException has no response, it should fall back to ex.Message
            Assert.NotNull(result.Message);
        }

        [Fact]
        public void ErrorHandling_WithWebException_ExtractsErrorMessage()
        {
            // Arrange
            var method = typeof(GlobalField).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var errorMessage = "Global field error";
            var ex = new Exception(errorMessage);

            // Act
            var result = method?.Invoke(null, new object[] { ex }) as Contentstack.Core.Internals.ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(errorMessage, result.Message);
        }

        #endregion
    }
}

