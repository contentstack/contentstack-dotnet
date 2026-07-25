using System;
using System.Net;
using System.Reflection;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for ContentstackOptions class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class ContentstackOptionsUnitTests
    {

        #region Initialization Tests

        [Fact]
        public void ContentstackOptions_Initialization_SetsDefaultValues()
        {
            // Act
            var options = new ContentstackOptions();

            // Assert
            Assert.Equal(30000, options.Timeout);
            Assert.Equal(ContentstackRegion.US, options.Region);
            Assert.Null(options.ApiKey);
            Assert.Null(options.DeliveryToken);
            Assert.Null(options.Environment);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ApiKey_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var apiKey = Guid.NewGuid().ToString("N");

            // Act
            options.ApiKey = apiKey;

            // Assert
            Assert.Equal(apiKey, options.ApiKey);
        }

        [Fact]
        public void DeliveryToken_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var deliveryToken = Guid.NewGuid().ToString("N");

            // Act
            options.DeliveryToken = deliveryToken;

            // Assert
            Assert.Equal(deliveryToken, options.DeliveryToken);
        }

        [Fact]
        public void AccessToken_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var accessToken = Guid.NewGuid().ToString("N");

            // Act — use reflection so we still validate the deprecated property without CS0618 at compile time
            var accessTokenProp = typeof(ContentstackOptions).GetProperty("AccessToken")!;
            accessTokenProp.SetValue(options, accessToken);

            // Assert
            Assert.Equal(accessToken, accessTokenProp.GetValue(options));
        }

        [Fact]
        public void Environment_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var environment = Guid.NewGuid().ToString("N");

            // Act
            options.Environment = environment;

            // Assert
            Assert.Equal(environment, options.Environment);
        }

        [Fact]
        public void Host_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var host = "cdn.contentstack.io";

            // Act
            options.Host = host;

            // Assert
            Assert.Equal(host, options.Host);
        }

        [Fact]
        public void Proxy_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var proxy = new WebProxy("http://proxy.example.com:8080");

            // Act
            options.Proxy = proxy;

            // Assert
            Assert.NotNull(options.Proxy);
            Assert.Equal(proxy, options.Proxy);
        }

        [Fact]
        public void Region_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var region = ContentstackRegion.EU;

            // Act
            options.Region = region;

            // Assert
            Assert.Equal(region, options.Region);
        }

        [Fact]
        public void Version_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var version = "v3";

            // Act
            options.Version = version;

            // Assert
            Assert.Equal(version, options.Version);
        }

        [Fact]
        public void LivePreview_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var livePreview = new LivePreviewConfig
            {
                Enable = true,
                PreviewToken = "preview_token"
            };

            // Act
            options.LivePreview = livePreview;

            // Assert
            Assert.NotNull(options.LivePreview);
            Assert.Equal(livePreview, options.LivePreview);
            Assert.True(options.LivePreview.Enable);
            Assert.Equal("preview_token", options.LivePreview.PreviewToken);
        }

        [Fact]
        public void Branch_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var branch = "main";

            // Act
            options.Branch = branch;

            // Assert
            Assert.Equal(branch, options.Branch);
        }

        [Fact]
        public void Timeout_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var timeout = 60000;

            // Act
            options.Timeout = timeout;

            // Assert
            Assert.Equal(timeout, options.Timeout);
        }

        [Fact]
        public void EarlyAccessHeader_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var options = new ContentstackOptions();
            var headers = new string[] { "header1", "header2" };

            // Act
            options.EarlyAccessHeader = headers;

            // Assert
            Assert.NotNull(options.EarlyAccessHeader);
            Assert.Equal(2, options.EarlyAccessHeader.Length);
            Assert.Equal("header1", options.EarlyAccessHeader[0]);
            Assert.Equal("header2", options.EarlyAccessHeader[1]);
        }

        #endregion

        #region Complete Configuration Tests

        [Fact]
        public void ContentstackOptions_WithAllPropertiesSet_ReturnsAllValues()
        {
            // Arrange
            var apiKey = Guid.NewGuid().ToString("N");
            var deliveryToken = Guid.NewGuid().ToString("N");
            var environment = Guid.NewGuid().ToString("N");
            var host = "cdn.contentstack.io";
            var region = ContentstackRegion.EU;
            var version = "v3";
            var branch = "main";
            var timeout = 60000;
            var livePreview = new LivePreviewConfig { Enable = true };

            // Act
            var options = new ContentstackOptions
            {
                ApiKey = apiKey,
                DeliveryToken = deliveryToken,
                Environment = environment,
                Host = host,
                Region = region,
                Version = version,
                Branch = branch,
                Timeout = timeout,
                LivePreview = livePreview
            };

            // Assert
            Assert.Equal(apiKey, options.ApiKey);
            Assert.Equal(deliveryToken, options.DeliveryToken);
            Assert.Equal(environment, options.Environment);
            Assert.Equal(host, options.Host);
            Assert.Equal(region, options.Region);
            Assert.Equal(version, options.Version);
            Assert.Equal(branch, options.Branch);
            Assert.Equal(timeout, options.Timeout);
            Assert.NotNull(options.LivePreview);
        }

        #endregion
    }
}



