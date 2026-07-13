using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Contentstack.Core.Models;

namespace Contentstack.Core.Internals
{
    [CSJsonConverter("EntryJsonConverter")]
    public class EntryJsonConverter : JsonConverter<Entry>
    {
        public override Entry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var jsonObject = JsonNode.Parse(doc.RootElement.GetRawText())!.AsObject();

            var entry = JsonSerializer.Deserialize<Entry>(
                jsonObject.ToJsonString(),
                ContentstackJsonDefaults.ModelDeserializeOnly);

            entry ??= new Entry();
            entry.ParseObject(jsonObject);
            return entry;
        }

        public override void Write(Utf8JsonWriter writer, Entry value, JsonSerializerOptions options)
        {
            // Serialization of Entry is handled outside this converter during normal flows.
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
    }
}
