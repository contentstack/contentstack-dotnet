using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Models
{
    public class Taxonomy: Query
    {


        #region Internal Variables
        private Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> QueryValueJson = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.Stack.Config;
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
        internal Taxonomy(ContentstackClient stack)
        {
            this.Stack = stack;
            this._StackHeaders = stack._LocalHeaders;
        }

        #endregion
        #region Public Functions

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
        public Taxonomy Above(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object>{ { "$above", value } };
                    QueryValueJson.Add(key, queryValue);
                } catch (Exception e) {
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
        public Taxonomy EqualAndBelow(String key, Object value)
        {
            if (key != null && value != null)
            {
                try
                {
                    Dictionary<string, object> queryValue = new Dictionary<string, object> { { "eq_$below", value } };
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
        #endregion
    }
}
