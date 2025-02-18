using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Utils.Interfaces;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Assets refer to all the media files (images, videos, PDFs, audio files, and so on) uploaded in your Contentstack repository for future use
    /// </summary>
    public class Asset : IEmbeddedObject
    {
        #region Internal & Private Properties
        private Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();
        private readonly Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.StackInstance.Config;
                return String.Format("{0}/assets/{1}", config.BaseUrl, this.Uid);
            }
        }
        #endregion
        public ContentstackClient StackInstance
        {
            get;
            set;
        }
        internal void SetStackInstance(ContentstackClient contentstackClient)
        {
            this.StackInstance = contentstackClient;
        }
        public object this[string key]
        {
            get
            {
                object output = null;

                if (this._ObjectAttributes.ContainsKey(key))
                {
                    output = this._ObjectAttributes[key];
                }

                return output;
            }
            set
            {
                try
                {
                    this._ObjectAttributes[key] = value;
                }
                catch (Exception e)
                {
                    if (e.Source != null)
                    {
                        Console.WriteLine("IOException source: {0}", e.Source);
                    }
                }
            }
        }

        /// <summary>
        /// An absolute URL to this file.
        /// </summary>
        public string Url
        {
            get
            {
                string url = string.Empty;
                if (this._ObjectAttributes.ContainsKey("url"))
                {
                    url = ContentstackConvert.ToString(this["url"]);
                }
                return url;
            }
            set
            {
                this["url"] = value;
            }
        }

        /// <summary>
        /// This is Asset Uid of an Asset.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// This is Asset type uid.
        /// </summary>
        [JsonPropertyName("_content_type_uid")]
        public string ContentTypeUid { get; set; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        [JsonPropertyName("file_size")]
        public string FileSize { get; set; }

        /// <summary>
        /// The original name of the file.
        /// </summary>
        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        /// <summary>
        /// This is Asset description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Set array of Tags
        /// </summary>
        public Object[] Tags { get; set; }

        /// <summary>
        /// This content_type in asset.
        /// </summary>
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// This for whether it is asset directory
        /// </summary>
        [JsonPropertyName("is_dir")]
        public Boolean IsDir { get; set; }

        /// <summary>
        /// Uid of user who updated the file
        /// </summary>
        [JsonPropertyName("updated_by")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Uid of user who updated the file
        /// </summary>
        [JsonPropertyName("created_by")]
        public string CreatedBy { get; set; }


        /// <summary>
        /// The Uid of folder in which the asset is present
        /// </summary>
        [JsonPropertyName("parent_uid")]
        public string ParentUid { get; set; }

        /// <summary>
        /// The Version of Asset
        /// </summary>
        [JsonPropertyName("_version")]
        public string Version { get; set; }

        /// <summary>
        /// Dimension Object of the asset containing Height and width
        /// </summary>
        [JsonPropertyName("dimension")]
        public Dictionary<string, object> Dimension { get; set; }

        /// <summary>
        /// Dimension Object of the asset containing Height and width
        /// </summary>
        [JsonPropertyName("publish_details")]
        public Dictionary<string, object> PublishDetails { get; set; }

        #region Internal Constructors
        internal Asset(ContentstackClient stack, string uid)
        {
            this.StackInstance = stack;
            this.Uid = uid;
            this._StackHeaders = stack._LocalHeaders;
        }

        internal Asset(ContentstackClient stack)
        {
            this.StackInstance = stack;
            this._StackHeaders = stack._LocalHeaders;
        }

        internal Asset()
        {

        }
        #endregion

        /// <summary>
        /// To set headers for Backend rest calls.
        /// </summary>
        /// <param name="key">header name.</param>
        /// <param name="value">header value against given header name.</param>
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
        /// Include fallback locale publish content, if specified locale content is not publish.
        /// </summary>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Asset asset = stack.Asset(&quot;asset_uid&quot;);
        ///     asset.IncludeFallback();
        ///     asset.Fetch&lt;Product&gt;().ContinueWith((assetResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Asset IncludeFallback()
        {
            this.UrlQueries.Add("include_fallback", "true");
            return this;
        }



        /// <summary>
        /// This call includes metadata in the response.
        /// </summary>
        /// <returns>Current instance of Asset, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Asset asset = stack.Asset(&quot;asset_uid&quot;);
        ///     asset.IncludeMetadata();
        ///     asset.Fetch&lt;Product&gt;().ContinueWith((assetResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Asset IncludeMetadata()
        {
            try
            {
                this.UrlQueries.Add("include_metadata", "true");
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
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Asset asset = stack.Asset(&quot;asset_uid&quot;);
        ///     asset.IncludeBranch();
        ///     asset.Fetch&lt;Product&gt;().ContinueWith((assetResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Asset IncludeBranch()
        {
            this.UrlQueries.Add("include_branch", "true");
            return this;
        }


        /// <summary>
        /// Add param in URL query.
        /// </summary>
        /// <returns>Current instance of Asset, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Asset asset = stack.Asset(&quot;asset_uid&quot;);
        ///     asset.AddParam("include_branch", "true");
        ///     asset.Fetch&lt;Product&gt;().ContinueWith((assetResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Asset AddParam(string key, string value)
        {
            this.UrlQueries.Add(key, value);
            return this;
        }

        public void RemoveHeader(string key)
        {
            if (this._Headers.ContainsKey(key))
                this._Headers.Remove(key);

        }

        internal void ParseObject(JsonElement jsonObj)
        {
            this._ObjectAttributes = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonObj);
        }

        public DateTime GetCreateAt()
        {

            try
            {
                String value = _ObjectAttributes["created_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
            }
            return DateTime.MinValue;
        }

        public Object Get(String key)
        {
            try
            {
                if (_ObjectAttributes.ContainsKey(key))
                {
                    var value = _ObjectAttributes[key];
                    return value;
                }
                else
                    return null;
            }
            catch
            {
            }
            return null;
        }

        public String GetCreatedBy()
        {

            return _ObjectAttributes["created_by"].ToString();
        }

        public DateTime GetUpdateAt()
        {

            try
            {
                String value = _ObjectAttributes["updated_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
            }
            return DateTime.MinValue;
        }

        public String GetUpdatedBy()
        {

            return _ObjectAttributes["updated_by"].ToString();
        }

        public DateTime GetDeleteAt()
        {

            try
            {
                String value = _ObjectAttributes["deleted_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
            }
            return DateTime.MinValue;
        }

        public String GetDeletedBy()
        {

            return _ObjectAttributes["deleted_by"].ToString();
        }

        public async Task<Asset> Fetch()
        {
            Dictionary<String, Object> headers = GetHeader(_Headers);

            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();

            if (headers != null && headers.Count() > 0)
            {
                foreach (KeyValuePair<string, object> header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }
            }
            mainJson.Add("environment", this.StackInstance.Config.Environment);

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }
            try
            {
                HttpRequestHandler RequestHandler = new HttpRequestHandler(this.StackInstance);
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson, Branch: this.StackInstance.Config.Branch, timeout: this.StackInstance.Config.Timeout, proxy: this.StackInstance.Config.Proxy);
                JsonObject obj = JsonNode.Parse(ContentstackConvert.ToString(outputResult, "{}")).AsObject();
                // TODO: the serializer earlier taken was this.StackInstance.Serializer
                return JsonSerializer.Deserialize<Asset>(obj["$.asset"].ToJsonString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
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
                    foreach (KeyValuePair<string, object> entry in localHeader)
                    {
                        String key = entry.Key;
                        classHeaders.Add(key, entry.Value);
                    }

                    foreach (KeyValuePair<string, object> entry in mainHeader)
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
                    JsonObject data = JsonNode.Parse(errorMessage.Replace("\r\n", "")).AsObject();

                    if (data.TryGetPropertyValue("error_code", out JsonNode token))
                        errorCode = token.GetValue<int>();

                    if (data.TryGetPropertyValue("error_message", out token))
                        errorMessage = token.GetValue<string>();

                    if (data.TryGetPropertyValue("errors", out token))
                        errors = JsonSerializer.Deserialize<Dictionary<string, object>>(token.ToJsonString());

                    var response = exResp as HttpWebResponse;
                    if (response != null)
                    {
                        statusCode = response.StatusCode;
                    }
                }
            }
            catch
            {
                if (errorMessage != null)
                {
                    errorMessage = ex.Message;
                }
            }

            ContentstackException contentstackError = new ContentstackException()
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

