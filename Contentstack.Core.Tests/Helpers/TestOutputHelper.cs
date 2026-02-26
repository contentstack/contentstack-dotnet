using System;
using System.Text.Json;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Helper to capture structured test output for enhanced HTML reporting
    /// Captures Expected vs Actual values, Request details, Response details
    /// </summary>
    public class TestOutputHelper
    {
        private readonly ITestOutputHelper _output;
        private readonly string _testName;

        public TestOutputHelper(ITestOutputHelper output, string testName = null)
        {
            _output = output;
            _testName = testName ?? "Unknown Test";
        }

        /// <summary>
        /// Log Expected vs Actual comparison
        /// </summary>
        public void LogAssertion(string assertionName, object expected, object actual, bool passed = true)
        {
            var data = new
            {
                Type = "ASSERTION",
                TestName = _testName,
                AssertionName = assertionName,
                Expected = FormatValue(expected),
                Actual = FormatValue(actual),
                Passed = passed,
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            
            WriteStructuredOutput(data);
        }

        /// <summary>
        /// Log HTTP Request details (including cURL and SDK method)
        /// </summary>
        public void LogRequest(string method, string url, Dictionary<string, string> headers = null, string body = null, string sdkMethod = null)
        {
            var curlCommand = GenerateCurlCommand(method, url, headers, body);
            
            var data = new
            {
                Type = "HTTP_REQUEST",
                TestName = _testName,
                Method = method,
                Url = url,
                Headers = headers ?? new Dictionary<string, string>(),
                Body = body,
                CurlCommand = curlCommand,
                SdkMethod = sdkMethod,
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            
            WriteStructuredOutput(data);
        }

        /// <summary>
        /// Log HTTP Response details
        /// </summary>
        public void LogResponse(int statusCode, string statusText, Dictionary<string, string> headers = null, string body = null)
        {
            var data = new
            {
                Type = "HTTP_RESPONSE",
                TestName = _testName,
                StatusCode = statusCode,
                StatusText = statusText,
                Headers = headers ?? new Dictionary<string, string>(),
                Body = TruncateBody(body, 5000), // Limit body size
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            
            WriteStructuredOutput(data);
        }

        /// <summary>
        /// Log test context information
        /// </summary>
        public void LogContext(string key, object value)
        {
            var data = new
            {
                Type = "CONTEXT",
                TestName = _testName,
                Key = key,
                Value = FormatValue(value),
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            
            WriteStructuredOutput(data);
        }

        /// <summary>
        /// Log test step
        /// </summary>
        public void LogStep(string stepName, string description = null)
        {
            var data = new
            {
                Type = "STEP",
                TestName = _testName,
                StepName = stepName,
                Description = description,
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };
            
            WriteStructuredOutput(data);
        }

        private void WriteStructuredOutput(object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                // Use special markers for easy parsing
                _output.WriteLine($"###TEST_OUTPUT_START###{json}###TEST_OUTPUT_END###");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error logging structured output: {ex.Message}");
            }
        }

        private string FormatValue(object value)
        {
            if (value == null) return "null";
            
            try
            {
                if (value is string str) return str;
                if (value.GetType().IsPrimitive || value is decimal) return value.ToString();
                
                return JsonSerializer.Serialize(value, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch
            {
                return value.ToString();
            }
        }

        private string GenerateCurlCommand(string method, string url, Dictionary<string, string> headers, string body)
        {
            var curl = $"curl -X {method.ToUpper()} '{url}'";
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    curl += $" \\\n  -H '{header.Key}: {header.Value}'";
                }
            }
            
            if (!string.IsNullOrEmpty(body))
            {
                var escapedBody = body.Replace("'", "\\'");
                curl += $" \\\n  -d '{escapedBody}'";
            }
            
            return curl;
        }

        private string TruncateBody(string body, int maxLength)
        {
            if (string.IsNullOrEmpty(body)) return body;
            if (body.Length <= maxLength) return body;
            
            return body.Substring(0, maxLength) + "\n... (truncated)";
        }
    }
}
