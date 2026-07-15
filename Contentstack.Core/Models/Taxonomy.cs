using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
namespace Contentstack.Core.Models
{
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
        public new ContentstackClient Stack
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
        /// Returns a Term instance for the given term UID within this taxonomy.
        /// Requires the Taxonomy to be initialised with a UID via <c>client.Taxonomies("uid")</c>.
        /// </summary>
        /// <param name="termUid">The UID of the term to retrieve.</param>
        /// <returns>A <see cref="Term"/> instance scoped to this taxonomy and term.</returns>
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
        public TermQuery Terms()
        {
            if (_uid == null)
                throw new TaxonomyException("Terms() requires a taxonomy UID. Use client.Taxonomies(\"uid\") to scope to a specific taxonomy.");
            return new TermQuery(Stack, _uid);
        }

        /// <summary>
        /// Fetches the published taxonomy by its UID from the CDA.
        /// Requires the Taxonomy to be initialised with a UID via <c>client.Taxonomies("uid")</c>.
        /// </summary>
        /// <param name="locale">Optional locale code (e.g. "hi-in"). Omit for the master locale.</param>
        /// <returns>The deserialized taxonomy object.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        ///     var taxonomy = await stack.Taxonomies("gadgets").Fetch&lt;MyTaxonomy&gt;();
        ///     var localized = await stack.Taxonomies("gadgets").Fetch&lt;MyTaxonomy&gt;("hi-in");
        /// </code>
        /// </example>
        public async System.Threading.Tasks.Task<T> Fetch<T>(string locale = null)
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
                if (!string.IsNullOrEmpty(locale))
                    mainJson["locale"] = locale;

                var handler = new HttpRequestHandler(Stack);
                var branch = Stack.Config?.Branch ?? "main";
                var result = await handler.ProcessRequest(
                    _Url, headerAll, mainJson,
                    Branch: branch,
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
                throw TaxonomyException.CreateForProcessingError(ex);
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
        internal static new ContentstackException GetContentstackError(Exception ex)
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
                                    ApiErrorBodyParser.TryApply(errorMessage.Replace("\r\n", ""), ref errorCode, ref errorMessage, ref errors);
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
