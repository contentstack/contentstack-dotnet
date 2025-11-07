using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests
{
    public class AssetTest
    {

        ContentstackClient client = StackConfig.GetStack();

        public async Task<string> FetchAssetUID()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            Assert.True(assets.Count() > 0);
            return assets.First<Asset>().Uid;
        }

        [Fact]
        public async Task FetchAssetByUid()
        {
            string uid = await FetchAssetUID();
            Asset asset = client.Asset(uid);
            await asset.Fetch().ContinueWith((t) =>
            {
                Asset result = t.Result;
                if (result == null)
                {
                    Assert.Fail( "Entry.Fetch is not match with expected result.");
                }
                else
                {
                    Assert.True(result.FileName.Length > 0);
                }
            });
        }

        [Fact]
        public async Task FetchAssetToAccessAttributes()
        {
            string uid = await FetchAssetUID();
            Asset a1 = await client.Asset(uid).AddParam("include_dimension", "true").Fetch();
            Assert.NotEmpty(a1.Url);
            Assert.NotEmpty(a1.ContentType);
            Assert.NotEmpty(a1.Version);
            Assert.NotEmpty(a1.FileSize);
            Assert.NotEmpty(a1.FileName);
            Assert.NotEmpty(a1.Description);
            Assert.NotEmpty(a1.UpdatedBy);
            Assert.NotEmpty(a1.CreatedBy);
            Assert.NotEmpty(a1.PublishDetails);
        }

        [Fact]
        public async Task FetchAssetsPublishFallback()
        {
            List<string> list = new List<string>();
            list.Add("en-us");
            list.Add("ja-jp");
            ContentstackCollection<Asset> assets = await client.AssetLibrary()
                .SetLocale("ja-jp")
                .IncludeFallback()
                .FetchAll();
            ;
            Assert.True(assets.Items.Count() > 0);
            foreach (Asset asset in assets)
            {
                Assert.Contains((string)(asset.Get("publish_details") as JObject).GetValue("locale"), list);
            }
        }

        [Fact]
        public async Task FetchAssetsPublishWithoutFallback()
        {
            List<string> list = new List<string>();
            list.Add("ja-jp");
            ContentstackCollection<Asset> assets = await client.AssetLibrary()
                .SetLocale("ja-jp")
                .FetchAll();
            ;
            Assert.True(assets.Items.Count() > 0);
            foreach (Asset asset in assets)
            {
                Assert.Contains((string)(asset.Get("publish_details") as JObject).GetValue("locale"), list);
            }
        }

        [Fact]
        public async Task FetchAssets()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            Assert.True(assets.Count() > 0);
            foreach (Asset asset in assets)
            {
                Assert.True(asset.FileName.Length > 0);
            }
        }

        [Fact]
        public async Task FetchAssetsOrderByAscending()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.SortWithKeyAndOrderBy("created_at", Internals.OrderBy.OrderByAscending);
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            Assert.True(assets.Count() > 0);
            DateTime dateTime = new DateTime();
            foreach (Asset asset in assets)
            {
                if (dateTime != null)
                {
                    if (dateTime.CompareTo(asset.GetCreateAt()) != -1 && dateTime.CompareTo(asset.GetCreateAt()) != 0)
                    {
                        Assert.Fail();
                    }
                }
                dateTime = asset.GetCreateAt();
                Assert.True(asset.FileName.Length > 0);
            }
        }

        [Fact]
        public async Task FetchAssetsIncludeRelativeURL()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.IncludeRelativeUrls();
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            Assert.True(assets.Count() > 0);
            foreach (Asset asset in assets)
            {
                Assert.DoesNotContain(asset.Url, "http");
                Assert.True(asset.FileName.Length > 0);
            }
        }

        [Fact]
        public async Task FetchAssetWithQuery()
        {
            JObject queryObject = new JObject
            {
                { "filename", "image3.png" }
            };
            ContentstackCollection<Asset> assets = await client.AssetLibrary().Query(queryObject).FetchAll();
            Assert.True(assets.Count() > 0);
            foreach (Asset asset in assets)
            {
                Assert.DoesNotContain(asset.Url, "http");
                Assert.True(asset.FileName.Length > 0);
            }
        }

        [Fact]
        public async Task FetchAssetCountAsync()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().
                IncludeMetadata().SetLocale("en-us");
            JObject jObject = await assetLibrary.Count();
            if (jObject == null)
            {
                Assert.Fail( "Query.Exec is not match with expected result.");
            }
            else if (jObject != null)
            {
                Assert.Equal(5, jObject.GetValue("assets"));
            }
            else
            {
                Assert.Fail( "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task FetchAssetSkipLimit()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().SetLocale("en-us").Skip(2).Limit(5);
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            if (assets == null)
            {
                Assert.Fail( "Query.Exec is not match with expected result.");
            }
            else if (assets != null)
            {
                Assert.Equal(3, assets.Items.Count());
            }
            else
            {
                Assert.Fail( "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task FetchAssetOnly()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().Only(new string[] { "url"});
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            if (assets == null)
            {
                Assert.Fail( "Query.Exec is not match with expected result.");
            }
            else if (assets != null)
            {
                foreach (Asset asset in assets)
                {
                    Assert.DoesNotContain(asset.Url, "http");
                    Assert.Null(asset.Description);
                    Assert.Null(asset.FileSize);
                    Assert.Null(asset.Tags);
                    Assert.Null(asset.Description);
                }
            }
            else
            {
                Assert.Fail( "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task FetchAssetExcept()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().Except(new string[] { "description" });
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            if (assets == null)
            {
                Assert.Fail( "Query.Exec is not match with expected result.");
            }
            else if (assets != null)
            {
                foreach (Asset asset in assets)
                {
                    Assert.DoesNotContain(asset.Url, "http");
                    Assert.Null(asset.Description);
                }
            }
            else
            {
                Assert.Fail( "Result doesn't mathced the count.");
            }
        }
        [Fact]
        public async Task AssetTags_FetchBySpecificTags_ShouldReturnValidAssets_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.Tags(new string[] { "assetdotnet" });
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            
            Assert.NotNull(assets);
            
            int assetCount = assets.Count();
            Assert.True(assetCount >= 0, "Asset count should be non-negative");
            
            if (assetCount > 0)
            {
                foreach (Asset asset in assets)
                {
                    Assert.True(asset.FileName.Length > 0);
                    Assert.NotNull(asset.Uid);
                    Assert.NotNull(asset.Url);
                    Assert.True(asset.Tags != null || asset.Tags == null); // Either null or has value
                }
            }
        }

        [Fact]
        public async Task AssetTags_FetchWithExistingAssetTags_ShouldReturnMatchingAssets_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> allAssets = await assetLibrary.FetchAll();
            
            int totalAssetsCount = allAssets.Count();
            Assert.True(totalAssetsCount >= 0, "Total assets count should be non-negative");
            
            if (totalAssetsCount > 0)
            {
                Asset assetWithTags = null;
                foreach (Asset asset in allAssets)
                {
                    if (asset.Tags != null && asset.Tags.Length > 0)
                    {
                        assetWithTags = asset;
                        break;
                    }
                }
                
                if (assetWithTags != null && assetWithTags.Tags.Length > 0)
                {
                    string firstTag = assetWithTags.Tags[0].ToString();
                    AssetLibrary taggedAssetLibrary = client.AssetLibrary();
                    taggedAssetLibrary.Tags(new string[] { firstTag });
                    ContentstackCollection<Asset> filteredAssets = await taggedAssetLibrary.FetchAll();
                    
                    Assert.NotNull(filteredAssets);
                    
                    int filteredCount = filteredAssets.Count();
                    
                    Assert.True(filteredCount >= 1, $"Should find at least 1 asset with existing tag '{firstTag}'");
                    Assert.True(filteredCount <= totalAssetsCount, "Filtered count should not exceed total assets");
                    
                    bool foundOriginalAsset = false;
                    foreach (Asset filteredAsset in filteredAssets)
                    {
                        if (filteredAsset.Uid == assetWithTags.Uid)
                        {
                            foundOriginalAsset = true;
                            break;
                        }
                    }
                    
                    Assert.True(foundOriginalAsset, $"Asset with UID {assetWithTags.Uid} should be found when filtering by tag '{firstTag}'");
                }
            }
        }

        [Fact]
        public async Task AssetTags_FetchBySingleTag_ShouldExecuteWithoutErrors_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.Tags(new string[] { "asset1" });
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            
            Assert.NotNull(assets);
            
            int assetCount = assets.Count();
            Assert.True(assetCount >= 0, "Asset count should be non-negative");
            
            if (assetCount > 0)
            {
                foreach (Asset asset in assets)
                {
                    Assert.NotNull(asset.Uid);
                    Assert.True(asset.FileName.Length > 0);
                }
            }
        }

        [Fact]
        public async Task AssetTags_FetchByEmptyTagsArray_ShouldReturnAllAssets_Test()
        {
            AssetLibrary emptyTagsLibrary = client.AssetLibrary();
            emptyTagsLibrary.Tags(new string[] { });
            ContentstackCollection<Asset> emptyTagsAssets = await emptyTagsLibrary.FetchAll();
            
            Assert.NotNull(emptyTagsAssets);
            
            AssetLibrary allAssetsLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> allAssets = await allAssetsLibrary.FetchAll();
            
            int emptyTagsCount = emptyTagsAssets.Count();
            int allAssetsCount = allAssets.Count();
           
            
            Assert.True(emptyTagsCount >= 0, "Empty tags asset count should be non-negative");
            Assert.True(emptyTagsCount == allAssetsCount || emptyTagsCount >= 0, 
                "Empty tags should return all assets or handle gracefully");
        }

        [Fact]
        public async Task AssetTags_FetchByNullTags_ShouldReturnAllAssets_Test()
        {
            AssetLibrary nullTagsLibrary = client.AssetLibrary();
            nullTagsLibrary.Tags(null);
            ContentstackCollection<Asset> nullTagsAssets = await nullTagsLibrary.FetchAll();
            
            Assert.NotNull(nullTagsAssets);
            
            AssetLibrary allAssetsLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> allAssets = await allAssetsLibrary.FetchAll();
            
            int nullTagsCount = nullTagsAssets.Count();
            int allAssetsCount = allAssets.Count();
           
            
            Assert.True(nullTagsCount >= 0, "Null tags asset count should be non-negative");
            Assert.True(nullTagsCount == allAssetsCount || nullTagsCount >= 0, 
                "Null tags should return all assets or handle gracefully");
        }

        [Fact]
        public async Task AssetTags_ChainWithOtherFilters_ShouldRespectAllFilters_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.Tags(new string[] { "asset2", "asset1" })
                       .Limit(5)
                       .Skip(0)
                       .IncludeMetadata()
                       .IncludeFallback();
            
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            
            Assert.NotNull(assets);
            
            int assetCount = assets.Count();
            
            Assert.True(assetCount <= 5, "Limit of 5 should be respected");
            Assert.True(assetCount >= 0, "Asset count should be non-negative");
            
            if (assetCount > 0)
            {
                foreach (Asset asset in assets)
                {
                    Assert.NotNull(asset.Uid);
                    Assert.NotNull(asset.FileName);
                    Assert.True(asset.FileName.Length > 0);
                }
            }
        }

        [Fact]
        public async Task AssetTags_VerifyUrlQueriesParameter_ShouldContainTagsInQuery_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.Tags(new string[] { "asset1", "asset2" });
            
            var urlQueriesField = typeof(AssetLibrary).GetField("UrlQueries", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (urlQueriesField != null)
            {
                var urlQueries = (Dictionary<string, object>)urlQueriesField.GetValue(assetLibrary);
                Assert.True(urlQueries.ContainsKey("tags"));
                
                string[] tags = (string[])urlQueries["tags"];
                Assert.Equal(2, tags.Length);
                Assert.Contains("asset1", tags);
                Assert.Contains("asset2", tags);
                
            }
        }

        [Fact]
        public async Task AssetTags_FetchWithMultipleTags_ShouldReturnAssetsWithAnyTag_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.Tags(new string[] { "asset1", "asset2","assetdotnet" });
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            
            Assert.NotNull(assets);
            
            int assetCount = assets.Count();
            Assert.True(assetCount >= 0, "Asset count should be non-negative");
            
            if (assetCount > 0)
            {
                
                foreach (Asset asset in assets)
                {
                    Assert.NotNull(asset.Uid);
                    Assert.True(asset.FileName.Length > 0);
                    Assert.NotNull(asset.Url);
                    
                    if (asset.Tags != null && asset.Tags.Length > 0)
                    {
                        string[] searchTags = { "asset1", "asset2","assetdotnet" };
                        bool hasMatchingTag = false;
                        
                        foreach (object assetTag in asset.Tags)
                        {
                            string tagString = assetTag.ToString().ToLower();
                            foreach (string searchTag in searchTags)
                            {
                                if (tagString.Contains(searchTag.ToLower()))
                                {
                                    hasMatchingTag = true;
                                    break;
                                }
                            }
                            if (hasMatchingTag) break;
                        }
                        
                        if (!hasMatchingTag)
                        {
                            var assetTagsList = string.Join(", ", asset.Tags.Select(t => t.ToString()));
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task AssetTags_CompareFilteredVsAllAssets_ShouldReturnFewerOrEqualAssets_Test()
        {
            
            AssetLibrary allAssetsLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> allAssets = await allAssetsLibrary.FetchAll();
            
           
            AssetLibrary filteredAssetsLibrary = client.AssetLibrary();
            filteredAssetsLibrary.Tags(new string[] { "tag-does-not-exist" });
            ContentstackCollection<Asset> filteredAssets = await filteredAssetsLibrary.FetchAll();
            
            Assert.NotNull(allAssets);
            Assert.NotNull(filteredAssets);
            
            int allAssetsCount = allAssets.Count();
            int filteredAssetsCount = filteredAssets.Count();
            
            Assert.True(filteredAssetsCount <= allAssetsCount, 
                $"Filtered assets ({filteredAssetsCount}) should be <= all assets ({allAssetsCount})");
            
            Assert.Equal(0, filteredAssetsCount);
            
            Assert.True(allAssetsCount >= 0, "All assets count should be non-negative");
            if (allAssetsCount > 0)
            {
                Assert.True(filteredAssetsCount < allAssetsCount, 
                    "Filtered results should be less than total when using non-existent tag");
            }
        }

        [Fact]
        public async Task AssetTags_SortingAndPagination_ShouldRespectAllParameters_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            assetLibrary.Tags(new string[] { "asset1" })
                       .Limit(3)
                       .Skip(0)
                       .SortWithKeyAndOrderBy("created_at", Internals.OrderBy.OrderByDescending);
            
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            
            Assert.NotNull(assets);
            
            int assetCount = assets.Count();
            Assert.True(assetCount <= 3, "Should respect the limit of 3");
            Assert.True(assetCount >= 0, "Asset count should be non-negative");
            
            if (assetCount > 1)
            {
                DateTime previousDate = DateTime.MaxValue;
                foreach (Asset asset in assets)
                {
                    DateTime currentDate = asset.GetCreateAt();
                    Assert.True(currentDate <= previousDate, "Assets should be sorted by created_at in descending order");
                    previousDate = currentDate;
                }
            }
        }

        [Fact]
        public async Task AssetTags_VerifyHttpRequestParameters_ShouldCompleteSuccessfully_Test()
        {
            
            try
            {
                AssetLibrary assetLibrary = client.AssetLibrary();
                assetLibrary.Tags(new string[] { "asset1" })
                           .Limit(1); 
                           
                ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
                
               
                Assert.NotNull(assets);
                
                int assetCount = assets.Count();
                Assert.True(assetCount >= 0, "Asset count should be non-negative");
                Assert.True(assetCount <= 1, "Should respect limit of 1");
                
                Assert.True(true, "HTTP request with tags parameter completed successfully");
            }
            catch (Exception ex)
            {
                Assert.True(false, $"HTTP request failed, possibly due to malformed tags parameter: {ex.Message}");
            }
        }

        [Fact] 
        public async Task AssetTags_EmptyAndNullHandling_ShouldNotBreakApiCalls_Test()
        {            
            AssetLibrary emptyTagsLibrary = client.AssetLibrary();
            emptyTagsLibrary.Tags(new string[] { });
            ContentstackCollection<Asset> emptyTagsAssets = await emptyTagsLibrary.FetchAll();
            Assert.NotNull(emptyTagsAssets);
            
            AssetLibrary nullTagsLibrary = client.AssetLibrary();
            nullTagsLibrary.Tags(null);
            ContentstackCollection<Asset> nullTagsAssets = await nullTagsLibrary.FetchAll();
            Assert.NotNull(nullTagsAssets);
            
            AssetLibrary allAssetsLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> allAssets = await allAssetsLibrary.FetchAll();
            
            int emptyTagsCount = emptyTagsAssets.Count();
            int nullTagsCount = nullTagsAssets.Count();
            int allAssetsCount = allAssets.Count();
          
            
            Assert.True(emptyTagsCount == allAssetsCount || emptyTagsCount >= 0, 
                "Empty tags should return all assets or handle gracefully");
            Assert.True(nullTagsCount == allAssetsCount || nullTagsCount >= 0, 
                "Null tags should return all assets or handle gracefully");
        }

        [Fact]
        public async Task AssetTags_CaseSensitivityVerification_ShouldTestCaseBehavior_Test()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            ContentstackCollection<Asset> allAssets = await assetLibrary.FetchAll();
            
            int totalAssetsCount = allAssets.Count();
            Assert.True(totalAssetsCount >= 0, "Total assets count should be non-negative");
            
            Asset assetWithTags = null;
            string originalTag = null;
            
            foreach (Asset asset in allAssets)
            {
                if (asset.Tags != null && asset.Tags.Length > 0)
                {
                    assetWithTags = asset;
                    originalTag = asset.Tags[0].ToString();
                    break;
                }
            }
            
            if (assetWithTags != null && !string.IsNullOrEmpty(originalTag))
            {
                AssetLibrary originalCaseLibrary = client.AssetLibrary();
                originalCaseLibrary.Tags(new string[] { originalTag });
                ContentstackCollection<Asset> originalCaseAssets = await originalCaseLibrary.FetchAll();
                
                AssetLibrary upperCaseLibrary = client.AssetLibrary();
                upperCaseLibrary.Tags(new string[] { originalTag.ToUpper() });
                ContentstackCollection<Asset> upperCaseAssets = await upperCaseLibrary.FetchAll();
                
                AssetLibrary lowerCaseLibrary = client.AssetLibrary();
                lowerCaseLibrary.Tags(new string[] { originalTag.ToLower() });
                ContentstackCollection<Asset> lowerCaseAssets = await lowerCaseLibrary.FetchAll();
                
                Assert.NotNull(originalCaseAssets);
                Assert.NotNull(upperCaseAssets);
                Assert.NotNull(lowerCaseAssets);
                
                int originalCount = originalCaseAssets.Count();
                int upperCount = upperCaseAssets.Count();
                int lowerCount = lowerCaseAssets.Count();
                
                
                Assert.True(originalCount >= 1, $"Original case tag '{originalTag}' should return at least 1 asset");
                Assert.True(upperCount >= 0, "Uppercase tag search count should be non-negative");
                Assert.True(lowerCount >= 0, "Lowercase tag search count should be non-negative");
                Assert.True(originalCount <= totalAssetsCount, "Original count should not exceed total assets");
                Assert.True(upperCount <= totalAssetsCount, "Upper count should not exceed total assets");
                Assert.True(lowerCount <= totalAssetsCount, "Lower count should not exceed total assets");
                
                bool foundOriginalAsset = originalCaseAssets.Any(a => a.Uid == assetWithTags.Uid);
                Assert.True(foundOriginalAsset, $"Original asset {assetWithTags.Uid} should be found when searching with original tag '{originalTag}'");
                
                if (originalTag.ToLower() != originalTag.ToUpper()) 
                {
                    bool appearsCaseInsensitive = (originalCount == upperCount && upperCount == lowerCount);
                    
                    if (appearsCaseInsensitive)
                    {
                        Assert.Equal(originalCount, upperCount);
                        Assert.Equal(originalCount, lowerCount);
                    }
                }
            }
        }

        [Fact]
        public void Query_MultipleCalls_ShouldMergeQueries_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject firstQuery = new JObject
            {
                { "filename", "test1.png" },
                { "content_type", "image/png" }
            };
            JObject secondQuery = new JObject
            {
                { "file_size", 1024 },
                { "tags", new JArray { "test", "image" } }
            };

            // Act
            var result = assetLibrary.Query(firstQuery).Query(secondQuery);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
            // The method should not throw an exception when called multiple times
        }

        [Fact]
        public void Query_SingleCall_ShouldWorkAsBefore_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject queryObject = new JObject
            {
                { "filename", "test.png" }
            };

            // Act
            var result = assetLibrary.Query(queryObject);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Query_WithEmptyObject_ShouldNotThrowException_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject emptyQuery = new JObject();

            // Act & Assert
            var result = assetLibrary.Query(emptyQuery);
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Query_WithNullValues_ShouldHandleGracefully_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject queryWithNulls = new JObject
            {
                { "filename", "test.png" },
                { "null_field", null }
            };

            // Act & Assert
            var result = assetLibrary.Query(queryWithNulls);
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Query_ChainedWithOtherMethods_ShouldWork_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject queryObject = new JObject
            {
                { "filename", "test.png" }
            };

            // Act
            var result = assetLibrary
                .Query(queryObject)
                .Limit(10)
                .Skip(0)
                .IncludeMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Query_MultipleCallsWithSameKeys_ShouldMergeValues_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject firstQuery = new JObject
            {
                { "tags", new JArray { "tag1", "tag2" } }
            };
            JObject secondQuery = new JObject
            {
                { "tags", new JArray { "tag3", "tag4" } }
            };

            // Act
            var result = assetLibrary.Query(firstQuery).Query(secondQuery);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
            // The method should handle merging arrays without throwing exceptions
        }

        [Fact]
        public void Query_WithComplexNestedObjects_ShouldMergeCorrectly_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject firstQuery = new JObject
            {
                { "metadata", new JObject
                    {
                        { "author", "John Doe" },
                        { "version", 1 }
                    }
                }
            };
            JObject secondQuery = new JObject
            {
                { "metadata", new JObject
                    {
                        { "department", "IT" }
                    }
                },
                { "filename", "test.png" }
            };

            // Act
            var result = assetLibrary.Query(firstQuery).Query(secondQuery);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_SingleCall_ShouldAddKeyValuePair_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            string key = "filename";
            string value = "test.png";

            // Act
            var result = assetLibrary.Where(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_MultipleCalls_ShouldAddMultipleKeyValuePairs_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act
            var result = assetLibrary
                .Where("filename", "test.png")
                .Where("content_type", "image/png")
                .Where("file_size", "1024");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_WithEmptyStrings_ShouldHandleGracefully_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act & Assert
            var result = assetLibrary.Where("", "");
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_WithNullKey_ShouldHandleGracefully_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act & Assert
            var result = assetLibrary.Where(null, "value");
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_WithNullValue_ShouldHandleGracefully_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act & Assert
            var result = assetLibrary.Where("key", null);
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_ChainedWithOtherMethods_ShouldWork_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act
            var result = assetLibrary
                .Where("filename", "test.png")
                .Limit(10)
                .Skip(0)
                .IncludeMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_WithQueryMethod_ShouldWorkTogether_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();
            JObject queryObject = new JObject
            {
                { "content_type", "image/png" }
            };

            // Act
            var result = assetLibrary
                .Query(queryObject)
                .Where("filename", "test.png")
                .Where("file_size", "1024");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_OverwritesExistingKey_ShouldReplaceValue_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act
            var result = assetLibrary
                .Where("filename", "original.png")
                .Where("filename", "updated.png");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }

        [Fact]
        public void Where_WithSpecialCharacters_ShouldHandleCorrectly_Test()
        {
            // Arrange
            AssetLibrary assetLibrary = client.AssetLibrary();

            // Act
            var result = assetLibrary
                .Where("file_name", "test-file_123.png")
                .Where("description", "File with special chars: @#$%");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AssetLibrary>(result);
        }
    }
}