using System;
namespace Contentstack.Core.Configuration
{
    public class LivePreviewConfig
    {
        public string ManagementToken { get; set; }
        public bool Enable { get; set; }
        public string Host { get; set; }
        internal string LivePreview { get; set; }
        internal string ContentTypeUID { get; set; }
    }
}
