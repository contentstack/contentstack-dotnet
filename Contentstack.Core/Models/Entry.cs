using Markdig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Entry is used to create, update and delete contentType&#39;s entries on the Contentstack.io Content stack.
    /// </summary>
    public class Entry
    {
        #region Private Variables

        private string[] _TagsArray = new string[] { };
        private ContentType ContentTypeInstance { get; set; }
        private CachePolicy _CachePolicy;
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();
        private string _Url;
        private bool _IsCachePolicySet;
        #endregion

        #region Internal Variables
        internal Dictionary<string, object> _FormHeaders = new Dictionary<string, object>();
        internal Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        internal Dictionary<string, object> ObjectValueJson = new Dictionary<string, object>();
        internal Dictionary<string, object> _Headers = new Dictionary<string, object>();

        internal Dictionary<string, object> _metadata = new Dictionary<string, object>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Title of an entry
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Title;
        ///     });
        /// </code>
        /// </example>
        public string Title { get; set; }

        /// <summary>
        /// This is Entry Uid of an entry.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.EntryUid;
        ///     });
        /// </code>
        /// </example>
        public string EntryUid { get; set; }

        /// <summary>
        /// Rest Url for an Entry on contentstack.io
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Url;
        ///     });
        /// </code>
        /// </example>
        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(this._Url))
                {
                    Config config = this.ContentTypeInstance.StackInstance.config;

                    if (!String.IsNullOrEmpty(this.EntryUid))
                        return String.Format("{0}/content_types/{1}/entries/{2}", config.BaseUrl, this.ContentTypeInstance.ContentTypeName, this.EntryUid);
                    else
                        return String.Format("{0}/content_types/{1}/entries", config.BaseUrl, this.ContentTypeInstance.ContentTypeName);
                }
                else
                {
                    if (!String.IsNullOrEmpty(this.EntryUid) && !this._Url.Contains(this.EntryUid))
                        this._Url += "/" + this.EntryUid;

                    return this._Url;
                }
            }
            set
            {
                this._Url = value;
            }
        }

        /// <summary>
        /// Set array of Tags
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Tags;
        ///     });
        /// </code>
        /// </example>
        public object[] Tags { get; set; }


        /// <summary>
        /// owner information if the object has owner.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Owner;
        ///     });
        /// </code>
        /// </example>
        private Dictionary<string, object> Owner { get; set; }


        /// <summary>
        /// Set key/value attributes of Metadata.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Metadata;
        ///     });
        /// </code>
        /// </example>
        public Dictionary<string, object> Metadata { get; set; }


        /// <summary>
        /// Owner Email if the entry has owner.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.OwnerEmailId;
        ///     });
        /// </code>
        /// </example>
        private string OwnerEmailId { get; set; }



        /// <summary>
        /// Owner uid if the entry has owner.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.OwnerUid;
        ///     });
        /// </code>
        /// </example>
        private string OwnerUid { get; set; }


        /// <summary>
        /// Set key/value attributes of an current entry instance.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Object;
        ///     });
        /// </code>
        /// </example>
        public Dictionary<string, object> Object
        {
            get
            {
                return this._ObjectAttributes;
            }
            set
            {
                this._ObjectAttributes = value;
            }
        }
        #endregion

        #region Internal Constructors
        internal Entry()
        {
        }
        internal Entry(Dictionary<string, object> objectKeyPair)
        {
            this._ObjectAttributes = objectKeyPair;
        }
        internal Entry(string contentTypeName)
        {

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
        internal void SetContentTypeInstance(ContentType contentTypeInstance)
        {
            this.ContentTypeInstance = contentTypeInstance;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Returns tags of this entry.
        /// </summary>
        /// <returns>Array of tags.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetTags();
        ///     });
        /// </code>
        /// </example>
        public object[] GetTags()
        {
            object[] result = null;
            try
            {
                if (this._ObjectAttributes.ContainsKey("tags"))
                {
                    try
                    {
                        result = (object[])_ObjectAttributes["tags"];
                    }
                    catch
                    { }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Assign a tag(s) for this Entry.
        /// </summary>
        /// <param name="tags">Collection of tags.</param>
        public void SetTags(String[] tags)
        {
            this.Tags = tags;
        }

        /// <summary>
        /// Assigns a uid to current instance of an entry.
        /// </summary>
        /// <param name="uid">Uid of an Entry</param>
        public void SetUid(String uid)
        {
            this.EntryUid = uid;
        }

        /// <summary>
        /// To set cache policy using Entry instance.
        /// </summary>
        /// <param name="cachePolicy">CachePolicy instance</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetCachePolicy(CachePolicy.NetworkElseCache);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry SetCachePolicy(CachePolicy cachePolicy)
        {
            this._CachePolicy = cachePolicy;
            this._IsCachePolicySet = true;
            return this;
        }

        /// <summary>
        /// Returns Rest Url for an Entry on contentstack.io
        /// </summary>
        /// <returns>url in string</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetURL();
        ///     });
        /// </code>
        /// </example>
        public string GetURL()
        {
            return Url;
        }

        /// <summary>
        /// Get title 
        /// </summary>
        /// <returns>title</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetTitle();
        ///     });
        /// </code>
        /// </example>
        public string GetTitle()
        {
            return Title;
        }

        /// <summary>
        /// Get contentType name.
        /// </summary>
        /// <returns>contentType name</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetContentType();
        ///     });
        /// </code>
        /// </example>
        public String GetContentType()
        {
            return this.ContentTypeInstance.ContentTypeName;
        }

        /// <summary>
        /// Get uid
        /// </summary>
        /// <returns>Uid</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetUid();
        ///     });
        /// </code>
        /// </example>
        public String GetUid()
        {
            return EntryUid;
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
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetHeader(&quot;custom_key&quot;, &quot;custom_value&quot;);
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
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.RemoveHeader(&quot;custom_key&quot;);
        /// </code>
        /// </example>
        public void RemoveHeader(string key)
        {
            if (this._Headers.ContainsKey(key))
                this._Headers.Remove(key);

        }

        /// <summary>
        /// Get metadata of entry.
        /// </summary>
        /// <returns>key/value attributes of metadata</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetMetadata();
        ///     });
        /// </code>
        /// </example>
        public Dictionary<string, Object> GetMetadata()
        {
            return Metadata;
        }


        private Language GetLanguage()
        {
            if (Metadata != null && Metadata.Count > 0)
            {
                String localeCode = (String)Metadata["locale"];
                localeCode = localeCode.Replace("-", "_");
                LanguageCode codeValue = (LanguageCode)Enum.Parse(typeof(LanguageCode), localeCode);
                int localeValue = (int)codeValue;
                Language[] language = Enum.GetValues(typeof(Language)).Cast<Language>().ToArray();

                return language[localeValue];
            }

            return new Language();
        }


        /// <summary>
        /// Get html text for markdown data type
        /// </summary>
        /// <param name="markdownKey">field_uid as key.</param>
        /// <returns>html text in string format.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetHTMLText(&quot;markdownKey&quot;)
        ///     });
        /// </code>
        /// </example>
        public String GetHTMLText(string markdownKey)
        {
            string result = string.Empty;
            if (this._ObjectAttributes.ContainsKey(markdownKey))
            {
                try
                {
                    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    result = Markdown.ToHtml(this._ObjectAttributes[markdownKey].ToString(), pipeline);
                    return result;
                }
                catch
                { }
            }
            return result;
        }

        /// <summary>
        /// Get html text for markdown data type which is multiple true
        /// </summary>
        /// <param name="markdownKey">field_uid as key.</param>
        /// <returns>html text in string format.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetMultipleHTMLText(&quot;markdownKey&quot;)
        ///     });
        /// </code>
        /// </example>
        public List<String> GetMultipleHTMLText(string markdownKey)
        {
            List<string> result = new List<string>();
            if (this._ObjectAttributes.ContainsKey(markdownKey))
            {
                try
                {
                    object[] jsonArray = (object[])this._ObjectAttributes[markdownKey];
                    foreach (var value in jsonArray)
                    {
                        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                        // var result = Markdown.ToHtml(value.ToString(), pipeline);
                        result.Add(Markdown.ToHtml(value.ToString(), pipeline));

                    }
                    return result;
                }
                catch (Exception ex)
                {
                }
            }
            return result;
        }


        /// <summary>
        /// Returns owner information if the object has owner.
        /// </summary>
        /// <returns>Get key/value pairs for owner details.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetOwner();
        ///     });
        /// </code>
        /// </example>
        private Dictionary<string, Object> GetOwner()
        {
            return Owner;
        }


        /// <summary>
        /// Get key/value pairs in json of current instance.
        /// </summary>
        /// <returns>json in string format</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.ToJson();
        ///     });
        /// </code>
        /// </example>
        public string ToJson()
        {
            string tempJson = "{}";
            try
            {
                Dictionary<string, object> tempObject = new Dictionary<string, object>(this._ObjectAttributes);

                tempJson = JsonConvert.SerializeObject(tempObject, ContentstackConvert.JsonSerializerSettings);
            }
            catch
            { }

            return tempJson;
        }

        /// <summary>
        /// Get object value for key.
        /// </summary>
        /// <param name="key">key to get value</param>
        /// <returns>object value</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Get(&quot;key&quot;);
        ///     });
        /// </code>
        /// </example>
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
            catch (Exception e)
            {
                //CSAppUtils.showLog(TAG, "-----------------getUpdateAtDate|" + e);
            }
            return null;
        }

        /// <summary>
        /// Get value of creation time of entry.
        /// </summary>
        /// <returns>created date time in datetime format</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetCreateAt();
        ///     });
        /// </code>
        /// </example>
        public DateTime GetCreateAt()
        {

            try
            {
                String value = _ObjectAttributes["created_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch (Exception e)
            {
                //CSAppUtils.showLog(TAG, "-----------------getCreateAtDate|" + e);
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Get uid who created this entry.
        /// </summary>
        /// <returns>uid who created this entry</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetCreatedBy();
        ///     });
        /// </code>
        /// </example>
        public string GetCreatedBy()
        {

            string result = null;
            try
            {
                if (this._ObjectAttributes.ContainsKey("created_by"))
                {
                    try
                    {
                        result = _ObjectAttributes["created_by"].ToString();
                    }
                    catch
                    { }
                }
            }
            catch { }
            return result;
        }


        private Entry IncludeCreatedBy()
        {
            try
            {
                UrlQueries.Add("include_created_by", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Get value of updating time of entry.
        /// </summary>
        /// <returns>updated date time in datetime format</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetUpdateAt();
        ///     });
        /// </code>
        /// </example>
        public DateTime GetUpdateAt()
        {

            try
            {
                String value = _ObjectAttributes["updated_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch (Exception e)
            {
                //CSAppUtils.showLog(TAG, "-----------------getUpdateAtDate|" + e);
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Get uid who updated this entry.
        /// </summary>
        /// <returns>uid who updated this entry</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetUpdatedBy();
        ///     });
        /// </code>
        /// </example>
        public string GetUpdatedBy()
        {

            string result = string.Empty;
            try
            {
                if (this._ObjectAttributes.ContainsKey("updated_by"))
                {
                    try
                    {
                        result = _ObjectAttributes["updated_by"].ToString();
                    }
                    catch
                    { }
                }
            }
            catch { }
            return result;
        }

        private Entry IncludeUpdatedBy()
        {

            try
            {
                UrlQueries.Add("include_updated_by", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// Get value of deleting time of entry.
        /// </summary>
        /// <returns>deleted date time in datetime format</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetDeleteAt();
        ///     });
        /// </code>
        /// </example>
        public DateTime GetDeleteAt()
        {

            try
            {
                String value = _ObjectAttributes["deleted_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch (Exception e)
            {
                // CSAppUtils.showLog(TAG, "-----------------getDeleteAt|" + e);
            }
            return DateTime.MinValue;
        }
        /// <summary>
        /// Get uid who deleted this entry.
        /// </summary>
        /// <returns>uid who deleted this entry</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetDeletedBy();
        ///     });
        /// </code>
        /// </example>
        public String GetDeletedBy()
        {

            return _ObjectAttributes["deleted_by"].ToString();
        }

        /// <summary>
        /// Get an asset from the entry
        /// </summary>
        /// <param name="key">field_uid as key.</param>
        /// <returns>Asset instance</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetAsset(&quot;field_uid&quot;);
        ///     });
        /// </code>
        /// </example>
        private Asset GetAsset(String key)
        {

            Dictionary<string, object> assetObject = (Dictionary<string, object>)_ObjectAttributes[key];
            Asset asset = ContentTypeInstance.StackInstance.Asset().ParseObject(assetObject);

            return asset;
        }

        /// <summary>
        /// Get an assets from the entry. This works with multiple true fields
        /// </summary>
        /// <param name="key">field_uid as key.</param>
        /// <returns>List of Asset instance</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetAssets(&quot;field_uid&quot;);
        ///     });
        /// </code>
        /// </example>
        /// 
        private List<Asset> GetAssets(String key)
        {
            List<Asset> assets = new List<Asset>();
            Dictionary<string, object> assetArray = (Dictionary<string, object>)_ObjectAttributes[key];

            for (int i = 0; i < assetArray.Count(); i++)
            {

                if (assetArray is Dictionary<string, object>)
                {
                    Asset asset = ContentTypeInstance.StackInstance.Asset().ParseObject(assetArray);
                    assets.Add(asset);
                }

            }
            return assets;
        }


        /// <summary>
        /// Specifies list of field uids that would be excluded from the response.
        /// </summary>
        /// <param name="fieldUid">field uid  which get excluded from the response.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Except(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry Except(String[] fieldUid)
        {
            try
            {
                if (fieldUid != null && fieldUid.Length > 0)
                {

                    int count = fieldUid.Length;
                    //for (int i = 0; i < count; i++) {
                    //    UrlQueries.Add("except[BASE][]", fieldUid[i]);
                    //}
                    UrlQueries.Add("except[BASE][]", fieldUid);
                    //exceptValueJson.Add("BASE", objectUidForExcept);

                }
            }
            catch (Exception e)
            {
                //CSAppUtils.showLog(TAG, "--include Reference-catch|" + e);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint that requires a particular reference key details.
        /// </summary>
        /// <param name="referenceField">key that to be constrained.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeReference(&quot;name&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry IncludeReference(String referenceField)
        {
            try
            {
                if (referenceField != null && referenceField.Length > 0)
                {
                    UrlQueries.Add("include[]", referenceField);
                }
                return this;
            } catch (Exception e) {
                //CSAppUtils.showLog(TAG, "--include Reference-catch|" + e);
            }

            return this;
        }

        /// <summary>
        /// Add a constraint that requires a particular reference key details.
        /// </summary>
        /// <param name="referenceFields">array key that to be constrained.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeReference(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry IncludeReference(String[] referenceFields)
        {
            try
            {
                if (referenceFields != null && referenceFields.Length > 0)
                {
                    UrlQueries.Add("include[]", referenceFields);
                }
                return this;
            }
            catch (Exception e)
            {
                //CSAppUtils.showLog(TAG, "--include Reference-catch|" + e);
            }

            return this;
        }

        /// <summary>
        /// Specifies an array of only keys that would be included in the response.
        /// </summary>
        /// <param name="keys">Array of the only reference keys to be included in response.</param>
        /// <param name="referenceKey">Key who has reference to some other class object.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeOnlyReference(new String[]{&quot;name&quot;, &quot;description&quot;}, &quot;referenceUid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry IncludeOnlyReference(string[] keys, string referenceKey)
        {
            List<string> objectUidForOnly = new List<string>();
            Dictionary<string, object> onlyValueJson = new Dictionary<string, object>();
            if (keys != null && keys.Length > 0)
            {

                int count = keys.Length;
                for (int i = 0; i < count; i++)
                {
                    objectUidForOnly.Add(keys[i]);
                }
                onlyValueJson.Add(referenceKey, objectUidForOnly);
                UrlQueries.Add("include", new object[] { referenceKey });
                UrlQueries.Add("only", onlyValueJson);

            }

            return this;
        }


        /// <summary>
        /// Include schemas of all returned objects along with objects themselves.
        /// </summary>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeSchema();
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        private Entry IncludeSchema()
        {
            try
            {
                UrlQueries.Add("include_schema", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        private Entry IncludeOwner()
        {
            try
            {
                UrlQueries.Add("include_owner", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }


        /// <summary>
        /// Specifies an array of except keys that would be excluded in the response.
        /// </summary>
        /// <param name="keys">Array of the except reference keys to be excluded in response.</param>
        /// <param name="referenceKey">Key who has reference to some other class object.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeExceptReference(new String[]{&quot;name&quot;, &quot;description&quot;},&quot;referenceUid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry IncludeExceptReference(string[] keys, string referenceKey)
        {
            List<string> objectUidForOnly = new List<string>();
            Dictionary<string, object> onlyValueJson = new Dictionary<string, object>();
            if (keys != null && keys.Length > 0)
            {

                int count = keys.Length;
                for (int i = 0; i < count; i++)
                {
                    objectUidForOnly.Add(keys[i]);
                }
                onlyValueJson.Add(referenceKey, objectUidForOnly);
                UrlQueries.Add("include", new object[] { referenceKey });
                UrlQueries.Add("except", onlyValueJson);

            }

            return this;
        }

        /// <summary>
        /// Specifies an array ofonly keys in BASE object that would be included in the response.
        /// </summary>
        /// <param name="fieldUid">Array of the only reference keys to be included in response.</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Only(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry Only(String[] fieldUid)
        {
            try
            {
                List<string> objectUidForOnly = new List<string>();
                Dictionary<string, object> onlyValueJson = new Dictionary<string, object>();
                if (fieldUid != null && fieldUid.Length > 0)
                {
                    UrlQueries.Add("only[BASE][]", fieldUid);
                }
            }
            catch (Exception e)
            {
                //CSAppUtils.showLog(TAG, "--include Reference-catch|" + e);
            }

            return this;
        }



        /// <summary>
        /// Fetches the latest version of the entries from Contentstack.io content stack
        /// </summary>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = Contentstack.Stack(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_name&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public async Task<Entry> Fetch()
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

                    mainJson.Add("environment", this.ContentTypeInstance.StackInstance.config.Environment);
                }
            }

            foreach (var kvp in UrlQueries) {
                mainJson.Add(kvp.Key, kvp.Value);
            }


            //foreach (var value in mainJson)
            //{
            //    if (value.Value is Dictionary<string, object>)
            //    {
            //        Url = Url + "&" + value.Key + "=" + JsonConvert.SerializeObject(value.Value);
            //    } 
            //    else
            //    {
            //        Url = Url + "&" + value.Key + "=" + value.Value;
            //    }
            //}

            String queryParam = String.Join("&",mainJson.Select(kvp =>{
                var value = "";
                if(kvp.Value is string[]) {
                    string[] vals = (string[])kvp.Value;
                    //Array<string> val = (Array<string>)kvp.Value;
                    value = String.Join("&", vals.Select(item => {
                        return String.Format("{0}={1}", kvp.Key, item);
                    }));
                    return value;           
                } else if (kvp.Value is Dictionary<string, object>)
                    value = JsonConvert.SerializeObject(kvp.Value);
                else 
                    value = (string)kvp.Value;
                
                return String.Format("{0}={1}", kvp.Key, value);

            }));

            Url = Url + "?" + queryParam;

            //mainJson.Add("query", UrlQueries);

            //mainJson.Add("_method", HttpMethods.Get.ToString().ToUpper());

            try
            {
                //String mainStringForMD5 = Url + JsonConvert.SerializeObject(mainJson) + JsonConvert.SerializeObject(headers);
                String mainStringForMD5 = Url + JsonConvert.SerializeObject(mainJson) + JsonConvert.SerializeObject(headers);
                String md5Value = ContentstackConvert.GetMD5FromString(mainStringForMD5.Trim());

                string CacheFilePath = Path.Combine(ContentstackConstants.Instance.CacheFolderName, md5Value);
                CachePolicy cachePolicy = CachePolicy.NetworkOnly;

                if (_IsCachePolicySet)
                {
                    cachePolicy = _CachePolicy;
                }
                switch (cachePolicy)
                {
                    case CachePolicy.IgnoreCache:

                        HTTPRequestHandler contentstackRequestHandler = new HTTPRequestHandler();
                        var result = await contentstackRequestHandler.ProcessRequest(Url, headers, mainJson);
                        StackOutput contentstackOutput = new StackOutput(ContentstackConvert.ToString(result, "{}"));
                        //Entry resultObject = new Entry();
                        ParseObject((Dictionary<string, object>)contentstackOutput.Object);
                        //Console.WriteLine(contentstackOutput);
                        break;
                    //}

                    case CachePolicy.NetworkOnly:
                        HTTPRequestHandler RequestHandler = new HTTPRequestHandler();
                        var outputResult = await RequestHandler.ProcessRequest(Url, headers, mainJson, CacheFilePath);
                        StackOutput stackOutput = new StackOutput(ContentstackConvert.ToString(outputResult, "{}"));
                        //Entry resultObject = new Entry();
                        ParseObject((Dictionary<string, object>)stackOutput.Object);
                        //await GetOutputAsync(stackOutput);
                        //Console.WriteLine(stackOutput);
                        break;

                    case CachePolicy.CacheOnly:
                        var output_Result = FetchFromCache(CacheFilePath);
                        StackOutput stackOutputResult = new StackOutput(ContentstackConvert.ToString(output_Result, "{}"));
                        ParseObject((Dictionary<string, object>)stackOutputResult.Object);
                        break;

                    case CachePolicy.CacheElseNetwork:
                        if (File.Exists(CacheFilePath))
                        {
                            var cacheResult = FetchFromCache(CacheFilePath);
                            StackOutput stackOutputCacheResult = new StackOutput(ContentstackConvert.ToString(cacheResult, "{}"));
                            Dictionary<string, object> getOutPutdictionary = (Dictionary<string, object>)stackOutputCacheResult.Output;
                            if (ContentstackConvert.GetResponseTimeFromCacheFile(CacheFilePath, (long)getOutPutdictionary["timestamp"], TimeSpan.TicksPerHour * 24))
                            {
                                HTTPRequestHandler requestHandler = new HTTPRequestHandler();
                                var CacheOutputResult = await requestHandler.ProcessRequest(Url, headers, mainJson, CacheFilePath);
                                StackOutput CacheStackOutput = new StackOutput(ContentstackConvert.ToString(CacheOutputResult, "{}"));
                                //Entry resultObject = new Entry();
                                ParseObject((Dictionary<string, object>)CacheStackOutput.Object);
                            }
                            else
                            {
                                ParseObject((Dictionary<string, object>)stackOutputCacheResult.Object);
                            }
                        }
                        else
                        {
                            HTTPRequestHandler request_handler = new HTTPRequestHandler();
                            var output_result = await request_handler.ProcessRequest(Url, headers, mainJson, CacheFilePath);
                            StackOutput stack_output = new StackOutput(ContentstackConvert.ToString(output_result, "{}"));
                            ParseObject((Dictionary<string, object>)stack_output.Object);
                            //Console.WriteLine(stack_output);
                        }
                        break;
                    case CachePolicy.NetworkElseCache:
                        if (ContentstackConvert.IsNetworkAvailable())
                        {
                            HTTPRequestHandler request_handler = new HTTPRequestHandler();
                            var output_result = await request_handler.ProcessRequest(Url, headers, mainJson, CacheFilePath);
                            StackOutput stack_output = new StackOutput(ContentstackConvert.ToString(output_result, "{}"));
                            ParseObject((Dictionary<string, object>)stack_output.Object);
                            //Console.WriteLine(stack_output);
                        }
                        else
                        {
                            var outputresult = FetchFromCache(CacheFilePath);
                            StackOutput stackoutputResult = new StackOutput(ContentstackConvert.ToString(outputresult, "{}"));
                            ParseObject((Dictionary<string, object>)stackoutputResult.Object);
                        }
                        break;

                    case CachePolicy.CacheThenNetwork:
                        if (File.Exists(CacheFilePath))
                        {
                            var outputresult = FetchFromCache(CacheFilePath);
                            StackOutput stackoutputResult = new StackOutput(ContentstackConvert.ToString(outputresult, "{}"));
                            ParseObject((Dictionary<string, object>)stackoutputResult.Object);
                        }
                        HTTPRequestHandler request = new HTTPRequestHandler();
                        var _result = await request.ProcessRequest(Url, headers, mainJson, CacheFilePath);
                        StackOutput _output = new StackOutput(ContentstackConvert.ToString(_result, "{}"));
                        ParseObject((Dictionary<string, object>)_output.Object);
                        //Console.WriteLine(_output);
                        break;

                    default:
                        break;
                }

                //HTTPRequestHandler contentstackRequestHandler = new HTTPRequestHandler();
                //var result = await contentstackRequestHandler.ProcessRequest(Url, headers, mainJson);
                //StackOutput contentstackOutput = new StackOutput(ContentstackConvert.ToString(result, "{}"));
                ////Entry resultObject = new Entry();
                //ParseObject((Dictionary<string, object>)contentstackOutput.Object);
                return this;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
        }

        #endregion


        private Task<Entry> GetOutputAsync(StackOutput output)
        {
            try {
                return Task<Entry>.Run(() =>
                {
                    ParseObject((Dictionary<string, object>)output.Object);
                    return this;
                });
            } catch (Exception ex) {
                throw new ContentstackError(ex);
            }
        }

        #region Private Functions
        private string FetchFromCache(string CacheFileName)
        {
            if (File.Exists(CacheFileName))
            {
                var cacheResult = ContentstackConvert.GetJsonFromCacheFile(CacheFileName);
                //var deserializeHson = JsonConvert.DeserializeObject(cacheResult);
                StackOutput stackOutputCacheResult = new StackOutput(ContentstackConvert.ToString(cacheResult, "{}"));
                Dictionary<string, object> getOutPutdictionary = (Dictionary<string, object>)stackOutputCacheResult.Output;
                if (ContentstackConvert.GetResponseTimeFromCacheFile(CacheFileName, (long)getOutPutdictionary["timestamp"], TimeSpan.TicksPerHour * 24))
                {
                    throw new Exception(StackConstants.ErrorMessage_EntryNotFoundInCache);
                }
                else
                {
                    // await GetOutputAsync(stackOutputCacheResult);
                    return cacheResult;
                }

            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_EntryNotFoundInCache);
            }
        }

        private Dictionary<string, object> GetHeader(Dictionary<string, object> localHeader)
        {
            Dictionary<string, object> mainHeader = _FormHeaders;
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
                return _FormHeaders;
            }
        }

        internal Entry ParseObject(Dictionary<string, object> jsonObj, string url = null)
        {
            Dictionary<string, object> hello = jsonObj;
            this._ObjectAttributes = jsonObj;
            if (jsonObj != null && jsonObj.ContainsKey("uid"))
            {
                this.EntryUid = string.IsNullOrEmpty(jsonObj["uid"].ToString()) ? null : jsonObj["uid"].ToString();
            }

            if (jsonObj != null && jsonObj.ContainsKey("title"))
            {
                this.Title = string.IsNullOrEmpty(jsonObj["title"].ToString()) ? " " : jsonObj["title"].ToString();
            }

            //if (jsonObj != null && jsonObj.ContainsKey("url"))
            //{
            //    this.Url = string.IsNullOrEmpty(jsonObj["url"].ToString()) ? " " : jsonObj["url"].ToString();
            //}

            if (url != null)
            {
                this.Url = url + "/" + this.EntryUid;
            }

            if (jsonObj != null && jsonObj.ContainsKey("_metadata"))
            {
                Dictionary<string, object> _metadataJSON = (Dictionary<string, object>)jsonObj["_metadata"];
                List<string> iterator = _metadataJSON.Keys.ToList();
                //_metadata = new Dictionary<string, object>();
                Metadata = new Dictionary<string, object>();
                foreach (var key in iterator)
                {
                    if (key.Equals("uid"))
                    {
                        this.EntryUid = _metadataJSON[key].ToString();
                    }
                    Metadata.Add(key, _metadataJSON[key]);
                }
            }
            /*
                        if (jsonObj != null && jsonObj.ContainsKey("_owner") && (jsonObj["_owner"] != null) && (!jsonObj["_owner"].ToString().Equals("null")))
                        {
                            Dictionary<string, object> ownerObject = (Dictionary<string, object>)jsonObj["_owner"];
                            if (ownerObject.ContainsKey("email") && ownerObject["email"] != null)
                            {
                                this.OwnerEmailId = ownerObject["email"].ToString();
                            }

                            if (ownerObject.ContainsKey("uid") && ownerObject["uid"] != null)
                            {
                                this.OwnerUid = ownerObject["uid"].ToString();
                            }

                            List<string> iterator = ownerObject.Keys.ToList();

                            foreach (var key in iterator)
                            {
                                Owner.Add(key, ownerObject[key]);
                            }
                        }
                        */
            if (jsonObj != null && jsonObj.ContainsKey("tags"))
            {
                var array = jsonObj["tags"];
                //_TagsArray =(string[])array;
                if (array != null)
                {
                    Tags = (object[])array;
                }
            }
            return this;
        }
        #endregion

    }
}
