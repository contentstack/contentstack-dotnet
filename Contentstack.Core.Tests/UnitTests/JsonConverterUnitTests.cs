using System;
using System.IO;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace Contentstack.Core.Tests.UnitTests
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
        public void ReadJson_WithValidJson_ReturnsEntry()
        {
            // Arrange
            var converter = new EntryJsonConverter();
            var json = "{\"uid\":\"test_uid\",\"title\":\"Test Title\"}";
            var reader = new JsonTextReader(new StringReader(json));

            // Act
            var entry = converter.ReadJson(reader, typeof(Entry), null, JsonSerializer.CreateDefault());

            // Assert
            Assert.NotNull(entry);
            Assert.IsType<Entry>(entry);
        }

        [Fact]
        public void WriteJson_WithEntry_DoesNothing()
        {
            // Arrange
            var converter = new EntryJsonConverter();
            var client = CreateClient();
            var contentType = client.ContentType("test_content_type");
            var entry = contentType.Entry("test_uid");
            var writer = new JsonTextWriter(new StringWriter());

            // Act - Should not throw
            converter.WriteJson(writer, entry, JsonSerializer.CreateDefault());

            // Assert
            Assert.True(true);
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
        public void ReadJson_WithValidJson_ReturnsAsset()
        {
            // Arrange
            var converter = new AssetJsonConverter();
            var json = "{\"uid\":\"test_uid\",\"title\":\"Test Asset\"}";
            var reader = new JsonTextReader(new StringReader(json));

            // Act
            var asset = converter.ReadJson(reader, typeof(Asset), null, JsonSerializer.CreateDefault());

            // Assert
            Assert.NotNull(asset);
            Assert.IsType<Asset>(asset);
        }

        [Fact]
        public void WriteJson_WithAsset_ThrowsNotImplementedException()
        {
            // Arrange
            var converter = new AssetJsonConverter();
            var client = CreateClient();
            var asset = client.Asset("test_uid");
            var writer = new JsonTextWriter(new StringWriter());

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => 
                converter.WriteJson(writer, asset, JsonSerializer.CreateDefault()));
        }
    }
}

