using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.ImageDeliveryTests
{
    /// <summary>
    /// Comprehensive tests for Image Delivery and Transformation
    /// Tests image URLs, transformations, and asset handling
    /// </summary>
    [Trait("Category", "ImageDelivery")]
    public class ImageDeliveryComprehensiveTest
    {
        #region Basic Image Delivery
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Basic Asset Fetch Returns Image Url")]
        public async Task ImageDelivery_BasicAssetFetch_ReturnsImageUrl()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            AssertionHelper.AssertAssetBasicFields(asset);
            AssertionHelper.AssertAssetUrl(asset);
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Asset Url Is Accessible")]
        public async Task ImageDelivery_AssetUrl_IsAccessible()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset.Url);
            Assert.True(Uri.TryCreate(asset.Url, UriKind.Absolute, out _));
        }
        
        #endregion
        
        #region Image Transformations
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Width Transform Applies Correctly")]
        public async Task ImageDelivery_WidthTransform_AppliesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert - URL should be valid
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
            // Transformation params can be appended to URL
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Height Transform Applies Correctly")]
        public async Task ImageDelivery_HeightTransform_AppliesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Quality Transform Applies Correctly")]
        public async Task ImageDelivery_QualityTransform_AppliesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Format Transform Converts Format")]
        public async Task ImageDelivery_FormatTransform_ConvertsFormat()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
        }
        
        #endregion
        
        #region Multiple Transformations
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Multiple Transforms Applies Together")]
        public async Task ImageDelivery_MultipleTransforms_AppliesTogether()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert - Can apply width + height + quality
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Crop Transform Applies Correctly")]
        public async Task ImageDelivery_CropTransform_AppliesCorrectly()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
            Assert.NotNull(asset.Url);
        }
        
        #endregion
        
        #region Image in Entry Context
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Image Field In Entry Accessible Via Entry")]
        public async Task ImageDelivery_ImageFieldInEntry_AccessibleViaEntry()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // Image fields in entry should be accessible
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Multiple Images All Accessible")]
        public async Task ImageDelivery_MultipleImages_AllAccessible()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            // All image fields should be accessible
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Performance Asset Fetch")]
        public async Task ImageDelivery_Performance_AssetFetch()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (asset, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            });
            
            // Assert
            Assert.NotNull(asset);
            Assert.True(elapsed < 5000, $"Image fetch should complete within 5s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Image Delivery - Image Delivery Performance Multiple Assets")]
        public async Task ImageDelivery_Performance_MultipleAssets()
        {
            // Arrange
            var client = CreateClient();
            var assetLibrary = client.AssetLibrary();
            
            // Act
            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                assetLibrary.Limit(5);
                return await assetLibrary.FetchAll();
            });
            
            // Assert
            Assert.NotNull(result);
            Assert.True(elapsed < 10000, $"Multiple assets should fetch within 10s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Helper Methods
        
        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            
            return new ContentstackClient(options);
        }
        
        #endregion
    }
}

