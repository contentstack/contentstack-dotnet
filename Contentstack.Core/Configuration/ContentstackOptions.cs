using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Configuration
{
    /// <summary>
    /// Represents a set of options to configure a Stack.
    /// </summary>
    public class ContentstackOptions
    {
        /// <summary>
        /// The api key used when communicating with the ContentStack API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The access token used when communicating with the ContentStack API.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The environment used when communicating with the ContentStack API.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// The Host used to set host url for the ContentStack API.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The Region used to set region for the ContentStack API.
        /// </summary>
        [TypeConverter(typeof(ContentstackRegionConverter))]
        public ContentstackRegion Region { get; set; } = ContentstackRegion.US;

        /// <summary>
        /// The Version number for the ContentStack API.
        /// </summary>
        public string Version { get; set; }
    }

    internal class ContentstackRegionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string stringValue;
            object result;

            result = null;
            stringValue = value as string;

            if (!string.IsNullOrEmpty(stringValue))
            {
                int nonDigitIndex;

                nonDigitIndex = stringValue.IndexOf(stringValue.FirstOrDefault(char.IsLetter));

                if (nonDigitIndex > 0)
                {
                    result = (ContentstackRegion)Enum.Parse(typeof(ContentstackRegion), stringValue.Substring(nonDigitIndex), true);
                }
            }

            return result ?? base.ConvertFrom(context, culture, value);
        }
    }
}

