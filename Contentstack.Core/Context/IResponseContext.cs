using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Context
{
    public interface IResponseContext
    {
        IResponse httpResponse { get; set; }
    }
}