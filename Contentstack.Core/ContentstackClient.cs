using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using Contentstack.Core.Interfaces;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;

namespace Contentstack.Core
{
    /// <summary>
    /// To fetch stack level information of your application from Contentstack server.
    /// </summary>
    public class ContentstackClient
    {
        /// <summary>
        /// Gets or sets the settings that should be used for deserialization.
        /// </summary>
        public JsonSerializerOptions SerializerSettings { get; set; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = {
                new JsonStringEnumConverter(),
                new CustomUtcDateTimeConverter(),
                new CustomNullableUtcDateTimeConverter(),
            }
        };

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
                Config config = this.Config;
                return String.Format("{0}/stacks/sync",
                                     config.BaseUrl);
            }
        }

        internal LivePreviewConfig LivePreviewConfig;
        private Dictionary<string, object> UrlQueries = new Dictionary<string, object>();
        private Dictionary<string, object> _Headers = new Dictionary<string, object>();
        private string _Url
        {
            get
            {
                return String.Format("{0}/content_types/", this.Config.BaseUrl);
            }
        }
        private Dictionary<string, object> _StackHeaders = new Dictionary<string, object>();
        public List<IContentstackPlugin> Plugins { get; set; } = new List<IContentstackPlugin>();
        /// <summary>
        /// Initializes a instance of the <see cref="ContentstackClient"/> class. 
        /// </summary>
        /// <param name="options"> used to get stack details via class <see cref="ContentstackOptions"/> to create client.</param>
        /// <example>
        /// <code>
        ///     var options = new ContentstackOptions()
        ///     {
        ///        ApiKey = &quot;api_key&quot;,
        ///        DeliveryToken = &quot;delivery_token&quot;
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
            if (_options.AccessToken != null)
            {
                this.SetHeader("access_token", _options.AccessToken);
            } else if (_options.DeliveryToken != null)
            {
                this.SetHeader("access_token", _options.DeliveryToken);
            }
            if(_options.EarlyAccessHeader !=null)
            {
                this.SetHeader("x-header-ea", string.Join(",", _options.EarlyAccessHeader));
            }
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
            if (_options.Proxy != null)
            {
                cnfig.Proxy = _options.Proxy;
            }
            cnfig.Region = _options.Region;
            cnfig.Branch = _options.Branch;
            cnfig.Timeout = _options.Timeout;
            this.SetConfig(cnfig);
            if (_options.LivePreview != null)
            {
                this.LivePreviewConfig = _options.LivePreview;
            }
            else
            {
                this.LivePreviewConfig = new LivePreviewConfig()
                {
                    Enable = false,
                };
            }
            if (this.LivePreviewConfig.Host == null && this.LivePreviewConfig.Enable)
            {
                if (this.LivePreviewConfig.ManagementToken != null)
                {
                    this.LivePreviewConfig.Host = "api.contentstack.io";
                }
                else if (this.LivePreviewConfig.PreviewToken != null)
                {
                    this.LivePreviewConfig.Host = "rest-preview.contentstack.com";
                } else {
                    throw new InvalidOperationException("Add PreviewToken or ManagementToken in LivePreviewConfig");
                }
            }

            foreach (Type t in CSJsonConverterAttribute.GetCustomAttribute(typeof(CSJsonConverterAttribute)))
            {
                SerializerSettings.Converters.Add((JsonConverter)Activator.CreateInstance(t));
            }
        }

        public ContentstackClient(ContentstackOptions options) :
            this(new OptionsWrapper<ContentstackOptions>(options))
        {


        }

        /// <summary>
        /// Initializes a instance of the <see cref="ContentstackClient"/> class. 
        /// </summary>
        /// <param name="apiKey">API Key of your stack on Contentstack.</param>
        /// <param name="deliveryToken">Accesstoken of your stack on Contentstack.</param>
        /// <param name="environment">Environment name</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClient(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     ContentType contentType = stack.ContentType(&quot;contentType_name&quot;);
        /// </code>
        /// </example>
        public ContentstackClient(string apiKey, string deliveryToken, string environment, string host = null, ContentstackRegion region = ContentstackRegion.US, string version = null, int? timeout = null, WebProxy proxy = null) :
        this(new OptionsWrapper<ContentstackOptions>(new ContentstackOptions()
        {
            ApiKey = apiKey,
            DeliveryToken = deliveryToken,
            Environment = environment,
            Host = host,
            Region = region,
            Version = version,
            Timeout = timeout ?? 30000,
            Proxy = proxy
        }
        ))
        {

        }

        internal string Version { get; set; }

        internal ContentstackConstants _Constants { get; set; }
        internal Dictionary<string, object> _LocalHeaders = new Dictionary<string, object>();
        internal Config Config;
        #endregion

        #region Private Constructor
        private ContentstackClient() { }
        #endregion

        #region Internal Constructor
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
                    var data = JsonSerializer.Deserialize<JsonElement>(errorMessage);

                    if (data.TryGetProperty("error_code", out var token))
                        errorCode = token.GetInt32();

                    if (data.TryGetProperty("error_message", out token))
                        errorMessage = token.GetString();

                    if (data.TryGetProperty("errors", out token))
                        errors = JsonSerializer.Deserialize<Dictionary<string, object>>(token.GetRawText());

                    if (exResp is HttpWebResponse response)
                        statusCode = response.StatusCode;
                }
            }
            catch
            {
                errorMessage = ex.Message;
            }

            contentstackError = new ContentstackException()
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
            this.Config = cnfig;

        }


        /// <summary>
        /// Represents a Asset. Creates Asset Instance.
        /// </summary>
        /// <returns>Current instance of Asset, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Asset asset  = stack.Asset();
        /// </code>
        /// </example>
        internal Asset Asset()
        {
            Asset asset = new Asset(this);
            return asset;
        }

        #endregion

        #region Public Functions
        /// <summary>
        /// This method returns comprehensive information of all the content types of a particular stack in your account.
        /// </summary>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     var param = new Dictionary&lt;string, object&gt;();
        ///     param.Add("include_global_field_schema",true);
        ///     param.Add("limit", 10);
        ///     param.Add("skip", 10);
        ///     param.Add("include_count", "true");
        ///     var result = await stack.GetContentTypes(param);
        /// </code>
        /// </example>
        /// <param name="param">is dictionary of additional parameter</param>
        /// <returns>The List of content types schema.</returns>
        public async Task<IList> GetContentTypes(Dictionary<string, object> param = null)
        {
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
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
            mainJson.Add("environment", this.Config.Environment);

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
                HttpRequestHandler RequestHandler = new HttpRequestHandler(this);
                var outputResult = await RequestHandler.ProcessRequest(_Url, headers, mainJson, Branch: this.Config.Branch, timeout: this.Config.Timeout, proxy: this.Config.Proxy);
                var data = JsonSerializer.Deserialize<JsonElement>(outputResult);
                var contentTypes = data.GetProperty("content_types").EnumerateArray().ToList();
                return contentTypes;
            }
            catch (Exception ex)
            {
                throw GetContentstackError(ex);
            }
        }

        private async Task<JsonElement> GetLivePreviewData()
        {
  
            Dictionary<String, object> headerAll = new Dictionary<string, object>();
            Dictionary<string, object> mainJson = new Dictionary<string, object>();
            Dictionary<String, Object> headers = GetHeader(_LocalHeaders);
            if (headers != null && headers.Count() > 0)
            {
                foreach (var header in headers)
                {
                    if (this.LivePreviewConfig.Enable == true
                        && header.Key == "access_token")
                    {
                        continue;
                    }
                    headerAll.Add(header.Key, (String)header.Value);
                }
            }
            mainJson.Add("live_preview", this.LivePreviewConfig.LivePreview ?? "init");
            
            if (!string.IsNullOrEmpty(this.LivePreviewConfig.ManagementToken)) {
                headerAll["authorization"] = this.LivePreviewConfig.ManagementToken;
            } else if (!string.IsNullOrEmpty(this.LivePreviewConfig.PreviewToken)) {
                headerAll["preview_token"] = this.LivePreviewConfig.PreviewToken;
            } else {
                throw new InvalidOperationException("Either ManagementToken or PreviewToken is required in LivePreviewConfig");
            }

            try
            {
                HttpRequestHandler RequestHandler = new HttpRequestHandler(this);
                var outputResult = await RequestHandler.ProcessRequest(String.Format("{0}/content_types/{1}/entries/{2}", this.Config.getLivePreviewUrl(this.LivePreviewConfig), this.LivePreviewConfig.ContentTypeUID, this.LivePreviewConfig.EntryUID), headerAll, mainJson, Branch: this.Config.Branch, isLivePreview: true, timeout: this.Config.Timeout, proxy: this.Config.Proxy);
                var data = JsonSerializer.Deserialize<JsonElement>(outputResult);
                return data.GetProperty("entry");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Represents a ContentType. Creates ContenntType Instance.
        /// </summary>
        /// <param name="contentTypeName">ContentType name.</param>
        /// <returns>Current instance of ContentType, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Asset asset  = stack.Asset(&quot;asset_uid&quot;);
        /// </code>
        /// </example>
        public Asset Asset(String Uid)
        {
            Asset asset = new Asset(this, Uid);
            return asset;
        }

        /// <summary>
        /// Represents a AssetLibrary. Creates AssetLibrary Instance.
        /// </summary>
        /// <returns>Current instance of Asset, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     AssetLibrary assetLibrary = stack.AssetLibrary();
        /// </code>
        /// </example>
        public AssetLibrary AssetLibrary()
        {
            AssetLibrary asset = new AssetLibrary(this);
            return asset;
        }

        /// <summary>
        /// Represents a Taxonomy. Creates Taxonomy Instance.
        /// </summary>
        /// <returns>Current instance of Taxonomy, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     Taxonomy taxonomy = stack.Taxonomy();
        /// </code>
        /// </example>
        public Taxonomy Taxonomies()
        {
            Taxonomy tx = new Taxonomy(this);
            return tx;
        }

        /// <summary>
        /// Get version.
        /// </summary>
        /// <returns>Version</returns>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     String environment = stack.GetEnvironment();
        /// </code>
        /// </example>
        public string GetEnvironment()
        {
            return this.Config.Environment;
        }

        /// <summary>
        /// Remove header key.
        /// </summary>
        /// <param name="key">key to be remove from header</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
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
        /// To add live preview Query for contentstack preview call
        /// </summary>
        /// <param name="query">Query parameter containing hash and content type UID </param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     stack.LivePreviewQuery(query);
        /// </code>
        /// </example>
        public async Task LivePreviewQueryAsync(Dictionary<string, string> query)
        {
            this.LivePreviewConfig.LivePreview = null;
            this.LivePreviewConfig.ContentTypeUID = null;
            this.LivePreviewConfig.EntryUID = null;

            if (query.Keys.Contains("content_type_uid"))
            {
                string contentTypeUID = null;
                query.TryGetValue("content_type_uid", out contentTypeUID);
                this.LivePreviewConfig.ContentTypeUID = contentTypeUID;
            }
            if (query.Keys.Contains("entry_uid"))
            {
                string contentTypeUID = null;
                query.TryGetValue("entry_uid", out contentTypeUID);
                this.LivePreviewConfig.EntryUID = contentTypeUID;
            }
            if (query.Keys.Contains("live_preview"))
            {
                string hash = null;
                query.TryGetValue("live_preview", out hash);
                this.LivePreviewConfig.LivePreview = hash;
            }
            this.LivePreviewConfig.PreviewResponse = await GetLivePreviewData();
        }

        /// <summary>
        /// Syncs the recursive language.
        /// </summary>
        /// <returns>The recursive language.</returns>
        /// <param name="Locale">Locale.</param>
        /// <param name="SyncType">Sync type.</param>
        /// <param name="ContentTypeUid">Content type uid.</param>
        /// <param name="StartFrom">Start from.</param>
        /// <example>
        /// <code>
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     stack.SyncRecursiveLanguage(&quot;SyncType&quot;, &quot;Locale&quot;);
        /// </code>
        /// </example>
        public async Task<SyncStack> SyncRecursive(String Locale = null, SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            SyncStack syncStack = await SyncLanguage(Locale: Locale, SyncType: SyncType, ContentTypeUid: ContentTypeUid, StartFrom: StartFrom);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     stack.SyncPaginationTokenn(&quot;pagination_token&quot;);
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
        ///     ContentstackClient stack = new ContentstackClinet(&quot;api_key&quot;, &quot;delivery_token&quot;, &quot;environment&quot;);
        ///     stack.SyncToken(&quot;sync_token&quot;);
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
            while (syncStack.PaginationToken != null)
            {
                SyncStack newSyncStack = await SyncPaginationToken(syncStack.PaginationToken);
                syncStack.Items = syncStack.Items.Concat(newSyncStack.Items);
                syncStack.PaginationToken = newSyncStack.PaginationToken;
                syncStack.TotalCount = newSyncStack.TotalCount;
                syncStack.SyncToken = newSyncStack.SyncToken;
            }
            return syncStack;
        }

        private async Task<SyncStack> Sync(SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            return await GetResultAsync(Init: "true", SyncType: SyncType, ContentTypeUid: ContentTypeUid, StartFrom: StartFrom);
        }


        private async Task<SyncStack> SyncLanguage(String Locale, SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null)
        {
            return await GetResultAsync(Init: "true", SyncType: SyncType, ContentTypeUid: ContentTypeUid, StartFrom: StartFrom, Locale: Locale);
        }

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

        private Dictionary<string, object> GetHeader()
        {

            Dictionary<string, object> mainHeader = _LocalHeaders;

            return mainHeader;
        }


        private async Task<SyncStack> GetResultAsync(string Init = "false", SyncType SyncType = SyncType.All, string ContentTypeUid = null, DateTime? StartFrom = null, string SyncToken = null, string PaginationToken = null, string Locale = null)
        {
            //mainJson = null;
            Dictionary<string, object> mainJson = new Dictionary<string, object>();
            if (Init != "false")
            {
                mainJson.Add("init", "true");
                mainJson.Add("environment", this.Config.Environment);
            }
            if (StartFrom != null)
            {
                DateTime startFrom = StartFrom ?? DateTime.MinValue;
                mainJson.Add("start_from", startFrom.ToString("yyyy-MM-ddTHH\\:mm\\:ss.sssZ"));
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
            if (Locale != null)
            {
                mainJson.Add("locale", Locale);
            }
            if (SyncType.HasFlag(SyncType.All))
            {

            }
            if (SyncType.HasFlag(SyncType.All))
            {
                mainJson.Add("type", "entry_published,asset_published,entry_unpublished,asset_unpublished,entry_deleted,asset_deleted,content_type_deleted");
            }
            else
            {
                List<string> Type = new List<string>();
                if (SyncType.HasFlag(SyncType.EntryDeleted))
                {
                    Type.Add("entry_deleted");
                }
                if (SyncType.HasFlag(SyncType.EntryPublished))
                {
                    Type.Add("entry_published");
                }
                if (SyncType.HasFlag(SyncType.EntryUnpublished))
                {
                    Type.Add("entry_unpublished");
                }
                if (SyncType.HasFlag(SyncType.AssetDeleted))
                {
                    Type.Add("asset_deleted");
                }
                if (SyncType.HasFlag(SyncType.AssetPublished))
                {
                    Type.Add("asset_published");
                }
                if (SyncType.HasFlag(SyncType.AssetUnpublished))
                {
                    Type.Add("asset_unpublished");
                }
                if (SyncType.HasFlag(SyncType.ContentTypeDeleted))
                {
                    Type.Add("content_type_deleted");
                }
                mainJson.Add("type", String.Join(",", Type.ToArray()));
            }

            try
            {
                HttpRequestHandler requestHandler = new HttpRequestHandler(this);
                string js = await requestHandler.ProcessRequest(_SyncUrl, _LocalHeaders, mainJson, Branch: this.Config.Branch, timeout: this.Config.Timeout, proxy: this.Config.Proxy);
                SyncStack stackSyncOutput = JsonSerializer.Deserialize<SyncStack>(js);
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
