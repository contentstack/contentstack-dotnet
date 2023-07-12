using System;
using System.Threading.Tasks;
using Contentstack.Core.Context;

namespace Contentstack.Core.Handler
{
    public class PipelineHandler : IPipelineHandler
    {

        public IPipelineHandler InnerHandler { get; set; }

        public virtual Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            if (InnerHandler != null)
            {
                return InnerHandler.InvokeAsync<T>(executionContext);
            }
            throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
        }

        public virtual void InvokeSync(IExecutionContext executionContext)
        {
            if (this.InnerHandler != null)
            {
                InnerHandler.InvokeSync(executionContext);
                return;
            }
            throw new InvalidOperationException("Cannot invoke InnerHandler. InnerHandler is not set.");
        }
    }
}

