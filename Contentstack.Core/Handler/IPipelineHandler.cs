using Contentstack.Core.Context;

namespace Contentstack.Core.Handler
{
    /// <summary>
    /// Interface for a handler in a pipeline.
    /// </summary>
    public interface IPipelineHandler
    {

        /// <summary>
        /// The inner handler which is called after the current 
        /// handler completes it's processing.
        /// </summary>
        IPipelineHandler InnerHandler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executionContext"></param>

        void InvokeSync(IExecutionContext executionContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<T> InvokeAsync<T>(IExecutionContext executionContext);
    }
}