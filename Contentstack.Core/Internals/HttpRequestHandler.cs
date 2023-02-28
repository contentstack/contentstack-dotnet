using Contentstack.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        ContentstackClient client
        {
            get; set;
        }
        internal HttpRequestHandler(ContentstackClient contentstackClient)
        {
            client = contentstackClient;
        }
        public async Task<string> ProcessRequest(string Url, Dictionary<string, object> Headers, Dictionary<string, object> BodyJson, string FileName = null, string Branch = null, bool isLivePreview = false) {

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
            request.Headers["x-user-agent"]="contentstack-dotnet/2.10.0";
            if (Branch != null)
            {
                request.Headers["branch"] = Branch;
            }
            if (Headers != default(IDictionary<string, string>)) {
                foreach (var header in Headers) {
                    try {
                        request.Headers[header.Key] = header.Value.ToString();
                    } catch {
                        
                    }
                }
            }

            foreach (var plugin in client.Plugins)
            {
                request = await plugin.OnRequest(client, request);
            };

            var serializedresult = JsonConvert.SerializeObject(BodyJson);
            byte[] requestBody = Encoding.UTF8.GetBytes(serializedresult);
            StreamReader reader = null;
            HttpWebResponse response = null;

            try {
                response = (HttpWebResponse)await request.GetResponseAsync();
                if (response != null) {
                    reader = new StreamReader(response.GetResponseStream());

                    string responseString = await reader.ReadToEndAsync();
                    foreach (var plugin in client.Plugins)
                    {
                        responseString = await plugin.OnResponse(client, request, response, responseString);
                    }

                    if (isLivePreview == false && this.client.LivePreviewConfig.Enable == true)
                    {
                        JObject data = JsonConvert.DeserializeObject<JObject>(responseString.Replace("\r\n", ""), this.client.SerializerSettings);
                        updateLivePreviewContent(data);
                        responseString = JsonConvert.SerializeObject(data);
                    }
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

        internal void updateLivePreviewContent(JObject response)
        {
            if (response.ContainsKey("uid") && response["uid"].ToString() == this.client.LivePreviewConfig.EntryUID)
            {
                response.Merge(this.client.LivePreviewConfig.PreviewResponse, new JsonMergeSettings()
                {
                    MergeArrayHandling = MergeArrayHandling.Replace
                });
            }
            else
            {
                foreach (var content in response)
                {
                    if (content.Value.Type == JTokenType.Array)
                    {
                        updateArray((JArray)response[content.Key]);
                    }
                    else if (content.Value.Type == JTokenType.Object)
                    {
                        updateLivePreviewContent((JObject)response[content.Key]);
                    }
                }
            }
        }

        internal void updateArray(JArray array)
        {
            for (var i = 0; i < array.Count(); i++)
            {
                if (array[i].Type == JTokenType.Array)
                {
                    updateArray((JArray)array[i]);
                }
                else if (array[i].Type == JTokenType.Object)
                {
                    updateLivePreviewContent((JObject)array[i]);
                }
            }
        }
    }
}
