using System;
using System.Threading;

namespace Contentstack.Core.Context
{
    internal class ExecutionContext : IExecutionContext
    {
        public ExecutionContext(IRequestContext requestContext, IResponseContext responseContext)
        {
            this.RequestContext = requestContext;
            this.ResponseContext = responseContext;
        }

        public IResponseContext ResponseContext { get; }

        public IRequestContext RequestContext { get; }
    }
}

