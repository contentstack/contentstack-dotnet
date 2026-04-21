using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Unit.Tests.Helpers;
using Contentstack.Core.Unit.Tests.Mokes;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Comprehensive error handling tests for Timeline Preview functionality
    /// Tests exception scenarios, error isolation, and graceful degradation
    /// </summary>
    [Trait("Category", "TimelinePreview")]
    [Trait("Category", "ErrorHandling")]
    [Trait("Category", "Exceptions")]
    public class TimelinePreviewErrorHandlingTests : ContentstackClientTestBase
    {
        #region Invalid Configuration Errors

        [Fact]
        public void Fork_NullConfiguration_HandlesGracefully()
        {
            // Arrange
            var client = CreateClient();
            SetInternalProperty(client, "LivePreviewConfig", null);

            // Act & Assert - Should not throw
            var exception = Record.Exception(() =>
            {
                var fork = client.Fork();
                Assert.NotNull(fork);
            });
            
            Assert.Null(exception);
        }

        [Fact]
        public void ResetLivePreview_CorruptedConfiguration_HandlesGracefully()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Corrupt the configuration
            config.PreviewTimestamp = "invalid-timestamp-format!@#$%";
            config.ReleaseId = null;
            config.PreviewResponse = JObject.Parse("{}"); // Empty/invalid response

            // Act & Assert - Should not throw
            var exception = Record.Exception(() => client.ResetLivePreview());
            Assert.Null(exception);

            // Assert - Configuration should be cleared despite corruption
            Assert.Null(config.PreviewTimestamp);
            Assert.Null(config.PreviewResponse);
        }

        [Fact]
        public void IsCachedPreviewForCurrentQuery_CorruptedFingerprints_HandlesGracefully()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithPreviewResponse()
                .Build();

            // Set corrupted fingerprint data
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "corrupted_timestamp");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "wrong_release");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "invalid_hash");

            // Act & Assert - Should handle gracefully and return false
            var exception = Record.Exception(() =>
            {
                var result = config.IsCachedPreviewForCurrentQuery();
                Assert.False(result); // Corrupted data should result in cache miss
            });
            
            Assert.Null(exception);
        }

        #endregion

        #region Network and Timeout Errors

        [Fact]
        public async Task LivePreviewQueryAsync_NetworkTimeout_HandlesGracefully()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var mockHandler = new TimelineMockHttpHandler().ThrowTimeout();
            client.Plugins.Add(mockHandler);

            var query = CreateLivePreviewQuery(
                previewTimestamp: "2024-11-29T14:30:00.000Z",
                releaseId: "timeout_test_release"
            );

            // Act - method should complete without throwing exception
            await client.LivePreviewQueryAsync(query);

            // Assert - timeout is handled gracefully, preview response is not set
            var config = client.GetLivePreviewConfig();
            Assert.Null(config.PreviewResponse); // Prefetch failed, no preview response set
            Assert.Equal("2024-11-29T14:30:00.000Z", config.PreviewTimestamp); // Basic config still set
            Assert.Equal("timeout_test_release", config.ReleaseId);
        }

        [Fact]
        public async Task LivePreviewQueryAsync_WebException_HandlesGracefully()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var mockHandler = new TimelineMockHttpHandler()
                .ThrowWebException("Network connection failed");
            client.Plugins.Add(mockHandler);

            var query = CreateLivePreviewQuery(
                previewTimestamp: "2024-11-29T14:30:00.000Z",
                releaseId: "network_error_release"
            );

            // Act - method should complete without throwing exception
            await client.LivePreviewQueryAsync(query);

            // Assert - network error is handled gracefully, preview response is not set
            var config = client.GetLivePreviewConfig();
            Assert.Null(config.PreviewResponse); // Prefetch failed, no preview response set
            Assert.Equal("2024-11-29T14:30:00.000Z", config.PreviewTimestamp); // Basic config still set
            Assert.Equal("network_error_release", config.ReleaseId);
        }

        [Fact]
        public async Task EntryFetch_NetworkError_FallsBackGracefully()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var config = client.GetLivePreviewConfig();

            // Set up cache miss scenario (force network request)
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            SetInternalProperty(config, "ContentTypeUID", "error_ct");
            SetInternalProperty(config, "EntryUID", "error_entry");
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "different_timestamp");

            var mockHandler = new TimelineMockHttpHandler()
                .ThrowWebException("Entry fetch failed");
            client.Plugins.Add(mockHandler);

            var contentType = client.ContentType("error_ct");
            var entry = contentType.Entry("error_entry");

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () =>
            {
                await entry.Fetch<JObject>();
            });

            // Network error should propagate but not corrupt client state
            Assert.NotNull(exception);
            
            // Client should remain in consistent state
            Assert.Equal("2024-11-29T14:30:00.000Z", config.PreviewTimestamp);
        }

        #endregion

        #region Concurrency and Threading Errors

        [Fact]
        public async Task ConcurrentOperations_ExceptionInOneThread_DoesNotCorruptOthers()
        {
            // Arrange
            var parentClient = CreateClientWithLivePreview();
            var numberOfThreads = 5;
            var operationsPerThread = 10;
            var exceptionThreadId = 2; // Thread that will throw exceptions

            var results = new ConcurrentBag<OperationResult>();

            // Act - Concurrent operations with exceptions in one thread
            var tasks = new Task[numberOfThreads];
            for (int threadId = 0; threadId < numberOfThreads; threadId++)
            {
                int currentThreadId = threadId;
                tasks[threadId] = Task.Run(async () =>
                {
                    try
                    {
                        for (int i = 0; i < operationsPerThread; i++)
                        {
                            var fork = parentClient.Fork();
                            
                            if (currentThreadId == exceptionThreadId && i >= 5)
                            {
                                // Simulate error conditions
                                throw new InvalidOperationException($"Simulated error in thread {currentThreadId}");
                            }

                            fork.GetLivePreviewConfig().PreviewTimestamp = $"2024-11-29T{currentThreadId:D2}:{i:D2}:00.000Z";
                            fork.GetLivePreviewConfig().ReleaseId = $"thread_{currentThreadId}_op_{i}";

                            // Perform timeline operations
                            var query = CreateLivePreviewQuery(
                                previewTimestamp: fork.GetLivePreviewConfig().PreviewTimestamp,
                                releaseId: fork.GetLivePreviewConfig().ReleaseId
                            );

                            await fork.LivePreviewQueryAsync(query);

                            results.Add(new OperationResult
                            {
                                ThreadId = currentThreadId,
                                OperationId = i,
                                Success = true,
                                Timestamp = fork.GetLivePreviewConfig().PreviewTimestamp
                            });
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        results.Add(new OperationResult
                        {
                            ThreadId = currentThreadId,
                            OperationId = -1,
                            Success = false,
                            Error = ex.Message
                        });
                    }
                });
            }

            await Task.WhenAll(tasks);

            // Assert
            var allResults = results.ToList();
            var successfulResults = allResults.Where(r => r.Success).ToList();
            var failedResults = allResults.Where(r => !r.Success).ToList();

            // Successful threads should complete all operations
            var successfulThreads = successfulResults.GroupBy(r => r.ThreadId)
                .Where(g => g.Key != exceptionThreadId).ToList();
            
            foreach (var threadGroup in successfulThreads)
            {
                Assert.Equal(operationsPerThread, threadGroup.Count());
            }

            // Exception thread should have some failed operations
            Assert.True(failedResults.Any(r => r.ThreadId == exceptionThreadId));

            // Parent client should remain unaffected
            Assert.Null(parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Null(parentClient.GetLivePreviewConfig().ReleaseId);
        }

        [Fact]
        public void ParallelForkCreation_ExceptionDuringFork_DoesNotCorruptParent()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var numberOfForks = 100;
            var corruptionIndex = 50; // Fork that will be corrupted

            var results = new ConcurrentBag<ForkResult>();

            // Act - Parallel fork creation with corruption
            Parallel.For(0, numberOfForks, i =>
            {
                try
                {
                    var fork = parentClient.Fork();
                    
                    if (i == corruptionIndex)
                    {
                        // Simulate corruption of one fork
                        SetInternalProperty(fork, "LivePreviewConfig", null);
                        throw new InvalidOperationException("Fork corruption simulation");
                    }

                    fork.GetLivePreviewConfig().PreviewTimestamp = $"2024-11-29T{i % 24:D2}:00:00.000Z";
                    
                    results.Add(new ForkResult
                    {
                        ForkIndex = i,
                        Success = true,
                        Timestamp = fork.GetLivePreviewConfig().PreviewTimestamp
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new ForkResult
                    {
                        ForkIndex = i,
                        Success = false,
                        Error = ex.Message
                    });
                }
            });

            // Assert
            var allResults = results.ToList();
            var successfulForks = allResults.Where(r => r.Success).ToList();
            var failedForks = allResults.Where(r => !r.Success).ToList();

            // Should have exactly one failure (the corrupted fork)
            Assert.Single(failedForks);
            Assert.Equal(corruptionIndex, failedForks[0].ForkIndex);

            // All other forks should succeed
            Assert.Equal(numberOfForks - 1, successfulForks.Count);

            // Parent client should remain unaffected
            Assert.Equal("2024-11-29T14:30:00.000Z", parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("test_release_123", parentClient.GetLivePreviewConfig().ReleaseId);
        }

        #endregion

        #region Malformed Data Errors

        [Fact]
        public void IsCachedPreviewForCurrentQuery_MalformedTimestamp_HandlesGracefully()
        {
            // Arrange
            var malformedTimestamps = new[]
            {
                "not-a-timestamp",
                "2024-13-50T25:99:99.999Z", // Invalid date/time
                "2024/11/29 14:30:00", // Wrong format
                "2024-11-29", // Incomplete
                "", // Empty string
                "   ", // Whitespace only
                "null", // String "null"
                "undefined" // String "undefined"
            };

            foreach (var malformedTimestamp in malformedTimestamps)
            {
                var config = TimelineTestDataBuilder.New()
                    .WithPreviewTimestamp(malformedTimestamp)
                    .WithPreviewResponse()
                    .Build();

                SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", malformedTimestamp);

                // Act & Assert - Should not throw
                var exception = Record.Exception(() =>
                {
                    var result = config.IsCachedPreviewForCurrentQuery();
                    // Result can be true or false, but shouldn't throw
                });

                Assert.Null(exception);
            }
        }

        [Fact]
        public void Timeline_MalformedReleaseId_HandlesGracefully()
        {
            // Arrange
            var malformedReleaseIds = new[]
            {
                "release\nwith\nnewlines",
                "release\twith\ttabs",
                "release with spaces and special chars !@#$%^&*()",
                new string('x', 10000), // Very long string
                "\0\0\0", // Null characters
                "🚀🎉💯", // Emojis
                "<script>alert('xss')</script>" // Potential XSS
            };

            foreach (var malformedReleaseId in malformedReleaseIds)
            {
                var client = CreateClientWithTimeline();
                
                // Act & Assert - Should not throw
                var exception = Record.Exception(() =>
                {
                    client.GetLivePreviewConfig().ReleaseId = malformedReleaseId;
                    client.ResetLivePreview();
                });

                Assert.Null(exception);
            }
        }

        [Fact]
        public async Task LivePreviewQuery_MalformedQueryParameters_HandlesGracefully()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var mockHandler = new TimelineMockHttpHandler()
                .ForSuccessfulLivePreview();
            client.Plugins.Add(mockHandler);

            var malformedQueries = new[]
            {
                // Null values
                new Dictionary<string, string> { ["content_type_uid"] = null, ["entry_uid"] = "test" },
                
                // Empty values
                new Dictionary<string, string> { ["content_type_uid"] = "", ["entry_uid"] = "" },
                
                // Special characters
                new Dictionary<string, string> 
                {
                    ["content_type_uid"] = "ct\nwith\nnewlines",
                    ["entry_uid"] = "entry\twith\ttabs",
                    ["preview_timestamp"] = "2024-11-29T14:30:00.000Z\0null"
                },
                
                // Very long values
                new Dictionary<string, string>
                {
                    ["content_type_uid"] = new string('x', 5000),
                    ["entry_uid"] = new string('y', 5000)
                }
            };

            foreach (var malformedQuery in malformedQueries)
            {
                // Act & Assert - Should handle gracefully
                var exception = await Record.ExceptionAsync(async () =>
                {
                    await client.LivePreviewQueryAsync(malformedQuery);
                });

                // May throw validation errors, but should not crash or corrupt state
                if (exception != null)
                {
                    Assert.IsNotType<NullReferenceException>(exception);
                    Assert.IsNotType<AccessViolationException>(exception);
                }
            }
        }

        #endregion

        #region Memory and Resource Errors

        [Fact]
        public void Timeline_LargeResponseObjects_HandlesMemoryPressure()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .Build();

            // Create extremely large response to test memory handling
            var largeArray = new JArray();
            for (int i = 0; i < 10000; i++)
            {
                largeArray.Add(JObject.Parse($@"{{
                    ""id"": {i},
                    ""data"": ""{new string('x', 100)}""
                }}"));
            }

            var largeResponse = new JObject
            {
                ["entry"] = new JObject
                {
                    ["uid"] = "large_entry",
                    ["massive_array"] = largeArray,
                    ["large_string"] = new string('y', 50000)
                }
            };

            // Act & Assert - Should handle large objects without crashing
            var exception = Record.Exception(() =>
            {
                config.PreviewResponse = largeResponse;
                SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
                
                // Test cache operations with large response
                var isCached = config.IsCachedPreviewForCurrentQuery();
                Assert.True(isCached);
            });

            Assert.Null(exception);
        }

        [Fact]
        public void Fork_MemoryPressure_HandlesGracefully()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var numberOfForks = 1000;

            // Act & Assert - Should handle many forks without memory issues
            var exception = Record.Exception(() =>
            {
                var forks = new ContentstackClient[numberOfForks];
                
                for (int i = 0; i < numberOfForks; i++)
                {
                    forks[i] = parentClient.Fork();
                    forks[i].GetLivePreviewConfig().PreviewTimestamp = $"2024-11-{(i % 12) + 1:D2}-01T00:00:00.000Z";
                    
                    // Occasionally force garbage collection to test memory pressure
                    if (i % 100 == 0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                // Verify forks are still functional
                for (int i = 0; i < Math.Min(10, numberOfForks); i++)
                {
                    Assert.NotNull(forks[i].GetLivePreviewConfig());
                }
            });

            Assert.Null(exception);
        }

        #endregion

        #region Cleanup and Disposal Errors

        [Fact]
        public void ResetLivePreview_AfterObjectDisposal_HandlesGracefully()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();
            
            // Set up timeline state
            config.PreviewResponse = CreateMockPreviewResponse();
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";

            // Simulate object disposal/corruption
            config.PreviewResponse = null;
            SetInternalProperty(config, "ContentTypeUID", null);
            SetInternalProperty(config, "EntryUID", null);

            // Act & Assert - Should handle cleanup gracefully
            var exception = Record.Exception(() =>
            {
                client.ResetLivePreview();
                
                // Multiple resets should be safe
                client.ResetLivePreview();
                client.ResetLivePreview();
            });

            Assert.Null(exception);
        }

        #endregion

        #region Helper Classes

        public class OperationResult
        {
            public int ThreadId { get; set; }
            public int OperationId { get; set; }
            public bool Success { get; set; }
            public string Timestamp { get; set; }
            public string Error { get; set; }
        }

        public class ForkResult
        {
            public int ForkIndex { get; set; }
            public bool Success { get; set; }
            public string Timestamp { get; set; }
            public string Error { get; set; }
        }

        public class ConcurrentBag<T> : List<T>
        {
            private readonly object _lock = new object();

            public new void Add(T item)
            {
                lock (_lock)
                {
                    base.Add(item);
                }
            }

            public new List<T> ToList()
            {
                lock (_lock)
                {
                    return new List<T>(this);
                }
            }
        }

        #endregion
    }
}