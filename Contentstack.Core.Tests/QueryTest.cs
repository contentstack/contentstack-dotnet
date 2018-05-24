using System;
using Xunit;
using Contentstack.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Contentstack.Core.Tests
{

    public class QueryTest
    {

        Stack Stack = StackConfig.GetStack();
        private String numbersContentType = "numbers_content_type";
        String source = "source";


        [Fact]
        public async Task FetchAll()
        {
            Query query = Stack.ContentType(source).Query();
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else if (result.Result != null && result.Result.Count() == 6)
            {
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    IsTrue = data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null;
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
            Query query = Stack.ContentType(numbersContentType).Query();
            query.GreaterThan("num_field", 11);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                if (result.Result != null && result.Result.Count() == 1)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {
                        var output = data.Object["num_field"].ToString();
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = (Convert.ToInt32(data.Object["num_field"]) > 11) && (data.Object.ContainsKey("publish_details"));
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
            Query query = Stack.ContentType(source).Query();
            query.GreaterThan("date", "2018-05-04");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                if (result.Result != null && result.Result.Count() == 2)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = DateTime.Compare(DateTime.Parse(Convert.ToString(data.Object["date"])), DateTime.Parse("2018-05-04")) > 0 && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(numbersContentType).Query();
            query.GreaterThanOrEqualTo("num_field", 11);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    var ouput = data.Object["num_field"].ToString();
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                        IsTrue = Convert.ToInt32(data.Object["num_field"]) >= 11 && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.GreaterThanOrEqualTo("date", "2018-05-04");
            var result = await query.Find();
            if (result.Result != null && result.Result.Count() == 3)
            {
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null) ;
                    DateTime dateToCompareWith = DateTime.Parse("2018-05-04");
                    DateTime dateToCompare = DateTime.Parse(Convert.ToString(data.Object["date"]));
                    IsTrue = (DateTime.Compare(dateToCompare, dateToCompareWith) == 0 || DateTime.Compare(dateToCompare, dateToCompareWith) > 0) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(numbersContentType).Query();
            query.LessThan("num_field", 11);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    var ouput = data.Object["num_field"].ToString();
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                        IsTrue = Convert.ToInt32(data.Object["num_field"]) < 11 && data.Object.ContainsKey("publish_details");
                    if (!IsTrue)
                        break;

                }

                Assert.True(IsTrue);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task LessThanForDate()
        {

            Query query = Stack.ContentType(source).Query();
            query.LessThan("date", "2018-05-04");
            var result = await query.Find();
            if (result.Result != null && result.Result.Count() == 2)
            {
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null) ;
                    DateTime dateToCompareWith = DateTime.Parse("2018-05-04");
                    DateTime dateToCompare = DateTime.Parse(Convert.ToString(data.Object["date"]));
                    IsTrue = DateTime.Compare(dateToCompare, dateToCompareWith) < 0 && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(numbersContentType).Query();
            query.LessThanOrEqualTo("num_field", 11);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    var ouput = data.Object["num_field"].ToString();
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                        IsTrue = Convert.ToInt32(data.Object["num_field"]) <= 11 && data.Object.ContainsKey("publish_details");
                    if (!IsTrue)
                        break;
                }

                Assert.True(IsTrue);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task LessThanOrEqualToForDate()
        {
            Query query = Stack.ContentType(source).Query();
            query.LessThanOrEqualTo("date", "2018-05-04");
            var result = await query.Find();
            if (result.Result != null && result.Result.Count() == 3)
            {
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null) ;
                    DateTime dateToCompareWith = DateTime.Parse("2018-05-04");
                    DateTime dateToCompare = DateTime.Parse(Convert.ToString(data.Object["date"]));
                    IsTrue = (DateTime.Compare(dateToCompare, dateToCompareWith) == 0 || DateTime.Compare(dateToCompare, dateToCompareWith) < 0) && data.Object.ContainsKey("publish_details");
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
            ContentType contentTypeObj = Stack.ContentType(source);
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

            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                if(result.Result.Count() == 1){
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {
                        //IsTrue = Convert.ToInt32(data.Object["price"]) == 786 && Convert.ToString(data.Object["title"]).Equals("laptop", StringComparison.InvariantCultureIgnoreCase);
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToString(data.Object["title"]) == "source1" && Convert.ToBoolean(data.Object["boolean"]) == true && data.Object.ContainsKey("publish_details");
                        if (!IsTrue)
                            break;
                    }
                    Assert.True(IsTrue);
                } else {
                    Assert.False(true, "Doesn't match the expected count.");
                }

            }
        }

        //Not working

        [Fact]
        public async Task Or()
        {
            ContentType contentTypeObj = Stack.ContentType(source);
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

            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                if(result.Result.Count()==2) {
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {
                        //IsTrue = Convert.ToInt32(data.Object["price"]) == 786 || Convert.ToInt32(data.Object["price"]).Equals(89);
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = (Convert.ToInt32(data.Object["number"]) > 10 || Convert.ToBoolean(data.Object["boolean"]) == false) && data.Object.ContainsKey("publish_details");
                        if (!IsTrue)
                            break;

                    }
                    Assert.True(IsTrue);
                } else {
                    Assert.False(true, "Doesn't match the expected count.");
                }

            }
        }

        /**
         * Test equals for text field
         */
        [Fact]
        public async Task WhereForText()
        {
            Query query = Stack.ContentType(source).Query();
            query.Where("title", "source1");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result.Count() == 1)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToString(data.Object["title"]).Equals("source1", StringComparison.InvariantCultureIgnoreCase) && data.Object.ContainsKey("publish_details");
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
         * Test equals for date field
         */
        [Fact]
        public async Task WhereForDate()
        {
            Query query = Stack.ContentType(source).Query();
            query.Where("date", "2018-05-04");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result.Count() == 1)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToString(data.Object["date"]).Equals("2018-05-04", StringComparison.InvariantCultureIgnoreCase) && data.Object.ContainsKey("publish_details");
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
         * Test equals for number field
         */
        [Fact]
        public async Task WhereForNumber()
        {
            Query query = Stack.ContentType(source).Query();
            query.Where("number", 12);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result != null && result.Result.Count() == 1)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToString(data.Object["number"]).Equals(11) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.Where("boolean", true);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result != null && result.Result.Count() == 4)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToBoolean(data.Object["boolean"]).Equals(true) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.NotEqualTo("boolean", true);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count().Equals(2))
                {
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToBoolean(data.Object["boolean"]).Equals(false) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.NotEqualTo("title", "source");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result != null && result.Result.Count() == 5)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = !data.Object["title"].Equals("source") && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.NotEqualTo("number", 12);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result != null && result.Result.Count() == 5)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = !data.Object["number"].Equals(12) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.NotEqualTo("date", "2018-05-04");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                if (result.Result != null && result.Result.Count() == 5)
                {
                    foreach (var data in result.Result)
                    {
                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = !data.Object["date"].Equals("2018-05-04") && data.Object.ContainsKey("publish_details");
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
        public async Task ContainedInForText()
        {
            Query query = Stack.ContentType(source).Query();
            query.ContainedIn("title", new object[] { "source1", "source2" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                if (result.Result != null && result.Result.Count() == 2)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToString(data.Object["title"]).Equals("source1") || Convert.ToString(data.Object["title"]).Equals("source2") && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.ContainedIn("number", new object[] { 12, 3 });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count() == 2)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = Convert.ToInt16(data.Object["number"]).Equals(12) || Convert.ToInt16(data.Object["number"]).Equals(3) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.ContainedIn("date", new object[] { "2018-05-04" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count() == 1)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = (DateTime.Compare(DateTime.Parse(Convert.ToString(data.Object["date"])), DateTime.Parse("2018-05-04")) == 0);
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
            Query query = Stack.ContentType(source).Query();
            query.ContainedIn("group.name", new object[] { "First", "third" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count() == 2)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                        {
                            Dictionary<string, object> grp = (Dictionary<string, object>)data.Object["group"];
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
            Query query = Stack.ContentType(source).Query();
            query.ContainedIn("modular_blocks.test1.single_line", new object[] { "Rohit", "Rahul" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count() == 2)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                        {
                            object[] modularBlocks = (object[])data.Object["modular_blocks"];
                            List<object> lstReference = modularBlocks.ToList();

                            foreach (var block in lstReference)
                            {
                                Dictionary<string, object> blockDictionary = (Dictionary<string, object>)block;
                                if (blockDictionary.ContainsKey("test1"))
                                {
                                    Dictionary<string, object> blockFiledDictionary = (Dictionary<string, object>)blockDictionary["test1"];
                                    String singleLine = Convert.ToString(blockFiledDictionary["single_line"]);
                                    IsTrue = singleLine.Equals("Rohit") || singleLine.Equals("Rahul");
                                    Assert.True(IsTrue);
                                }

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
            Query query = Stack.ContentType(source).Query();
            query.NotContainedIn("title", new object[] { "source1", "source2" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                        IsTrue = !(Convert.ToString(data.Object["title"]).Equals("source1")) || !(Convert.ToString(data.Object["title"]).Equals("source2")) && data.Object.ContainsKey("publish_details");
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task NotContainedInForNumber()
        {
            Query query = Stack.ContentType(source).Query();
            query.NotContainedIn("number", new object[] { 12, 3 });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count() == 4)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = !Convert.ToInt16(data.Object["number"]).Equals(12) || !Convert.ToInt16(data.Object["number"]).Equals(3) && data.Object.ContainsKey("publish_details");
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
            Query query = Stack.ContentType(source).Query();
            query.NotContainedIn("date", new object[] { "2018-05-04" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                if (result.Result != null && result.Result.Count() == 5)
                {
                    bool IsTrue = false;

                    foreach (var data in result.Result)
                    {

                        if (data.Object.ContainsKey("publish_details") && data.Object["publish_details"] != null)
                            IsTrue = data.Object["date"] == null || Convert.ToString(data.Object["date"]) == "" || DateTime.Compare(DateTime.Parse(Convert.ToString(data.Object["date"])), DateTime.Parse("2018-05-04")) != 0 ;
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
            Query query = Stack.ContentType(source).Query();
            query.Exists("number");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    IsTrue = data.Object.ContainsKey("number");
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task NotExists()
        {
            Query query = Stack.ContentType(source).Query();
            query.NotExists("name");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    IsTrue = (!data.Object.ContainsKey("name"));
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Ascending()
        {
            Query query = Stack.ContentType(source).Query();
            //query.NotEqualTo("number", "");
            query.Ascending("number");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                for (int i = 0; i < result.Result.Count(); i++)
                {
                    if (i < result.Result.Count() && i + 1 != result.Result.Count())
                    {
                        if (!string.IsNullOrEmpty(result.Result[i].Object["number"].ToString())
                            && !string.IsNullOrEmpty(result.Result[i + 1].Object["number"].ToString()))
                        {
                            IsTrue = Convert.ToInt32(result.Result[i].Object["number"]) <= Convert.ToInt32(result.Result[i + 1].Object["number"]);
                        }
                    }
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }

        }

        [Fact]
        public async Task Descending()
        {
            Query query = Stack.ContentType(source).Query();
            query.Exists("number");
            query.Descending("number");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                for (int i = 0; i < result.Result.Count(); i++)
                {
                    if (i < result.Result.Count() && i + 1 != result.Result.Count())
                    {
                        if (!string.IsNullOrEmpty(result.Result[i].Object["number"].ToString()) && !string.IsNullOrEmpty(result.Result[i + 1].Object["number"].ToString()))
                        {
                            IsTrue = Convert.ToInt32(result.Result[i].Object["number"]) >= Convert.ToInt32(result.Result[i + 1].Object["number"]);
                        }
                    }
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Skip()
        {
            Query skipQuery = Stack.ContentType(source).Query();
            skipQuery.Skip(2);
            skipQuery.IncludeCount();
            var skipResult = await skipQuery.Find();
            if (skipResult == null && skipResult.Result.Count() == 0 && skipQuery.TotalCount == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                IsTrue = skipQuery.TotalCount - 2 <= skipResult.Result.Count();
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Limit()
        {
            Query query = Stack.ContentType(source).Query();
            query.Limit(3);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                bool IsTrue = false;
                IsTrue = result.Result.Count() <= 3;
                Assert.True(IsTrue);
            }
        }


        [Fact]
        public async Task IncludeReference()
        {
            ContentType contenttype = Stack.ContentType(source);
            Query query = contenttype.Query();
            query.IncludeReference("reference");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Object.Count > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                if (result.Result != null && result.Result.Count() == 6)
                {
                    bool IsTrue = false;
                    foreach (var data in result.Result)
                    {
                        object[] refDetails = (object[])data.Object["reference"];
                        List<object> lstReference = refDetails.ToList();

                        if (lstReference.Count > 0)
                        {
                            IsTrue = lstReference.All(a => a is Dictionary<string, object>);
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
        public async Task IncludeCount()
        {
            Query query = Stack.ContentType(source).Query();
            query.IncludeCount();
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                Assert.True(result.TotalCount == result.Result.Count());
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
            }
        }

        [Fact]
        public async Task Only()
        {
            Query query = Stack.ContentType(source).Query();
            query.Only(new string[] { "title", "number" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");
                List<string> uidKeys = new List<string>() { "title", "number", "uid" };
                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    //IsTrue = data.Object.Keys.Count == 3 && data.Object.Keys.ToList().Contains(a=>  ui);
                    IsTrue = data.Object.Keys.All(p => uidKeys.Contains(p));
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Except()
        {
            Query query = Stack.ContentType(source).Query();
            query.Except(new string[] { "title", "number" });
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.Exec is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                List<string> uidKeys = new List<string>() { "title", "number" };
                bool IsTrue = false;
                foreach (var data in result.Result)
                {

                    IsTrue = data.Object.Keys.All(p => !uidKeys.Contains(p));
                    if (!IsTrue)
                        break;
                }
                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task FindOne()
        {
            Query query = Stack.ContentType(source).Query();
            //query.Limit(2);
            var result = await query.FindOne();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;

                IsTrue = result.Result.Count().Equals(1);

                Assert.True(IsTrue);
            }
        }

        [Fact]
        public async Task Regex()
        {
            Query query = Stack.ContentType(source).Query();
            query.Regex("title", "^source");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("title"))
                    {
                        IsTrue = data.Object["title"].ToString().StartsWith("source");
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
            Query query = Stack.ContentType(source).Query();
            query.Regex("title", "^s", "i");
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("title"))
                    {
                        IsTrue = data.Object["title"].ToString().StartsWith("s", StringComparison.InvariantCultureIgnoreCase);
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
            Query query = Stack.ContentType(source).Query();
            String[] tags = { "tag1", "tag2" };
            query.WhereTags(tags);
            var result = await query.Find();
            if (result == null && result.Result.Count() == 0)
            {
                Assert.False(true, "Query.FindOne is not match with expected result.");
            }
            else
            {
                //Assert.True(result.Result.Count() > 0);
                //Assert.True(true, "BuiltObject.Fetch is pass successfully.");

                bool IsTrue = false;
                foreach (var data in result.Result)
                {
                    if (data.Object.ContainsKey("tags"))
                    {
                        object[] tagsArray = (object[])data.Object["tags"];
                        List<object> tagsList = tagsArray.ToList();
                        IsTrue = tagsList.Contains("tag1") || tagsList.Contains("tag2");
                        if (!IsTrue)
                            break;
                    }
                }
                Assert.True(IsTrue);
            }
        }


    }
}

