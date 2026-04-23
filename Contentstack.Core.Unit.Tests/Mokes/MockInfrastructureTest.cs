using System;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Unit.Tests.Mokes;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Test to verify Phase 1 infrastructure setup works correctly
    /// </summary>
    public class MockInfrastructureTest
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public void MockResponse_CreateContentstackResponse_ShouldLoadFromResource()
        {
            // Arrange & Act
            var response = MockResponse.CreateContentstackResponse("MockResponse.txt");

            // Assert
            Assert.NotNull(response);
            Assert.Contains("{}", response);
        }

        [Fact]
        public void MockResponse_CreateContentstackResponseAsJsonObject_ShouldParseJson()
        {
            // Arrange & Act
            var jsonObject = MockResponse.CreateContentstackResponseAsJsonObject("MockResponse.txt");

            // Assert
            Assert.NotNull(jsonObject);
        }

        [Fact]
        public void ContentstackResponse_OpenResponse_ShouldReturnResponseString()
        {
            // Arrange
            var responseString = "{\"test\": \"value\"}";
            var response = new ContentstackResponse(responseString);

            // Act
            var result = response.OpenResponse();

            // Assert
            Assert.Equal(responseString, result);
        }

        [Fact]
        public void ContentstackResponse_OpenJsonObjectResponse_ShouldParseJson()
        {
            // Arrange
            var responseString = "{\"test\": \"value\"}";
            var response = new ContentstackResponse(responseString);

            // Act
            var jsonObject = response.OpenJsonObjectResponse();

            // Assert
            Assert.NotNull(jsonObject);
            Assert.Equal("value", jsonObject["test"]?.GetValue<string>());
        }

        [Fact]
        public void MockHttpHandler_WithStringResponse_ShouldCreateHandler()
        {
            // Arrange
            var mockResponse = "{\"test\": \"value\"}";

            // Act
            var handler = new MockHttpHandler(mockResponse);

            // Assert
            Assert.NotNull(handler);
        }

        [Fact]
        public void MockHttpHandler_WithContentstackResponse_ShouldCreateHandler()
        {
            // Arrange
            var response = new ContentstackResponse("{\"test\": \"value\"}");

            // Act
            var handler = new MockHttpHandler(response);

            // Assert
            Assert.NotNull(handler);
        }

        [Fact]
        public void Utilities_GetResourceText_ShouldLoadResource()
        {
            // Arrange & Act
            var resourceText = Utilities.GetResourceText("MockResponse.txt");

            // Assert
            Assert.NotNull(resourceText);
            Assert.NotEmpty(resourceText);
        }
    }
}



