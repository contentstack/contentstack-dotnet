using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        public async Task<string> ProcessRequest(string Url, Dictionary<string, object> Headers, Dictionary<string, object> BodyJson, string FileName = null, string Branch = null, bool isLivePreview = false, int timeout = 30000, WebProxy proxy = null)
        {

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
                    value = JsonSerializer.Serialize(kvp.Value);
                else
                    return String.Format("{0}={1}", kvp.Key, kvp.Value);

                return String.Format("{0}={1}", kvp.Key, value);

            }));

            var uri = new Uri(Url+"?"+queryParam);

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["x-user-agent"]="contentstack-delivery-dotnet/2.20.0";
            request.Timeout = timeout;

            if (proxy != null)
            {
                request.Proxy = proxy;
            }

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

            var serializedresult = JsonSerializer.Serialize(BodyJson);
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

        internal void updateLivePreviewContent(JsonObject response)
        {
            if (response.ContainsKey("uid") && response["uid"].ToString() == this.client.LivePreviewConfig.EntryUID)
            {
                foreach (var property in this.client.LivePreviewConfig.PreviewResponse.AsObject())
                {
                    response[property.Key] = property.Value;
                }
            }
            else
            {
                foreach (var content in response)
                {
                    if (content.Value is JsonArray)
                    {
                        updateArray((JsonArray)response[content.Key]);
                    }
                    else if (content.Value is JsonObject)
                    {
                        updateLivePreviewContent((JsonObject)response[content.Key]);
                    }
                }
            }
        }

        internal void updateArray(JsonArray array)
        {
            for (var i = 0; i < array.Count(); i++)
            {
                if (array[i] is JsonArray)
                {
                    updateArray((JsonArray)array[i]);
                }
                else if (array[i] is JsonObject)
                {
                    updateLivePreviewContent((JsonObject)array[i]);
                }
            }
        }
    }
}
