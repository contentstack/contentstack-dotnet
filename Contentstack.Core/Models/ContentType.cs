﻿using System;
using System.Collections.Generic;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// ContentType provides Entry and Query instance.
    /// </summary>
    public class ContentType
    {
        #region Public Properties
        internal ContentstackClient StackInstance { get; set; }
        internal string Uid { get; set; }
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        /// <summary>
        /// Content type uid
        /// </summary>
        public string ContentTypeName
        {
            get;
            set;
        }
        #endregion

        #region Private Properties
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.StackInstance.config;
                return String.Format("{0}/content_types/{1}", config.BaseUrl,this.ContentTypeName);
             }
        }
        #endregion

        #region Internal Constructors
        /// <summary>
        /// 
        /// </summary>
        protected ContentType() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentTypeName"></param>
        internal ContentType(String contentTypeName)
        {
            this.ContentTypeName = contentTypeName;
            this._Headers = new Dictionary<string, object>();
        }
        #endregion

        #region Internal Functions

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

        internal void SetStackInstance(ContentstackClient stack)
        {
            this.StackInstance = stack;
            this._StackHeaders = stack._LocalHeaders;
        }
        #endregion

        #region Public Functions

        public async Task<JObject>Fetch()
        {
            Dictionary<String, Object> headers = GetHeader(_Headers);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            //Dictionary<string, object> urlQueries = new Dictionary<string, object>();

            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }

                if (headers.ContainsKey("environment"))
                {
                    UrlQueries.Add("environment", headers["environment"]);
                    //Url = Url + "?environment=" + headers["environment"];
                }
                else if (headers.ContainsKey("environment_uid"))
                {
                    UrlQueries.Add("environment_uid", headers["environment_uid"]);
                    //Url = Url + "?environment_uid=" + headers["environment_uid"];
                }
                else
                {

                    mainJson.Add("environment", this.StackInstance.config.Environment);
                }
            }

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }
            try
            {
                HTTPRequestHandler RequestHandler = new HTTPRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson);
                JObject data = JsonConvert.DeserializeObject<JObject>(outputResult.Replace("\r\n", ""), ContentstackConvert.JsonSerializerSettings);
                JObject contentTypes = (Newtonsoft.Json.Linq.JObject)data["content_type"];
                return contentTypes;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
        }

        /// <summary>
        /// To set headers for Contentstack rest calls.
        /// </summary>
        /// <param name="key">header name.</param>
        /// <param name="value">header value against given header name.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        ///     contentType.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void SetHeader(string key, string value)
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
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        ///     contentType.RemoveHeader(&quot;custom_header_key&quot;);
        /// </code>
        /// </example>
        public void RemoveHeader(string key)
        {
            if (this._Headers.ContainsKey(key))
                this._Headers.Remove(key);

        }
            
        /// <summary>
        /// Represents a Entry. 
        /// Create Entry Instance.
        /// </summary>
        /// <param name="entryUid">Set entry uid.</param>
        /// <returns>Entry Instance</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        ///     Entry entry = contentType.Entry(&quot;bltf4fbbc94e8c851db&quot;);
        /// </code>
        /// </example>
        public Entry Entry(String entryUid)
        {
            Entry entry = new Entry(ContentTypeName);
            entry._FormHeaders = GetHeader(_Headers);
            entry.SetContentTypeInstance(this);
            Console.Write("entry UID: ", entryUid);
            entry.SetUid(entryUid);
            return entry;
        }

        /// <summary>
        /// Represents a Query
        /// Create Query Instance.
        /// </summary>
        /// <returns>Query Instance.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        ///     Query csQuery = contentType.Query();
        /// </code>
        /// </example>
        public Query Query()
        {
            Query query = new Query(ContentTypeName);
            query._FormHeaders = GetHeader(_Headers);
            query.SetContentTypeInstance(this);

            return query;
        }
        #endregion

        protected Entry Entry()
        {
            Entry entry = new Entry(ContentTypeName);
            entry._FormHeaders = GetHeader(_Headers);
            entry.SetContentTypeInstance(this);

            return entry;
        }

        #region Private Functions
        private Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
        {
            Dictionary<string, object> mainHeader = _StackHeaders;
            Dictionary<string, object> classHeaders = new Dictionary<string, object>();

            if (localHeader != null && localHeader.Count > 0)
            {
                if (mainHeader != null && mainHeader.Count > 0)
                {
                    foreach (var entry in  localHeader)
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
        #endregion
    }
}
