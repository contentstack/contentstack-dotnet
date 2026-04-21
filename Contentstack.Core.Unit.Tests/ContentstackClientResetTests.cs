using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Core.Configuration;
using Contentstack.Core.Unit.Tests.Helpers;
using Contentstack.Core.Unit.Tests.Mokes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for ContentstackClient.ResetLivePreview() method
    /// Tests timeline state clearing, configuration preservation, and edge cases
    /// </summary>
    [Trait("Category", "TimelinePreview")]
    [Trait("Category", "Reset")]
    public class ContentstackClientResetTests : ContentstackClientTestBase
    {
        #region Positive Test Cases

        [Fact]
        public void ResetLivePreview_ClearsTimelineProperties()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set timeline properties
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            config.ReleaseId = "test_release_123";

            // Act
            client.ResetLivePreview();

            // Assert
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.ReleaseId);
        }

        [Fact]
        public void ResetLivePreview_ClearsPreviewResponse()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            config.PreviewResponse = CreateMockPreviewResponse();

            // Verify response is set
            Assert.NotNull(config.PreviewResponse);

            // Act
            client.ResetLivePreview();

            // Assert
            Assert.Null(config.PreviewResponse);
        }

        [Fact]
        public void ResetLivePreview_ClearsFingerprintProperties()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();

            // Set fingerprint properties
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "fingerprint_timestamp");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "fingerprint_release");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "fingerprint_hash");

            // Act
            client.ResetLivePreview();

            // Assert
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview"));
        }

        [Fact]
        public void ResetLivePreview_ClearsContentTypeContext()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set content type context
            SetInternalProperty(config, "ContentTypeUID", "test_content_type");

            // Act
            client.ResetLivePreview();

            // Assert
            Assert.Null(GetInternalProperty<string>(config, "ContentTypeUID"));
        }

        [Fact]
        public void ResetLivePreview_ClearsEntryContext()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set entry context
            SetInternalProperty(config, "EntryUID", "test_entry");

            // Act
            client.ResetLivePreview();

            // Assert
            Assert.Null(GetInternalProperty<string>(config, "EntryUID"));
        }

        [Fact]
        public void ResetLivePreview_ClearsLivePreviewHash()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set live preview hash
            SetInternalProperty(config, "LivePreview", "test_hash_123");

            // Act
            client.ResetLivePreview();

            // Assert
            Assert.Null(GetInternalProperty<string>(config, "LivePreview"));
        }

        [Fact]
        public void ResetLivePreview_PreservesBaseConfiguration()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            var originalEnable = config.Enable;
            var originalHost = config.Host;
            var originalManagementToken = config.ManagementToken;
            var originalPreviewToken = config.PreviewToken;

            // Act
            client.ResetLivePreview();

            // Assert - Base configuration should be preserved
            Assert.Equal(originalEnable, config.Enable);
            Assert.Equal(originalHost, config.Host);
            Assert.Equal(originalManagementToken, config.ManagementToken);
            Assert.Equal(originalPreviewToken, config.PreviewToken);
        }

        [Fact]
        public void ResetLivePreview_PreservesClientConfiguration()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            
            var originalApiKey = client.GetApplicationKey();
            var originalAccessToken = client.GetAccessToken();
            var originalEnvironment = client.GetEnvironment();
            var originalVersion = client.GetVersion();

            // Act
            client.ResetLivePreview();

            // Assert - Client configuration should be preserved
            Assert.Equal(originalApiKey, client.GetApplicationKey());
            Assert.Equal(originalAccessToken, client.GetAccessToken());
            Assert.Equal(originalEnvironment, client.GetEnvironment());
            Assert.Equal(originalVersion, client.GetVersion());
        }

        [Fact]
        public void ResetLivePreview_MultipleCallsIdempotent()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set up timeline state
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            config.ReleaseId = "test_release";
            config.PreviewResponse = CreateMockPreviewResponse();

            // Act - Multiple resets
            client.ResetLivePreview();
            client.ResetLivePreview();
            client.ResetLivePreview();

            // Assert - Should be safe to call multiple times
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.ReleaseId);
            Assert.Null(config.PreviewResponse);
        }

        [Fact]
        public void ResetLivePreview_AfterComplexTimelineOperations()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();

            // Set up complex timeline state
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            config.ReleaseId = "complex_release_123";
            config.PreviewResponse = JObject.Parse(@"{
                ""entry"": {
                    ""uid"": ""complex_entry"",
                    ""title"": ""Complex Test Entry"",
                    ""nested"": {
                        ""deep"": {
                            ""structure"": ""value""
                        }
                    },
                    ""array"": [1, 2, 3, 4, 5]
                }
            }");
            
            SetInternalProperty(config, "ContentTypeUID", "complex_ct");
            SetInternalProperty(config, "EntryUID", "complex_entry");
            SetInternalProperty(config, "LivePreview", "complex_hash");
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "complex_timestamp");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "complex_release");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "complex_hash");

            // Act
            client.ResetLivePreview();

            // Assert - All complex state cleared
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.ReleaseId);
            Assert.Null(config.PreviewResponse);
            Assert.Null(GetInternalProperty<string>(config, "ContentTypeUID"));
            Assert.Null(GetInternalProperty<string>(config, "EntryUID"));
            Assert.Null(GetInternalProperty<string>(config, "LivePreview"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview"));
        }

        [Fact]
        public async Task ResetLivePreview_AfterLivePreviewQuery_ClearsAllState()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var mockHandler = new TimelineMockHttpHandler().ForSuccessfulLivePreview();
            client.Plugins.Add(mockHandler);

            var query = CreateLivePreviewQuery(
                contentTypeUid: "reset_test_ct",
                entryUid: "reset_test_entry",
                previewTimestamp: "2024-11-29T14:30:00.000Z",
                releaseId: "reset_test_release"
            );

            // Execute live preview query to set up state
            await client.LivePreviewQueryAsync(query);
            
            var config = client.GetLivePreviewConfig();
            
            // Verify state is set
            Assert.Equal("2024-11-29T14:30:00.000Z", config.PreviewTimestamp);
            Assert.Equal("reset_test_release", config.ReleaseId);

            // Act
            client.ResetLivePreview();

            // Assert - All state cleared
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.ReleaseId);
            Assert.Null(config.PreviewResponse);
            Assert.Null(GetInternalProperty<string>(config, "ContentTypeUID"));
            Assert.Null(GetInternalProperty<string>(config, "EntryUID"));
            Assert.Null(GetInternalProperty<string>(config, "LivePreview"));
        }

        [Fact]
        public void ResetLivePreview_DoesNotAffectForkedClients()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var forkedClient = parentClient.Fork();

            // Set different timeline states
            parentClient.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T10:00:00.000Z";
            forkedClient.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:00:00.000Z";

            // Act - Reset parent client only
            parentClient.ResetLivePreview();

            // Assert - Parent is reset but fork is unaffected
            Assert.Null(parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("2024-11-29T14:00:00.000Z", forkedClient.GetLivePreviewConfig().PreviewTimestamp);
        }

        #endregion

        #region Negative Test Cases

        [Fact]
        public void ResetLivePreview_WithNullConfig_HandlesGracefully()
        {
            // Arrange
            var client = CreateClient();
            SetInternalProperty(client, "LivePreviewConfig", null);

            // Act & Assert - Should not throw
            var exception = Record.Exception(() => client.ResetLivePreview());
            Assert.Null(exception);
        }

        [Fact]
        public void ResetLivePreview_DisabledLivePreview_NoException()
        {
            // Arrange
            var client = CreateClientWithLivePreview(enabled: false);

            // Act & Assert - Should not throw
            var exception = Record.Exception(() => client.ResetLivePreview());
            Assert.Null(exception);
        }

        [Fact]
        public void ResetLivePreview_AlreadyClearedState_HandlesCorrectly()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();

            // Clear all state first
            config.PreviewTimestamp = null;
            config.ReleaseId = null;
            config.PreviewResponse = null;
            SetInternalProperty(config, "ContentTypeUID", null);
            SetInternalProperty(config, "EntryUID", null);
            SetInternalProperty(config, "LivePreview", null);

            // Act & Assert - Should not throw with already cleared state
            var exception = Record.Exception(() => client.ResetLivePreview());
            Assert.Null(exception);
            
            // Verify state remains cleared
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.ReleaseId);
            Assert.Null(config.PreviewResponse);
        }

        [Fact]
        public void ResetLivePreview_WithCorruptedState_HandlesGracefully()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();

            // Create corrupted state
            config.PreviewTimestamp = "invalid-timestamp-format";
            config.PreviewResponse = JObject.Parse("{}"); // Empty invalid response

            // Act & Assert - Should not throw with corrupted state
            var exception = Record.Exception(() => client.ResetLivePreview());
            Assert.Null(exception);

            // Assert - Corrupted state is cleared
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.PreviewResponse);
        }

        #endregion

        #region Performance and Edge Cases

        [Fact]
        public void ResetLivePreview_Performance_FastExecution()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set up state to reset
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            config.ReleaseId = "perf_test_release";
            config.PreviewResponse = CreateMockPreviewResponse();

            var iterations = 1000;
            var startTime = DateTime.UtcNow;

            // Act - Multiple resets to test performance
            for (int i = 0; i < iterations; i++)
            {
                // Set some state
                config.PreviewTimestamp = $"2024-11-{(i % 12) + 1:D2}-01T00:00:00.000Z";
                config.ReleaseId = $"perf_release_{i}";
                
                // Reset
                client.ResetLivePreview();
            }

            var duration = DateTime.UtcNow - startTime;

            // Assert - Should be very fast (under 100ms for 1000 resets)
            Assert.True(duration.TotalMilliseconds < 100, 
                $"ResetLivePreview took {duration.TotalMilliseconds}ms for {iterations} operations");
        }

        [Fact]
        public void ResetLivePreview_ConcurrentCalls_ThreadSafe()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";

            var tasks = new Task[10];

            // Act - Concurrent reset calls
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    try
                    {
                        client.ResetLivePreview();
                    }
                    catch (Exception ex)
                    {
                        // Should not throw
                        throw new Exception($"Concurrent reset failed: {ex.Message}", ex);
                    }
                });
            }

            // Assert - All tasks should complete without exception
            var exception = Record.Exception(() => Task.WaitAll(tasks));
            Assert.Null(exception);

            // Final state should be cleared
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.ReleaseId);
            Assert.Null(config.PreviewResponse);
        }

        [Fact]
        public void ResetLivePreview_MemoryEfficiency_ReleasesReferences()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();

            // Create large objects to test memory release
            var largeResponse = JObject.Parse(@"{
                ""entry"": {
                    ""large_field"": """ + new string('x', 10000) + @""",
                    ""another_large_field"": """ + new string('y', 10000) + @"""
                }
            }");
            
            config.PreviewResponse = largeResponse;

            // Act
            client.ResetLivePreview();

            // Force garbage collection
            largeResponse = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Assert - References should be cleared
            Assert.Null(config.PreviewResponse);
        }

        #endregion
    }
}