using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    internal class Config
    {
        #region Private Variable
        private string _Protocol;
        private string _Host;
        private string _Port;
        private string _BaseURL;
        private string _Version;
        private string _Environment;
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
            get { return this._Host ?? "cdn.contentstack.io"; }
            set { this._Host = value; }
        }

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

        public string BaseUrl
        {
            get
            {
                string port = (this.Port == "80") ? string.Empty : ":" + this.Port;
                this._BaseURL = string.Format("{0}://{1}{2}/{3}",
                this.Protocol.Trim('/').Trim('\\'),
                this.Host.Trim('/').Trim('\\'), port.Trim('/').Trim('\\'), this.Version.Trim('/').Trim('\\'));
                return this._BaseURL;
            }
            set
            {
                this._BaseURL = value;
            }
        }

        #endregion
    }
}
