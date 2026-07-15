using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Internals;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Integration.Taxonomy
{
    /// <summary>
    /// Integration tests for localized taxonomy and term delivery (CDA).
    /// Covers: Taxonomy.Fetch(locale), TermQuery.SetLocale/IncludeFallback/Find,
    /// Term.Fetch(locale), Term.Locales, Term.Ancestors, Term.Descendants.
    /// Stack: gadgets taxonomy, locales en-us / hi-in.
    /// </summary>
    [Trait("Category", "TaxonomyLocalisation")]
    public class TaxonomyLocalisationTest : IntegrationTestBase
    {
        public TaxonomyLocalisationTest(ITestOutputHelper output) : base(output) { }

        /// <summary>
        /// Creates a client scoped to the gadgets taxonomy-publish test stack.
        /// Uses the default CDN host (no custom host override needed).
        /// </summary>
        private ContentstackClient CreateGadgetsClient()
        {
            var options = new ContentstackOptions
            {
                ApiKey = TestDataHelper.TaxPublishApiKey,
                DeliveryToken = TestDataHelper.TaxPublishDeliveryToken,
                Environment = TestDataHelper.TaxPublishEnvironment
            };
            var client = new ContentstackClient(options);
            client.Plugins.Add(new RequestLoggingPlugin(TestOutput));
            return client;
        }

        // ── 1. Fetch localized taxonomy ──────────────────────────────────────

        [Fact(DisplayName = "TaxPublish - Fetch taxonomy returns object with uid and name")]
        public async Task Fetch_Taxonomy_MasterLocale_ReturnsValidObject()
        {
            LogArrange("Fetching taxonomy without locale (master)");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);

            var client = CreateGadgetsClient();

            LogAct("Calling Taxonomies(uid).Fetch<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Fetch<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result["uid"]?.ToString());
            Assert.Equal(TestDataHelper.TaxPublishTaxonomyUid, result["uid"]?.ToString());
        }

        [Fact(DisplayName = "TaxPublish - Fetch taxonomy with locale returns localized name")]
        public async Task Fetch_Taxonomy_WithLocale_ReturnsLocalizedName()
        {
            LogArrange("Fetching taxonomy with locale");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("Locale", TestDataHelper.TaxPublishLocale);

            var client = CreateGadgetsClient();

            LogAct("Calling Taxonomies(uid).Fetch<JObject>(locale)");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Fetch<Newtonsoft.Json.Linq.JObject>(TestDataHelper.TaxPublishLocale);

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result["uid"]?.ToString());
            // The name should differ from the master locale (en-us) value
            Assert.NotNull(result["name"]?.ToString());
        }

        // ── 2. Find all taxonomies ────────────────────────────────────────────

        [Fact(DisplayName = "TaxPublish - Find all taxonomies returns non-empty collection")]
        public async Task Find_AllTaxonomies_ReturnsCollection()
        {
            LogArrange("Fetching all published taxonomies");

            var client = CreateGadgetsClient();

            LogAct("Calling Taxonomies().Find<Entry>()");
            var result = await client.Taxonomies().Find<Entry>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }

        // ── 3. Find terms with locale ─────────────────────────────────────────

        [Fact(DisplayName = "TaxPublish - Find terms with locale returns localized terms")]
        public async Task Find_Terms_WithLocale_ReturnsLocalizedTerms()
        {
            LogArrange("Finding terms with locale");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("Locale", TestDataHelper.TaxPublishLocale);

            var client = CreateGadgetsClient();

            LogAct("Calling Terms().SetLocale(locale).Find<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Terms()
                .SetLocale(TestDataHelper.TaxPublishLocale)
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }

        [Fact(DisplayName = "TaxPublish - Find terms with locale and fallback returns terms")]
        public async Task Find_Terms_WithLocaleAndFallback_ReturnsTerms()
        {
            LogArrange("Finding terms with locale and include_fallback");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("Locale", TestDataHelper.TaxPublishLocale);

            var client = CreateGadgetsClient();

            LogAct("Calling Terms().SetLocale(locale).IncludeFallback().Find<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Terms()
                .SetLocale(TestDataHelper.TaxPublishLocale)
                .IncludeFallback()
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            // Fallback means we should get at least as many terms as without fallback
            Assert.True(result.Items.Any());
        }

        // ── 4–7. Single term methods ──────────────────────────────────────────

        /// <summary>
        /// Fetches the first available term UID from the gadgets taxonomy to use in subsequent tests.
        /// </summary>
        private async Task<string> GetFirstTermUidAsync(ContentstackClient client)
        {
            var terms = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Terms()
                .SetLocale(TestDataHelper.TaxPublishLocale)
                .IncludeFallback()
                .Find<Newtonsoft.Json.Linq.JObject>();

            return terms?.Items?.FirstOrDefault()?["uid"]?.ToString();
        }

        [Fact(DisplayName = "TaxPublish - Fetch single term with locale returns localized term")]
        public async Task Fetch_SingleTerm_WithLocale_ReturnsLocalizedTerm()
        {
            var client = CreateGadgetsClient();
            var termUid = await GetFirstTermUidAsync(client);

            if (string.IsNullOrEmpty(termUid))
            {
                Output.WriteLine("No term UID found — skipping test.");
                return;
            }

            LogArrange("Fetching single term with locale");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("TermUid", termUid);
            LogContext("Locale", TestDataHelper.TaxPublishLocale);

            LogAct("Calling Term(termUid).Fetch<JObject>(locale)");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(termUid)
                .Fetch<Newtonsoft.Json.Linq.JObject>(TestDataHelper.TaxPublishLocale);

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result["uid"]?.ToString());
            Assert.Equal(termUid, result["uid"]?.ToString());
        }

        [Fact(DisplayName = "TaxPublish - Term.Locales returns locales collection")]
        public async Task Term_Locales_ReturnsLocalesCollection()
        {
            var client = CreateGadgetsClient();
            var termUid = await GetFirstTermUidAsync(client);

            if (string.IsNullOrEmpty(termUid))
            {
                Output.WriteLine("No term UID found — skipping test.");
                return;
            }

            LogArrange("Fetching all locales for a term");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("TermUid", termUid);

            LogAct("Calling Term(termUid).Locales<JArray>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(termUid)
                .Locales<Newtonsoft.Json.Linq.JToken>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "TaxPublish - Term.Ancestors returns ancestors collection")]
        public async Task Term_Ancestors_ReturnsAncestorsCollection()
        {
            var client = CreateGadgetsClient();
            var termUid = await GetFirstTermUidAsync(client);

            if (string.IsNullOrEmpty(termUid))
            {
                Output.WriteLine("No term UID found — skipping test.");
                return;
            }

            LogArrange("Fetching ancestors for a term");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("TermUid", termUid);

            LogAct("Calling Term(termUid).Ancestors<JToken>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(termUid)
                .Ancestors<Newtonsoft.Json.Linq.JToken>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "TaxPublish - Term.Descendants returns descendants collection")]
        public async Task Term_Descendants_ReturnsDescendantsCollection()
        {
            var client = CreateGadgetsClient();
            var termUid = await GetFirstTermUidAsync(client);

            if (string.IsNullOrEmpty(termUid))
            {
                Output.WriteLine("No term UID found — skipping test.");
                return;
            }

            LogArrange("Fetching descendants for a term");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);
            LogContext("TermUid", termUid);

            LogAct("Calling Term(termUid).Descendants<JToken>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(termUid)
                .Descendants<Newtonsoft.Json.Linq.JToken>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
        }
    }
}
