using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Contentstack.Core.Models
{
    public class ContentstackCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Builds a collection from a Delivery / Management API envelope object.
        /// (Do not use JsonSerializer on this type: it implements IEnumerable and STJ would expect a JSON array at the root.)
        /// </summary>
        internal static ContentstackCollection<T> FromDeliveryEnvelope(JsonObject envelope, IEnumerable<T> items)
        {
            static JsonNode FindProperty(JsonObject obj, params string[] candidateNames)
            {
                var wanted = new HashSet<string>(candidateNames, System.StringComparer.OrdinalIgnoreCase);
                foreach (var kv in obj)
                {
                    if (wanted.Contains(kv.Key))
                        return kv.Value;
                }
                return null;
            }

            static int ReadInt(JsonObject obj, params string[] candidateNames)
            {
                var node = FindProperty(obj, candidateNames);
                if (node == null)
                    return 0;
                try
                {
                    return node.GetValue<int>();
                }
                catch
                {
                    try
                    {
                        var s = node.ToString()?.Trim();
                        if (string.IsNullOrEmpty(s))
                            return 0;
                        return int.Parse(s, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }

            return new ContentstackCollection<T>
            {
                Items = items,
                Skip = ReadInt(envelope, "skip"),
                Limit = ReadInt(envelope, "limit"),
                Count = ReadInt(envelope, "count", "total_count"),
            };
        }

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
        /// The System.Collections.Generic.IEnumerable<T> of items to be serialized from the API response.
        /// </summary>
        /// <value>System.Collections.Generic.IEnumerable<T></value>
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
