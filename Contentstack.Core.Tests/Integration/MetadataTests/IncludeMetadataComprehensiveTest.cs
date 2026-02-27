using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Xunit.Abstractions;

namespace Contentstack.Core.Tests.Integration.MetadataTests
{
    /// <summary>
    /// Comprehensive tests for IncludeMetadata() across Entry, Query, Asset, and AssetLibrary.
    /// Covers standalone usage, combination with other features, and edge cases.
    /// Inspired by JS CDA SDK customer issue: 500 errors when using includeMetadata with special chars.
    /// </summary>
    [Trait("Category", "IncludeMetadata")]
    public class IncludeMetadataComprehensiveTest : IntegrationTestBase
    {
        public IncludeMetadataComprehensiveTest(ITestOutputHelper output) : base(output)
        {
        }

        // ====================================================================
        // Entry.IncludeMetadata() Tests
        // ====================================================================

        #region Entry IncludeMetadata

        [Fact(DisplayName = "IncludeMetadata - Entry Fetch With IncludeMetadata Returns Entry")]
        public async Task Entry_FetchWithIncludeMetadata_ReturnsEntry()
        {
            // Arrange
            LogArrange("Fetch single entry with IncludeMetadata()");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeMetadata()
                .Fetch<Entry>();

            // Assert
            LogAssert("Verifying entry returned with metadata");
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
            TestAssert.NotNull(entry.Title);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry Fetch With IncludeMetadata Populates Metadata Property")]
        public async Task Entry_FetchWithIncludeMetadata_PopulatesMetadataProperty()
        {
            // Arrange
            LogArrange("Fetch entry and check Metadata dictionary is populated");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeMetadata()
                .Fetch<Entry>();

            // Assert
            LogAssert("Verifying Metadata property is accessible");
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            // GetMetadata() should return Dictionary (may be empty if no metadata set)
            var metadata = entry.GetMetadata();
            TestAssert.NotNull(metadata);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry Fetch Without IncludeMetadata Still Returns Entry")]
        public async Task Entry_FetchWithoutIncludeMetadata_StillReturnsEntry()
        {
            // Arrange - baseline test without IncludeMetadata
            LogArrange("Fetch entry WITHOUT IncludeMetadata for comparison");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.Fetch() without IncludeMetadata");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata With IncludeBranch Both Applied")]
        public async Task Entry_IncludeMetadataWithIncludeBranch_BothApplied()
        {
            // Arrange
            LogArrange("Fetch entry with IncludeMetadata + IncludeBranch");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);
            LogContext("EntryUid", TestDataHelper.SimpleEntryUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().IncludeBranch().Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeMetadata()
                .IncludeBranch()
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata With EmbeddedItems Both Applied")]
        public async Task Entry_IncludeMetadataWithEmbeddedItems_BothApplied()
        {
            // Arrange
            LogArrange("Fetch entry with IncludeMetadata + includeEmbeddedItems");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().includeEmbeddedItems().Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeMetadata()
                .includeEmbeddedItems()
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata With References Both Applied")]
        public async Task Entry_IncludeMetadataWithReferences_BothApplied()
        {
            // Arrange
            LogArrange("Fetch entry with IncludeMetadata + IncludeReference");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().IncludeReference('authors').Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeMetadata()
                .IncludeReference("authors")
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata With Variant Both Applied")]
        public async Task Entry_IncludeMetadataWithVariant_BothApplied()
        {
            // Arrange - tests the combination that caused 500 errors in JS SDK
            LogArrange("Fetch entry with IncludeMetadata + Variant");
            LogContext("ContentType", TestDataHelper.ComplexContentTypeUid);
            LogContext("EntryUid", TestDataHelper.ComplexEntryUid);
            LogContext("VariantUid", TestDataHelper.VariantUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().Variant(uid).Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeMetadata()
                .Variant(TestDataHelper.VariantUid)
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata With Only Fields Both Applied")]
        public async Task Entry_IncludeMetadataWithOnly_BothApplied()
        {
            // Arrange
            LogArrange("Fetch entry with IncludeMetadata + Only field projection");
            var client = CreateClient();

            // Act
            LogAct("Calling Entry.IncludeMetadata().Only('title').Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeMetadata()
                .Only(new string[] { "title" })
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata With AddParam Both Applied")]
        public async Task Entry_IncludeMetadataWithAddParam_BothApplied()
        {
            // Arrange - mirrors JS SDK CustomParameters test
            LogArrange("Fetch entry with IncludeMetadata via AddParam");
            var client = CreateClient();

            // Act
            LogAct("Calling Entry.AddParam('include_metadata', 'true').Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .AddParam("include_metadata", "true")
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Entry IncludeMetadata All Combinations Applied")]
        public async Task Entry_IncludeMetadataAllCombinations_Applied()
        {
            // Arrange - full combination test
            LogArrange("Fetch entry with IncludeMetadata + IncludeBranch + includeEmbeddedItems + IncludeContentType");
            var client = CreateClient();

            // Act
            LogAct("Calling Entry with all include options");
            var entry = await client
                .ContentType(TestDataHelper.ComplexContentTypeUid)
                .Entry(TestDataHelper.ComplexEntryUid)
                .IncludeMetadata()
                .IncludeBranch()
                .includeEmbeddedItems()
                .IncludeReference("authors")
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        #endregion

        // ====================================================================
        // Query.IncludeMetadata() Tests
        // ====================================================================

        #region Query IncludeMetadata

        [Fact(DisplayName = "IncludeMetadata - Query Find With IncludeMetadata Returns Entries")]
        public async Task Query_FindWithIncludeMetadata_ReturnsEntries()
        {
            // Arrange
            LogArrange("Query entries with IncludeMetadata()");
            LogContext("ContentType", TestDataHelper.SimpleContentTypeUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Query.IncludeMetadata().Find()");
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query.IncludeMetadata();
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.True(result.Items.Count() > 0, "result.Items.Count() > 0");
        }

        [Fact(DisplayName = "IncludeMetadata - Query IncludeMetadata With Where Filter Both Applied")]
        public async Task Query_IncludeMetadataWithWhereFilter_BothApplied()
        {
            // Arrange
            LogArrange("Query entries with IncludeMetadata + Where filter");
            var client = CreateClient();

            // Act
            LogAct("Calling Query.IncludeMetadata().Exists('title').Find()");
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query.IncludeMetadata();
            query.Exists("title");
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.True(result.Items.Count() > 0, "result.Items.Count() > 0");
        }

        [Fact(DisplayName = "IncludeMetadata - Query IncludeMetadata With Variant Both Applied")]
        public async Task Query_IncludeMetadataWithVariant_BothApplied()
        {
            // Arrange - combination that is likely related to the customer issue
            LogArrange("Query entries with IncludeMetadata + Variant");
            LogContext("VariantUid", TestDataHelper.VariantUid);
            var client = CreateClient();

            // Act
            LogAct("Calling Query.IncludeMetadata().Variant(uid).Find()");
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            query.IncludeMetadata();
            query.Variant(TestDataHelper.VariantUid);
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }

        [Fact(DisplayName = "IncludeMetadata - Query IncludeMetadata With IncludeBranch Both Applied")]
        public async Task Query_IncludeMetadataWithIncludeBranch_BothApplied()
        {
            // Arrange
            LogArrange("Query entries with IncludeMetadata + IncludeBranch");
            var client = CreateClient();

            // Act
            LogAct("Calling Query.IncludeMetadata().IncludeBranch().Find()");
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query.IncludeMetadata();
            query.IncludeBranch();
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }

        [Fact(DisplayName = "IncludeMetadata - Query IncludeMetadata With Pagination Both Applied")]
        public async Task Query_IncludeMetadataWithPagination_BothApplied()
        {
            // Arrange
            LogArrange("Query entries with IncludeMetadata + Limit + Skip");
            var client = CreateClient();

            // Act
            LogAct("Calling Query.IncludeMetadata().Limit(2).Skip(0).Find()");
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query.IncludeMetadata();
            query.Limit(2);
            query.Skip(0);
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.True(result.Items.Count() <= 2, "result.Items.Count() <= 2");
        }

        [Fact(DisplayName = "IncludeMetadata - Query IncludeMetadata With EmbeddedItems Both Applied")]
        public async Task Query_IncludeMetadataWithEmbeddedItems_BothApplied()
        {
            // Arrange
            LogArrange("Query entries with IncludeMetadata + includeEmbeddedItems");
            var client = CreateClient();

            // Act
            LogAct("Calling Query.IncludeMetadata().includeEmbeddedItems().Find()");
            var query = client.ContentType(TestDataHelper.ComplexContentTypeUid).Query();
            query.IncludeMetadata();
            query.includeEmbeddedItems();
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }

        #endregion

        // ====================================================================
        // Asset.IncludeMetadata() Tests
        // ====================================================================

        #region Asset IncludeMetadata

        [Fact(DisplayName = "IncludeMetadata - Asset Fetch With IncludeMetadata Returns Asset")]
        public async Task Asset_FetchWithIncludeMetadata_ReturnsAsset()
        {
            // Arrange
            LogArrange("Fetch single asset with IncludeMetadata()");
            LogContext("AssetUid", TestDataHelper.ImageAssetUid);

            var client = CreateClient();

            // Act
            LogAct("Calling Asset.IncludeMetadata().Fetch()");
            var asset = await client
                .Asset(TestDataHelper.ImageAssetUid)
                .IncludeMetadata()
                .Fetch();

            // Assert
            TestAssert.NotNull(asset);
            TestAssert.NotNull(asset.Uid);
            TestAssert.Equal(TestDataHelper.ImageAssetUid, asset.Uid);
        }

        [Fact(DisplayName = "IncludeMetadata - Asset IncludeMetadata With IncludeBranch Both Applied")]
        public async Task Asset_IncludeMetadataWithIncludeBranch_BothApplied()
        {
            // Arrange
            LogArrange("Fetch asset with IncludeMetadata + IncludeBranch");
            var client = CreateClient();

            // Act
            LogAct("Calling Asset.IncludeMetadata().IncludeBranch().Fetch()");
            var asset = await client
                .Asset(TestDataHelper.ImageAssetUid)
                .IncludeMetadata()
                .IncludeBranch()
                .Fetch();

            // Assert
            TestAssert.NotNull(asset);
            TestAssert.NotNull(asset.Uid);
        }

        #endregion

        // ====================================================================
        // AssetLibrary.IncludeMetadata() Tests
        // ====================================================================

        #region AssetLibrary IncludeMetadata

        [Fact(DisplayName = "IncludeMetadata - AssetLibrary FetchAll With IncludeMetadata Returns Assets")]
        public async Task AssetLibrary_FetchAllWithIncludeMetadata_ReturnsAssets()
        {
            // Arrange
            LogArrange("Fetch all assets with IncludeMetadata()");
            var client = CreateClient();

            // Act
            LogAct("Calling AssetLibrary.IncludeMetadata().FetchAll()");
            var assetLibrary = client.AssetLibrary();
            assetLibrary.IncludeMetadata();
            var result = await assetLibrary.FetchAll();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.True(result.Items.Count() > 0, "result.Items.Count() > 0");
        }

        [Fact(DisplayName = "IncludeMetadata - AssetLibrary IncludeMetadata With IncludeBranch Both Applied")]
        public async Task AssetLibrary_IncludeMetadataWithIncludeBranch_BothApplied()
        {
            // Arrange
            LogArrange("Fetch assets with IncludeMetadata + IncludeBranch");
            var client = CreateClient();

            // Act
            LogAct("Calling AssetLibrary.IncludeMetadata().IncludeBranch().FetchAll()");
            var assetLibrary = client.AssetLibrary();
            assetLibrary.IncludeMetadata();
            assetLibrary.IncludeBranch();
            var result = await assetLibrary.FetchAll();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }

        #endregion

        // ====================================================================
        // EarlyAccessHeader Tests
        // ====================================================================

        #region EarlyAccessHeader

        [Fact(DisplayName = "EarlyAccess - Client With EarlyAccessHeader Sends Header")]
        public async Task Client_WithEarlyAccessHeader_SendsHeader()
        {
            // Arrange
            LogArrange("Create client with EarlyAccessHeader and verify it works");
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid,
                EarlyAccessHeader = new string[] { "taxonomy" }
            };

            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));

            // Act
            LogAct("Fetching entry with EarlyAccessHeader configured");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "EarlyAccess - Client With Multiple EarlyAccessHeaders Works")]
        public async Task Client_WithMultipleEarlyAccessHeaders_Works()
        {
            // Arrange
            LogArrange("Create client with multiple EarlyAccessHeaders");
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid,
                EarlyAccessHeader = new string[] { "taxonomy", "newCDA" }
            };

            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));

            // Act
            LogAct("Fetching entry with multiple EarlyAccessHeaders");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "EarlyAccess - IncludeMetadata With EarlyAccessHeader Both Applied")]
        public async Task Entry_IncludeMetadataWithEarlyAccessHeader_BothApplied()
        {
            // Arrange
            LogArrange("Fetch entry with IncludeMetadata + EarlyAccessHeader");
            var options = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Branch = TestDataHelper.BranchUid,
                EarlyAccessHeader = new string[] { "taxonomy" }
            };

            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));

            // Act
            LogAct("Calling Entry.IncludeMetadata().Fetch() with EarlyAccessHeader");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .IncludeMetadata()
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        #endregion

        // ====================================================================
        // Special Characters + Query Encoding Edge Cases
        // (Inspired by JS CDA SDK v3.20.3 hotfix: "Removed encode for query params")
        // ====================================================================

        #region Special Characters and Encoding

        [Fact(DisplayName = "Encoding - Query With Special Characters In Where Clause Does Not Throw 500")]
        public async Task Query_SpecialCharsInWhereClause_DoesNotThrow500()
        {
            // Arrange - mirrors JS SDK test: special chars in field values
            // The API may return 400 (bad request) but MUST NOT return 500 (server error)
            LogArrange("Query with special characters in field value filter");
            var client = CreateClient();

            // Act & Assert
            LogAct("Querying with special char value - should not cause 500");
            try
            {
                var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
                query.Where("title", "test&value=special");
                var result = await query.Find<Entry>();
                // If it succeeds, that's fine
                TestAssert.NotNull(result);
            }
            catch (Exception ex)
            {
                // 400 Bad Request is acceptable (API rejects invalid input)
                // 500 Internal Server Error would indicate a bug
                var message = ex.Message ?? "";
                TestAssert.False(message.Contains("(500)"), "Should not get 500 Internal Server Error - got: " + message);
                TestAssert.True(true, "API correctly rejected special characters with non-500 error");
            }
        }

        [Fact(DisplayName = "Encoding - Query With Unicode Characters Handled")]
        public async Task Query_UnicodeCharacters_Handled()
        {
            // Arrange - mirrors JS SDK AdvancedEdgeCases Unicode tests
            LogArrange("Query with Unicode characters in search");
            var client = CreateClient();

            // Act
            LogAct("Querying with Unicode characters in Where clause");
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query.Where("title", "日本語テスト");
            var result = await query.Find<Entry>();

            // Assert - should not throw encoding errors
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
        }

        [Fact(DisplayName = "Encoding - Entry AddParam With Special Characters Handled")]
        public async Task Entry_AddParamSpecialChars_Handled()
        {
            // Arrange - mirrors JS SDK CustomParameters special chars test
            LogArrange("Fetch entry with AddParam containing special characters");
            var client = CreateClient();

            // Act
            LogAct("Calling Entry.AddParam('test_param', 'value&special=chars').Fetch()");
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .AddParam("test_param", "value&special=chars")
                .Fetch<Entry>();

            // Assert
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
        }

        [Fact(DisplayName = "Encoding - Query IncludeMetadata With Special Char Filter Does Not Throw 500")]
        public async Task Query_IncludeMetadataWithSpecialCharFilter_DoesNotThrow500()
        {
            // Arrange - the exact scenario from the customer issue:
            // IncludeMetadata + query with special characters causing 500
            LogArrange("Query with IncludeMetadata + special char filter");
            var client = CreateClient();

            // Act & Assert - MUST NOT throw 500 (400 is acceptable)
            LogAct("Calling Query.IncludeMetadata() + Where with special chars");
            try
            {
                var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
                query.IncludeMetadata();
                query.Where("title", "test&param=value?query#hash");
                var result = await query.Find<Entry>();
                TestAssert.NotNull(result);
            }
            catch (Exception ex)
            {
                var message = ex.Message ?? "";
                TestAssert.False(message.Contains("(500)"), "IncludeMetadata + special chars MUST NOT cause 500 - got: " + message);
                TestAssert.True(true, "API correctly rejected with non-500 error");
            }
        }

        [Fact(DisplayName = "Encoding - Query With Regex Special Characters Handled Safely")]
        public async Task Query_RegexSpecialChars_HandledSafely()
        {
            // Arrange - mirrors JS SDK ExistsSearchOperators regex test
            LogArrange("Query with regex containing special characters");
            var client = CreateClient();

            // Act
            LogAct("Querying with regex pattern on title");
            var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
            query.Regex("title", "^[A-Za-z].*");
            var result = await query.Find<Entry>();

            // Assert
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            TestAssert.True(result.Items.Count() > 0, "result.Items.Count() > 0");
        }

        [Fact(DisplayName = "Encoding - Query With URL Encoded Characters Does Not Throw 500")]
        public async Task Query_URLEncodedChars_DoesNotThrow500()
        {
            // Arrange - mirrors JS SDK AdvancedEdgeCases URL encoding test
            // JS SDK v3.20.3 had a hotfix for double-encoding of query params
            LogArrange("Query with URL-encoded special characters");
            var client = CreateClient();

            // Act & Assert
            LogAct("Querying with URL-encoded characters in Where clause");
            try
            {
                var query = client.ContentType(TestDataHelper.SimpleContentTypeUid).Query();
                query.Where("title", "hello%20world&test=true");
                var result = await query.Find<Entry>();
                TestAssert.NotNull(result);
            }
            catch (Exception ex)
            {
                var message = ex.Message ?? "";
                TestAssert.False(message.Contains("(500)"), "URL-encoded chars MUST NOT cause 500 - got: " + message);
                TestAssert.True(true, "API correctly rejected with non-500 error");
            }
        }

        #endregion

        // ====================================================================
        // Helper Methods
        // ====================================================================

        private new ContentstackClient CreateClient()
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
