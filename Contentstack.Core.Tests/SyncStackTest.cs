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
        //String PaginationToken = "***REMOVED***";
        //String SyncToken = "***REMOVED***";


        //PROD
        String PaginationToken = "***REMOVED***";
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
