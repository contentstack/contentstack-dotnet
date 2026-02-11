using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// SDK Plugin that captures the ACTUAL HTTP request and response made by the SDK.
    /// Implements IContentstackPlugin to intercept requests via the SDK's plugin pipeline.
    /// This gives us the real URL (with all query params like environment, locale, 
    /// include_fallback, query filters, etc.) and real headers (api_key, access_token, 
    /// branch, x-cs-variant-uid, etc.) — not a manual approximation.
    /// Also detects and logs the SDK method chain from URL patterns.
    /// </summary>
    public class RequestLoggingPlugin : IContentstackPlugin
    {
        private readonly TestOutputHelper _output;

        public RequestLoggingPlugin(TestOutputHelper output)
        {
            _output = output;
        }

        public async Task<HttpWebRequest> OnRequest(ContentstackClient stack, HttpWebRequest request)
        {
            if (_output == null)
                return request;

            // Capture ALL headers from the actual request
            var headers = new Dictionary<string, string>();

            // Standard headers set directly on HttpWebRequest
            if (!string.IsNullOrEmpty(request.ContentType))
                headers["Content-Type"] = request.ContentType;

            // All custom headers (api_key, access_token, branch, x-cs-variant-uid, x-user-agent, etc.)
            foreach (string key in request.Headers.AllKeys)
            {
                headers[key] = request.Headers[key];
            }

            // Detect SDK method from URL pattern
            var sdkMethod = DetectSdkMethod(request.Method, request.RequestUri.ToString(), headers);

            // Log the REAL request with REAL URL (includes all SDK-added query params)
            _output.LogRequest(
                request.Method,
                request.RequestUri.ToString(),
                headers,
                null,
                sdkMethod
            );

            return await Task.FromResult(request);
        }

        public async Task<string> OnResponse(ContentstackClient stack, HttpWebRequest request, HttpWebResponse response, string responseString)
        {
            if (_output == null)
                return responseString;

            // Capture response headers
            var headers = new Dictionary<string, string>();
            foreach (string key in response.Headers.AllKeys)
            {
                headers[key] = response.Headers[key];
            }

            // Truncate response body for logging (keep first 2000 chars)
            var truncatedBody = responseString;
            if (!string.IsNullOrEmpty(responseString) && responseString.Length > 2000)
            {
                truncatedBody = responseString.Substring(0, 2000) + "... [truncated]";
            }

            _output.LogResponse(
                (int)response.StatusCode,
                response.StatusDescription,
                headers,
                truncatedBody
            );

            return await Task.FromResult(responseString);
        }

        // ====================================================================
        // SDK METHOD DETECTION
        // Maps HTTP method + URL pattern to .NET CDA SDK method chains
        // ====================================================================

        private static readonly (Regex pattern, string method, string sdk)[] SdkMethodPatterns = new[]
        {
            // Sync API
            (new Regex(@"/v3/stacks/sync\b"), "GET", "client.SyncRecursive() / SyncToken() / SyncPaginationToken()"),

            // Content Types
            (new Regex(@"/v3/content_types/[^/]+/entries/[^/?]+"), "GET", "client.ContentType(uid).Entry(uid).Fetch<T>()"),
            (new Regex(@"/v3/content_types/[^/]+/entries\b"), "GET", "client.ContentType(uid).Query().Find<T>()"),
            (new Regex(@"/v3/content_types/[^/?]+$"), "GET", "client.ContentType(uid).Fetch()"),
            (new Regex(@"/v3/content_types\b"), "GET", "client.GetContentTypes()"),

            // Assets
            (new Regex(@"/v3/assets/[^/?]+$"), "GET", "client.Asset(uid).Fetch()"),
            (new Regex(@"/v3/assets\b"), "GET", "client.AssetLibrary().FetchAll()"),

            // Global Fields
            (new Regex(@"/v3/global_fields/[^/?]+$"), "GET", "client.GlobalField(uid).Fetch()"),
            (new Regex(@"/v3/global_fields\b"), "GET", "client.GlobalFieldQuery().Find()"),

            // Taxonomies
            (new Regex(@"/v3/taxonomies/[^/]+/terms\b"), "GET", "taxonomy.Terms().Query()"),
            (new Regex(@"/v3/taxonomies/entries\b"), "GET", "client.Taxonomies().Entries()"),
            (new Regex(@"/v3/taxonomies/[^/?]+$"), "GET", "client.Taxonomy(uid).Fetch()"),
            (new Regex(@"/v3/taxonomies\b"), "GET", "client.Taxonomies().Query()"),

            // Image Delivery (image transform URLs)
            (new Regex(@"/v3/assets/.*\?.*(?:width|height|format|quality|crop|trim|orient)"), "GET", "client.Asset(uid).Fetch() [with ImageTransform]"),

            // Live Preview
            (new Regex(@"/v3/content_types.*live_preview"), "GET", "client.LivePreviewQueryAsync()"),
        };

        /// <summary>
        /// Detects the SDK method chain from the HTTP request URL pattern.
        /// Also checks headers for additional context (variants, branch).
        /// </summary>
        private static string DetectSdkMethod(string httpMethod, string url, Dictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var method = httpMethod?.ToUpper() ?? "GET";

            // Extract path from URL (remove query string for pattern matching)
            string path;
            try
            {
                var uri = new Uri(url);
                path = uri.AbsolutePath;
            }
            catch
            {
                path = url;
            }

            // Find matching pattern
            string sdkMethod = null;
            foreach (var mapping in SdkMethodPatterns)
            {
                if (mapping.method == method && mapping.pattern.IsMatch(path))
                {
                    sdkMethod = mapping.sdk;
                    break;
                }
            }

            if (sdkMethod == null)
                return $"Unknown ({method} {path})";

            // Enrich with header context
            var extras = new List<string>();

            if (headers.ContainsKey("x-cs-variant-uid"))
                extras.Add(".Variant()");

            if (headers.ContainsKey("branch"))
                extras.Add($"[branch: {headers["branch"]}]");

            if (extras.Count > 0)
                sdkMethod += " " + string.Join(" ", extras);

            return sdkMethod;
        }
    }
}
