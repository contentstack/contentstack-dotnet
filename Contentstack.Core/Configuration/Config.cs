using System;
using System.Linq;
using System.Net;
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
        private string _Branch;
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
            get { return _Host ?? HostURL; }
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
                string BaseURL = string.Format("{0}://{1}{2}/{3}",
                                              this.Protocol.Trim('/').Trim('\\'),
                                              regionCode(),
                                              this.Host.Trim('/').Trim('\\'),
                                              this.Version.Trim('/').Trim('\\'));
                return BaseURL;
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
            if (livePreviewConfig != null && livePreviewConfig.Enable && livePreviewConfig.ContentTypeUID == contentTypeUID)
            {
                return getLivePreviewUrl(livePreviewConfig);
            }
            return BaseUrl;
        }

        internal string regionCode()
        {
            if (Region == ContentstackRegion.US) return "";
            ContentstackRegionCode[] regionCodes = Enum.GetValues(typeof(ContentstackRegionCode)).Cast<ContentstackRegionCode>().ToArray();
            return string.Format("{0}-", regionCodes[(int)Region].ToString().Replace("_", "-"));
        }

        internal string HostURL
        {
            get
            {
                if (Region == ContentstackRegion.EU || Region == ContentstackRegion.AZURE_EU || Region == ContentstackRegion.AZURE_NA)
                    return "cdn.contentstack.com";
                return "cdn.contentstack.io";
            }
        }
        #endregion
    }
}
