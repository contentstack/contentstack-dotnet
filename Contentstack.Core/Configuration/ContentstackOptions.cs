using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using Contentstack.Core.Handler;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Configuration
{
    /// <summary>
    /// Represents a set of options to configure a Stack.
    /// </summary>
    public class ContentstackOptions
    {
        /// <summary>
        /// The api key used when communicating with the Contentstack API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The access token used when communicating with the Contentstack API.
        /// </summary>
        [Obsolete("We have deprecated AccessToken and we will stop supporting it in the near future. " +
            "We strongly recommend using DeliveryToken.")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The delivery token used when communicating with the Contentstack API.
        /// </summary>
        public string DeliveryToken { get; set; }

        /// <summary>
        /// The environment used when communicating with the Contentstack API.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The Host used to set host url for the Contentstack API.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The Region used to set region for the Contentstack API.
        /// </summary>
        [TypeConverter(typeof(ContentstackRegionConverter))]
        public ContentstackRegion Region { get; set; } = ContentstackRegion.US;

        /// <summary>
        /// The Version number for the Contentstack API.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The Live preview configuration for the Contentstack API.
        /// </summary>
        public LivePreviewConfig LivePreview { get; set; }

        /// <summary>
        /// The Branch used to set Branch for the Contentstack API.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// Gets or sets the DisableLogging. When set to true, the logging of the client is disabled.
        /// The default value is false.
        /// </summary>
        public bool DisableLogging { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// The default value for this property is 1 gigabytes.
        /// </summary>
        public long MaxResponseContentBufferSize { get; set; } = CSConstants.ContentBufferSize;

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// The default value for time out is 30 seconds.
        /// </summary>
        public TimeSpan Timeout { get; set; } = CSConstants.Timeout;

        /// <summary>
        /// When set to true, the client will retry requests.
        /// When set to false, the client will not retry request.
        /// The default value is true
        /// </summary>
        public bool RetryOnError { get; set; } = true;

        /// <summary>
        /// Returns the flag indicating how many retry HTTP requests an SDK should
        /// make for a single SDK operation invocation before giving up.
        /// The default value is 5.
        /// </summary>
        public int RetryLimit { get; set; } = 5;

        /// <summary>
        /// Returns the flag indicating delay in retrying HTTP requests.
        /// The default value is 300ms.
        /// </summary>
        public TimeSpan RetryDelay { get; set; } = CSConstants.Delay;

        /// <summary>
        /// The retry policy which specifies when 
        /// a retry should be performed.
        /// </summary>
        public RetryPolicy RetryPolicy { get; set; }


        /// <summary>
        /// Host for the Proxy.
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// Port for the Proxy.
        /// </summary>
        public int ProxyPort { get; set; } = -1;

        /// <summary>
        /// Credentials to use with a proxy.
        /// </summary>
        public ICredentials ProxyCredentials { get; set; }

        /// <summary>
        /// Returns a WebProxy instance configured to match the proxy settings
        /// in the configuration.
        /// </summary>
        /// <returns></returns>
        public IWebProxy GetWebProxy()
        {
            const string httpPrefix = "http://";

            WebProxy webProxy = null;
            if (!string.IsNullOrEmpty(ProxyHost) && ProxyPort != -1)
            {
                var host = ProxyHost.StartsWith(httpPrefix, StringComparison.OrdinalIgnoreCase)
                               ? ProxyHost.Substring(httpPrefix.Length)
                               : ProxyHost;
                webProxy = new WebProxy(host, ProxyPort);

                if (ProxyCredentials != null)
                {
                    webProxy.Credentials = ProxyCredentials;
                }
            }

            return webProxy;
        }
    }

    internal class ContentstackRegionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue;
            object result;

            result = null;
            stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue))
            {
                int nonDigitIndex;

                nonDigitIndex = stringValue.IndexOf(stringValue.FirstOrDefault(char.IsLetter));

                if (nonDigitIndex > 0)
                {
                    result = (ContentstackRegion)Enum.Parse(typeof(ContentstackRegion), stringValue.Substring(nonDigitIndex), true);
                }
            }

            return result ?? base.ConvertFrom(context, culture, value);
        }
    }
}

