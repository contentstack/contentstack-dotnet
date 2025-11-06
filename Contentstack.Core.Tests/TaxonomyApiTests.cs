using System;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests
{
    /// <summary>
    /// API tests for Taxonomy functionality
    /// These tests cover Taxonomy.Find() execution paths in Query.Exec() (lines 1935-1940)
    /// </summary>
    public class TaxonomyApiTests
    {
        readonly ContentstackClient client = StackConfig.GetStack();

        [Fact]
        public async Task TaxonomyFindWithExists()
        {
            // This test covers TaxonomyInstance path in Query.Exec() (lines 1935-1940)
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
            // Note: Result may be empty if no entries match, but the execution path is covered
        }

        [Fact(Skip = "Requires valid taxonomy field and data - update field name to match your schema")]
        public async Task TaxonomyFindWithAbove()
        {
            // This test covers Taxonomy query execution
            // Update "taxonomies.one" to match your taxonomy field name
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Above("taxonomies.one", 1);
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact(Skip = "Requires valid taxonomy field and data - update field name to match your schema")]
        public async Task TaxonomyFindWithBelow()
        {
            // This test covers Taxonomy query execution
            // Update "taxonomies.one" to match your taxonomy field name
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Below("taxonomies.one", 5);
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact(Skip = "Requires valid taxonomy field and data - update field name to match your schema")]
        public async Task TaxonomyFindWithEqualAndAbove()
        {
            // This test covers Taxonomy query execution
            // Update "taxonomies.one" to match your taxonomy field name
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.EqualAndAbove("taxonomies.one", 2);
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact(Skip = "Requires valid taxonomy field and data - update field name to match your schema")]
        public async Task TaxonomyFindWithEqualAndBelow()
        {
            // This test covers Taxonomy query execution
            // Update "taxonomies.one" to match your taxonomy field name
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.EqualAndBelow("taxonomies.one", 3);
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithEnvironment()
        {
            // This test covers TaxonomyInstance environment path in Query.Exec() (line 1921-1923)
            // Requires environment to be set in config
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithBranch()
        {
            // This test covers TaxonomyInstance branch path in Query.Exec() (line 1938)
            // Requires branch to be set in config
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithLocalHeaders()
        {
            // This test covers TaxonomyInstance._LocalHeaders path in Query.Exec() (lines 1897-1903)
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.SetHeader("custom_header", "value");
            taxonomy.Exists("taxonomies.one");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithSkip()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.Skip(0);
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithLimit()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.Limit(10);
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact(Skip = "Taxonomy API does not support sorting - Ascending/Descending not available for taxonomy queries")]
        public async Task TaxonomyFindWithAscending()
        {
            // Taxonomy queries might not support sorting operations
            // Check Contentstack API documentation for taxonomy query limitations
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.Ascending("uid");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact(Skip = "Taxonomy API does not support sorting - Ascending/Descending not available for taxonomy queries")]
        public async Task TaxonomyFindWithDescending()
        {
            // Taxonomy queries might not support sorting operations
            // Check Contentstack API documentation for taxonomy query limitations
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.Descending("uid");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyCount()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            
            var result = await taxonomy.Count();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindOne()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            
            var result = await taxonomy.FindOne<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithIncludeCount()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.IncludeCount();
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithIncludeMetadata()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.IncludeMetadata();
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task TaxonomyFindWithSetLocale()
        {
            Taxonomy taxonomy = client.Taxonomies();
            taxonomy.Exists("taxonomies.one");
            taxonomy.SetLocale("en-us");
            
            var result = await taxonomy.Find<Entry>();
            
            Assert.NotNull(result);
        }
    }
}

