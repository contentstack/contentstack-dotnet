using System;
using System.Collections.Generic;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using System.Threading.Tasks;
using System.Net;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// GlobalFieldQuery provides query options for working with global fields in Contentstack.
    /// </summary>
    public class GlobalFieldQuery
    {
        #region Public Properties
        internal ContentstackClient StackInstance { get; set; }
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        #endregion

        #region Private Properties
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();

        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.StackInstance.Config;
                return String.Format("{0}/global_fields", config.BaseUrl);
            }
        }
        #endregion

        #region Internal Constructors
       
        internal GlobalFieldQuery()
        {
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
                    ApiErrorBodyParser.TryApply(errorMessage.Replace("\r\n", ""), ref errorCode, ref errorMessage, ref errors);

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
        /// This method returns the complete information for all global fields.
        /// </summary>
        /// <example>
        /// <code>
        /// ContentstackClient stack = new ContentstackClient("api_key", "delivery_token", "environment");
        /// GlobalFieldQuery globalFieldQuery = stack.GlobalFieldQuery();
        /// var param = new Dictionary<string, object>();
        /// param.Add("include_global_field_schema", true);
        /// var result = await globalFieldQuery.Find(param);
        /// </code>
        /// </example>
        /// <param name="param">A dictionary of additional parameters.</param>
        /// <returns>The global field schema object.</returns>
        public async Task<JsonObject> Find(Dictionary<string, object> param = null)
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
                JsonObject data = JsonNode.Parse(outputResult.Replace("\r\n", ""))!.AsObject();

                return data;
            }
            catch (Exception ex)
            {
                if (ex is System.Net.WebException)
                {
                    var contentstackError = GetContentstackError(ex);
                    throw new ContentstackException(contentstackError.Message, ex)
                    {
                        ErrorCode = contentstackError.ErrorCode,
                        StatusCode = contentstackError.StatusCode,
                        Errors = contentstackError.Errors
                    };
                }
                throw new ContentstackException(ErrorMessages.GlobalFieldQueryError, ex);
            }
        }

        public GlobalFieldQuery IncludeBranch()
        {
            this.UrlQueries.Add("include_branch", true);
            return this;
        }

        public GlobalFieldQuery IncludeGlobalFieldSchema()
        {
            this.UrlQueries.Add("include_global_field_schema", true);
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
        #endregion
    }
}
