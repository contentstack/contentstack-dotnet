﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Assets refer to all the media files (images, videos, PDFs, audio files, and so on) uploaded in your Contentstack repository for future use
    /// </summary>
    public class Asset
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
                Config config = this.StackInstance.config;
                return String.Format("{0}/assets/{1}", config.BaseUrl, this.Uid);
            }
        }
        #endregion
        public ContentstackClient StackInstance
        {
            get;
            set;
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
            }set
            {
                this["url"] = value;
            }
        }

        /// <summary>
        /// This is Entry Uid of an entry.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        [JsonProperty(PropertyName = "file_size")]
        public string FileSize { get; set; }

        /// <summary>
        /// The original name of the file.
        /// </summary>
        public string FileName { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Set array of Tags
        /// </summary>
        public Object[] Tags { get; set; }

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

        public void RemoveHeader(string key)
        {
            if (this._Headers.ContainsKey(key))
                this._Headers.Remove(key);

        }

        internal void ParseObject(JObject jsonObj)
        {
            this._ObjectAttributes = jsonObj.ToObject<Dictionary<string, object>>();
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

            //Dictionary<string, object> urlQueries = new Dictionary<string, object>();

            if (headers != null && headers.Count() > 0)
            {
                foreach (KeyValuePair<string, object> header in headers)
                {
                    headerAll.Add(header.Key, (String)header.Value);
                }

                if (headers.ContainsKey("environment"))
                {
                    UrlQueries.Add("environment", headers["environment"]);
                }
                else if (headers.ContainsKey("environment_uid"))
                {
                    UrlQueries.Add("environment_uid", headers["environment_uid"]);
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
                JObject obj = JObject.Parse(ContentstackConvert.ToString(outputResult, "{}"));
                return obj.SelectToken("$.asset").ToObject<Asset>(this.StackInstance.Serializer);
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
        internal static ContentstackError GetContentstackError(Exception ex)
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

            ContentstackError contentstackError = new ContentstackError()
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

