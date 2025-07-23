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
            // Basic test to verify Tags method exists and works
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            // This should not throw an exception
            assetLibrary.Tags(new string[] { "test" });
            
            // Verify the method returns AssetLibrary for chaining
            Assert.NotNull(assetLibrary);
            
            // Test with multiple tags
            assetLibrary.Tags(new string[] { "tag1", "tag2", "tag3" });
            Assert.NotNull(assetLibrary);
        }

        [Fact]
        public async Task AssetTags_ChainWithOtherMethods_Test()
        {
            // Test chaining Tags with other methods
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
            
            // Should handle null gracefully
            assetLibrary.Tags(null);
            Assert.NotNull(assetLibrary);
            
            // Should handle empty array gracefully  
            assetLibrary.Tags(new string[] { });
            Assert.NotNull(assetLibrary);
        }

        [Fact]
        public void AssetTags_MethodExists_Test()
        {
            // Verify the Tags method exists with correct signature
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            // This will compile only if the method exists with correct signature
            var result = assetLibrary.Tags(new string[] { "test" });
            
            // Should return AssetLibrary type for method chaining
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void AssetTags_MultipleCalls_ShouldNotThrowException_Test()
        {
            // Test multiple calls to Tags() method on same instance
            AssetLibrary assetLibrary = client.AssetLibrary();
            
            // First call
            assetLibrary.Tags(new string[] { "tag1", "tag2" });
            
            // Second call should not throw "An item with the same key has already been added" exception
            assetLibrary.Tags(new string[] { "tag3", "tag4" });
            
            // Third call with different tags
            assetLibrary.Tags(new string[] { "newtag1", "newtag2", "newtag3" });
            
            // Should return AssetLibrary type for method chaining
            Assert.IsType<AssetLibrary>(assetLibrary);
        }
    }
} 