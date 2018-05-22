using System;
namespace Contentstack.Core.Internals
{
    public class ConfigNew
    {
        public ConfigNew()
        {
            
        }

        #region Variables
        public string Version { get; private set; } = ContentstackConstantsNew.ApiVersion;
        public string Host { get; set; } = ContentstackConstantsNew.ApiVersion;
        public bool IsSSL { get; private set; } = ContentstackConstantsNew.DefaultHostSSL;

        #endregion
    }
}
