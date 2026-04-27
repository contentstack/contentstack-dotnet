using System;
using System.Reflection;
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

        #region Timeline Preview Property Tests

        [Fact]
        public void PreviewResponse_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var response = JObject.Parse(@"{
                ""entry"": {
                    ""uid"": ""test_entry"",
                    ""title"": ""Test Entry""
                }
            }");

            // Act
            config.PreviewResponse = response;

            // Assert
            Assert.Same(response, config.PreviewResponse);
        }

        [Fact]
        public void PreviewResponse_SetNull_ReturnsNull()
        {
            // Arrange
            var config = new LivePreviewConfig();
            config.PreviewResponse = JObject.Parse(@"{ ""test"": ""value"" }");

            // Act
            config.PreviewResponse = null;

            // Assert
            Assert.Null(config.PreviewResponse);
        }

        #endregion

        #region Fingerprint Property Tests

        [Fact]
        public void PreviewResponseFingerprintPreviewTimestamp_SetAndGet_InternalProperty()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var timestamp = "2024-11-29T14:30:00.000Z";

            // Act
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", timestamp);
            var result = GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp");

            // Assert
            Assert.Equal(timestamp, result);
        }

        [Fact]
        public void PreviewResponseFingerprintReleaseId_SetAndGet_InternalProperty()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var releaseId = _fixture.Create<string>();

            // Act
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", releaseId);
            var result = GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId");

            // Assert
            Assert.Equal(releaseId, result);
        }

        [Fact]
        public void PreviewResponseFingerprintLivePreview_SetAndGet_InternalProperty()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var hash = _fixture.Create<string>();

            // Act
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", hash);
            var result = GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview");

            // Assert
            Assert.Equal(hash, result);
        }

        [Fact]
        public void ContentTypeUID_SetAndGet_InternalProperty()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var contentTypeUid = _fixture.Create<string>();

            // Act
            SetInternalProperty(config, "ContentTypeUID", contentTypeUid);
            var result = GetInternalProperty<string>(config, "ContentTypeUID");

            // Assert
            Assert.Equal(contentTypeUid, result);
        }

        [Fact]
        public void EntryUID_SetAndGet_InternalProperty()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var entryUid = _fixture.Create<string>();

            // Act
            SetInternalProperty(config, "EntryUID", entryUid);
            var result = GetInternalProperty<string>(config, "EntryUID");

            // Assert
            Assert.Equal(entryUid, result);
        }

        [Fact]
        public void LivePreview_SetAndGet_InternalProperty()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var hash = _fixture.Create<string>();

            // Act
            SetInternalProperty(config, "LivePreview", hash);
            var result = GetInternalProperty<string>(config, "LivePreview");

            // Assert
            Assert.Equal(hash, result);
        }

        #endregion

        #region Basic Cache Method Tests

        [Fact]
        public void IsCachedPreviewForCurrentQuery_NullPreviewResponse_ReturnsFalse()
        {
            // Arrange
            var config = new LivePreviewConfig
            {
                PreviewTimestamp = "2024-11-29T14:30:00.000Z",
                ReleaseId = "test_release"
            };
            config.PreviewResponse = null;

            // Act
            var result = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_AllValuesNull_WithPreviewResponse_ReturnsTrue()
        {
            // Arrange
            var config = new LivePreviewConfig();
            config.PreviewResponse = JObject.Parse(@"{ ""entry"": { ""uid"": ""test"" } }");
            
            // Ensure all values and fingerprints are null
            config.PreviewTimestamp = null;
            config.ReleaseId = null;
            SetInternalProperty(config, "LivePreview", null);
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", null);

            // Act
            var result = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_SingleMatchingProperty_ReturnsTrue()
        {
            // Arrange
            var config = new LivePreviewConfig
            {
                PreviewTimestamp = "2024-11-29T14:30:00.000Z"
            };
            config.PreviewResponse = JObject.Parse(@"{ ""entry"": { ""uid"": ""test"" } }");
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);

            // Act
            var result = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_SingleNonMatchingProperty_ReturnsFalse()
        {
            // Arrange
            var config = new LivePreviewConfig
            {
                PreviewTimestamp = "2024-11-29T14:30:00.000Z"
            };
            config.PreviewResponse = JObject.Parse(@"{ ""entry"": { ""uid"": ""test"" } }");
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T10:00:00.000Z");

            // Act
            var result = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Fingerprint Integration Tests

        [Fact]
        public void FingerprintProperties_IndependentValues_NoInterference()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var timestamp = "2024-11-29T14:30:00.000Z";
            var releaseId = "test_release_123";
            var hash = "test_hash_456";

            // Act - Set each fingerprint independently
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", timestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", releaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", hash);

            // Assert - Each maintains its own value
            Assert.Equal(timestamp, GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Equal(releaseId, GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));
            Assert.Equal(hash, GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview"));

            // Modify one - others should be unaffected
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "modified_timestamp");
            
            Assert.Equal("modified_timestamp", GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Equal(releaseId, GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));
            Assert.Equal(hash, GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview"));
        }

        [Fact]
        public void FingerprintProperties_NullValues_HandledCorrectly()
        {
            // Arrange
            var config = new LivePreviewConfig();

            // Act - Set to null
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", null);

            // Assert
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview"));
        }

        [Fact]
        public void ContextProperties_IndependentValues_NoInterference()
        {
            // Arrange
            var config = new LivePreviewConfig();
            var contentTypeUid = "test_ct_123";
            var entryUid = "test_entry_456";
            var livePreview = "test_hash_789";

            // Act
            SetInternalProperty(config, "ContentTypeUID", contentTypeUid);
            SetInternalProperty(config, "EntryUID", entryUid);
            SetInternalProperty(config, "LivePreview", livePreview);

            // Assert
            Assert.Equal(contentTypeUid, GetInternalProperty<string>(config, "ContentTypeUID"));
            Assert.Equal(entryUid, GetInternalProperty<string>(config, "EntryUID"));
            Assert.Equal(livePreview, GetInternalProperty<string>(config, "LivePreview"));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets internal property value using reflection (for testing internal state)
        /// </summary>
        private void SetInternalProperty(object target, string propertyName, object value)
        {
            var property = target.GetType().GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            property?.SetValue(target, value);
        }

        /// <summary>
        /// Gets internal property value using reflection (for testing internal state)
        /// </summary>
        private T GetInternalProperty<T>(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)property?.GetValue(target);
        }

        #endregion
    }
}



