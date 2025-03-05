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

        public async Task TaxonomyExists() 
        {
            // Description: Taxonomy Exists - Get Entries With Any Taxonomy Terms ($exists)
            Taxonomy query = client.Taxonomies();
            query.Exists("taxonomies.one");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.Fail("Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (Entry data in result.Items)
                {
                    IsTrue = data.Get("_content_type_uid") != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.Fail("Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task TaxonomyEqualAndBelow()
        {
            // Description: Taxonomy EqualAndBelow - Get Entries With Taxonomy Terms and Also Matching Its Children Term ($eq_below, level)
            Taxonomy query = client.Taxonomies();
            query.EqualAndBelow("taxonomies.one", "term_one");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.Fail("Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (Entry data in result.Items)
                {
                    IsTrue = data.Get("_content_type_uid") != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.Fail("Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task TaxonomyBelow()
        {
            // Description: Taxonomy Below - Get Entries With Taxonomy Terms Children\'s and Excluding the term itself ($below, level)
            Taxonomy query = client.Taxonomies();
            query.Below("taxonomies.one", "term_one");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.Fail("Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (Entry data in result.Items)
                {
                    IsTrue = data.Get("_content_type_uid") != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.Fail("Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task TaxonomyEqualAndAbove()
        {
            // Description: Taxonomy EqualAndAbove - Get Entries With Taxonomy Terms and Also Matching Its Parent Term ($eq_above, level)
            Taxonomy query = client.Taxonomies();
            query.EqualAndAbove("taxonomies.one", "term_one_child");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.Fail("Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (Entry data in result.Items)
                {
                    IsTrue = data.Get("_content_type_uid") != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.Fail("Result doesn't mathced the count.");
            }
        }

        [Fact]
        public async Task TaxonomyAbove()
        {
            // Description: Taxonomy Above - Get Entries With Taxonomy Terms Parent and Excluding the term itself ($above, level)
            Taxonomy query = client.Taxonomies();
            query = query.Above("taxonomies.one", "term_one_child");
            var result = await query.Find<Entry>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.Fail("Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = data.Get("_content_type_uid") != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
            }
            else
            {
                Assert.Fail("Result doesn't mathced the count.");
            }
        }

    }
}

