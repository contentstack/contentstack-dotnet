using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    internal class HTTPRequestHandler
    {
        public async Task<string> ProcessRequest(string Url, Dictionary<string, object> Headers, Dictionary<string, object> BodyJson, string FileName = null) {
            var uri = new Uri(Url);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.UserAgent = "DOTNET 1.0.0";

            if (Headers != default(IDictionary<string, string>)) {
                foreach (var header in Headers) {
                    try {
                        request.Headers[header.Key] = header.Value.ToString();
                    } catch (Exception e) {
                        
                    }
                }
            }
            Console.WriteLine(".." + request.ToString());

            var serializedresult = JsonConvert.SerializeObject(BodyJson);
            byte[] requestBody = Encoding.UTF8.GetBytes(serializedresult);
            StreamReader reader = null;
            HttpWebResponse response = null;

            try {
                //using (var postStream = await request.GetRequestStreamAsync()) {
                //    await postStream.WriteAsync(requestBody, 0, requestBody.Length);
                //}

                response = (HttpWebResponse)await request.GetResponseAsync();
                if (response != null) {
                    reader = new StreamReader(response.GetResponseStream());

                    string responseString = await reader.ReadToEndAsync();

                    return responseString;
                } else {
                    return null;
                }
            } catch (Exception we) {
                throw we;
            } finally {
                if (reader != null) {
                    reader.Close();
                    reader.Dispose();
                }
                if (response != null)
                {

                    response.Close();
                    response.Dispose();
                }
            }

        }
    }
}
