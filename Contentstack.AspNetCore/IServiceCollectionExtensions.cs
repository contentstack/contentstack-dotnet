using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Contentstack.AspNetCore
{
    /// <summary>
    /// Extension methods for IServiceCollection.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Contentstack services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The IConfigurationRoot used to retrieve configuration from.</param>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddContentstack(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddOptions();
            services.Configure<ContentstackOptions>(configuration.GetSection("ContentstackOptions"));
            services.TryAddSingleton<HttpClient>();
            services.TryAddTransient<ContentstackClient>();
            return services;
        }

        /// <summary>
        /// Adds Contentstack services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection.</param>
        /// <param name="configuration">The IConfiguration used to retrieve configuration from.</param>
        /// <returns>The IServiceCollection.</returns>
        public static IServiceCollection AddContentstack(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ContentstackOptions>(configuration.GetSection("ContentstackOptions"));
            services.TryAddSingleton<HttpClient>();
            services.TryAddTransient<ContentstackClient>();
            return services;
        }
    }
}