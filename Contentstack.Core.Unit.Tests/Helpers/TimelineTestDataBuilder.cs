using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core.Configuration;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Unit.Tests.Helpers
{
    /// <summary>
    /// Fluent builder for creating Timeline Preview test data and configurations
    /// </summary>
    public class TimelineTestDataBuilder
    {
        private readonly IFixture _fixture = new Fixture();
        private LivePreviewConfig _config;

        private TimelineTestDataBuilder()
        {
            _config = new LivePreviewConfig();
        }

        /// <summary>
        /// Creates a new TimelineTestDataBuilder instance
        /// </summary>
        /// <returns>New builder instance</returns>
        public static TimelineTestDataBuilder New()
        {
            return new TimelineTestDataBuilder();
        }

        /// <summary>
        /// Sets the preview timestamp for timeline operations
        /// </summary>
        /// <param name="timestamp">ISO 8601 timestamp string</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithPreviewTimestamp(string timestamp)
        {
            _config.PreviewTimestamp = timestamp;
            return this;
        }

        /// <summary>
        /// Sets the preview timestamp using a DateTime
        /// </summary>
        /// <param name="dateTime">DateTime to convert to ISO 8601 string</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithPreviewTimestamp(DateTime dateTime)
        {
            _config.PreviewTimestamp = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return this;
        }

        /// <summary>
        /// Sets the release ID for timeline operations
        /// </summary>
        /// <param name="releaseId">Release identifier</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithReleaseId(string releaseId)
        {
            _config.ReleaseId = releaseId;
            return this;
        }

        /// <summary>
        /// Sets the live preview hash
        /// </summary>
        /// <param name="hash">Live preview hash</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithLivePreview(string hash)
        {
            SetInternalProperty(_config, "LivePreview", hash);
            return this;
        }

        /// <summary>
        /// Sets the content type UID for the configuration
        /// </summary>
        /// <param name="contentTypeUid">Content type identifier</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithContentTypeUid(string contentTypeUid)
        {
            SetInternalProperty(_config, "ContentTypeUID", contentTypeUid);
            return this;
        }

        /// <summary>
        /// Sets the entry UID for the configuration
        /// </summary>
        /// <param name="entryUid">Entry identifier</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithEntryUid(string entryUid)
        {
            SetInternalProperty(_config, "EntryUID", entryUid);
            return this;
        }

        /// <summary>
        /// Sets the management token
        /// </summary>
        /// <param name="token">Management token</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithManagementToken(string token)
        {
            _config.ManagementToken = token;
            return this;
        }

        /// <summary>
        /// Sets the preview token
        /// </summary>
        /// <param name="token">Preview token</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithPreviewToken(string token)
        {
            _config.PreviewToken = token;
            return this;
        }

        /// <summary>
        /// Enables or disables live preview
        /// </summary>
        /// <param name="enabled">Whether live preview should be enabled</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithEnabled(bool enabled)
        {
            _config.Enable = enabled;
            return this;
        }

        /// <summary>
        /// Sets the preview host
        /// </summary>
        /// <param name="host">Preview host URL</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithHost(string host)
        {
            _config.Host = host;
            return this;
        }

        /// <summary>
        /// Sets a mock preview response
        /// </summary>
        /// <param name="response">JObject response</param>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithPreviewResponse(JObject response = null)
        {
            _config.PreviewResponse = response ?? CreateDefaultPreviewResponse();
            return this;
        }

        /// <summary>
        /// Sets matching fingerprints to create a cache hit scenario
        /// </summary>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithMatchingFingerprints()
        {
            SetInternalProperty(_config, "PreviewResponseFingerprintPreviewTimestamp", _config.PreviewTimestamp);
            SetInternalProperty(_config, "PreviewResponseFingerprintReleaseId", _config.ReleaseId);
            SetInternalProperty(_config, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(_config, "LivePreview"));
            return this;
        }

        /// <summary>
        /// Sets non-matching fingerprints to create a cache miss scenario
        /// </summary>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithNonMatchingFingerprints()
        {
            SetInternalProperty(_config, "PreviewResponseFingerprintPreviewTimestamp", $"different_{_fixture.Create<string>()}");
            SetInternalProperty(_config, "PreviewResponseFingerprintReleaseId", $"different_{_fixture.Create<string>()}");
            SetInternalProperty(_config, "PreviewResponseFingerprintLivePreview", $"different_{_fixture.Create<string>()}");
            return this;
        }

        /// <summary>
        /// Clears all fingerprints (simulates no previous cache)
        /// </summary>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithNoFingerprints()
        {
            SetInternalProperty(_config, "PreviewResponseFingerprintPreviewTimestamp", null);
            SetInternalProperty(_config, "PreviewResponseFingerprintReleaseId", null);
            SetInternalProperty(_config, "PreviewResponseFingerprintLivePreview", null);
            return this;
        }

        /// <summary>
        /// Creates a default timeline configuration
        /// </summary>
        /// <returns>Builder instance for chaining</returns>
        public TimelineTestDataBuilder WithDefaultTimelineConfig()
        {
            return WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                   .WithReleaseId("default_release_123")
                   .WithLivePreview("default_hash_456")
                   .WithContentTypeUid("default_ct")
                   .WithEntryUid("default_entry")
                   .WithEnabled(true)
                   .WithHost("rest-preview.contentstack.com")
                   .WithManagementToken(_fixture.Create<string>());
        }

        /// <summary>
        /// Builds and returns the configured LivePreviewConfig
        /// </summary>
        /// <returns>Configured LivePreviewConfig instance</returns>
        public LivePreviewConfig Build()
        {
            return _config;
        }

        /// <summary>
        /// Creates a valid preview response JObject for testing
        /// </summary>
        /// <param name="entryUid">Entry UID for the response</param>
        /// <param name="contentTypeUid">Content Type UID for the response</param>
        /// <returns>Mock JObject preview response</returns>
        public JObject CreateValidPreviewResponse(string entryUid = null, string contentTypeUid = null)
        {
            entryUid ??= _fixture.Create<string>();
            contentTypeUid ??= _fixture.Create<string>();

            return JObject.Parse($@"{{
                ""uid"": ""{entryUid}"",
                ""content_type_uid"": ""{contentTypeUid}"",
                ""title"": ""Timeline Preview Test Entry"",
                ""created_at"": ""2024-11-29T10:00:00.000Z"",
                ""updated_at"": ""2024-11-29T14:30:00.000Z"",
                ""publish_details"": {{
                    ""environment"": ""test"",
                    ""locale"": ""en-us"",
                    ""time"": ""{_config.PreviewTimestamp ?? "2024-11-29T14:30:00.000Z"}""
                }},
                ""test_field"": ""timeline_test_value_{_fixture.Create<int>()}"",
                ""metadata"": {{
                    ""timeline_marker"": ""{DateTime.UtcNow.Ticks}""
                }}
            }}");
        }

        /// <summary>
        /// Creates a preview response for a specific timeline scenario
        /// </summary>
        /// <param name="scenario">Scenario identifier</param>
        /// <returns>Scenario-specific JObject response</returns>
        public JObject CreateScenarioResponse(string scenario)
        {
            var baseResponse = CreateValidPreviewResponse();
            baseResponse["scenario"] = scenario;
            baseResponse["timestamp"] = _config.PreviewTimestamp;
            baseResponse["release_id"] = _config.ReleaseId;
            return baseResponse;
        }

        /// <summary>
        /// Creates a live preview query dictionary from the current configuration
        /// </summary>
        /// <returns>Dictionary suitable for LivePreviewQueryAsync</returns>
        public Dictionary<string, string> CreateQuery()
        {
            var query = new Dictionary<string, string>();

            var contentTypeUid = GetInternalProperty<string>(_config, "ContentTypeUID");
            var entryUid = GetInternalProperty<string>(_config, "EntryUID");
            var livePreview = GetInternalProperty<string>(_config, "LivePreview");

            if (!string.IsNullOrEmpty(contentTypeUid))
                query["content_type_uid"] = contentTypeUid;

            if (!string.IsNullOrEmpty(entryUid))
                query["entry_uid"] = entryUid;

            if (!string.IsNullOrEmpty(livePreview))
                query["live_preview"] = livePreview;

            if (!string.IsNullOrEmpty(_config.PreviewTimestamp))
                query["preview_timestamp"] = _config.PreviewTimestamp;

            if (!string.IsNullOrEmpty(_config.ReleaseId))
                query["release_id"] = _config.ReleaseId;

            return query;
        }

        #region Private Helper Methods

        private JObject CreateDefaultPreviewResponse()
        {
            return CreateValidPreviewResponse();
        }

        private void SetInternalProperty(object target, string propertyName, object value)
        {
            var property = target.GetType().GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            property?.SetValue(target, value);
        }

        private T GetInternalProperty<T>(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)property?.GetValue(target);
        }

        #endregion
    }
}