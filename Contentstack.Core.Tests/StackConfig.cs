using System;
using Contentstack.Core;
using Contentstack.Core.Models;
using System.Configuration;

namespace Contentstack.Core.Tests
{

    public class StackConfig
    {
        Configuration currentConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        Configuration assemblyConfiguration
        {
            get
            {
                return ConfigurationManager.OpenExeConfiguration(new Uri(uriString: GetType().Assembly.CodeBase).LocalPath);
            }
        }


        public static Stack GetStack()
        {
            StackConfig config = new StackConfig();
            if (config.assemblyConfiguration.HasFile && string.Compare(config.assemblyConfiguration.FilePath, config.currentConfiguration.FilePath, true) != 0)
            {
                config.assemblyConfiguration.SaveAs(config.currentConfiguration.FilePath);
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            string apiKey = ConfigurationManager.AppSettings["api_key"];
            string accessToken = ConfigurationManager.AppSettings["access_token"];
            string environment = ConfigurationManager.AppSettings["environment"];

            return ContentstackClient.Stack(apiKey, accessToken, environment);

        }
    }
}
