using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace Contentstack.Core.Unit.Tests.Mokes
{
    /// <summary>
    /// Utility class for test helpers - matches Management SDK pattern
    /// </summary>
    public static class Utilities
    {
        public static string GetResourceText(string resourceName)
        {
            var stream = GetResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Resource stream for '{resourceName}' is null");
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream GetResourceStream(string resourceName)
        {
            Assembly assembly = typeof(Utilities).Assembly;
            var resource = FindResourceName(resourceName);
            Stream stream = assembly.GetManifestResourceStream(resource);
            if (stream == null)
            {
                // Fallback: try to read from file system
                var baseDirectory = Path.GetDirectoryName(assembly.Location) ?? "";
                var filePath = Path.Combine(baseDirectory, "..", "..", "..", "Mokes", "Response", resourceName);
                if (File.Exists(filePath))
                {
                    return new FileStream(filePath, FileMode.Open, FileAccess.Read);
                }
                // Try alternative path (for test execution)
                var altPath = Path.Combine(baseDirectory, "Mokes", "Response", resourceName);
                if (File.Exists(altPath))
                {
                    return new FileStream(altPath, FileMode.Open, FileAccess.Read);
                }
                throw new FileNotFoundException($"Resource '{resourceName}' not found. Searched for: {resource}");
            }
            return stream;
        }

        public static string FindResourceName(string partialName)
        {
            return FindResourceName(s => s.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0).SingleOrDefault() 
                ?? $"Contentstack.Core.Unit.Tests.Mokes.Response.{partialName}";
        }

        public static IEnumerable<string> FindResourceName(Predicate<string> match)
        {
            Assembly assembly = typeof(Utilities).Assembly;
            var allResources = assembly.GetManifestResourceNames();
            foreach (var resource in allResources)
            {
                if (match(resource))
                    yield return resource;
            }
        }

        public static MemoryStream CreateStreamFromString(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        public static Stream CreateStreamFromString(string s, Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}

