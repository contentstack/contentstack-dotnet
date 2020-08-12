using System;
using Contentstack.Core;
using Contentstack.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Internals
{
    [CSJsonConverter("AssetJsonConverter")]
    public class AssetJsonConverter : JsonConverter<Asset>
    {
        public override Asset ReadJson(JsonReader reader, Type objectType, Asset existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();
            JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);
            Asset asset = jObject.ToObject<Asset>(Serializer);
            asset.ParseObject(jObject);
            return asset;
        }

        public override void WriteJson(JsonWriter writer, Asset value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
