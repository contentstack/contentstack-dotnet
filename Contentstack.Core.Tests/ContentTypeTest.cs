using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
                Assert.Fail( "contenttype.FetchSchema() is not match with expected result.");
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
                Assert.Fail( "contenttype.FetchSchema() is not match with expected result.");
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
                Assert.Fail( "client.getContentTypes is not match with expected result.");
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
                Assert.Fail( "client.getContentTypes is not match with expected result.");
            }
            else
            {
                Assert.True(true);

            }
        }

        [Fact]
        public async Task FetchGlobalFieldSchema()
        {
            string globalFieldUid = "global_field_uid";
            GlobalField globalField = client.GlobalField(globalFieldUid);

            var result = await globalField.Fetch();
            Assert.NotNull(result);
            Assert.True(result.HasValues, "GlobalField.Fetch() did not return expected schema.");
        }

        [Fact]
        public async Task FetchGlobalFieldSchema_InvalidUid_ThrowsOrReturnsNull()
        {
            string invalidUid = "invalid_uid";
            GlobalField globalField = client.GlobalField(invalidUid);
            await Assert.ThrowsAnyAsync<Exception>(async () => await globalField.Fetch());
        }

        [Fact]
        public async Task FetchGlobalFieldSchema_WithParameters_ReturnsSchema()
        {
            string globalFieldUid = "global_field_uid";
            GlobalField globalField = client.GlobalField(globalFieldUid);
            var param = new Dictionary<string, object> { { "include_global_field_schema", true } };
            var result = await globalField.Fetch(param);
            Assert.NotNull(result);
            Assert.True(result.HasValues, "GlobalField.Fetch() with params did not return expected schema.");
        }

        [Fact]
        public void SetAndRemoveHeader_WorksCorrectly()
        {
            string globalFieldUid = "global_field_uid";
            GlobalField globalField = client.GlobalField(globalFieldUid);
            globalField.SetHeader("custom_key", "custom_value");
            globalField.RemoveHeader("custom_key");
            Assert.True(true);
        }

        [Fact]
        public async Task FetchGlobalFieldSchema_WithCustomHeader()
        {
            string globalFieldUid = "global_field_uid";
            GlobalField globalField = client.GlobalField(globalFieldUid);
            globalField.SetHeader("custom_key", "custom_value");
            var result = await globalField.Fetch();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task FetchGlobalFieldSchema_NullParameters_Succeeds()
        {
            string globalFieldUid = "global_field_uid";
            GlobalField globalField = client.GlobalField(globalFieldUid);
            var result = await globalField.Fetch(null);
            Assert.NotNull(result);
        }

        [Fact]
        public void GlobalField_EmptyUid_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => {
                GlobalField globalField = client.GlobalField("");
            });
        }

        [Fact]
        public async Task GlobalFieldQuery_Find_ReturnsArray()
        {
            var query = client.GlobalFieldQuery();
            var result = await query.Find();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GlobalFieldQuery_Find_WithParameters_ReturnsArray()
        {
            var query = client.GlobalFieldQuery();
            var param = new Dictionary<string, object> { { "include_global_field_schema", true } };
            var result = await query.Find(param);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GlobalFieldQuery_Find_WithSkipAndLimit_ReturnsArray()
        {
            var query = client.GlobalFieldQuery();
            var param = new Dictionary<string, object> { { "skip", 1 }, { "limit", 2 } };
            var result = await query.Find(param);
            Assert.Empty(result["global_fields"]);
        }

        [Fact]
        public void GlobalFieldQuery_IncludeBranch_SetsQueryParam()
        {
            var query = client.GlobalFieldQuery();
            var result = query.IncludeBranch();
            Assert.NotNull(result);
            Assert.Equal(query, result);
        }

        [Fact]
        public void GlobalFieldQuery_IncludeGlobalFieldSchema_SetsQueryParam()
        {
            var query = client.GlobalFieldQuery();
            var result = query.IncludeGlobalFieldSchema();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GlobalFieldQuery_Find_InvalidParams_ThrowsOrReturnsEmpty()
        {
            var query = client.GlobalFieldQuery();
            var invalidParams = new Dictionary<string, object> { { "invalid_param", true } };

            var result = await query.Find(invalidParams);

            Assert.NotNull(result);
            Assert.IsType<JObject>(result);
            var globalFields = result["global_fields"] as JArray; 
            Assert.NotNull(globalFields);
        }
    }
}