using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using Newtonsoft.Json.Linq;
using Contentstack.Utils;
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
                    Assert.False(true, "Entry.Fetch is not match with expected result.");
                }
                else
                {
                    Assert.True(result.FileName.Length > 0);
                }
            });
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
                        Assert.False(true);
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
        public async Task FetchAssetCountAsync()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().SetLocale("en-us");
            JObject jObject = await assetLibrary.Count();
            if (jObject == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (jObject != null)
            {
                Assert.Equal(5, jObject.GetValue("assets"));
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task FetchAssetSkipLimit()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().SetLocale("en-us").Skip(2).Limit(5);
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            if (assets == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (assets != null)
            {
                Assert.Equal(3, assets.Items.Count());
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task FetchAssetOnly()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().Only(new string[] { "url"});
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            if (assets == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
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
                Assert.False(true, "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task FetchAssetExcept()
        {
            AssetLibrary assetLibrary = client.AssetLibrary().Except(new string[] { "description" });
            ContentstackCollection<Asset> assets = await assetLibrary.FetchAll();
            if (assets == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
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
                Assert.False(true, "Result doesn't mathced the count.");
            }
        }
    }
}