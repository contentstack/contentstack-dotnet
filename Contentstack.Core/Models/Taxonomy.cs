using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Newtonsoft.Json.Linq;

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
        private Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();
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
        public ContentstackClient Stack
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
            this._StackHeaders = stack._LocalHeaders;
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
                var headerAll = new Dictionary<string, object>();
                foreach (var header in Stack._LocalHeaders)
                    headerAll[header.Key] = header.Value;

                var mainJson = new Dictionary<string, object>();
                if (Stack.Config?.Environment != null)
                    mainJson["environment"] = Stack.Config.Environment;
                foreach (var kvp in UrlQueries)
                    mainJson[kvp.Key] = kvp.Value;

                var handler = new HttpRequestHandler(Stack);
                var result = await handler.ProcessRequest(
                    _Url, headerAll, mainJson,
                    Branch: Stack.Config.Branch,
                    timeout: Stack.Config.Timeout,
                    proxy: Stack.Config.Proxy
                );

                var jObject = Newtonsoft.Json.Linq.JObject.Parse(result);
                var token = jObject.SelectToken("$.taxonomy");
                if (token != null)
                    return token.ToObject<T>(Stack.Serializer);
                return jObject.ToObject<T>(Stack.Serializer);
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
        #region Private Functions

        private Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
        {
            Dictionary<string, object> mainHeader = _StackHeaders;
            Dictionary<string, object> classHeaders = new Dictionary<string, object>();

            if (localHeader != null && localHeader.Count > 0)
            {
                if (mainHeader != null && mainHeader.Count > 0)
                {
                    foreach (var entry in localHeader)
                    {
                        String key = entry.Key;
                        classHeaders.Add(key, entry.Value);
                    }

                    foreach (var entry in mainHeader)
                    {
                        String key = entry.Key;
                        if (!classHeaders.ContainsKey(key))
                        {
                            classHeaders.Add(key, entry.Value);
                        }
                    }

                    return classHeaders;

                }
                else
                {
                    return localHeader;
                }

            }
            else
            {
                return _StackHeaders;
            }
        }
        internal new static ContentstackException GetContentstackError(Exception ex)
        {
            Int32 errorCode = 0;
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ContentstackException contentstackError = new ContentstackException(ex);
            Dictionary<string, object> errors = null;

            try
            {
                System.Net.WebException webEx = ex as System.Net.WebException;
                
                if (webEx != null && webEx.Response != null)
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
                                {
                                    try
                                    {
                                        JObject data = JObject.Parse(errorMessage.Replace("\r\n", ""));

                                        JToken token = data["error_code"];
                                        if (token != null)
                                            errorCode = token.Value<int>();

                                        token = data["error_message"];
                                        if (token != null)
                                            errorMessage = token.Value<string>();

                                        token = data["errors"];
                                        if (token != null)
                                            errors = token.ToObject<Dictionary<string, object>>();
                                    }
                                    catch (Newtonsoft.Json.JsonException)
                                    {
                                        // If JSON parsing fails, use the raw error message
                                        // errorMessage is already set from ReadToEnd()
                                    }
                                }

                                var response = exResp as HttpWebResponse;
                                if (response != null)
                                    statusCode = response.StatusCode;
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

            contentstackError = new ContentstackException(errorMessage)
            {
                ErrorCode = errorCode,
                StatusCode = statusCode,
                Errors = errors
            };

            return contentstackError;
        }
        #endregion
    }
}
