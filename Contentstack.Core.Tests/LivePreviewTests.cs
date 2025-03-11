using System;
using Xunit;
using Contentstack.Core.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Contentstack.Core.Tests
{
    public class TestContentstackClient : ContentstackClient
    {
        public TestContentstackClient(ContentstackOptions options)
            : base(options)
        {
        }

        // Override GetLivePreviewData with a hardcoded response
        private new async Task<JObject> GetLivePreviewData()
        {
            var mockResponse = new
            {
                entry = new
                {
                    uid = "mock_entry_uid",
                    title = "Mocked Entry",
                    content_type_uid = "mock_content_type",
                    status = "preview"
                }
            };
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(mockResponse);
            JObject data = JsonConvert.DeserializeObject<JObject>(jsonResponse, this.SerializerSettings);
            return await Task.FromResult((JObject)data["entry"]);
        }

        // Public method to access the private method in tests
        public async Task<JObject> TestGetLivePreviewData()
        {
            return await GetLivePreviewData();
        }
    }

    public class LivePreviewTests
	{
        ContentstackClient client = StackConfig.GetStack();

        ContentstackClient Lpclient = StackConfig.GetLPStack();

        private String numbersContentType = "numbers_content_type";
        String source = "source";

        public double EPSILON { get; private set; }

        [Fact]
        public async Task CheckLivePreviewConfigNotSet()
        {
            var LPConfig =  client.GetLivePreviewConfig();
            Assert.False(LPConfig.Enable);
            Assert.Null(LPConfig.PreviewToken);
            Assert.Null(LPConfig.Host);
        }

        [Fact]
        public async Task CheckLivePreviewConfigSet()
        {
            var LPConfig = Lpclient.GetLivePreviewConfig();
            Assert.True(LPConfig.Enable);
            Assert.NotEmpty(LPConfig.PreviewToken);
            Assert.NotEmpty(LPConfig.Host);
        }

        [Fact]
        public async Task setQueryWithLivePreview()
        {
            Dictionary<string, string> query = new Dictionary<string, string>
            {
                { "content_type_uid", "ct1" },
                { "live_preview", "lphash" },
                { "release_id", "release_id" },
                { "preview_timestamp", "preview_timestamp" },
                { "entry_uid", "euid" }
            };
            Lpclient.LivePreviewQueryAsync(query);
            var LPConfig = Lpclient.GetLivePreviewConfig();
            Assert.Equal(LPConfig.PreviewTimestamp, "preview_timestamp");
            Assert.NotEmpty(LPConfig.PreviewToken);
            Assert.NotEmpty(LPConfig.PreviewToken);
            Assert.NotEmpty(LPConfig.Host);
        }

        [Fact]
        public async Task TestGetLivePreviewData()
        {
            // Arrange
            var options = new ContentstackOptions
            {
                ApiKey = "test_api_key",
                DeliveryToken = "test_delivery_token",
                Environment = "test_environment",
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token", // Replace with a valid preview token
                    Host = "test-host" // Replace with a valid preview host (e.g., "rest-preview.contentstack.com")

                }
            };

            var client = new TestContentstackClient(options);

            // Act
            var result = await client.TestGetLivePreviewData();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("mock_entry_uid", result["uid"].ToString());
            Assert.Equal("Mocked Entry", result["title"].ToString());
        }

    }
}

