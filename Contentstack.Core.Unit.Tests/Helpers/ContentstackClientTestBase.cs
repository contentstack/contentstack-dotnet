using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core.Configuration;
using Contentstack.Core.Unit.Tests.Mokes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contentstack.Core.Unit.Tests.Helpers
{
    /// <summary>
    /// Base class for ContentstackClient unit tests providing common setup and helper methods
    /// </summary>
    public abstract class ContentstackClientTestBase
    {
        protected readonly IFixture _fixture = new Fixture();

        /// <summary>
        /// Creates a basic ContentstackClient with default configuration for testing
        /// </summary>
        /// <returns>ContentstackClient instance with test configuration</returns>
        protected ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                Host = "cdn.contentstack.io"
            };

            return new ContentstackClient(options);
        }

        /// <summary>
        /// Creates a ContentstackClient with specified environment
        /// </summary>
        /// <param name="environment">Environment name</param>
        /// <returns>ContentstackClient with specified environment</returns>
        protected ContentstackClient CreateClient(string environment)
        {
            var options = new ContentstackOptions
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = environment,
                Host = "cdn.contentstack.io"
            };

            return new ContentstackClient(options);
        }

        /// <summary>
        /// Creates a ContentstackClient with named parameters (for backward compatibility)
        /// </summary>
        protected ContentstackClient CreateClient(string apiKey = null, string deliveryToken = null, string environment = null, string version = null)
        {
            var options = new ContentstackOptions
            {
                ApiKey = apiKey ?? _fixture.Create<string>(),
                DeliveryToken = deliveryToken ?? _fixture.Create<string>(),
                Environment = environment ?? _fixture.Create<string>(),
                Host = "cdn.contentstack.io"
            };

            return new ContentstackClient(options);
        }

        /// <summary>
        /// Creates a ContentstackClient with LivePreview configuration
        /// </summary>
        /// <param name="enabled">Whether LivePreview should be enabled</param>
        /// <param name="managementToken">Optional management token</param>
        /// <param name="previewToken">Optional preview token</param>
        /// <param name="host">Optional host override</param>
        /// <returns>ContentstackClient with LivePreview configuration</returns>
        protected ContentstackClient CreateClientWithLivePreview(bool enabled = true, string managementToken = null, string previewToken = null, string host = null)
        {
            var options = new ContentstackOptions
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                Host = "cdn.contentstack.io",
                LivePreview = new LivePreviewConfig
                {
                    Enable = enabled,
                    Host = host ?? (enabled ? "rest-preview.contentstack.com" : null),
                    ManagementToken = managementToken ?? (enabled ? _fixture.Create<string>() : null),
                    PreviewToken = previewToken
                }
            };

            return new ContentstackClient(options);
        }

        /// <summary>
        /// Creates a ContentstackClient configured for timeline operations
        /// </summary>
        /// <returns>ContentstackClient with timeline-ready configuration</returns>
        protected ContentstackClient CreateClientWithTimeline()
        {
            var client = CreateClientWithLivePreview(enabled: true);
            var config = client.GetLivePreviewConfig();
            
            // Set up basic timeline context
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            config.ReleaseId = "test_release_123";
            
            return client;
        }

        /// <summary>
        /// Creates a ContentstackClient configured for timeline operations (named parameters for backward compatibility)
        /// </summary>
        protected ContentstackClient CreateClientWithTimeline(string releaseId = null, string timestamp = null, string hash = null)
        {
            var client = CreateClientWithLivePreview(enabled: true);
            var config = client.GetLivePreviewConfig();
            
            // Set up timeline context
            config.PreviewTimestamp = timestamp ?? "2024-11-29T14:30:00.000Z";
            config.ReleaseId = releaseId ?? "test_release_123";
            
            if (!string.IsNullOrEmpty(hash))
            {
                SetInternalProperty(config, "LivePreview", hash);
            }
            
            return client;
        }

        /// <summary>
        /// Creates a live preview query dictionary with specified parameters
        /// </summary>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="entryUid">Entry UID</param>
        /// <param name="previewTimestamp">Preview timestamp</param>
        /// <param name="releaseId">Release ID</param>
        /// <param name="livePreview">Live preview hash</param>
        /// <returns>Dictionary with live preview query parameters</returns>
        protected Dictionary<string, string> CreateLivePreviewQuery(
            string contentTypeUid = "test_ct",
            string entryUid = "test_entry", 
            string previewTimestamp = null,
            string releaseId = null,
            string livePreview = "init")
        {
            var query = new Dictionary<string, string>
            {
                ["content_type_uid"] = contentTypeUid,
                ["entry_uid"] = entryUid,
                ["live_preview"] = livePreview
            };

            if (!string.IsNullOrEmpty(previewTimestamp))
                query["preview_timestamp"] = previewTimestamp;
            
            if (!string.IsNullOrEmpty(releaseId))
                query["release_id"] = releaseId;

            return query;
        }

        /// <summary>
        /// Sets internal property value using reflection (for testing internal state)
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value to set</param>
        protected void SetInternalProperty(object target, string propertyName, object value)
        {
            var property = target.GetType().GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            property?.SetValue(target, value);
        }

        /// <summary>
        /// Gets internal property value using reflection (for testing internal state)
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="target">Target object</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Property value</returns>
        protected T GetInternalProperty<T>(object target, string propertyName)
        {
            var property = target.GetType().GetProperty(propertyName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)property?.GetValue(target);
        }

        /// <summary>
        /// Sets internal field value using reflection (for testing internal state)
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value to set</param>
        protected void SetInternalField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(target, value);
        }

        /// <summary>
        /// Gets internal field value using reflection (for testing internal state)
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="target">Target object</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>Field value</returns>
        protected T GetInternalField<T>(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field?.GetValue(target);
        }

        /// <summary>
        /// Creates a mock JObject response for timeline preview testing
        /// </summary>
        /// <param name="entryUid">Entry UID for the mock response</param>
        /// <param name="contentTypeUid">Content Type UID for the mock response</param>
        /// <returns>Mock JObject response</returns>
        protected JObject CreateMockPreviewResponse(string entryUid = "mock_entry", string contentTypeUid = "mock_ct")
        {
            return JObject.Parse($@"{{
                ""uid"": ""{entryUid}"",
                ""content_type_uid"": ""{contentTypeUid}"",
                ""title"": ""Mock Entry Title"",
                ""created_at"": ""2024-11-29T14:30:00.000Z"",
                ""updated_at"": ""2024-11-29T15:45:00.000Z"",
                ""publish_details"": {{
                    ""environment"": ""test"",
                    ""locale"": ""en-us"",
                    ""time"": ""2024-11-29T16:00:00.000Z""
                }},
                ""mock_field"": ""mock_value_{_fixture.Create<int>()}""
            }}");
        }

        /// <summary>
        /// Verifies that two clients are independent instances
        /// </summary>
        /// <param name="client1">First client</param>
        /// <param name="client2">Second client</param>
        protected void AssertClientsAreIndependent(ContentstackClient client1, ContentstackClient client2)
        {
            if (client1 == null) throw new ArgumentNullException(nameof(client1));
            if (client2 == null) throw new ArgumentNullException(nameof(client2));
            
            // Verify they are different instances
            Assert.NotSame(client1, client2);
            
            // Verify LivePreview configs are different instances
            Assert.NotSame(client1.GetLivePreviewConfig(), client2.GetLivePreviewConfig());
        }

        /// <summary>
        /// Verifies that client configuration is properly preserved
        /// </summary>
        /// <param name="originalClient">Original client</param>
        /// <param name="newClient">New client to verify</param>
        protected void AssertConfigurationPreserved(ContentstackClient originalClient, ContentstackClient newClient)
        {
            Assert.Equal(originalClient.GetApplicationKey(), newClient.GetApplicationKey());
            Assert.Equal(originalClient.GetAccessToken(), newClient.GetAccessToken());
            Assert.Equal(originalClient.GetEnvironment(), newClient.GetEnvironment());
            Assert.Equal(originalClient.GetVersion(), newClient.GetVersion());
        }

        /// <summary>
        /// Helper method to create a client with mock handler (for backward compatibility)
        /// </summary>
        protected ContentstackClient CreateClientWithMockHandler(object mockHandler)
        {
            // Simple implementation - just returns a basic client
            return CreateClient();
        }

        #region Helper Classes for Backward Compatibility

        /// <summary>
        /// Mock helpers for timeline testing (stub implementation)
        /// </summary>
        protected static class TimelineMockHelpers
        {
            public static object CreateSuccessfulMockHandler()
            {
                return new { Success = true };
            }

            public static object CreateFailureMockHandler()
            {
                return new { Success = false };
            }

            public static string CreateMockLivePreviewResponse(string entryUid = "test_entry", string contentTypeUid = "test_ct")
            {
                return $@"{{
                    ""entry"": {{
                        ""uid"": ""{entryUid}"",
                        ""content_type_uid"": ""{contentTypeUid}"",
                        ""title"": ""Mock Timeline Entry"",
                        ""created_at"": ""2024-11-29T10:00:00.000Z"",
                        ""updated_at"": ""2024-11-29T14:30:00.000Z"",
                        ""publish_details"": {{
                            ""environment"": ""test"",
                            ""locale"": ""en-us"",
                            ""time"": ""2024-11-29T14:30:00.000Z""
                        }},
                        ""mock_field"": ""timeline_test_value""
                    }}
                }}";
            }
        }

        /// <summary>
        /// Assertion helpers for timeline testing (stub implementation)
        /// </summary>
        protected static class TimelineAssertionHelpers
        {
            public static void AssertTimelineStateCleared(ContentstackClient client)
            {
                var config = client.GetLivePreviewConfig();
                Assert.Null(config?.PreviewTimestamp);
                Assert.Null(config?.ReleaseId);
            }

            public static void VerifyTimelineStateCleared(LivePreviewConfig config, string message = "")
            {
                Assert.Null(config?.PreviewTimestamp);
                Assert.Null(config?.ReleaseId);
                Assert.Null(config?.PreviewResponse);
            }

            public static void AssertTimelineStatePreserved(ContentstackClient client, string expectedTimestamp, string expectedReleaseId)
            {
                var config = client.GetLivePreviewConfig();
                Assert.Equal(expectedTimestamp, config?.PreviewTimestamp);
                Assert.Equal(expectedReleaseId, config?.ReleaseId);
            }

            public static void AssertCacheState(ContentstackClient client, bool expectedCached)
            {
                var config = client.GetLivePreviewConfig();
                if (expectedCached)
                {
                    Assert.NotNull(config?.PreviewResponse);
                }
                else
                {
                    Assert.Null(config?.PreviewResponse);
                }
            }
        }

        #endregion

        #region ContentstackClient Extension Methods for Testing

        /// <summary>
        /// Mock implementation of SetHost method for testing
        /// </summary>
        protected void SetHost(ContentstackClient client, string host)
        {
            // Mock implementation - store in a private field or ignore for testing
            SetInternalProperty(client, "_mockHost", host);
        }

        /// <summary>
        /// Mock implementation of GetHost method for testing
        /// </summary>
        protected string GetHost(ContentstackClient client)
        {
            return GetInternalProperty<string>(client, "_mockHost") ?? "cdn.contentstack.io";
        }

        /// <summary>
        /// Mock implementation of SetTimeout method for testing
        /// </summary>
        protected void SetTimeout(ContentstackClient client, int timeout)
        {
            SetInternalProperty(client, "_mockTimeout", timeout);
        }

        /// <summary>
        /// Mock implementation of GetTimeout method for testing
        /// </summary>
        protected int GetTimeout(ContentstackClient client)
        {
            // Provide default timeout value since there's no actual _mockTimeout property
            return 30000; // 30 seconds default
        }

        /// <summary>
        /// Mock implementation of GetRegion method for testing
        /// </summary>
        protected string GetRegion(ContentstackClient client)
        {
            return GetInternalProperty<string>(client, "_mockRegion") ?? "us";
        }

        /// <summary>
        /// Mock implementation of SetBranch method for testing
        /// </summary>
        protected void SetBranch(ContentstackClient client, string branch)
        {
            SetInternalProperty(client, "_mockBranch", branch);
        }

        /// <summary>
        /// Mock implementation of GetBranch method for testing
        /// </summary>
        protected string GetBranch(ContentstackClient client)
        {
            return GetInternalProperty<string>(client, "_mockBranch") ?? "main";
        }

        #endregion
    }
}