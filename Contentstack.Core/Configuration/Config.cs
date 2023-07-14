using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (Region == ContentstackRegion.AZURE_NA) return "azure-";
            ContentstackRegionCode[] regionCodes = Enum.GetValues(typeof(ContentstackRegionCode)).Cast<ContentstackRegionCode>().ToArray();
            return string.Format("{0}-", regionCodes[(int)Region].ToString());
        }

        internal string HostURL
        {
            get
            {
                if (Region == ContentstackRegion.EU)
                    return "cdn.contentstack.com";
                if (Region == ContentstackRegion.AZURE_EU)
                    return "eu-cdn.contentstack.com";
                if (Region == ContentstackRegion.AZURE_NA)
                    return "na-cdn.contentstack.com";
                return "cdn.contentstack.io";
            }
        }
        #endregion
    }
}
