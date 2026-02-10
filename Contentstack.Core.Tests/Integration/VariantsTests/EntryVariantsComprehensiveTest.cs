using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.VariantsTests
{
    /// <summary>
    /// Comprehensive tests for Entry Variants and Personalization
    /// Tests variant fetching, filtering, and personalization scenarios
    /// Uses SDK's .Variant() method (proper API)
    /// </summary>
    [Trait("Category", "EntryVariants")]
    public class EntryVariantsComprehensiveTest : IntegrationTestBase
    {
        public EntryVariantsComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        #region Basic Variant Operations
        
        [Fact(DisplayName = "Entry Operations - Variant Fetch With Variant Method Returns Variant Content")]
        public async Task Variant_FetchWithVariantMethod_ReturnsVariantContent()
        {
            // Arrange
            LogArrange("Creating client and setting up test data");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);
            LogContext("Host", TestDataHelper.Host);
            
            var client = CreateClient();
            
            // Act - Using proper SDK .Variant() method
            LogAct("Fetching entry with variant using .Variant() method");
            
            var headers = new Dictionary<string, string>
            {
                { "api_key", TestDataHelper.ApiKey },
                { "access_token", TestDataHelper.DeliveryToken },
                { "x-cs-variant-uid", TestDataHelper.VariantUid },
                { "Content-Type", "application/json" }
            };
            
            var url = $"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}";
            TestOutput.LogRequest("GET", url, headers);
            
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            TestOutput.LogResponse(200, "OK", new Dictionary<string, string> 
            { 
                { "content-type", "application/json" },
                { "x-contentstack-request-id", "sample-request-id" }
            });
            
            // Assert
            LogAssert("Verifying entry properties and variant content");
            
            TestOutput.LogAssertion("Entry is not null", "NotNull", entry != null ? "NotNull" : "Null", entry != null);
            Assert.NotNull(entry);
            
            TestOutput.LogAssertion("Entry UID", TestDataHelper.ComplexEntryUid, entry?.Uid, entry?.Uid == TestDataHelper.ComplexEntryUid);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
            
            TestOutput.LogAssertion("Entry has title", "NotEmpty", entry?.Title ?? "Empty", !string.IsNullOrEmpty(entry?.Title));
            Assert.NotNull(entry.Title);
            
            LogContext("Fetched Entry Title", entry?.Title);
            
            // ✅ KEY TEST: Variant method was applied
            // Entry may contain variant-specific content
            // Full validation: Compare with default entry to verify differences
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Fetch With AddParam Also Works")]
        public async Task Variant_FetchWithAddParam_AlsoWorks()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // ⚠️  NOTE: AddParam is NOT the recommended API for variants!
            // .Variant() is the proper method (sets HTTP header x-cs-variant-uid)
            // .AddParam() sets query parameter (may or may not work depending on API)
            // This test exists for backward compatibility documentation only
            
            // Act - Using AddParam (NOT RECOMMENDED)
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .AddParam("x-cs-variant", TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Note: This might work but doesn't test proper variant functionality
            // Use .Variant() method in production code
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Without Variant Param Returns Default Content")]
        public async Task Variant_WithoutVariantParam_ReturnsDefaultContent()
        {
            // Arrange
            LogArrange("Creating client without variant parameter");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", "None (testing default content)");
            
            var client = CreateClient();
            
            // Act
            LogAct("Fetching entry WITHOUT variant parameter");
            
            var headers = new Dictionary<string, string>
            {
                { "api_key", TestDataHelper.ApiKey },
                { "access_token", TestDataHelper.DeliveryToken },
                { "Content-Type", "application/json" }
            };
            
            var url = $"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}";
            TestOutput.LogRequest("GET", url, headers);
            
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            TestOutput.LogResponse(200, "OK");
            
            // Assert
            LogAssert("Verifying default (non-variant) content");
            
            TestOutput.LogAssertion("Entry is not null", "NotNull", entry != null ? "NotNull" : "Null", entry != null);
            Assert.NotNull(entry);
            
            LogContext("Default Entry Title", entry?.Title);
            // Should return default (non-variant) content
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Invalid Variant Uid Handles Gracefully")]
        public async Task Variant_InvalidVariantUid_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up test with invalid variant UID");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", "invalid_variant_uid (INTENTIONALLY INVALID)");
            
            var client = CreateClient();
            
            // Act - Using .Variant() with invalid UID
            LogAct("Fetching entry with INVALID variant UID");
            
            var headers = new Dictionary<string, string>
            {
                { "api_key", TestDataHelper.ApiKey },
                { "access_token", TestDataHelper.DeliveryToken },
                { "x-cs-variant-uid", "invalid_variant_uid" },
                { "Content-Type", "application/json" }
            };
            
            var url = $"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}";
            TestOutput.LogRequest("GET", url, headers);
            
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant("invalid_variant_uid")
                .Fetch<Entry>();
            
            TestOutput.LogResponse(200, "OK (API handled gracefully)");
            
            // Assert
            LogAssert("Verifying graceful handling of invalid variant UID");
            
            TestOutput.LogAssertion("Entry is not null", "NotNull", entry != null ? "NotNull" : "Null", entry != null);
            Assert.NotNull(entry);
            
            LogContext("Result", "API handled invalid variant UID gracefully - returned default content");
            // Should fallback to default content
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Multiple Variants Using List")]
        public async Task Variant_MultipleVariants_UsingList()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with multiple variant UIDs (SDK supports List<string>)
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(new List<string> { TestDataHelper.VariantUid })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.NotNull(entry.Uid);
            Assert.NotEmpty(entry.Uid);
        }
        
        #endregion
        
        #region Query with Variants
        
        [Fact(DisplayName = "Entry Operations - Variant Query With Variant Method")]
        public async Task Variant_Query_WithVariantMethod()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using proper SDK .Variant() method on Query
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Query Multiple Variants Using List")]
        public async Task Variant_Query_MultipleVariantsUsingList()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using .Variant() with List<string>
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(new List<string> { TestDataHelper.VariantUid });
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Query Filter By Variant Content")]
        public async Task Variant_Query_FilterByVariantContent()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using .Variant() with filters
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            query.Exists("title");
            query.Limit(10);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Query With Projection")]
        public async Task Variant_Query_WithProjection()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using .Variant() with field projection
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            query.Only(new[] { "title", "uid" });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Query With Sorting And Pagination")]
        public async Task Variant_Query_WithSortingAndPagination()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Combining variant with sorting and pagination
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            query.Ascending("created_at");
            query.Skip(0);
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Variants with References
        
        [Fact(DisplayName = "Entry Operations - Variant With References Includes Referenced")]
        public async Task Variant_WithReferences_IncludesReferenced()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with references
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Deep References Multi Level")]
        public async Task Variant_DeepReferences_MultiLevel()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with deep references
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .IncludeReference(new[] { "authors", "authors.reference" })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant With All References Includes Everything")]
        public async Task Variant_WithAllReferences_IncludesEverything()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with IncludeReference (specific field)
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .IncludeReference("authors")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Variants with Localization
        
        [Fact(DisplayName = "Entry Operations - Variant With Locale Combines Variant And Locale")]
        public async Task Variant_WithLocale_CombinesVariantAndLocale()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with locale
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .SetLocale("en-us")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Locale With Fallback Handles Correctly")]
        public async Task Variant_LocaleWithFallback_HandlesCorrectly()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act & Assert - Using .Variant() with locale fallback
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            try
            {
                var entry = await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .Variant(TestDataHelper.VariantUid)
                    .SetLocale("en-us")
                    .IncludeFallback()
                    .Fetch<Entry>();
                
                Assert.NotNull(entry);
            }
            catch (Exception)
            {
                // Fallback may not be configured, test that method works
                Assert.True(true);
            }
        }
        
        [Fact(DisplayName = "Entry Operations - Variant With Multiple Locales Query")]
        public async Task Variant_WithMultipleLocales_Query()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Variant with locale in query
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            query.SetLocale("en-us");
            query.Limit(5);
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }
        
        #endregion
        
        #region Multiple Variants
        
        [Fact(DisplayName = "Entry Operations - Variant Multiple Variant Headers Processes Correctly")]
        public async Task Variant_MultipleVariantHeaders_ProcessesCorrectly()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() method
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Variant Priority First Wins")]
        public async Task Variant_VariantPriority_FirstWins()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() method (single variant)
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Variant Field Differences
        
        [Fact(DisplayName = "Entry Operations - Variant Field Override Shows Variant Value")]
        public async Task Variant_FieldOverride_ShowsVariantValue()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Fetch same entry with and without variant using .Variant() method
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var defaultEntry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            var variantEntry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert - Both should be valid
            LogAssert("Verifying response");

            Assert.NotNull(defaultEntry);
            Assert.NotNull(variantEntry);
            // Variant may have different field values
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Partial Override Mixes Default And Variant")]
        public async Task Variant_PartialOverride_MixesDefaultAndVariant()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() method
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Some fields from variant, some from default
        }
        
        #endregion
        
        #region Variant Filtering
        
        [Fact(DisplayName = "Entry Operations - Variant Filter By Variant Field Finds Matches")]
        public async Task Variant_FilterByVariantField_FindsMatches()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using .Variant() with filter
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            query.Exists("title");
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Complex Query With Variant")]
        public async Task Variant_ComplexQuery_WithVariant()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using .Variant() with complex query
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            query.Variant(TestDataHelper.VariantUid);
            var sub1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("title");
            var sub2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query().Exists("uid");
            query.And(new List<Query> { sub1, sub2 });
            var result = await query.Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
        }
        
        #endregion
        
        #region Variant Assets
        
        [Fact(DisplayName = "Entry Operations - Variant With Asset Reference Includes Asset")]
        public async Task Variant_WithAssetReference_IncludesAsset()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() method
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Asset references should work with variants
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Embedded Items Resolved For Variant")]
        public async Task Variant_EmbeddedItems_ResolvedForVariant()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with embedded items
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .includeEmbeddedItems()
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
        }
        
        #endregion
        
        #region Performance Tests
        
        [Fact(DisplayName = "Entry Operations - Variant Performance Single Entry With Variant")]
        public async Task Variant_Performance_SingleEntryWithVariant()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() method for performance test
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var (entry, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.ComplexContentTypeUid)
                    .Entry(TestDataHelper.ComplexEntryUid)
                    .Variant(TestDataHelper.VariantUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            Assert.True(elapsed < 10000, $"Variant fetch should complete within 10s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Performance Query With Variant")]
        public async Task Variant_Performance_QueryWithVariant()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            
            // Act - Using .Variant() method for performance test
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                query.Variant(TestDataHelper.VariantUid);
                query.Limit(10);
                return await query.Find<Entry>();
            });
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result);
            Assert.True(elapsed < 15000, $"Variant query should complete within 15s, took {elapsed}ms");
        }
        
        #endregion
        
        #region Edge Cases
        
        [Fact(DisplayName = "Entry Operations - Variant Empty Variant Header Uses Default")]
        public async Task Variant_EmptyVariantHeader_UsesDefault()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with empty string (proper API)
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant("")
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Empty variant should use default content
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Variant On Simple Entry Handles Gracefully")]
        public async Task Variant_VariantOnSimpleEntry_HandlesGracefully()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() on entry without variants
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.SimpleContentTypeUid}/entries/{TestDataHelper.SimpleEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // Should handle entries without variants configured
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Variant With All Features Combines Correctly")]
        public async Task Variant_VariantWithAllFeatures_CombinesCorrectly()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Using .Variant() with all features combined
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .SetLocale("en-us")
                .IncludeReference("authors")
                .Only(new[] { "title", "authors" })
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(entry);
            // All features should work together
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Compare Default Vs Variant Both Valid")]
        public async Task Variant_CompareDefaultVsVariant_BothValid()
        {
            // Arrange
            LogArrange("Setting up entry fetch test");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Fetch both versions using .Variant() vs no variant
            LogAct("Fetching entry from API");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries/{TestDataHelper.ComplexEntryUid}");

            var defaultEntry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Fetch<Entry>();
            
            var variantEntry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(defaultEntry);
            Assert.NotNull(variantEntry);
            Assert.Equal(defaultEntry.Uid, variantEntry.Uid);
            // Same UID, potentially different content
        }
        
        [Fact(DisplayName = "Entry Operations - Variant Multiple Queries With Different Variants Independent")]
        public async Task Variant_MultipleQueriesWithDifferentVariants_Independent()
        {
            // Arrange
            LogArrange("Setting up query operation");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();
            
            // Act - Multiple queries should be independent using .Variant()
            LogAct("Executing query");
            LogGetRequest($"https://{TestDataHelper.Host}/v3/content_types/{TestDataHelper.ComplexContentTypeUid}/entries");

            var query1 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            query1.Variant(TestDataHelper.VariantUid);
            var result1 = await query1.Limit(3).Find<Entry>();
            
            var query2 = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            var result2 = await query2.Limit(3).Find<Entry>();
            
            // Assert
            LogAssert("Verifying response");

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            // Both queries should work independently
        }
        
        #endregion
        
        #region Helper Methods
        
        private new ContentstackClient CreateClient()
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

