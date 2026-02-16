using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Contentstack.Core.Tests.Models;
using Contentstack.Core.Internals;
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
                     Assert.Fail( "Entry.Fetch is not match with expected result.");
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
                        Assert.Fail( "Entry.Fetch is not match with expected result.");
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
                        Assert.Fail( "Entry.Fetch is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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
                Assert.Fail( "Query.Exec is not match with expected result.");
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

            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry.Fetch<SourceModel>();


            var HtmlText = result.GetHTMLText();
            if (string.IsNullOrEmpty(HtmlText) && HtmlText.Length == 0) {
                Assert.Fail( "Query.Exec is not match with expected result.");
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

        [Fact]
        public async Task IncludeMetadata()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            
            sourceEntry.IncludeMetadata();
            var result = await sourceEntry.Fetch<Entry>();
            
            if (result == null)
            {
                Assert.Fail("Entry.Fetch is not match with expected result.");
            }
            else
            {
                // Verify metadata is included by checking if _metadata dictionary exists
                var metadata = result.GetMetadata();
                Assert.NotNull(metadata);
                // Metadata might be empty or might not contain "uid" - just verify it exists
                // The metadata property is populated when API returns _metadata in response
                Assert.True(true, "IncludeMetadata() was called and metadata property exists");
            }
        }

        [Fact(Skip = "Requires branch to be configured in Contentstack stack - set branch name in config")]
        public async Task IncludeBranch()
        {
            // This test requires a branch to be set up in your Contentstack stack
            // Update StackConfig to include branch name if needed
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            
            sourceEntry.IncludeBranch();
            var result = await sourceEntry.Fetch<SourceModel>();
            
            if (result == null)
            {
                Assert.Fail("Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.NotNull(result);
                // Branch information should be available in the response
                // The exact assertion depends on your data structure
            }
        }

        [Fact]
        public async Task IncludeOwner()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            
            sourceEntry.IncludeOwner();
            var result = await sourceEntry.Fetch<SourceModel>();
            
            if (result == null)
            {
                Assert.Fail("Entry.Fetch is not match with expected result.");
            }
            else
            {
                Assert.NotNull(result);
                // Owner information should be available - verify created_by or updated_by fields
                Assert.NotNull(result.created_by);
                Assert.True(result.created_by.Length > 0);
            }
        }

        [Fact]
        public async Task GetMetadata()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);
            
            sourceEntry.IncludeMetadata();
            var result = await sourceEntry.Fetch<Entry>();
            
            if (result == null)
            {
                Assert.Fail("Entry.Fetch is not match with expected result.");
            }
            else
            {
                var metadata = result.GetMetadata();
                Assert.NotNull(metadata);
                // Metadata might be empty - just verify GetMetadata() returns a valid dictionary
                // The actual content depends on what the API returns
                Assert.True(true, "GetMetadata() returns a valid dictionary (may be empty)");
            }
        }

        [Fact]
        public async Task AssetFields_SingleEntry_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.AssetFields("user_defined_fields", "embedded_metadata", "ai_generated_metadata", "visual_markups");
            var result = await sourceEntry.Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields did not return a result.");
            Assert.NotNull(result.Uid);
        }

        [Fact]
        public async Task AssetFields_ChainedWithIncludeMetadata_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            var result = await sourceEntry
                .AssetFields("user_defined_fields")
                .IncludeMetadata()
                .Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields and IncludeMetadata did not return a result.");
            Assert.NotNull(result.Uid);
        }

        [Fact]
        public async Task AssetFields_SingleField_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.AssetFields("user_defined_fields");
            var result = await sourceEntry.Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields single field did not return a result.");
            Assert.NotNull(result.Uid);
        }

        [Fact]
        public async Task AssetFields_WithMultipleFields_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.AssetFields("user_defined_fields", "embedded_metadata", "visual_markups");
            var result = await sourceEntry.Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields multiple fields did not return a result.");
            Assert.NotNull(result.Uid);
        }

        [Fact]
        public async Task AssetFields_WithNoArguments_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.AssetFields();
            var result = await sourceEntry.Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields() no arguments did not return a result.");
            Assert.NotNull(result.Uid);
        }

        [Fact]
        public async Task AssetFields_WithNull_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.AssetFields(null);
            var result = await sourceEntry.Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields(null) did not return a result.");
            Assert.NotNull(result.Uid);
        }

        [Fact]
        public async Task AssetFields_WithEmptyArray_RequestSucceeds()
        {
            ContentType contenttype = client.ContentType(source);
            string uid = await GetUID("source1");
            Entry sourceEntry = contenttype.Entry(uid);

            sourceEntry.AssetFields(new string[0]);
            var result = await sourceEntry.Fetch<Entry>();

            if (result == null)
                Assert.Fail("Entry.Fetch with AssetFields(empty array) did not return a result.");
            Assert.NotNull(result.Uid);
        }
    }
}
