using System;
using System.Collections.Generic;
using Contentstack.Core.Internals;
using Contentstack.Core.Configuration;
using Microsoft.Extensions.Options;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Contentstack.Core
{
    /// <summary>
    /// To fetch stack level information of your application from Contentstack server.
    /// </summary>
    public class ContentstackClient
    {
        #region Internal Variables

        internal string StackApiKey
        {
            get;
            set;
        }
        private ContentstackOptions _options;


        internal string _SyncUrl
        {
            get
            {
                Config config = this.config;
                return String.Format("{0}/stacks/sync",
                                     config.BaseUrl);
            }
        }

        /// <summary>
        /// Initializes a instance of the <see cref="ContentstackClient"/> class. 
        /// </summary>
        /// <param name="options"> used to get stack details via class <see cref="ContentstackOptions"/> to create client.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     var options = new ContentstackOptions()
        ///     {
        ///        ApiKey = &quot;api_key&quot;,
        ///        AccessToken = &quot;access_token&quot;
        ///        Environment = &quot;environment&quot;
        ///      }
        ///     ContentstackClient stack = new ContentstackClient(options);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        /// </code>
        /// </example>
        public ContentstackClient(IOptions<ContentstackOptions> options)
        {
            _options = options.Value;
            this.StackApiKey = _options.ApiKey;
            this._LocalHeaders = new Dictionary<string, object>();
            this.SetHeader("api_key", _options.ApiKey);
            this.SetHeader("access_token", _options.AccessToken);
            Config cnfig = new Config();
            cnfig.Environment = _options.Environment;
            if (_options.Host != null)
            {
                cnfig.Host = _options.Host;
            }
            if (_options.Version != null)
            {
                cnfig.Version = _options.Version;
            }
            this.SetConfig(cnfig);

        }

        public ContentstackClient(ContentstackOptions options) :
            this(new OptionsWrapper<ContentstackOptions>(options))
        {


        }

        /// <summary>
        /// Initializes a instance of the <see cref="ContentstackClient"/> class. 
        /// </summary>
        /// <param name="apiKey">API Key of your stack on Contentstack.</param>
        /// <param name="accessToken">Accesstoken of your stack on Contentstack.</param>
        /// <param name="environment">Environment name</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClient(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        /// </code>
        /// </example>
        public ContentstackClient(string apiKey, string accessToken, string environment, string host = null, string version = null) :
        this(new OptionsWrapper<ContentstackOptions>(new ContentstackOptions()
            {
                ApiKey = apiKey,
                AccessToken = accessToken,
                Environment = environment,
                Host = host,
                Version = version
            }
        ))
        {

        }

        internal string Version { get; set; }

        internal ContentstackConstants _Constants { get; set; }
        internal Dictionary<string, object> _LocalHeaders = new Dictionary<string, object>();
        internal Config config;
        #endregion

        #region Private Constructor
        private ContentstackClient() { }
        #endregion

        #region Internal Constructor
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
        internal ContentstackClient(String stackApiKey)
        {
            this.StackApiKey = stackApiKey;
            this._LocalHeaders = new Dictionary<string, object>();

        }
        #endregion

        #region Internal Functions
        internal void SetConfig(Config cnfig)
        {
            this.config = cnfig;

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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     stack.RemoveHeader(&quot;custom_header_key&quot;);
        /// </code>
        /// </example>
        public void RemoveHeader(string key)
        {
            if (this._LocalHeaders.ContainsKey(key))
                this._LocalHeaders.Remove(key);

        }

        /// <summary>
        /// To set headers for Contentstack rest calls.
        /// </summary>
        /// <param name="key">header name.</param>
        /// <param name="value">header value against given header name.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
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
        /// <summary>
        /// Syncs the recursive.
        /// </summary>
        /// <returns>The recursive.</returns>
        /// <param name="SyncType">Sync type.</param>
        /// <param name="ContentTypeUid">Content type uid.</param>
        /// <param name="StartFrom">Start from Date.</param>
        ///  <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     stack.SyncRecursive(&quot;SyncType&quot;);
        /// </code>
        /// </example>
        /// 
        public async Task<SyncStack> SyncRecursive(SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            SyncStack syncStack = await Sync(SyncType: SyncType, ContentTypeUid: ContentTypeUid, StartFrom: StartFrom);
            syncStack = await SyncPageinationRecursive(syncStack);
            return syncStack;
        }

        /// <summary>
        /// Syncs the recursive with language.
        /// </summary>
        /// <returns>The recursive with language.</returns>
        /// <param name="SyncType">Sync type.</param>
        /// <param name="ContentTypeUid">Content type uid.</param>
        /// <param name="StartFrom">Start from Date.</param>
        /// <param name="Lang">Lang.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     stack.SyncRecursiveLanguage(&quot;SyncType&quot;, &quot;Language&quot;);
        /// </code>
        /// </example>
        /// 
        public async Task<SyncStack> SyncRecursiveLanguage(Language Lang, SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            SyncStack syncStack = await SyncLanguage(Lang: Lang, SyncType: SyncType, ContentTypeUid: ContentTypeUid, StartFrom: StartFrom);
            syncStack = await SyncPageinationRecursive(syncStack);
            return syncStack;
        }

        /// <summary>
        /// Syncs the pagination token.
        /// </summary>
        /// <returns>The pagination token.</returns>
        /// <param name="paginationToken">Pagination token.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     stack.SyncPaginationTokenn(&quot;blt123343&quot;);
        /// </code>
        /// </example>

        public async Task<SyncStack> SyncPaginationToken(string paginationToken)
        {
            return await GetResultAsync(PaginationToken: paginationToken);
        }

        /// <summary>
        /// Syncs the token.
        /// </summary>
        /// <returns>The token.</returns>
        /// <param name="SyncToken">Sync token.</param>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     ContentstackClient stack = new ContentstackClinet(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        ///     stack.SyncToken(&quot;blt123343&quot;);
        /// </code>
        /// </example>
        public async Task<SyncStack> SyncToken(string SyncToken)
        {
            return await GetResultAsync(SyncToken: SyncToken);
        }
        #endregion

        #region Private Functions

        private async Task<SyncStack> SyncPageinationRecursive(SyncStack syncStack)
        {
            while (syncStack.pagination_token != null)
            {
                SyncStack newSyncStack = await SyncPaginationToken(syncStack.pagination_token);
                syncStack.items = syncStack.items.Concat(newSyncStack.items);
                syncStack.pagination_token = newSyncStack.pagination_token;
                syncStack.skip = newSyncStack.skip;
                syncStack.total_count = newSyncStack.total_count;
                syncStack.sync_token = newSyncStack.sync_token;
            }
            return syncStack;
        }

        private async Task<SyncStack> Sync(SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            return await GetResultAsync(Init: "true", ContentTypeUid: ContentTypeUid, StartFrom: StartFrom);
        }


        private async Task<SyncStack> SyncLanguage(Language Lang, SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            return await GetResultAsync(Init: "true", ContentTypeUid: ContentTypeUid, StartFrom: StartFrom, Lang: GetLocaleCode(Lang));
        }

        //GetLanguage code 
        private string GetLocaleCode(Language language)
        {
            string localeCode = null;
            try
            {
                int localeValue = (int)language;
                LanguageCode[] languageCodeValues = Enum.GetValues(typeof(LanguageCode)).Cast<LanguageCode>().ToArray();
                localeCode = languageCodeValues[localeValue].ToString();
                localeCode = localeCode.Replace("_", "-");
            }
            catch (Exception e)
            {
                throw new Exception(StackConstants.ErrorMessage_QueryFilterException, e);
            }
            return localeCode;
        }

        private Dictionary<string, object> GetHeader()
        {

            Dictionary<string, object> mainHeader = _LocalHeaders;

            return mainHeader;
        }


        private async Task<SyncStack> GetResultAsync(string Init = "false", string ContentTypeUid = null, DateTime? StartFrom = null, string SyncToken = null, string PaginationToken = null, string Lang = null)
        {
            //mainJson = null;
            Dictionary<string, object> mainJson = new Dictionary<string, object>();
            if (Init != "false")
            {
                mainJson.Add("init", "true");
                mainJson.Add("environment", config.Environment);
            }
            if (StartFrom != null)
            {
                DateTime startFrom = StartFrom ?? DateTime.MinValue;
                mainJson.Add("start_from", startFrom.ToString("yyyy-MM-dd"));
            }
            if (SyncToken != null)
            {
                mainJson.Add("sync_token", SyncToken);
            }
            if (PaginationToken != null)
            {
                mainJson.Add("pagination_token", PaginationToken);
            }
            if (ContentTypeUid != null)
            {
                mainJson.Add("content_type_uid", ContentTypeUid);
            }
            if (Lang != null)
            {
                mainJson.Add("locale", Lang);
            }
            try
            {
                HTTPRequestHandler requestHandler = new HTTPRequestHandler();
                string js = await requestHandler.ProcessRequest(_SyncUrl, _LocalHeaders, mainJson);
                SyncStack stackSyncOutput = JsonConvert.DeserializeObject<SyncStack>(js);
                return stackSyncOutput;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }           
        }
        #endregion

    }
}
