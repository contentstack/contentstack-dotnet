using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentstack.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Mock HTTP handler specialized for Timeline Preview functionality testing
    /// </summary>
    public class TimelineMockHttpHandler : IContentstackPlugin
    {
        private TimeSpan _delay = TimeSpan.Zero;
        private bool _shouldThrowTimeout = false;
        private bool _shouldThrowWebException = false;
        private string _webExceptionMessage = "Network error";
        private JObject _mockResponse;
        private string _expectedEntryUid;
        private string _expectedContentTypeUid;

        /// <summary>
        /// List of requests that have been processed by this handler
        /// </summary>
        public List<HttpWebRequest> Requests { get; private set; } = new List<HttpWebRequest>();

        /// <summary>
        /// Configure handler to return successful live preview responses
        /// </summary>
        /// <param name="entryUid">Expected entry UID in requests</param>
        /// <param name="contentTypeUid">Expected content type UID in requests</param>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler ForSuccessfulLivePreview(string entryUid = "test_entry", string contentTypeUid = "test_ct")
        {
            _expectedEntryUid = entryUid;
            _expectedContentTypeUid = contentTypeUid;

            _mockResponse = JObject.Parse($@"{{
                ""entry"": {{
                    ""uid"": ""{entryUid}"",
                    ""content_type_uid"": ""{contentTypeUid}"",
                    ""title"": ""Timeline Mock Entry"",
                    ""created_at"": ""2024-11-29T10:00:00.000Z"",
                    ""updated_at"": ""2024-11-29T14:30:00.000Z"",
                    ""publish_details"": {{
                        ""environment"": ""test"",
                        ""locale"": ""en-us"",
                        ""time"": ""2024-11-29T14:30:00.000Z""
                    }},
                    ""mock_field"": ""timeline_test_value"",
                    ""_metadata"": {{
                        ""mock_handler"": true,
                        ""timestamp"": ""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}""
                    }}
                }}
            }}");

            return this;
        }

        /// <summary>
        /// Configure handler to return a custom live preview response
        /// </summary>
        /// <param name="response">Custom JObject response</param>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler ForLivePreview(JObject response)
        {
            _mockResponse = response;
            return this;
        }

        /// <summary>
        /// Configure handler to return timeline-specific response for different timestamps
        /// </summary>
        /// <param name="timestamp">Timeline timestamp</param>
        /// <param name="releaseId">Release ID</param>
        /// <param name="entryUid">Entry UID</param>
        /// <param name="contentTypeUid">Content Type UID</param>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler ForTimelineScenario(string timestamp, string releaseId = null, string entryUid = "test_entry", string contentTypeUid = "test_ct")
        {
            _expectedEntryUid = entryUid;
            _expectedContentTypeUid = contentTypeUid;

            _mockResponse = JObject.Parse($@"{{
                ""entry"": {{
                    ""uid"": ""{entryUid}"",
                    ""content_type_uid"": ""{contentTypeUid}"",
                    ""title"": ""Timeline Entry at {timestamp}"",
                    ""created_at"": ""2024-11-29T10:00:00.000Z"",
                    ""updated_at"": ""{timestamp}"",
                    ""publish_details"": {{
                        ""environment"": ""test"",
                        ""locale"": ""en-us"",
                        ""time"": ""{timestamp}"",
                        ""release_id"": ""{releaseId ?? "default_release"}""
                    }},
                    ""timeline_content"": ""Content for {timestamp}"",
                    ""_metadata"": {{
                        ""timeline_timestamp"": ""{timestamp}"",
                        ""timeline_release"": ""{releaseId ?? "default_release"}"",
                        ""mock_handler"": true
                    }}
                }}
            }}");

            return this;
        }

        /// <summary>
        /// Configure handler to simulate network timeout
        /// </summary>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler ThrowTimeout()
        {
            _shouldThrowTimeout = true;
            return this;
        }

        /// <summary>
        /// Configure handler to simulate network error
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler ThrowWebException(string message = "Network error")
        {
            _shouldThrowWebException = true;
            _webExceptionMessage = message;
            return this;
        }

        /// <summary>
        /// Add artificial delay to simulate network latency
        /// </summary>
        /// <param name="delay">Delay duration</param>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler WithDelay(TimeSpan delay)
        {
            _delay = delay;
            return this;
        }

        /// <summary>
        /// Handle HTTP request processing (IContentstackPlugin implementation)
        /// </summary>
        /// <param name="stack">Contentstack client</param>
        /// <param name="request">HTTP request</param>
        /// <returns>Modified request</returns>
        public async Task<HttpWebRequest> OnRequest(ContentstackClient stack, HttpWebRequest request)
        {
            // Track the request for testing purposes
            Requests.Add(request);

            // Simulate delay if configured
            if (_delay > TimeSpan.Zero)
            {
                await Task.Delay(_delay);
            }

            // Simulate timeout if configured
            if (_shouldThrowTimeout)
            {
                throw new TaskCanceledException("Request timeout");
            }

            // Just pass through the request (no modifications needed for our mock)
            return await Task.FromResult(request);
        }

        /// <summary>
        /// Handle HTTP response processing (IContentstackPlugin implementation)
        /// </summary>
        /// <param name="stack">Contentstack client</param>
        /// <param name="request">HTTP request</param>
        /// <param name="response">HTTP response</param>
        /// <param name="responseString">Original response string</param>
        /// <returns>Mock response string</returns>
        public async Task<string> OnResponse(ContentstackClient stack, HttpWebRequest request, HttpWebResponse response, string responseString)
        {
            // Simulate web exception if configured
            if (_shouldThrowWebException)
            {
                throw new WebException(_webExceptionMessage);
            }

            // Return mock response instead of actual response
            if (_mockResponse != null)
            {
                return await Task.FromResult(_mockResponse.ToString());
            }

            // Default fallback response
            return await Task.FromResult(CreateDefaultResponse());
        }

        /// <summary>
        /// Create multiple timeline responses for comparison testing
        /// </summary>
        /// <param name="timestamps">Array of timestamps</param>
        /// <param name="entryUid">Entry UID</param>
        /// <param name="contentTypeUid">Content Type UID</param>
        /// <returns>Handler configured for multiple scenarios</returns>
        public TimelineMockHttpHandler ForMultipleTimelines(string[] timestamps, string entryUid = "test_entry", string contentTypeUid = "test_ct")
        {
            // For simplicity, this returns the first timestamp scenario
            // In a more sophisticated implementation, this could track request parameters
            // and return different responses based on the request context
            if (timestamps != null && timestamps.Length > 0)
            {
                return ForTimelineScenario(timestamps[0], null, entryUid, contentTypeUid);
            }

            return ForSuccessfulLivePreview(entryUid, contentTypeUid);
        }

        /// <summary>
        /// Configure for cache testing scenarios
        /// </summary>
        /// <param name="fingerprint">Unique fingerprint for this response</param>
        /// <returns>Handler instance for chaining</returns>
        public TimelineMockHttpHandler ForCacheScenario(string fingerprint)
        {
            _mockResponse = JObject.Parse($@"{{
                ""entry"": {{
                    ""uid"": ""cache_test_entry"",
                    ""content_type_uid"": ""cache_test_ct"",
                    ""title"": ""Cache Test Entry"",
                    ""cache_fingerprint"": ""{fingerprint}"",
                    ""created_at"": ""2024-11-29T10:00:00.000Z"",
                    ""updated_at"": ""2024-11-29T14:30:00.000Z"",
                    ""_metadata"": {{
                        ""cache_test"": true,
                        ""fingerprint"": ""{fingerprint}""
                    }}
                }}
            }}");

            return this;
        }

        private string CreateDefaultResponse()
        {
            return JObject.Parse(@"{
                ""entry"": {
                    ""uid"": ""default_entry"",
                    ""content_type_uid"": ""default_ct"",
                    ""title"": ""Default Mock Entry"",
                    ""created_at"": ""2024-11-29T10:00:00.000Z"",
                    ""updated_at"": ""2024-11-29T14:30:00.000Z"",
                    ""_metadata"": {
                        ""default_response"": true
                    }
                }
            }").ToString();
        }
    }
}