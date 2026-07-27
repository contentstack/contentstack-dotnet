using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Represents a published taxonomy from the CDA, providing methods to fetch the taxonomy,
    /// list its terms, and navigate to individual terms.
    /// Use <see cref="SetLocale"/>, <see cref="IncludeFallback"/>, and <see cref="AddParam"/> to
    /// configure the request before calling <see cref="Fetch{T}"/>.
    /// </summary>
    public class Taxonomy: Query
    {

        #region Internal Variables
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();
        private string _uid = null;

        protected override string _Url
        {
            get
            {
                if (this.Stack == null)
                {
                    throw new TaxonomyException("Taxonomy Stack instance is null. Please ensure the Taxonomy is properly initialized with a ContentstackClient instance.");
                }
                if (this.Stack.Config == null)
                {
                    throw new TaxonomyException("Taxonomy Stack Config is null. Please ensure the ContentstackClient is properly configured.");
                }
                Config config = this.Stack.Config;
                if (_uid != null)
                    return String.Format("{0}/taxonomies/{1}", config.BaseUrl, _uid);
                return String.Format("{0}/taxonomies/entries", config.BaseUrl);
            }
        }
        #endregion
        internal new ContentstackClient Stack
        {
            get;
            set;
        }

        #region Internal Constructors

        internal Taxonomy()
        {
        }
        internal Taxonomy(ContentstackClient stack): base(stack)
        {
            if (stack == null)
            {
                throw new TaxonomyException("ContentstackClient instance cannot be null when creating a Taxonomy instance.");
            }
            this.Stack = stack;
        }

        internal Taxonomy(ContentstackClient stack, string uid) : this(stack)
        {
            if (string.IsNullOrEmpty(uid))
                throw new TaxonomyException("Taxonomy UID cannot be null or empty.");
            _uid = uid;
        }

        #endregion
        #region Public Functions

        /// <summary>
        /// Sets the locale for this taxonomy fetch.
        /// Passing null or empty is a no-op.
        /// </summary>
        /// <param name="locale">Locale code (e.g. "hi-in", "en-us").</param>
        /// <returns>The current <see cref="Taxonomy"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var taxonomy = await stack.Taxonomies("gadgets").SetLocale("hi-in").Fetch&lt;MyTaxonomy&gt;();
        /// </code>
        /// </example>
        public new Taxonomy SetLocale(string locale)
        {
            if (!string.IsNullOrEmpty(locale))
                UrlQueries["locale"] = locale;
            return this;
        }

        /// <summary>
        /// Falls back to the master locale when the taxonomy is not published in the requested locale.
        /// Can be used with or without <see cref="SetLocale"/>.
        /// </summary>
        /// <returns>The current <see cref="Taxonomy"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var taxonomy = await stack.Taxonomies("gadgets").SetLocale("hi-in").IncludeFallback().Fetch&lt;MyTaxonomy&gt;();
        /// </code>
        /// </example>
        public new Taxonomy IncludeFallback()
        {
            UrlQueries["include_fallback"] = "true";
            return this;
        }

        /// <summary>
        /// Adds a custom query parameter to the request.
        /// </summary>
        /// <param name="key">Parameter key.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>The current <see cref="Taxonomy"/> for chaining.</returns>
        /// <example>
        /// <code>
        ///     var taxonomy = await stack.Taxonomies("gadgets").AddParam("include_branch", "true").Fetch&lt;MyTaxonomy&gt;();
        /// </code>
        /// </example>
        public new Taxonomy AddParam(string key, string value)
        {
            UrlQueries[key] = value;
            return this;
        }

        /// <summary>
        /// Returns a Term instance for the given term UID within this taxonomy.
        /// Requires the Taxonomy to be initialised with a UID via <c>client.Taxonomies("uid")</c>.
        /// </summary>
        /// <param name="termUid">The UID of the term to retrieve.</param>
        /// <returns>A <see cref="Term"/> instance scoped to this taxonomy and term.</returns>
        /// <example>
        /// <code>
        ///     var term = await stack.Taxonomies("gadgets").Term("smartwatch").Fetch&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public Term Term(string termUid)
        {
            if (_uid == null)
                throw new TaxonomyException("Term() requires a taxonomy UID. Use client.Taxonomies(\"uid\") to scope to a specific taxonomy.");
            return new Term(Stack, _uid, termUid);
        }

        /// <summary>
        /// Returns a TermQuery for listing all published terms within this taxonomy.
        /// Requires the Taxonomy to be initialised with a UID via <c>client.Taxonomies("uid")</c>.
        /// </summary>
        /// <returns>A <see cref="TermQuery"/> instance for this taxonomy.</returns>
        /// <example>
        /// <code>
        ///     var terms = await stack.Taxonomies("gadgets").Terms().SetLocale("hi-in").Find&lt;MyTerm&gt;();
        /// </code>
        /// </example>
        public TermQuery Terms()
        {
            if (_uid == null)
                throw new TaxonomyException("Terms() requires a taxonomy UID. Use client.Taxonomies(\"uid\") to scope to a specific taxonomy.");
            return new TermQuery(Stack, _uid);
        }

        /// <summary>
        /// Fetches the published taxonomy by its UID from the CDA.
        /// Requires the Taxonomy to be initialised with a UID via <c>client.Taxonomies("uid")</c>.
        /// Use <see cref="SetLocale"/> and <see cref="IncludeFallback"/> to set query parameters before calling.
        /// </summary>
        /// <returns>The deserialized taxonomy object.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        ///     var taxonomy = await stack.Taxonomies("gadgets").Fetch&lt;MyTaxonomy&gt;();
        ///     var localized = await stack.Taxonomies("gadgets").SetLocale("hi-in").Fetch&lt;MyTaxonomy&gt;();
        ///     var withFallback = await stack.Taxonomies("gadgets").SetLocale("hi-in").IncludeFallback().Fetch&lt;MyTaxonomy&gt;();
        /// </code>
        /// </example>
        public async Task<T> Fetch<T>()
        {
            if (_uid == null)
                throw new TaxonomyException("Fetch() requires a taxonomy UID. Use client.Taxonomies(\"uid\") to scope to a specific taxonomy.");

            try
            {
                var result = await TaxonomyRequestHelper.ExecuteRequest(Stack, _Url, UrlQueries);

                var jObject = Newtonsoft.Json.Linq.JObject.Parse(result);
                var token = jObject.SelectToken("$.taxonomy");
                if (token != null)
                    return token.ToObject<T>(Stack.Serializer);
                return jObject.ToObject<T>(Stack.Serializer);
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

        /// <summary>
        /// Fetches all published taxonomies from the CDA.
        /// Only valid on an unscoped <see cref="Taxonomy"/> instance (<c>client.Taxonomies()</c>, no UID) —
        /// throws <see cref="TaxonomyException"/> if called on a UID-scoped instance.
        /// Use <see cref="Query.Skip(int)"/>-style pagination via <see cref="AddParam"/>, or the
        /// dedicated <c>skip</c>/<c>limit</c>/<c>include_count</c> params, before calling.
        /// </summary>
        /// <returns>A <see cref="ContentstackCollection{T}"/> containing the published taxonomies.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        ///     var result = await stack.Taxonomies().List&lt;MyTaxonomy&gt;();
        ///     var page = await stack.Taxonomies().AddParam("skip", "0").AddParam("limit", "10").List&lt;MyTaxonomy&gt;();
        /// </code>
        /// </example>
        public async Task<ContentstackCollection<T>> List<T>()
        {
            if (_uid != null)
                throw new TaxonomyException("List() is only valid on an unscoped Taxonomy. Use client.Taxonomies() without a UID.");

            try
            {
                Config config = this.Stack.Config;
                var url = String.Format("{0}/taxonomies", config.BaseUrl);
                var result = await TaxonomyRequestHelper.ExecuteRequest(Stack, url, UrlQueries);

                var jObject = Newtonsoft.Json.Linq.JObject.Parse(result);
                var taxonomies = jObject.SelectToken("$.taxonomies")?.ToObject<IEnumerable<T>>(Stack.Serializer);
                var collection = jObject.ToObject<ContentstackCollection<T>>(Stack.Serializer);
                collection.Items = taxonomies ?? Enumerable.Empty<T>();
                return collection;
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

        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be less than the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">the value that provides an upper bound.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet("api_key", "delivery_token", "environment");
        ///     Query csQuery = stack.ContentType("contentType_id").Query();
        ///     
        ///     csQuery.LessThan("due_date", "2013-06-25T00:00:00+05:30");
        ///     csQuery.Find<Product>().ContinueWith((queryResult) => {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Taxonomy Above(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>{ { "$above", value } };
                    QueryValueJson.Add(key, queryValue);
                } catch (Exception e) {
                    throw TaxonomyException.CreateForProcessingError(e);
                }
            }
            else
            {
                throw TaxonomyException.CreateForProcessingError(null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be less than the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">the value that provides an upper bound.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet("api_key", "delivery_token", "environment");
        ///     Query csQuery = stack.ContentType("contentType_id").Query();
        ///     
        ///     csQuery.LessThan("due_date", "2013-06-25T00:00:00+05:30");
        ///     csQuery.Find<Product>().ContinueWith((queryResult) => {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Taxonomy EqualAndAbove(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>{ { "$eq_above", value } };
                    QueryValueJson.Add(key, queryValue);
                }
                catch (Exception e)
                {
                    throw TaxonomyException.CreateForProcessingError(e);
                }
            }
            else
            {
                throw TaxonomyException.CreateForProcessingError(null);
            }

            return this;
        }
        
        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be less than the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">the value that provides an upper bound.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet("api_key", "delivery_token", "environment");
        ///     Query csQuery = stack.ContentType("contentType_id").Query();
        ///     
        ///     csQuery.LessThan("due_date", "2013-06-25T00:00:00+05:30");
        ///     csQuery.Find<Product>().ContinueWith((queryResult) => {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Taxonomy Below(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object> { { "$below", value } };
                    QueryValueJson.Add(key, queryValue);
                }
                catch (Exception e)
                {
                    throw TaxonomyException.CreateForProcessingError(e);
                }
            }
            else
            {
                throw TaxonomyException.CreateForProcessingError(null);
            }

            return this;
        }
        
        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be less than the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">the value that provides an upper bound.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet("api_key", "delivery_token", "environment");
        ///     Query csQuery = stack.ContentType("contentType_id").Query();
        ///     
        ///     csQuery.LessThan("due_date", "2013-06-25T00:00:00+05:30");
        ///     csQuery.Find<Product>().ContinueWith((queryResult) => {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Taxonomy EqualAndBelow(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object> { { "$eq_below", value } };
                    QueryValueJson.Add(key, queryValue);
                }
                catch (Exception e)
                {
                    throw TaxonomyException.CreateForProcessingError(e);
                }
            }
            else
            {
                throw TaxonomyException.CreateForProcessingError(null);
            }

            return this;
        }

        #endregion
    }
}
