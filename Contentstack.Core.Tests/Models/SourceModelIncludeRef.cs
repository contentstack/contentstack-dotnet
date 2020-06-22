using System;
using System.Collections.Generic;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests.Models
{
    public class SourceModelIncludeRef
    {
        public string Uid;
        public string Title;
        public string Url;
        public string Markdown;
        public double? Number;
        public Asset file;
        public Boolean Boolean;
        public string Date;
        public List<Entry> Reference;
        public List<object> Other_reference;
        // public List<Dictionary<string, object>> Other_reference;
        public Dictionary<string, object> Group;
        public List<Dictionary<string, object>> Modular_blocks;

    }
    public class SourceModelIncludeRefAndOther
    {
        public string Uid;
        public string Title;
        public string Url;
        public string Markdown;
        public Double Number;
        public Boolean Boolean;
        public Asset File;
        public string Date;
        public List<Dictionary<string, object>> Reference;
        public List<Dictionary<string, object>> Other_reference;
        public Dictionary<string, object> Group;
        public List<Dictionary<string, object>> Modular_blocks;
    }
}
