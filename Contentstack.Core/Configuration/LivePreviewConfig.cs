using System;
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

        /// <summary>
        /// Snapshot of preview_timestamp / release_id / live_preview when <see cref="PreviewResponse"/> was set (prefetch).
        /// Prevents Entry.Fetch from short-circuiting with a draft from a previous Live Preview query.
        /// </summary>
        internal string PreviewResponseFingerprintPreviewTimestamp { get; set; }
        internal string PreviewResponseFingerprintReleaseId { get; set; }
        internal string PreviewResponseFingerprintLivePreview { get; set; }

        public string ReleaseId {get; set;}
        public string PreviewTimestamp {get; set;}

        internal bool IsCachedPreviewForCurrentQuery()
        {
            if (PreviewResponse == null) return false;
            return string.Equals(PreviewTimestamp ?? "", PreviewResponseFingerprintPreviewTimestamp ?? "", StringComparison.Ordinal)
                && string.Equals(ReleaseId ?? "", PreviewResponseFingerprintReleaseId ?? "", StringComparison.Ordinal)
                && string.Equals(LivePreview ?? "", PreviewResponseFingerprintLivePreview ?? "", StringComparison.Ordinal);
        }
    }
}
