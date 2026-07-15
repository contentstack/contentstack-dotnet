using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Contentstack.Core.Models;

namespace Contentstack.Core.Internals
{
    [CSJsonConverter("AssetJsonConverter")]
    public class AssetJsonConverter : JsonConverter<Asset>
    {
        public override Asset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var jsonObject = JsonNode.Parse(doc.RootElement.GetRawText())!.AsObject();

            var asset = JsonSerializer.Deserialize<Asset>(
                jsonObject.ToJsonString(),
                ContentstackJsonDefaults.ModelDeserializeOnly);

            asset ??= new Asset();
            asset.ParseObject(jsonObject);
            return asset;
        }

        public override void Write(Utf8JsonWriter writer, Asset value, JsonSerializerOptions options)
        {
            throw AssetException.CreateForJsonConversionError();
        }
    }
}
