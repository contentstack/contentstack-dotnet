using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Mock HTTP handler for testing - matches Management SDK pattern
    /// Adapts plugin system to intercept HttpWebRequest responses
    /// </summary>
    public class MockHttpHandler : IContentstackPlugin
    {
        private readonly string _mockResponse;
        private readonly Dictionary<string, string> _mockResponses;

        public MockHttpHandler(string mockResponse)
        {
            _mockResponse = mockResponse;
            _mockResponses = new Dictionary<string, string>();
        }

        public MockHttpHandler(Dictionary<string, string> mockResponses)
        {
            _mockResponse = null;
            _mockResponses = mockResponses ?? new Dictionary<string, string>();
        }

        public MockHttpHandler(ContentstackResponse response)
        {
            _mockResponse = response?.OpenResponse();
            _mockResponses = new Dictionary<string, string>();
        }

        public async Task<HttpWebRequest> OnRequest(Contentstack.Core.ContentstackClient stack, HttpWebRequest request)
        {
            // Just pass through the request
            return await Task.FromResult(request);
        }

        public async Task<string> OnResponse(
            Contentstack.Core.ContentstackClient stack, 
            HttpWebRequest request, 
            HttpWebResponse response, 
            string responseString)
        {
            // Return mock response instead of actual response
            var url = request.RequestUri?.ToString() ?? "";
            
            if (_mockResponses.ContainsKey(url))
            {
                return await Task.FromResult(_mockResponses[url]);
            }
            
            if (!string.IsNullOrEmpty(_mockResponse))
            {
                return await Task.FromResult(_mockResponse);
            }
            
            // Fallback to original response
            return await Task.FromResult(responseString);
        }
    }

    /// <summary>
    /// Wrapper for mock response - matches Management SDK ContentstackResponse pattern
    /// </summary>
    public class ContentstackResponse
    {
        private readonly string _response;

        public ContentstackResponse(string response)
        {
            _response = response;
        }

        public string OpenResponse()
        {
            return _response;
        }

        public Newtonsoft.Json.Linq.JObject OpenJObjectResponse()
        {
            return Newtonsoft.Json.Linq.JObject.Parse(_response ?? "{}");
        }
    }
}

