using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Contentstack.Core.Models
{
    /// <summary>
    /// Represents the result of a sync operation.
    /// </summary>
    public class SyncStack
    {
        /// <summary>
        /// Readonly property contains all the Contents
        /// </summary>
        public IEnumerable<dynamic> Items { get; set; }
        /// <summary>
        /// Readonly property to check totalCount
        /// </summary>
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        /// <summary>
        /// Readonly property to delta sync.
        /// </summary>
        [JsonPropertyName("sync_token")]
        public string SyncToken { get; set; }
        /// <summary>
        /// Readonly property for paginating sync
        /// </summary>
        [JsonPropertyName("pagination_token")]
        public string PaginationToken { get; set;}
    }
}