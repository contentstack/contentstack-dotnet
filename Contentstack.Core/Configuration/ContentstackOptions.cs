using System;
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

    }
}
