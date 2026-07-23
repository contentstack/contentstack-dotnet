using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Represents a single published taxonomy term from the Contentstack Content Delivery API.
    /// Provides fluent query modifiers and hierarchy traversal methods
    /// (<see cref="Ancestors{T}"/>, <see cref="Descendants{T}"/>, <see cref="Locales{T}"/>).
    /// </summary>
    /// <remarks>
    /// Obtain an instance via <see cref="TaxonomyCDA.Term(string)"/>.
    /// All modifier methods return <c>this</c> for method chaining.
    /// Call a terminal method (<see cref="Fetch{T}"/>, <see cref="Ancestors{T}"/>,
    /// <see cref="Descendants{T}"/>, <see cref="Locales{T}"/>) to execute the HTTP request.
    /// </remarks>
    /// <example>
    /// <code>
    /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
    ///
    /// // Fetch a single term
    /// var term = await stack.Taxonomy("regions")
    ///     .Term("california")
    ///     .SetLocale("en-us")
    ///     .Fetch&lt;JsonObject&gt;();
    ///
    /// // Traverse all ancestor terms (up to depth 5)
    /// var ancestors = await stack.Taxonomy("regions")
    ///     .Term("san-francisco")
    ///     .Depth(5)
    ///     .Ancestors&lt;JsonObject&gt;();
    ///
    /// // Traverse descendant terms (shallow — depth 1)
    /// var children = await stack.Taxonomy("electronics")
    ///     .Term("laptops")
    ///     .Depth(1)
    ///     .Descendants&lt;JsonObject&gt;();
    ///
    /// // Fetch all published locales for a term
    /// var locales = await stack.Taxonomy("regions")
    ///     .Term("california")
    ///     .Locales&lt;JsonObject&gt;();
    /// </code>
    /// </example>
    public class TaxonomyTerm
    {
        #region Private Variables

        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private readonly string _termUid;
        private Dictionary<string, object> _urlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> _headers = new Dictionary<string, object>();
        private Dictionary<string, object> _stackHeaders = new Dictionary<string, object>();

        #endregion

        #region Internal Constructor

        internal TaxonomyTerm(ContentstackClient stack, string taxonomyUid, string termUid)
        {
            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
            _taxonomyUid = taxonomyUid ?? throw new ArgumentNullException(nameof(taxonomyUid));
            _termUid = termUid ?? throw new ArgumentNullException(nameof(termUid));
            _stackHeaders = stack._LocalHeaders;
        }

        #endregion

        #region Public Methods — Query Modifiers

        /// <summary>
        /// Sets the locale for this term fetch (e.g., <c>"en-us"</c>, <c>"fr-fr"</c>).
        /// Mirrors <c>SetLocale()</c> on <see cref="Entry"/> — preferred over <see cref="Param"/>.
        /// </summary>
        /// <param name="locale">The locale code string, e.g. <c>"en-us"</c> or <c>"de-de"</c>.</param>
        /// <returns>The current <see cref="TaxonomyTerm"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var term = await stack.Taxonomy("regions")
        ///     .Term("california")
        ///     .SetLocale("en-us")
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTerm SetLocale(string locale)
        {
            _urlQueries["locale"] = locale;
            return this;
        }

        /// <summary>
        /// Limits the depth of term hierarchy traversal for <see cref="Ancestors{T}"/>,
        /// <see cref="Descendants{T}"/>, and term listing operations.
        /// </summary>
        /// <param name="depth">
        /// The maximum number of hierarchy levels to traverse.
        /// For example, <c>Depth(1)</c> returns only direct parents or children.
        /// </param>
        /// <returns>The current <see cref="TaxonomyTerm"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// // Fetch only the immediate parent (depth = 1)
        /// var parent = await stack.Taxonomy("regions")
        ///     .Term("san-francisco")
        ///     .Depth(1)
        ///     .Ancestors&lt;JsonObject&gt;();
        ///
        /// // Fetch all descendants up to 3 levels deep
        /// var subtree = await stack.Taxonomy("electronics")
        ///     .Term("computers")
        ///     .Depth(3)
        ///     .Descendants&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTerm Depth(int depth)
        {
            _urlQueries["depth"] = depth;
            return this;
        }

        /// <summary>
        /// Enables locale fallback through the branch hierarchy.
        /// If the term is not published in the requested locale, the API falls back
        /// to the nearest parent locale in the branch locale hierarchy.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyTerm"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var term = await stack.Taxonomy("regions")
        ///     .Term("california")
        ///     .SetLocale("fr-fr")
        ///     .IncludeFallback()
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTerm IncludeFallback()
        {
            _urlQueries["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Includes the <c>_branch</c> field in the API response.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyTerm"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var term = await stack.Taxonomy("regions")
        ///     .Term("california")
        ///     .IncludeBranch()
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTerm IncludeBranch()
        {
            _urlQueries["include_branch"] = "true";
            return this;
        }

        /// <summary>
        /// Adds an arbitrary query parameter to the request URL.
        /// </summary>
        /// <param name="key">The query parameter key.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The current <see cref="TaxonomyTerm"/> instance for method chaining.</returns>
        public TaxonomyTerm Param(string key, object value)
        {
            _urlQueries[key] = value;
            return this;
        }

        #endregion

        #region Public Methods — Data Retrieval

        /// <summary>
        /// Fetches this term from the Contentstack CDA.
        /// The top-level <c>"term"</c> wrapper in the response is unwrapped automatically.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the term into. Use <see cref="JsonObject"/> for a
        /// schema-less response, or a custom POCO mapping <c>uid</c>, <c>name</c>,
        /// <c>parent_uid</c>, <c>taxonomy_uid</c>, <c>order</c>, <c>locale</c>,
        /// and <c>publish_details</c>.
        /// </typeparam>
        /// <returns>The deserialized term object.</returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when the term is not found (HTTP 404), the feature flag is disabled (HTTP 403),
        /// or a network error occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// var term = await stack.Taxonomy("regions")
        ///     .Term("california")
        ///     .SetLocale("en-us")
        ///     .Fetch&lt;JsonObject&gt;();
        ///
        /// Console.WriteLine(term["name"]);          // "California"
        /// Console.WriteLine(term["parent_uid"]);    // "usa"
        /// </code>
        /// </example>
        public async Task<T> Fetch<T>()
        {
            string url = $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms/{_termUid}";
            return await ExecuteRequest<T>(url, "term");
        }

        /// <summary>
        /// Fetches all published localized versions of this term.
        /// Returns each locale in which this term has been published to the requested environment.
        /// The top-level <c>"terms"</c> wrapper is unwrapped automatically.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response into. Use <see cref="JsonObject"/> to receive
        /// the raw object containing a <c>terms</c> array, or a collection type.
        /// </typeparam>
        /// <returns>
        /// The deserialized response. Each item in the response contains the term
        /// fields plus a <c>locale</c> and <c>publish_details</c> for that locale.
        /// </returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when the term is not found (HTTP 404), the feature flag is disabled (HTTP 403),
        /// or a network error occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// // Find every locale this term has been published in
        /// var locales = await stack.Taxonomy("regions")
        ///     .Term("california")
        ///     .Locales&lt;JsonObject&gt;();
        ///
        /// // locales contains: { "terms": [ { "locale": "en-us", ... }, { "locale": "fr-fr", ... } ] }
        /// </code>
        /// </example>
        public async Task<T> Locales<T>()
        {
            string url = $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms/{_termUid}/locales";
            // API returns { "terms": [...] } — return the full response object, not an unwrapped key.
            return await ExecuteRequest<T>(url, null);
        }

        /// <summary>
        /// Fetches all ancestor terms of this term, traversing up to the root of the taxonomy tree.
        /// Use <see cref="Depth(int)"/> before calling this method to limit traversal depth.
        /// The top-level <c>"term"</c> wrapper is unwrapped automatically; the ancestors are
        /// available in the <c>ancestors</c> property of the returned object.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the term into. The response includes an <c>ancestors</c>
        /// array alongside the term's own fields.
        /// </typeparam>
        /// <returns>
        /// The deserialized term object with an <c>ancestors</c> array containing each
        /// ancestor from direct parent up to the root.
        /// </returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when the term is not found (HTTP 404), the feature flag is disabled (HTTP 403),
        /// or a network error occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// // Fetch all ancestors of "san-francisco" up to the root
        /// var result = await stack.Taxonomy("regions")
        ///     .Term("san-francisco")
        ///     .Ancestors&lt;JsonObject&gt;();
        ///
        /// // result["ancestors"] → [ { "uid": "california", ... }, { "uid": "usa", ... } ]
        ///
        /// // Limit to the immediate parent only
        /// var parentOnly = await stack.Taxonomy("regions")
        ///     .Term("san-francisco")
        ///     .Depth(1)
        ///     .Ancestors&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public async Task<T> Ancestors<T>()
        {
            string url = $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms/{_termUid}/ancestors";
            // API returns { "terms": [...] } — return the full response object, not an unwrapped key.
            return await ExecuteRequest<T>(url, null);
        }

        /// <summary>
        /// Fetches all descendant terms of this term in the taxonomy tree.
        /// Use <see cref="Depth(int)"/> before calling this method to limit traversal depth
        /// and avoid fetching deeply nested subtrees.
        /// The top-level <c>"term"</c> wrapper is unwrapped automatically; the descendants are
        /// available in the <c>descendants</c> property of the returned object.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the term into. The response includes a <c>descendants</c>
        /// array alongside the term's own fields.
        /// </typeparam>
        /// <returns>
        /// The deserialized term object with a <c>descendants</c> array containing all
        /// descendant terms down to the specified depth (or all levels if depth is not set).
        /// </returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when the term is not found (HTTP 404), the feature flag is disabled (HTTP 403),
        /// or a network error occurs.
        /// </exception>
        /// <example>
        /// <code>
        /// // Fetch all descendants of "electronics"
        /// var result = await stack.Taxonomy("electronics")
        ///     .Term("electronics")
        ///     .Descendants&lt;JsonObject&gt;();
        ///
        /// // result["descendants"] → [ { "uid": "laptops", ... }, { "uid": "phones", ... }, ... ]
        ///
        /// // Fetch only direct children (depth = 1)
        /// var children = await stack.Taxonomy("electronics")
        ///     .Term("electronics")
        ///     .Depth(1)
        ///     .Descendants&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public async Task<T> Descendants<T>()
        {
            string url = $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms/{_termUid}/descendants";
            // API returns { "terms": [...] } — return the full response object, not an unwrapped key.
            return await ExecuteRequest<T>(url, null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Shared HTTP execution logic for all terminal methods.
        /// Builds the request, calls ProcessRequest, and optionally unwraps the response envelope.
        /// Pass <c>responseKey</c> to unwrap a single-resource response (e.g. <c>"term"</c>),
        /// or <c>null</c> to return the full response as-is (used for list responses).
        /// </summary>
        private async Task<T> ExecuteRequest<T>(string url, string responseKey)
        {
            Dictionary<string, object> headers = GetHeader(_headers);
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            mainJson.Add("environment", _stack.Config.Environment);
            foreach (var kvp in _urlQueries)
                mainJson.Add(kvp.Key, kvp.Value);

            try
            {
                HttpRequestHandler handler = new HttpRequestHandler(_stack);
                string result = await handler.ProcessRequest(
                    url, headers, mainJson,
                    Branch: string.IsNullOrEmpty(_stack.Config.Branch) ? null : _stack.Config.Branch,
                    timeout: _stack.Config.Timeout,
                    proxy: _stack.Config.Proxy);

                var rootNode = JsonNode.Parse(result ?? "{}");

                // When no key is requested, return the full response (list endpoints).
                if (responseKey == null)
                    return JsonSerializer.Deserialize<T>(rootNode!.ToJsonString(), _stack.SerializerOptions);

                // Single-resource endpoints: unwrap the envelope key if present.
                if (rootNode is JsonObject obj)
                {
                    var inner = obj[responseKey];
                    var json = inner != null ? inner.ToJsonString() : obj.ToJsonString();
                    return JsonSerializer.Deserialize<T>(json, _stack.SerializerOptions);
                }

                // Fallback: deserialize root directly.
                return JsonSerializer.Deserialize<T>(rootNode!.ToJsonString(), _stack.SerializerOptions);
            }
            catch (Exception ex)
            {
                var contentstackError = GetContentstackError(ex);
                throw new TaxonomyException(contentstackError.Message, ex)
                {
                    ErrorCode = contentstackError.ErrorCode,
                    StatusCode = contentstackError.StatusCode,
                    Errors = contentstackError.Errors
                };
            }
        }

        private Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
        {
            Dictionary<string, object> classHeaders = new Dictionary<string, object>();
            if (localHeader != null && localHeader.Count > 0)
            {
                foreach (var entry in localHeader)
                    classHeaders[entry.Key] = entry.Value;
            }
            if (_stackHeaders != null)
            {
                foreach (var entry in _stackHeaders)
                {
                    if (!classHeaders.ContainsKey(entry.Key))
                        classHeaders[entry.Key] = entry.Value;
                }
            }
            return classHeaders.Count > 0 ? classHeaders : _stackHeaders;
        }

        private static ContentstackException GetContentstackError(Exception ex)
        {
            int errorCode = 0;
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            Dictionary<string, object> errors = null;

            try
            {
                WebException webEx = ex as WebException;
                if (webEx?.Response != null)
                {
                    using (var exResp = webEx.Response)
                    {
                        var stream = exResp.GetResponseStream();
                        if (stream != null)
                        {
                            using (stream)
                            using (var reader = new StreamReader(stream))
                            {
                                errorMessage = reader.ReadToEnd();
                                if (!string.IsNullOrWhiteSpace(errorMessage))
                                    ApiErrorBodyParser.TryApply(errorMessage.Replace("\r\n", ""),
                                        ref errorCode, ref errorMessage, ref errors);
                                if (exResp is HttpWebResponse httpResp)
                                    statusCode = httpResp.StatusCode;
                            }
                        }
                        else
                        {
                            errorMessage = webEx.Message;
                        }
                    }
                }
                else
                {
                    errorMessage = ex.Message;
                }
            }
            catch
            {
                errorMessage = ex.Message;
            }

            return new ContentstackException(errorMessage)
            {
                ErrorCode = errorCode,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        #endregion
    }
}
