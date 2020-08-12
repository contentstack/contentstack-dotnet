using System;
using Contentstack.Core;
using Contentstack.Core.Models;
using System.Configuration;
using Microsoft.Extensions.Options;

namespace Contentstack.Core.Tests
{

    public class StackConfig
    {
        System.Configuration.Configuration currentConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        System.Configuration.Configuration assemblyConfiguration
        {
            get
            {
                return ConfigurationManager.OpenExeConfiguration(new Uri(uriString: GetType().Assembly.CodeBase).LocalPath);
            }
        }

        public static ContentstackClient GetStack()
        {
            StackConfig config = new StackConfig();
            if (config.assemblyConfiguration.HasFile && string.Compare(config.assemblyConfiguration.FilePath, config.currentConfiguration.FilePath, true) != 0)
            {
                config.assemblyConfiguration.SaveAs(config.currentConfiguration.FilePath);
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            string apiKey = ConfigurationManager.AppSettings["api_key"];
            string delivery_token = ConfigurationManager.AppSettings["delivery_token"];
            string environment = ConfigurationManager.AppSettings["environment"];
            string host = ConfigurationManager.AppSettings["host"];
            Configuration.ContentstackOptions contentstackOptions = new Configuration.ContentstackOptions
            {
                ApiKey = apiKey,
                DeliveryToken = delivery_token,
                Environment = environment,
                Host = host,
            };

            ContentstackClient contentstackClient = new ContentstackClient(new OptionsWrapper<Configuration.ContentstackOptions>(contentstackOptions));
            return contentstackClient;

        }
    }
}
