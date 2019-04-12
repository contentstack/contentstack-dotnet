using System;
using System.Collections.Generic;
using System.Linq;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal class Asset
    {
        #region Internal & Private Properties
        private Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        private Dictionary<string, string> _Headers = new Dictionary<string, string>();
        private string _jsonString = String.Empty;
        private string _Url = string.Empty;
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

        public string UploadUrl
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
                this._Url = value;
            }
        }

        public string UploadUid
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
        public string ContentType
        {
            get
            {
                string contentType = string.Empty;

                if (this._ObjectAttributes.ContainsKey("content_type"))
                {
                    contentType = ContentstackConvert.ToString(this["content_type"]);
                }

                return contentType;
            }
            set
            {
                this["content_type"] = value;
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

        public List<string> Tags
        {
            get; set;
        }
        public int Count { get; set; }
        public int TotalCount { get; set; }
        #region Internal Constructors
        internal Asset(ContentstackClient stack)
        {
            this.Stack = stack;
        }

        //internal Asset(Stack stack, string uid)
        //{
        //    this.Stack = stack;
        //    this["uid"] = uid;
        //}
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
            this._ObjectAttributes = (Dictionary<string, object>)jsonObj["upload"];

            this.UploadUid = _ObjectAttributes["uid"].ToString();
            this.ContentType = _ObjectAttributes["content_type"].ToString();
            this.FileSize = _ObjectAttributes["file_size"].ToString();
            this.FileName = _ObjectAttributes["filename"].ToString();
            this.UploadUrl = _ObjectAttributes["url"].ToString();

            if (_ObjectAttributes["tags"] is Array)
            {
                if ((_ObjectAttributes.ContainsKey("tags")) && (_ObjectAttributes["tags"] != null) && (!(_ObjectAttributes.ContainsKey("tags").Equals(string.Empty))))
                {
                    List<string> tagsArray = (List<string>)_ObjectAttributes["tags"];
                    if (tagsArray.Count() > 0)
                    {
                        int count = tagsArray.Count();
                        Tags = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            //Tags[i] = (String)tagsArray[i];
                            Tags.Add((String)tagsArray[i]);
                        }
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

        public Asset SetTags(String[] tags)
        {
            if (Tags == null)
            {
                Tags = new List<string>();
            }
            if (tags != null)
            {
                foreach (String tag in tags)
                {
                    Tags.Add(tag);
                }
            }
            return this;
        }


    }
}

