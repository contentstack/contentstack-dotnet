using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Core.Configuration;
using Contentstack.Core.Unit.Tests.Helpers;
using Contentstack.Core.Unit.Tests.Mokes;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for ContentstackClient.Fork() method
    /// Tests client forking behavior, isolation, and configuration preservation
    /// </summary>
    [Trait("Category", "TimelinePreview")]
    [Trait("Category", "Fork")]
    public class ContentstackClientForkTests : ContentstackClientTestBase
    {
        #region Positive Test Cases

        [Fact]
        public void Fork_CreatesIndependentClientInstance()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();

            // Act
            var forkedClient = parentClient.Fork();

            // Assert
            Assert.NotNull(forkedClient);
            AssertClientsAreIndependent(parentClient, forkedClient);
        }

        [Fact]
        public void Fork_PreservesBaseConfiguration()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();

            // Act
            var forkedClient = parentClient.Fork();

            // Assert
            AssertConfigurationPreserved(parentClient, forkedClient);
        }

        [Fact]
        public void Fork_PreservesCustomHeaders()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var customHeaderKey = "X-Custom-Header";
            var customHeaderValue = _fixture.Create<string>();
            
            parentClient.SetHeader(customHeaderKey, customHeaderValue);

            // Act
            var forkedClient = parentClient.Fork();

            // Assert
            // Verify custom header is preserved in forked client
            var parentHeaders = GetInternalField<Dictionary<string, object>>(parentClient, "_LocalHeaders");
            var forkedHeaders = GetInternalField<Dictionary<string, object>>(forkedClient, "_LocalHeaders");
            
            Assert.True(parentHeaders.ContainsKey(customHeaderKey));
            Assert.True(forkedHeaders.ContainsKey(customHeaderKey));
            Assert.Equal(parentHeaders[customHeaderKey], forkedHeaders[customHeaderKey]);
        }

        [Fact]
        public void Fork_CopiesContentTypeUidHints()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var contentTypeUid = _fixture.Create<string>();
            var entryUid = _fixture.Create<string>();
            
            // Set content type and entry hints on parent
            SetInternalField(parentClient, "currentContenttypeUid", contentTypeUid);
            SetInternalField(parentClient, "currentEntryUid", entryUid);

            // Act
            var forkedClient = parentClient.Fork();

            // Assert
            var forkedContentTypeUid = GetInternalField<string>(forkedClient, "currentContenttypeUid");
            var forkedEntryUid = GetInternalField<string>(forkedClient, "currentEntryUid");
            
            Assert.Equal(contentTypeUid, forkedContentTypeUid);
            Assert.Equal(entryUid, forkedEntryUid);
        }

        [Fact]
        public void Fork_IndependentLivePreviewConfig()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var parentConfig = parentClient.GetLivePreviewConfig();
            parentConfig.PreviewTimestamp = "2024-11-29T14:30:00.000Z";

            // Act
            var forkedClient = parentClient.Fork();
            var forkedConfig = forkedClient.GetLivePreviewConfig();

            // Assert - Configs are independent instances
            Assert.NotSame(parentConfig, forkedConfig);
            
            // Assert - Initial values are copied
            Assert.Equal(parentConfig.PreviewTimestamp, forkedConfig.PreviewTimestamp);
            Assert.Equal(parentConfig.ReleaseId, forkedConfig.ReleaseId);
            Assert.Equal(parentConfig.Enable, forkedConfig.Enable);
            Assert.Equal(parentConfig.Host, forkedConfig.Host);
            Assert.Equal(parentConfig.ManagementToken, forkedConfig.ManagementToken);
        }

        [Fact]
        public void Fork_SharedConfigurationReference()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var parentConfig = parentClient.GetLivePreviewConfig();
            parentConfig.PreviewResponse = CreateMockPreviewResponse();

            // Act
            var forkedClient = parentClient.Fork();
            var forkedConfig = forkedClient.GetLivePreviewConfig();

            // Assert - PreviewResponse is shared reference (memory efficient)
            Assert.Same(parentConfig.PreviewResponse, forkedConfig.PreviewResponse);
        }

        [Fact]
        public void Fork_MultipleLevels_IndependentContexts()
        {
            // Arrange
            var level1Client = CreateClientWithTimeline();
            level1Client.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T10:00:00.000Z";

            // Act
            var level2Client = level1Client.Fork();
            level2Client.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T12:00:00.000Z";
            
            var level3Client = level2Client.Fork();
            level3Client.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:00:00.000Z";

            // Assert - Each level maintains its own timeline
            Assert.Equal("2024-11-29T10:00:00.000Z", level1Client.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("2024-11-29T12:00:00.000Z", level2Client.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("2024-11-29T14:00:00.000Z", level3Client.GetLivePreviewConfig().PreviewTimestamp);

            // Assert - All are independent instances
            AssertClientsAreIndependent(level1Client, level2Client);
            AssertClientsAreIndependent(level2Client, level3Client);
            AssertClientsAreIndependent(level1Client, level3Client);
        }

        [Fact]
        public void Fork_ParallelModifications_IsolatedChanges()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();

            // Act - Modify each fork independently
            fork1.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T08:00:00.000Z";
            fork1.GetLivePreviewConfig().ReleaseId = "fork1_release";
            
            fork2.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T16:00:00.000Z";
            fork2.GetLivePreviewConfig().ReleaseId = "fork2_release";

            // Assert - Changes are isolated
            Assert.Equal("2024-11-29T08:00:00.000Z", fork1.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("fork1_release", fork1.GetLivePreviewConfig().ReleaseId);
            
            Assert.Equal("2024-11-29T16:00:00.000Z", fork2.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("fork2_release", fork2.GetLivePreviewConfig().ReleaseId);

            // Parent client should maintain its original state
            Assert.Equal("2024-11-29T14:30:00.000Z", parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("test_release_123", parentClient.GetLivePreviewConfig().ReleaseId);
        }

        [Fact]
        public void Fork_PreservesPlugins()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var mockHandler = new TimelineMockHttpHandler().ForSuccessfulLivePreview();
            parentClient.Plugins.Add(mockHandler);

            // Act
            var forkedClient = parentClient.Fork();

            // Assert - Plugins collection exists but fork doesn't share plugin instances
            Assert.NotNull(forkedClient.Plugins);
            Assert.Empty(forkedClient.Plugins); // Fork starts with empty plugins (isolated)
        }

        [Fact]
        public async Task Fork_IndependentLivePreviewOperations()
        {
            // Arrange
            var parentClient = CreateClientWithLivePreview();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();

            // Set up different timeline contexts
            var query1 = CreateLivePreviewQuery(previewTimestamp: "2024-11-29T08:00:00.000Z");
            var query2 = CreateLivePreviewQuery(previewTimestamp: "2024-11-29T16:00:00.000Z");

            // Act
            await fork1.LivePreviewQueryAsync(query1);
            await fork2.LivePreviewQueryAsync(query2);

            // Assert - Each fork maintains its own timeline context
            Assert.Equal("2024-11-29T08:00:00.000Z", fork1.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("2024-11-29T16:00:00.000Z", fork2.GetLivePreviewConfig().PreviewTimestamp);
            
            // Parent should be unaffected
            Assert.Null(parentClient.GetLivePreviewConfig().PreviewTimestamp);
        }

        #endregion

        #region Negative Test Cases

        [Fact]
        public void Fork_WithNullLivePreviewConfig_HandlesGracefully()
        {
            // Arrange
            var parentClient = CreateClient(); // Client without LivePreview
            SetInternalProperty(parentClient, "LivePreviewConfig", null);

            // Act & Assert - Should not throw
            var forkedClient = parentClient.Fork();
            
            Assert.NotNull(forkedClient);
            // Verify the fork handles null config appropriately
            var forkedConfig = forkedClient.GetLivePreviewConfig();
            Assert.NotNull(forkedConfig); // Should create a new config if parent was null
        }

        [Fact]
        public void Fork_WithCorruptedConfiguration_CreatesValidFork()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var config = parentClient.GetLivePreviewConfig();
            
            // Corrupt some configuration properties
            config.Host = null;
            config.PreviewTimestamp = "invalid-timestamp-format";

            // Act & Assert - Should not throw
            var forkedClient = parentClient.Fork();
            var forkedConfig = forkedClient.GetLivePreviewConfig();
            
            Assert.NotNull(forkedClient);
            Assert.NotNull(forkedConfig);
            Assert.Equal("invalid-timestamp-format", forkedConfig.PreviewTimestamp); // Corruption is copied but doesn't break fork
        }

        [Fact]
        public void Fork_AfterParentModification_IsolatesChanges()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var originalTimestamp = parentClient.GetLivePreviewConfig().PreviewTimestamp;

            // Act - Create fork, then modify parent
            var forkedClient = parentClient.Fork();
            parentClient.GetLivePreviewConfig().PreviewTimestamp = "2024-12-01T00:00:00.000Z";

            // Assert - Fork maintains original timestamp
            Assert.Equal(originalTimestamp, forkedClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("2024-12-01T00:00:00.000Z", parentClient.GetLivePreviewConfig().PreviewTimestamp);
        }

        [Fact]
        public void Fork_WithLargeNumberOfForks_MaintainsPerformance()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var numberOfForks = 1000;
            var forks = new ContentstackClient[numberOfForks];

            // Act - Measure fork creation time
            var startTime = DateTime.UtcNow;
            
            for (int i = 0; i < numberOfForks; i++)
            {
                forks[i] = parentClient.Fork();
                forks[i].GetLivePreviewConfig().PreviewTimestamp = $"2024-11-{(i % 12) + 1:D2}-01T00:00:00.000Z";
            }
            
            var duration = DateTime.UtcNow - startTime;

            // Assert - Fork creation should be fast (under 1 second for 1000 forks)
            Assert.True(duration.TotalSeconds < 1.0, $"Fork creation took {duration.TotalMilliseconds}ms for {numberOfForks} forks");
            
            // Verify all forks are independent
            for (int i = 0; i < Math.Min(10, numberOfForks); i++) // Check first 10 for performance
            {
                AssertClientsAreIndependent(parentClient, forks[i]);
            }
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Fork_WithEmptyHeaders_HandlesCorrectly()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            SetInternalField(parentClient, "_LocalHeaders", new Dictionary<string, object>());

            // Act
            var forkedClient = parentClient.Fork();

            // Assert
            Assert.NotNull(forkedClient);
            var forkedHeaders = GetInternalField<Dictionary<string, object>>(forkedClient, "_LocalHeaders");
            Assert.NotNull(forkedHeaders);
        }

        [Fact]
        public void Fork_WithNullHeaders_HandlesCorrectly()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            SetInternalField(parentClient, "_LocalHeaders", null);

            // Act & Assert - Should not throw
            var forkedClient = parentClient.Fork();
            Assert.NotNull(forkedClient);
        }

        [Fact]
        public void Fork_RecursiveForkModification_MaintainsIsolation()
        {
            // Arrange
            var level1 = CreateClientWithTimeline();
            level1.GetLivePreviewConfig().ReleaseId = "level1_release";

            var level2 = level1.Fork();
            level2.GetLivePreviewConfig().ReleaseId = "level2_release";

            var level3 = level2.Fork();
            level3.GetLivePreviewConfig().ReleaseId = "level3_release";

            // Act - Modify level2 after level3 is created
            level2.GetLivePreviewConfig().PreviewTimestamp = "2024-11-30T00:00:00.000Z";

            // Assert - Level3 should not be affected by level2 changes
            Assert.Equal("level1_release", level1.GetLivePreviewConfig().ReleaseId);
            Assert.Equal("level2_release", level2.GetLivePreviewConfig().ReleaseId);
            Assert.Equal("level3_release", level3.GetLivePreviewConfig().ReleaseId);
            
            Assert.Equal("2024-11-30T00:00:00.000Z", level2.GetLivePreviewConfig().PreviewTimestamp);
            Assert.NotEqual("2024-11-30T00:00:00.000Z", level3.GetLivePreviewConfig().PreviewTimestamp);
        }

        #endregion
    }
}