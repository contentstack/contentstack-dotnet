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
    /// </summary>
    public class Term
    {
        private readonly ContentstackClient _stack;
        private readonly string _taxonomyUid;
        private readonly string _termUid;

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
        /// Fetches the published term from the CDA.
        /// </summary>
        /// <param name="locale">Optional locale code (e.g. "mr-in"). Omit for the master locale.</param>
        /// <param name="includeFallback">When true, falls back to the master locale if the term has no localized version.</param>
        /// <returns>The deserialized term object.</returns>
        /// <example>
        /// <code>
        ///     var term           = await stack.Taxonomies("gadgets").Term("smartwatch").Fetch&lt;MyTerm&gt;();
        ///     var localizedTerm  = await stack.Taxonomies("gadgets").Term("smartwatch").Fetch&lt;MyTerm&gt;("mr-in");
        ///     var withFallback   = await stack.Taxonomies("gadgets").Term("laptop").Fetch&lt;MyTerm&gt;("hi-in", includeFallback: true);
        /// </code>
        /// </example>
        public async Task<T> Fetch<T>(string locale = null, bool includeFallback = false)
        {
            try
            {
                var queryParams = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(locale))
                    queryParams["locale"] = locale;
                if (includeFallback)
                    queryParams["include_fallback"] = "true";

                var result = await ExecuteRequest(BaseUrlPath, queryParams);
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
        /// Fetches all published, localized versions of this term across every locale.
        /// Maps to GET /taxonomies/{uid}/terms/{termUid}/locales.
        /// </summary>
        /// <returns>The deserialized locales collection.</returns>
        /// <example>
        /// <code>
        ///     var locales = await stack.Taxonomies("gadgets").Term("smartwatch").Locales&lt;MyLocales&gt;();
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
        /// Fetches all ancestors of this term up to the root.
        /// Maps to GET /taxonomies/{uid}/terms/{termUid}/ancestors.
        /// </summary>
        /// <returns>The deserialized ancestors collection.</returns>
        /// <example>
        /// <code>
        ///     var ancestors = await stack.Taxonomies("gadgets").Term("smartwatch").Ancestors&lt;MyTerms&gt;();
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
        /// Maps to GET /taxonomies/{uid}/terms/{termUid}/descendants.
        /// </summary>
        /// <returns>The deserialized descendants collection.</returns>
        /// <example>
        /// <code>
        ///     var descendants = await stack.Taxonomies("gadgets").Term("smartwatch").Descendants&lt;MyTerms&gt;();
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
                foreach (var param in extraParams)
                    mainJson[param.Key] = param.Value;

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
