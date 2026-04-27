using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Contentstack.Core.Unit.Tests.Helpers
{
    /// <summary>
    /// Helper methods for Timeline Preview performance testing
    /// </summary>
    public static class TimelinePerformanceHelpers
    {
        /// <summary>
        /// Measures execution time of a synchronous operation
        /// </summary>
        /// <param name="operation">Operation to measure</param>
        /// <returns>Elapsed time</returns>
        public static TimeSpan MeasureExecutionTime(Action operation)
        {
            var stopwatch = Stopwatch.StartNew();
            operation();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Measures execution time of an asynchronous operation
        /// </summary>
        /// <param name="operation">Async operation to measure</param>
        /// <returns>Elapsed time</returns>
        public static async Task<TimeSpan> MeasureExecutionTime(Func<Task> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            await operation();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Asserts that cache operation is significantly faster than network operation
        /// </summary>
        /// <param name="cacheOperation">Cache operation to test</param>
        /// <param name="networkOperation">Network operation to compare against</param>
        /// <param name="description">Test description for assertion messages</param>
        /// <param name="minimumSpeedupFactor">Minimum expected speedup factor (default: 5x)</param>
        public static async Task AssertCachePerformance(
            Func<Task> cacheOperation,
            Func<Task> networkOperation,
            string description,
            int minimumSpeedupFactor = 5)
        {
            // Measure cache operation
            var cacheTime = await MeasureExecutionTime(cacheOperation);
            
            // Measure network operation  
            var networkTime = await MeasureExecutionTime(networkOperation);

            // Calculate speedup factor
            var speedupFactor = networkTime.TotalMilliseconds / Math.Max(cacheTime.TotalMilliseconds, 0.001);

            Assert.True(speedupFactor >= minimumSpeedupFactor,
                $"{description}: Cache operation ({cacheTime.TotalMilliseconds:F2}ms) should be at least {minimumSpeedupFactor}x faster than network operation ({networkTime.TotalMilliseconds:F2}ms). Actual speedup: {speedupFactor:F2}x");
        }

        /// <summary>
        /// Asserts that an operation doesn't cause significant memory leaks
        /// </summary>
        /// <param name="operation">Operation to test for memory leaks</param>
        /// <param name="iterations">Number of iterations to run</param>
        /// <param name="maxMemoryGrowth">Maximum allowed memory growth in bytes</param>
        public static void AssertNoMemoryLeak(Action operation, int iterations, long maxMemoryGrowth)
        {
            // Force initial garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var initialMemory = GC.GetTotalMemory(true);

            // Run the operation multiple times
            for (int i = 0; i < iterations; i++)
            {
                operation();
            }

            // Force garbage collection again
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(true);
            var memoryGrowth = finalMemory - initialMemory;

            Assert.True(memoryGrowth <= maxMemoryGrowth,
                $"Memory grew by {memoryGrowth:N0} bytes over {iterations} iterations. Maximum allowed: {maxMemoryGrowth:N0} bytes");
        }

        /// <summary>
        /// Measures average execution time over multiple iterations
        /// </summary>
        /// <param name="operation">Operation to measure</param>
        /// <param name="iterations">Number of iterations</param>
        /// <returns>Average execution time per iteration</returns>
        public static TimeSpan MeasureAverageExecutionTime(Action operation, int iterations)
        {
            var totalTime = MeasureExecutionTime(() =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    operation();
                }
            });

            return TimeSpan.FromTicks(totalTime.Ticks / iterations);
        }

        /// <summary>
        /// Measures average execution time of async operations over multiple iterations
        /// </summary>
        /// <param name="operation">Async operation to measure</param>
        /// <param name="iterations">Number of iterations</param>
        /// <returns>Average execution time per iteration</returns>
        public static async Task<TimeSpan> MeasureAverageExecutionTime(Func<Task> operation, int iterations)
        {
            var totalTime = await MeasureExecutionTime(async () =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    await operation();
                }
            });

            return TimeSpan.FromTicks(totalTime.Ticks / iterations);
        }

        /// <summary>
        /// Asserts that an operation completes within the specified time limit
        /// </summary>
        /// <param name="operation">Operation to test</param>
        /// <param name="timeLimit">Maximum allowed execution time</param>
        /// <param name="description">Description for assertion message</param>
        public static void AssertExecutionTime(Action operation, TimeSpan timeLimit, string description)
        {
            var executionTime = MeasureExecutionTime(operation);
            
            Assert.True(executionTime <= timeLimit,
                $"{description}: Operation took {executionTime.TotalMilliseconds:F2}ms, should be under {timeLimit.TotalMilliseconds:F2}ms");
        }

        /// <summary>
        /// Asserts that an async operation completes within the specified time limit
        /// </summary>
        /// <param name="operation">Async operation to test</param>
        /// <param name="timeLimit">Maximum allowed execution time</param>
        /// <param name="description">Description for assertion message</param>
        public static async Task AssertExecutionTime(Func<Task> operation, TimeSpan timeLimit, string description)
        {
            var executionTime = await MeasureExecutionTime(operation);
            
            Assert.True(executionTime <= timeLimit,
                $"{description}: Operation took {executionTime.TotalMilliseconds:F2}ms, should be under {timeLimit.TotalMilliseconds:F2}ms");
        }
    }
}