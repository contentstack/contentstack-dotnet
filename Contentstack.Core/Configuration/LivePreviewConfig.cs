using System.Text.Json;

namespace Contentstack.Core.Configuration
{
    public class LivePreviewConfig
    {
        public string ManagementToken { get; set; }
        public string PreviewToken { get; set; }
        public bool Enable { get; set; }
        public string Host { get; set; }
        internal string LivePreview { get; set; }
        internal string ContentTypeUID { get; set; }
        internal string EntryUID { get; set; }
        internal JsonElement PreviewResponse { get; set; }
        internal string releaseId {get; set;}
        internal string previewTimestamp {get; set;}
    }
}
