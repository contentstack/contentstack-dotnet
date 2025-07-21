using Newtonsoft.Json.Linq;

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
        internal JObject PreviewResponse { get; set; }
        public string ReleaseId {get; set;}
        public string PreviewTimestamp {get; set;}

        /// <summary>
        /// Creates a deep clone of the LivePreviewConfig for request isolation
        /// </summary>
        public LivePreviewConfig Clone()
        {
            return new LivePreviewConfig
            {
                ManagementToken = this.ManagementToken,
                PreviewToken = this.PreviewToken,
                Enable = this.Enable,
                Host = this.Host,
                LivePreview = this.LivePreview,
                ContentTypeUID = this.ContentTypeUID,
                EntryUID = this.EntryUID,
                PreviewResponse = this.PreviewResponse?.DeepClone() as JObject,
                ReleaseId = this.ReleaseId,
                PreviewTimestamp = this.PreviewTimestamp
            };
        }
    }
}
