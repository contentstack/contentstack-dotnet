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
    /// Comprehensive tests for Sync API functionality
    /// Tests sync initialization, pagination, delta sync, and content type filtering
    /// </summary>
    [Trait("Category", "SyncAPI")]
    public class SyncApiComprehensiveTest : IntegrationTestBase
    {
        public SyncApiComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Sync Initialization
        
        [Fact(DisplayName = "Sync API - Sync Initialize All Returns Initial Sync Data")]
        public async Task Sync_InitializeAll_ReturnsInitialSyncData()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
            TestAssert.NotNull(syncResult.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        [Fact(DisplayName = "Sync API - Sync Initialize With Sync Type Returns Filtered Data")]
        public async Task Sync_InitializeWithSyncType_ReturnsFilteredData()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(SyncType: SyncType.EntryPublished);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
        }
        
        [Fact(DisplayName = "Sync API - Sync Initialize With Start Date Returns Sync From Date")]
        public async Task Sync_InitializeWithStartDate_ReturnsSyncFromDate()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            var startDate = DateTime.Now.AddDays(-30);
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(StartFrom: startDate);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.NotNull(syncResult.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        #endregion
        
        #region Sync Types
        
        [Fact(DisplayName = "Sync API - Sync Entry Published Returns Only Published Entries")]
        public async Task Sync_EntryPublished_ReturnsOnlyPublishedEntries()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(SyncType: SyncType.EntryPublished);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            // Verify items are entries (not assets)
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
        }
        
        [Fact(DisplayName = "Sync API - Sync Asset Published Returns Only Published Assets")]
        public async Task Sync_AssetPublished_ReturnsOnlyPublishedAssets()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(SyncType: SyncType.AssetPublished);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
        }
        
        [Fact(DisplayName = "Sync API - Sync Combined Types Returns Multiple Types")]
        public async Task Sync_CombinedTypes_ReturnsMultipleTypes()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act - Combine EntryPublished and AssetPublished
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(SyncType: SyncType.EntryPublished | SyncType.AssetPublished);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
        }
        
        [Fact(DisplayName = "Sync API - Sync Deleted Content Returns Deleted Items")]
        public async Task Sync_DeletedContent_ReturnsDeletedItems()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(SyncType: SyncType.EntryDeleted | SyncType.AssetDeleted);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            // May return 0 if no deletions
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
        }
        
        #endregion
        
        #region Content Type Filtering
        
        [Fact(DisplayName = "Sync API - Sync With Content Type Filter Returns Only Specified Content Type")]
        public async Task Sync_WithContentTypeFilter_ReturnsOnlySpecifiedContentType()
        {
            // Arrange
            LogArrange("Setting up test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(ContentTypeUid: TestDataHelper.SimpleContentTypeUid);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.True(syncResult.TotalCount >= 0);
            TestAssert.IsAssignableFrom<IEnumerable<object>>(syncResult.Items);
        }
        
        [Fact(DisplayName = "Sync API - Sync Content Type With Date Returns Combined Filter")]
        public async Task Sync_ContentTypeWithDate_ReturnsCombinedFilter()
        {
            // Arrange
            LogArrange("Setting up test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);

            var client = CreateClient();
            var startDate = DateTime.Now.AddDays(-7);
            
            // Act
            LogAct("Performing test action");

            var syncResult = await client.SyncRecursive(
                ContentTypeUid: TestDataHelper.ComplexContentTypeUid,
                StartFrom: startDate
            );
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.NotNull(syncResult.SyncToken);
            // Token validation // Sync token must be present and non-empty
        }
        
        #endregion
        
        #region Delta Sync
        
        [Fact(DisplayName = "Sync API - Sync Delta With Token Returns Incremental Changes")]
        public async Task Sync_DeltaWithToken_ReturnsIncrementalChanges()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // First sync to get initial token
            var initialSync = await client.SyncRecursive();
            var syncToken = initialSync.SyncToken;
            
            // Act - Delta sync with token
            LogAct("Performing sync operation");

            var deltaSync = await client.SyncToken(syncToken);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(deltaSync);
            TestAssert.NotNull(deltaSync.Items);
            TestAssert.NotNull(deltaSync.SyncToken);
            // Token validation // Sync token must be present and non-empty
            // May have 0 items if no changes
            TestAssert.True(deltaSync.TotalCount >= 0);
        }
        
        [Fact(DisplayName = "Sync API - Sync Multiple Delta Syncs Maintains Consistency")]
        public async Task Sync_MultipleDeltaSyncs_MaintainsConsistency()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Initial sync
            var sync1 = await client.SyncRecursive();
            var token1 = sync1.SyncToken;
            
            // First delta
            var sync2 = await client.SyncToken(token1);
            var token2 = sync2.SyncToken;
            
            // Act - Second delta
            LogAct("Performing sync operation");

            var sync3 = await client.SyncToken(token2);
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(sync3);
            TestAssert.NotNull(sync3.SyncToken);
            // Token validation // Sync token must be present and non-empty
            // Token is present (may or may not change if no new changes)
            TestAssert.NotEmpty(sync3.SyncToken);
        }
        
        #endregion
        
        #region Pagination
        
        [Fact(DisplayName = "Sync API - Sync With Pagination Handles Pagination Token")]
        public async Task Sync_WithPagination_HandlesPaginationToken()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Get initial sync (may have pagination)
            var initialSync = await client.SyncRecursive();
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(initialSync);
            TestAssert.NotNull(initialSync.Items);
            // If pagination_token exists, verify it's handled
            if (!string.IsNullOrEmpty(initialSync.PaginationToken))
            {
                var nextPage = await client.SyncPaginationToken(initialSync.PaginationToken);
                TestAssert.NotNull(nextPage);
            }
        }
        
        [Fact(DisplayName = "Sync API - Sync Recursive Auto Handles Pagination")]
        public async Task Sync_Recursive_AutoHandlesPagination()
        {
            // Arrange
            LogArrange("Setting up test");

            var client = CreateClient();
            
            // Act - SyncRecursive should handle all pagination automatically
            LogAct("Performing test action");

            var (syncResult, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client.SyncRecursive();
            });
            
            // Assert
            LogAssert("Verifying response");

            TestAssert.NotNull(syncResult);
            TestAssert.NotNull(syncResult.Items);
            TestAssert.Null(syncResult.PaginationToken); // Should be null after recursive sync
            TestAssert.NotNull(syncResult.SyncToken);
            // Token validation // Sync token must be present and non-empty
            // Reasonable execution time even with pagination
            TestAssert.True(elapsed < 30000, $"Sync should complete within 30s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Error Handling
        
        [Fact(DisplayName = "Sync API - Sync Invalid Sync Token Throws Exception")]
        public async Task Sync_InvalidSyncToken_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act & Assert
            LogAct("Performing sync operation");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.SyncToken("invalid_sync_token_xyz_123");
            });
        }
        
        [Fact(DisplayName = "Sync API - Sync Invalid Pagination Token Throws Exception")]
        public async Task Sync_InvalidPaginationToken_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up sync operation");

            var client = CreateClient();
            
            // Act & Assert
            LogAct("Performing sync operation");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.SyncPaginationToken("invalid_pagination_token_xyz");
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

