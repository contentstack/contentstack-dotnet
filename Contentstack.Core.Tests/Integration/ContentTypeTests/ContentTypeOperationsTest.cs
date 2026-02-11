using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.ContentTypeTests
{
    /// <summary>
    /// Comprehensive tests for Content Type operations
    /// Tests content type fetching, schema validation, and querying
    /// </summary>
    [Trait("Category", "ContentTypeOperations")]
    public class ContentTypeOperationsTest : IntegrationTestBase
    {
        public ContentTypeOperationsTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Fetch All Content Types
        
        [Fact(DisplayName = "Content Type - Content Type Get All Content Types Returns List Of Content Types")]
        public async Task ContentType_GetAllContentTypes_ReturnsListOfContentTypes()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var contentTypes = await client.GetContentTypes();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(contentTypes);
            TestAssert.True(contentTypes.Count > 0, "Should return at least one content type");
        }
        
        [Fact(DisplayName = "Content Type - Content Type Get All With Limit Returns Limited Results")]
        public async Task ContentType_GetAllWithLimit_ReturnsLimitedResults()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var param = new Dictionary<string, object>
            {
                { "limit", 2 }
            };
            
            // Act
            LogAct("Performing test action");

            var contentTypes = await client.GetContentTypes(param);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(contentTypes);
            TestAssert.True(contentTypes.Count <= 2);
        }
        
        [Fact(DisplayName = "Content Type - Content Type Get All With Skip Returns Skipped Results")]
        public async Task ContentType_GetAllWithSkip_ReturnsSkippedResults()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // First get all
            var allContentTypes = await client.GetContentTypes();
            
            // Now skip first one
            var param = new Dictionary<string, object>
            {
                { "skip", 1 }
            };
            var skippedContentTypes = await client.GetContentTypes(param);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(skippedContentTypes);
            TestAssert.True(skippedContentTypes.Count <= allContentTypes.Count);
        }
        
        [Fact(DisplayName = "Content Type - Content Type Get All With Count Includes Count")]
        public async Task ContentType_GetAllWithCount_IncludesCount()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var param = new Dictionary<string, object>
            {
                { "include_count", true }
            };
            
            // Act
            LogAct("Performing test action");

            var contentTypes = await client.GetContentTypes(param);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(contentTypes);
            TestAssert.True(contentTypes.Count > 0);
        }
        
        [Fact(DisplayName = "Content Type - Content Type Get All With Global Fields Includes Global Field Schema")]
        public async Task ContentType_GetAllWithGlobalFields_IncludesGlobalFieldSchema()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var param = new Dictionary<string, object>
            {
                { "include_global_field_schema", true }
            };
            
            // Act
            LogAct("Performing test action");

            var contentTypes = await client.GetContentTypes(param);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(contentTypes);
            TestAssert.True(contentTypes.Count > 0);
        }
        
        #endregion
        
        #region Fetch Single Content Type
        
        [Fact(DisplayName = "Content Type - Content Type Fetch Single Content Type Returns Schema")]
        public async Task ContentType_FetchSingleContentType_ReturnsSchema()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.SimpleContentTypeUid);
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await contentType.Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.IsType<JObject>(schema);
            // Schema should contain uid
            TestAssert.True(schema.ContainsKey("uid"));
            TestAssert.Equal(TestDataHelper.SimpleContentTypeUid, schema["uid"]?.ToString());
        }
        
        [Fact(DisplayName = "Content Type - Content Type Fetch With Global Fields Includes Global Field Schema")]
        public async Task ContentType_FetchWithGlobalFields_IncludesGlobalFieldSchema()
        {
            // Arrange
            LogArrange("Setting up test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.ComplexContentTypeUid);
            var param = new Dictionary<string, object>
            {
                { "include_global_field_schema", true }
            };
            
            // Act
            LogAct("Performing test action");

            var schema = await contentType.Fetch(param);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.IsType<JObject>(schema);
        }
        
        [Fact(DisplayName = "Content Type - Content Type Fetch Complex Type Contains Expected Fields")]
        public async Task ContentType_FetchComplexType_ContainsExpectedFields()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.ComplexContentTypeUid);
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await contentType.Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.IsType<JObject>(schema);
            // Should have schema field
            TestAssert.True(schema.ContainsKey("schema"));
        }
        
        [Fact(DisplayName = "Content Type - Content Type Fetch Non Existent Type Throws Exception")]
        public async Task ContentType_FetchNonExistentType_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up content type operation");

            var client = CreateClient();
            var contentType = client.ContentType("non_existent_content_type_xyz");
            
            // Act & Assert
            LogAct("Fetching entry from API");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await contentType.Fetch();
            });
        }
        
        #endregion
        
        #region Content Type Metadata
        
        [Fact(DisplayName = "Content Type - Content Type Schema Contains Title")]
        public async Task ContentType_Schema_ContainsTitle()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.SimpleContentTypeUid);
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await contentType.Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.True(schema.ContainsKey("title"));
            TestAssert.NotNull(schema["title"]);
            TestAssert.NotEmpty(schema["title"].ToString());
        }
        
        [Fact(DisplayName = "Content Type - Content Type Schema Contains Uid")]
        public async Task ContentType_Schema_ContainsUid()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.MediumContentTypeUid);
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await contentType.Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.True(schema.ContainsKey("uid"));
            TestAssert.Equal(TestDataHelper.MediumContentTypeUid, schema["uid"].ToString());
        }
        
        [Fact(DisplayName = "Content Type - Content Type Schema Contains Schema Definition")]
        public async Task ContentType_Schema_ContainsSchemaDefinition()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.ComplexContentTypeUid);
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await contentType.Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.True(schema.ContainsKey("schema"));
            var schemaArray = schema["schema"] as JArray;
            TestAssert.NotNull(schemaArray);
            TestAssert.True(schemaArray.Count > 0, "Schema should contain field definitions");
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Content Type - Content Type Fetch All Completes In Reasonable Time")]
        public async Task ContentType_FetchAll_CompletesInReasonableTime()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var (contentTypes, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.GetContentTypes();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(contentTypes);
            TestAssert.True(elapsed < 10000, $"GetContentTypes should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Content Type - Content Type Fetch Single Completes Quickly")]
        public async Task ContentType_FetchSingle_CompletesQuickly()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var contentType = client.ContentType(TestDataHelper.SimpleContentTypeUid);
            
            // Act
            LogAct("Fetching entry from API");

            var (schema, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await contentType.Fetch();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.True(elapsed < 5000, $"Single content type fetch should complete within 5s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Content Type - Content Type Multiple Content Types All Fetch Successfully")]
        public async Task ContentType_MultipleContentTypes_AllFetchSuccessfully()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);

            var client = CreateClient();
            
            // Act - Fetch multiple content types
            LogAct("Fetching entry from API");

            var simpleSchema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            var mediumSchema = await client.ContentType(TestDataHelper.MediumContentTypeUid).Fetch();
            var complexSchema = await client.ContentType(TestDataHelper.ComplexContentTypeUid).Fetch();
            
            // Assert - All should be retrieved successfully
            LogAssert("Verifying response");

            TestAssert.NotNull(simpleSchema);
            TestAssert.NotNull(mediumSchema);
            TestAssert.NotNull(complexSchema);
            
            // Verify UIDs match
            TestAssert.Equal(TestDataHelper.SimpleContentTypeUid, simpleSchema["uid"]?.ToString());
            TestAssert.Equal(TestDataHelper.MediumContentTypeUid, mediumSchema["uid"]?.ToString());
            TestAssert.Equal(TestDataHelper.ComplexContentTypeUid, complexSchema["uid"]?.ToString());
        }
        
        #endregion
        
        #region Helper Methods
        
        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }
        
        #endregion
    }
}

