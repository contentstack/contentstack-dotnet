using System;
using System.Collections.Generic;
using Contentstack.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Internals
{
    [CSJsonConverter("EntryJsonConverter")]
    public class EntryJsonConverter : JsonConverter<Entry>
    {
        public override Entry ReadJson(JsonReader reader, Type objectType, Entry existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();
            JsonSerializer Serializer = JsonSerializer.Create(SerializerSettings);
            Entry entry = jObject.ToObject<Entry>(Serializer);
            entry.ParseObject(jObject);
            return entry;
        }

        public override void WriteJson(JsonWriter writer, Entry value, JsonSerializer serializer)
        {

        }
    }
}
