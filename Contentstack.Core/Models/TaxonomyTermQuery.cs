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
    /// Query builder for fetching all published terms within a taxonomy from the
    /// Contentstack Content Delivery API.
    /// </summary>
    /// <remarks>
    /// Obtain an instance via <see cref="TaxonomyCDA.Term()"/> (no argument).
    /// Chain modifier methods before calling <see cref="Find{T}"/> to execute.
    /// </remarks>
    /// <example>
    /// <code>
    /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
    ///
    /// // Fetch all terms in a taxonomy
    /// var result = await stack.Taxonomy("electronics")
    ///     .Term()
    ///     .SetLocale("en-us")
    ///     .Depth(3)
    ///     .Limit(50)
    ///     .IncludeCount()
    ///     .Find&lt;JsonObject&gt;();
    ///
    /// Console.WriteLine($"Total terms: {result.Count}");
    /// foreach (var term in result.Items)
    ///     Console.WriteLine(term["uid"] + ": " + term["name"]);
    /// </code>
    /// </example>
    public class TaxonomyTermQuery
    {
        #region Private Variables

        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private Dictionary<string, object> _urlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> _headers = new Dictionary<string, object>();
        private Dictionary<string, object> _stackHeaders = new Dictionary<string, object>();

        #endregion

        #region Internal Constructor

        internal TaxonomyTermQuery(ContentstackClient stack, string taxonomyUid)
        {
            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
            _taxonomyUid = taxonomyUid ?? throw new ArgumentNullException(nameof(taxonomyUid));
            _stackHeaders = stack._LocalHeaders;
        }

        #endregion

        #region Public Methods — Query Modifiers

        /// <summary>
        /// Filters terms by locale (e.g., <c>"en-us"</c>, <c>"fr-fr"</c>).
        /// Mirrors <c>SetLocale()</c> on <see cref="Entry"/> — preferred over <see cref="Param"/>.
        /// </summary>
        /// <param name="locale">The locale code string to filter by.</param>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var terms = await stack.Taxonomy("regions")
        ///     .Term()
        ///     .SetLocale("fr-fr")
        ///     .Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery SetLocale(string locale)
        {
            _urlQueries["locale"] = locale;
            return this;
        }

        /// <summary>
        /// Limits the depth of the term hierarchy returned in the response.
        /// By default all levels are returned; use this to avoid fetching deeply nested trees.
        /// </summary>
        /// <param name="depth">
        /// The maximum number of hierarchy levels to include.
        /// <c>Depth(1)</c> returns only root-level terms; <c>Depth(2)</c> includes their children, etc.
        /// </param>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// // Fetch only root-level terms (depth = 1)
        /// var rootTerms = await stack.Taxonomy("electronics")
        ///     .Term()
        ///     .Depth(1)
        ///     .Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery Depth(int depth)
        {
            _urlQueries["depth"] = depth;
            return this;
        }

        /// <summary>
        /// Sets the number of terms to skip in the result set (pagination offset).
        /// </summary>
        /// <param name="skip">The number of terms to skip. Must be &gt;= 0.</param>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// // Fetch the second page (20 items per page)
        /// var page2 = await stack.Taxonomy("regions").Term().Skip(20).Limit(20).Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery Skip(int skip)
        {
            _urlQueries["skip"] = skip;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of terms to return.
        /// </summary>
        /// <param name="limit">The maximum result count.</param>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var terms = await stack.Taxonomy("electronics").Term().Limit(25).Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery Limit(int limit)
        {
            _urlQueries["limit"] = limit;
            return this;
        }

        /// <summary>
        /// Enables locale fallback through the branch hierarchy.
        /// If a term is not published in the requested locale, the API falls back
        /// to the nearest parent locale in the branch locale hierarchy.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var terms = await stack.Taxonomy("regions")
        ///     .Term()
        ///     .SetLocale("fr-fr")
        ///     .IncludeFallback()
        ///     .Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery IncludeFallback()
        {
            _urlQueries["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Includes the <c>_branch</c> field in each term in the API response.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var terms = await stack.Taxonomy("regions")
        ///     .Term()
        ///     .IncludeBranch()
        ///     .Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyTermQuery IncludeBranch()
        {
            _urlQueries["include_branch"] = "true";
            return this;
        }

        /// <summary>
        /// Includes the total count of matching terms in the response.
        /// Access it via <see cref="ContentstackCollection{T}.Count"/> on the result.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var result = await stack.Taxonomy("electronics").Term().IncludeCount().Find&lt;JsonObject&gt;();
        /// Console.WriteLine($"Total terms: {result.Count}");
        /// </code>
        /// </example>
        public TaxonomyTermQuery IncludeCount()
        {
            _urlQueries["include_count"] = "true";
            return this;
        }

        /// <summary>
        /// Adds an arbitrary query parameter to the request URL.
        /// </summary>
        /// <param name="key">The query parameter key.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The current <see cref="TaxonomyTermQuery"/> instance for method chaining.</returns>
        public TaxonomyTermQuery Param(string key, object value)
        {
            _urlQueries[key] = value;
            return this;
        }

        #endregion

        #region Public Methods — Data Retrieval

        /// <summary>
        /// Executes the query and fetches all published terms in the taxonomy from the CDA.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize each term into.
        /// Use <see cref="JsonObject"/> for a schema-less response, or a custom POCO
        /// mapping fields such as <c>uid</c>, <c>name</c>, <c>parent_uid</c>,
        /// <c>taxonomy_uid</c>, <c>order</c>, <c>locale</c>, and <c>publish_details</c>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="ContentstackCollection{T}"/> containing:
        /// <list type="bullet">
        /// <item><description><see cref="ContentstackCollection{T}.Items"/> — the deserialized term objects.</description></item>
        /// <item><description><see cref="ContentstackCollection{T}.Count"/> — total count when <see cref="IncludeCount"/> was called.</description></item>
        /// <item><description><see cref="ContentstackCollection{T}.Skip"/> / <see cref="ContentstackCollection{T}.Limit"/> — pagination metadata.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The taxonomy UID does not exist (HTTP 404).</description></item>
        /// <item><description>The <c>taxonomy_publish</c> feature flag is disabled on the stack plan (HTTP 403).</description></item>
        /// <item><description>A network error or server error occurs.</description></item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code>
        /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        ///
        /// var result = await stack.Taxonomy("electronics")
        ///     .Term()
        ///     .SetLocale("en-us")
        ///     .Depth(2)
        ///     .Limit(50)
        ///     .IncludeCount()
        ///     .Find&lt;JsonObject&gt;();
        ///
        /// Console.WriteLine($"Found {result.Items.Count()} of {result.Count} terms.");
        /// foreach (var term in result.Items)
        ///     Console.WriteLine($"{term["uid"]}: {term["name"]} (parent: {term["parent_uid"]})");
        /// </code>
        /// </example>
        public async Task<ContentstackCollection<T>> Find<T>()
        {
            Dictionary<string, object> headers = GetHeader(_headers);
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            mainJson.Add("environment", _stack.Config.Environment);
            foreach (var kvp in _urlQueries)
                mainJson.Add(kvp.Key, kvp.Value);

            try
            {
                string url = $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms";
                HttpRequestHandler handler = new HttpRequestHandler(_stack);
                string result = await handler.ProcessRequest(
                    url, headers, mainJson,
                    Branch: string.IsNullOrEmpty(_stack.Config.Branch) ? null : _stack.Config.Branch,
                    timeout: _stack.Config.Timeout,
                    proxy: _stack.Config.Proxy);

                JsonObject obj = JsonNode.Parse(result ?? "{}")!.AsObject();

                JsonArray termsArray = obj["terms"]?.AsArray();
                IEnumerable<T> terms = Enumerable.Empty<T>();
                if (termsArray != null)
                {
                    terms = JsonSerializer.Deserialize<List<T>>(
                        termsArray.ToJsonString(),
                        _stack.SerializerOptions) ?? new List<T>();
                }

                return ContentstackCollection<T>.FromDeliveryEnvelope(obj, terms);
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
