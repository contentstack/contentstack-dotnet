using System;
using System.Collections.Generic;

namespace Contentstack.Core.Models
{
    public class SyncStack
    {
        public IEnumerable<dynamic> items { get; set; }

        public int skip { get; set; }

        public int limit { get; set; }

        public int total_count { get; set; }

        public string sync_token { get; set; }

        public string pagination_token { get; set; }

    }
}