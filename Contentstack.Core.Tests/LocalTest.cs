using System;

using Contentstack.Core;
using Contentstack.Core.Models;
using Contentstack.Core.Configuration;
using Xunit;
using System.Threading.Tasks;

namespace Contentstack.Core.Tests
{
    public class LocalTest
    {
        [Fact]
        public async Task FetchQueryForEntry()
        {
            ContentstackClient contentstackClient = new ContentstackClient("blt520df8d675f11a0a", "bltcc8f202fa758bdf2", "development");
            Query csQuery = contentstackClient.ContentType("test").Query();
            csQuery.AddQuery("uid", "blt9f9e183545e2fe5b");
            csQuery.SetLanguage(Internals.Language.GERMEN_SWITZERLAND);
            Query query = await csQuery.FindOne();
            Console.WriteLine(query.Result.ToString());
            Assert.True(true, "result is greater than 11");
        }
        [Fact]
        public async Task FetchEntry()
        {
            ContentstackClient contentstackClient = new ContentstackClient("blt520df8d675f11a0a", "bltcc8f202fa758bdf2", "development");
            Entry csEntry = contentstackClient.ContentType("test").Entry("blt9f9e183545e2fe5b");
            csEntry.SetLanguage(Internals.Language.GERMEN_SWITZERLAND);
            Entry entry = await csEntry.Fetch();
            Console.WriteLine(entry.ToString());
            Assert.True(true, "result is greater than 11");
        }
    }
}
