using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Represents a single published taxonomy fetched from the Contentstack Content Delivery API.
    /// Provides fluent methods to set query modifiers and factory methods to access its terms.
    /// </summary>
    /// <remarks>
    /// Obtain an instance via <see cref="ContentstackClient.Taxonomy(string)"/>.
    /// The existing <see cref="Taxonomy"/> class (accessed via <see cref="ContentstackClient.Taxonomies()"/>)
    /// is for entry-level query operators and remains unchanged.
    /// </remarks>
    /// <example>
    /// <code>
    /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
    ///
    /// // Fetch a single taxonomy
    /// var taxonomy = await stack.Taxonomy("regions").Fetch&lt;JsonObject&gt;();
    ///
    /// // Fetch with locale and fallback
    /// var localized = await stack.Taxonomy("regions")
    ///     .SetLocale("fr-fr")
    ///     .IncludeFallback()
    ///     .Fetch&lt;JsonObject&gt;();
    ///
    /// // List all terms in the taxonomy
    /// var terms = await stack.Taxonomy("regions")
    ///     .Term()
    ///     .SetLocale("en-us")
    ///     .Depth(3)
    ///     .Find&lt;JsonObject&gt;();
    ///
    /// // Fetch a single term and traverse its ancestors
    /// var ancestors = await stack.Taxonomy("regions")
    ///     .Term("california")
    ///     .Depth(5)
    ///     .Ancestors&lt;JsonObject&gt;();
    /// </code>
    /// </example>
    public class TaxonomyCDA
    {
        #region Private Variables

        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private Dictionary<string, object> _urlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> _headers = new Dictionary<string, object>();
        private Dictionary<string, object> _stackHeaders = new Dictionary<string, object>();

        #endregion

        #region Internal Constructor

        internal TaxonomyCDA(ContentstackClient stack, string taxonomyUid)
        {
            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
            _taxonomyUid = taxonomyUid ?? throw new ArgumentNullException(nameof(taxonomyUid));
            _stackHeaders = stack._LocalHeaders;
        }

        #endregion

        #region Public Methods — Term Factories

        /// <summary>
        /// Returns a <see cref="TaxonomyTermQuery"/> for listing all published terms in this taxonomy.
        /// </summary>
        /// <returns>
        /// A <see cref="TaxonomyTermQuery"/> pre-configured for
        /// <c>GET /taxonomies/{taxonomy_uid}/terms</c>.
        /// Call <see cref="TaxonomyTermQuery.Find{T}"/> to execute.
        /// </returns>
        /// <example>
        /// <code>
        /// var terms = await stack.Taxonomy("electronics")
        ///     .Term()
        ///     .SetLocale("en-us")
        ///     .Depth(2)
        ///     .Limit(50)
        ///     .Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery Term()
        {
            return new TaxonomyTermQuery(_stack, _taxonomyUid);
        }

        /// <summary>
        /// Returns a <see cref="TaxonomyTerm"/> for the specified term UID.
        /// </summary>
        /// <param name="termUid">The unique identifier of the term.</param>
        /// <returns>
        /// A <see cref="TaxonomyTerm"/> pre-configured for
        /// <c>GET /taxonomies/{taxonomy_uid}/terms/{term_uid}</c>.
        /// Call <see cref="TaxonomyTerm.Fetch{T}"/>, <see cref="TaxonomyTerm.Ancestors{T}"/>,
        /// <see cref="TaxonomyTerm.Descendants{T}"/>, or <see cref="TaxonomyTerm.Locales{T}"/> to execute.
        /// </returns>
        /// <example>
        /// <code>
        /// // Fetch a specific term
        /// var term = await stack.Taxonomy("regions").Term("california").Fetch&lt;JsonObject&gt;();
        ///
        /// // Traverse ancestors up to depth 5
        /// var ancestors = await stack.Taxonomy("regions")
        ///     .Term("san-francisco")
        ///     .Depth(5)
        ///     .Ancestors&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTerm Term(string termUid)
        {
            return new TaxonomyTerm(_stack, _taxonomyUid, termUid);
        }

        #endregion

        #region Public Methods — Query Modifiers

        /// <summary>
        /// Sets the locale for this taxonomy fetch (e.g., <c>"en-us"</c>, <c>"fr-fr"</c>).
        /// Mirrors <c>SetLocale()</c> on <see cref="Entry"/> — preferred over <see cref="Param"/>.
        /// </summary>
        /// <param name="locale">
        /// The locale code string. Must match a locale published on the stack,
        /// e.g. <c>"en-us"</c>, <c>"fr-fr"</c>, <c>"de-de"</c>.
        /// </param>
        /// <returns>The current <see cref="TaxonomyCDA"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var taxonomy = await stack.Taxonomy("regions")
        ///     .SetLocale("fr-fr")
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyCDA SetLocale(string locale)
        {
            _urlQueries["locale"] = locale;
            return this;
        }

        /// <summary>
        /// Enables locale fallback through the branch hierarchy.
        /// If the taxonomy is not published in the requested locale, the API falls back
        /// to the nearest parent locale defined in the branch locale hierarchy.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyCDA"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// // Falls back to "en-us" if "fr-fr" is not published
        /// var taxonomy = await stack.Taxonomy("regions")
        ///     .SetLocale("fr-fr")
        ///     .IncludeFallback()
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyCDA IncludeFallback()
        {
            _urlQueries["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Includes the <c>_branch</c> field in the API response.
        /// Useful when working with multiple branches and you need to identify
        /// which branch the taxonomy was fetched from.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyCDA"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var taxonomy = await stack.Taxonomy("regions")
        ///     .IncludeBranch()
        ///     .Fetch&lt;JsonObject&gt;();
        /// // taxonomy["_branch"] will be present in the response
        /// </code>
        /// </example>
        public TaxonomyCDA IncludeBranch()
        {
            _urlQueries["include_branch"] = "true";
            return this;
        }

        /// <summary>
        /// Adds an arbitrary query parameter to the request URL.
        /// Use this for parameters not yet covered by a dedicated method.
        /// </summary>
        /// <param name="key">The query parameter key.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The current <see cref="TaxonomyCDA"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var taxonomy = await stack.Taxonomy("regions")
        ///     .Param("custom_param", "custom_value")
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyCDA Param(string key, object value)
        {
            _urlQueries[key] = value;
            return this;
        }

        #endregion

        #region Public Methods — Data Retrieval

        /// <summary>
        /// Fetches this taxonomy from the Contentstack CDA.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the taxonomy into.
        /// Use <see cref="JsonObject"/> for a schema-less response, or a custom POCO
        /// whose property names match the taxonomy fields (e.g., <c>uid</c>, <c>name</c>,
        /// <c>description</c>, <c>publish_details</c>).
        /// </typeparam>
        /// <returns>
        /// The deserialized taxonomy object. The <c>publish_details</c>, <c>uid</c>,
        /// <c>name</c>, and <c>description</c> fields are always present when the
        /// taxonomy has been published to the requested environment and locale.
        /// The top-level <c>"taxonomy"</c> wrapper is unwrapped automatically.
        /// </returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The taxonomy UID does not exist or is not published in the requested locale (HTTP 404).</description></item>
        /// <item><description>The <c>taxonomy_publish</c> feature flag is disabled on the stack plan (HTTP 403).</description></item>
        /// <item><description>A network error or server error occurs.</description></item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code>
        /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        ///
        /// // Basic fetch
        /// var taxonomy = await stack.Taxonomy("regions").Fetch&lt;JsonObject&gt;();
        /// Console.WriteLine(taxonomy["name"]);
        ///
        /// // Fetch with locale fallback
        /// var localized = await stack.Taxonomy("regions")
        ///     .SetLocale("fr-fr")
        ///     .IncludeFallback()
        ///     .Fetch&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public async Task<T> Fetch<T>()
        {
            Dictionary<string, object> headers = GetHeader(_headers);
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            mainJson.Add("environment", _stack.Config.Environment);
            foreach (var kvp in _urlQueries)
                mainJson.Add(kvp.Key, kvp.Value);

            try
            {
                string url = $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}";
                HttpRequestHandler handler = new HttpRequestHandler(_stack);
                string result = await handler.ProcessRequest(
                    url, headers, mainJson,
                    Branch: string.IsNullOrEmpty(_stack.Config.Branch) ? null : _stack.Config.Branch,
                    timeout: _stack.Config.Timeout,
                    proxy: _stack.Config.Proxy);

                JsonObject obj = JsonNode.Parse(result ?? "{}")!.AsObject();
                return JsonSerializer.Deserialize<T>(
                    obj["taxonomy"]!.ToJsonString(),
                    _stack.SerializerOptions);
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

        #endregion

        #region Private Methods

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
