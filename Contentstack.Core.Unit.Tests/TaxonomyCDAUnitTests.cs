using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for the Taxonomy CDA classes (TaxonomyCDA, TaxonomyQuery,
    /// TaxonomyTerm, TaxonomyTermQuery).
    ///
    /// All tests are pure unit tests — no real API calls are made.
    /// They verify URL-building logic, query-parameter accumulation,
    /// method chaining, and that factory methods return the correct types.
    /// </summary>
    public class TaxonomyCDAUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public TaxonomyCDAUnitTests()
        {
            var options = new ContentstackOptions
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>()
            };
            _client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static Dictionary<string, object> GetUrlQueries(object instance)
        {
            var field = instance.GetType().GetField(
                "_urlQueries",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(instance) as Dictionary<string, object>;
        }

        // ── ContentstackClient factory methods ───────────────────────────────

        [Fact(DisplayName = "ContentstackClient.Taxonomy() returns TaxonomyQuery")]
        public void Taxonomy_NoArg_ReturnsTaxonomyQuery()
        {
            var result = _client.Taxonomy();
            Assert.IsType<TaxonomyQuery>(result);
        }

        [Fact(DisplayName = "ContentstackClient.Taxonomy(uid) returns TaxonomyCDA")]
        public void Taxonomy_WithUid_ReturnsTaxonomyCDA()
        {
            var result = _client.Taxonomy("regions");
            Assert.IsType<TaxonomyCDA>(result);
        }

        [Fact(DisplayName = "ContentstackClient.Taxonomies() still returns Taxonomy (entry operator — unchanged)")]
        public void Taxonomies_StillReturnsOldTaxonomyClass()
        {
            var result = _client.Taxonomies();
            Assert.IsType<Taxonomy>(result);
        }

        // ── TaxonomyCDA — modifier methods ───────────────────────────────────

        [Fact(DisplayName = "TaxonomyCDA.SetLocale sets locale query param")]
        public void TaxonomyCDA_SetLocale_SetsQueryParam()
        {
            var tax = _client.Taxonomy("regions");
            tax.SetLocale("en-us");

            var queries = GetUrlQueries(tax);
            Assert.True(queries.ContainsKey("locale"));
            Assert.Equal("en-us", queries["locale"]);
        }

        [Fact(DisplayName = "TaxonomyCDA.IncludeFallback sets include_fallback=true")]
        public void TaxonomyCDA_IncludeFallback_SetsQueryParam()
        {
            var tax = _client.Taxonomy("regions");
            tax.IncludeFallback();

            var queries = GetUrlQueries(tax);
            Assert.True(queries.ContainsKey("include_fallback"));
            Assert.Equal("true", queries["include_fallback"]);
        }

        [Fact(DisplayName = "TaxonomyCDA.IncludeBranch sets include_branch=true")]
        public void TaxonomyCDA_IncludeBranch_SetsQueryParam()
        {
            var tax = _client.Taxonomy("regions");
            tax.IncludeBranch();

            var queries = GetUrlQueries(tax);
            Assert.True(queries.ContainsKey("include_branch"));
            Assert.Equal("true", queries["include_branch"]);
        }

        [Fact(DisplayName = "TaxonomyCDA.Param adds arbitrary query param")]
        public void TaxonomyCDA_Param_AddsArbitraryParam()
        {
            var tax = _client.Taxonomy("regions");
            tax.Param("custom_key", "custom_value");

            var queries = GetUrlQueries(tax);
            Assert.True(queries.ContainsKey("custom_key"));
            Assert.Equal("custom_value", queries["custom_key"]);
        }

        [Fact(DisplayName = "TaxonomyCDA methods chain fluently and return same instance")]
        public void TaxonomyCDA_MethodChaining_ReturnsSameInstance()
        {
            var tax = _client.Taxonomy("regions");
            var result = tax
                .SetLocale("en-us")
                .IncludeFallback()
                .IncludeBranch()
                .Param("k", "v");

            Assert.Same(tax, result);
        }

        // ── TaxonomyCDA — factory methods for terms ──────────────────────────

        [Fact(DisplayName = "TaxonomyCDA.Term() returns TaxonomyTermQuery")]
        public void TaxonomyCDA_Term_NoArg_ReturnsTaxonomyTermQuery()
        {
            var result = _client.Taxonomy("regions").Term();
            Assert.IsType<TaxonomyTermQuery>(result);
        }

        [Fact(DisplayName = "TaxonomyCDA.Term(uid) returns TaxonomyTerm")]
        public void TaxonomyCDA_Term_WithUid_ReturnsTaxonomyTerm()
        {
            var result = _client.Taxonomy("regions").Term("california");
            Assert.IsType<TaxonomyTerm>(result);
        }

        // ── TaxonomyQuery — modifier methods ─────────────────────────────────

        [Fact(DisplayName = "TaxonomyQuery.Skip sets skip query param")]
        public void TaxonomyQuery_Skip_SetsQueryParam()
        {
            var query = _client.Taxonomy();
            query.Skip(10);

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("skip"));
            Assert.Equal(10, queries["skip"]);
        }

        [Fact(DisplayName = "TaxonomyQuery.Limit sets limit query param")]
        public void TaxonomyQuery_Limit_SetsQueryParam()
        {
            var query = _client.Taxonomy();
            query.Limit(25);

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("limit"));
            Assert.Equal(25, queries["limit"]);
        }

        [Fact(DisplayName = "TaxonomyQuery.IncludeCount sets include_count=true")]
        public void TaxonomyQuery_IncludeCount_SetsQueryParam()
        {
            var query = _client.Taxonomy();
            query.IncludeCount();

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("include_count"));
            Assert.Equal("true", queries["include_count"]);
        }

        [Fact(DisplayName = "TaxonomyQuery methods chain fluently and return same instance")]
        public void TaxonomyQuery_MethodChaining_ReturnsSameInstance()
        {
            var query = _client.Taxonomy();
            var result = query.Skip(0).Limit(10).IncludeCount().Param("k", "v");

            Assert.Same(query, result);
        }

        // ── TaxonomyTerm — modifier methods ──────────────────────────────────

        [Fact(DisplayName = "TaxonomyTerm.SetLocale sets locale query param")]
        public void TaxonomyTerm_SetLocale_SetsQueryParam()
        {
            var term = _client.Taxonomy("regions").Term("california");
            term.SetLocale("fr-fr");

            var queries = GetUrlQueries(term);
            Assert.True(queries.ContainsKey("locale"));
            Assert.Equal("fr-fr", queries["locale"]);
        }

        [Fact(DisplayName = "TaxonomyTerm.Depth sets depth query param")]
        public void TaxonomyTerm_Depth_SetsQueryParam()
        {
            var term = _client.Taxonomy("regions").Term("california");
            term.Depth(3);

            var queries = GetUrlQueries(term);
            Assert.True(queries.ContainsKey("depth"));
            Assert.Equal(3, queries["depth"]);
        }

        [Fact(DisplayName = "TaxonomyTerm.IncludeFallback sets include_fallback=true")]
        public void TaxonomyTerm_IncludeFallback_SetsQueryParam()
        {
            var term = _client.Taxonomy("regions").Term("california");
            term.IncludeFallback();

            var queries = GetUrlQueries(term);
            Assert.True(queries.ContainsKey("include_fallback"));
            Assert.Equal("true", queries["include_fallback"]);
        }

        [Fact(DisplayName = "TaxonomyTerm.IncludeBranch sets include_branch=true")]
        public void TaxonomyTerm_IncludeBranch_SetsQueryParam()
        {
            var term = _client.Taxonomy("regions").Term("california");
            term.IncludeBranch();

            var queries = GetUrlQueries(term);
            Assert.True(queries.ContainsKey("include_branch"));
            Assert.Equal("true", queries["include_branch"]);
        }

        [Fact(DisplayName = "TaxonomyTerm.Param adds arbitrary query param")]
        public void TaxonomyTerm_Param_AddsArbitraryParam()
        {
            var term = _client.Taxonomy("regions").Term("california");
            term.Param("my_key", 42);

            var queries = GetUrlQueries(term);
            Assert.True(queries.ContainsKey("my_key"));
            Assert.Equal(42, queries["my_key"]);
        }

        [Fact(DisplayName = "TaxonomyTerm methods chain fluently and return same instance")]
        public void TaxonomyTerm_MethodChaining_ReturnsSameInstance()
        {
            var term = _client.Taxonomy("regions").Term("california");
            var result = term
                .SetLocale("en-us")
                .Depth(5)
                .IncludeFallback()
                .IncludeBranch()
                .Param("k", "v");

            Assert.Same(term, result);
        }

        // ── TaxonomyTermQuery — modifier methods ──────────────────────────────

        [Fact(DisplayName = "TaxonomyTermQuery.SetLocale sets locale query param")]
        public void TaxonomyTermQuery_SetLocale_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.SetLocale("de-de");

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("locale"));
            Assert.Equal("de-de", queries["locale"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery.Depth sets depth query param")]
        public void TaxonomyTermQuery_Depth_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.Depth(2);

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("depth"));
            Assert.Equal(2, queries["depth"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery.Skip sets skip query param")]
        public void TaxonomyTermQuery_Skip_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.Skip(5);

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("skip"));
            Assert.Equal(5, queries["skip"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery.Limit sets limit query param")]
        public void TaxonomyTermQuery_Limit_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.Limit(50);

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("limit"));
            Assert.Equal(50, queries["limit"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery.IncludeFallback sets include_fallback=true")]
        public void TaxonomyTermQuery_IncludeFallback_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.IncludeFallback();

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("include_fallback"));
            Assert.Equal("true", queries["include_fallback"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery.IncludeBranch sets include_branch=true")]
        public void TaxonomyTermQuery_IncludeBranch_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.IncludeBranch();

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("include_branch"));
            Assert.Equal("true", queries["include_branch"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery.IncludeCount sets include_count=true")]
        public void TaxonomyTermQuery_IncludeCount_SetsQueryParam()
        {
            var query = _client.Taxonomy("regions").Term();
            query.IncludeCount();

            var queries = GetUrlQueries(query);
            Assert.True(queries.ContainsKey("include_count"));
            Assert.Equal("true", queries["include_count"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery methods chain fluently and return same instance")]
        public void TaxonomyTermQuery_MethodChaining_ReturnsSameInstance()
        {
            var query = _client.Taxonomy("regions").Term();
            var result = query
                .SetLocale("en-us")
                .Depth(3)
                .Skip(0)
                .Limit(50)
                .IncludeFallback()
                .IncludeBranch()
                .IncludeCount()
                .Param("k", "v");

            Assert.Same(query, result);
        }

        // ── Guard clause tests ────────────────────────────────────────────────

        [Fact(DisplayName = "ContentstackClient.Taxonomy(null) throws ArgumentNullException")]
        public void Taxonomy_NullUid_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _client.Taxonomy(null));
        }

        [Fact(DisplayName = "TaxonomyCDA.Term(null) throws ArgumentNullException")]
        public void TaxonomyCDA_Term_NullUid_ThrowsArgumentNullException()
        {
            var tax = _client.Taxonomy("regions");
            Assert.Throws<ArgumentNullException>(() => tax.Term(null));
        }

        // ── Multiple params accumulate correctly ───────────────────────────

        [Fact(DisplayName = "TaxonomyCDA accumulates multiple query params independently")]
        public void TaxonomyCDA_MultipleParams_AllPresent()
        {
            var tax = _client.Taxonomy("regions")
                .SetLocale("en-us")
                .IncludeFallback()
                .IncludeBranch()
                .Param("depth", 3);

            var queries = GetUrlQueries(tax);
            Assert.Equal("en-us", queries["locale"]);
            Assert.Equal("true", queries["include_fallback"]);
            Assert.Equal("true", queries["include_branch"]);
            Assert.Equal(3, queries["depth"]);
        }

        [Fact(DisplayName = "TaxonomyTermQuery accumulates multiple query params independently")]
        public void TaxonomyTermQuery_MultipleParams_AllPresent()
        {
            var query = _client.Taxonomy("electronics").Term()
                .SetLocale("fr-fr")
                .Depth(2)
                .Skip(10)
                .Limit(25)
                .IncludeFallback()
                .IncludeCount();

            var queries = GetUrlQueries(query);
            Assert.Equal("fr-fr", queries["locale"]);
            Assert.Equal(2, queries["depth"]);
            Assert.Equal(10, queries["skip"]);
            Assert.Equal(25, queries["limit"]);
            Assert.Equal("true", queries["include_fallback"]);
            Assert.Equal("true", queries["include_count"]);
        }
    }
}
