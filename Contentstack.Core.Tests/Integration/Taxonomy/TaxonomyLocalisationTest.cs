using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Contentstack.Core.Configuration;
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
        /// Creates a client scoped to the gadgets taxonomy stack.
        /// Host must be set from config — the test stack lives on a non-prod CDN.
        /// </summary>
        private ContentstackClient CreateGadgetsClient()
        {
            var options = new ContentstackOptions
            {
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment,
                Host = TestDataHelper.Host
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

            LogAct("Calling Taxonomies(uid).SetLocale(locale).Fetch<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .SetLocale(TestDataHelper.TaxPublishLocale)
                .Fetch<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result["uid"]?.ToString());
            // The name should differ from the master locale (en-us) value
            Assert.NotNull(result["name"]?.ToString());
        }

        // ── 2. Find all taxonomies ────────────────────────────────────────────

        [Fact(DisplayName = "TaxPublish - Find all terms without locale returns master-locale terms")]
        public async Task Find_AllTaxonomies_ReturnsCollection()
        {
            LogArrange("Fetching all terms in master locale (no locale filter)");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);

            var client = CreateGadgetsClient();

            LogAct("Calling Taxonomies(uid).Terms().Find<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Terms()
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Any());
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

            LogAct("Calling Term(termUid).SetLocale(locale).IncludeFallback().Fetch<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(termUid)
                .SetLocale(TestDataHelper.TaxPublishLocale)
                .IncludeFallback()
                .Fetch<Newtonsoft.Json.Linq.JObject>();

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

        // ── 8. Term hierarchy — Depth / IncludeBranch ────────────────────────

        /// <summary>
        /// Walks the gadgets taxonomy to find a term with at least one descendant that itself
        /// has a descendant — i.e. a parent/child/grandchild chain — for hierarchy-depth tests.
        /// Returns (null, null, null) if no such chain exists in the fixture data.
        /// </summary>
        private async Task<(string parentUid, string childUid, string grandchildUid)> GetTermHierarchyAsync(ContentstackClient client)
        {
            var terms = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Terms()
                .Find<Newtonsoft.Json.Linq.JObject>();

            foreach (var candidateParent in terms.Items)
            {
                var parentUid = candidateParent["uid"]?.ToString();
                if (string.IsNullOrEmpty(parentUid)) continue;

                var children = await client
                    .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                    .Term(parentUid)
                    .Descendants<Newtonsoft.Json.Linq.JArray>();
                var childUid = children?.FirstOrDefault()?["uid"]?.ToString();
                if (string.IsNullOrEmpty(childUid)) continue;

                var grandchildren = await client
                    .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                    .Term(childUid)
                    .Descendants<Newtonsoft.Json.Linq.JArray>();
                var grandchildUid = grandchildren?.FirstOrDefault()?["uid"]?.ToString();
                if (string.IsNullOrEmpty(grandchildUid)) continue;

                return (parentUid, childUid, grandchildUid);
            }
            return (null, null, null);
        }

        [Fact(DisplayName = "TaxPublish - Term.Descendants with depth=1 returns only direct children")]
        public async Task Term_Descendants_WithDepth1_ReturnsOnlyDirectChildren()
        {
            var client = CreateGadgetsClient();
            var (parentUid, childUid, grandchildUid) = await GetTermHierarchyAsync(client);

            if (string.IsNullOrEmpty(parentUid))
            {
                Output.WriteLine("No parent/child/grandchild term chain found — skipping test.");
                return;
            }

            LogArrange("Fetching direct-child-only descendants (depth=1)");
            LogContext("ParentTermUid", parentUid);

            LogAct("Calling Term(parentUid).Depth(1).Descendants<JArray>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(parentUid)
                .Depth(1)
                .Descendants<Newtonsoft.Json.Linq.JArray>();

            LogAssert("Verifying only the direct child is present, not the grandchild");
            Assert.NotNull(result);
            var uids = result.Select(t => t["uid"]?.ToString()).ToList();
            Assert.Contains(childUid, uids);
            Assert.DoesNotContain(grandchildUid, uids);
        }

        [Fact(DisplayName = "TaxPublish - Term.Descendants with depth=2 includes grandchildren")]
        public async Task Term_Descendants_WithDepth2_IncludesGrandchildren()
        {
            var client = CreateGadgetsClient();
            var (parentUid, childUid, grandchildUid) = await GetTermHierarchyAsync(client);

            if (string.IsNullOrEmpty(parentUid))
            {
                Output.WriteLine("No parent/child/grandchild term chain found — skipping test.");
                return;
            }

            LogArrange("Fetching descendants two levels deep (depth=2)");
            LogContext("ParentTermUid", parentUid);

            LogAct("Calling Term(parentUid).Depth(2).Descendants<JArray>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(parentUid)
                .Depth(2)
                .Descendants<Newtonsoft.Json.Linq.JArray>();

            LogAssert("Verifying both the child and grandchild are present");
            Assert.NotNull(result);
            var uids = result.Select(t => t["uid"]?.ToString()).ToList();
            Assert.Contains(childUid, uids);
            Assert.Contains(grandchildUid, uids);
        }

        [Fact(DisplayName = "TaxPublish - Term.Ancestors for a grandchild term returns the full parent chain")]
        public async Task Term_Ancestors_ForGrandchildTerm_ReturnsFullChain()
        {
            var client = CreateGadgetsClient();
            var (parentUid, childUid, grandchildUid) = await GetTermHierarchyAsync(client);

            if (string.IsNullOrEmpty(grandchildUid))
            {
                Output.WriteLine("No parent/child/grandchild term chain found — skipping test.");
                return;
            }

            LogArrange("Fetching ancestors for the grandchild term");
            LogContext("GrandchildTermUid", grandchildUid);

            LogAct("Calling Term(grandchildUid).Ancestors<JArray>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(grandchildUid)
                .Ancestors<Newtonsoft.Json.Linq.JArray>();

            LogAssert("Verifying both the parent and child (grandparent chain) are present");
            Assert.NotNull(result);
            var uids = result.Select(t => t["uid"]?.ToString()).ToList();
            Assert.Contains(childUid, uids);
            Assert.Contains(parentUid, uids);
        }

        [Fact(DisplayName = "TaxPublish - TermQuery.Find with depth limits the returned hierarchy")]
        public async Task TermQuery_Find_WithDepth_LimitsHierarchyDepth()
        {
            var client = CreateGadgetsClient();

            LogArrange("Finding terms with depth=1");
            LogContext("TaxonomyUid", TestDataHelper.TaxPublishTaxonomyUid);

            LogAct("Calling Terms().Depth(1).Find<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Terms()
                .Depth(1)
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }

        [Fact(DisplayName = "TaxPublish - Term.Fetch with IncludeBranch returns branch info")]
        public async Task Term_Fetch_WithIncludeBranch_ReturnsBranchInfo()
        {
            var client = CreateGadgetsClient();
            var termUid = await GetFirstTermUidAsync(client);

            if (string.IsNullOrEmpty(termUid))
            {
                Output.WriteLine("No term UID found — skipping test.");
                return;
            }

            LogArrange("Fetching a term with branch info included");
            LogContext("TermUid", termUid);

            LogAct("Calling Term(termUid).IncludeBranch().Fetch<JObject>()");
            var result = await client
                .Taxonomies(TestDataHelper.TaxPublishTaxonomyUid)
                .Term(termUid)
                .IncludeBranch()
                .Fetch<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result["uid"]?.ToString());
        }

        // ── 9. List all taxonomies ────────────────────────────────────────────

        [Fact(DisplayName = "TaxPublish - List all taxonomies returns a collection")]
        public async Task List_AllTaxonomies_ReturnsCollection()
        {
            var client = CreateGadgetsClient();

            LogArrange("Listing all published taxonomies");

            LogAct("Calling Taxonomies().Find<JObject>()");
            var result = await client
                .Taxonomies()
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Any());
        }

        [Fact(DisplayName = "TaxPublish - List all taxonomies with skip/limit returns a paged subset")]
        public async Task List_AllTaxonomies_WithSkipAndLimit_ReturnsPagedSubset()
        {
            var client = CreateGadgetsClient();

            LogArrange("Listing taxonomies with skip/limit");

            LogAct("Calling Taxonomies().AddParam(\"skip\",\"0\").AddParam(\"limit\",\"1\").Find<JObject>()");
            var result = await client
                .Taxonomies()
                .AddParam("skip", "0")
                .AddParam("limit", "1")
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response is limited to at most one item");
            Assert.NotNull(result);
            Assert.True(result.Items.Count() <= 1);
        }

        [Fact(DisplayName = "TaxPublish - List all taxonomies with include_count returns a count")]
        public async Task List_AllTaxonomies_WithIncludeCount_ReturnsCount()
        {
            var client = CreateGadgetsClient();

            LogArrange("Listing taxonomies with include_count");

            LogAct("Calling Taxonomies().AddParam(\"include_count\",\"true\").Find<JObject>()");
            var result = await client
                .Taxonomies()
                .AddParam("include_count", "true")
                .Find<Newtonsoft.Json.Linq.JObject>();

            LogAssert("Verifying response");
            Assert.NotNull(result);
        }
    }
}
