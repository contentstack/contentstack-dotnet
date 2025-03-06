using System;
using System.Collections.Generic;


namespace Contentstack.Core.Internals
{
    internal class StackOutput
    {
        private Int32 _TotalCount = 0;
        private string _Json = string.Empty;
        private string _Notice = string.Empty;
        private object _Output = default(object);
        private Dictionary<string, object> _ObjectAttributes = new Dictionary<string, object>();
        private object _Schema = default(object);
        //private object _Stack = default(object);
        //private object _ContentType = default(object);
        //private object _ContentTypes = default(object);
        private object _Object = default(object);
        private object _Objects = default(object);
        //private object _Asset = default(object);
        //private object _Assets = default(object);
        private object _Result = default(object);
        private object _ApplicationUser = default(object);
        private object _Tags = default(object);
        private string _Owner = string.Empty;
        private string _Uid = String.Empty;

    //    public string Json
    //    {
    //        get
    //        {
    //            return this._Json;
    //        }
    //    }
    //    public string Notice
    //    {
    //        get
    //        {
    //            return this._Notice;
    //        }
    //    }

    //    public object Output
    //    {
    //        get
    //        {
    //            return this._Output;
    //        }
    //    }

    //    public object Schema
    //    {
    //        get
    //        {
    //            return this._Schema;
    //        }
    //    }

    //    public object Result
    //    {
    //        get
    //        {
    //            return this._Result;
    //        }
    //    }



    //    public object ApplicationUser
    //    {
    //        get
    //        {
    //            return this._ApplicationUser;
    //        }
    //    }





    //    public object Object
    //    {
    //        get
    //        {
    //            return this._Object;
    //        }
    //    }

    //    public object Objects
    //    {
    //        get
    //        {
    //            return this._Objects;
    //        }
    //    }

    //    public string Owner
    //    {
    //        get
    //        {
    //            return this._Owner;
    //        }
    //    }

    //    public string Uid
    //    {
    //        get
    //        {
    //            return this._Uid;
    //        }
    //    }

    //    public Int32 TotalCount
    //    {
    //        get
    //        {
    //            return this._TotalCount;
    //        }
    //    }

    //    public object Tags
    //    {
    //        get
    //        {
    //            return this._Tags;
    //        }
    //    }
    //    private void SetOutput(string json)
    //    {
    //        try
    //        {
    //            this._Json = json;
    //            this._Output = default(object);
    //            //bool isOtherTypeOfJson = true;
    //            //Newtonsoft.Json.Linq.JObject data = Newtonsoft.Json.Linq.JObject.Parse(json.Replace("\r\n", ""));
    //            JObject data = JsonConvert.DeserializeObject<JObject>(json.Replace("\r\n", ""));//, ContentstackConvert.JsonSerializerSettings);
    //            //var tempData = data["result"];
    //            JContainer dataContainer = ((JContainer)data);

    //            /*
    //            if (tempData != null)
    //            {
    //                //string tempJson = tempData.ToString().Replace("\r\n", string.Empty).Replace("\n", string.Empty);
    //                //data = JsonConvert.DeserializeObject<JObject>(tempJson, BuiltConvert.JsonSerializerSettings);

    //                dataContainer = (JContainer)tempData;
    //            }
    //            */

    //            Dictionary<string, object> output = this.GetObjectDict(dataContainer);
    //            this._Output = output;

    //            if (output.ContainsKey("count"))
    //            {
    //                this._TotalCount = ContentstackConvert.ToInt32(output["count"]);
    //            }

    //            if (output.ContainsKey("schema"))
    //            {
    //                this._Schema = output["schema"];
    //            }

    //            if (output.ContainsKey("notice"))
    //            {
    //                this._Notice = ContentstackConvert.ToString(output["notice"]);
    //            }

    //            if (output.ContainsKey("uid"))
    //            {
    //                this._Uid = ContentstackConvert.ToString(output["uid"]);
    //            }

    //            if (output.ContainsKey("result"))
    //            {
    //                this._Result = output["result"];
    //            }




    //            if (output.ContainsKey("entry"))
    //            {
    //                this._Object = output["entry"];

    //                if (this._Object != null)
    //                {
    //                    Dictionary<string, object> temp = (Dictionary<string, object>)this._Object;

    //                    if (temp.ContainsKey("uid"))
    //                    {
    //                        this._Uid = ContentstackConvert.ToString(temp["uid"]);
    //                    }

    //                    if (temp.ContainsKey("_owner"))
    //                    {
    //                        //this._Owner = temp["_owner"];
    //                    }

    //                    if (temp.ContainsKey("tags"))
    //                    {
    //                        this._Tags = temp["tags"];
    //                    }
    //                }
    //            }

    //            if (output.ContainsKey("entries"))
    //            {
    //                if (output["entries"].GetType() == typeof(string))
    //                {
    //                    this._Objects = this.GetValue(ContentstackConvert.ToString(output["entries"]));
    //                }
    //                else
    //                {
    //                    this._Objects = output["entries"];
    //                    var tmp = this._Objects as object[];
    //                    //if (tmp != null)
    //                    //    this._Object = tmp.FirstOrDefault();
    //                }
    //            }

    //            if (output.ContainsKey("asset"))
    //            {
    //                if (output["asset"].GetType() == typeof(string))
    //                {
    //                    this._Object = output["asset"];
    //                }
    //                else
    //                {
    //                    this._Object = output["asset"];
    //                    //var tmp = this._Objects as object[];
    //                    //if (tmp != null)
    //                    //    this._Object = tmp.FirstOrDefault();
    //                }
    //            }
    //            if (output.ContainsKey("assets"))
    //            {
    //                if (output["assets"].GetType() == typeof(string))
    //                {
    //                    this._Objects = output["assets"];
    //                }
    //                else
    //                {
    //                    this._Objects = output["assets"];
    //                    //var tmp = this._Objects as object[];
    //                    //if (tmp != null)
    //                    //    this._Object = tmp.FirstOrDefault();
    //                }
    //            }

    //            if (output.ContainsKey("response"))
    //            {
    //                if (output["response"].GetType() == typeof(string))
    //                {
    //                    var temp = (Dictionary<string,object>)this.GetValue(ContentstackConvert.ToString(output["response"]));
    //                    this._Objects = temp["entries"];
    //                }
    //                else
    //                {
    //                    var tempResult = (Dictionary<string,object>)output["response"];
    //                    if (tempResult.ContainsKey("entry"))
    //                    {
    //                        this._Object = tempResult["entry"];

    //                        if (this._Object != null)
    //                        {
    //                            Dictionary<string, object> temp = (Dictionary<string, object>)this._Object;

    //                            if (temp.ContainsKey("uid"))
    //                            {
    //                                this._Uid = ContentstackConvert.ToString(temp["uid"]);
    //                            }

    //                            if (temp.ContainsKey("_owner"))
    //                            {
    //                                //this._Owner = temp["_owner"];
    //                            }

    //                            if (temp.ContainsKey("tags"))
    //                            {
    //                                this._Tags = temp["tags"];
    //                            }
    //                        }
    //                    }
    //                    if (tempResult.ContainsKey("entries"))
    //                    {
    //                        if (tempResult["entries"].GetType() == typeof(string))
    //                        {
    //                            this._Objects = tempResult["entries"];
    //                        }
    //                        else
    //                        {
    //                            this._Objects = tempResult["entries"];
    //                            //var tmp = this._Objects as object[];
    //                            //if (tmp != null)
    //                            //    this._Object = tmp.FirstOrDefault();
    //                        }
    //                    }

    //                    //if (tmp != null)
    //                    //    this._Object = tmp.FirstOrDefault();
    //                }
    //            }


    //        }
    //        catch (Exception ex)
    //        {
    //            throw new ContentstackError(ex);
    //        }
    //    }
    //    public Dictionary<string, object> GetObjectDict(JContainer container)
    //    {
    //        Dictionary<string, object> output = new Dictionary<string, object>();

    //        try
    //        {
    //            foreach (JToken token in container.Children())
    //            {
    //                if (token is JProperty)
    //                {
    //                    JProperty item = token as JProperty;

    //                    if (item.Value == null)
    //                    {
    //                        output.Add(item.Name, null);
    //                    }
    //                    else
    //                    {
    //                        string valueStr = string.Empty;

    //                        try
    //                        {
    //                            //valueStr = ((JProperty)item).Value.ToString();
    //                            //test
    //                            if (item.Value.Type == JTokenType.Date)
    //                            {
    //                                valueStr = JsonConvert.SerializeObject(((JProperty)item).Value, ContentstackConvert.JsonSerializerSettings);
    //                                valueStr = valueStr.Replace("\"", String.Empty);
    //                            }
    //                            else
    //                            {
    //                                valueStr = ((JProperty)item).Value.ToString();
    //                            }
    //                            //
    //                        }
    //                        catch
    //                        { }

    //                        try
    //                        {
    //                            if ((valueStr.StartsWith("{") && valueStr.EndsWith("}")) || (valueStr.StartsWith("[") && valueStr.EndsWith("]")))
    //                            {
    //                                if ((valueStr.StartsWith("{") && valueStr.EndsWith("}")))
    //                                {
    //                                    JContainer jContainer = (JContainer)JsonConvert.DeserializeObject(valueStr, ContentstackConvert.JsonSerializerSettings);
    //                                    output.Add(item.Name, this.GetObjectDict(jContainer));
    //                                }
    //                                else
    //                                {
    //                                    JArray itemArr = (JArray)JsonConvert.DeserializeObject(valueStr, ContentstackConvert.JsonSerializerSettings);
    //                                    List<object> objArr = new List<object>();

    //                                    for (int i = 0, length = itemArr.Count(); i < length; i++)
    //                                    {
    //                                        try
    //                                        {
    //                                            var tempItem = itemArr[i];

    //                                            if (tempItem.GetType() == typeof(JValue))
    //                                            {
    //                                                objArr.Add(this.GetValue(ContentstackConvert.ToString(tempItem)));
    //                                            }
    //                                            else
    //                                            {
    //                                                objArr.Add(this.GetObjectDict((JContainer)tempItem));
    //                                            }
    //                                        }
    //                                        catch
    //                                        { }
    //                                    }

    //                                    /*
    //                                    foreach (var tempItem in itemArr)
    //                                    {
    //                                        try
    //                                        {
    //                                            if (tempItem.GetType() == typeof(JValue))
    //                                            {
    //                                                objArr.Add(this.GetValue(BuiltConvert.ToString(tempItem)));
    //                                            }
    //                                            else
    //                                            {
    //                                                objArr.Add(this.GetObjectDict((JContainer)tempItem));
    //                                            }
    //                                        }
    //                                        catch
    //                                        { }
    //                                    }
    //                                    */

    //                                    output.Add(item.Name, objArr.ToArray());
    //                                }
    //                            }
    //                            else
    //                            {
    //                                //output[item.Name] = this.GetValue(valueStr);
    //                                //test
    //                                if (item.Value.Type == JTokenType.Integer)
    //                                {
    //                                    output[item.Name] = item.Value.Value<Int64>();
    //                                }
    //                                else if (item.Value.Type == JTokenType.Float)
    //                                {
    //                                    output[item.Name] = item.Value.Value<Double>();
    //                                }
    //                                else
    //                                {
    //                                    output[item.Name] = this.GetValue(valueStr);
    //                                }
    //                                //
    //                            }
    //                        }
    //                        catch
    //                        {
    //                            output[item.Name] = this.GetValue(valueStr);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch
    //        {
    //            output.Add("__built__io__static__value__", this.GetValue(container.ToString()));
    //        }

    //        return output;
    //    }

    //    public object GetValue(string value)
    //    {
    //        object obj = value;

    //        try
    //        {
    //            float tempFloat = 0;
    //            double tempDouble = 0;
    //            Int32 tempInt32 = 0;
    //            Int64 tempInt64 = 0;

    //            if (value.ToLower() == "true" || value.ToLower() == "false")
    //            {
    //                obj = Convert.ToBoolean(value);
    //            }
    //            else if (value == "[]")
    //            {
    //                obj = new object[0];
    //            }
    //            else if (value == "{}")
    //            {
    //                obj = null;
    //            }
    //            else if (Int32.TryParse(value, out tempInt32))
    //            {
    //                obj = value.ToString();
    //            }
    //            else if (Int64.TryParse(value, out tempInt64))
    //            {
    //                obj = value.ToString();
    //            }
    //            else if (float.TryParse(value, out tempFloat))
    //            {
    //                obj = value.ToString();
    //            }
    //            else if (Double.TryParse(value, out tempDouble))
    //            {
    //                obj = value.ToString();
    //            }
    //            else if (value.GetType() == typeof(string))
    //            {
    //                obj = value.ToString();
    //            }
    //        }
    //        catch
    //        { }

    //        return obj;
    //    }

    //    public StackOutput(string json)
    //    {


    //        if (json.Trim() != string.Empty)
    //        {
    //            this.SetOutput(json);
    //        }
    //    }
    }
}
