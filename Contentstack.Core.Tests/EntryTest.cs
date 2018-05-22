using System;
using Contentstack.Core;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using Contentstack.Core.Internals;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Configuration;
            
namespace Contentstack.Core.Tests
{
    public class EntryTest
    {

        Stack Stack = StackConfig.GetStack();
        String numbersContentType = "numbers_content_type";
        String source = "source";
        String singelEntryFetchUID = "blt1f94e478501bba46";
        String referenceFieldUID = "reference";

        [Fact]
        public async Task FetchByUid() {
            ContentType contenttype = Stack.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            var result = await sourceEntry.Fetch();

            if (result == null) {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            } else {
                Assert.True(result.Object.Count > 0 && result.EntryUid == sourceEntry.EntryUid && result.Object.ContainsKey("publish_details") && result.Object["publish_details"] != null);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task IncludeReference() {
            ContentType contenttype = Stack.ContentType(source);
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
        public async Task Only() {
            ContentType contenttype = Stack.ContentType(source);
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
            ContentType contenttype = Stack.ContentType(source);
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
            ContentType contenttype = Stack.ContentType(source);
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
            ContentType contenttype = Stack.ContentType(source);
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
            ContentType contenttype = Stack.ContentType(source);
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
            ContentType contenttype = Stack.ContentType(source);
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
            ContentType contenttype = Stack.ContentType(source);
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
    }
}
