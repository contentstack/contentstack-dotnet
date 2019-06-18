﻿using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using Contentstack.Core.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using Contentstack.Core.Tests.Models;
namespace Contentstack.Core.Tests
{
   
    public class EntryTest
    {
        ContentstackClient client = StackConfig.GetStack();


        String source = "source";
        String singelEntryFetchUID = "blt1f94e478501bba46";
        String referenceFieldUID = "reference";

        [Fact]
        public async Task FetchByUid() {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            SourceModel result = await sourceEntry.Fetch<SourceModel>();

            if (result == null) {
                Assert.False(true, "Entry.Fetch is not match with expected result.");
            } else {
                Assert.True(result.Uid == sourceEntry.Uid);
            }
        }

        [Fact]
        public async Task IncludeReference() {
            ContentType contenttype = client.ContentType(source);
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
            sourceEntry.IncludeReference(referenceFieldUID);
            var result = await sourceEntry.Fetch<SourceModelIncludeRef>();
            if (result == null) {
                Assert.False(true, "Query.Exec is not match with expected result.");
            } else {

                bool IsTrue = false;
                List<Dictionary<string, object>> lstReference = result.Reference;

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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry(singelEntryFetchUID);
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
            Entry sourceEntry = contenttype.Entry("blt2f0dd6a81f7f40e7");
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
