using System.Collections.Generic;
using Xunit.Abstractions;
using Contentstack.Core.Configuration;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Base class for integration tests with built-in enhanced logging support
    /// Provides TestOutputHelper and common helper methods for logging
    /// </summary>
    public abstract class IntegrationTestBase
    {
        protected readonly ITestOutputHelper Output;
        protected readonly TestOutputHelper TestOutput;

        protected IntegrationTestBase(ITestOutputHelper output)
        {
            Output = output;
            TestOutput = new TestOutputHelper(output, GetType().Name);
            TestAssert.SetHelper(TestOutput);
        }

        /// <summary>
        /// Log test arrangement step with context
        /// </summary>
        protected void LogArrange(string description, Dictionary<string, object> context = null)
        {
            TestOutput.LogStep("Arrange", description);
            
            if (context != null)
            {
                foreach (var kvp in context)
                {
                    TestOutput.LogContext(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Log test action step
        /// </summary>
        protected void LogAct(string description)
        {
            TestOutput.LogStep("Act", description);
        }

        /// <summary>
        /// Log test assertion step
        /// </summary>
        protected void LogAssert(string description)
        {
            TestOutput.LogStep("Assert", description);
        }

        /// <summary>
        /// Log assertion with expected and actual values
        /// </summary>
        protected void LogAssertion(string name, object expected, object actual)
        {
            var passed = AreEqual(expected, actual);
            TestOutput.LogAssertion(name, expected, actual, passed);
        }

        /// <summary>
        /// Log context information
        /// </summary>
        protected void LogContext(string key, object value)
        {
            TestOutput.LogContext(key, value);
        }

        /// <summary>
        /// Helper to check equality for logging
        /// </summary>
        private bool AreEqual(object expected, object actual)
        {
            if (expected == null && actual == null) return true;
            if (expected == null || actual == null) return false;
            return expected.Equals(actual) || expected.ToString() == actual.ToString();
        }

        /// <summary>
        /// Create Contentstack client with standard configuration.
        /// Automatically registers RequestLoggingPlugin to capture actual HTTP requests/responses.
        /// </summary>
        protected ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid
            };
            
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }

    }
}
