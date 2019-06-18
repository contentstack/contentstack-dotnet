using System;
using System.Collections.Generic;
using Contentstack.Core.Models;
using Markdig;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        public List<string> Reference;
        public List<string> Other_reference;
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

    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.SSSZ";
        }
    }
}
