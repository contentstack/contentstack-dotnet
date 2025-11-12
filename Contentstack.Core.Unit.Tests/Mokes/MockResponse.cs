using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Contentstack.Core.Unit.Tests.Mokes;

namespace Contentstack.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Mock response helper - matches Management SDK pattern
    /// Adapted for Delivery SDK which uses HttpWebRequest instead of HttpClient
    /// </summary>
    public static class MockResponse
    {
        /// <summary>
        /// Creates a Contentstack response string from an embedded resource file
        /// </summary>
        /// <param name="resourceName">Name of the resource file (e.g., "MockResponse.txt")</param>
        /// <returns>JSON string response</returns>
        public static string CreateContentstackResponse(string resourceName)
        {
            try
            {
                // Try to get from embedded resource
                var rawResponse = Utilities.GetResourceText(resourceName);
                
                // If it's an HTTP response format (like Management SDK), parse it
                if (rawResponse.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase))
                {
                    var response = ParseRawResponse(rawResponse);
                    return response.Body;
                }
                
                // Otherwise, assume it's already JSON
                return rawResponse;
            }
            catch (Exception ex)
            {
                // Fallback: try to read from file system
                var assembly = Assembly.GetExecutingAssembly();
                var baseDirectory = Path.GetDirectoryName(assembly.Location) ?? "";
                var filePath = Path.Combine(baseDirectory, "..", "..", "..", "Mokes", "Response", resourceName);
                
                if (File.Exists(filePath))
                {
                    var rawResponse = File.ReadAllText(filePath);
                    if (rawResponse.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase))
                    {
                        var response = ParseRawResponse(rawResponse);
                        return response.Body;
                    }
                    return rawResponse;
                }
                
                throw new FileNotFoundException($"Mock response file not found: {resourceName}", ex);
            }
        }

        /// <summary>
        /// Creates a JObject from a mock response file
        /// </summary>
        /// <param name="resourceName">Name of the resource file</param>
        /// <returns>JObject parsed from the response</returns>
        public static JObject CreateContentstackResponseAsJObject(string resourceName)
        {
            var jsonString = CreateContentstackResponse(resourceName);
            return JObject.Parse(jsonString);
        }

        /// <summary>
        /// Parses raw HTTP response format (like Management SDK)
        /// </summary>
        static HttpResponse ParseRawResponse(string rawResponse)
        {
            var response = new HttpResponse();
            var responseLines = rawResponse.Split('\n');

            if (responseLines.Length == 0)
                throw new ArgumentException("The resource does not contain a valid HTTP response.", nameof(rawResponse));

            response.StatusLine = responseLines[0];
            var currentLine = responseLines[0];

            var lineIndex = 0;
            if (responseLines.Length > 1)
            {
                for (lineIndex = 1; lineIndex < responseLines.Length; lineIndex++)
                {
                    currentLine = responseLines[lineIndex];
                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        if (lineIndex > 0)
                            currentLine = responseLines[lineIndex - 1];
                        break;
                    }

                    var index = currentLine.IndexOf(":");
                    if (index != -1)
                    {
                        var headerKey = currentLine.Substring(0, index).Trim();
                        var headerValue = currentLine.Substring(index + 1).Trim();
                        response.Headers.Add(headerKey, headerValue);
                    }
                }
            }

            var startOfBody = rawResponse.IndexOf(currentLine) + currentLine.Length;
            if (startOfBody < rawResponse.Length)
            {
                response.Body = rawResponse.Substring(startOfBody).Trim();
            }
            else
            {
                response.Body = "{}";
            }
            
            return response;
        }

        class HttpResponse
        {
            public HttpResponse()
            {
                this.Headers = new Dictionary<string, string>();
            }
            public string StatusLine { get; set; }
            public IDictionary<string, string> Headers { get; private set; }
            public string Body { get; set; }
        }
    }
}

