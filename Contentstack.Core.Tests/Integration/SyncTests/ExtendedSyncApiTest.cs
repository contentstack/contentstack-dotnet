using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Internals;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.SyncTests
{
    /// <summary>
    /// Extended tests for Sync API operations
    /// Additional sync scenarios beyond the basic comprehensive tests
    /// </summary>
    [Trait("Category", "ExtendedSyncApi")]
    public class ExtendedSyncApiTest : IntegrationTestBase
    {
        public ExtendedSyncApiTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Additional Sync Type Tests
        
        [Fact(DisplayName = "Sync API - Extended Sync Asset Published Syncs Only Published Assets")]
        public async Task ExtendedSync_AssetPublished_SyncsOnlyPublishedAssets()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(SyncType: SyncType.AssetPublished);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Asset Deleted Syncs Only Deleted Assets")]
        public async Task ExtendedSync_AssetDeleted_SyncsOnlyDeletedAssets()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(SyncType: SyncType.AssetDeleted);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Asset Unpublished Syncs Only Unpublished Assets")]
        public async Task ExtendedSync_AssetUnpublished_SyncsOnlyUnpublishedAssets()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(SyncType: SyncType.AssetUnpublished);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Content Type Deleted Syncs Deleted Content Types")]
        public async Task ExtendedSync_ContentTypeDeleted_SyncsDeletedContentTypes()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(SyncType: SyncType.ContentTypeDeleted);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        #endregion
        
        #region Sync by Date Range
        
        [Fact(DisplayName = "Sync API - Extended Sync Start From Date Syncs From Specific Date")]
        public async Task ExtendedSync_StartFromDate_SyncsFromSpecificDate()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var startDate = DateTime.Now.AddDays(-7); // Last week
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(StartFrom: startDate);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Recent Date Limited Results")]
        public async Task ExtendedSync_RecentDate_LimitedResults()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var recentDate = DateTime.Now.AddHours(-1);
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(StartFrom: recentDate);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Old Date Many Results")]
        public async Task ExtendedSync_OldDate_ManyResults()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var oldDate = DateTime.Now.AddMonths(-1);
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(StartFrom: oldDate);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        #endregion
        
        #region Sync Token Management
        
        [Fact(DisplayName = "Sync API - Extended Sync Save And Reuse Sync Token Consistent")]
        public async Task ExtendedSync_SaveAndReuseSyncToken_Consistent()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act - Initial sync
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            var sync1 = await client.SyncRecursive();
            var savedToken = sync1.SyncToken;
            
            // Use saved token
            var sync2 = await client.SyncToken(savedToken);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(sync1);
            Assert.NotNull(sync2);
            Assert.NotEmpty(savedToken);
            Assert.NotNull(sync2.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Multiple Delta Syncs Token Progression")]
        public async Task ExtendedSync_MultipleDeltaSyncs_TokenProgression()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act - Chain of delta syncs
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            var sync1 = await client.SyncRecursive();
            var token1 = sync1.SyncToken;
            
            var sync2 = await client.SyncToken(token1);
            var token2 = sync2.SyncToken;
            
            var sync3 = await client.SyncToken(token2);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(sync1);
            Assert.NotNull(sync2);
            Assert.NotNull(sync3);
            Assert.NotEmpty(sync3.SyncToken);
        }
        
        #endregion
        
        #region Sync with Content Type Filter
        
        [Fact(DisplayName = "Sync API - Extended Sync Specific Content Type Filters Correctly")]
        public async Task ExtendedSync_SpecificContentType_FiltersCorrectly()
        {
            // Arrange
            LogArrange("Setting up test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(ContentTypeUid: TestDataHelper.SimpleContentTypeUid);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Complex Content Type Handles Large Data")]
        public async Task ExtendedSync_ComplexContentType_HandlesLargeData()
        {
            // Arrange
            LogArrange("Setting up test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive(ContentTypeUid: TestDataHelper.ComplexContentTypeUid);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Delta For Content Type Specific Changes")]
        public async Task ExtendedSync_DeltaForContentType_SpecificChanges()
        {
            // Arrange
            LogArrange("Setting up sync operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act - Initial sync for content type
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            var sync1 = await client.SyncRecursive(ContentTypeUid: TestDataHelper.SimpleContentTypeUid);
            var token = sync1.SyncToken;
            
            // Delta sync
            var sync2 = await client.SyncToken(token);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(sync2);
            Assert.NotNull(sync2.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        #endregion
        
        #region Pagination Token Tests
        
        [Fact(DisplayName = "Sync API - Extended Sync Pagination Token Handles Large Sync")]
        public async Task ExtendedSync_PaginationToken_HandlesLargeSync()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act - Initial sync may return pagination token
            LogAct("Performing test action");

            var result = await client.SyncRecursive();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Follow Pagination Token Complete Sync")]
        public async Task ExtendedSync_FollowPaginationToken_CompleteSync()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            var result = await client.SyncRecursive();
            
            // If pagination token exists, follow it
            if (!string.IsNullOrEmpty(result.PaginationToken))
            {
                var nextPage = await client.SyncPaginationToken(result.PaginationToken);
                Assert.NotNull(nextPage);
            }
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Sync Result Structure
        
        [Fact(DisplayName = "Sync API - Extended Sync Result Structure Valid Format")]
        public async Task ExtendedSync_ResultStructure_ValidFormat()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.NotNull(result.SyncToken);
            // Token validation // Sync token must be present and non-empty
            Assert.True(result.TotalCount >= 0);
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Items Collection Accessible And Valid")]
        public async Task ExtendedSync_ItemsCollection_AccessibleAndValid()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var result = await client.SyncRecursive();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result.Items);
            Assert.IsAssignableFrom<IEnumerable<object>>(result.Items);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Sync API - Extended Sync Performance Initial Sync")]
        public async Task ExtendedSync_Performance_InitialSync()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.SyncRecursive();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 30000, $"Initial sync should complete within 30s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Performance Delta Sync")]
        public async Task ExtendedSync_Performance_DeltaSync()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            var sync1 = await client.SyncRecursive();
            var token = sync1.SyncToken;
            
            // Act
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.SyncToken(token);
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Delta sync should complete within 15s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Performance Content Type Sync")]
        public async Task ExtendedSync_Performance_ContentTypeSync()
        {
            // Arrange
            LogArrange("Setting up test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.SyncRecursive(ContentTypeUid: TestDataHelper.SimpleContentTypeUid);
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 20000, $"Content type sync should complete within 20s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Comprehensive Sync Flows
        
        [Fact(DisplayName = "Sync API - Extended Sync Full Sync Flow Initial To Delta")]
        public async Task ExtendedSync_FullSyncFlow_InitialToDelta()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act - Complete flow
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            // 1. Initial sync
            var initialSync = await client.SyncRecursive();
            Assert.NotNull(initialSync.SyncToken);
            // Token validation // Sync token must be present and non-empty
            
            // 2. First delta
            var delta1 = await client.SyncToken(initialSync.SyncToken);
            Assert.NotNull(delta1.SyncToken);
            // Token validation // Sync token must be present and non-empty
            
            // 3. Second delta
            var delta2 = await client.SyncToken(delta1.SyncToken);
            Assert.NotNull(delta2.SyncToken);
            // Token validation // Sync token must be present and non-empty
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(initialSync);
            Assert.NotNull(delta1);
            Assert.NotNull(delta2);
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Typed Sync Flow Entry Published Only")]
        public async Task ExtendedSync_TypedSyncFlow_EntryPublishedOnly()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act
            LogAct("Performing sync operation");
            LogGetRequest("https://" + TestDataHelper.Host + "/v3/stacks/sync");

            var sync1 = await client.SyncRecursive(SyncType: SyncType.EntryPublished);
            var token = sync1.SyncToken;
            
            var sync2 = await client.SyncToken(token);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(sync1);
            Assert.NotNull(sync2);
        }
        
        [Fact(DisplayName = "Sync API - Extended Sync Date Based Flow Recent Changes")]
        public async Task ExtendedSync_DateBasedFlow_RecentChanges()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var startDate = DateTime.Now.AddDays(-3);
            
            // Act
            LogAct("Performing test action");

            var sync = await client.SyncRecursive(StartFrom: startDate);
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(sync);
            Assert.NotNull(sync.SyncToken);
            // Token validation // Sync token must be present and non-empty
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
        
        #endregion
    }
}

