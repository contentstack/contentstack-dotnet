using System;
using System.Collections.Generic;
using System.Net;
using AutoFixture;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for ContentstackException class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class ContentstackExceptionUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        #region Initialization Tests

        [Fact]
        public void ContentstackException_DefaultConstructor_InitializesCorrectly()
        {
            // Act
            var exception = new ContentstackException();

            // Assert
            Assert.NotNull(exception);
            // ErrorMessage is initialized as string.Empty in private field, but Message may be null
            Assert.Equal(string.Empty, exception.ErrorMessage ?? string.Empty);
            // Message property from base Exception class may be null initially
            Assert.True(string.IsNullOrEmpty(exception.Message));
            Assert.Equal(0, exception.ErrorCode);
            Assert.Null(exception.Errors);
            Assert.Equal(ResponseType.Network, exception.ResponseType);
        }

        [Fact]
        public void ContentstackException_WithErrorMessage_InitializesCorrectly()
        {
            // Arrange
            var errorMessage = _fixture.Create<string>();

            // Act
            var exception = new ContentstackException(errorMessage);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(errorMessage, exception.ErrorMessage);
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public void ContentstackException_WithException_InitializesCorrectly()
        {
            // Arrange
            var sourceException = new Exception("Source exception message");

            // Act
            var exception = new ContentstackException(sourceException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(sourceException.Message, exception.ErrorMessage);
            Assert.Equal(sourceException.Message, exception.Message);
            // The base constructor sets InnerException from sourceException.InnerException (which is null here)
            // So InnerException will be null if source exception doesn't have one
            Assert.Null(exception.InnerException);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ErrorMessage_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var exception = new ContentstackException();
            var errorMessage = _fixture.Create<string>();

            // Act
            exception.ErrorMessage = errorMessage;

            // Assert
            Assert.Equal(errorMessage, exception.ErrorMessage);
            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public void ErrorCode_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var exception = new ContentstackException();
            var errorCode = _fixture.Create<int>();

            // Act
            exception.ErrorCode = errorCode;

            // Assert
            Assert.Equal(errorCode, exception.ErrorCode);
        }

        [Fact]
        public void StatusCode_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var exception = new ContentstackException();
            var statusCode = HttpStatusCode.BadRequest;

            // Act
            exception.StatusCode = statusCode;

            // Assert
            Assert.Equal(statusCode, exception.StatusCode);
        }

        [Fact]
        public void ResponseType_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var exception = new ContentstackException();
            var responseType = ResponseType.Cache;

            // Act
            exception.ResponseType = responseType;

            // Assert
            Assert.Equal(responseType, exception.ResponseType);
        }

        [Fact]
        public void Errors_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var exception = new ContentstackException();
            var errors = new Dictionary<string, object>
            {
                { "field1", "Error message 1" },
                { "field2", "Error message 2" }
            };

            // Act
            exception.Errors = errors;

            // Assert
            Assert.NotNull(exception.Errors);
            Assert.Equal(2, exception.Errors.Count);
            Assert.Equal("Error message 1", exception.Errors["field1"]);
            Assert.Equal("Error message 2", exception.Errors["field2"]);
        }

        #endregion

        #region Complete Exception Tests

        [Fact]
        public void ContentstackException_WithAllPropertiesSet_ReturnsAllValues()
        {
            // Arrange
            var errorMessage = _fixture.Create<string>();
            var errorCode = 404;
            var statusCode = HttpStatusCode.NotFound;
            var responseType = ResponseType.Network;
            var errors = new Dictionary<string, object>
            {
                { "error", "Not found" }
            };

            // Act
            var exception = new ContentstackException(errorMessage)
            {
                ErrorCode = errorCode,
                StatusCode = statusCode,
                ResponseType = responseType,
                Errors = errors
            };

            // Assert
            Assert.Equal(errorMessage, exception.ErrorMessage);
            Assert.Equal(errorMessage, exception.Message);
            Assert.Equal(errorCode, exception.ErrorCode);
            Assert.Equal(statusCode, exception.StatusCode);
            Assert.Equal(responseType, exception.ResponseType);
            Assert.NotNull(exception.Errors);
            Assert.Single(exception.Errors);
        }

        #endregion
    }
}

