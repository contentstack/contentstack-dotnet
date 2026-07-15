using System;
using System.Collections.Generic;
using System.IO;
using Contentstack.Core.Endpoints;
using Xunit;

namespace Contentstack.Core.Tests
{
    /// <summary>
    /// Tests for <see cref="Endpoint"/> — dynamic CDN-backed region resolution.
    /// Mirrors the coverage from contentstack-utils-dotnet PR #66 EndpointTest.cs,
    /// adapted for the Delivery SDK (contentDelivery as primary service key).
    /// </summary>
    public class EndpointTest : IDisposable
    {
        public EndpointTest()
        {
            // Each test starts with a clean cache so CDN/disk state doesn't bleed across tests.
            Endpoint.ResetCache();
        }

        public void Dispose()
        {
            Endpoint.ResetCache();
        }

        // ── Basic resolution ──────────────────────────────────────────────────

        [Fact]
        public void GetContentstackEndpoint_Na_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("na", "contentDelivery");
            Assert.Equal("https://cdn.contentstack.io", url);
        }

        [Fact]
        public void GetContentstackEndpoint_Eu_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("eu", "contentDelivery");
            Assert.Equal("https://eu-cdn.contentstack.com", url);
        }

        [Fact]
        public void GetContentstackEndpoint_Au_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("au", "contentDelivery");
            Assert.Equal("https://au-cdn.contentstack.com", url);
        }

        [Fact]
        public void GetContentstackEndpoint_AzureNa_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("azure-na", "contentDelivery");
            Assert.Equal("https://azure-na-cdn.contentstack.com", url);
        }

        [Fact]
        public void GetContentstackEndpoint_AzureEu_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("azure-eu", "contentDelivery");
            Assert.Equal("https://azure-eu-cdn.contentstack.com", url);
        }

        [Fact]
        public void GetContentstackEndpoint_GcpNa_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("gcp-na", "contentDelivery");
            Assert.Equal("https://gcp-na-cdn.contentstack.com", url);
        }

        [Fact]
        public void GetContentstackEndpoint_GcpEu_ReturnsCorrectCdnUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("gcp-eu", "contentDelivery");
            Assert.Equal("https://gcp-eu-cdn.contentstack.com", url);
        }

        // ── NA alias resolution ───────────────────────────────────────────────

        [Theory]
        [InlineData("na")]
        [InlineData("us")]
        [InlineData("NA")]
        [InlineData("US")]
        [InlineData("AWS-NA")]
        [InlineData("aws_na")]
        [InlineData("AWS_NA")]
        public void GetContentstackEndpoint_NaAliasVariants_AllResolveToSameCdn(string alias)
        {
            string url = Endpoint.GetContentstackEndpoint(alias, "contentDelivery");
            Assert.Equal("https://cdn.contentstack.io", url);
        }

        // ── omitHttps flag ────────────────────────────────────────────────────

        [Fact]
        public void GetContentstackEndpoint_OmitHttps_ReturnsHostOnly()
        {
            string host = Endpoint.GetContentstackEndpoint("na", "contentDelivery", omitHttps: true);
            Assert.Equal("cdn.contentstack.io", host);
        }

        [Fact]
        public void GetContentstackEndpoint_OmitHttps_Eu_ReturnsHostOnly()
        {
            string host = Endpoint.GetContentstackEndpoint("eu", "contentDelivery", omitHttps: true);
            Assert.Equal("eu-cdn.contentstack.com", host);
        }

        [Fact]
        public void GetContentstackEndpoint_OmitHttpsFalse_ReturnsFullUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("na", "contentDelivery", omitHttps: false);
            Assert.StartsWith("https://", url);
        }

        // ── Management endpoint (also valid for Delivery SDK to know) ─────────

        [Fact]
        public void GetContentstackEndpoint_Na_ContentManagement_ReturnsApiUrl()
        {
            string url = Endpoint.GetContentstackEndpoint("na", "contentManagement");
            Assert.Equal("https://api.contentstack.io", url);
        }

        // ── Dictionary overload ───────────────────────────────────────────────

        [Fact]
        public void GetContentstackEndpoint_DictionaryOverload_Na_ContainsContentDelivery()
        {
            Dictionary<string, string> all = Endpoint.GetContentstackEndpoint("na");
            Assert.True(all.ContainsKey("contentDelivery"));
            Assert.Equal("https://cdn.contentstack.io", all["contentDelivery"]);
        }

        [Fact]
        public void GetContentstackEndpoint_DictionaryOverload_Na_HasExpectedKeyCount()
        {
            Dictionary<string, string> all = Endpoint.GetContentstackEndpoint("na");
            Assert.True(all.Count >= 18, $"Expected at least 18 service keys, got {all.Count}");
        }

        [Fact]
        public void GetContentstackEndpoint_DictionaryOverload_OmitHttps_AllValuesLackScheme()
        {
            Dictionary<string, string> all = Endpoint.GetContentstackEndpoint("na", omitHttps: true);
            foreach (var kvp in all)
                Assert.DoesNotMatch(@"^https?://", kvp.Value);
        }

        [Fact]
        public void GetContentstackEndpoint_DictionaryOverload_Eu_ContainsContentDelivery()
        {
            Dictionary<string, string> all = Endpoint.GetContentstackEndpoint("eu");
            Assert.True(all.ContainsKey("contentDelivery"));
        }

        // ── Error handling ────────────────────────────────────────────────────

        [Fact]
        public void GetContentstackEndpoint_EmptyRegion_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                Endpoint.GetContentstackEndpoint("", "contentDelivery"));
        }

        [Fact]
        public void GetContentstackEndpoint_WhitespaceRegion_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                Endpoint.GetContentstackEndpoint("   ", "contentDelivery"));
        }

        [Fact]
        public void GetContentstackEndpoint_UnknownRegion_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                Endpoint.GetContentstackEndpoint("invalid-region-xyz", "contentDelivery"));
        }

        [Fact]
        public void GetContentstackEndpoint_UnknownService_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                Endpoint.GetContentstackEndpoint("na", "nonExistentService"));
        }

        [Fact]
        public void GetContentstackEndpoint_DictionaryOverload_EmptyRegion_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                Endpoint.GetContentstackEndpoint("", omitHttps: false));
        }

        [Fact]
        public void GetContentstackEndpoint_DictionaryOverload_UnknownRegion_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() =>
                Endpoint.GetContentstackEndpoint("invalid-region-xyz"));
        }

        // ── Cache consistency ─────────────────────────────────────────────────

        [Fact]
        public void GetContentstackEndpoint_CalledTwice_ReturnsSameResult()
        {
            string url1 = Endpoint.GetContentstackEndpoint("na", "contentDelivery");
            string url2 = Endpoint.GetContentstackEndpoint("na", "contentDelivery");
            Assert.Equal(url1, url2);
        }

        [Fact]
        public void ResetCache_ThenResolve_StillReturnsCorrectUrl()
        {
            // Warm the cache
            Endpoint.GetContentstackEndpoint("na", "contentDelivery");

            // Reset and resolve again
            Endpoint.ResetCache();
            string url = Endpoint.GetContentstackEndpoint("na", "contentDelivery");
            Assert.Equal("https://cdn.contentstack.io", url);
        }

        // ── Local file path ───────────────────────────────────────────────────

        [Fact]
        public void GetLocalFilePath_ContainsAssetsAndRegionsJson()
        {
            string path = Endpoint.GetLocalFilePath();
            Assert.Contains("Assets", path);
            Assert.EndsWith("regions.json", path);
        }

        [Fact]
        public void GetLocalFilePath_PathIsAbsolute()
        {
            string path = Endpoint.GetLocalFilePath();
            Assert.True(Path.IsPathRooted(path), $"Expected absolute path, got: {path}");
        }

        // ── Local file self-heal ──────────────────────────────────────────────

        [Fact]
        public void GetContentstackEndpoint_WritesLocalFile_AfterCdnDownload()
        {
            // Delete local file to force CDN download
            string localFile = Endpoint.GetLocalFilePath();
            if (File.Exists(localFile))
                File.Delete(localFile);

            Endpoint.ResetCache();

            // This call should trigger CDN download and write the file
            Endpoint.GetContentstackEndpoint("na", "contentDelivery");

            Assert.True(File.Exists(localFile), $"Expected regions.json to be written to: {localFile}");
        }

        // ── URL format validation ─────────────────────────────────────────────

        [Theory]
        [InlineData("na")]
        [InlineData("eu")]
        [InlineData("au")]
        [InlineData("azure-na")]
        [InlineData("azure-eu")]
        [InlineData("gcp-na")]
        [InlineData("gcp-eu")]
        public void GetContentstackEndpoint_AllRegions_ContentDelivery_StartsWithHttps(string region)
        {
            string url = Endpoint.GetContentstackEndpoint(region, "contentDelivery");
            Assert.StartsWith("https://", url);
        }

        [Theory]
        [InlineData("na")]
        [InlineData("eu")]
        [InlineData("au")]
        [InlineData("azure-na")]
        [InlineData("azure-eu")]
        [InlineData("gcp-na")]
        [InlineData("gcp-eu")]
        public void GetContentstackEndpoint_AllRegions_OmitHttps_DoesNotStartWithHttps(string region)
        {
            string host = Endpoint.GetContentstackEndpoint(region, "contentDelivery", omitHttps: true);
            Assert.DoesNotMatch(@"^https?://", host);
        }
    }
}
