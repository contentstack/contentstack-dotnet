using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    internal class HTTPRequestHandler
    {
        Dictionary<string, object> cacheObject = new Dictionary<string, object>();
        public async Task<string> ProcessRequest(string Url, Dictionary<string, object> Headers, Dictionary<string, object> BodyJson, string FileName = null) {

            var uri = new Uri(Url);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            
            request.ContentType = "application/json";

            if (Headers != default(IDictionary<string, string>)) {
                foreach (var header in Headers) {
                    try {
                        request.Headers[header.Key] = header.Value.ToString();
                    } catch { }
                }
            }
            cacheObject.Add("url", Url);
            cacheObject.Add("headers", Headers);
            cacheObject.Add("params", BodyJson);

            var serializedresult = JsonConvert.SerializeObject(BodyJson);
            //Temporary fix for OR query. Unexpected ",{}" char comes after serialization 
            //BodyJson looks proper with only expected keys.
            serializedresult = serializedresult.Replace(",{}", ""); 
            byte[] requestBody = Encoding.UTF8.GetBytes(serializedresult);
            try {

                using (var postStream = await request.GetRequestStreamAsync()) {
                    await postStream.WriteAsync(requestBody, 0, requestBody.Length);
                }

                var response = (HttpWebResponse)await request.GetResponseAsync();
                if (response != null) {
                    var reader = new StreamReader(response.GetResponseStream());

                    string responseString = await reader.ReadToEndAsync();
                    cacheObject.Add("responseJson", responseString);
                    if (!string.IsNullOrEmpty(FileName))
                        CreateFileIntoCacheDir(cacheObject, FileName);
                    return responseString;
                } else {
                    return null;
                }
            } catch (Exception we) {
                throw we;
                //if (we.Response != null)
                //{
                //    var reader = new StreamReader(we.Response.GetResponseStream());
                //    string responseString = reader.ReadToEnd();
                //    Console.WriteLine(responseString);
                //    //ErrorCallback(we);
                //    return responseString;
                //}
                //else
                //{
                //    Console.WriteLine(we.Message);
                //    //ErrorCallback(we);
                //   // return we.Message;
                //}
            }

        }

        protected void CreateFileIntoCacheDir(Dictionary<string, object> jsonObject, string fileName)
        {
            try
            {
                Dictionary<string, object> jsonObj = new Dictionary<string, object>();
                Dictionary<string, object> mainJsonObj = new Dictionary<string, object>();
                Dictionary<string, object> headerJson = new Dictionary<string, object>();

                //jsonObj = paramsJSON;

                mainJsonObj.Add("url", jsonObject["url"].ToString());
                mainJsonObj.Add("timestamp", DateTime.UtcNow.Ticks);
                mainJsonObj.Add("params", jsonObject["params"]);
                mainJsonObj.Add("response", jsonObject["responseJson"]);

                foreach (var header in (Dictionary<string, object>)jsonObject["headers"])
                {
                    headerJson.Add(header.Key, header.Value);
                }
                mainJsonObj.Add("header", headerJson);

                // File cacheFile = new File(cacheFileName);

                if (File.Exists(fileName)) //cacheFileName
                {
                    File.Delete(fileName); //cacheFileName
                }
                StreamWriter file = new StreamWriter(fileName);
                file.Write(JsonConvert.SerializeObject(mainJsonObj));
                file.Flush();
                file.Close();
            }
            catch (Exception e)
            {
                // TODO: Handle exception
            }

        }

    }
}
