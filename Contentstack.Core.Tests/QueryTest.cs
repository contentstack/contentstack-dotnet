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

namespace Contentstack.Core.Tests
{

    public class QueryTest
    {
        ContentstackClient client = StackConfig.GetStack();

        private String numbersContentType = "numbers_content_type";
        String source = "source";

        public double EPSILON { get; private set; }

        [Fact]
        public async Task FetchAllGetContentType()
        {
            Query query = client.ContentType(source).Query();
            query.SetLocale("en-us");
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
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");

            }
        }

        [Fact]
        public async Task FetchEntriesPublishFallback()
        {
            List<string> list = new List<string>();
            list.Add("en-us");
            list.Add("ja-jp");
            ContentstackCollection<Entry> entries = await client.ContentType(source).Query()
                .SetLocale("ja-jp")
                .IncludeFallback()
                .Find<Entry>();
            ;
            Assert.True(entries.Items.Count() > 0);
            foreach (Entry entry in entries)
            {
                Assert.Contains((string)(entry.Get("publish_details") as JObject).GetValue("locale"), list);
            }
        }

        [Fact]
        public async Task FetchEntriesPublishWithoutFallback()
        {
            List<string> list = new List<string>();
            list.Add("ja-jp");
            ContentstackCollection<Entry> entries = await client.ContentType(source).Query()
                .SetLocale("ja-jp")
                .Find<Entry>();
            ;
            Assert.True(entries.Items.Count() > 0);
            foreach (Entry entry in entries)
            {
                Assert.Contains((string)(entry.Get("publish_details") as JObject).GetValue("locale"), list);
            }
        }

        [Fact]
        public async Task FetchAllCount()
        {
            Query query = client.ContentType(source).Query();
            query.SetLocale("en-us");
            var result = await query.Count();
            if (result == null)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {

                Assert.Equal(7, result.GetValue("entries"));
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");

            }
        }

        [Fact]
        public async Task FetchAll()
        {
            Query query = client.ContentType(source).Query();
            query.SetLocale("en-us");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (result != null)
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = data.Title != null;
                    if (!IsTrue)
                    {
                        break;
                    }
                }
                Assert.True(IsTrue);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
            else
            {
                Assert.False(true, "Result doesn't mathced the count.");

            }
        }

        [Fact]
        public async Task GreaterThanForNumber()
        {
            Query query = client.ContentType(numbersContentType).Query();
            query.GreaterThan("num_field", 11);
            var result = await query.Find<NumberContentType>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                if (result.Items  != null)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Items)
                    {
                        IsTrue = data.num_field > 11;
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue, "result is greater than 11");
                }
                else
                {
                    Assert.False(true, "Doesn't match the expected count.");

                }
            }
        }

        [Fact]
        public async Task GreaterThanForDate()
        {
            Query query = client.ContentType(source).Query();
            query.GreaterThan("date", "2018-05-04");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                if (result.Items != null)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Items)
                    {
                        IsTrue = DateTime.Compare(DateTime.Parse(Convert.ToString(data.Date)), DateTime.Parse("2018-05-04")) > 0;
                        if (!IsTrue)
                            break;

                    }

                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't match the expected count.");

                }
            }
        }


        [Fact]
        public async Task GreaterThanOrEqualToForNumber()
        {
            Query query = client.ContentType(numbersContentType).Query();
            query.GreaterThanOrEqualTo("num_field", 11);
            var result = await query.Find<NumberContentType>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = Convert.ToInt32(data.num_field) >= 11;
                    if (!IsTrue)
                        break;

                }

                Assert.True(IsTrue);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task GreaterThanOrEqualToForDate()
        {
            Query query = client.ContentType(source).Query();
            query.GreaterThanOrEqualTo("date", "2018-05-04");
            var result = await query.Find<SourceModel>();
            if (result.Items != null)
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    DateTime dateToCompareWith = DateTime.Parse("2018-05-04");
                    DateTime dateToCompare = DateTime.Parse(Convert.ToString(data.Date));
                    IsTrue = (DateTime.Compare(dateToCompare, dateToCompareWith) == 0 || DateTime.Compare(dateToCompare, dateToCompareWith) > 0);
                    if (!IsTrue)
                        break;

                }

                Assert.True(IsTrue);
            }
            else
            {
                Assert.False(true, "Doesn't match the expected count.");

            }
        }

        [Fact]
        public async Task LessThanForNumber()
        {
            Query query = client.ContentType(numbersContentType).Query();
            query.LessThan("num_field", 11);
            var result = await query.Find<NumberContentType>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = Convert.ToInt32(data.num_field) < 11;
                    if (!IsTrue)
                        break;

                }

                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task LessThanForDate()
        {

            Query query = client.ContentType(source).Query();
            query.LessThan("date", "2018-05-04");
            var result = await query.Find<SourceModel>();
            if (result.Items != null)
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    DateTime dateToCompareWith = DateTime.Parse("2018-05-04");
                    DateTime dateToCompare = DateTime.Parse(Convert.ToString(data.Date));
                    IsTrue = DateTime.Compare(dateToCompare, dateToCompareWith) < 0;
                    if (!IsTrue)
                        break;

                }

                Assert.True(IsTrue);
            }
            else
            {
                Assert.False(true, "Doesn't match the expected count.");

            }
        }

        [Fact]
        public async Task LessThanOrEqualToForNumber()
        {
            Query query = client.ContentType(numbersContentType).Query();
            query.LessThanOrEqualTo("num_field", 11);
            var result = await query.Find<NumberContentType>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                        IsTrue = Convert.ToInt32(data.num_field) <= 11;
                    if (!IsTrue)
                        break;
                }

                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task LessThanOrEqualToForDate()
        {
            Query query = client.ContentType(source).Query();
            query.LessThanOrEqualTo("date", "2018-05-04");
            var result = await query.Find<SourceModel>();
            if (result.Items != null)
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    DateTime dateToCompareWith = DateTime.Parse("2018-05-04");
                    DateTime dateToCompare = DateTime.Parse(Convert.ToString(data.Date));
                    IsTrue = (DateTime.Compare(dateToCompare, dateToCompareWith) == 0 || DateTime.Compare(dateToCompare, dateToCompareWith) < 0);
                    if (!IsTrue)
                        break;

                }

                Assert.True(IsTrue);
            }
            else
            {
                Assert.False(true, "Doesn't match the expected count.");

            }
        }

        [Fact]
        public async Task And()
        {
            ContentType contentTypeObj = client.ContentType(source);
            Query query = contentTypeObj.Query();


            Query query1 = contentTypeObj.Query();
            //query1.Where("price", 786);
            query1.Where("title", "source1");

            Query query2 = contentTypeObj.Query();
            //query2.Where("title", "laptop");
            query2.Where("boolean", true);

            List<Query> array = new List<Query>();
            array.Add(query1);
            array.Add(query2);

            query.And(array);

            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = Convert.ToString(data.Title) == "source1";
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
               

            }
        }


        [Fact]
        public async Task Or()
        {
            ContentType contentTypeObj = client.ContentType(source);
            Query query = contentTypeObj.Query();

            Query query1 = contentTypeObj.Query();
            //query1.Where("price", 786);
            query1.GreaterThan("number", 10);

            Query query2 = contentTypeObj.Query();
            //query2.Where("price", 89);
            query2.Where("boolean", false);

            List<Query> array = new List<Query>();
            array.Add(query1);
            array.Add(query2);

            query.Or(array);

            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = (data.Number > 10 || data.Boolean == false);
                    if (!IsTrue)
                        break;

                }
                Assert.True(IsTrue);
           

            }
        }

        /**
         * Test equals for text field
         */
        [Fact]
        public async Task WhereForText()
        {
            Query query = client.ContentType(source).Query();
            query.Where("title", "source1");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;

                foreach (var data in result.Items)
                {
                    IsTrue = Convert.ToString(data.Title).Equals("source1", StringComparison.InvariantCultureIgnoreCase);
                    if (!IsTrue)
                        break;

                }
                Assert.True(IsTrue);
            }
        }

        /**
         * Test equals for date field
         */
        [Fact]
        public async Task WhereForDate()
        {
            Query query = client.ContentType(source).Query();
            query.Where("date", "2018-05-04");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
               
                foreach (var data in result.Items)
                {
                    IsTrue = data.Date.Equals("2018-05-04", StringComparison.InvariantCultureIgnoreCase);
                    if (!IsTrue)
                        break;

                }
                Assert.True(IsTrue);
              }
        }

        /**
         * Test equals for number field
         */
        [Fact]
        public async Task WhereForNumber()
        {
            Query query = client.ContentType(source).Query();
            query.Where("number", 12);
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Items != null)
                {
                    foreach (var data in result.Items)
                    {
                        IsTrue = data.Number.Equals(11);
                        if (!IsTrue)
                            break;

                        Assert.True(IsTrue);
                    }
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }

            }
        }

        /**
         * Test equals for boolean field
         */
        [Fact]
        public async Task WhereForBoolen()
        {
            Query query = client.ContentType(source).Query();
            query.Where("boolean", true);
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Items != null)
                {
                    foreach (var data in result.Items)
                    {
                            IsTrue = data.Boolean.Equals(true);
                        if (!IsTrue)
                            break;

                        Assert.True(IsTrue);
                    }
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }

            }
        }


        /**
         * Test not equals for boolean field
         */
        [Fact]
        public async Task NotEqualToForBoolean()
        {
            Query query = client.ContentType(source).Query();
            query.NotEqualTo("boolean", true);
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Items != null)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Items)
                    {
                            IsTrue = data.Boolean.Equals(false);
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");

                }

            }
        }

        /**
         * Test not equals for text field
         */
        [Fact]
        public async Task NotEqualToForText()
        {
            Query query = client.ContentType(source).Query();
            query.NotEqualTo("title", "source");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Items != null)
                {
                    foreach (var data in result.Items)
                    {
                        IsTrue = !data.Title.Equals("source");
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }

            }
        }

        /**
         * Test not equals for number field
         */
        [Fact]
        public async Task NotEqualToForNumber()
        {
            Query query = client.ContentType(source).Query();
            query.NotEqualTo("number", 12);
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Items != null)
                {
                    foreach (var data in result.Items)
                    {
                        IsTrue = !data.Number.Equals(12);
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }

            }
        }

        /**
         * Test not equals for date field
         */
        [Fact]
        public async Task NotEqualToForDate()
        {
            Query query = client.ContentType(source).Query();
            query.NotEqualTo("date", "2018-05-04");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Items != null)
                {
                    foreach (var data in result.Items)
                    {
                        if (data.Date != null)
                        {
                            IsTrue = !data.Date.Equals("2018-05-04");
                            if (!IsTrue)
                                break;
                        }
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }

            }
        }

        [Fact]
        public async Task ContainedInForText()
        {
            Query query = client.ContentType(source).Query();
            query.ContainedIn("title", new object[] { "source1", "source2" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
               if (result.Items != null)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Items)
                    {

                        IsTrue = data.Title.Equals("source1") || data.Title.Equals("source2");;
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task ContainedInForNumber()
        {
            Query query = client.ContentType(source).Query();
            query.ContainedIn("number", new object[] { 12, 3 });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Items != null)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Items)
                    {

                        IsTrue = data.Number == 12 || data.Number == 3;
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task ContainedInForDate()
        {
            Query query = client.ContentType(source).Query();
            query.ContainedIn("date", new object[] { "2018-05-04" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Items != null)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Items)
                    {

                        IsTrue = (DateTime.Compare(DateTime.Parse(data.Date), DateTime.Parse("2018-05-04")) == 0);
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task ContainedInForGroup()
        {
            Query query = client.ContentType(source).Query();
            query.ContainedIn("group.name", new object[] { "First", "third" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                if (result.Items != null)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Items)
                    {

                        {
                            Dictionary<string, object> grp = (Dictionary<string, object>)data.Group;
                            foreach (var item in grp)
                            {
                                if (item.Key.Equals("name"))
                                {
                                    IsTrue = Convert.ToString(item.Value).Equals("First") || Convert.ToString(item.Value).Equals("third");
                                }
                            }
                        }
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }


        [Fact]
        public async Task ContainedInForModularBlock()
        {
            Query query = client.ContentType(source).Query();
            query.ContainedIn("modular_blocks.test1.single_line", new object[] { "Rohit", "Rahul" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Items != null)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Items)
                    {
                        List<Dictionary<string, object>> lstReference = data.Modular_blocks;

                        foreach (var block in lstReference)
                        {
                            Dictionary<string, object> blockDictionary = (Dictionary<string, object>)block;
                            if (blockDictionary.ContainsKey("test1"))
                            {
                                JObject blockFiledDictionary = (JObject)blockDictionary["test1"];
                                String singleLine = Convert.ToString(blockFiledDictionary["single_line"]);
                                IsTrue = singleLine.Equals("Rohit") || singleLine.Equals("Rahul");
                                Assert.True(IsTrue);
                            }

                        }

                        if (!IsTrue)
                            break;
                    }
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task NotContainedInForText()
        {
            Query query = client.ContentType(source).Query();
            query.NotContainedIn("title", new object[] { "source1", "source2" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    IsTrue = !(data.Title.Equals("source1")) || !(data.Title.Equals("source2"));
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task NotContainedInForNumber()
        {
            Query query = client.ContentType(source).Query();
            query.NotContainedIn("number", new object[] { 12, 3 });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Items != null)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Items)
                    {

                        IsTrue = (data.Number != (12)) || !(data.Number == (3));
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task NotContainedInForDate()
        {
            Query query = client.ContentType(source).Query();
            query.NotContainedIn("date", new object[] { "2018-05-04" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Items != null)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Items)
                    {

                        IsTrue = data.Date == null || data.Date == "" || DateTime.Compare(DateTime.Parse(data.Date), DateTime.Parse("2018-05-04")) != 0;
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task Exists()
        {
            Query query = client.ContentType(source).Query();
            query.Exists("number");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    if (data.Number != null)
                    {
                        IsTrue = data.Number > EPSILON;
                        if (!IsTrue)
                            break;
                    }
                }
                Assert.True(IsTrue);
            }
        }

        //[Fact]
        //public async Task NotExists()
        //{
        //    Query query = client.ContentType(source).Query();
        //    query.NotExists("reference");
        //    var result = await query.Find<SourceModel>();
        //    if (result == null && result.items.Count() == 0)
        //    {
        //        Assert.False(true, "Query.Exec is not match with expected result.");
        //    }
        //    else
        //    {
        //        //Assert.True(result.Result.Count() > 0);
        //        //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

        //        bool IsTrue = false;
        //        foreach (var data in result.items)
        //        {
        //            IsTrue = Math.Abs(data.Number) < EPSILON;
        //            if (!IsTrue)
        //                break;
        //        }
        //        Assert.True(IsTrue);
        //    }
        //}

        [Fact]
        public async Task Ascending()
        {
            Query query = client.ContentType(source).Query();
            //query.NotEqualTo("number", "");
            query.Exists("number");
            query.Ascending("number");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                Double number = -1;
                foreach (var data in result.Items)
                {
                    if (data.Number != null)
                    {
                        IsTrue = (data.Number >= number);
                        if (!IsTrue)
                            break;
                        number = data.Number ?? number;
                    }
                }
                Assert.True(IsTrue);
            }

        }

        [Fact]
        public async Task Descending()
        {
            Query query = client.ContentType(source).Query();
            query.Exists("number");
            query.Descending("number");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                Double number = Double.MaxValue;
                foreach (var data in result.Items)
                {
                    if (data.Number != null)
                    {
                        IsTrue = (data.Number <= number);
                        if (!IsTrue)
                            break;
                        number = data.Number ?? number;
                    }
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Skip()
        {
            Query skipQuery = client.ContentType(source).Query();
            skipQuery.Skip(2);
            skipQuery.IncludeCount();
            var skipResult = await skipQuery.Find<SourceModel>();
            if (skipResult == null && skipResult.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                bool IsTrue = false;
                IsTrue = skipResult.Count - 2 <= skipResult.Items.Count();
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Limit()
        {
            Query query = client.ContentType(source).Query();
            query.Limit(3);
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                IsTrue = result.Items.Count() <= 3;
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task IncludeEmbeddedItems()
        {
            ContentType contenttype = client.ContentType(source);
            Query query = contenttype.Query();
            query.includeEmbeddedItems();
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                if (result.Items != null)
                {
                    Assert.True(true);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }

        [Fact]
        public async Task IncludeReference()
        {
            ContentType contenttype = client.ContentType(source);
            Query query = contenttype.Query();
            query.IncludeReference("reference");
            var result = await query.Find<SourceModelIncludeRef>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Object.Count > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                if (result.Items != null)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Items)
                    {
                        List<Entry> lstReference = data.Reference;
                        if (lstReference.Count > 0)
                        {
                            IsTrue = lstReference.All(a => a is Entry);
                            if (!IsTrue)
                                break;
                        }
                    
                    }
                    Assert.True(IsTrue);
                }
                else
                {
                    Assert.False(true, "Doesn't mached the expected count.");
                }
            }
        }


        [Fact]
        public async Task IncludeCount()
        {
            Query query = client.ContentType(source).Query();
            query.IncludeCount();
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(result.Count == result.Items.Count());
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task Only()
        {
            Query query = client.ContentType(source).Query();
            query.Only(new string[] { "title", "number" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                List<string> uidKeys = new List<string>() { "title", "number", "uid" };
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    //IsTrue = data.Object.Keys.Count == 3 && data.Object.Keys.ToList().Contains(a=>  ui);
                    IsTrue = data.Title != null;
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Except()
        {
            Query query = client.ContentType(source).Query();
            query.Except(new string[] { "title", "number" });
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                List<string> uidKeys = new List<string>() { "title", "number" };
                bool IsTrue = false;
                foreach (var data in result.Items)
                {

                    IsTrue = data.Title == null && data.Number == null;
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task FindOne()
        {
            Query query = client.ContentType(source).Query();
            //query.Limit(2);
            var result = await query.FindOne<Entry>();
            if (result == null)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = true; ;

                //IsTrue = result.Count().Equals(1);

                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Regex()
        {
            Query query = client.ContentType(source).Query();
            query.Regex("title", "^source");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    if (data.Title != null)
                    {
                        IsTrue = data.Title.StartsWith("source", StringComparison.Ordinal);
                        if (!IsTrue)
                            break;
                    }
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task RegexWithModifiers()
        {
            Query query = client.ContentType(source).Query();
            query.Regex("title", "^s", "i");
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    if (data.Title != null)
                    {
                        IsTrue = data.Title.StartsWith("s", StringComparison.InvariantCultureIgnoreCase);
                        if (!IsTrue)
                            break;
                    }
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task WhereTags()
        {
            Query query = client.ContentType(source).Query();
            String[] tags = { "tag1", "tag2" };
            query.WhereTags(tags);
            var result = await query.Find<SourceModel>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    if (data.Tags != null)
                    {
                        object[] tagsArray = (object[])data.Tags;
                        List<object> tagsList = tagsArray.ToList();
                        IsTrue = tagsList.Contains("tag1") || tagsList.Contains("tag2");
                        if (!IsTrue)
                            break;
                    }
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task ReferenceIn()
        {
            ContentType contentTypeObj = client.ContentType(source);
            Query query = contentTypeObj.Query();
            query.IncludeReference("reference");
            Query referencequery = contentTypeObj.Query();
            referencequery.Where("title", "ref-1 test3");

            query.ReferenceIn("reference", referencequery);

            var result = await query.Find<SourceModelIncludeRef>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    foreach (var entry in data.Reference)
                    {
                        IsTrue = (entry.Title == "ref-1 test3");
                        if (!IsTrue)
                            break;
                    }

                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task ReferenceNotIn()
        {
            ContentType contentTypeObj = client.ContentType(source);
            Query query = contentTypeObj.Query();
            query.IncludeReference("reference");
            Query referencequery = contentTypeObj.Query();
            referencequery.Where("title", "ref-1 test3");

            query.ReferenceNotIn("reference", referencequery);

            var result = await query.Find<SourceModelIncludeRef>();
            if (result == null && result.Items.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                bool IsTrue = false;
                foreach (var data in result.Items)
                {
                    foreach (var entry in data.Reference)
                    {
                        IsTrue = (entry.Title != "ref-1 test3");
                        if (!IsTrue)
                            break;
                    }

                }
                Assert.True(IsTrue);
            }
        }
    }
}

