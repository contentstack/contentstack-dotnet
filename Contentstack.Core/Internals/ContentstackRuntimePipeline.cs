using System;
using System.Collections.Generic;
using Contentstack.Core.Context;
using Contentstack.Core.Handler;

namespace Contentstack.Core.Internals
{
    public partial class ContentstackRuntimePipeline : IDisposable
    {
        #region Private members

        bool _disposed;

        // The top-most handler in the pipeline.
        IPipelineHandler _handler;

        /// <summary>
        /// The top-most handler in the pipeline.
        /// </summary>
        public IPipelineHandler Handler
        {
            get { return _handler; }
        }
        #endregion

        public ContentstackRuntimePipeline(IPipelineHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _handler = handler;
            
        }

        public ContentstackRuntimePipeline(List<IPipelineHandler> handlers)
        {
            if (handlers == null || handlers.Count == 0)
                throw new ArgumentNullException("handler");

            

            foreach (IPipelineHandler handler in handlers)
            {
                AddHanlder(handler);
            }
           
        }

        public void AddHanlder(IPipelineHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            ThrowIfDisposed();

            var currentHanler = handler;
            while (currentHanler.InnerHandler != null)
            {
                currentHanler = currentHanler.InnerHandler;
            }

            if (_handler != null)
            {
                currentHanler.InnerHandler = _handler;
            }

            _handler = currentHanler;
        }

        public System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            ThrowIfDisposed();

            return _handler.InvokeAsync<T>(executionContext);
        }

        public IResponseContext InvokeSync(IExecutionContext executionContext)
        {
            ThrowIfDisposed();

            _handler.InvokeSync(executionContext);
            return executionContext.ResponseContext;
        }

        public void ReplaceHandler(IPipelineHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            // TODO to add Multiple Handlers
            _handler = handler;
        }

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                var handler = this.Handler;
                while (handler != null)
                {
                    var innerHandler = handler.InnerHandler;
                    var disposable = handler as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    handler = innerHandler;
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion
    }
}

