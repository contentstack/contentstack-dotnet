using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        /// <summary>
        /// Readonly property to delta sync.
        /// </summary>
        [JsonProperty("sync_token")]
        public string SyncToken { get; set; }
        /// <summary>
        /// Readonly property for paginating sync
        /// </summary>
        [JsonProperty("pagination_token")]
        public string PaginationToken { get; set;}
    }
}