using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Replaces Newtonsoft <c>JObject.Merge</c> for query objects in <see cref="Models.AssetLibrary"/>.
    /// </summary>
    internal static class JsonObjectMerge
    {
        /// <summary>
        /// Merges <paramref name="addition"/> into <paramref name="target"/> (modifies <paramref name="target"/>).
        /// For array values, appends items from <paramref name="addition"/> that are not already present
        /// (string compare of JSON text), similar to Newtonsoft <c>MergeArrayHandling.Union</c>.
        /// </summary>
        internal static void UnionMergeInto(JsonObject target, JsonObject addition)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (addition == null)
            {
                return;
            }

            foreach (var kvp in addition)
            {
                if (!target.TryGetPropertyValue(kvp.Key, out var existing) || existing == null)
                {
                    target[kvp.Key] = kvp.Value?.DeepClone();
                    continue;
                }

                if (existing is JsonObject existingObj && kvp.Value is JsonObject addObj)
                {
                    UnionMergeInto(existingObj, addObj);
                }
                else if (existing is JsonArray existingArr && kvp.Value is JsonArray addArr)
                {
                    var present = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var n in existingArr)
                    {
                        if (n != null)
                        {
                            present.Add(n.ToJsonString());
                        }
                    }
                    foreach (var n in addArr)
                    {
                        if (n == null)
                        {
                            continue;
                        }
                        var s = n.ToJsonString();
                        if (present.Add(s))
                        {
                            existingArr.Add(n.DeepClone());
                        }
                    }
                }
                else
                {
                    target[kvp.Key] = kvp.Value?.DeepClone();
                }
            }
        }
    }
}
