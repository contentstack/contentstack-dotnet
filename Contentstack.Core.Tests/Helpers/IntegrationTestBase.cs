using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Base class for integration tests with built-in enhanced logging support
    /// Provides TestOutputHelper and common helper methods for logging
    /// </summary>
    public abstract class IntegrationTestBase
    {
        protected readonly ITestOutputHelper Output;
        protected readonly TestOutputHelper TestOutput;

        protected IntegrationTestBase(ITestOutputHelper output)
        {
            Output = output;
            TestOutput = new TestOutputHelper(output, GetType().Name);
        }

        /// <summary>
        /// Log test arrangement step with context
        /// </summary>
        protected void LogArrange(string description, Dictionary<string, object> context = null)
        {
            TestOutput.LogStep("Arrange", description);
            
            if (context != null)
            {
                foreach (var kvp in context)
                {
                    TestOutput.LogContext(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Log test action step
        /// </summary>
        protected void LogAct(string description)
        {
            TestOutput.LogStep("Act", description);
        }

        /// <summary>
        /// Log test assertion step
        /// </summary>
        protected void LogAssert(string description)
        {
            TestOutput.LogStep("Assert", description);
        }

        /// <summary>
        /// Log HTTP GET request with standard Contentstack headers
        /// </summary>
        protected void LogGetRequest(string url, string variantUid = null, Dictionary<string, string> additionalHeaders = null)
        {
            var headers = new Dictionary<string, string>
            {
                { "api_key", TestDataHelper.ApiKey },
                { "access_token", TestDataHelper.DeliveryToken },
                { "Content-Type", "application/json" }
            };

            if (!string.IsNullOrEmpty(variantUid))
            {
                headers["x-cs-variant-uid"] = variantUid;
            }

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    headers[header.Key] = header.Value;
                }
            }

            TestOutput.LogRequest("GET", url, headers);
        }

        /// <summary>
        /// Log successful HTTP response
        /// </summary>
        protected void LogSuccessResponse(int statusCode = 200, string statusText = "OK", Dictionary<string, string> headers = null)
        {
            var responseHeaders = headers ?? new Dictionary<string, string>
            {
                { "content-type", "application/json" }
            };

            TestOutput.LogResponse(statusCode, statusText, responseHeaders);
        }

        /// <summary>
        /// Log assertion with expected and actual values
        /// </summary>
        protected void LogAssertion(string name, object expected, object actual)
        {
            var passed = AreEqual(expected, actual);
            TestOutput.LogAssertion(name, expected, actual, passed);
        }

        /// <summary>
        /// Log context information
        /// </summary>
        protected void LogContext(string key, object value)
        {
            TestOutput.LogContext(key, value);
        }

        /// <summary>
        /// Helper to check equality for logging
        /// </summary>
        private bool AreEqual(object expected, object actual)
        {
            if (expected == null && actual == null) return true;
            if (expected == null || actual == null) return false;
            return expected.Equals(actual) || expected.ToString() == actual.ToString();
        }

        /// <summary>
        /// Create Contentstack client with standard configuration
        /// </summary>
        protected ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid
            };
            
            return new ContentstackClient(options);
        }

        /// <summary>
        /// Build API URL for entry fetch
        /// </summary>
        protected string BuildEntryUrl(string contentType, string entryUid, Dictionary<string, string> queryParams = null)
        {
            var url = $"https://{TestDataHelper.Host}/v3/content_types/{contentType}/entries/{entryUid}";
            
            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", 
                    System.Linq.Enumerable.Select(queryParams, kvp => $"{kvp.Key}={kvp.Value}"));
                url += "?" + queryString;
            }
            
            return url;
        }

        /// <summary>
        /// Build API URL for query
        /// </summary>
        protected string BuildQueryUrl(string contentType, Dictionary<string, string> queryParams = null)
        {
            var url = $"https://{TestDataHelper.Host}/v3/content_types/{contentType}/entries";
            
            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", 
                    System.Linq.Enumerable.Select(queryParams, kvp => $"{kvp.Key}={kvp.Value}"));
                url += "?" + queryString;
            }
            
            return url;
        }
    }
}
