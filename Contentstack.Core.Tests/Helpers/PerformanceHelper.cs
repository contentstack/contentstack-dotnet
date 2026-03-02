using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Helper class for performance measurement and benchmarking
    /// </summary>
    public static class PerformanceHelper
    {
        #region Performance Thresholds
        
        /// <summary>
        /// Default timeout for single entry fetch (5 seconds)
        /// </summary>
        public const int DefaultSingleFetchThresholdMs = 5000;
        
        /// <summary>
        /// Default timeout for query operations (10 seconds)
        /// </summary>
        public const int DefaultQueryThresholdMs = 10000;
        
        /// <summary>
        /// Default timeout for deep reference queries (15 seconds)
        /// </summary>
        public const int DefaultDeepReferenceThresholdMs = 15000;
        
        /// <summary>
        /// Default timeout for sync operations (30 seconds)
        /// </summary>
        public const int DefaultSyncThresholdMs = 30000;
        
        #endregion

        #region Measurement Methods
        
        /// <summary>
        /// Measures the execution time of a synchronous action
        /// </summary>
        /// <param name="action">Action to measure</param>
        /// <returns>Elapsed milliseconds</returns>
        public static long MeasureExecutionTime(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// Measures the execution time of an asynchronous action
        /// </summary>
        /// <param name="action">Async action to measure</param>
        /// <returns>Elapsed milliseconds</returns>
        public static async Task<long> MeasureExecutionTimeAsync(Func<Task> action)
        {
            var stopwatch = Stopwatch.StartNew();
            await action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        /// <summary>
        /// Measures the execution time and returns both result and time
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Function to measure</param>
        /// <returns>Tuple of (result, elapsed milliseconds)</returns>
        public static async Task<(T result, long elapsedMs)> MeasureExecutionTimeAsync<T>(Func<Task<T>> func)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await func();
            stopwatch.Stop();
            return (result, stopwatch.ElapsedMilliseconds);
        }
        
        #endregion

        #region Assertion Methods
        
        /// <summary>
        /// Asserts that an operation completes within the specified threshold
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="thresholdMs">Threshold in milliseconds</param>
        /// <param name="operationName">Name of the operation for error messages</param>
        public static void AssertPerformance(Action action, int thresholdMs, string operationName = "Operation")
        {
            var elapsed = MeasureExecutionTime(action);
            Assert.True(elapsed < thresholdMs, 
                $"{operationName} took {elapsed}ms, expected < {thresholdMs}ms (threshold exceeded by {elapsed - thresholdMs}ms)");
        }
        
        /// <summary>
        /// Asserts that an async operation completes within the specified threshold
        /// </summary>
        /// <param name="action">Async action to execute</param>
        /// <param name="thresholdMs">Threshold in milliseconds</param>
        /// <param name="operationName">Name of the operation for error messages</param>
        public static async Task AssertPerformanceAsync(Func<Task> action, int thresholdMs, string operationName = "Operation")
        {
            var elapsed = await MeasureExecutionTimeAsync(action);
            Assert.True(elapsed < thresholdMs, 
                $"{operationName} took {elapsed}ms, expected < {thresholdMs}ms (threshold exceeded by {elapsed - thresholdMs}ms)");
        }
        
        /// <summary>
        /// Asserts that an async operation with result completes within the specified threshold
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Async function to execute</param>
        /// <param name="thresholdMs">Threshold in milliseconds</param>
        /// <param name="operationName">Name of the operation for error messages</param>
        /// <returns>The result from the function</returns>
        public static async Task<T> AssertPerformanceAsync<T>(Func<Task<T>> func, int thresholdMs, string operationName = "Operation")
        {
            var (result, elapsed) = await MeasureExecutionTimeAsync(func);
            Assert.True(elapsed < thresholdMs, 
                $"{operationName} took {elapsed}ms, expected < {thresholdMs}ms (threshold exceeded by {elapsed - thresholdMs}ms)");
            return result;
        }
        
        #endregion

        #region Benchmarking Methods
        
        /// <summary>
        /// Runs a benchmark of an operation multiple times and returns statistics
        /// </summary>
        /// <param name="action">Action to benchmark</param>
        /// <param name="iterations">Number of iterations to run</param>
        /// <returns>Benchmark statistics</returns>
        public static BenchmarkResult Benchmark(Action action, int iterations = 10)
        {
            var times = new long[iterations];
            
            for (int i = 0; i < iterations; i++)
            {
                times[i] = MeasureExecutionTime(action);
            }
            
            return new BenchmarkResult(times);
        }
        
        /// <summary>
        /// Runs an async benchmark of an operation multiple times and returns statistics
        /// </summary>
        /// <param name="action">Async action to benchmark</param>
        /// <param name="iterations">Number of iterations to run</param>
        /// <returns>Benchmark statistics</returns>
        public static async Task<BenchmarkResult> BenchmarkAsync(Func<Task> action, int iterations = 10)
        {
            var times = new long[iterations];
            
            for (int i = 0; i < iterations; i++)
            {
                times[i] = await MeasureExecutionTimeAsync(action);
            }
            
            return new BenchmarkResult(times);
        }
        
        #endregion
        
        #region Benchmark Result Class
        
        /// <summary>
        /// Contains statistics from a benchmark run
        /// </summary>
        public class BenchmarkResult
        {
            public long[] AllTimes { get; }
            public long MinMs { get; }
            public long MaxMs { get; }
            public long AverageMs { get; }
            public long MedianMs { get; }
            public int Iterations { get; }
            
            public BenchmarkResult(long[] times)
            {
                AllTimes = times;
                Iterations = times.Length;
                
                if (times.Length == 0)
                {
                    MinMs = MaxMs = AverageMs = MedianMs = 0;
                    return;
                }
                
                MinMs = long.MaxValue;
                MaxMs = long.MinValue;
                long sum = 0;
                
                foreach (var time in times)
                {
                    if (time < MinMs) MinMs = time;
                    if (time > MaxMs) MaxMs = time;
                    sum += time;
                }
                
                AverageMs = sum / times.Length;
                
                // Calculate median
                Array.Sort(times);
                MedianMs = times[times.Length / 2];
            }
            
            public override string ToString()
            {
                return $"Benchmark Results ({Iterations} iterations):\n" +
                       $"  Min: {MinMs}ms\n" +
                       $"  Max: {MaxMs}ms\n" +
                       $"  Avg: {AverageMs}ms\n" +
                       $"  Median: {MedianMs}ms";
            }
            
            /// <summary>
            /// Asserts that the average time is within threshold
            /// </summary>
            public void AssertAverageWithinThreshold(int thresholdMs, string operationName = "Operation")
            {
                Assert.True(AverageMs < thresholdMs,
                    $"{operationName} average time {AverageMs}ms exceeded threshold {thresholdMs}ms\n{this}");
            }
            
            /// <summary>
            /// Asserts that the max time is within threshold
            /// </summary>
            public void AssertMaxWithinThreshold(int thresholdMs, string operationName = "Operation")
            {
                Assert.True(MaxMs < thresholdMs,
                    $"{operationName} max time {MaxMs}ms exceeded threshold {thresholdMs}ms\n{this}");
            }
        }
        
        #endregion
    }
}

