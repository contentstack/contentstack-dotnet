using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Contentstack.Core.Tests.Models;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests
{

    public class EntryTest
    {
        ContentstackClient client = StackConfig.GetStack();

        ////PROD STAG
        String source = "source";
        String singelEntryFetchUID = "";
        string htmlSource = "";
        String referenceFieldUID = "reference";
        //EU
        //String source = "source";
        //String singelEntryFetchUID = "bltf4268538a14fc5e1";
        //string htmlSource = "blt7c4197d43c1156ba";
        //String referenceFieldUID = "reference";
        public async Task<string> GetUID(string title)
        {
            Query query = client.ContentType(source).Query();
            var result = await query.Find<SourceModel>();
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
        public async Task FetchByUid() {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            sourceEntry.IncludeMetadata();
            await sourceEntry.Fetch<Entry>().ContinueWith((t) =>
             {
                 Entry result = t.Result;
                 if (result == null)
                 {
                     Assert.False(true, "Entry.Fetch is not match with expected result.");
                 }
                 else
                 {
                     Assert.True(result.Uid == sourceEntry.Uid);
                 }
             });
        }

        [Fact]
        public async Task FetchEntryByUIDPublishFallback()
        {
            List<string> list = new List<string>();
            list.Add("en-us");
            list.Add("ja-jp");
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            sourceEntry = await sourceEntry
                .SetLocale("ja-jp")
                .IncludeFallback()
                .Fetch<Entry>();

            Assert.Contains((string)(sourceEntry.Get("publish_details") as JObject).GetValue("locale"), list);
        }

        [Fact]
        public async Task FetchEntryByVariant()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            await sourceEntry
                .Variant("variant1")
                .Fetch<Entry>().ContinueWith((t) =>
                {
                    Entry result = t.Result;
                    if (result == null)
                    {
                        Assert.False(true, "Entry.Fetch is not match with expected result.");
                    }
                    else
                    {
                        Assert.True(result.Uid == sourceEntry.Uid);
                        Assert.Null(result._variant);
                    }
                });
        }

        [Fact]
        public async Task FetchEntryByVariants()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            await sourceEntry
                .Variant(new List<string> { "variant1", "variant2" })
                .Fetch<Entry>().ContinueWith((t) =>
                {
                    Entry result = t.Result;
                    if (result == null)
                    {
                        Assert.False(true, "Entry.Fetch is not match with expected result.");
                    }
                    else
                    {
                        Assert.True(result.Uid == sourceEntry.Uid);
                        Assert.Null(result._variant);
                    }
                });
        }

        [Fact]
        public async Task FetchEntryByUIDPublishWithoutFallback()
        {
            List<string> list = new List<string>();
            list.Add("ja-jp");
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = await contenttype.Entry(uid)
                .SetLocale("ja-jp")
                .Fetch<Entry>();

            Assert.Contains((string)(sourceEntry.Get("publish_details") as JObject).GetValue("locale"), list);
        }

        [Fact]
        public async Task IncludeReference() {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.IncludeReference(referenceFieldUID);
            var result = await sourceEntry.Fetch<SourceModelIncludeRef>();
            if (result == null) {
                Assert.False(true, "Query.Exec is not match with expected result.");
            } else {

                bool IsTrue = false;
                List<Entry> lstReference = result.Reference;

                if (lstReference.Count > 0) {
                    IsTrue = lstReference.All(a => a is Entry);
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task IncludeReferenceArray()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.IncludeReference(new string[] {referenceFieldUID,"other_reference"});
            var result = await sourceEntry.Fetch<SourceModelIncludeRefAndOther>();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                bool IsTrue = false;
                List<Dictionary<string, object>>  firstReference = result.Reference;
                List<Dictionary<string, object>>  secondReference = result.Other_reference;
                IsTrue = firstReference.All(a => a is Dictionary<string, object>);
                Assert.True(IsTrue);
                IsTrue = secondReference.All(a => a is Dictionary<string, object>);
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Only() {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.Only(new string[] { "title", "number" });
            SourceModel result = await sourceEntry.Fetch<SourceModel>();
            if (result == null) {
                Assert.False(true, "Query.Exec is not match with expected result.");
            } else {

                List<string> uidKeys = new List<string>() { "title", "number", "uid" };
                bool IsTrue = false;
                //IsTrue = data.Object.Keys.Count == 3 && data.Object.Keys.ToList().Contains(a=>  ui);
                IsTrue = result.Uid != null && result.Title != null && result.Number == 4 ? true : false;
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Except() {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.Except(new string[] { "title", "number" });
            var result = await sourceEntry.Fetch<SourceModel>();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {

                List<string> uidKeys = new List<string>() { "title", "number" };
                bool IsTrue = false;
                IsTrue = result.Title == null && result.Number != 4.0 ? true : false;
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task GetCreateAt()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();
            var Created_at = result.Created_at;
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(Created_at != default(DateTime));
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task GetUpdateAt()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();
            var updated_at = result.updated_at;
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(updated_at != default(DateTime));
            }
        }

        [Fact]
        public async Task GetCreatedBy()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();
            var created_by = result.created_by;
            if (created_by == null && created_by.Length == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(created_by.Length > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task GetUpdatedBy()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();
            var Updated_by = result.Updated_by;
            if (Updated_by == null && Updated_by.Length == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(Updated_by.Length > 0);
            }
        }

        [Fact]
        public async Task GetTags()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();
            var Tags = result.Tags;
            if (Tags == null && Tags.Length == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(Tags is object[] && Tags.Length > 0);
            }
        }

        [Fact]
        public async Task GetHTMLText()
        {
            ContentType contenttype = client.ContentType(source);

            string uid = await GetUID("source");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();


            var HtmlText = result.GetHTMLText();
            if (string.IsNullOrEmpty(HtmlText) && HtmlText.Length == 0) {
                Assert.False(true, "Query.Exec is not match with expected result.");
            } else {
                var tagList = new List<string>();
                string pattern = @"(?<=</?)([^ >/]+)";
                var matches = Regex.Matches(HtmlText, pattern);
                for (int i = 0; i < matches.Count; i++)
                {
                    tagList.Add(matches[i].ToString());
                }
                Assert.True(!string.IsNullOrEmpty(HtmlText) && HtmlText.Length > 0 && tagList.Count > 0);
            }
        }
    }
}
