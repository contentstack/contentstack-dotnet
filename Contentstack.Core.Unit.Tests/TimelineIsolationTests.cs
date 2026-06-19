using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    /// Unit tests focused on Timeline Preview isolation behavior
    /// Tests fork independence, parallel operations, and state isolation
    /// </summary>
    [Trait("Category", "TimelinePreview")]
    [Trait("Category", "Isolation")]
    [Trait("Category", "Parallel")]
    public class TimelineIsolationTests : ContentstackClientTestBase
    {
        #region Fork Independence

        [Fact]
        public void ForkIsolation_IndependentTimelineContexts()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();
            var fork3 = parentClient.Fork();

            // Act - Set different timeline contexts
            parentClient.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T08:00:00.000Z";
            parentClient.GetLivePreviewConfig().ReleaseId = "parent_release";

            fork1.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T10:00:00.000Z";
            fork1.GetLivePreviewConfig().ReleaseId = "fork1_release";

            fork2.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T12:00:00.000Z";
            fork2.GetLivePreviewConfig().ReleaseId = "fork2_release";

            fork3.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:00:00.000Z";
            fork3.GetLivePreviewConfig().ReleaseId = "fork3_release";

            // Assert - Each maintains its own timeline context
            Assert.Equal("2024-11-29T08:00:00.000Z", parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("parent_release", parentClient.GetLivePreviewConfig().ReleaseId);

            Assert.Equal("2024-11-29T10:00:00.000Z", fork1.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("fork1_release", fork1.GetLivePreviewConfig().ReleaseId);

            Assert.Equal("2024-11-29T12:00:00.000Z", fork2.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("fork2_release", fork2.GetLivePreviewConfig().ReleaseId);

            Assert.Equal("2024-11-29T14:00:00.000Z", fork3.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("fork3_release", fork3.GetLivePreviewConfig().ReleaseId);
        }

        [Fact]
        public async Task ForkIsolation_ParallelLivePreviewQueryOperations()
        {
            // Arrange
            var parentClient = CreateClientWithLivePreview();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();
            var fork3 = parentClient.Fork();

            var query1 = CreateLivePreviewQuery(
                entryUid: "entry1",
                previewTimestamp: "2024-11-29T08:00:00.000Z",
                releaseId: "release1"
            );

            var query2 = CreateLivePreviewQuery(
                entryUid: "entry2", 
                previewTimestamp: "2024-11-29T12:00:00.000Z",
                releaseId: "release2"
            );

            var query3 = CreateLivePreviewQuery(
                entryUid: "entry3",
                previewTimestamp: "2024-11-29T16:00:00.000Z",
                releaseId: "release3"
            );

            // Act - Parallel operations
            var tasks = new[]
            {
                fork1.LivePreviewQueryAsync(query1),
                fork2.LivePreviewQueryAsync(query2),
                fork3.LivePreviewQueryAsync(query3)
            };

            await Task.WhenAll(tasks);

            // Assert - Each fork maintains its context without interference
            Assert.Equal("2024-11-29T08:00:00.000Z", fork1.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("release1", fork1.GetLivePreviewConfig().ReleaseId);
            
            Assert.Equal("2024-11-29T12:00:00.000Z", fork2.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("release2", fork2.GetLivePreviewConfig().ReleaseId);
            
            Assert.Equal("2024-11-29T16:00:00.000Z", fork3.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("release3", fork3.GetLivePreviewConfig().ReleaseId);

            // Parent should remain unaffected
            Assert.Null(parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Null(parentClient.GetLivePreviewConfig().ReleaseId);
        }

        [Fact]
        public void ForkIsolation_IndependentCacheHitMissBehavior()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();

            // Set up fork1 with cached data (cache hit scenario)
            var fork1Config = fork1.GetLivePreviewConfig();
            fork1Config.PreviewTimestamp = "2024-11-29T10:00:00.000Z";
            fork1Config.ReleaseId = "cached_release";
            fork1Config.PreviewResponse = CreateMockPreviewResponse("entry1", "ct1");
            
            SetInternalProperty(fork1Config, "PreviewResponseFingerprintPreviewTimestamp", fork1Config.PreviewTimestamp);
            SetInternalProperty(fork1Config, "PreviewResponseFingerprintReleaseId", fork1Config.ReleaseId);

            // Set up fork2 with different data (cache miss scenario)
            var fork2Config = fork2.GetLivePreviewConfig();
            fork2Config.PreviewTimestamp = "2024-11-29T14:00:00.000Z";
            fork2Config.ReleaseId = "different_release";
            fork2Config.PreviewResponse = CreateMockPreviewResponse("entry2", "ct2");

            SetInternalProperty(fork2Config, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T08:00:00.000Z"); // Different
            SetInternalProperty(fork2Config, "PreviewResponseFingerprintReleaseId", "old_release"); // Different

            // Act & Assert
            Assert.True(fork1Config.IsCachedPreviewForCurrentQuery()); // Cache hit
            Assert.False(fork2Config.IsCachedPreviewForCurrentQuery()); // Cache miss

            // Verify independence - changes to one don't affect the other
            fork1Config.PreviewTimestamp = "2024-11-29T20:00:00.000Z";
            Assert.False(fork1Config.IsCachedPreviewForCurrentQuery()); // Now cache miss for fork1
            Assert.False(fork2Config.IsCachedPreviewForCurrentQuery()); // Still cache miss for fork2
        }

        [Fact]
        public void ForkIsolation_ResetLivePreview_DoesNotAffectOtherForks()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();
            var fork3 = parentClient.Fork();

            // Set up different states
            parentClient.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T08:00:00.000Z";
            fork1.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T10:00:00.000Z";
            fork2.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T12:00:00.000Z";
            fork3.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:00:00.000Z";

            // Act - Reset only fork2
            fork2.ResetLivePreview();

            // Assert - Only fork2 is affected
            Assert.Equal("2024-11-29T08:00:00.000Z", parentClient.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("2024-11-29T10:00:00.000Z", fork1.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Null(fork2.GetLivePreviewConfig().PreviewTimestamp); // Reset
            Assert.Equal("2024-11-29T14:00:00.000Z", fork3.GetLivePreviewConfig().PreviewTimestamp);
        }

        #endregion

        #region Concurrent Modifications

        [Fact]
        public void ConcurrentModifications_IndependentStates()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var numberOfForks = 10;
            var forks = new ContentstackClient[numberOfForks];
            
            for (int i = 0; i < numberOfForks; i++)
            {
                forks[i] = parentClient.Fork();
            }

            // Act - Concurrent modifications
            var tasks = new Task[numberOfForks];
            for (int i = 0; i < numberOfForks; i++)
            {
                int index = i; // Capture for closure
                tasks[i] = Task.Run(() =>
                {
                    var config = forks[index].GetLivePreviewConfig();
                    config.PreviewTimestamp = $"2024-11-{(index % 12) + 1:D2}-01T{index:D2}:00:00.000Z";
                    config.ReleaseId = $"release_{index}";
                    
                    // Simulate some work
                    Thread.Sleep(10);
                    
                    // Modify again
                    config.PreviewTimestamp = $"2024-11-{(index % 12) + 1:D2}-01T{index:D2}:30:00.000Z";
                });
            }

            Task.WaitAll(tasks);

            // Assert - Each fork should have its final state
            for (int i = 0; i < numberOfForks; i++)
            {
                var expectedTimestamp = $"2024-11-{(i % 12) + 1:D2}-01T{i:D2}:30:00.000Z";
                var expectedReleaseId = $"release_{i}";
                
                Assert.Equal(expectedTimestamp, forks[i].GetLivePreviewConfig().PreviewTimestamp);
                Assert.Equal(expectedReleaseId, forks[i].GetLivePreviewConfig().ReleaseId);
            }
        }

        [Fact]
        public void HighVolume_ParallelForkOperations_NoStateCorruption()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var operationsPerThread = 100;
            var numberOfThreads = 10;
            var allResults = new ConcurrentBag<string>();

            // Act - High volume parallel operations
            var tasks = new Task[numberOfThreads];
            for (int threadId = 0; threadId < numberOfThreads; threadId++)
            {
                int currentThreadId = threadId; // Capture for closure
                tasks[threadId] = Task.Run(() =>
                {
                    for (int i = 0; i < operationsPerThread; i++)
                    {
                        try
                        {
                            var fork = parentClient.Fork();
                            var timestamp = $"2024-11-29T{currentThreadId:D2}:{i % 60:D2}:00.000Z";
                            var releaseId = $"thread_{currentThreadId}_op_{i}";

                            fork.GetLivePreviewConfig().PreviewTimestamp = timestamp;
                            fork.GetLivePreviewConfig().ReleaseId = releaseId;

                            // Verify state immediately
                            var actualTimestamp = fork.GetLivePreviewConfig().PreviewTimestamp;
                            var actualReleaseId = fork.GetLivePreviewConfig().ReleaseId;

                            if (actualTimestamp == timestamp && actualReleaseId == releaseId)
                            {
                                allResults.Add($"SUCCESS_{currentThreadId}_{i}");
                            }
                            else
                            {
                                allResults.Add($"CORRUPTION_{currentThreadId}_{i}_{actualTimestamp}_{actualReleaseId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            allResults.Add($"EXCEPTION_{currentThreadId}_{i}_{ex.Message}");
                        }
                    }
                });
            }

            Task.WaitAll(tasks);

            // Assert - No state corruption or exceptions
            var results = allResults.ToList();
            var expectedResultCount = numberOfThreads * operationsPerThread;
            Assert.Equal(expectedResultCount, results.Count);

            var successResults = results.Where(r => r.StartsWith("SUCCESS")).ToList();
            var corruptionResults = results.Where(r => r.StartsWith("CORRUPTION")).ToList();
            var exceptionResults = results.Where(r => r.StartsWith("EXCEPTION")).ToList();

            Assert.Equal(expectedResultCount, successResults.Count);
            Assert.Empty(corruptionResults);
            Assert.Empty(exceptionResults);
        }

        #endregion

        #region Cache Operations Isolation

        [Fact]
        public void ConcurrentCacheOperations_IsolatedBehavior()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var fork1 = parentClient.Fork();
            var fork2 = parentClient.Fork();
            var fork3 = parentClient.Fork();

            // Set up different cache scenarios
            var response1 = CreateMockPreviewResponse("entry1", "ct1");
            var response2 = CreateMockPreviewResponse("entry2", "ct2");
            var response3 = CreateMockPreviewResponse("entry3", "ct3");

            // Act - Concurrent cache operations
            var tasks = new[]
            {
                Task.Run(() =>
                {
                    var config1 = fork1.GetLivePreviewConfig();
                    config1.PreviewTimestamp = "2024-11-29T10:00:00.000Z";
                    config1.PreviewResponse = response1;
                    SetInternalProperty(config1, "PreviewResponseFingerprintPreviewTimestamp", config1.PreviewTimestamp);
                    SetInternalProperty(config1, "PreviewResponseFingerprintReleaseId", config1.ReleaseId);
                    SetInternalProperty(config1, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(config1, "LivePreview"));
                }),
                Task.Run(() =>
                {
                    var config2 = fork2.GetLivePreviewConfig();
                    config2.ReleaseId = "concurrent_release";
                    config2.PreviewResponse = response2;
                    SetInternalProperty(config2, "PreviewResponseFingerprintPreviewTimestamp", config2.PreviewTimestamp);
                    SetInternalProperty(config2, "PreviewResponseFingerprintReleaseId", config2.ReleaseId);
                    SetInternalProperty(config2, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(config2, "LivePreview"));
                }),
                Task.Run(() =>
                {
                    var config3 = fork3.GetLivePreviewConfig();
                    SetInternalProperty(config3, "LivePreview", "concurrent_hash");
                    config3.PreviewResponse = response3;
                    SetInternalProperty(config3, "PreviewResponseFingerprintPreviewTimestamp", config3.PreviewTimestamp);
                    SetInternalProperty(config3, "PreviewResponseFingerprintReleaseId", config3.ReleaseId);
                    SetInternalProperty(config3, "PreviewResponseFingerprintLivePreview", GetInternalProperty<string>(config3, "LivePreview"));
                })
            };

            Task.WaitAll(tasks);

            // Assert - Each fork has its own cache state
            Assert.Same(response1, fork1.GetLivePreviewConfig().PreviewResponse);
            Assert.True(fork1.GetLivePreviewConfig().IsCachedPreviewForCurrentQuery());

            Assert.Same(response2, fork2.GetLivePreviewConfig().PreviewResponse);
            Assert.True(fork2.GetLivePreviewConfig().IsCachedPreviewForCurrentQuery());

            Assert.Same(response3, fork3.GetLivePreviewConfig().PreviewResponse);
            Assert.True(fork3.GetLivePreviewConfig().IsCachedPreviewForCurrentQuery());

            // Cross-verification - each fork doesn't match others' fingerprints
            fork1.GetLivePreviewConfig().ReleaseId = "concurrent_release";
            Assert.False(fork1.GetLivePreviewConfig().IsCachedPreviewForCurrentQuery()); // Different fingerprint
        }

        #endregion

        #region Error Isolation

        [Fact]
        public void ErrorIsolation_InvalidTimestamp_DoesNotAffectOtherForks()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var validFork = parentClient.Fork();
            var invalidFork = parentClient.Fork();

            // Set up valid state for validFork
            validFork.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            validFork.GetLivePreviewConfig().ReleaseId = "valid_release";

            // Act - Set invalid timestamp on invalidFork
            invalidFork.GetLivePreviewConfig().PreviewTimestamp = "invalid-timestamp-format";
            invalidFork.GetLivePreviewConfig().ReleaseId = "invalid_release";

            // Assert - validFork is unaffected by invalid state in invalidFork
            Assert.Equal("2024-11-29T14:30:00.000Z", validFork.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("valid_release", validFork.GetLivePreviewConfig().ReleaseId);

            Assert.Equal("invalid-timestamp-format", invalidFork.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("invalid_release", invalidFork.GetLivePreviewConfig().ReleaseId);
        }

        [Fact]
        public void ErrorIsolation_ExceptionInOneFork_DoesNotCorruptOthers()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var stableFork = parentClient.Fork();
            var unstableFork = parentClient.Fork();

            // Set up stable state
            stableFork.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            stableFork.GetLivePreviewConfig().PreviewResponse = CreateMockPreviewResponse();

            // Act & Assert - Exception in unstable fork doesn't affect stable fork
            var stableTask = Task.Run(() =>
            {
                // Stable operations
                for (int i = 0; i < 100; i++)
                {
                    stableFork.GetLivePreviewConfig().PreviewTimestamp = $"2024-11-29T14:{i % 60:D2}:00.000Z";
                    Assert.NotNull(stableFork.GetLivePreviewConfig().PreviewTimestamp);
                }
            });

            var unstableTask = Task.Run(() =>
            {
                // Potentially problematic operations
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (i == 50)
                        {
                            // Simulate error condition
                            throw new InvalidOperationException("Simulated error in unstable fork");
                        }
                        unstableFork.GetLivePreviewConfig().ReleaseId = $"unstable_release_{i}";
                    }
                }
                catch (InvalidOperationException)
                {
                    // Expected exception - should not affect stable fork
                }
            });

            Task.WaitAll(stableTask, unstableTask);

            // Assert - Stable fork completed successfully
            Assert.NotNull(stableFork.GetLivePreviewConfig().PreviewTimestamp);
            Assert.True(stableFork.GetLivePreviewConfig().PreviewTimestamp.StartsWith("2024-11-29T14:"));

            // Unstable fork state might be partially set, but stable fork is unaffected
            Assert.NotEqual(stableFork.GetLivePreviewConfig().ReleaseId, 
                          unstableFork.GetLivePreviewConfig().ReleaseId);
        }

        [Fact]
        public void ErrorIsolation_NullReferenceInOneFork_IsolatedFailure()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var workingFork = parentClient.Fork();
            var problematicFork = parentClient.Fork();

            // Set up working state
            workingFork.GetLivePreviewConfig().PreviewTimestamp = "2024-11-29T14:30:00.000Z";

            // Act - Cause null reference in problematic fork
            SetInternalProperty(problematicFork, "LivePreviewConfig", null);

            // Assert - Working fork should continue functioning
            Assert.Equal("2024-11-29T14:30:00.000Z", workingFork.GetLivePreviewConfig().PreviewTimestamp);

            // Operations on working fork should continue to work
            workingFork.GetLivePreviewConfig().ReleaseId = "still_working";
            Assert.Equal("still_working", workingFork.GetLivePreviewConfig().ReleaseId);

            // Problematic fork operations might throw, but don't affect working fork
            var exception = Record.Exception(() => problematicFork.ResetLivePreview());
            // Exception is acceptable for problematic fork, but working fork remains unaffected
            
            Assert.Equal("2024-11-29T14:30:00.000Z", workingFork.GetLivePreviewConfig().PreviewTimestamp);
            Assert.Equal("still_working", workingFork.GetLivePreviewConfig().ReleaseId);
        }

        #endregion

        #region Memory and Resource Isolation

        [Fact]
        public void ResourceIsolation_IndependentMemoryFootprint()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var numberOfForks = 100;
            var forks = new ContentstackClient[numberOfForks];

            // Create large response objects to test memory isolation
            var largeResponse = JObject.Parse($@"{{
                ""entry"": {{
                    ""uid"": ""large_entry"",
                    ""large_field"": ""{new string('x', 1000)}"",
                    ""another_large_field"": ""{new string('y', 1000)}""
                }}
            }}");

            // Act - Create forks with independent large objects
            for (int i = 0; i < numberOfForks; i++)
            {
                forks[i] = parentClient.Fork();
                forks[i].GetLivePreviewConfig().PreviewResponse = JObject.Parse(largeResponse.ToString());
                forks[i].GetLivePreviewConfig().PreviewTimestamp = $"2024-11-29T{i % 24:D2}:00:00.000Z";
            }

            // Assert - Each fork should have its own copy
            for (int i = 0; i < numberOfForks; i++)
            {
                Assert.NotNull(forks[i].GetLivePreviewConfig().PreviewResponse);
                Assert.Equal($"2024-11-29T{i % 24:D2}:00:00.000Z", 
                           forks[i].GetLivePreviewConfig().PreviewTimestamp);
            }

            // Modify one fork's response - others should be unaffected
            forks[0].GetLivePreviewConfig().PreviewResponse["entry"]["uid"] = "modified_entry";
            
            for (int i = 1; i < Math.Min(10, numberOfForks); i++) // Check first 10 for performance
            {
                Assert.Equal("large_entry", 
                           forks[i].GetLivePreviewConfig().PreviewResponse["entry"]["uid"].ToString());
            }
        }

        #endregion
    }

    /// <summary>
    /// Thread-safe collection for concurrent testing
    /// </summary>
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
}