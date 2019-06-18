using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Contentstack.Core.Models
{
    [JsonObject]
   public class ContentstackCollection<T> : IEnumerable<T> 
    {
        public int skip { get; set; }

        public int limit { get; set; }

        public int Count { get; set; }

        public IEnumerable<T> entries { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }
    }
}
