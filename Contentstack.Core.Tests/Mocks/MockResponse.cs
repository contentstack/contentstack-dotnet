using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Nodes;

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
        /// Creates a <see cref="JsonObject"/> from a mock response file
        /// </summary>
        /// <param name="fileName">Name of the JSON file</param>
        /// <returns>JSON object parsed from the response</returns>
        public static JsonObject CreateContentstackResponseAsJsonObject(string fileName)
        {
            var jsonString = CreateContentstackResponse(fileName);
            return JsonNode.Parse(jsonString).AsObject();
        }

        /// <summary>
        /// Creates a mock response from a <see cref="JsonObject"/>
        /// </summary>
        /// <param name="jsonObject">The JSON object to serialize</param>
        /// <returns>JSON string response</returns>
        public static string CreateContentstackResponseFromJsonObject(JsonObject jsonObject)
        {
            return jsonObject.ToJsonString();
        }
    }
}

