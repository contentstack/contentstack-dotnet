using System;
using System.Threading;
using Contentstack.Core.Context;

namespace Contentstack.Core.Handler
{
    public abstract partial class RetryPolicy
    {
        public bool RetryOnError { get; set; }
        public int RetryLimit { get; set; }


        public bool Retry(IExecutionContext executionContext, Exception exception)
        {

            bool canRetry = !RetryLimitExceeded(executionContext) && CanRetry(executionContext);

            if (canRetry && RetryForException(executionContext, exception))
            {
                return true;
            }

            return false;
        }

        protected abstract bool RetryForException(IExecutionContext excutionContext, Exception exception);

        protected abstract bool CanRetry(IExecutionContext excutionContext);

        protected abstract bool RetryLimitExceeded(IExecutionContext excutionContext);
        internal abstract void WaitBeforeRetry(IExecutionContext executionContext);
    }
}

