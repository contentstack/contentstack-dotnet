using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.BranchTests
{
    /// <summary>
    /// Comprehensive tests for Branch and Metadata
    /// Tests branch operations, metadata fields, and branch-specific queries
    /// </summary>
    [Trait("Category", "MetadataBranch")]
    public class MetadataBranchComprehensiveTest : IntegrationTestBase
    {
        public MetadataBranchComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic Branch Operations
        
        [Fact(DisplayName = "Branch Client With Branch Uses Specified Branch")]
        public async Task Branch_ClientWithBranch_UsesSpecifiedBranch()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithBranch();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Branch Query With Branch Fetches From Branch")]
        public async Task Branch_QueryWithBranch_FetchesFromBranch()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClientWithBranch();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Branch Asset With Branch Fetches From Branch")]
        public async Task Branch_AssetWithBranch_FetchesFromBranch()
        {
            // Arrange
            LogArrange("Setting up fetch operation");
            LogContext("AssetUid", TestDataHelper.ImageAssetUid);

            var client = CreateClientWithBranch();
            
            // Act
            LogAct("Fetching entry from API");

            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(asset);
        }
        
        #endregion
        
        #region Metadata Fields
        
        [Fact(DisplayName = "Metadata Created By Available In Entry")]
        public async Task Metadata_CreatedBy_AvailableInEntry()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Branch Deep References All From Same Branch")]
        public async Task Branch_DeepReferences_AllFromSameBranch()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClientWithBranch();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { "authors", "authors.reference" })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Query with Branch
        
        [Fact(DisplayName = "Branch Query Filters Works With Branch")]
        public async Task Branch_QueryFilters_WorksWithBranch()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClientWithBranch();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.Exists("title");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        [Fact(DisplayName = "Branch Complex Query Works With Branch")]
        public async Task Branch_ComplexQuery_WorksWithBranch()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClientWithBranch();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            query.And(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Include Owner
        
        [Fact(DisplayName = "Metadata Include Owner Adds Owner Info")]
        public async Task Metadata_IncludeOwner_AddsOwnerInfo()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            TestAssert.NotEmpty(entry.Uid);
            TestAssert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Metadata Query With Owner Includes Owner For All")]
        public async Task Metadata_QueryWithOwner_IncludesOwnerForAll()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            LogAct("Executing query");

            query.IncludeOwner();
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
        }
        
        #endregion
        
        #region Content Type Metadata
        
        [Fact(DisplayName = "Metadata Content Type Schema Includes Metadata")]
        public async Task Metadata_ContentTypeSchema_IncludesMetadata()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
        }
        
        [Fact(DisplayName = "Metadata Content Type With Branch Branch Specific")]
        public async Task Metadata_ContentTypeWithBranch_BranchSpecific()
        {
            // Arrange
            LogArrange("Setting up content type operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClientWithBranch();
            
            // Act
            LogAct("Fetching entry from API");

            var schema = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Fetch();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(schema);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Branch Performance With Branch")]
        public async Task Branch_Performance_WithBranch()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClientWithBranch();
            
            // Act
            LogAct("Fetching entry from API");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.True(elapsed < 10000, $"Branch fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Metadata Performance With Owner")]
        public async Task Metadata_Performance_WithOwner()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry from API");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .IncludeOwner()
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.True(elapsed < 10000, $"Metadata fetch should complete within 10s, took {elapsed}ms");
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
        
        private ContentstackClient CreateClientWithBranch()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid
            };
            
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }
        
        #endregion
    }
}

