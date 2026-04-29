using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    public class EntryJsonConverterUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>()
            };
            return new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        [Fact]
        public void Read_WithValidJson_ReturnsEntry()
        {
            var converter = new EntryJsonConverter();
            var json = "{\"uid\":\"test_uid\",\"title\":\"Test Title\"}";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();

            var entry = converter.Read(ref reader, typeof(Entry), CreateClient().SerializerOptions);

            Assert.NotNull(entry);
            Assert.IsType<Entry>(entry);
        }

        [Fact]
        public void Write_WithEntry_Completes()
        {
            var converter = new EntryJsonConverter();
            var client = CreateClient();
            var contentType = client.ContentType("test_content_type");
            var entry = contentType.Entry("test_uid");
            var buffer = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(buffer))
            {
                converter.Write(writer, entry, client.SerializerOptions);
            }

            Assert.True(buffer.WrittenCount > 0);
        }
    }

    public class AssetJsonConverterUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>()
            };
            return new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        [Fact]
        public void Read_WithValidJson_ReturnsAsset()
        {
            var converter = new AssetJsonConverter();
            var json = "{\"uid\":\"test_uid\",\"title\":\"Test Asset\"}";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();

            var asset = converter.Read(ref reader, typeof(Asset), CreateClient().SerializerOptions);

            Assert.NotNull(asset);
            Assert.IsType<Asset>(asset);
        }

        [Fact]
        public void Write_WithAsset_ThrowsAssetException()
        {
            var converter = new AssetJsonConverter();
            var client = CreateClient();
            var asset = client.Asset("test_uid");
            var buffer = new ArrayBufferWriter<byte>();

            Assert.Throws<AssetException>(() =>
            {
                using (var writer = new Utf8JsonWriter(buffer))
                {
                    converter.Write(writer, asset, client.SerializerOptions);
                }
            });
        }
    }
}
