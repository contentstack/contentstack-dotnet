using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    internal static class WebRequestAsyncExtensions
    {
        public static Task<Stream> GetRequestStreamAsync(this WebRequest request)
        {
            return Task.Factory.FromAsync<Stream>(
                request.BeginGetRequestStream, request.EndGetRequestStream, null);
        }

        public static Task<WebResponse> GetResponseAsync(this WebRequest request)
        {
            return Task.Factory.FromAsync<WebResponse>(
                request.BeginGetResponse, request.EndGetResponse, null);
        }
    }
}
