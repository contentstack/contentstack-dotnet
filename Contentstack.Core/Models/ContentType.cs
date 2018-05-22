using System;
using System.Collections.Generic;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// ContentType provides Entry and Query instance.
    /// </summary>
    public class ContentType
    {
        #region Public Properties
        internal Stack StackInstance { get; set; }
        internal string Uid { get; set; }
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
        internal void SetStackInstance(Stack stack)
        {
            this.StackInstance = stack;
            this._StackHeaders = stack._LocalHeaders;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// To set headers for Built.io Contentstack rest calls.
        /// </summary>
        /// <param name="key">header name.</param>
        /// <param name="value">header value against given header name.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        ///     Entry entry = contentType.Entry(&quot;bltf4fbbc94e8c851db&quot;);
        /// </code>
        /// </example>
        public Entry Entry(String entryUid)
        {
            Entry entry = new Entry(ContentTypeName);
            entry._FormHeaders = GetHeader(_Headers);
            entry.SetContentTypeInstance(this);
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
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
