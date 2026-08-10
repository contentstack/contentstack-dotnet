using System;
using Contentstack.Core;
using Contentstack.Core.Configuration;

namespace Contentstack.Core.Unit.Tests
{
    internal static class UnitTestHelpers
    {
        private const string TestDeliveryToken = "DUMMY_DELIVERY_TOKEN";
        private const string TestEnvironment = "DUMMY_ENVIRONMENT";

        internal static ContentstackClient GetMockClient(string stackBranch = null)
        {
            var options = new ContentstackOptions
            {
                ApiKey = Guid.NewGuid().ToString("N"),
                DeliveryToken = TestDeliveryToken,
                Environment = TestEnvironment,
                Branch = stackBranch
            };
            return new ContentstackClient(options);
        }
    }
}
