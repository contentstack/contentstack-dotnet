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
        /// <summary>
        /// The number of items skipped in this resultset.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// The maximum number of items returned in this result.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The total number of items available.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The System.Collections.Generic.IEnumerable&lt;T&gt; of items to be serialized from the API response.
        /// </summary>
        /// <value>System.Collections.Generic.IEnumerable&lt;T&gt;</value>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Items"/> collection
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Items"/> collection
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
