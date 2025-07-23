using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Contentstack.Core.Tests
{
    public class AssetTagsBasicTest
    {
        ContentstackClient client = StackConfig.GetStack();

        [Fact]
        public async Task AssetTags_BasicFunctionality_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            assetLibrary.Tags(new string[] { "test" });
            
            Assert.NotNull(assetLibrary);
            
            assetLibrary.Tags(new string[] { "tag1", "tag2", "tag3" });
            Assert.NotNull(assetLibrary);
        }

        [Fact]
        public async Task AssetTags_ChainWithOtherMethods_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            var chainedLibrary = assetLibrary
                .Tags(new string[] { "test" })
                .Limit(1)
                .Skip(0);
                
            Assert.NotNull(chainedLibrary);
        }

        [Fact]
        public async Task AssetTags_NullAndEmptyHandling_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            assetLibrary.Tags(null);
            Assert.NotNull(assetLibrary);
            
            assetLibrary.Tags(new string[] { });
            Assert.NotNull(assetLibrary);
        }

        [Fact]
        public void AssetTags_MethodExists_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            var result = assetLibrary.Tags(new string[] { "test" });
            
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void AssetTags_MultipleCalls_ShouldNotThrowException_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            assetLibrary.Tags(new string[] { "tag1", "tag2" });
            assetLibrary.Tags(new string[] { "tag3", "tag4" });
            assetLibrary.Tags(new string[] { "newtag1", "newtag2", "newtag3" });
            
            Assert.IsType<AssetLibrary>(assetLibrary);
        }
    }
} 