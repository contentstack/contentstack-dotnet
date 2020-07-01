using System;
namespace Contentstack.Core.Tests.Models
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContentstackConverterAttribute: Attribute
    {
        internal string name;
        public ContentstackConverterAttribute(string name)
        {
            this.name = name;
        }
    }

    [ContentstackConverter("name")]
    public class NumberContentType
    {
        public string Uid;
        public string Title;
        public double num_field;
    }
}