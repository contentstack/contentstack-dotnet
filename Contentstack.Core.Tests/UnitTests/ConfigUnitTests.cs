using System;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoFixture;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Tests.UnitTests
{
    public class ConfigUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        private Config CreateConfig()
        {
            var configType = typeof(Config);
            return (Config)Activator.CreateInstance(configType, true);
        }

        [Fact]
        public void Port_Get_ReturnsDefault443()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.Port;

            // Assert
            Assert.Equal("443", result);
        }

        [Fact]
        public void Port_Set_UpdatesValue()
        {
            // Arrange
            var config = CreateConfig();
            var port = "8080";

            // Act
            config.Port = port;
            var result = config.Port;

            // Assert
            Assert.Equal(port, result);
        }

        [Fact]
        public void Protocol_Get_ReturnsDefaultHttps()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.Protocol;

            // Assert
            Assert.Equal("https", result);
        }

        [Fact]
        public void Protocol_Set_UpdatesValue()
        {
            // Arrange
            var config = CreateConfig();
            var protocol = "http";

            // Act
            config.Protocol = protocol;
            var result = config.Protocol;

            // Assert
            Assert.Equal(protocol, result);
        }

        [Fact]
        public void Host_Get_ReturnsHostURL()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.Host;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("cdn.contentstack", result);
        }

        [Fact]
        public void Host_Set_UpdatesValue()
        {
            // Arrange
            var config = CreateConfig();
            var host = "custom.host.com";

            // Act
            config.Host = host;
            var result = config.Host;

            // Assert
            Assert.Equal(host, result);
        }

        [Fact]
        public void Version_Get_ReturnsDefaultV3()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.Version;

            // Assert
            Assert.Equal("v3", result);
        }

        [Fact]
        public void Version_Set_UpdatesValue()
        {
            // Arrange
            var config = CreateConfig();
            var version = "v2";

            // Act
            config.Version = version;
            var result = config.Version;

            // Assert
            Assert.Equal(version, result);
        }

        [Fact]
        public void Environment_Get_ReturnsNull()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.Environment;

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Environment_Set_UpdatesValue()
        {
            // Arrange
            var config = CreateConfig();
            var environment = _fixture.Create<string>();

            // Act
            config.Environment = environment;
            var result = config.Environment;

            // Assert
            Assert.Equal(environment, result);
        }

        [Fact]
        public void Branch_Get_ReturnsDefaultMain()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.Branch;

            // Assert
            Assert.Equal("main", result);
        }

        [Fact]
        public void Branch_Set_UpdatesValue()
        {
            // Arrange
            var config = CreateConfig();
            var branch = "develop";

            // Act
            config.Branch = branch;
            var result = config.Branch;

            // Assert
            Assert.Equal(branch, result);
        }

        [Fact]
        public void Timeout_GetSet_Works()
        {
            // Arrange
            var config = CreateConfig();
            var timeout = 5000;

            // Act
            config.Timeout = timeout;
            var result = config.Timeout;

            // Assert
            Assert.Equal(timeout, result);
        }

        [Fact]
        public void Proxy_GetSet_Works()
        {
            // Arrange
            var config = CreateConfig();
            var proxy = new WebProxy("http://proxy.example.com:8080");

            // Act
            config.Proxy = proxy;
            var result = config.Proxy;

            // Assert
            Assert.Equal(proxy, result);
        }

        [Fact]
        public void BaseUrl_WithDefaultValues_ReturnsCorrectUrl()
        {
            // Arrange
            var config = CreateConfig();

            // Act
            var result = config.BaseUrl;

            // Assert
            Assert.Contains("https://", result);
            Assert.Contains("cdn.contentstack", result);
            Assert.Contains("/v3", result);
        }

        [Fact]
        public void BaseUrl_WithCustomValues_ReturnsCorrectUrl()
        {
            // Arrange
            var config = CreateConfig();
            config.Protocol = "http";
            config.Host = "custom.host.com";
            config.Version = "v2";

            // Act
            var result = config.BaseUrl;

            // Assert
            Assert.Contains("http://", result);
            Assert.Contains("custom.host.com", result);
            Assert.Contains("/v2", result);
        }

        [Fact]
        public void BaseUrl_TrimsSlashes()
        {
            // Arrange
            var config = CreateConfig();
            config.Protocol = "https://";
            config.Host = "//host.com/";
            config.Version = "/v3/";

            // Act
            var result = config.BaseUrl;

            // Assert
            Assert.DoesNotContain("//", result.Split('/').Where(s => !string.IsNullOrEmpty(s)));
        }

        [Fact]
        public void HostURL_WithUSRegion_ReturnsIO()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.US;

            // Act
            var hostUrlProperty = typeof(Config).GetProperty("HostURL", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = hostUrlProperty?.GetValue(config) as string;

            // Assert
            Assert.Equal("cdn.contentstack.io", result);
        }

        [Fact]
        public void HostURL_WithEURegion_ReturnsCOM()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.EU;

            // Act
            var hostUrlProperty = typeof(Config).GetProperty("HostURL", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = hostUrlProperty?.GetValue(config) as string;

            // Assert
            Assert.Equal("cdn.contentstack.com", result);
        }

        [Fact]
        public void HostURL_WithAzureEURegion_ReturnsCOM()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.AZURE_EU;

            // Act
            var hostUrlProperty = typeof(Config).GetProperty("HostURL", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = hostUrlProperty?.GetValue(config) as string;

            // Assert
            Assert.Equal("cdn.contentstack.com", result);
        }

        [Fact]
        public void HostURL_WithAzureNARegion_ReturnsCOM()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.AZURE_NA;

            // Act
            var hostUrlProperty = typeof(Config).GetProperty("HostURL", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = hostUrlProperty?.GetValue(config) as string;

            // Assert
            Assert.Equal("cdn.contentstack.com", result);
        }

        [Fact]
        public void HostURL_WithGCPNARegion_ReturnsCOM()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.GCP_NA;

            // Act
            var hostUrlProperty = typeof(Config).GetProperty("HostURL", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = hostUrlProperty?.GetValue(config) as string;

            // Assert
            Assert.Equal("cdn.contentstack.com", result);
        }

        [Fact]
        public void HostURL_WithAURegion_ReturnsCOM()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.AU;

            // Act
            var hostUrlProperty = typeof(Config).GetProperty("HostURL", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = hostUrlProperty?.GetValue(config) as string;

            // Assert
            Assert.Equal("cdn.contentstack.com", result);
        }

        [Fact]
        public void RegionCode_WithUSRegion_ReturnsEmpty()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.US;

            // Act
            var regionCodeMethod = typeof(Config).GetMethod("regionCode", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = regionCodeMethod?.Invoke(config, null) as string;

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void RegionCode_WithEURegion_ReturnsFormattedCode()
        {
            // Arrange
            var config = CreateConfig();
            config.Region = ContentstackRegion.EU;

            // Act
            var regionCodeMethod = typeof(Config).GetMethod("regionCode", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = regionCodeMethod?.Invoke(config, null) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("-", result);
        }

        [Fact]
        public void GetLivePreviewUrl_WithValidConfig_ReturnsUrl()
        {
            // Arrange
            var config = CreateConfig();
            var livePreviewConfig = new LivePreviewConfig
            {
                Host = "preview.host.com"
            };

            // Act
            var getLivePreviewUrlMethod = typeof(Config).GetMethod("getLivePreviewUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = getLivePreviewUrlMethod?.Invoke(config, new object[] { livePreviewConfig }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("preview.host.com", result);
        }

        [Fact]
        public void GetBaseUrl_WithLivePreviewDisabled_ReturnsBaseUrl()
        {
            // Arrange
            var config = CreateConfig();
            var livePreviewConfig = new LivePreviewConfig
            {
                Enable = false
            };

            // Act
            var getBaseUrlMethod = typeof(Config).GetMethod("getBaseUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = getBaseUrlMethod?.Invoke(config, new object[] { livePreviewConfig, "contentType1" }) as string;

            // Assert
            Assert.Equal(config.BaseUrl, result);
        }

        [Fact]
        public void GetBaseUrl_WithLivePreviewEnabled_ReturnsLivePreviewUrl()
        {
            // Arrange
            var config = CreateConfig();
            var livePreviewConfig = new LivePreviewConfig
            {
                Enable = true,
                LivePreview = "token123",
                ContentTypeUID = "contentType1",
                Host = "preview.host.com"
            };

            // Act
            var getBaseUrlMethod = typeof(Config).GetMethod("getBaseUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = getBaseUrlMethod?.Invoke(config, new object[] { livePreviewConfig, "contentType1" }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("preview.host.com", result);
        }

        [Fact]
        public void GetBaseUrl_WithLivePreviewInit_ReturnsBaseUrl()
        {
            // Arrange
            var config = CreateConfig();
            var livePreviewConfig = new LivePreviewConfig
            {
                Enable = true,
                LivePreview = "init",
                ContentTypeUID = "contentType1"
            };

            // Act
            var getBaseUrlMethod = typeof(Config).GetMethod("getBaseUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = getBaseUrlMethod?.Invoke(config, new object[] { livePreviewConfig, "contentType1" }) as string;

            // Assert
            Assert.Equal(config.BaseUrl, result);
        }

        [Fact]
        public void GetBaseUrl_WithDifferentContentTypeUID_ReturnsBaseUrl()
        {
            // Arrange
            var config = CreateConfig();
            var livePreviewConfig = new LivePreviewConfig
            {
                Enable = true,
                LivePreview = "token123",
                ContentTypeUID = "contentType1"
            };

            // Act
            var getBaseUrlMethod = typeof(Config).GetMethod("getBaseUrl", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = getBaseUrlMethod?.Invoke(config, new object[] { livePreviewConfig, "contentType2" }) as string;

            // Assert
            Assert.Equal(config.BaseUrl, result);
        }
    }
}

