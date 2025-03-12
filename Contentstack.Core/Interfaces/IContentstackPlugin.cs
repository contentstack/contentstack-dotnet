using System.Net;
using System.Threading.Tasks;
namespace Contentstack.Core.Interfaces
{
    public interface IContentstackPlugin
    {
        Task<HttpWebRequest> OnRequest(ContentstackClient stack, HttpWebRequest request);
        Task<string> OnResponse(ContentstackClient stack, HttpWebRequest request, HttpWebResponse response, string responseString);
    }
}
