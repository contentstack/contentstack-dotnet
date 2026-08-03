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
        /// Limits the depth of the term hierarchy returned in the response.
        /// <c>Depth(1)</c> returns only root-level terms; higher values include deeper levels.
        /// </summary>
        /// <param name="depth">The maximum number of hierarchy levels to include.</param>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().Depth(1).Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery Depth(int depth)
        {
            UrlQueries["depth"] = depth;
            return this;
        }

        /// <summary>
        /// Includes branch information in the response for each term.
        /// </summary>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().IncludeBranch().Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery IncludeBranch()
        {
            UrlQueries["include_branch"] = "true";
            return this;
        }

        /// <summary>
        /// Sets the number of terms to skip in the result set (pagination offset).
        /// </summary>
        /// <param name="skip">The number of terms to skip. Must be &gt;= 0.</param>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var page2 = await stack.Taxonomies("gadgets").Terms().Skip(10).Limit(10).Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery Skip(int skip)
        {
            UrlQueries["skip"] = skip;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of terms to return.
        /// </summary>
        /// <param name="limit">The maximum result count.</param>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().Limit(5).Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery Limit(int limit)
        {
            UrlQueries["limit"] = limit;
            return this;
        }

        /// <summary>
        /// Includes the total count of matching terms in the response.
        /// Access it via <see cref="ContentstackCollection{T}.Count"/> on the result.
        /// </summary>
        /// <returns>The current <see cref="TermQuery"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var result = await stack.Taxonomies("gadgets").Terms().IncludeCount().Find&lt;MyTerm&gt;();
        ///     Console.WriteLine($"Total terms: {result.Count}");
        /// </code>
        /// </example>
        public TermQuery IncludeCount()
        {
            UrlQueries["include_count"] = "true";
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
                var result = await TaxonomyRequestHelper.ExecuteRequest(_stack, Url, UrlQueries);

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
                var contentstackError = TaxonomyRequestHelper.GetContentstackError(ex);
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
