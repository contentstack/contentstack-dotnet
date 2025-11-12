using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Contentstack.Core.Tests.Mocks
{
    /// <summary>
    /// Helper class for creating mock Contentstack API responses
    /// </summary>
    public static class MockResponse
    {
        /// <summary>
        /// Creates a Contentstack response from a JSON file in the MockResponses directory
        /// </summary>
        /// <param name="fileName">Name of the JSON file (e.g., "entry_deleted.json")</param>
        /// <returns>JSON string response</returns>
        public static string CreateContentstackResponse(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            // Try to read from file system (relative to test execution directory)
            var baseDirectory = Path.GetDirectoryName(assembly.Location) ?? "";
            var filePath = Path.Combine(baseDirectory, "MockResponses", fileName);
            
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            
            // Fallback: try relative to source directory
            var sourcePath = Path.Combine(
                Path.GetDirectoryName(assembly.Location) ?? "",
                "..", "..", "..", "..",
                "Contentstack.Core.Tests",
                "MockResponses",
                fileName
            );
            
            if (File.Exists(sourcePath))
            {
                return File.ReadAllText(sourcePath);
            }
            
            // Try as embedded resource
            var resourceName = $"Contentstack.Core.Tests.MockResponses.{fileName}";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            
            throw new FileNotFoundException($"Mock response file not found: {fileName}. Checked: {filePath}, {sourcePath}, and embedded resource {resourceName}");
        }

        /// <summary>
        /// Creates a JObject from a mock response file
        /// </summary>
        /// <param name="fileName">Name of the JSON file</param>
        /// <returns>JObject parsed from the response</returns>
        public static JObject CreateContentstackResponseAsJObject(string fileName)
        {
            var jsonString = CreateContentstackResponse(fileName);
            return JObject.Parse(jsonString);
        }

        /// <summary>
        /// Creates a mock response from a JObject
        /// </summary>
        /// <param name="jObject">The JObject to serialize</param>
        /// <returns>JSON string response</returns>
        public static string CreateContentstackResponseFromJObject(JObject jObject)
        {
            return jObject.ToString();
        }
    }
}

