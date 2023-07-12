using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Http;
using Contentstack.Core.Configuration;
using Contentstack.Core.Http;
using Contentstack.Core.Queryable;

namespace Contentstack.Core.Interfaces
{
    /// <summary>
    /// Interface for a Contentstack service.
    /// </summary>
    public interface IContentstackService : IDisposable
    {
        /// <summary>
        /// Gets and sets a flag that indicates whether the request is sent as a query string instead of the request body.
        /// </summary>
        bool UseQueryString { get; set; }


        /// <summary>
        /// Management Tokens are tokens that provide you with read-write access to the content of your stack.
        /// </summary>
        string ManagementToken { get; set; }

        /// <summary>
        /// Returns a dictionary of the parameters included in this request.
        /// </summary>
        ParameterCollection Parameters { get; }

        /// <summary>
        /// Returns a dictionary of the headers included in this request.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets and Sets the resource path added on to the endpoint.
        /// </summary>
        string ResourcePath
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the QueryResource that should be appended to the resource path.
        /// </summary>
        IDictionary<string, string> QueryResources
        {
            get;
        }

        /// <summary>
        /// Gets and sets the type of http request to make, whether it should be POST,GET or DELETE
        /// </summary>
        string HttpMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the path resources that should be used within the resource path.
        /// This is used for services where path keys can contain '/'
        /// characters, making string-splitting of a resource path potentially 
        /// hazardous.
        /// </summary>
        IDictionary<string, string> PathResources
        {
            get;
        }

        /// <summary>
        /// Adds a new entry to the QueryResources collection for the request
        /// </summary>
        /// <param name="queryResources">The name of the QueryResource</param>
        /// <param name="value">Value of the entry</param>
        void AddQueryResource(string queryResource, string value);

        /// <summary>
        /// Adds a new entry to the PathResources collection for the request
        /// </summary>
        /// <param name="key">The name of the pathresource with potential greedy syntax: {key+}</param>
        /// <param name="value">Value of the entry</param>
        void AddPathResource(string key, string value);
        /// <summary>
        /// Gets and Sets the content for this request.
        /// </summary>
        HttpContent Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the header value from the request.
        /// </summary>
        string GetHeaderValue(string headerName);

        IHttpRequest CreateHttpRequest(HttpClient httpClient, ContentstackOptions config);
        void OnResponse(IResponse httpResponse, ContentstackOptions config);
        bool HasRequestBody();
    }
}

