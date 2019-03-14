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
   
    public class EntryTest
    {
        ContentstackClient client = StackConfig.GetStack();


        String numbersContentType = "numbers_content_type";
        String source = "source";
        String singelEntryFetchUID = "blt1f94e478501bba46";
        String referenceFieldUID = "reference";

        [Fact]
        public async Task FetchByUid() {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();

            if (result == null) {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            } else {
                Assert.True(result.Object.Count > 0 && result.EntryUid == sourceEntry.EntryUid && result.Object.ContainsKey("publish_details") && result.Object["publish_details"] != null);
            }
        }

        [Fact]
        public async Task GetContentTypes()
        {
            var result = await client.getContentTypes();

            if (result == null)
            {
                Assert.False(true, "client.getContentTypes is not match with expected result.");
            }
            else
            {
                Assert.True(true);

            }
        }

        [Fact]
        public async Task FetchContenTypeSchema()
        {
            ContentType contenttype = client.ContentType(source);
            var result = await contenttype.Fetch();
            if (result == null)
            {
                Assert.False(true, "contenttype.FetchSchema() is not match with expected result.");
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async Task IncludeReference() {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            sourceEntry.IncludeReference(referenceFieldUID);
            var result = await sourceEntry.Fetch();
            if (result == null) {
                Assert.False(true, "Query.Exec is not match with expected result.");
            } else {

                bool IsTrue = false;
                object[] refDetails = (object[])result.Object[referenceFieldUID];

                List<object> lstReference = refDetails.ToList();
                if (lstReference.Count > 0) {
                    IsTrue = lstReference.All(a => a is Dictionary<string, object>);

                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task IncludeReferenceArray()
        {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            sourceEntry.IncludeReference(new string[] {referenceFieldUID,"other_reference"});
            var result = await sourceEntry.Fetch();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {

                bool IsTrue = false;
                object[] firstReference = (object[])result.Object[referenceFieldUID];
                object[] secondReference = (object[])result.Object["other_reference"];

                List<object[]> references = new List<object[]>();
                references.Add(firstReference);
                references.Add(secondReference);

                foreach(object[] referene in references) {
                    List<object> lstReference = referene.ToList();
                    if (lstReference.Count > 0)
                    {
                        IsTrue = lstReference.All(a => a is Dictionary<string, object>);

                    }
                    Assert.True(IsTrue);
                }

            }
        }

        [Fact]
        public async Task Only() {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            sourceEntry.Only(new string[] { "title", "number" });
            var result = await sourceEntry.Fetch();
            if (result == null) {
                Assert.False(true, "Query.Exec is not match with expected result.");
            } else {

                List<string> uidKeys = new List<string>() { "title", "number", "uid" };
                bool IsTrue = false;
                //IsTrue = data.Object.Keys.Count == 3 && data.Object.Keys.ToList().Contains(a=>  ui);
                IsTrue = result.Object.Keys.All(p => uidKeys.Contains(p));

                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Except() {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            sourceEntry.Except(new string[] { "title", "number" });
            var result = await sourceEntry.Fetch();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {

                List<string> uidKeys = new List<string>() { "title", "number" };
                bool IsTrue = false;

                IsTrue = result.Object.Keys.All(p => !uidKeys.Contains(p));

                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task GetCreateAt()
        {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();
            var updated_at = result.GetCreateAt();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(result.Object["created_at"] is string);
                Assert.True(updated_at != default(DateTime));
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task GetUpdateAt()
        {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();
            var updated_at = result.GetUpdateAt();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(result.Object["updated_at"] is string);
                Assert.True(updated_at != default(DateTime));
            }
        }

        [Fact]
        public async Task GetCreatedBy()
        {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();
            var created_by = result.GetCreatedBy();
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();
            var created_by = result.GetUpdatedBy();
            if (created_by == null && created_by.Length == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(created_by.Length > 0);
            }
        }

        [Fact]
        public async Task GetTags()
        {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();
            var Tags = result.GetTags();
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
            Entry sourceEntry = contenttype.Entry("blt2f0dd6a81f7f40e7");
            var result = await sourceEntry.Fetch();
            var HtmlText = result.GetHTMLText("markdown");
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
