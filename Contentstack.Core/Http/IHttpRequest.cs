using System;
using System.Collections.Generic;
using System.Net.Http;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Http
{
    public interface IHttpRequest : IDisposable
    {
        /// <summary>
        /// The HTTP method or verb.
        /// </summary>
        HttpMethod Method { get; set; }

        /// <summary>
        /// The request URI.
        /// </summary>
        Uri RequestUri { get; }

        /// <summary>
        /// Sets the headers on the request.
        /// </summary>
        /// <param name="headers">A dictionary of header names and values.</param>
        void SetRequestHeaders(IDictionary<string, string> headers);

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <returns></returns>
        HttpContent GetRequestContent();

        /// <summary>
        /// Returns the HTTP response.
        /// </summary>
        /// <returns></returns>
        IResponse GetResponse();

        /// <summary>
        ///  Returns the HTTP response.
        /// </summary>
        /// <returns></returns>
        System.Threading.Tasks.Task<IResponse> GetResponseAsync();


        /// <summary>
        /// Writes a byte array to the request body.
        /// </summary>
        /// <param name="content">The content stream to be written.</param>
        /// <param name="contentHeaders">HTTP content headers.</param>
        void WriteToRequestBody(HttpContent content, IDictionary<string, string> contentHeaders);
    }
}

