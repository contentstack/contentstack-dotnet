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
    public class AssetLibrary
    {


        #region Internal Variables
        private Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.Stack.Config;
                return String.Format("{0}/assets", config.BaseUrl);
            }
        }
        #endregion

        public ContentstackClient Stack
        {
            get;
            set;
        }

        #region Internal Constructors

        internal AssetLibrary()
        {
        }
        internal AssetLibrary(ContentstackClient stack)
        {
            this.Stack = stack;
            this._StackHeaders = stack._LocalHeaders;
        }

        #endregion
        #region Public Functions

        /// <summary>
        /// Sorts the assets in the given order on the basis of the specified field.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="order">Order.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void SortWithKeyAndOrderBy(String key, OrderBy order)
        {
            if (order == OrderBy.OrderByAscending)
            {
                UrlQueries.Add("asc", key);
            }
            else
            {
                UrlQueries.Add("desc", key);
            }
        }
        /// <summary>
        ///  Provides only the number of assets.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void ObjectsCount()
        {
            UrlQueries.Add("count", "true");
        }

        /// <summary>
        /// This method also includes the total number of assets returned in the response.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void IncludeCount()
        {
            UrlQueries.Add("include_count", "true");
        }

        /// <summary>
        ///  This method includes the relative url of assets.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void IncludeRelativeUrls()
        {
            UrlQueries.Add("relative_urls", "true");
        }

        /// <summary>
        /// Set headers.
        /// </summary>
        /// <param name="key">custom_header_key</param>
        /// <param name="value">custom_header_value</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void SetHeaderForKey(String key, String value)
        {
            if (key != null && value != null)
            {
                if (this._Headers.ContainsKey(key))
                    this._Headers.Remove(key);
                this._Headers.Add(key, value);
            }
        }

        /// <summary>
        /// Remove header key.
        /// </summary>
        /// <param name="key">custom_header_key</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.RemoveHeader(&quot;custom_key&quot;);
        /// </code>
        /// </example>
        public void RemoveHeader(string key)
        {
            if (this._Headers.ContainsKey(key))
            {
                this._Headers.Remove(key);
            }
        }

        public async Task<ContentstackCollection<Asset>> FetchAll()
        {
            Dictionary<string, object> headers = GetHeader(_Headers);

            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            //Dictionary<string, object> urlQueries = new Dictionary<string, object>();

            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }
            }

            mainJson.Add("environment", this.Stack.Config.Environment);

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }
            try
            {
                HttpRequestHandler RequestHandler = new HttpRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson);
                JObject json = JObject.Parse(ContentstackConvert.ToString(outputResult, "{}"));
                var assets = json.SelectToken("$.assets").ToObject<IEnumerable<Asset>>(this.Stack.Serializer);
                var collection = json.ToObject<ContentstackCollection<Asset>>(this.Stack.Serializer);
                collection.Items = assets;
                return collection;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
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
        internal static ContentstackError GetContentstackError(Exception ex)
        {
            Int32 errorCode = 0;
            string errorMessage = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            ContentstackError contentstackError = new ContentstackError(ex);
            Dictionary<string, object> errors = null;
            //ContentstackError.OtherErrors errors = null;

            try
            {
                System.Net.WebException webEx = (System.Net.WebException)ex;

                using (var exResp = webEx.Response)
                using (var stream = exResp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    errorMessage = reader.ReadToEnd();
                    JObject data = JObject.Parse(errorMessage.Replace("\r\n", ""));
                    //errorCode = ContentstackConvert.ToInt32(data.Property("error_code").Value);
                    //errorMessage = ContentstackConvert.ToString(data.Property("error_message").Value);

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

            contentstackError = new ContentstackError()
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
