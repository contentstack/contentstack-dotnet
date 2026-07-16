using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Core.Internals;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Provides a fluent query builder for listing published terms within a taxonomy from the CDA.
    /// Use <see cref="SetLocale"/>, <see cref="IncludeFallback"/>, and <see cref="AddParam"/> to
    /// configure the request before calling <see cref="Find{T}"/>.
    /// </summary>
    public class TermQuery
    {
        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private readonly Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        private string Url =>
            $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms";

        internal TermQuery(ContentstackClient stack, string taxonomyUid)
        {
            _stack = stack ?? throw new TaxonomyException("ContentstackClient cannot be null when creating a TermQuery instance.");
            if (string.IsNullOrEmpty(taxonomyUid)) throw new TaxonomyException("taxonomyUid cannot be null or empty.");
            _taxonomyUid = taxonomyUid;
        }

        /// <summary>
        /// Filters terms to those published in the specified locale.
        /// Passing null or empty is a no-op.
        /// </summary>
        /// <param name="locale">Locale code (e.g. "hi-in", "en-us").</param>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery SetLocale(string locale)
        {
            if (!string.IsNullOrEmpty(locale))
                UrlQueries["locale"] = locale;
            return this;
        }

        /// <summary>
        /// Falls back to the master locale when a term is not published in the requested locale.
        /// Can be used with or without <see cref="SetLocale"/>.
        /// </summary>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").IncludeFallback().Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery IncludeFallback()
        {
            UrlQueries["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Adds a custom query parameter to the request.
        /// </summary>
        /// <param name="key">Parameter key.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().AddParam("depth", "2").Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery AddParam(string key, string value)
        {
            UrlQueries[key] = value;
            return this;
        }

        /// <summary>
        /// Executes the query and returns all matching published terms.
        /// Maps to GET /v3/taxonomies/{uid}/terms with any configured query parameters.
        /// </summary>
        /// <returns>A <see cref="ContentstackCollection{T}"/> containing the matched terms.</returns>
        /// <example>
        /// <code>
        ///     var result = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").IncludeFallback().Find&lt;MyTerm&gt;();
        ///     foreach (var term in result.Items) { ... }
        /// </code>
        /// </example>
        public async Task<ContentstackCollection<T>> Find<T>()
        {
            try
            {
                var headerAll = new Dictionary<string, object>();
                foreach (var header in _stack._LocalHeaders)
                    headerAll[header.Key] = header.Value;

                var mainJson = new Dictionary<string, object>();
                if (_stack.Config?.Environment != null)
                    mainJson["environment"] = _stack.Config.Environment;

                foreach (var kvp in UrlQueries)
                    mainJson[kvp.Key] = kvp.Value;

                var handler = new HttpRequestHandler(_stack);
                var result = await handler.ProcessRequest(
                    Url, headerAll, mainJson,
                    Branch: _stack.Config.Branch,
                    timeout: _stack.Config.Timeout,
                    proxy: _stack.Config.Proxy
                );

                var jObject = JObject.Parse(result);
                var terms = jObject.SelectToken("$.terms")?.ToObject<IEnumerable<T>>(_stack.Serializer);
                var collection = jObject.ToObject<ContentstackCollection<T>>(_stack.Serializer);
                collection.Items = terms ?? Enumerable.Empty<T>();
                return collection;
            }
            catch (TaxonomyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var contentstackError = Taxonomy.GetContentstackError(ex);
                throw new TaxonomyException(contentstackError.Message, ex)
                {
                    ErrorCode = contentstackError.ErrorCode,
                    StatusCode = contentstackError.StatusCode,
                    Errors = contentstackError.Errors
                };
            }
        }
    }
}
