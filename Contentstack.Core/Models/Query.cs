using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Internals;
using Contentstack.Core.Configuration;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// A class that defines a query that is used to query for Entry instance.
    /// </summary>
    public class Query
    {
        #region Private Variables

        internal Dictionary<string, object> _FormHeaders = new Dictionary<string, object>();
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        protected Dictionary<string, object> QueryValueJson = new Dictionary<string, object>();
        private string _ResultJson = string.Empty; private CachePolicy _CachePolicy;
        private ContentType ContentTypeInstance { get; set; }
        private ContentstackClient TaxonomyInstance { get; set; }
        protected virtual string _Url
        {
            get
            {
                Config config = this.ContentTypeInstance.StackInstance.Config;
                string baseURL = config.getBaseUrl(this.ContentTypeInstance.StackInstance.LivePreviewConfig, this.ContentTypeId);

                return String.Format("{0}/content_types/{1}/entries",
                                     baseURL,
                                     this.ContentTypeId);
            }
           
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Content type uid.
        /// </summary>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///         var result = queryResult.Result.ContentTypeId;
        ///     });
        /// </code>
        /// </example>
        public string ContentTypeId { get; set; }
        public ContentstackClient Stack { get; private set; }

        #endregion

        #region Internal Constructors
        internal Query()
        {

        }
        internal Query(string contentTypeName)
        {
            this.ContentTypeId = contentTypeName;
        }
        internal Query(ContentstackClient Tax)
        {
            SetTaxonomyInstance(Tax);
        }
        #endregion

        #region Internal Functions

        internal static ContentstackException GetContentstackError(Exception ex)
        {
            Int32 errorCode = 0;
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ContentstackException contentstackError = new ContentstackException(ex);
            Dictionary<string, object> errors = null;

            try
            {
                System.Net.WebException webEx = (System.Net.WebException)ex;

                using (var exResp = webEx.Response)
                using (var stream = exResp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    errorMessage = reader.ReadToEnd();
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

                    var response = exResp as HttpWebResponse;
                    if (response != null)
                        statusCode = response.StatusCode;
                }
            }
            catch
            {
                errorMessage = ex.Message;
            }

            contentstackError = new ContentstackException()
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                StatusCode = statusCode,
                Errors = errors
            };

            return contentstackError;
        }
        internal void SetContentTypeInstance(ContentType contentTypeInstance)
        {
            this.ContentTypeInstance = contentTypeInstance;
            //SetLocale("en-us");
        }
        internal void SetTaxonomyInstance(ContentstackClient Tax)
        {
            this.TaxonomyInstance = Tax;
            //SetLocale("en-us");
        }


        #endregion

        #region Public Functions

        /// <summary>
        /// Set Language instance
        /// </summary>
        /// <param name="language">Language value</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.SetLanguage(Language.ENGLISH_UNITED_STATES);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        [ObsoleteAttribute("This method has been deprecated. Use SetLocale instead.", true)]
        public Query SetLanguage(Language language)
        {

            try
            {
                Language languageName = language;
                int localeValue = (int)languageName;
                LanguageCode[] languageCodeValues = Enum.GetValues(typeof(LanguageCode)).Cast<LanguageCode>().ToArray();
                string localeCode = languageCodeValues[localeValue].ToString();
                localeCode = localeCode.Replace("_", "-");

                if (QueryValueJson != null && !QueryValueJson.ContainsKey("locale"))
                {
                    UrlQueries.Remove("locale");
                    UrlQueries.Add("locale", localeCode);
                }
                else
                {
                    UrlQueries["locale"] = localeCode;
                }

            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }


            return this;
        }

        /// <summary>
        /// Sets the locale.
        /// </summary>
        /// <returns>The locale.</returns>
        /// <param name="Locale">Locale.</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.SetLocale("en-us");
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query SetLocale(String Locale)
        {
            if (QueryValueJson != null && !QueryValueJson.ContainsKey("locale"))
            {
                UrlQueries.Remove("locale");
                UrlQueries.Add("locale", Locale);
            }
            else
            {
                UrlQueries["locale"] = Locale;
            }
            return this;
        }

        /// <summary>
        /// To set headers for Contentstack.io Contentstack rest calls.
        /// </summary>
        /// <param name="key">header name.</param>
        /// <param name="value">header value against given header name.</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void SetHeader(String key, String value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                _Headers.Add(key, value);
            }
        }

        /// <summary>
        /// Remove header key
        /// </summary>
        /// <param name="key">header name.</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.RemoveHeader(&quot;custom_key&quot;);
        /// </code>
        /// </example>
        public void RemoveHeader(String key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _Headers.Remove(key);
            }
        }

        /// <summary>
        /// Add a constraint to fetch all entries that contains given value against specified  key.
        /// </summary>
        /// <param name="key">field uid.</param>
        /// <param name="value">field value which get included from the response.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.Where(&quot;uid&quot;, &quot;entry_uid&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Where(String key, Object value)
        {
            try
            {
                if (key != null && value != null)
                {
                    QueryValueJson.Add(key, value);
                }
                else
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
                }
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }

            return this;
        }

        public Query ReferenceIn(String key, Query query)
        {
            try
            {
                if (key != null && query != null)
                {
                    Dictionary<string, object> queryDictionary = new Dictionary<string, object>();
                    queryDictionary.Add(StackConstants.InQuery, query.QueryValueJson);
                    QueryValueJson.Add(key, queryDictionary);
                }
                else
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
                }
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }

            return this;
        }

        public Query ReferenceNotIn(String key, Query query)
        {
            try
            {
                if (key != null && query != null)
                {
                    Dictionary<string, object> queryDictionary = new Dictionary<string, object>();
                    queryDictionary.Add(StackConstants.NotInQuery, query.QueryValueJson);
                    QueryValueJson.Add(key, queryDictionary);
                }
                else
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
                }
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }

            return this;
        }

        /// <summary>
        /// Add a custom query against specified key.
        /// </summary>
        /// <param name="key">field uid.</param>
        /// <param name="value">field value.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.AddQuery(&quot;query_param_key&quot;, &quot;query_param_value&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query AddQuery(String key, String value)
        {
            try
            {
                if (key != null && value != null)
                {
                    QueryValueJson.Add(key, value);
                }
                else
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
                }
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Remove provided query key from custom query if exist.
        /// </summary>
        /// <param name="key">Query name to remove.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.RemoveQuery(&quot;Query_Key&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query RemoveQuery(String key)
        {
            try
            {
                if (QueryValueJson.ContainsKey(key))
                {
                    QueryValueJson.Remove(key);
                }
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Combines all the queries together using AND operator
        /// </summary>
        /// <param name="queryObjects">list of Query instances on which AND query executes.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     ContentType contentTypeObj = stack.ContentType(&quot;contentType_id&quot;);
        ///     Query csQuery = contentTypeObj.Query();
        ///     
        ///     Query query1 = contentTypeObj.Query();
        ///     query1.Where(&quot;username&quot;,&quot;content&quot;);
        ///     
        ///     Query query2 = contentTypeObj.Query();
        ///     query2.Where(&quot;email_address&quot;,&quot;content@email.com&quot;);
        ///     
        ///     List&lt;Query&gt; queryList = new List&lt;Query&gt;();
        ///     queryList.Add(query1);
        ///     queryList.Add(query2);
        ///     
        ///     csQuery.And(queryList);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query And(List<Query> queryObjects)
        {
            if (queryObjects != null && queryObjects.Count > 0)
            {
                try
                {
                    List<Dictionary<string, object>> andValueJson = new List<Dictionary<string, object>>();
                    int count = queryObjects.Count;

                    for (int i = 0; i < count; i++)
                    {
                        andValueJson.Add(queryObjects[i].QueryValueJson);
                    }
                    if (QueryValueJson.ContainsKey(StackConstants.And))
                    {
                        QueryValueJson.Remove(StackConstants.And);
                        QueryValueJson.Add(StackConstants.And, andValueJson);
                    }
                    else
                    {
                        QueryValueJson.Add(StackConstants.And, andValueJson);
                    }

                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint to fetch all entries which satisfy any queries.
        /// </summary>
        /// <param name="queryObjects">list of Query instances on which OR query executes.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     ContentType contentTypeObj = stack.ContentType(&quot;contentType_id&quot;);
        ///     Query csQuery = contentTypeObj.Query();
        ///     
        ///     Query query1 = contentTypeObj.Query();
        ///     query1.Where(&quot;username",&quot;content&quot;);
        ///     
        ///     Query query2 = contentTypeObj.Query();
        ///     query2.Where(&quot;email_address&quot;,&quot;content@email.com&quot;);
        ///     
        ///     List&lt;Query&gt; queryList = new List&lt;Query&gt;();
        ///     queryList.Add(query1);
        ///     queryList.Add(query2);
        ///     
        ///     csQuery.Or(queryList);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Or(List<Query> queryObjects)
        {
            if (queryObjects != null && queryObjects.Count > 0)
            {
                try
                {
                    List<Dictionary<string, object>> orValueJson = new List<Dictionary<string, object>>();
                    int count = queryObjects.Count;

                    for (int i = 0; i < count; i++)
                    {
                        var l = queryObjects[i].QueryValueJson;
                        orValueJson.Add(queryObjects[i].QueryValueJson);
                    }

                    if (QueryValueJson.ContainsKey(StackConstants.Or))
                    {
                        QueryValueJson.Remove(StackConstants.Or);
                        QueryValueJson.Add(StackConstants.Or, orValueJson);
                    }
                    else
                    {
                        QueryValueJson.Add(StackConstants.Or, orValueJson);
                    }

                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.LessThan(&quot;due_date&quot;, &quot;2013-06-25T00:00:00+05:30&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query LessThan(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    //if (QueryValueJson.ContainsKey(key))
                    //{
                    queryValue.Add("$lt", value);
                    QueryValueJson.Add(key, queryValue);
                    //}

                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be less than or equal to the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">the value that provides an upper bound or equal.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.LessThanOrEqualTo(&quot;due_date&quot;, &quot;2013-06-25T00:00:00+05:30&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query LessThanOrEqualTo(String key, Object value)
        {
            if (key != null && value != null)
            {

                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    //if (QueryValueJson.ContainsKey(key))
                    //{
                    queryValue.Add("$lte", value);
                    QueryValueJson.Add(key, queryValue);
                    //}
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }
            return this;
        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be greater than the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">The value that provides an lower bound.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.GreaterThan(&quot;due_date&quot;, &quot;2013-06-25T00:00:00+05:30&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query GreaterThan(String key, Object value)
        {

            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    //if (QueryValueJson.ContainsKey(key))
                    //{
                    queryValue.Add("$gt", value);
                    QueryValueJson.Add(key, queryValue);
                    //}
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key entry to be greater than or equal to the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">The value that provides an lower bound or equal</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.GreaterThanOrEqualTo(&quot;due_date&quot;, &quot;2013-06-25T00:00:00+05:30&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query GreaterThanOrEqualTo(String key, Object value)
        {

            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    //if (QueryValueJson.ContainsKey(key))
                    //{
                    queryValue.Add("$gte", value);
                    QueryValueJson.Add(key, queryValue);
                    //}
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key&#39;s entry to be not equal to the provided value.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="value">The object that must not be equaled.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.NotEqualTo(&quot;due_date&quot;, &quot;2013-06-25T00:00:00+05:30&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query NotEqualTo(String key, Object value)
        {

            if (key != null && value != null)
            {

                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    //if (QueryValueJson.ContainsKey(key))
                    //{
                    queryValue.Add("$ne", value);
                    QueryValueJson.Add(key, queryValue);
                    //}
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }

            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }
            return this;

        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key&#39;s entry to be contained in the provided array.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="values">The possible values for the key&#39;s object.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.ContainedIn(&quot;severity&quot;, new Object[]{&quot;Show Stopper&quot;, &quot;Critical&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query ContainedIn(String key, Object[] values)
        {

            if (key != null && values != null)
            {
                try
                {
                    List<object> valuesArray = new List<object>();
                    int count = values.Length;

                    for (int i = 0; i < count; i++)
                    {
                        valuesArray.Add(values[i]);
                    }
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    queryValue.Add("$in", valuesArray);
                    QueryValueJson.Add(key, queryValue);

                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint to the query that requires a particular key entry&#39;s value not be contained in the provided array.
        /// </summary>
        /// <param name="key">the key to be constrained.</param>
        /// <param name="values">The list of values the key object should not be.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.NotContainedIn(&quot;severity&quot;, new Object[]{&quot;Show Stopper&quot;, &quot;Critical&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query NotContainedIn(String key, Object[] values)
        {

            if (key != null && values != null)
            {
                try
                {
                    List<object> valuesArray = new List<object>();
                    int count = values.Length;

                    for (int i = 0; i < count; i++)
                    {
                        valuesArray.Add(values[i]);
                    }
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    queryValue.Add("$nin", valuesArray);
                    QueryValueJson.Add(key, queryValue);
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;

        }

        /// <summary>
        /// Add a constraint that requires, a specified key exists in response.
        /// </summary>
        /// <param name="key">The key to be constrained.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Exists(&quot;severity&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Exists(String key)
        {

            if (key != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    queryValue.Add("$exists", true);
                    QueryValueJson.Add(key, queryValue);

                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint that requires, a specified key does not exists in response.
        /// </summary>
        /// <param name="key">The key to be constrained.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.NotExists(&quot;severity&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query NotExists(String key)
        {

            if (key != null)
            {
                try
                {

                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    queryValue.Add("$exists", false);
                    QueryValueJson.Add(key, queryValue);
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }

            return this;
        }

        /// <summary>
        ///  This method also includes the content type UIDs of the referenced entries returned in the response.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///      ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeReferenceContentTypeUID();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeReferenceContentTypeUID()
        {
            try
            {
                UrlQueries.Add("include_reference_content_type_uid", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Add a constraint that requires a particular reference key details.
        /// </summary>
        /// <param name="filed_uid">The key to be constrained.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeReference(&quot;for_bug&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeReference(String filed_uid)
        {

            if (filed_uid != null && filed_uid.Length > 0)
            {
                UrlQueries.Add("include[]", filed_uid);
            }
            return this;
        }


        /// <summary>
        /// Add a constraint that requires a particular reference key details.
        /// </summary>
        /// <param name="filed_uids">array keys that to be constrained.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeReference(new String[]{&quot;for_bug&quot;, &quot;assignee&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeReference(String[] filed_uids)
        {
            if (filed_uids != null && filed_uids.Length > 0)
            {
                UrlQueries.Add("include[]", filed_uids);
            }
            return this;
        }


        /// <summary>
        /// Include tags with which to search entries.
        /// </summary>
        /// <param name="tags">Comma separated array of tags with which to search entries.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Tags(new String[]{&quot;tag1&quot;, &quot;tag2&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        private Query Tags(String[] tags)
        {
            try
            {
                if (tags != null)
                {

                    String tagsvalue = null;
                    int count = tags.Length;
                    for (int i = 0; i < count; i++)
                    {
                        tagsvalue = tagsvalue + "," + tags[i];
                    }
                    QueryValueJson.Add("tags", tagsvalue);
                }
                else
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
                }
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Sort the results in ascending order with the given key.
        /// </summary>
        /// <param name="key">The key to order by.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Ascending(&quot;name&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Ascending(String key)
        {
            if (key != null)
            {
                try
                {
                    UrlQueries.Add("asc", key);
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }
            return this;
        }

        /// <summary>
        /// Sort the results in descending order with the given key.
        /// </summary>
        /// <param name="key">The key to order by.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Descending(&quot;name&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Descending(String key)
        {
            if (key != null)
            {
                try
                {
                    UrlQueries.Add("desc", key);
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }
            return this;
        }

        /// <summary>
        /// Retrieve count and data of objects in result.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeCount();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeCount()
        {
            try
            {
                UrlQueries.Add("include_count", "true");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }



        /// <summary>
        /// This call includes metadata in the response.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.IncludeMetadata();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your metadata code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeMetadata()
        {
            try
            {
                UrlQueries.Add("include_metadata", "true");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// This method also includes owner information for all the entries returned in the response.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeOwner();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeOwner()
        {
            try
            {
                UrlQueries.Add("include_owner", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Specifies an array of &#39;only&#39; keys in BASE object that would be &#39;included&#39; in the response.
        /// </summary>
        /// <param name="fieldUid">Array of the &#39;only&#39; keys to be included in response.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Only(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Only(String[] fieldUid)
        {
            try
            {
                if (fieldUid != null && fieldUid.Length > 0)
                {
                    UrlQueries.Add("only[BASE][]", fieldUid);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("IOException source: {0}", e.Source);
            }

            return this;
        }

        /// <summary>
        /// Specifies an array of only keys that would be included in the response.
        /// </summary>
        /// <param name="keys">Array of the only reference keys to be included in response.</param>
        /// <param name="referenceKey">Key who has reference to some other class object.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeOnlyReference(new String[]{&quot;name&quot;, &quot;description&quot;}, &quot;referenceUid&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeOnlyReference(string[] keys, string referenceKey)
        {
            if (keys != null && keys.Length > 0)
            {
                var referenceKeys = new string[] { referenceKey };
                if (UrlQueries.ContainsKey("include[]") == false)
                {
                    UrlQueries.Add("include[]", referenceKeys);

                }
                if (UrlQueries.ContainsKey($"only[{ referenceKey}][]") == false)
                {
                    UrlQueries.Add($"only[{referenceKey}][]", keys);
                }
            }
            return this;
        }
        /// <summary>
        /// Specifies list of field uids that would be excluded from the response.
        /// </summary>
        /// <param name="fieldUids">field uid  which get excluded from the response.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Except(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Except(String[] fieldUids)
        {
            try
            {
                List<string> objectUidForExcept = new List<string>();
                Dictionary<string, object> exceptValueJson = new Dictionary<string, object>();
                if (fieldUids != null && fieldUids.Length > 0)
                {
                    UrlQueries.Add("except[BASE][]", fieldUids);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("IOException source: {0}", e.Source);
            }
            return this;
        }

        /// <summary>
        /// Specifies an array of except keys that would be excluded in the response.
        /// </summary>
        /// <param name="keys">Array of the except reference keys to be excluded in response.</param>
        /// <param name="referenceKey">Key who has reference to some other class object.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     csQuery.IncludeExceptReference(new String[]{&quot;name&quot;, &quot;description&quot;},&quot;referenceUid&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeExceptReference(string[] keys, string referenceKey)
        {
            if (keys != null && keys.Length > 0)
            {
                var referenceKeys = new string[] { referenceKey };
                if (UrlQueries.ContainsKey("include[]") == false)
                {
                    UrlQueries.Add("include[]", referenceKeys);

                }
                if (UrlQueries.ContainsKey($"except[{ referenceKey}][]") == false)
                {
                    UrlQueries.Add($"except[{referenceKey}][]", keys);
                }
            }
            return this;
        }

        /// <summary>
        /// Include fallback locale publish content, if specified locale content is not publish.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeFallback();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeFallback()
        {
            try
            {
                UrlQueries.Add("include_fallback", "true");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Include branch for publish content.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeBranch();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeBranch()
        {
            try
            {
                UrlQueries.Add("include_branch", "true");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }


        /// <summary>
        /// Add param in URL query.
        /// </summary>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.AddParam("include_branch", "true");
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((assetResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query AddParam(string key, string value)
        {
            try
            {
                UrlQueries.Add(key, value);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Include Embedded Objects (Entries and Assets) along with entry/entries details.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.includeEmbeddedItems();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query includeEmbeddedItems()
        {
            try
            {
                UrlQueries.Add("include_embedded_items[]", "BASE");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Include schemas of all returned objects along with objects themselves.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.IncludeSchema();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query IncludeSchema()
        {
            try
            {
                UrlQueries.Add("include_schema", true);
                UrlQueries.Add("include_global_field_schema", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// The number of objects to skip before returning any.
        /// </summary>
        /// <param name="number">No of objects to skip from returned objects.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Skip(2);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Skip(int number)
        {
            try
            {
                UrlQueries.Add("skip", number);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// A limit on the number of objects to return.
        /// </summary>
        /// <param name="number">No of objects to limit.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Limit(2);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Limit(int number)
        {
            try
            {
                UrlQueries.Add("limit", number);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Add a regular expression constraint for finding string values that match the provided regular expression.
        /// </summary>
        /// <param name="key">The key to be constrained.</param>
        /// <param name="regex">The regular expression pattern to match.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Regex(&quot;name&quot;, &quot;^browser&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Regex(String key, String regex)
        {
            if (key != null && regex != null)
            {

                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    queryValue.Add("$regex", regex);

                    QueryValueJson.Add(key, queryValue);
                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }
            return this;
        }

        /// <summary>
        /// Add a regular expression constraint for finding string values that match the provided regular expression.
        /// </summary>
        /// <param name="key">The key to be constrained.</param>
        /// <param name="regex">The regular expression pattern to match.</param>
        /// <param name="modifiers">Any of the following supported Regular expression modifiers.
        /// 				&lt;li&gt;use&lt;b&gt; i &lt;/b&gt; for case-insensitive matching.&lt;/li&gt;
        ///					&lt;li&gt;use&lt;b&gt; m &lt;/b&gt; for making dot match newlines.&lt;/li&gt;
        ///					&lt;li&gt;use&lt;b&gt; x &lt;/b&gt; for ignoring whitespace in regex&lt;/li&gt;</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Regex(&quot;name&quot;, &quot;^browser&quot;, &quot;i&quot;);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Regex(String key, String regex, String modifiers)
        {
            if (key != null && regex != null)
            {

                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>();
                    queryValue.Add("$regex", regex);
                    if (modifiers != null)
                    {
                        queryValue.Add("$options", modifiers);
                    }
                    QueryValueJson.Add(key, queryValue);

                }
                catch (Exception e)
                {
                    throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, null);
            }
            return this;
        }

        /// <summary>
        /// Include tags with which to search entries.
        /// </summary>
        /// <param name="tags">Comma separated array of tags with which to search entries.</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.WhereTags(new String[]{&quot;tag1&quot;, &quot;tag2&quot;});
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query WhereTags(string[] tags)
        {
            if (tags != null && tags.Length > 0)
            {

                UrlQueries.Add("tags", tags);
            }

            return this;
        }



        /// <summary>
        /// To set cache policy using query instance.
        /// </summary>
        /// <param name="cachePolicy">CachePolicy instance</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.SetCachePolicy(CachePolicy.CacheElseNetwork);
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query SetCachePolicy(CachePolicy cachePolicy)
        {
            this._CachePolicy = cachePolicy;
            return this;
        }



        /// <summary>
        /// To set variants header using query instance.
        /// </summary>
        /// <param name="Variant">Query instance</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Variant("variant_entry_1");
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Variant(string variant_header)
        {
            this.SetHeader("x-cs-variant-uid", variant_header);
            return this;
        }



        /// <summary>
        /// To set multiple variants headers using query instance.
        /// </summary>
        /// <param name="Variant">Query instance</param>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Variant(new List<string> { "variant_entry_1", "variant_entry_2", "variant_entry_3" });
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Query Variant(List<string> variant_headers)
        {
            this.SetHeader("x-cs-variant-uid", string.Join(",", variant_headers));
            return this;
        }

        /// <summary>
        /// Execute a Query and Caches its result (Optional)
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public async Task<ContentstackCollection<T>> Find<T>()
        {
            return this.parseJObject<T>(await Exec());

        }

        /// <summary>
        /// Retrieve only count of entries in result.
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.Count();
        ///     csQuery.Find&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public async Task<JObject> Count()
        {
            try
            {
                UrlQueries.Add("count", "true");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return await Exec();
        }


        /// <summary>
        /// Execute a Query and Caches its result (Optional)
        /// </summary>
        /// <returns>Current instance of Query, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Query csQuery = stack.ContentType(&quot;contentType_id&quot;).Query();
        ///     
        ///     csQuery.FindOne&lt;Product&gt;().ContinueWith((queryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public async Task<ContentstackCollection<T>> FindOne<T>()
        {
            if (UrlQueries != null && UrlQueries.ContainsKey("limit"))
            {
                UrlQueries["limit"] = 1;
            }
            else
            {
                UrlQueries.Add("limit", 1);
            }
            return this.parseJObject<T>(await Exec());
        }

        private ContentstackCollection<T> parseJObject<T>(JObject jObject)
        {
          
            if(this.TaxonomyInstance!=null)
            {
                var entries = jObject.SelectToken("$.entries").ToObject<IEnumerable<T>>(this.TaxonomyInstance.Serializer);
                var collection = jObject.ToObject<ContentstackCollection<T>>(this.TaxonomyInstance.Serializer);
                foreach (var entry in entries)
                {
                    if (entry.GetType() == typeof(Entry))
                    {
                        (entry as Entry).SetContentTypeInstance(this.ContentTypeInstance);
                    }
                }
                collection.Items = entries;
                return collection;

            } else
            {
                var entries = jObject.SelectToken("$.entries").ToObject<IEnumerable<T>>(this.ContentTypeInstance.StackInstance.Serializer);
                var collection = jObject.ToObject<ContentstackCollection<T>>(this.ContentTypeInstance.StackInstance.Serializer);
                foreach (var entry in entries)
                {
                    if (entry.GetType() == typeof(Entry))
                    {
                        (entry as Entry).SetContentTypeInstance(this.ContentTypeInstance);
                    }
                }
                collection.Items = entries;
                return collection;
            }
           
        }

        #endregion
        private async Task<JObject> Exec()
        {
            Dictionary<string, Object> headers = GetHeader(_Headers);
            Dictionary<string, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            bool isLivePreview = false;
            if (this.ContentTypeInstance!=null && this.ContentTypeInstance.StackInstance.LivePreviewConfig.Enable == true
                && this.ContentTypeInstance.StackInstance?.LivePreviewConfig.ContentTypeUID == this.ContentTypeInstance.ContentTypeId)
            {
                mainJson.Add("live_preview", this.ContentTypeInstance.StackInstance.LivePreviewConfig.LivePreview ?? "init");

                if (!string.IsNullOrEmpty(this.ContentTypeInstance.StackInstance.LivePreviewConfig.ManagementToken)) {
                    headerAll["authorization"] = this.ContentTypeInstance.StackInstance.LivePreviewConfig.ManagementToken;
                } else if (!string.IsNullOrEmpty(this.ContentTypeInstance.StackInstance.LivePreviewConfig.PreviewToken)) {
                    headerAll["preview_token"] = this.ContentTypeInstance.StackInstance.LivePreviewConfig.PreviewToken;
                } else {
                    throw new InvalidOperationException("Either ManagementToken or PreviewToken is required in LivePreviewConfig");
                }

                if (!string.IsNullOrEmpty(this.ContentTypeInstance.StackInstance.LivePreviewConfig.releaseId))
                {
                    headerAll["release_id"] = this.ContentTypeInstance.StackInstance.LivePreviewConfig.releaseId;
                }
                if (!string.IsNullOrEmpty(this.ContentTypeInstance.StackInstance.LivePreviewConfig.previewTimestamp))
                {
                    headerAll["preview_timestamp"] = this.ContentTypeInstance.StackInstance.LivePreviewConfig.previewTimestamp;
                }

                isLivePreview = true;
            }

            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    if (this.ContentTypeInstance!=null && this.ContentTypeInstance?.StackInstance.LivePreviewConfig.Enable == true
                        && this.ContentTypeInstance?.StackInstance.LivePreviewConfig.ContentTypeUID == this.ContentTypeInstance?.ContentTypeId
                        && header.Key == "access_token"
                        && isLivePreview)
                    {
                        continue;
                    }
                    headerAll.Add(header.Key, (string)header.Value);
                }
            }

            if(this.TaxonomyInstance!=null && this.TaxonomyInstance._LocalHeaders!=null)
            {
                foreach (var header in this.TaxonomyInstance._LocalHeaders)
                {
                    headerAll.Add(header.Key, (string)header.Value);
                }
            }
            if (!isLivePreview && headerAll.ContainsKey("preview_token"))
            {
                headerAll.Remove("preview_token");
            }
            if (!isLivePreview && headerAll.ContainsKey("release_id"))
            {
                headerAll.Remove("release_id");
            }
            if (!isLivePreview && headerAll.ContainsKey("preview_timestamp"))
            {
                headerAll.Remove("preview_timestamp");
            }

            if(this.ContentTypeInstance!=null)
            {
                mainJson.Add("environment", this.ContentTypeInstance?.StackInstance.Config.Environment);
            }
            if (this.TaxonomyInstance!=null && this.TaxonomyInstance.Config.Environment != null)
            {
                mainJson.Add("environment", this.TaxonomyInstance?.Config.Environment);
            }
            if (QueryValueJson != null && QueryValueJson.Count > 0)
                mainJson.Add("query", QueryValueJson);

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }

            try
            {
                if(this.TaxonomyInstance!=null)
                {
                    HttpRequestHandler requestHandler = new HttpRequestHandler(this.TaxonomyInstance);
                    var branch = this.TaxonomyInstance.Config.Branch != null ? this.TaxonomyInstance.Config.Branch : "main";
                    var outputResult = await requestHandler.ProcessRequest(this._Url, headerAll, mainJson, Branch: branch, isLivePreview: isLivePreview, timeout: this.TaxonomyInstance.Config.Timeout);
                    return JObject.Parse(ContentstackConvert.ToString(outputResult, "{}"));
                }
                else
                {
                    HttpRequestHandler requestHandler = new HttpRequestHandler(this.ContentTypeInstance.StackInstance);
                    var outputResult = await requestHandler.ProcessRequest(_Url, headerAll, mainJson, Branch: this.ContentTypeInstance.StackInstance.Config.Branch, isLivePreview: isLivePreview, timeout: this.ContentTypeInstance.StackInstance.Config.Timeout);
                    return JObject.Parse(ContentstackConvert.ToString(outputResult, "{}"));
                }
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
        }


        #region Private Functions
        private Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
        {
            Dictionary<string, object> mainHeader = _FormHeaders;
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
                return _FormHeaders;
            }
        }

        #endregion
    }
}


