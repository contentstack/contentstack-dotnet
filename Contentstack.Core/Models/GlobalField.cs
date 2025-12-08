using System;
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
    /// GlobalField provides Entry and Query instances for working with global fields in Contentstack.
    /// </summary>
    public class GlobalField
    {
        #region Public Properties
        internal ContentstackClient StackInstance { get; set; }
        internal string Uid { get; set; }
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        /// <summary>
        /// GlobalField uid
        /// </summary>
        public string GlobalFieldId
        {
            get
            {
                return this.Uid;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw GlobalFieldException.CreateForIdNull();
                this.Uid = value;
            }
        }

        #endregion

        #region Private Properties
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();

        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.StackInstance.Config;
                return String.Format("{0}/global_fields/{1}", config.BaseUrl, this.GlobalFieldId);
            }
        }
        #endregion

        #region Internal Constructors
        /// <summary>
        /// 
        /// </summary>
        protected GlobalField() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalFieldId"></param>
        internal GlobalField(String globalFieldId = null)
        {
            if (globalFieldId != null)
            {
                this.GlobalFieldId = globalFieldId;
            }
            this._Headers = new Dictionary<string, object>();
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

            contentstackError = new ContentstackException(errorMessage)
            {
                ErrorCode = errorCode,
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
        /// <summary>
        /// This method returns the complete information of a specific global field.
        /// </summary>
        /// <example>
        /// <code>
        /// ContentstackClient stack = new ContentstackClient(api_key, delivery_token, environment);
        /// GlobalField globalField = stack.GlobalField("globalField_uid");
        /// var param = new Dictionary<string, object>();
        /// param.Add("include_global_field_schema", true);
        /// var result = await globalField.Fetch(param);
        /// </code>
        /// </example>
        /// <param name="param">A dictionary of additional parameters.</param>
        /// <returns>The Global Field schema object.</returns>
        public async Task<JObject> Fetch(Dictionary<string, object> param = null)
        {
            Dictionary<String, Object> headers = GetHeader(_Headers);
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }
            }
            mainJson.Add("environment", this.StackInstance.Config.Environment);
            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }
            if (param != null && param.Count() > 0)
            {
                foreach (var kvp in param)
                {
                    mainJson.Add(kvp.Key, kvp.Value);
                }
            }
            try
            {
                HttpRequestHandler RequestHandler = new HttpRequestHandler(this.StackInstance);
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson, Branch: this.StackInstance.Config.Branch, timeout: this.StackInstance.Config.Timeout, proxy: this.StackInstance.Config.Proxy);
                JObject data = JsonConvert.DeserializeObject<JObject>(outputResult.Replace("\r\n", ""), this.StackInstance.SerializerSettings);

                JObject globalTypes = (Newtonsoft.Json.Linq.JObject)data["global_field"];
                return globalTypes;
            }
            catch (Exception ex)
            {
                if (ex is System.Net.WebException)
                {
                    var contentstackError = GetContentstackError(ex);
                    throw new GlobalFieldException(contentstackError.Message, ex)
                    {
                        ErrorCode = contentstackError.ErrorCode,
                        StatusCode = contentstackError.StatusCode,
                        Errors = contentstackError.Errors
                    };
                }
                throw GlobalFieldException.CreateForProcessingError(ex);
            }
        }

        public GlobalField IncludeBranch()
        {
            this.UrlQueries.Add("include_branch", true);
            return this;
        }

        public GlobalField IncludeGlobalFieldSchema()
        {
            this.UrlQueries.Add("include_global_field_schema", true);
            return this;
        }
        /// <summary>
        /// Sets a header for Contentstack REST calls.
        /// </summary>
        /// <param name="key">Header name.</param>
        /// <param name="value">Header value for the given header name.</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClient(api_key, delivery_token, environment);
        ///     GlobalField globalField = stack.GlobalField(globalField_name);
        ///     globalField.SetHeader(custom_header_key, custom_header_value);
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
        /// Removes a header key.
        /// </summary>
        /// <param name="key">Custom header key to remove.</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClient(api_key, delivery_token, environment);
        ///     GlobalField globalField = stack.GlobalField(globalField_name);
        ///     globalField.RemoveHeader(custom_header_key);
        /// </code>
        /// </example>
        public void RemoveHeader(string key)
        {
            if (this._Headers.ContainsKey(key))
                this._Headers.Remove(key);
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
        #endregion
    }
}
