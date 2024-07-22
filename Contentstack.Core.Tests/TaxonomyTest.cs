using System;
using Xunit;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Contentstack.Core.Tests.Models;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;

namespace Contentstack.Core.Tests
{

    public class TaxonomyTest
    {
        ContentstackClient client = StackConfig.GetStack();

        private String numbersContentType = "numbers_content_type";
        String source = "source";

        public double EPSILON { get; private set; }

        [Fact]

        public async Task GetEntriesWithAnyTaxonomyTerms() 
        {
            Taxonomy query = client.Taxonomies();
            query.Exists("taxonomies.one");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (Entry data in result.Items)
                {
                    IsTrue = data.GetContentType() != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task GetEntriesWithTaxonomyTermsandAlsoMatchingItsChildrenTerm()
        {
            Taxonomy query = client.Taxonomies();
            query.EqualAndBelow("taxonomies.one", "term_one");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (Entry data in result.Items)
                {
                    IsTrue = data.GetContentType() != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");
            }
        }

    }
}

