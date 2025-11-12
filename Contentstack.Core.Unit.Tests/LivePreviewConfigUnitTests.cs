using System;
using AutoFixture;
using Contentstack.Core.Configuration;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for LivePreviewConfig class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class LivePreviewConfigUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        #region Initialization Tests

        [Fact]
        public void LivePreviewConfig_Initialization_SetsDefaultValues()
        {
            // Act
            var config = new LivePreviewConfig();

            // Assert
            Assert.False(config.Enable);
            Assert.Null(config.ManagementToken);
            Assert.Null(config.PreviewToken);
            Assert.Null(config.Host);
            Assert.Null(config.ReleaseId);
            Assert.Null(config.PreviewTimestamp);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void ManagementToken_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var token = _fixture.Create<string>();

            // Act
            config.ManagementToken = token;

            // Assert
            Assert.Equal(token, config.ManagementToken);
        }

        [Fact]
        public void PreviewToken_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var token = _fixture.Create<string>();

            // Act
            config.PreviewToken = token;

            // Assert
            Assert.Equal(token, config.PreviewToken);
        }

        [Fact]
        public void Enable_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();

            // Act
            config.Enable = true;

            // Assert
            Assert.True(config.Enable);
        }

        [Fact]
        public void Host_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var host = "preview.contentstack.io";

            // Act
            config.Host = host;

            // Assert
            Assert.Equal(host, config.Host);
        }

        [Fact]
        public void ReleaseId_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var releaseId = _fixture.Create<string>();

            // Act
            config.ReleaseId = releaseId;

            // Assert
            Assert.Equal(releaseId, config.ReleaseId);
        }

        [Fact]
        public void PreviewTimestamp_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var timestamp = _fixture.Create<string>();

            // Act
            config.PreviewTimestamp = timestamp;

            // Assert
            Assert.Equal(timestamp, config.PreviewTimestamp);
        }

        #endregion

        #region Complete Configuration Tests

        [Fact]
        public void LivePreviewConfig_WithAllPropertiesSet_ReturnsAllValues()
        {
            // Arrange
            var managementToken = _fixture.Create<string>();
            var previewToken = _fixture.Create<string>();
            var host = "preview.contentstack.io";
            var releaseId = _fixture.Create<string>();
            var timestamp = _fixture.Create<string>();

            // Act
            var config = new LivePreviewConfig
            {
                ManagementToken = managementToken,
                PreviewToken = previewToken,
                Enable = true,
                Host = host,
                ReleaseId = releaseId,
                PreviewTimestamp = timestamp
            };

            // Assert
            Assert.Equal(managementToken, config.ManagementToken);
            Assert.Equal(previewToken, config.PreviewToken);
            Assert.True(config.Enable);
            Assert.Equal(host, config.Host);
            Assert.Equal(releaseId, config.ReleaseId);
            Assert.Equal(timestamp, config.PreviewTimestamp);
        }

        #endregion
    }
}



