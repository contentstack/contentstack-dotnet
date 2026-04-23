using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Contentstack.Core.Internals;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Tests.Models
{
    public class TestPlugin : IContentstackPlugin
    {
        private ContentstackClient Client;
        private JsonObject injectData;

        private string resp = $"{{\"emails\":[{string.Join(",", new List<string>() { $"\"test\"" })}]}}";

        public TestPlugin(ContentstackClient client)
        {
            Client = client;
            injectData = JsonNode.Parse(resp.Replace("\r\n", "")).AsObject();
        }

        public virtual async Task<HttpWebRequest> OnRequest(ContentstackClient stack, HttpWebRequest request)
        {
            request.Headers["test-header"] = "new header";
            _ = await Client.AssetLibrary().FetchAll();
            return request;
        }

        public virtual async Task<string> OnResponse(ContentstackClient stack, HttpWebRequest request, HttpWebResponse response, string responseString)
        {
            JsonObject data = JsonNode.Parse(responseString.Replace("\r\n", "")).AsObject();
            _ = await Client.AssetLibrary().FetchAll();

            updateLivePreviewContent(data);
            return JsonSerializer.Serialize(data, Client.SerializerOptions);
        }

        internal void updateLivePreviewContent(JsonObject response)
        {
            if (response.ContainsKey("uid"))
            {
                JsonObjectMerge.UnionMergeInto(response, injectData);
                return;
            }

            foreach (var key in response.Select(p => p.Key).ToList())
            {
                var node = response[key];
                if (node is JsonArray arr)
                {
                    updateArray(arr);
                }
                else if (node is JsonObject jo)
                {
                    updateLivePreviewContent(jo);
                }
            }
        }

        internal void updateArray(JsonArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                var item = array[i];
                if (item is JsonArray childArr)
                {
                    updateArray(childArr);
                }
                else if (item is JsonObject jo)
                {
                    updateLivePreviewContent(jo);
                }
            }
        }
    }
}
