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
        ContentstackClient client = StackConfig.GetStack();

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
    }
}

