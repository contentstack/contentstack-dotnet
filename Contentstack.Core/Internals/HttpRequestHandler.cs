using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    internal class HttpRequestHandler
    {
        public async Task<string> ProcessRequest(string Url, Dictionary<string, object> Headers, Dictionary<string, object> BodyJson, string FileName = null) {

            String queryParam = String.Join("&", BodyJson.Select(kvp => {
                var value = "";
                if (kvp.Value is string[])
                {
                    string[] vals = (string[])kvp.Value;
                    value = String.Join("&", vals.Select(item =>
                    {
                        return String.Format("{0}={1}", kvp.Key, item);
                    }));
                    return value;
                }
                else if (kvp.Value is Dictionary<string, object>)
                    value = JsonConvert.SerializeObject(kvp.Value);
                else
                    return String.Format("{0}={1}", kvp.Key, kvp.Value);

                return String.Format("{0}={1}", kvp.Key, value);

            }));

            var uri = new Uri(Url+"?"+queryParam);

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["x-user-agent"]="DOTNET 1.1.0";

            if (Headers != default(IDictionary<string, string>)) {
                foreach (var header in Headers) {
                    try {
                        request.Headers[header.Key] = header.Value.ToString();
                    } catch {
                        
                    }
                }
            }

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
                    reader.Dispose();
                }
                if (response != null)
                {
                     response.Dispose();
                }
            }

        }
    }
}
