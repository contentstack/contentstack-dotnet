using System;
using System.Collections.Generic;
using System.Net;
using Contentstack.Core.Context;

namespace Contentstack.Core.Handler
{
    public partial class DefaultRetryPolicy : RetryPolicy
    {
        protected TimeSpan retryDelay { get; set; }

        protected ICollection<HttpStatusCode> statusCodesToRetryOn = new HashSet<HttpStatusCode>
        {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.BadGateway,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.RequestTimeout,
            (HttpStatusCode)429,
            HttpStatusCode.Unauthorized
        };

        internal DefaultRetryPolicy(int retryLimit, TimeSpan delay)
        {
            RetryLimit = retryLimit;
            retryDelay = delay;
        }

        protected override bool CanRetry(IExecutionContext executionContext)
        {
            return true;
        }

        protected override bool RetryForException(IExecutionContext executionContext, Exception exception)
        {
            //if (exception is Exceptions.ContentstackErrorException)
            //{
            //    var contentstackExecption = exception as Exceptions.ContentstackErrorException;

            //    if (statusCodesToRetryOn.Contains(contentstackExecption.StatusCode))
            //    {
            //        return true;
            //    }
            //}

            return false;

        }

        protected override bool RetryLimitExceeded(IExecutionContext executionContext)
        {
            return executionContext.RequestContext.Retries >= this.RetryLimit;

        }

        internal override void WaitBeforeRetry(IExecutionContext executionContext)
        {
            System.Threading.Tasks.Task.Delay(retryDelay.Milliseconds).Wait();
        }
    }
}

