using System.Threading.Tasks;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Models;
using Xunit;

namespace Contentstack.Core.Tests
{
    public class PluginsTest
    {
        ContentstackClient client = StackConfig.GetStack();

        ////PROD STAG
        string source = "source";

        public async Task<string> GetUID(string title)
        {
            Query query = client.ContentType(source).Query();
            var result = await query.Find<SourceModel>();
            client.Plugins.Add(new TestPlugin(StackConfig.GetStack()));

            if (result != null)
            {
                foreach (var data in result.Items)
                {
                    if (data.Title == title)
                    {
                        return data.Uid;
                    }
                }
            }
           
            return null;
        }

        [Fact]
        public async Task FetchByUid()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            await sourceEntry.Fetch<Entry>().ContinueWith((t) =>
            {
                Entry result = t.Result;
                
                if (result == null)
                {
                    Assert.False(true, "Entry.Fetch is not match with expected result.");
                }
                else
                {
                    Assert.Contains("emails", result.Object.Keys);
                }
            });
        }
    }
}
