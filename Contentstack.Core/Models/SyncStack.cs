using System;
using System.Collections.Generic;

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
        public IEnumerable<dynamic> items { get; set; }
        /// <summary>
        /// Readonly property to check skip count
        /// </summary>
        public int skip { get; set; }
        /// <summary>
        /// Readonly property to check limit
        /// </summary>
        public int limit { get; set; }
        /// <summary>
        /// Readonly property to check totalCount
        /// </summary>
        public int total_count { get; set; }
        /// <summary>
        /// Readonly property to delta sync.
        /// </summary>
        public string sync_token { get; set; }
        /// <summary>
        /// Readonly property for paginating sync
        /// </summary>
        public string pagination_token { get; set;}
    }
}