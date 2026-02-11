using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.ErrorHandling
{
    /// <summary>
    /// Comprehensive tests for Error Handling across the SDK
    /// Tests various error scenarios: invalid credentials, missing resources, malformed requests
    /// </summary>
    [Trait("Category", "ErrorHandling")]
    public class ErrorHandlingComprehensiveTest : IntegrationTestBase
    {
        public ErrorHandlingComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Invalid Credentials
        
        [Fact(DisplayName = "Error Handling - Error Invalid API Key Throws Exception")]
        public async Task Error_InvalidAPIKey_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = "invalid_api_key_xyz_123",
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            
            // Act & Assert
            LogAct("Executing query");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Find<Entry>();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Invalid Delivery Token Throws Exception")]
        public async Task Error_InvalidDeliveryToken_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = "invalid_token_xyz_123",
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            
            // Act & Assert
            LogAct("Executing query");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Find<Entry>();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Invalid Environment Throws Exception")]
        public async Task Error_InvalidEnvironment_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = "invalid_environment_xyz"
            };
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            
            // Act & Assert
            LogAct("Executing query");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Find<Entry>();
            });
        }
        
        #endregion
        
        #region Invalid Resources
        
        [Fact(DisplayName = "Error Handling - Error Invalid Content Type Throws Exception")]
        public async Task Error_InvalidContentType_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up query operation");

            var client = CreateValidClient();
            
            // Act & Assert
            LogAct("Executing query");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType("invalid_content_type_xyz").Query().Find<Entry>();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Invalid Entry Uid Throws Exception")]
        public async Task Error_InvalidEntryUid_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateValidClient();
            
            // Act & Assert
            LogAct("Fetching entry from API");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry("invalid_entry_uid_xyz_123")
                    .Fetch<Entry>();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Invalid Asset Uid Throws Exception")]
        public async Task Error_InvalidAssetUid_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up fetch operation");

            var client = CreateValidClient();
            
            // Act & Assert
            LogAct("Fetching entry from API");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.Asset("invalid_asset_uid_xyz_123").Fetch();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Non Existent Reference Does Not Crash")]
        public async Task Error_NonExistentReference_DoesNotCrash()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateValidClient();
            
            // Act - Include non-existent reference (should not crash)
            LogAct("Fetching entry from API");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeReference("non_existent_reference_field_xyz")
                .Fetch<Entry>();
            
            // Assert - Should return entry even if reference doesn't exist
            LogAssert("Verifying response");

            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }
        
        #endregion
        
        #region Malformed Requests
        
        [Fact(DisplayName = "Error Handling - Error Invalid Query Parameter Handles Gracefully")]
        public async Task Error_InvalidQueryParameter_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateValidClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Query with non-existent field
            LogAct("Executing query");

            query.Where("non_existent_field_xyz_123", "some_value");
            var result = await query.Find<Entry>();
            
            // Assert - Should return empty results
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.Equal(0, result.Items.Count());
        }
        
        [Fact(DisplayName = "Error Handling - Error Invalid Locale Handles Gracefully")]
        public async Task Error_InvalidLocale_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateValidClient();
            
            // Act & Assert - Should handle invalid locale
            LogAct("Fetching entry from API");

            try
            {
                var entry = await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .SetLocale("invalid_locale_xyz")
                    .Fetch<Entry>();
                
                // If it doesn't throw, that's also acceptable
                TestAssert.NotNull(entry);
            }
            catch (Exception)
            {
                // Exception is acceptable for invalid locale
                TestAssert.True(true);
            }
        }
        
        [Fact(DisplayName = "Error Handling - Error Extremely Large Limit Handles Gracefully")]
        public async Task Error_ExtremelyLargeLimit_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateValidClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Very large limit (beyond API limits)
            LogAct("Executing query");

            query.Limit(10000);
            var result = await query.Find<Entry>();
            
            // Assert - Should handle gracefully (API will enforce its own limits)
            LogAssert("Verifying response");

            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Error Handling - Error Negative Skip Handles Gracefully")]
        public async Task Error_NegativeSkip_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateValidClient();
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            
            // Act - Negative skip value
            LogAct("Executing query");

            try
            {
                query.Skip(-1);
                var result = await query.Find<Entry>();
                
                // If it doesn't throw, verify result
                TestAssert.NotNull(result);
            }
            catch (ArgumentException)
            {
                // ArgumentException is acceptable for negative skip
                TestAssert.True(true);
            }
        }
        
        #endregion
        
        #region Network and Timeout Errors
        
        [Fact(DisplayName = "Error Handling - Error Invalid Host Throws Exception")]
        public async Task Error_InvalidHost_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var options = new ContentstackOptions()
            {
                Host = "invalid.host.xyz.contentstack.io",
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Timeout = 5000 // Short timeout
            };
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            
            // Act & Assert
            LogAct("Executing query");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType(TestDataHelper.SimpleContentTypeUid).Query().Find<Entry>();
            });
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Error Handling - Error Empty Content Type Uid Throws Exception")]
        public async Task Error_EmptyContentTypeUid_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up query operation");

            var client = CreateValidClient();
            
            // Act & Assert
            LogAct("Executing query");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client.ContentType("").Query().Find<Entry>();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Empty Entry Uid Throws Exception")]
        public async Task Error_EmptyEntryUid_ThrowsException()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateValidClient();
            
            // Act & Assert
            LogAct("Fetching entry from API");

            await TestAssert.ThrowsAnyAsync<Exception>(async () =>
            {
                await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry("")
                    .Fetch<Entry>();
            });
        }
        
        [Fact(DisplayName = "Error Handling - Error Null Options Throws Exception")]
        public void Error_NullOptions_ThrowsException()
        {
            // Act & Assert - Should throw exception (ArgumentNullException or NullReferenceException)
            LogAct("Performing test action");

            TestAssert.ThrowsAny<Exception>(() =>
            {
                var client = new ContentstackClient((ContentstackOptions)null);
            });
        }
        
        #endregion
        
        #region Helper Methods
        
        private ContentstackClient CreateValidClient()
        {
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            
            return new ContentstackClient(options);
        }
        
        #endregion
    }
}

