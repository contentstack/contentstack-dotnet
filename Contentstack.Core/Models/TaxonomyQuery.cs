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
    /// Query builder for fetching all published taxonomies from the Contentstack Content Delivery API.
    /// </summary>
    /// <remarks>
    /// Obtain an instance via <see cref="ContentstackClient.Taxonomy()"/> (no argument).
    /// Supports pagination and count inclusion. Chain modifier methods before calling
    /// <see cref="Find{T}"/> to execute the request.
    /// </remarks>
    /// <example>
    /// <code>
    /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
    ///
    /// // Fetch all taxonomies with count
    /// var result = await stack.Taxonomy()
    ///     .Limit(10)
    ///     .Skip(0)
    ///     .IncludeCount()
    ///     .Find&lt;JsonObject&gt;();
    ///
    /// Console.WriteLine($"Total: {result.Count}");
    /// foreach (var taxonomy in result.Items)
    ///     Console.WriteLine(taxonomy["name"]);
    /// </code>
    /// </example>
    public class TaxonomyQuery
    {
        #region Private Variables

        private readonly ContentstackClient _stack;
        private Dictionary<string, object> _urlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> _headers = new Dictionary<string, object>();
        private Dictionary<string, object> _stackHeaders = new Dictionary<string, object>();

        #endregion

        #region Internal Constructor

        internal TaxonomyQuery(ContentstackClient stack)
        {
            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
            _stackHeaders = stack._LocalHeaders;
        }

        #endregion

        #region Public Methods — Query Modifiers

        /// <summary>
        /// Sets the number of taxonomies to skip in the result set (pagination offset).
        /// </summary>
        /// <param name="skip">The number of taxonomies to skip. Must be &gt;= 0.</param>
        /// <returns>The current <see cref="TaxonomyQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// // Fetch the second page (10 items per page)
        /// var page2 = await stack.Taxonomy().Skip(10).Limit(10).Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyQuery Skip(int skip)
        {
            _urlQueries["skip"] = skip;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of taxonomies to return.
        /// </summary>
        /// <param name="limit">The maximum result count. Contentstack's default limit applies if not set.</param>
        /// <returns>The current <see cref="TaxonomyQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var taxonomies = await stack.Taxonomy().Limit(5).Find&lt;JsonObject&gt;();
        /// </code>
        /// </example>
        public TaxonomyQuery Limit(int limit)
        {
            _urlQueries["limit"] = limit;
            return this;
        }

        /// <summary>
        /// Includes the total count of published taxonomies in the response.
        /// Access it via <see cref="ContentstackCollection{T}.Count"/> on the result.
        /// </summary>
        /// <returns>The current <see cref="TaxonomyQuery"/> instance for method chaining.</returns>
        /// <example>
        /// <code>
        /// var result = await stack.Taxonomy().IncludeCount().Find&lt;JsonObject&gt;();
        /// Console.WriteLine($"Total taxonomies: {result.Count}");
        /// </code>
        /// </example>
        public TaxonomyQuery IncludeCount()
        {
            _urlQueries["include_count"] = "true";
            return this;
        }

        /// <summary>
        /// Adds an arbitrary query parameter to the request URL.
        /// </summary>
        /// <param name="key">The query parameter key.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The current <see cref="TaxonomyQuery"/> instance for method chaining.</returns>
        public TaxonomyQuery Param(string key, object value)
        {
            _urlQueries[key] = value;
            return this;
        }

        #endregion

        #region Public Methods — Data Retrieval

        /// <summary>
        /// Executes the query and fetches all published taxonomies from the CDA.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize each taxonomy into.
        /// Use <see cref="JsonObject"/> for a schema-less response, or a custom POCO
        /// whose properties map to the taxonomy fields (<c>uid</c>, <c>name</c>,
        /// <c>description</c>, <c>publish_details</c>).
        /// </typeparam>
        /// <returns>
        /// A <see cref="ContentstackCollection{T}"/> containing:
        /// <list type="bullet">
        /// <item><description><see cref="ContentstackCollection{T}.Items"/> — the deserialized taxonomy objects.</description></item>
        /// <item><description><see cref="ContentstackCollection{T}.Count"/> — total count when <see cref="IncludeCount"/> was called.</description></item>
        /// <item><description><see cref="ContentstackCollection{T}.Skip"/> / <see cref="ContentstackCollection{T}.Limit"/> — pagination metadata.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="TaxonomyException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The <c>taxonomy_publish</c> feature flag is disabled on the stack plan (HTTP 403).</description></item>
        /// <item><description>A network error or server error occurs.</description></item>
        /// </list>
        /// </exception>
        /// <example>
        /// <code>
        /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        ///
        /// var result = await stack.Taxonomy()
        ///     .Limit(10)
        ///     .IncludeCount()
        ///     .Find&lt;JsonObject&gt;();
        ///
        /// Console.WriteLine($"Fetched {result.Items.Count()} of {result.Count} taxonomies.");
        /// foreach (var taxonomy in result.Items)
        ///     Console.WriteLine(taxonomy["uid"] + ": " + taxonomy["name"]);
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
                string url = $"{_stack.Config.BaseUrl}/taxonomies";
                HttpRequestHandler handler = new HttpRequestHandler(_stack);
                string result = await handler.ProcessRequest(
                    url, headers, mainJson,
                    Branch: string.IsNullOrEmpty(_stack.Config.Branch) ? null : _stack.Config.Branch,
                    timeout: _stack.Config.Timeout,
                    proxy: _stack.Config.Proxy);

                JsonObject obj = JsonNode.Parse(result ?? "{}")!.AsObject();

                JsonArray taxonomiesArray = obj["taxonomies"]?.AsArray();
                IEnumerable<T> taxonomies = Enumerable.Empty<T>();
                if (taxonomiesArray != null)
                {
                    taxonomies = JsonSerializer.Deserialize<List<T>>(
                        taxonomiesArray.ToJsonString(),
                        _stack.SerializerOptions) ?? new List<T>();
                }

                return ContentstackCollection<T>.FromDeliveryEnvelope(obj, taxonomies);
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
