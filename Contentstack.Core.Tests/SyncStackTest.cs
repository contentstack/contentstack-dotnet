using System;
using Xunit;
using Contentstack.Core.Configuration;
using System.Threading.Tasks;

namespace Contentstack.Core.Tests
{
    public class SyncStackTest
    {
        ContentstackClient client = StackConfig.GetSyncStack();

        [Fact]
        public async Task SyncInit()
        {

            var result = await client.SyncRecursive();

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async Task SyncSyncType()
        {

            var result = await client.SyncRecursive(SyncType: Models.SyncType.asset_published);

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }
        [Fact]
        public async Task SyncContentType()
        {

            var result = await client.SyncRecursive(ContentTypeUid: "session");

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }
        [Fact]
        public async Task SyncStartFrom()
        {

            var result = await client.SyncRecursive(StartFrom:DateTime.Now);

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }
        [Fact]
        public async Task SyncTypeWithContentType()
        {

            var result = await client.SyncRecursive(SyncType: Models.SyncType.entry_published, ContentTypeUid: "session");

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }
        [Fact]
        public async Task SyncTypeWithStartFrom()
        {

            var result = await client.SyncRecursive(SyncType: Models.SyncType.entry_published, StartFrom:DateTime.Now);

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async Task SyncPaginationToken()
        {

            var result = await client.SyncPaginationToken("***REMOVED***");

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async Task SyncToken()
        {

            var result = await client.SyncToken("***REMOVED***");

            if (result == null)
            {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }
    }
}
