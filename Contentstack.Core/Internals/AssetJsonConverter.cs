using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Core.Models;

namespace Contentstack.Core.Internals
{
    [CSJsonConverter("AssetJsonConverter")]
    public class AssetJsonConverter : JsonConverter<Asset>
    {
        public override Asset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = document.RootElement;
                Asset asset = JsonSerializer.Deserialize<Asset>(root.GetRawText(), options);
                asset.ParseObject(root);
                return asset;
            }
        }

        public override void Write(Utf8JsonWriter writer, Asset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}