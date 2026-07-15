using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests.Integration.Taxonomy
{
    /// <summary>
    /// End-to-end runner for all taxonomy localisation CDA calls.
    /// Mirrors the TypeScript smoke-test script — runs every endpoint in sequence,
    /// prints the SDK call, real HTTP request (URL + headers), and full JSON response.
    /// Run with:
    ///   dotnet test --filter "FullyQualifiedName~TaxonomyLocalisationRunner" --logger "console;verbosity=detailed"
    /// </summary>
    [Trait("Category", "TaxonomyLocalisationRunner")]
    public class TaxonomyLocalisationRunner
    {
        private readonly ITestOutputHelper _out;

        private const string ApiKey         = "blt168147f34138ebac";
        private const string DeliveryToken  = "cs91c6d782d3c9ca953c0a2685";
        private const string Environment    = "dev";
        private const string TaxonomyUid    = "gadgets";
        private const string Locale         = "hi-in";

        public TaxonomyLocalisationRunner(ITestOutputHelper output)
        {
            _out = output;
        }

        private ContentstackClient CreateClient()
        {
            var options = new ContentstackOptions
            {
                ApiKey        = ApiKey,
                DeliveryToken = DeliveryToken,
                Environment   = Environment
            };
            return new ContentstackClient(options);
        }

        private void Section(int num, string title)
        {
            var line = new string('─', 60);
            _out.WriteLine("");
            _out.WriteLine(line);
            _out.WriteLine($"  {num}. {title}");
            _out.WriteLine(line);
        }

        private void PrintResponse(string label, object data)
        {
            _out.WriteLine($"✅  {label}");
            _out.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        private void PrintError(string label, Exception ex)
        {
            _out.WriteLine($"❌  {label}");
            _out.WriteLine($"   {ex.Message}");
        }

        [Fact(DisplayName = "TaxonomyLocalisationRunner - Full end-to-end run of all CDA localisation calls")]
        public async Task Run_All_TaxonomyLocalisation_Calls()
        {
            var client = CreateClient();
            string firstTermUid = null;

            // ── 1. GET /v3/taxonomies/gadgets?environment=dev ─────────────────
            Section(1, $"Fetch taxonomy  uid={TaxonomyUid}  (master locale)");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Fetch<JObject>()");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Fetch<JObject>();
                PrintResponse($"GET /taxonomies/{TaxonomyUid}", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}", ex); }

            // ── 2. GET /v3/taxonomies/gadgets?environment=dev&locale=hi-in ────
            Section(2, $"Fetch taxonomy  uid={TaxonomyUid}  locale={Locale}");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Fetch<JObject>(\"{Locale}\")");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Fetch<JObject>(Locale);
                PrintResponse($"GET /taxonomies/{TaxonomyUid}?locale={Locale}", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}?locale={Locale}", ex); }

            // ── 3. GET /v3/taxonomies/gadgets/terms?environment=dev&locale=hi-in
            Section(3, $"Find terms  taxonomy={TaxonomyUid}  locale={Locale}");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Terms().SetLocale(\"{Locale}\").Find<JObject>()");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Terms()
                    .SetLocale(Locale)
                    .Find<JObject>();
                PrintResponse($"GET /taxonomies/{TaxonomyUid}/terms?locale={Locale}", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}/terms?locale={Locale}", ex); }

            // ── 4. GET /v3/taxonomies/gadgets/terms?locale=hi-in&include_fallback=true
            Section(4, $"Find terms  taxonomy={TaxonomyUid}  locale={Locale}  include_fallback=true");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Terms().SetLocale(\"{Locale}\").IncludeFallback().Find<JObject>()");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Terms()
                    .SetLocale(Locale)
                    .IncludeFallback()
                    .Find<JObject>();
                PrintResponse($"GET /taxonomies/{TaxonomyUid}/terms?locale={Locale}&include_fallback=true", result);

                // Grab first term uid for subsequent calls
                var terms = result?.Items;
                foreach (var t in terms ?? System.Array.Empty<JObject>())
                {
                    firstTermUid = t?["uid"]?.ToString();
                    if (!string.IsNullOrEmpty(firstTermUid)) break;
                }
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}/terms with fallback", ex); }

            if (string.IsNullOrEmpty(firstTermUid))
            {
                _out.WriteLine("");
                _out.WriteLine("⚠️  No term UID found — skipping single-term calls.");
                return;
            }

            _out.WriteLine($"\n📌  Using term uid = \"{firstTermUid}\" for single-term calls.");

            // ── 5. GET /v3/taxonomies/gadgets/terms/:uid?locale=hi-in ─────────
            Section(5, $"Fetch single term  uid={firstTermUid}  locale={Locale}");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Term(\"{firstTermUid}\").Fetch<JObject>(\"{Locale}\")");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Term(firstTermUid).Fetch<JObject>(Locale);
                PrintResponse($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}?locale={Locale}", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}?locale={Locale}", ex); }

            // ── 6. GET /v3/taxonomies/gadgets/terms/:uid/locales ──────────────
            Section(6, $"Term locales  uid={firstTermUid}");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Term(\"{firstTermUid}\").Locales<JToken>()");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Term(firstTermUid).Locales<JToken>();
                PrintResponse($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}/locales", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}/locales", ex); }

            // ── 7. GET /v3/taxonomies/gadgets/terms/:uid/ancestors ────────────
            Section(7, $"Term ancestors  uid={firstTermUid}");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Term(\"{firstTermUid}\").Ancestors<JToken>()");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Term(firstTermUid).Ancestors<JToken>();
                PrintResponse($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}/ancestors", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}/ancestors", ex); }

            // ── 8. GET /v3/taxonomies/gadgets/terms/:uid/descendants ──────────
            Section(8, $"Term descendants  uid={firstTermUid}");
            _out.WriteLine($"SDK  : client.Taxonomies(\"{TaxonomyUid}\").Term(\"{firstTermUid}\").Descendants<JToken>()");
            try
            {
                var result = await client.Taxonomies(TaxonomyUid).Term(firstTermUid).Descendants<JToken>();
                PrintResponse($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}/descendants", result);
            }
            catch (Exception ex) { PrintError($"GET /taxonomies/{TaxonomyUid}/terms/{firstTermUid}/descendants", ex); }
        }
    }
}
