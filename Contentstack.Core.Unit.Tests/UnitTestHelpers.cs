using Contentstack.Core;
using Contentstack.Core.Configuration;

namespace Contentstack.Core.Unit.Tests
{
    internal static class UnitTestHelpers
    {
        internal static ContentstackClient GetMockClient(string stackBranch = null)
        {
            var options = new ContentstackOptions
            {
                ApiKey = "DUMMY_API_KEY",
                DeliveryToken = "DUMMY_DELIVERY_TOKEN",
                Environment = "DUMMY_ENVIRONMENT",
                Branch = stackBranch
            };
            return new ContentstackClient(options);
        }
    }
}
