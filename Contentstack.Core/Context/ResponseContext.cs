using System;
using Contentstack.Core.Interfaces;

namespace Contentstack.Core.Context
{
    internal class ResponseContext : IResponseContext
    {
        public IResponse httpResponse { get; set; }
    }
}

