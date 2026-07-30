using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Shared request-building, header-merging, and error-handling logic for the CDA
    /// Taxonomy/Term/TermQuery classes. Every taxonomy-related HTTP call goes through
    /// <see cref="ExecuteRequest"/> so header merging and error parsing behave identically
    /// across <c>Taxonomy</c>, <c>Term</c>, and <c>TermQuery</c>.
    /// </summary>
    internal static class TaxonomyRequestHelper
    {
        /// <summary>
        /// Merges a call-local header set with the stack's headers, local values taking precedence.
        /// Passing null or empty for <paramref name="localHeader"/> returns the stack headers unchanged.
        /// </summary>
        internal static Dictionary<string, object> GetHeader(Dictionary<string, object> stackHeaders, Dictionary<string, object> localHeader)
        {
            if (localHeader != null && localHeader.Count > 0)
            {
                if (stackHeaders != null && stackHeaders.Count > 0)
                {
                    Dictionary<string, object> classHeaders = new Dictionary<string, object>();
                    foreach (var entry in localHeader)
                        classHeaders.Add(entry.Key, entry.Value);

                    foreach (var entry in stackHeaders)
                        if (!classHeaders.ContainsKey(entry.Key))
                            classHeaders.Add(entry.Key, entry.Value);

                    return classHeaders;
                }
                return localHeader;
            }
            return stackHeaders;
        }

        /// <summary>
        /// Builds and sends a taxonomy/term GET request against the CDA and returns the raw response body.
        /// </summary>
        /// <param name="stack">The owning client, providing config, headers, and the HTTP handler.</param>
        /// <param name="url">The fully-qualified request URL.</param>
        /// <param name="urlQueries">Query parameters accumulated by the calling class's modifiers (locale, depth, skip, ...).</param>
        /// <param name="localHeader">Optional call-local headers, merged with precedence over the stack's headers.</param>
        internal static async Task<string> ExecuteRequest(
            ContentstackClient stack,
            string url,
            Dictionary<string, object> urlQueries,
            Dictionary<string, object> localHeader = null)
        {
            var headerAll = GetHeader(stack._LocalHeaders, localHeader);

            var mainJson = new Dictionary<string, object>();
            if (stack.Config?.Environment != null)
                mainJson["environment"] = stack.Config.Environment;

            if (urlQueries != null)
                foreach (var kvp in urlQueries)
                    mainJson[kvp.Key] = kvp.Value;

            var handler = new HttpRequestHandler(stack);
            return await handler.ProcessRequest(
                url, headerAll, mainJson,
                Branch: stack.Config.Branch,
                timeout: stack.Config.Timeout,
                proxy: stack.Config.Proxy
            );
        }

        /// <summary>
        /// Parses a failed taxonomy/term request into a <see cref="ContentstackException"/>,
        /// extracting <c>error_code</c>/<c>error_message</c>/<c>errors</c> from the response body when present.
        /// </summary>
        internal static ContentstackException GetContentstackError(Exception ex)
        {
            Int32 errorCode = 0;
            string errorMessage;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            Dictionary<string, object> errors = null;

            try
            {
                System.Net.WebException webEx = ex as System.Net.WebException;

                if (webEx != null && webEx.Response != null)
                {
                    using (var exResp = webEx.Response)
                    {
                        var stream = exResp.GetResponseStream();
                        if (stream != null)
                        {
                            using (stream)
                            using (var reader = new System.IO.StreamReader(stream))
                            {
                                errorMessage = reader.ReadToEnd();

                                if (!string.IsNullOrWhiteSpace(errorMessage))
                                {
                                    ApiErrorBodyParser.TryApply(errorMessage.Replace("\r\n", ""), ref errorCode, ref errorMessage, ref errors);
                                }

                                var response = exResp as HttpWebResponse;
                                if (response != null)
                                    statusCode = response.StatusCode;
                            }
                        }
                        else
                        {
                            errorMessage = webEx.Message;
                        }
                    }
                }
                else
                {
                    errorMessage = ex.Message;
                }
            }
            catch
            {
                errorMessage = ex.Message;
            }

            return new ContentstackException(errorMessage)
            {
                ErrorCode = errorCode,
                StatusCode = statusCode,
                Errors = errors
            };
        }
    }
}
