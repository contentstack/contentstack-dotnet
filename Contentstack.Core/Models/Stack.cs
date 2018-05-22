using System;
using System.Collections.Generic;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// To fetch stack level information of your application from Built.io Contentstack server.
    /// </summary>
    public class Stack
    {
        #region Internal Variables

        internal string StackApiKey
        {
            get;
            set;
        }


        internal string Version { get; set; }

        internal ContentstackConstants _Constants { get; set; }
        internal Dictionary<string, object> _LocalHeaders = new Dictionary<string, object>();
        internal Config config;
        #endregion

        #region Private Constructor
        private Stack() { }
        #endregion

        #region Internal Constructor
        internal Stack(String stackApiKey)
        {
            this.StackApiKey = stackApiKey;
            this._LocalHeaders = new Dictionary<string, object>();

        }
        #endregion

        #region Internal Functions
        internal void SetConfig(Config config)
        {
            this.config = config;

        }
        #endregion

        #region Public Functions

        /// <summary>
        /// Represents a ContentType. Creates ContenntType Instance.
        /// </summary>
        /// <param name="contentTypeName">ContentType name.</param>
        /// <returns>Current instance of ContentType, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        /// </code>
        /// </example>
        public ContentType ContentType(String contentTypeName)
        {
            ContentType contentType = new ContentType(contentTypeName);
            contentType.SetStackInstance(this);

            return contentType;
        }
       
        /// <summary>
        /// Represents a Asset. Creates Asset Instance.
        /// </summary>
        /// <returns>Current instance of Asset, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Asset asset  = asset.Asset();
        /// </code>
        /// </example>
        internal Asset Asset()
        {
            Asset asset = new Asset(this);
            return asset;
        }
       
        /// <summary>
        /// Represents a AssetLibrary. Creates AssetLibrary Instance.
        /// </summary>
        /// <returns>Current instance of Asset, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     AssetLibrary assetLibrary  = asset.AssetLibrary();
        /// </code>
        /// </example>
        internal AssetLibrary AssetLibrary()
        {
            AssetLibrary asset = new AssetLibrary();
            return asset;
        }

        /// <summary>
        /// Get version.
        /// </summary>
        /// <returns>Version</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     String url = stack.GetVersion();
        /// </code>
        /// </example>
        public string GetVersion()
        {
            return Version;
        }

        /// <summary>
        /// Get stack application key
        /// </summary>
        /// <returns>stack application key</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     String url = stack.GetApplicationKey();
        /// </code>
        /// </example>
        public string GetApplicationKey()
        {
            return StackApiKey;
        }

        /// <summary>
        /// Get stack access token
        /// </summary>
        /// <returns>access token</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     String accessToken = stack.GetAccessToken();
        /// </code>
        /// </example>
        public string GetAccessToken()
        {
            return _LocalHeaders != null ? _LocalHeaders["access_token"].ToString() : null;
        }

        /// <summary>
        /// Get stack environment
        /// </summary>
        /// <returns>stack environment</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     String environment = stack.GetEnvironment();
        /// </code>
        /// </example>
        public string GetEnvironment()
        {
            return _LocalHeaders != null & _LocalHeaders.ContainsKey("environment") ? _LocalHeaders["environment"].ToString() : (_LocalHeaders != null & _LocalHeaders.ContainsKey("environment_uid")) ? _LocalHeaders["environment_uid"].ToString() : null;
        }

        ///// <summary>
        ///// Get whether environment or environment uid.
        ///// </summary>
        ///// <returns>true if environment id is present</returns>
        ///// <example>
        ///// <code>
        /////     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        /////     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        /////     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        /////     bool isEnvironmentUid = stack.IsEnvironmentUid();
        ///// </code>
        ///// </example>
        //public bool IsEnvironmentUid()
        //{
        //    if (_LocalHeaders != null & _LocalHeaders.ContainsKey("environment"))
        //    {
        //        return false;
        //    }
        //    else if (_LocalHeaders != null & _LocalHeaders.ContainsKey("environment_uid"))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Remove header key.
        /// </summary>
        /// <param name="key">key to be remove from header</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     stack.RemoveHeader(&quot;custom_header_key&quot;);
        /// </code>
        /// </example>
        public void RemoveHeader(string key)
        {
            if (this._LocalHeaders.ContainsKey(key))
                this._LocalHeaders.Remove(key);

        }

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
        ///     stack.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
        /// </code>
        /// </example>
        public void SetHeader(string key, string value)
        {
            if (key != null & value != null)
            {
                if (this._LocalHeaders.ContainsKey(key))
                    this._LocalHeaders.Remove(key);
                this._LocalHeaders.Add(key, value);
            }

        }

        ///// <summary>
        ///// set environment.
        ///// </summary>
        ///// <param name="environment">environment uid/name</param>
        ///// <param name="isEnvironmentUid"> true - If environment uid is present
        /////                                 false - If environment uid is not present</param>
        ///// <example>
        ///// <code>
        /////     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        /////     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        /////     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        /////     stack.SetEnvironment(&quot;stag&quot;, false);
        ///// </code>
        ///// </example>
        //public void SetEnvironment(string environment, bool isEnvironmentUid)
        //{
        //    if (!string.IsNullOrEmpty(environment))
        //    {
        //        if (isEnvironmentUid)
        //        {
        //            RemoveHeader("environment");
        //            SetHeader("environment_uid", environment);
        //        }
        //        else
        //        {
        //            RemoveHeader("environment_uid");
        //            SetHeader("environment", environment);
        //        }
        //    }
        //}

        #endregion

        #region Private Functions
        private Dictionary<string, object> GetHeader()
        {

            Dictionary<string, object> mainHeader = _LocalHeaders;

            return mainHeader;
        }
        #endregion

    }
}
