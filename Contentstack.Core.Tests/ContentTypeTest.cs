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
    public class ContentTypeTest

    {
        ContentstackClient client = StackConfig.GetStack();
        String source = "source";

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
        public async Task FetchContenTypeSchemaIncludeGlobalFields()
        {
            ContentType contenttype = client.ContentType(source);
            var param = new Dictionary<string, object>();
            param.Add("include_global_field_schema", true);
            var result = await contenttype.Fetch(param);
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
        public async Task GetContentTypes()
        {
            var result = await client.GetContentTypes();

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
        public async Task GetContentTypesIncludeGlobalFields()
        {
            var param = new Dictionary<string, object>();
            param.Add("include_global_field_schema", true);

            var result = await client.GetContentTypes(param);

            if (result == null)
            {
                Assert.False(true, "client.getContentTypes is not match with expected result.");
            }
            else
            {
                Assert.True(true);

            }
        }
    }
}