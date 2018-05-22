using System;
using System.IO;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;

namespace Contentstack.Core
{
    /// <summary>
    /// Contains all Contentstack API classes and functions.
    /// </summary>
    public class ContentstackClient
    {
        #region Internal Constructor
        internal ContentstackClient()
        {

        }
        #endregion 

        #region Public Functions
        /// <summary>
        /// Authenticates the stack api key of your stack. 
        /// This must be called before your stack uses Built.io Contentstack sdk.
        /// </summary>
        /// <param name="stackApiKey">stack api Key of your application on Contentstack.</param>
        /// <param name="accessToken">access token</param>
        /// <param name="environment">environment name</param>
        /// <returns>Current instance of Stack, this will be useful for a chaining calls.</returns>
        /// <example>
        /// <code>
        ///     //&quot;blt5d4sample2633b&quot; is a dummy Stack API key
        ///     //&quot;blt6d0240b5sample254090d&quot; is dummy access token.
        ///     Stack stack = ContentstackClient(&quot;blt5d4sample2633b&quot;, &quot;blt6d0240b5sample254090d&quot;, &quot;stag&quot;);
        /// </code>
        /// </example>
        public static Stack Stack(String stackApiKey, String accessToken, String environment)
        {
            if (!string.IsNullOrEmpty(stackApiKey))
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    if (!string.IsNullOrEmpty(environment))
                    {
                        Config config = new Config();
                        config.Environment = (environment);

                        return InitializeStack(stackApiKey, accessToken, config);

                    }
                    else
                    {
                        throw new Exception(StackConstants.ErrorMessage_Stack_Environment_IsNull);
                    }
                }
                else
                {
                    throw new Exception(StackConstants.ErrorMessage_Stack_AccessToken_IsNull);
                }
            }
            else
            {
                throw new Exception(StackConstants.ErrorMessage_StackApiKey);
            }
        }

        #endregion

        #region Private Functions
        private static Stack InitializeStack(String stackApiKey, String accessToken, Config config)
        {
            Stack stack = new Stack(stackApiKey.Trim());
            stack.SetHeader("api_key", stackApiKey);
            stack.SetHeader("access_token", accessToken);
            stack.SetConfig(config);

            var queryCacheFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ContentstackCache");
            if (!Directory.Exists(queryCacheFile))
                Directory.CreateDirectory(queryCacheFile);
            ContentstackConstants.Instance.CacheFolderName = queryCacheFile;

            //try
            //{

            //    //cache folder
            //    File queryCacheFile = context.getDir("ContentstackCache", 0);
            //    CSAppConstants.cacheFolderName = queryCacheFile.getPath();

            //    clearCache(context);
            //}
            //catch (Exception e)
            //{
            //    CSAppUtils.showLog(TAG, "-------------------stack-Contentstack-" + e.toString());
            //}

            return stack;
        }

        #endregion
    }
}


