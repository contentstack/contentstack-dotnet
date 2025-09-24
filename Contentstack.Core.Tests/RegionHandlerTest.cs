using System;
using System.Reflection;
using Xunit;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Microsoft.Extensions.Options;

namespace Contentstack.Core.Tests
{
    public class RegionHandlerTest
    {
        #region ContentstackRegion Enum Tests

        [Theory]
        [InlineData(ContentstackRegion.US, 0)]
        [InlineData(ContentstackRegion.EU, 1)]
        [InlineData(ContentstackRegion.AZURE_EU, 2)]
        [InlineData(ContentstackRegion.AZURE_NA, 3)]
        [InlineData(ContentstackRegion.GCP_NA, 4)]
        [InlineData(ContentstackRegion.AU, 5)]
        public void ContentstackRegion_EnumValues_AreCorrect(ContentstackRegion region, int expectedValue)
        {
            Assert.Equal(expectedValue, (int)region);
        }

        [Fact]
        public void ContentstackRegion_AllValues_AreDefined()
        {
            var regions = Enum.GetValues<ContentstackRegion>();
            Assert.Equal(6, regions.Length);
            Assert.Contains(ContentstackRegion.US, regions);
            Assert.Contains(ContentstackRegion.EU, regions);
            Assert.Contains(ContentstackRegion.AZURE_EU, regions);
            Assert.Contains(ContentstackRegion.AZURE_NA, regions);
            Assert.Contains(ContentstackRegion.GCP_NA, regions);
            Assert.Contains(ContentstackRegion.AU, regions);
        }

        #endregion

        #region ContentstackOptions Region Tests

        [Fact]
        public void ContentstackOptions_Region_DefaultValue_IsUS()
        {
            var options = new ContentstackOptions();
            Assert.Equal(ContentstackRegion.US, options.Region);
        }

        [Theory]
        [InlineData(ContentstackRegion.US)]
        [InlineData(ContentstackRegion.EU)]
        [InlineData(ContentstackRegion.AZURE_EU)]
        [InlineData(ContentstackRegion.AZURE_NA)]
        [InlineData(ContentstackRegion.GCP_NA)]
        [InlineData(ContentstackRegion.AU)]
        public void ContentstackOptions_Region_CanBeSet(ContentstackRegion region)
        {
            var options = new ContentstackOptions();
            options.Region = region;
            Assert.Equal(region, options.Region);
        }

        #endregion

        #region ContentstackClient Region Tests

        [Theory]
        [InlineData(ContentstackRegion.US)]
        [InlineData(ContentstackRegion.EU)]
        [InlineData(ContentstackRegion.AZURE_EU)]
        [InlineData(ContentstackRegion.AZURE_NA)]
        [InlineData(ContentstackRegion.GCP_NA)]
        [InlineData(ContentstackRegion.AU)]
        public void ContentstackClient_Constructor_WithRegion_SetsCorrectRegion(ContentstackRegion region)
        {
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = region
            };

            var client = new ContentstackClient(options);
            
            // Access the private Config field to verify region is set
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var regionProperty = config.GetType().GetProperty("Region");
            var actualRegion = (ContentstackRegion)regionProperty.GetValue(config);
            
            Assert.Equal(region, actualRegion);
        }

        [Theory]
        [InlineData(ContentstackRegion.US)]
        [InlineData(ContentstackRegion.EU)]
        [InlineData(ContentstackRegion.AZURE_EU)]
        [InlineData(ContentstackRegion.AZURE_NA)]
        [InlineData(ContentstackRegion.GCP_NA)]
        [InlineData(ContentstackRegion.AU)]
        public void ContentstackClient_Constructor_WithRegionParameter_SetsCorrectRegion(ContentstackRegion region)
        {
            var client = new ContentstackClient("test_api_key", "test_delivery_token", "test_environment", region: region);
            
            // Access the private Config field to verify region is set
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var regionProperty = config.GetType().GetProperty("Region");
            var actualRegion = (ContentstackRegion)regionProperty.GetValue(config);
            
            Assert.Equal(region, actualRegion);
        }

        [Fact]
        public void ContentstackClient_Constructor_WithOptionsWrapper_SetsCorrectRegion()
        {
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = ContentstackRegion.AU
            };

            var optionsWrapper = new OptionsWrapper<ContentstackOptions>(options);
            var client = new ContentstackClient(optionsWrapper);
            
            // Access the private Config field to verify region is set
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var regionProperty = config.GetType().GetProperty("Region");
            var actualRegion = (ContentstackRegion)regionProperty.GetValue(config);
            
            Assert.Equal(ContentstackRegion.AU, actualRegion);
        }

        [Fact]
        public void ContentstackClient_Constructor_DefaultRegion_IsUS()
        {
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment"
                // Region not set, should default to US
            };

            var client = new ContentstackClient(options);
            
            // Access the private Config field to verify region is set
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var regionProperty = config.GetType().GetProperty("Region");
            var actualRegion = (ContentstackRegion)regionProperty.GetValue(config);
            
            Assert.Equal(ContentstackRegion.US, actualRegion);
        }

        #endregion

        #region Integration Tests

        [Theory]
        [InlineData(ContentstackRegion.US, "https://cdn.contentstack.io/v3")]
        [InlineData(ContentstackRegion.EU, "https://eu-cdn.contentstack.com/v3")]
        [InlineData(ContentstackRegion.AZURE_EU, "https://azure-eu-cdn.contentstack.com/v3")]
        [InlineData(ContentstackRegion.AZURE_NA, "https://azure-na-cdn.contentstack.com/v3")]
        [InlineData(ContentstackRegion.GCP_NA, "https://gcp-na-cdn.contentstack.com/v3")]
        [InlineData(ContentstackRegion.AU, "https://au-cdn.contentstack.com/v3")]
        public void ContentstackClient_Integration_GeneratesCorrectBaseUrl(ContentstackRegion region, string expectedBaseUrl)
        {
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = region
            };

            var client = new ContentstackClient(options);
            
            // Access the private Config field to get BaseUrl
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var baseUrlProperty = config.GetType().GetProperty("BaseUrl");
            var actualBaseUrl = baseUrlProperty.GetValue(config) as string;
            
            Assert.Equal(expectedBaseUrl, actualBaseUrl);
        }

        [Fact]
        public void ContentstackClient_Integration_WithCustomHost_OverridesRegionHost()
        {
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = ContentstackRegion.EU,
                Host = "custom.contentstack.com"
            };

            var client = new ContentstackClient(options);
            
            // Access the private Config field to get BaseUrl
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var baseUrlProperty = config.GetType().GetProperty("BaseUrl");
            var actualBaseUrl = baseUrlProperty.GetValue(config) as string;
            
            // Should use custom host instead of region-specific host
            Assert.Equal("https://eu-custom.contentstack.com/v3", actualBaseUrl);
        }

        [Fact]
        public void ContentstackClient_Integration_WithCustomVersion_AppendsToBaseUrl()
        {
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = ContentstackRegion.AU,
                Version = "v2"
            };

            var client = new ContentstackClient(options);
            
            // Access the private Config field to get BaseUrl
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            var config = configField.GetValue(client);
            var baseUrlProperty = config.GetType().GetProperty("BaseUrl");
            var actualBaseUrl = baseUrlProperty.GetValue(config) as string;
            
            Assert.Equal("https://au-cdn.contentstack.com/v2", actualBaseUrl);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public void ContentstackRegion_Enum_CanBeParsedFromString()
        {
            Assert.True(Enum.TryParse<ContentstackRegion>("US", out var usRegion));
            Assert.Equal(ContentstackRegion.US, usRegion);

            Assert.True(Enum.TryParse<ContentstackRegion>("EU", out var euRegion));
            Assert.Equal(ContentstackRegion.EU, euRegion);

            Assert.True(Enum.TryParse<ContentstackRegion>("AU", out var auRegion));
            Assert.Equal(ContentstackRegion.AU, auRegion);
        }

        [Fact]
        public void ContentstackRegion_Enum_CanBeParsedFromStringIgnoreCase()
        {
            Assert.True(Enum.TryParse<ContentstackRegion>("us", true, out var usRegion));
            Assert.Equal(ContentstackRegion.US, usRegion);

            Assert.True(Enum.TryParse<ContentstackRegion>("eu", true, out var euRegion));
            Assert.Equal(ContentstackRegion.EU, euRegion);

            Assert.True(Enum.TryParse<ContentstackRegion>("au", true, out var auRegion));
            Assert.Equal(ContentstackRegion.AU, auRegion);
        }

        [Fact]
        public void ContentstackRegion_Enum_InvalidString_ReturnsFalse()
        {
            Assert.False(Enum.TryParse<ContentstackRegion>("INVALID", out var invalidRegion));
            Assert.Equal(default(ContentstackRegion), invalidRegion);
        }

        [Fact]
        public void ContentstackOptions_Region_CanBeChangedAfterCreation()
        {
            var options = new ContentstackOptions
            {
                Region = ContentstackRegion.US
            };

            Assert.Equal(ContentstackRegion.US, options.Region);

            options.Region = ContentstackRegion.AU;
            Assert.Equal(ContentstackRegion.AU, options.Region);
        }

        [Fact]
        public void ContentstackClient_WithDifferentRegions_CreatesDifferentInstances()
        {
            var usOptions = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = ContentstackRegion.US
            };

            var auOptions = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                Region = ContentstackRegion.AU
            };

            var usClient = new ContentstackClient(usOptions);
            var auClient = new ContentstackClient(auOptions);

            // Access the private Config field to verify different regions
            var configField = typeof(ContentstackClient).GetField("Config", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var usConfig = configField.GetValue(usClient);
            var usRegionProperty = usConfig.GetType().GetProperty("Region");
            var usRegion = (ContentstackRegion)usRegionProperty.GetValue(usConfig);

            var auConfig = configField.GetValue(auClient);
            var auRegionProperty = auConfig.GetType().GetProperty("Region");
            var auRegion = (ContentstackRegion)auRegionProperty.GetValue(auConfig);

            Assert.NotEqual(usRegion, auRegion);
            Assert.Equal(ContentstackRegion.US, usRegion);
            Assert.Equal(ContentstackRegion.AU, auRegion);
        }

        #endregion
    }
}