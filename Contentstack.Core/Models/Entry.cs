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
using Contentstack.Core.Configuration;

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
        private bool _IsCachePolicySet;
        private JObject jObject;
        private string _Url
        {
            get
            {

                Config config = this.ContentTypeInstance.StackInstance.Config;

                if (!String.IsNullOrEmpty(this.Uid))
                    return String.Format("{0}/content_types/{1}/entries/{2}", config.BaseUrl, this.ContentTypeInstance.ContentTypeId, this.Uid);
                else
                    return String.Format("{0}/content_types/{1}/entries", config.BaseUrl, this.ContentTypeInstance.ContentTypeId);
            }
        }
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Uid;
        ///     });
        /// </code>
        /// </example>
        public string Uid { get; set; }

        /// <summary>
        /// Set array of Tags
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Tags;
        ///     });
        /// </code>
        /// </example>
        public object[] Tags { get; set; }

        /// <summary>
        /// Set key/value attributes of Metadata.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.Metadata;
        ///     });
        /// </code>
        /// </example>
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Set key/value attributes of an current entry instance.
        /// </summary>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            this.Uid = uid;
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetCachePolicy(CachePolicy.NetworkElseCache);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        /// Get title 
        /// </summary>
        /// <returns>title</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.GetContentType()
        /// </code>
        /// </example>
        public String GetContentType()
        {
            return this.ContentTypeInstance.ContentTypeId;
        }

        /// <summary>
        /// Get uid
        /// </summary>
        /// <returns>Uid</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetUid();
        ///     });
        /// </code>
        /// </example>
        public String GetUid()
        {
            return Uid;
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetMetadata();
        ///     });
        /// </code>
        /// </example>
        public Dictionary<string, Object> GetMetadata()
        {
            return Metadata;
        }

        /// <summary>
        /// Set Language instance
        /// </summary>
        /// <param name="language">Language value</param>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetLanguage(Language.ENGLISH_UNITED_STATES);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetMetadata();
        ///     });
        /// </code>
        /// </example>
        [ObsoleteAttribute("This method has been deprecated. Use SetLocale instead.", true)]
        public Entry SetLanguage(Language language)
        {

            try
            {
                Language languageName = language;
                int localeValue = (int)languageName;
                LanguageCode[] languageCodeValues = Enum.GetValues(typeof(LanguageCode)).Cast<LanguageCode>().ToArray();
                string localeCode = languageCodeValues[localeValue].ToString();
                localeCode = localeCode.Replace("_", "-");

                if (ObjectValueJson != null && !ObjectValueJson.ContainsKey("locale"))
                {
                    UrlQueries.Remove("locale");
                    UrlQueries.Add("locale", localeCode);
                }
                else
                {
                    UrlQueries["locale"] = localeCode;
                }

            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }


            return this;
        }

        /// <summary>
        /// Sets the locale.
        /// </summary>
        /// <returns>The locale.</returns>
        /// <param name="Locale">Locale.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.SetLocale("en-us");
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetMetadata();
        ///     });
        /// </code>
        /// </example>
        public Entry SetLocale(String Locale)
        {
            if (ObjectValueJson != null && !ObjectValueJson.ContainsKey("locale"))
            {
                UrlQueries.Remove("locale");
                UrlQueries.Add("locale", Locale);
            }
            else
            {
                UrlQueries["locale"] = Locale;
            }
            return this;
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
                catch
                {
                }
            }
            return result;
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            catch
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            catch
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            catch
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetDeletedAt();
        ///     });
        /// </code>
        /// </example>
        public DateTime GetDeletedAt()
        {

            try
            {
                String value = _ObjectAttributes["deleted_at"].ToString();
                return ContentstackConvert.ToDateTime(value);
            }
            catch
            {
                // CSAppUtils.showLog(TAG, "-----------------GetDeletedAt|" + e);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        /// Get key/value pairs in json of current instance.
        /// </summary>
        /// <returns>json in string format</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.ToJson();
        ///     });
        /// </code>
        /// </example>
        public JObject ToJson()
        {
            return this.jObject;
        }

        /// <summary>

        /// <summary>
        /// Get an asset from the entry
        /// </summary>
        /// <param name="key">field_uid as key.</param>
        /// <returns>Asset instance</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetAsset(&quot;field_uid&quot;);
        ///     });
        /// </code>
        /// </example>
        private Asset GetAsset(String key)
        {

            JObject assetObject = (JObject)jObject.GetValue(key);
            var asset = ContentTypeInstance.StackInstance.Asset();
            asset.ParseObject(assetObject);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///         //var result = entryResult.Result.GetAssets(&quot;field_uid&quot;);
        ///     });
        /// </code>
        /// </example>
        /// 
        private List<Asset> GetAssets(String key)
        {
            List<Asset> assets = new List<Asset>();
            JArray assetArray = (Newtonsoft.Json.Linq.JArray)jObject.GetValue(key);
            //Dictionary<string, object> assetArray = (Dictionary<string, object>)_ObjectAttributes[key];

            foreach (JToken v in assetArray)
            {
                JObject assetobj = (JObject)v;
                Asset asset = ContentTypeInstance.StackInstance.Asset();
                asset.ParseObject(assetobj);
                assets.Add(asset);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Except(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            catch
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeReference(&quot;name&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            }
            catch  {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeReference(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            catch
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeOnlyReference(new String[]{&quot;name&quot;, &quot;description&quot;}, &quot;referenceUid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///  This method also includes the content type UIDs of the referenced entries returned in the response.
        /// </summary>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeReferenceContentTypeUID();
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry IncludeReferenceContentTypeUID()
        {
            try
            {
                UrlQueries.Add("include_reference_content_type_uid", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeSchema();
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        private Entry IncludeSchema()
        {
            try
            {
                UrlQueries.Add("include_schema", true);
                UrlQueries.Add("include_global_field_schema", true);
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return this;
        }

        /// <summary>
        /// This method also includes owner information for all the entries returned in the response.
        /// </summary>
        /// <returns>Current instance of Entry, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeOwner();
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public Entry IncludeOwner()
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.IncludeExceptReference(new String[]{&quot;name&quot;, &quot;description&quot;},&quot;referenceUid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Only(new String[]{&quot;name&quot;, &quot;description&quot;});
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
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
            catch
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     Entry entry = stack.ContentType(&quot;contentType_id&quot;).Entry(&quot;entry_uid&quot;);
        ///     entry.Fetch&lt;Product&gt;().ContinueWith((entryResult) =&gt; {
        ///         //Your callback code.
        ///     });
        /// </code>
        /// </example>
        public async Task<T> Fetch<T>()
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
            }
            mainJson.Add("environment", this.ContentTypeInstance.StackInstance.Config.Environment);

            foreach (var kvp in UrlQueries)
            {
                mainJson.Add(kvp.Key, kvp.Value);
            }

            try
            {
                CachePolicy cachePolicy = CachePolicy.NetworkOnly;
                if (_IsCachePolicySet)
                {
                    cachePolicy = _CachePolicy;
                }

                HttpRequestHandler RequestHandler = new HttpRequestHandler();
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson);
                JObject obj = JObject.Parse(ContentstackConvert.ToString(outputResult, "{}"));
                var serializedObject = obj.SelectToken("$.entry").ToObject<T>(this.ContentTypeInstance.StackInstance.Serializer);
                if (serializedObject.GetType() == typeof(Entry))
                {
                    (serializedObject as Entry).ContentTypeInstance = this.ContentTypeInstance;
                }
                return serializedObject;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
        }

        #endregion

        #region Private Functions

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

        internal void ParseObject(JObject jsonObj, string url = null)
        {
            this.jObject = jsonObj;
            this._ObjectAttributes = jsonObj.ToObject<Dictionary<string, object>>();
            if (_ObjectAttributes != null && _ObjectAttributes.ContainsKey("_metadata"))
            {
                Dictionary<string, object> _metadataJSON = (Dictionary<string, object>)_ObjectAttributes["_metadata"];
                List<string> iterator = _metadataJSON.Keys.ToList();
                Metadata = new Dictionary<string, object>();
                foreach (var key in iterator)
                {
                    Metadata.Add(key, _metadataJSON[key]);
                }
            }
        }

        #endregion

    }
}
