using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

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
    }
}
