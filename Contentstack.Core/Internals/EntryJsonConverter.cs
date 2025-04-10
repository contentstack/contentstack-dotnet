using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Core.Models;

namespace Contentstack.Core.Internals
{
    [CSJsonConverter("EntryJsonConverter")]
    public class EntryJsonConverter : JsonConverter<Entry>
    {
        public override Entry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = document.RootElement;
                Entry entry = JsonSerializer.Deserialize<Entry>(root.GetRawText(), options);
                entry.ParseObject(root);
                return entry;
            }
        }

        public override void Write(Utf8JsonWriter writer, Entry value, JsonSerializerOptions options)
        {

        }
    }
}
