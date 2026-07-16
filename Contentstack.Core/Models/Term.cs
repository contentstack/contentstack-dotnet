using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contentstack.Core.Internals;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Represents a single published term within a taxonomy, providing methods to fetch
    /// the term, its localized versions, ancestors, and descendants from the CDA.
    /// Use <see cref="SetLocale"/>, <see cref="IncludeFallback"/>, and <see cref="AddParam"/> to
    /// configure the request before calling <see cref="Fetch{T}"/>.
    /// </summary>
    public class Term
    {
        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private readonly string _termUid;
        private readonly Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        private string BaseUrlPath =>
            $"{_stack.Config.BaseUrl}/taxonomies/{_taxonomyUid}/terms/{_termUid}";

        internal Term(ContentstackClient stack, string taxonomyUid, string termUid)
        {
            _stack = stack ?? throw new TaxonomyException("ContentstackClient cannot be null when creating a Term instance.");
            if (string.IsNullOrEmpty(taxonomyUid)) throw new TaxonomyException("taxonomyUid cannot be null or empty.");
            if (string.IsNullOrEmpty(termUid))     throw new TaxonomyException("termUid cannot be null or empty.");
            _taxonomyUid = taxonomyUid;
            _termUid = termUid;
        }

        /// <summary>
        /// Sets the locale for this term fetch.
        /// Passing null or empty is a no-op.
        /// </summary>
        /// <param name="locale">Locale code (e.g. "hi-in", "en-us").</param>
        /// <returns>The current <see cref="Term"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var term = await stack.Taxonomies("gadgets").Term("smartwatch").SetLocale("hi-in").Fetch&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public Term SetLocale(string locale)
        {
            if (!string.IsNullOrEmpty(locale))
                UrlQueries["locale"] = locale;
            return this;
        }

        /// <summary>
        /// Falls back to the master locale when the term is not published in the requested locale.
        /// Can be used with or without <see cref="SetLocale"/>.
        /// </summary>
        /// <returns>The current <see cref="Term"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var term = await stack.Taxonomies("gadgets").Term("smartwatch").SetLocale("hi-in").IncludeFallback().Fetch&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public Term IncludeFallback()
        {
            UrlQueries["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Adds a custom query parameter to the request.
        /// </summary>
        /// <param name="key">Parameter key.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The current <see cref="Term"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var term = await stack.Taxonomies("gadgets").Term("smartwatch").AddParam("include_branch", "true").Fetch&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public Term AddParam(string key, string value)
        {
            UrlQueries[key] = value;
            return this;
        }

        /// <summary>
        /// Fetches the published term from the CDA.
        /// Maps to GET /v3/taxonomies/{uid}/terms/{termUid} with any configured query parameters.
        /// </summary>
        /// <returns>The deserialized term object.</returns>
        /// <example>
        /// <code>
        ///     var term         = await stack.Taxonomies("gadgets").Term("smartwatch").Fetch&lt;MyTerm&gt;();
        ///     var localized    = await stack.Taxonomies("gadgets").Term("smartwatch").SetLocale("hi-in").Fetch&lt;MyTerm&gt;();
        ///     var withFallback = await stack.Taxonomies("gadgets").Term("smartwatch").SetLocale("hi-in").IncludeFallback().Fetch&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public async Task<T> Fetch<T>()
        {
            try
            {
                var result = await ExecuteRequest(BaseUrlPath, UrlQueries);
                var jObject = JObject.Parse(result);
                var token = jObject.SelectToken("$.term");
                if (token != null)
                    return token.ToObject<T>(_stack.Serializer);
                return jObject.ToObject<T>(_stack.Serializer);
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

        /// <summary>
        /// Fetches all published localized versions of this term across every locale.
        /// Maps to GET /v3/taxonomies/{uid}/terms/{termUid}/locales.
        /// </summary>
        /// <returns>The deserialized locales collection.</returns>
        /// <example>
        /// <code>
        ///     var locales = await stack.Taxonomies("gadgets").Term("smartwatch").Locales&lt;JToken&gt;();
        /// </code>
        /// </example>
        public async Task<T> Locales<T>()
        {
            try
            {
                var result = await ExecuteRequest($"{BaseUrlPath}/locales");
                var jObject = JObject.Parse(result);
                var token = jObject.SelectToken("$.locales");
                if (token != null)
                    return token.ToObject<T>(_stack.Serializer);
                return jObject.ToObject<T>(_stack.Serializer);
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

        /// <summary>
        /// Fetches all ancestors of this term up to the taxonomy root.
        /// Maps to GET /v3/taxonomies/{uid}/terms/{termUid}/ancestors.
        /// </summary>
        /// <returns>The deserialized ancestors collection.</returns>
        /// <example>
        /// <code>
        ///     var ancestors = await stack.Taxonomies("gadgets").Term("smartwatch").Ancestors&lt;JToken&gt;();
        /// </code>
        /// </example>
        public async Task<T> Ancestors<T>()
        {
            try
            {
                var result = await ExecuteRequest($"{BaseUrlPath}/ancestors");
                var jObject = JObject.Parse(result);
                var token = jObject.SelectToken("$.ancestors");
                if (token != null)
                    return token.ToObject<T>(_stack.Serializer);
                return jObject.ToObject<T>(_stack.Serializer);
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

        /// <summary>
        /// Fetches all descendants of this term.
        /// Maps to GET /v3/taxonomies/{uid}/terms/{termUid}/descendants.
        /// </summary>
        /// <returns>The deserialized descendants collection.</returns>
        /// <example>
        /// <code>
        ///     var descendants = await stack.Taxonomies("gadgets").Term("smartwatch").Descendants&lt;JToken&gt;();
        /// </code>
        /// </example>
        public async Task<T> Descendants<T>()
        {
            try
            {
                var result = await ExecuteRequest($"{BaseUrlPath}/descendants");
                var jObject = JObject.Parse(result);
                var token = jObject.SelectToken("$.descendants");
                if (token != null)
                    return token.ToObject<T>(_stack.Serializer);
                return jObject.ToObject<T>(_stack.Serializer);
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

        private async Task<string> ExecuteRequest(string url, Dictionary<string, object> extraParams = null)
        {
            var headerAll = new Dictionary<string, object>();
            foreach (var header in _stack._LocalHeaders)
                headerAll[header.Key] = header.Value;

            var mainJson = new Dictionary<string, object>();
            if (_stack.Config?.Environment != null)
                mainJson["environment"] = _stack.Config.Environment;

            if (extraParams != null)
                foreach (var kvp in extraParams)
                    mainJson[kvp.Key] = kvp.Value;

            var handler = new HttpRequestHandler(_stack);
            return await handler.ProcessRequest(
                url, headerAll, mainJson,
                Branch: _stack.Config.Branch,
                timeout: _stack.Config.Timeout,
                proxy: _stack.Config.Proxy
            );
        }
    }
}
