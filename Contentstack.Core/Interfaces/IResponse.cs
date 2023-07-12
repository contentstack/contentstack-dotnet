using System;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Contentstack.Core.Interfaces
{
    /// <summary>
    /// Interface for a response.
    /// </summary>
    public interface IResponse
    {
        long ContentLength { get; }
        string ContentType { get; }
        HttpStatusCode StatusCode { get; }
        bool IsSuccessStatusCode { get; }
        string[] GetHeaderNames();
        bool IsHeaderPresent(string headerName);
        string GetHeaderValue(string headerName);

        string OpenResponse();

        JObject OpenJObjectResponse();

        TResponse OpenTResponse<TResponse>();
    }
}

