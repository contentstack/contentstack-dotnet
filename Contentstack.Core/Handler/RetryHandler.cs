using System;
using System.Threading.Tasks;
using Contentstack.Core.Context;
using Contentstack.Core.Internals;

namespace Contentstack.Core.Handler
{
    public class RetryHandler : PipelineHandler
    {
        public RetryPolicy RetryPolicy { get; private set; }

        public RetryHandler(RetryPolicy retryPolicy)
        {
            this.RetryPolicy = retryPolicy;
        }
        public override async Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            var responseContext = executionContext.ResponseContext;
            bool shouldRetry = false;
            do
            {
                try
                {
                    var response = await base.InvokeAsync<T>(executionContext);
                    return response;
                }
                catch (Exception exception)
                {
                    shouldRetry = this.RetryPolicy.Retry(executionContext, exception);
                    if (!shouldRetry)
                    {
                        throw;
                    }
                    else
                    {
                        requestContext.Retries++;
                    }
                }

                this.RetryPolicy.WaitBeforeRetry(executionContext);

            } while (shouldRetry == true);
            throw new ContentstackException("No response was return nor exception was thrown");
        }

        public override void InvokeSync(IExecutionContext executionContext)
        {
            var requestContext = executionContext.RequestContext;
            bool shouldRetry = false;
            do
            {
                try
                {
                    base.InvokeSync(executionContext);
                    return;
                }
                catch (Exception exception)
                {
                    shouldRetry = this.RetryPolicy.Retry(executionContext, exception);
                    if (!shouldRetry)
                    {
                        throw;
                    }
                    else
                    {
                        requestContext.Retries++;
                    }

                }

                this.RetryPolicy.WaitBeforeRetry(executionContext);

            } while (shouldRetry == true);
        }
    }
}

