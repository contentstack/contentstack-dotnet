using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contentstack.Core.Models;
using Markdig;

namespace Contentstack.Core.Tests.Models
{
    public class SourceModel
    {
        public string Uid;
        public string Title;
        public string Url;
        public string Markdown;
        public double? Number;
        public Boolean Boolean;
        public string Date;
        public Asset file;
        public List<Object> Reference;
        public List<object> Other_reference;
        public Dictionary<string, object> Group;
        public List<Dictionary<string, object>> Modular_blocks;
        public object[] Tags;
        public DateTime Created_at;
        public string created_by;
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime updated_at;
        public string Updated_by;

        public String GetHTMLText()
        {
            string result = string.Empty;
            if (this.Markdown != null)
            {
                try
                {
                    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    result = Markdig.Markdown.ToHtml(this.Markdown, pipeline);
                    return result;
                }
                catch
                { }
            }
            return result;
        }
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd'T'HH:mm:ss.SSSZ";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrEmpty(s))
            {
                return default;
            }
            return DateTime.ParseExact(s, Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}
