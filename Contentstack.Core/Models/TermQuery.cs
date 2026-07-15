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
    /// Supports locale filtering and master-locale fallback.
    /// </summary>
    public class TermQuery
    {
        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private readonly Dictionary<string, object> _queryParams = new Dictionary<string, object>();

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
        /// </summary>
        /// <param name="locale">The locale code (e.g. "hi-in", "en-us").</param>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery SetLocale(string locale)
        {
            if (string.IsNullOrEmpty(locale))
                throw new TaxonomyException("Locale cannot be null or empty.");
            _queryParams["locale"] = locale;
            return this;
        }

        /// <summary>
        /// When a term is not localized in the requested locale, falls back to the master locale.
        /// Must be used together with <see cref="SetLocale"/>.
        /// </summary>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").IncludeFallback().Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery IncludeFallback()
        {
            _queryParams["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Executes the query and returns all matching published terms.
        /// Maps to GET /taxonomies/{uid}/terms with any configured locale and fallback params.
        /// </summary>
        /// <returns>A <see cref="ContentstackCollection{T}"/> containing the matched terms.</returns>
        /// <example>
        /// <code>
        ///     var result = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").IncludeFallback().Find&lt;MyTerm&gt;();
        ///     foreach (var term in result) { ... }
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

                foreach (var param in _queryParams)
                    mainJson[param.Key] = param.Value;

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
