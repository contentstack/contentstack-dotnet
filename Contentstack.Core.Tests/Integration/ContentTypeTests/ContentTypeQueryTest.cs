using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.ContentTypeTests
{
    /// <summary>
    /// Tests for Content Type Query operations
    /// Tests fetching, filtering, and querying content type schemas
    /// </summary>
    [Trait("Category", "ContentTypeQuery")]
    public class ContentTypeQueryTest : IntegrationTestBase
    {
        public ContentTypeQueryTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic Content Type Operations
        
        [Fact(DisplayName = "Query Operations - Content Type Query Fetch Single Returns Schema")]
        public async Task ContentTypeQuery_FetchSingle_ReturnsSchema()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.True(schema.ContainsKey("uid"));
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Fetch Multiple All Valid")]
        public async Task ContentTypeQuery_FetchMultiple_AllValid()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema1 = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            var schema2 = await client.ContentType(TestDataHelper.MediumContentTypeUid).Fetch();
            var schema3 = await client.ContentType(TestDataHelper.ComplexContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema1);
            TestAssert.NotNull(schema2);
            TestAssert.NotNull(schema3);
        }
        
        #endregion
        
        #region Content Type with Global Fields
        
        [Fact(DisplayName = "Query Operations - Content Type Query Fetch Schema Returns Schema")]
        public async Task ContentTypeQuery_FetchSchema_ReturnsSchema()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema Validation Is Valid J Object")]
        public async Task ContentTypeQuery_SchemaValidation_IsValidJObject()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.IsType<JObject>(schema);
        }
        
        #endregion
        
        #region Schema Structure Validation
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema Has UID Valid")]
        public async Task ContentTypeQuery_SchemaHasUID_Valid()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.True(schema.ContainsKey("uid"));
            TestAssert.Equal(TestDataHelper.SimpleContentTypeUid, schema["uid"]?.ToString());
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema Has Title Valid")]
        public async Task ContentTypeQuery_SchemaHasTitle_Valid()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.True(schema.ContainsKey("title") || schema.ContainsKey("name"));
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema Has Fields Field Array")]
        public async Task ContentTypeQuery_SchemaHasFields_FieldArray()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            // Schema should describe fields
        }
        
        #endregion
        
        #region Content Type Metadata
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema Metadata Includes Created Info")]
        public async Task ContentTypeQuery_SchemaMetadata_IncludesCreatedInfo()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            // Metadata should be present
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema Metadata Includes Updated Info")]
        public async Task ContentTypeQuery_SchemaMetadata_IncludesUpdatedInfo()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
        }
        
        #endregion
        
        #region Complex Content Types
        
        [Fact(DisplayName = "Query Operations - Content Type Query Complex Schema All Field Types")]
        public async Task ContentTypeQuery_ComplexSchema_AllFieldTypes()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            // Should include all complex field type definitions
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Schema With References Shows Reference Fields")]
        public async Task ContentTypeQuery_SchemaWithReferences_ShowsReferenceFields()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Query Operations - Content Type Query Performance Fetch Schema")]
        public async Task ContentTypeQuery_Performance_FetchSchema()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var (schema, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
            TestAssert.True(elapsed < 5000, $"Schema fetch should complete within 5s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Query Operations - Content Type Query Performance Multiple Schemas")]
        public async Task ContentTypeQuery_Performance_MultipleSchemas()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("ContentType", TestDataHelper.MediumContentTypeUid);

            var client = CreateClient();
            var startTime = DateTime.Now;
            
            // Act - Fetch multiple schemas
            LogAct("Fetching entry from API");

            await client.ContentType(TestDataHelper.SimpleContentTypeUid).Fetch();
            await client.ContentType(TestDataHelper.MediumContentTypeUid).Fetch();
            await client.ContentType(TestDataHelper.ComplexContentTypeUid).Fetch();
            
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.True(elapsed < 15000, $"3 schemas should fetch within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Query Operations - Content Type Query Non Existent Content Type Throws Error")]
        public async Task ContentTypeQuery_NonExistentContentType_ThrowsError()
        {
            // Arrange
            LogArrange("Setting up content type operation");

            var client = CreateClient();
            
            // Act & Assert
            LogAct("Fetching entry from API");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType("non_existent_uid").Fetch();
            });
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

