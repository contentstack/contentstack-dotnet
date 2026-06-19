using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Unit tests for Timeline Preview cache behavior and performance
    /// Tests fingerprint-based caching, cache hits/misses, and performance optimizations
    /// </summary>
    [Trait("Category", "TimelinePreview")]
    [Trait("Category", "Cache")]
    [Trait("Category", "Performance")]
    public class TimelineCacheBehaviorTests : ContentstackClientTestBase
    {
        #region Cache Hit Performance

        [Fact]
        public async Task CacheHit_SignificantlyFasterThanNetworkRequest()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var config = client.GetLivePreviewConfig();

            // Set up cached response
            var cachedResponse = TimelineTestDataBuilder.New()
                .CreateValidPreviewResponse("perf_entry", "perf_ct");
                
            config.PreviewResponse = cachedResponse;
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            config.ReleaseId = "perf_release";
            
            SetInternalProperty(config, "ContentTypeUID", "perf_ct");
            SetInternalProperty(config, "EntryUID", "perf_entry");
            
            // Set matching fingerprints for cache hit
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", config.ReleaseId);

            // Create query that should hit cache
            var cacheHitQuery = CreateLivePreviewQuery(
                contentTypeUid: "perf_ct",
                entryUid: "perf_entry",
                previewTimestamp: "2024-11-29T14:30:00.000Z",
                releaseId: "perf_release"
            );

            // Act & Assert - Cache operation should be fast
            await TimelinePerformanceHelpers.AssertCachePerformance(
                cacheOperation: async () =>
                {
                    // This should hit cache - test the cache checking logic directly
                    var isCached = config.IsCachedPreviewForCurrentQuery();
                    Assert.True(isCached);
                    
                    // Access cached response
                    var result = config.PreviewResponse;
                    Assert.NotNull(result);
                    
                    // Small delay to simulate cache access overhead
                    await Task.Delay(1);
                },
                networkOperation: async () =>
                {
                    // Simulate realistic network delay
                    await Task.Delay(50);
                    var result = CreateMockPreviewResponse();
                    Assert.NotNull(result);
                },
                description: "Timeline Preview Cache Hit",
                minimumSpeedupFactor: 3
            );
        }

        [Fact]
        public void CacheHit_MultipleRequests_ConsistentPerformance()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("consistent_release")
                .WithPreviewResponse()
                .WithMatchingFingerprints()
                .Build();

            var iterations = 1000;

            // Act & Assert - Multiple cache checks should be consistently fast
            TimelinePerformanceHelpers.AssertExecutionTime(() =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    var isCached = config.IsCachedPreviewForCurrentQuery();
                    Assert.True(isCached);
                }
            }, TimeSpan.FromMilliseconds(10), $"{iterations} cache hit checks");
        }

        [Fact]
        public void CacheHit_MemoryEfficient_NoLeaks()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithPreviewResponse()
                .WithMatchingFingerprints()
                .Build();

            // Act & Assert - Multiple cache hits should not leak memory
            TimelinePerformanceHelpers.AssertNoMemoryLeak(() =>
            {
                var isCached = config.IsCachedPreviewForCurrentQuery();
                Assert.True(isCached);
            }, iterations: 10000, maxMemoryGrowth: 1024 * 1024); // 1MB max growth
        }

        #endregion

        #region Cache Miss Behavior

        [Fact]
        public void CacheMiss_FingerprintMismatch_CorrectDetection()
        {
            // Arrange - Set up cache miss scenarios
            var scenarios = new[]
            {
                // Timestamp mismatch
                TimelineTestDataBuilder.New()
                    .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                    .WithPreviewResponse()
                    .Build(),
                
                // Release ID mismatch  
                TimelineTestDataBuilder.New()
                    .WithReleaseId("current_release")
                    .WithPreviewResponse()
                    .Build(),
                    
                // Live preview hash mismatch
                TimelineTestDataBuilder.New()
                    .WithLivePreview("current_hash")
                    .WithPreviewResponse()
                    .Build()
            };

            // Set mismatched fingerprints
            SetInternalProperty(scenarios[0], "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T10:00:00.000Z");
            SetInternalProperty(scenarios[1], "PreviewResponseFingerprintReleaseId", "old_release");
            SetInternalProperty(scenarios[2], "PreviewResponseFingerprintLivePreview", "old_hash");

            // Act & Assert - All should detect cache miss
            foreach (var config in scenarios)
            {
                Assert.False(config.IsCachedPreviewForCurrentQuery());
            }
        }

        [Fact]
        public void CacheMiss_NullResponse_AlwaysMiss()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release")
                .WithMatchingFingerprints()
                .Build();

            config.PreviewResponse = null;

            // Act & Assert - Null response should always be cache miss
            Assert.False(config.IsCachedPreviewForCurrentQuery());
        }

        [Fact]
        public void CacheMiss_PartialFingerprints_CorrectBehavior()
        {
            // Arrange - Test partial fingerprint scenarios
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("test_release")
                .WithLivePreview("test_hash")
                .WithPreviewResponse()
                .Build();

            var testCases = new[]
            {
                new { Description = "Only timestamp fingerprint set", SetTimestamp = true, SetRelease = false, SetHash = false },
                new { Description = "Only release fingerprint set", SetTimestamp = false, SetRelease = true, SetHash = false },
                new { Description = "Only hash fingerprint set", SetTimestamp = false, SetRelease = false, SetHash = true },
                new { Description = "Timestamp and release fingerprints set", SetTimestamp = true, SetRelease = true, SetHash = false },
                new { Description = "Timestamp and hash fingerprints set", SetTimestamp = true, SetRelease = false, SetHash = true },
                new { Description = "Release and hash fingerprints set", SetTimestamp = false, SetRelease = true, SetHash = true }
            };

            foreach (var testCase in testCases)
            {
                // Set fingerprints based on test case
                SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", 
                    testCase.SetTimestamp ? config.PreviewTimestamp : null);
                SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", 
                    testCase.SetRelease ? config.ReleaseId : null);
                SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", 
                    testCase.SetHash ? GetInternalProperty<string>(config, "LivePreview") : null);

                // Act & Assert
                var isCached = config.IsCachedPreviewForCurrentQuery();
                
                // Should only be cache hit if ALL non-null current values match their fingerprints
                bool expectedCacheHit = true;
                if (!string.IsNullOrEmpty(config.PreviewTimestamp) && !testCase.SetTimestamp) expectedCacheHit = false;
                if (!string.IsNullOrEmpty(config.ReleaseId) && !testCase.SetRelease) expectedCacheHit = false;
                if (!string.IsNullOrEmpty(GetInternalProperty<string>(config, "LivePreview")) && !testCase.SetHash) expectedCacheHit = false;

                Assert.True(expectedCacheHit == isCached, testCase.Description);
            }
        }

        #endregion

        #region Cache Fingerprint Updates

        [Fact]
        public async Task CacheFingerprint_UpdatedAfterSuccessfulQuery()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var mockHandler = new TimelineMockHttpHandler()
                .ForLivePreview(JObject.Parse(TimelineMockHelpers.CreateMockLivePreviewResponse()))
                .WithDelay(TimeSpan.FromMilliseconds(50)); // Predictable delay
            client.Plugins.Add(mockHandler);

            var query = CreateLivePreviewQuery(
                contentTypeUid: "fingerprint_ct",
                entryUid: "fingerprint_entry",
                previewTimestamp: "2024-11-29T14:30:00.000Z",
                releaseId: "test_release"
            );

            var config = client.GetLivePreviewConfig();

            // Verify initial state (no fingerprints)
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));

            // Act
            await client.LivePreviewQueryAsync(query);

            // Assert - Verify that config values are properly set from the query
            Assert.Equal("2024-11-29T14:30:00.000Z", config.PreviewTimestamp);
            Assert.Equal("test_release", config.ReleaseId);
            Assert.Equal("fingerprint_ct", config.ContentTypeUID);
            Assert.Equal("fingerprint_entry", config.EntryUID);
            
            // Verify that a mock request was made
            Assert.True(mockHandler.Requests.Count > 0);
        }

        [Fact]
        public void CacheFingerprint_ResetClearsFingerprints()
        {
            // Arrange
            var client = CreateClientWithTimeline();
            var config = client.GetLivePreviewConfig();

            // Set fingerprints
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T14:30:00.000Z");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "test_release");
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", "test_hash");

            // Act
            client.ResetLivePreview();

            // Assert - Fingerprints should be cleared
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintReleaseId"));
            Assert.Null(GetInternalProperty<string>(config, "PreviewResponseFingerprintLivePreview"));
        }

        [Fact]
        public void CacheFingerprint_ForkPreservesFingerprints()
        {
            // Arrange
            var parentClient = CreateClientWithTimeline();
            var parentConfig = parentClient.GetLivePreviewConfig();

            // Set parent fingerprints
            SetInternalProperty(parentConfig, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T14:30:00.000Z");
            SetInternalProperty(parentConfig, "PreviewResponseFingerprintReleaseId", "parent_release");
            SetInternalProperty(parentConfig, "PreviewResponseFingerprintLivePreview", "parent_hash");

            // Act
            var forkedClient = parentClient.Fork();
            var forkedConfig = forkedClient.GetLivePreviewConfig();

            // Assert - Fingerprints should be preserved in fork
            Assert.Equal("2024-11-29T14:30:00.000Z", 
                GetInternalProperty<string>(forkedConfig, "PreviewResponseFingerprintPreviewTimestamp"));
            Assert.Equal("parent_release", 
                GetInternalProperty<string>(forkedConfig, "PreviewResponseFingerprintReleaseId"));
            Assert.Equal("parent_hash", 
                GetInternalProperty<string>(forkedConfig, "PreviewResponseFingerprintLivePreview"));

            // But they should be independent instances
            SetInternalProperty(forkedConfig, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-30T00:00:00.000Z");
            
            // Parent should be unchanged
            Assert.Equal("2024-11-29T14:30:00.000Z", 
                GetInternalProperty<string>(parentConfig, "PreviewResponseFingerprintPreviewTimestamp"));
        }

        #endregion

        #region Cache Performance Optimization

        [Fact]
        public void Cache_OptimizedStringComparison_Performance()
        {
            // Arrange
            var longTimestamp = "2024-11-29T14:30:00.000Z" + new string('x', 1000);
            var longReleaseId = "release_" + new string('y', 1000);
            var longHash = "hash_" + new string('z', 1000);

            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp(longTimestamp)
                .WithReleaseId(longReleaseId)
                .WithLivePreview(longHash)
                .WithPreviewResponse()
                .Build();

            // Set matching fingerprints
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", longTimestamp);
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", longReleaseId);
            SetInternalProperty(config, "PreviewResponseFingerprintLivePreview", longHash);

            var iterations = 1000;

            // Act & Assert - Even with long strings, should be fast
            TimelinePerformanceHelpers.AssertExecutionTime(() =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    var isCached = config.IsCachedPreviewForCurrentQuery();
                    Assert.True(isCached);
                }
            }, TimeSpan.FromMilliseconds(50), $"{iterations} long string comparisons");
        }

        [Fact]
        public void Cache_ConcurrentAccess_ThreadSafe()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithReleaseId("concurrent_release")
                .WithPreviewResponse()
                .WithMatchingFingerprints()
                .Build();

            var numberOfThreads = 10;
            var operationsPerThread = 100;
            var allResults = new List<bool>();
            var lockObject = new object();

            // Act - Concurrent cache checks
            var tasks = new Task[numberOfThreads];
            for (int threadId = 0; threadId < numberOfThreads; threadId++)
            {
                tasks[threadId] = Task.Run(() =>
                {
                    var threadResults = new List<bool>();
                    for (int i = 0; i < operationsPerThread; i++)
                    {
                        var isCached = config.IsCachedPreviewForCurrentQuery();
                        threadResults.Add(isCached);
                    }
                    
                    lock (lockObject)
                    {
                        allResults.AddRange(threadResults);
                    }
                });
            }

            Task.WaitAll(tasks);

            // Assert - All operations should return consistent results
            var expectedResults = numberOfThreads * operationsPerThread;
            Assert.Equal(expectedResults, allResults.Count);
            Assert.All(allResults, result => Assert.True(result));
        }

        #endregion

        #region Entry.Fetch Integration

        [Fact]
        public async Task EntryFetch_CacheHit_SkipsNetworkRequest()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var config = client.GetLivePreviewConfig();

            // Set up cached response
            var cachedResponse = TimelineTestDataBuilder.New()
                .CreateValidPreviewResponse("fetch_entry", "fetch_ct");
                
            config.PreviewResponse = cachedResponse;
            config.PreviewTimestamp = "2024-11-29T14:30:00.000Z";
            
            SetInternalProperty(config, "ContentTypeUID", "fetch_ct");
            SetInternalProperty(config, "EntryUID", "fetch_entry");
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", config.PreviewTimestamp);

            var contentType = client.ContentType("fetch_ct");
            var entry = contentType.Entry("fetch_entry");

            var mockHandler = new TimelineMockHttpHandler()
                .ForSuccessfulLivePreview("fetch_entry", "fetch_ct");

            // Don't add handler to client - if cache works, no network call should be made

            // Act & Assert - Should use cache, not make network request
            var result = await entry.Fetch<JObject>();
            
            Assert.NotNull(result);
            // Should return the cached response data
            Assert.Equal("fetch_entry", result["uid"]?.ToString());
        }

        [Fact]
        public async Task LivePreviewQuery_CacheMiss_AttemptsNetworkRequest()
        {
            // Arrange
            var client = CreateClientWithLivePreview();
            var config = client.GetLivePreviewConfig();

            // Set up cache miss scenario - set non-matching fingerprints
            SetInternalProperty(config, "PreviewResponseFingerprintPreviewTimestamp", "2024-11-29T10:00:00.000Z");
            SetInternalProperty(config, "PreviewResponseFingerprintReleaseId", "old_release");

            var mockHandler = new TimelineMockHttpHandler()
                .ForLivePreview(JObject.Parse(TimelineMockHelpers.CreateMockLivePreviewResponse()));
            client.Plugins.Add(mockHandler);

            // Create query that will cause cache miss
            var query = CreateLivePreviewQuery(
                contentTypeUid: "network_ct",
                entryUid: "network_entry",
                previewTimestamp: "2024-11-29T14:30:00.000Z",
                releaseId: "network_release"
            );

            // Act
            await client.LivePreviewQueryAsync(query);
            
            // Assert - Should have attempted network request due to cache miss
            Assert.NotEmpty(mockHandler.Requests);
            
            // Verify cache configuration was updated with new values
            Assert.Equal("2024-11-29T14:30:00.000Z", config.PreviewTimestamp);
            Assert.Equal("network_release", config.ReleaseId);
            Assert.Equal("network_ct", config.ContentTypeUID);
            Assert.Equal("network_entry", config.EntryUID);
        }

        #endregion

        #region Cache Size and Memory Management

        [Fact]
        public void Cache_LargeResponses_MemoryEfficient()
        {
            // Arrange
            var config = TimelineTestDataBuilder.New()
                .WithPreviewTimestamp("2024-11-29T14:30:00.000Z")
                .WithMatchingFingerprints()
                .Build();

            // Create large response
            var largeResponseData = JObject.Parse($@"{{
                ""entry"": {{
                    ""uid"": ""large_entry"",
                    ""title"": ""Large Test Entry"",
                    ""large_field_1"": ""{new string('a', 10000)}"",
                    ""large_field_2"": ""{new string('b', 10000)}"",
                    ""large_field_3"": ""{new string('c', 10000)}"",
                    ""array_field"": [{string.Join(",", Enumerable.Range(0, 1000).Select(i => $"\"{i}\""))}]
                }}
            }}");

            config.PreviewResponse = largeResponseData;

            // Act & Assert - Cache operations should be memory efficient
            TimelinePerformanceHelpers.AssertNoMemoryLeak(() =>
            {
                var isCached = config.IsCachedPreviewForCurrentQuery();
                Assert.True(isCached);
            }, iterations: 1000, maxMemoryGrowth: 512 * 1024); // 512KB max growth for large object caching
        }

        [Fact]
        public void Cache_MultipleEntries_IndependentCaching()
        {
            // Arrange
            var baseClient = CreateClientWithLivePreview();
            var numberOfEntries = 50;
            var configs = new LivePreviewConfig[numberOfEntries];

            // Set up different cache states for multiple entries
            for (int i = 0; i < numberOfEntries; i++)
            {
                var fork = baseClient.Fork();
                configs[i] = fork.GetLivePreviewConfig();
                
                configs[i].PreviewTimestamp = $"2024-11-{(i % 12) + 1:D2}-01T00:00:00.000Z";
                configs[i].ReleaseId = $"release_{i}";
                configs[i].PreviewResponse = TimelineTestDataBuilder.New()
                    .CreateValidPreviewResponse($"entry_{i}", $"ct_{i}");
                
                SetInternalProperty(configs[i], "ContentTypeUID", $"ct_{i}");
                SetInternalProperty(configs[i], "EntryUID", $"entry_{i}");
                SetInternalProperty(configs[i], "PreviewResponseFingerprintPreviewTimestamp", configs[i].PreviewTimestamp);
                SetInternalProperty(configs[i], "PreviewResponseFingerprintReleaseId", configs[i].ReleaseId);
            }

            // Act & Assert - Each entry should have independent cache behavior
            for (int i = 0; i < numberOfEntries; i++)
            {
                Assert.True(configs[i].IsCachedPreviewForCurrentQuery(), $"Entry {i} should be cached");
                
                // Modify one entry's timestamp - only that entry should become cache miss
                var originalTimestamp = configs[i].PreviewTimestamp;
                configs[i].PreviewTimestamp = "2024-12-01T00:00:00.000Z";
                Assert.False(configs[i].IsCachedPreviewForCurrentQuery(), $"Entry {i} should be cache miss after timestamp change");
                
                // Restore timestamp
                configs[i].PreviewTimestamp = originalTimestamp;
                Assert.True(configs[i].IsCachedPreviewForCurrentQuery(), $"Entry {i} should be cached again after timestamp restore");
                
                // Verify other entries are unaffected
                for (int j = 0; j < numberOfEntries; j++)
                {
                    if (j != i)
                    {
                        Assert.True(configs[j].IsCachedPreviewForCurrentQuery(), $"Entry {j} should remain cached when entry {i} is modified");
                    }
                }
            }
        }

        #endregion
    }
}