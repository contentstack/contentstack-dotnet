using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Converts <see cref="JsonNode"/> trees to CLR types similar to Newtonsoft
    /// <c>ToObject&lt;Dictionary&lt;string, object&gt;&gt;</c> (nested objects and collections).
    /// </summary>
    internal static class JsonNodeConversion
    {
        internal static Dictionary<string, object> JsonObjectToDictionary(JsonObject obj)
        {
            var dict = new Dictionary<string, object>(StringComparer.Ordinal);
            if (obj == null)
            {
                return dict;
            }

            foreach (var kvp in obj)
            {
                dict[kvp.Key] = JsonNodeToClr(kvp.Value);
            }

            return dict;
        }

        internal static object JsonNodeToClr(JsonNode node)
        {
            if (node == null)
            {
                return null;
            }

            if (node is JsonObject jo)
            {
                return JsonObjectToDictionary(jo);
            }

            if (node is JsonArray ja)
            {
                var list = new List<object>(ja.Count);
                foreach (var item in ja)
                {
                    list.Add(JsonNodeToClr(item));
                }
                return list;
            }

            if (node is JsonValue jv)
            {
                return JsonValueToClr(jv);
            }

            return null;
        }

        private static object JsonValueToClr(JsonValue jv)
        {
            var el = JsonSerializer.Deserialize<JsonElement>(jv.ToJsonString());
            return JsonElementToObject(el);
        }

        private static object JsonElementToObject(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    var o = new Dictionary<string, object>(StringComparer.Ordinal);
                    foreach (var p in el.EnumerateObject())
                    {
                        o[p.Name] = JsonElementToObject(p.Value);
                    }
                    return o;
                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in el.EnumerateArray())
                    {
                        list.Add(JsonElementToObject(item));
                    }
                    return list;
                case JsonValueKind.String:
                    return el.GetString();
                case JsonValueKind.Number:
                    if (el.TryGetInt32(out var i32))
                    {
                        return i32;
                    }
                    if (el.TryGetInt64(out var i64))
                    {
                        return i64;
                    }
                    if (el.TryGetDouble(out var d))
                    {
                        return d;
                    }
                    return el.GetRawText();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return null;
            }
        }
    }
}
