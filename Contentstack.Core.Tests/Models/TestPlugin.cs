using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests.Models
{
    public class TestPlugin: IContentstackPlugin
    {
        private ContentstackClient Client;
        private JObject injectData;
        
        private string resp = $"{{\"emails\":[{string.Join(",", new List<string>(){$"\"test\""})}]}}";
        public TestPlugin(ContentstackClient client)
        {
            Client = client;
            injectData = JsonConvert.DeserializeObject<JObject>(resp.Replace("\r\n", ""), Client.SerializerSettings);
        }
        
        public virtual async void OnRequest(ContentstackClient stack, HttpWebRequest request)
        {
            request.Headers["test-header"] = "new header";
            _ = await Client.AssetLibrary().FetchAll();
        }

        public virtual async Task<string> OnResponse(ContentstackClient stack, HttpWebRequest request, HttpWebResponse response, string responseString)
        {
            JObject data = JsonConvert.DeserializeObject<JObject>(responseString.Replace("\r\n", ""), Client.SerializerSettings);
            _ = await Client.AssetLibrary().FetchAll();

            updateLivePreviewContent(data);
            return JsonConvert.SerializeObject(data); ;
        }

        internal void updateLivePreviewContent(JObject response)
        {
            if (response.ContainsKey("uid"))
            {
                response.Merge(injectData, new JsonMergeSettings()
                {
                    MergeArrayHandling = MergeArrayHandling.Union
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
