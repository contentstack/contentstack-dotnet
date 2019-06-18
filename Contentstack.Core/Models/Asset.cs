using System;
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
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();

        private string _Url
        {
            get
            {
                Config config = this.Stack.config;
                return String.Format("{0}/assets/{1}", config.BaseUrl,this.Uid);
            }
        }

        #endregion
        public ContentstackClient Stack
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
                catch
                { }
            }
        }

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

        public string Uid
        {
            get
            {
                string uid = string.Empty;

                if (this._ObjectAttributes.ContainsKey("uid"))
                {
                    uid = ContentstackConvert.ToString(this["uid"]);
                }

                return uid;
            }
            set
            {
                this["uid"] = value;
            }
        }

        public string FileSize
        {
            get
            {
                string fileSize = string.Empty;

                if (this._ObjectAttributes.ContainsKey("file_size"))
                {
                    fileSize = ContentstackConvert.ToString(this["file_size"]);
                }

                return fileSize;
            }
            set
            {
                this["file_size"] = value;
            }
        }

        public string FileName
        {
            get
            {
                string fileName = string.Empty;

                if (this._ObjectAttributes.ContainsKey("filename"))
                {
                    fileName = ContentstackConvert.ToString(this["filename"]);
                }

                return fileName;
            }
            set
            {
                this["filename"] = value;
            }
        }

        public Object[] Tags
        {
            get; set;
        }
        public int Count { get; set; }
        public int TotalCount { get; set; }

        #region Internal Constructors
        internal Asset(ContentstackClient stack, string uid)
        {
            this.Stack = stack;
            this.Uid = uid;
            this._StackHeaders = stack._LocalHeaders;
        }

        internal Asset(ContentstackClient stack)
        {
            this.Stack = stack;
            this._StackHeaders = stack._LocalHeaders;
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

        internal Asset ParseObject(Dictionary<string, object> jsonObj)
        {
            if (jsonObj.ContainsKey("upload"))
            {
                this._ObjectAttributes = (Dictionary<string, object>)jsonObj["upload"];
            }else
            {
                this._ObjectAttributes = jsonObj;
            }

            this.Uid = _ObjectAttributes["uid"].ToString();
            this.FileSize = _ObjectAttributes["file_size"].ToString();
            this.FileName = _ObjectAttributes["filename"].ToString();
            this.Url = _ObjectAttributes["url"].ToString();

            if (_ObjectAttributes["tags"] is Array)
            {
                if ((_ObjectAttributes.ContainsKey("tags")) && (_ObjectAttributes["tags"] != null) && (!(_ObjectAttributes.ContainsKey("tags").Equals(string.Empty))))
                {
                    var array = jsonObj["tags"];
                    //_TagsArray =(string[])array;
                    if (array != null)
                    {
                        Tags = (object[])array;
                    }
                }
            }

            if (_ObjectAttributes.ContainsKey("count"))
            {
                Count = (int)_ObjectAttributes["count"];
            }
            if (_ObjectAttributes.ContainsKey("objects"))
            {
                TotalCount = (int)_ObjectAttributes["objects"];
            }
            return this;
        }

        public DateTime GetCreateAt()
        {

            try
            {
                String value = _ObjectAttributes["created_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch
            {
                //CSAppUtils.showLog(TAG, "-----------------getCreateAtDate|" + e);
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
            catch 
            {
                //CSAppUtils.showLog(TAG, "-----------------getUpdateAtDate|" + e);
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
            catch
            {
                // CSAppUtils.showLog(TAG, "-----------------getDeleteAt|" + e);
            }
            return DateTime.MinValue;
        }

        public String GetDeletedBy()
        {

            return _ObjectAttributes["deleted_by"].ToString();
        }

        public async Task Fetch()
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
                    mainJson.Add("environment", this.Stack.config.Environment);
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
                StackOutput stackOutput = new StackOutput(ContentstackConvert.ToString(outputResult, "{}"));
                ParseObject((Dictionary<string, object>)stackOutput.Object);
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

