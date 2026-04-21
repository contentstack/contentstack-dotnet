using System;
using AutoFixture;
using Contentstack.Core.Configuration;
using Contentstack.Core.Unit.Tests.Helpers;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for LivePreviewConfig.IsCachedPreviewForCurrentQuery() method
    /// Tests fingerprint-based cache hit/miss logic for Timeline Preview
    /// </summary>
    [Trait("Category", "TimelinePreview")]
    [Trait("Category", "Fingerprint")]
    [Trait("Category", "Cache")]
    public class LivePreviewConfigFingerprintTests : ContentstackClientTestBase
    {
        #region Cache Hit Scenarios (All Fingerprints Match)

        [Fact]
        public void IsCachedPreviewForCurrentQuery_AllFingerprintsMatch_ReturnsTrue()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithLivePreview("test_hash_456")
                .WithPreviewResponse()
                .WithMatchingFingerprints()
                .Build();

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_OnlyTimestampSet_MatchingFingerprint_ReturnsTrue()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithPreviewResponse()
                .Build();

            // Set matching fingerprint only for timestamp
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", null);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_OnlyReleaseIdSet_MatchingFingerprint_ReturnsTrue()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithReleaseId("test_release_123")
                .WithPreviewResponse()
                .Build();

            // Set matching fingerprint only for release ID
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", null);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_OnlyLivePreviewSet_MatchingFingerprint_ReturnsTrue()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithLivePreview("test_hash_789")
                .WithPreviewResponse()
                .Build();

            // Set matching fingerprint only for live preview
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(config, "LivePreview"));

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_AllNullValues_MatchingNullFingerprints_ReturnsTrue()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewResponse()
                .Build();

            // Ensure all values and fingerprints are null
            config.PreviewTimestamp = null;
            config.ReleaseId = null;
            SetInternalProperty(config, "LivePreview", null);
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", null);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_EmptyStrings_MatchingEmptyFingerprints_ReturnsTrue()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewResponse()
                .Build();

            // Set empty strings for current values and matching fingerprints
            config.PreviewTimestamp = "";
            config.ReleaseId = "";
            SetInternalProperty(config, "LivePreview", "");
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "");

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        #endregion

        #region Cache Miss Scenarios

        [Fact]
        public void IsCachedPreviewForCurrentQuery_NullPreviewResponse_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithMatchingFingerprints()
                .Build();

            config.PreviewResponse = null;

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_DifferentTimestamp_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithPreviewResponse()
                .Build();

            // Set different fingerprint for timestamp
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T10:00:00.000Z");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(config, "LivePreview"));

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_DifferentReleaseId_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithPreviewResponse()
                .Build();

            // Set different fingerprint for release ID
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "different_release_456");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(config, "LivePreview"));

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_DifferentLivePreviewHash_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithLivePreview("current_hash")
                .WithPreviewResponse()
                .Build();

            // Set different fingerprint for live preview hash
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "different_hash");

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_NullCurrentTimestamp_NonNullFingerprint_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithReleaseId("test_release_123")
                .WithPreviewResponse()
                .Build();

            config.PreviewTimestamp = null;
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T14:30:00.000Z");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_NonNullCurrentTimestamp_NullFingerprint_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithPreviewResponse()
                .Build();

            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_NullCurrentReleaseId_NonNullFingerprint_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithPreviewResponse()
                .Build();

            config.ReleaseId = null;
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "fingerprint_release");

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_PartialMatch_TimestampAndReleaseMatch_LivePreviewDifferent_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release_123")
                .WithLivePreview("current_hash")
                .WithPreviewResponse()
                .Build();

            // Set matching fingerprints for timestamp and release, but different for live preview
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "different_hash");

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void IsCachedPreviewForCurrentQuery_EmptyStringVsNull_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewResponse()
                .Build();

            // Current values are empty strings, fingerprints are null
            config.PreviewTimestamp = "";
            config.ReleaseId = "";
            SetInternalProperty(config, "LivePreview", "");
            
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", null);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached); // Empty string != null
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_NullVsEmptyString_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewResponse()
                .Build();

            // Current values are null, fingerprints are empty strings
            config.PreviewTimestamp = null;
            config.ReleaseId = null;
            SetInternalProperty(config, "LivePreview", null);
            
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "");

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached); // null != empty string
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_CaseSensitive_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("Test_Release_123")
                .WithLivePreview("Test_Hash")
                .WithPreviewResponse()
                .Build();

            // Set fingerprints with different casing
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "test_release_123"); // Different case
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "test_hash"); // Different case

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached); // Should be case-sensitive
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_WhitespaceSignificant_ReturnsFalse()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithPreviewResponse()
                .Build();

            // Set fingerprint with extra whitespace
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", " 2024-11-29T14:30:00.000Z ");

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.False(isCached); // Whitespace should be significant
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_UnicodeCharacters_MatchesCorrectly()
        {
            // Arrange
            var unicodeTimestamp = "2024-11-29T14:30:00.000Z-üñíçødé";
            var unicodeReleaseId = "release-测试-🚀";
            var unicodeHash = "hash-αβγδε-💯";

            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp(unicodeTimestamp)
                .WithReleaseId(unicodeReleaseId)
                .WithLivePreview(unicodeHash)
                .WithPreviewResponse()
                .Build();

            // Set matching unicode fingerprints
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", unicodeTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", unicodeReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", unicodeHash);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_VeryLongStrings_HandlesCorrectly()
        {
            // Arrange
            var longTimestamp = "2024-11-29T14:30:00.000Z" + new string('x', 1000);
            var longReleaseId = "release_" + new string('y', 1000);
            var longHash = "hash_" + new string('z', 1000);

            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp(longTimestamp)
                .WithReleaseId(longReleaseId)
                .WithLivePreview(longHash)
                .WithPreviewResponse()
                .Build();

            // Set matching long fingerprints
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", longTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", longReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", longHash);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_SpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var specialTimestamp = "2024-11-29T14:30:00.000Z!@#$%^&*()";
            var specialReleaseId = "release_<>&\"'`~";
            var specialHash = "hash_{[]}|\\:;?,./";

            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp(specialTimestamp)
                .WithReleaseId(specialReleaseId)
                .WithLivePreview(specialHash)
                .WithPreviewResponse()
                .Build();

            // Set matching special character fingerprints
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", specialTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", specialReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", specialHash);

            // Act
            var isCached = config.IsCachedPreviewForCurrentQuery();

            // Assert
            Assert.True(isCached);
        }

        #endregion

        #region Performance

        [Fact]
        public void IsCachedPreviewForCurrentQuery_Performance_FastComparison()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("performance_test_release")
                .WithLivePreview("performance_test_hash")
                .WithPreviewResponse()
                .WithMatchingFingerprints()
                .Build();

            var iterations = 10000;
            var startTime = DateTime.UtcNow;

            // Act - Multiple cache checks
            for (int i = 0; i < iterations; i++)
            {
                var result = config.IsCachedPreviewForCurrentQuery();
                Assert.True(result); // Verify correctness during performance test
            }

            var duration = DateTime.UtcNow - startTime;

            // Assert - Should be very fast (under 50ms for 10,000 checks)
            Assert.True(duration.TotalMilliseconds < 50, 
                $"Cache check took {duration.TotalMilliseconds}ms for {iterations} operations");
        }

        #endregion
    }
}