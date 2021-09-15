using System;
namespace Contentstack.Core.Configuration
{
    public class LivePreviewConfig
    {
        public string Authorization { get; set; }
        public bool Enable { get; set; }
        public string Host { get; set; }
        internal string Hash { get; set; }
        internal string ContentTypeUID { get; set; }
    }
}
