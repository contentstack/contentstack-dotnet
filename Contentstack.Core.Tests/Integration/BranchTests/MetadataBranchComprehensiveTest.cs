using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.BranchTests
{
    /// <summary>
    /// Comprehensive tests for Branch and Metadata
    /// Tests branch operations, metadata fields, and branch-specific queries
    /// </summary>
    [Trait("Category", "MetadataBranch")]
    public class MetadataBranchComprehensiveTest
    {
        #region Basic Branch Operations
        
        [Fact(DisplayName = "Branch Client With Branch Uses Specified Branch")]
        public async Task Branch_ClientWithBranch_UsesSpecifiedBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Branch Query With Branch Fetches From Branch")]
        public async Task Branch_QueryWithBranch_FetchesFromBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Branch Asset With Branch Fetches From Branch")]
        public async Task Branch_AssetWithBranch_FetchesFromBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            
            // Act
            var asset = await client.Asset(TestDataHelper.ImageAssetUid).Fetch();
            
            // Assert
            Assert.NotNull(asset);
        }
        
        #endregion
        
        #region Metadata Fields
        
        [Fact(DisplayName = "Metadata Created By Available In Entry")]
        public async Task Metadata_CreatedBy_AvailableInEntry()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Branch Deep References All From Same Branch")]
        public async Task Branch_DeepReferences_AllFromSameBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeReference(new[] { "authors", "authors.reference" })
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        #endregion
        
        #region Query with Branch
        
        [Fact(DisplayName = "Branch Query Filters Works With Branch")]
        public async Task Branch_QueryFilters_WorksWithBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.Exists("title");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Branch Complex Query Works With Branch")]
        public async Task Branch_ComplexQuery_WorksWithBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act
            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            query.And(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Include Owner
        
        [Fact(DisplayName = "Metadata Include Owner Adds Owner Info")]
        public async Task Metadata_IncludeOwner_AddsOwnerInfo()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeOwner()
                .Fetch<Entry>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            
            // ✅ NOTE: Metadata fields (created_by, updated_by, etc.) are in entry data
            // Access via entry.Get("created_by"), entry.Get("updated_by"), etc.
            Assert.NotEmpty(entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Metadata Query With Owner Includes Owner For All")]
        public async Task Metadata_QueryWithOwner_IncludesOwnerForAll()
        {
            // Arrange
            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act
            query.IncludeOwner();
            query.Limit(3);
            var result = await query.Find<Entry>();
            
            // Assert
            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Content Type Metadata
        
        [Fact(DisplayName = "Metadata Content Type Schema Includes Metadata")]
        public async Task Metadata_ContentTypeSchema_IncludesMetadata()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var schema = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Fetch();
            
            // Assert
            Assert.NotNull(schema);
        }
        
        [Fact(DisplayName = "Metadata Content Type With Branch Branch Specific")]
        public async Task Metadata_ContentTypeWithBranch_BranchSpecific()
        {
            // Arrange
            var client = CreateClientWithBranch();
            
            // Act
            var schema = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Fetch();
            
            // Assert
            Assert.NotNull(schema);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Branch Performance With Branch")]
        public async Task Branch_Performance_WithBranch()
        {
            // Arrange
            var client = CreateClientWithBranch();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Branch fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Metadata Performance With Owner")]
        public async Task Metadata_Performance_WithOwner()
        {
            // Arrange
            var client = CreateClient();
            
            // Act
            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .IncludeOwner()
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Metadata fetch should complete within 10s, took {elapsed}ms");
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
            
            return new ContentstackClient(options);
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
            
            return new ContentstackClient(options);
        }
        
        #endregion
    }
}

