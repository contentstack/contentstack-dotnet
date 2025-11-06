using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Tests.Mocks
{
    /// <summary>
    /// Mock HTTP handler for testing - intercepts WebRequest calls
    /// Note: This is a simplified version for the Delivery SDK which uses HttpWebRequest
    /// For actual mocking, we'll use test plugins or direct object creation
    /// </summary>
    public class MockHttpHandler
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

        public string GetMockResponse(string url = null)
        {
            if (url != null && _mockResponses.ContainsKey(url))
            {
                return _mockResponses[url];
            }
            return _mockResponse;
        }
    }

    /// <summary>
    /// Test plugin that intercepts HTTP responses and returns mock data
    /// </summary>
    public class MockResponsePlugin : IContentstackPlugin
    {
        private readonly string _mockResponse;
        private readonly Dictionary<string, string> _mockResponses;

        public MockResponsePlugin(string mockResponse)
        {
            _mockResponse = mockResponse;
            _mockResponses = new Dictionary<string, string>();
        }

        public MockResponsePlugin(Dictionary<string, string> mockResponses)
        {
            _mockResponse = null;
            _mockResponses = mockResponses ?? new Dictionary<string, string>();
        }

        public async Task<HttpWebRequest> OnRequest(Contentstack.Core.ContentstackClient stack, HttpWebRequest request)
        {
            // Just pass through the request
            return await Task.FromResult(request);
        }

        public async Task<string> OnResponse(Contentstack.Core.ContentstackClient stack, HttpWebRequest request, HttpWebResponse response, string responseString)
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
}


