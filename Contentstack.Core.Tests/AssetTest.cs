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

        [Fact]
        public async Task FetchAssetByUid()
        {
            Asset asset = client.Asset("blt649cfadb08b577db");
            await asset.Fetch();
            Assert.True(asset.FileName.Length > 0);
       }

        [Fact]
        public async Task FetchAssets()
        {
            AssetLibrary assetLibrary = client.AssetLibrary();
            Asset[] assets = await assetLibrary.FetchAll();
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
            Asset[] assets = await assetLibrary.FetchAll();
            Assert.True(assets.Count() > 0);
            DateTime dateTime = new DateTime();
            foreach (Asset asset in assets)
            {
                if (dateTime != null)
                {
                    if (dateTime.CompareTo(asset.GetCreateAt()) != -1)
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
            Asset[] assets = await assetLibrary.FetchAll();
            Assert.True(assets.Count() > 0);
            foreach (Asset asset in assets)
            {
                Assert.DoesNotContain(asset.Url, "http");
                Assert.True(asset.FileName.Length > 0);
            }
        }
    }
}
