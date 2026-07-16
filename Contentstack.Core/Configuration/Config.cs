using System.Net;
using Contentstack.Core.Endpoints;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Configuration
{
    internal class Config
    {
        #region Private Variable
        private string _Protocol;
        private string _Host;
        private string _Port;
        private string _Version;
        private string _Environment;
        private string _Branch="main";
        private int _Timeout;
        private WebProxy _proxy;
        #endregion

        #region Public Properties
        public string ApiKey { get; set; }

        public string AppUid { get; set; }

        public string AuthToken { get; set; }

        public string Port { 
            get { return this._Port ?? "443"; }
            set { this._Port = value; }
        }
       
        public string Protocol { 
            get { return this._Protocol ?? "https"; }
            set { this._Protocol = value; }
        }

        public string Host {
            get { return _Host ?? string.Empty; }
            set { this._Host = value; }
        }

        public ContentstackRegion Region { get; set; } = ContentstackRegion.US;

        public string Version
        {
            get { return this._Version ?? "v3"; }
            set { this._Version = value; }
        }

        public string Environment
        {
            get { return this._Environment ?? null; }
            set { this._Environment = value; }
        }

        public string Branch
        {
            get { return this._Branch ?? null; }
            set { this._Branch = value; }
        }

        public int Timeout
        {
            get { return this._Timeout; }
            set { this._Timeout = value; }
        }

        public WebProxy Proxy
        {
            get { return this._proxy; }
            set { this._proxy = value; }
        }

        public string BaseUrl
        {
            get
            {
                string protocol = this.Protocol.Trim('/').Trim('\\');
                string version = this.Version.Trim('/').Trim('\\');

                // Custom host explicitly set — bypass CDN registry and use it directly.
                if (!string.IsNullOrEmpty(_Host))
                    return string.Format("{0}://{1}/{2}", protocol, _Host.Trim('/').Trim('\\'), version);

                // Resolve host from CDN-backed regions registry.
                string regionId = ContentstackRegionMap.RegionIdMap[Region];
                string host = Endpoint.GetContentstackEndpoint(regionId, "contentDelivery", omitHttps: true);
                return string.Format("{0}://{1}/{2}", protocol, host, version);
            }
        }

        #endregion

        #region Internal
        internal string getLivePreviewUrl(LivePreviewConfig livePreviewConfig)
        {
            
            return string.Format("{0}://{1}/{2}",
                                            this.Protocol.Trim('/').Trim('\\'),
                                            livePreviewConfig.Host.Trim('/').Trim('\\'),
                                            this.Version.Trim('/').Trim('\\'));
        }

        internal string getBaseUrl (LivePreviewConfig livePreviewConfig, string contentTypeUID)
        {
            if (livePreviewConfig != null 
                && livePreviewConfig.Enable 
                && livePreviewConfig.LivePreview != "init" && !string.IsNullOrEmpty(livePreviewConfig.LivePreview)
                && livePreviewConfig.ContentTypeUID == contentTypeUID)
            {
                return getLivePreviewUrl(livePreviewConfig);
            }
            return BaseUrl;
        }

        #endregion
    }
}
