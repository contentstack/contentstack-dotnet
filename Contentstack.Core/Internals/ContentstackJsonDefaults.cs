using System.Text.Json;
using System.Text.Json.Serialization;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Options used inside custom converters when deserializing POCO properties without re-entering SDK converters (matches former Newtonsoft empty <see cref="JsonSerializer"/> usage).
    /// </summary>
    internal static class ContentstackJsonDefaults
    {
        internal static readonly JsonSerializerOptions ModelDeserializeOnly = CreateModelDeserializeOnly();

        private static JsonSerializerOptions CreateModelDeserializeOnly()
        {
            var o = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            return o;
        }
    }
}
