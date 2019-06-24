using System;
using Xunit;
using Contentstack.Core.Configuration;
using System.Threading.Tasks;
using Contentstack.Core.Models;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Tests
{
    public class SyncStackTest
    {
        ContentstackClient client = StackConfig.GetSyncStack();

        //STAG 
        //String PaginationToken = "blt222be844e75a1fca332e39";
        //String SyncToken = "***REMOVED***";


        //PROD
        String PaginationToken = "blt99c1e34e65f6cc0fd1d82b";
        String SyncToken = "***REMOVED***";
        [Fact]
        public async Task SyncInit()
        {

            SyncStack result = await client.SyncRecursive();

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

            SyncStack result = await client.SyncRecursive(SyncType: SyncType.asset_published);

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

            SyncStack result = await client.SyncRecursive(ContentTypeUid: "session");

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

            SyncStack result = await client.SyncRecursive(StartFrom:DateTime.Now);

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

            SyncStack result = await client.SyncRecursive(SyncType: SyncType.entry_published, ContentTypeUid: "session");

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

            SyncStack result = await client.SyncRecursive(SyncType: SyncType.entry_published, StartFrom:DateTime.Now);

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

            SyncStack result = await client.SyncPaginationToken(PaginationToken);

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
        public async Task SyncToketest()
        {

            SyncStack result = await client.SyncToken(SyncToken);

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
