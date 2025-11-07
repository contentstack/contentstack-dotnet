using System;
using System.Reflection;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Utility class for handling SDK version information and user-agent string generation.
    /// </summary>
    internal static class VersionUtility
    {
        /// <summary>
        /// Gets the SDK version dynamically from the assembly.
        /// </summary>
        /// <returns>The SDK version string in format: contentstack-delivery-dotnet/Major.Minor.Patch</returns>
        public static string GetSdkVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;

                // Check if version is valid (not 0.0.0.0 which is default for unversioned assemblies)
                if (
                    version != null
                    && (version.Major > 0 || version.Minor > 0 || version.Build > 0)
                )
                {
                    return $"contentstack-delivery-dotnet/{version.Major}.{version.Minor}.{version.Build}";
                }

                // Try to get version from assembly file version as fallback
                var fileVersion = assembly
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()
                    ?.Version;
                if (!string.IsNullOrEmpty(fileVersion))
                {
                    // Parse file version and extract only Major.Minor.Build (first 3 parts)
                    var versionParts = fileVersion.Split('.');
                    if (versionParts.Length >= 3)
                    {
                        return $"contentstack-delivery-dotnet/{versionParts[0]}.{versionParts[1]}.{versionParts[2]}";
                    }
                    return $"contentstack-delivery-dotnet/{fileVersion}";
                }

                // Try to get version from assembly informational version
                var infoVersion = assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;
                if (!string.IsNullOrEmpty(infoVersion))
                {
                    // Extract semantic version (Major.Minor.Patch) from informational version
                    // Handle formats like "2.25.0", "2.25.0-beta.1", "2.25.0+abc123"
                    var semanticVersion = ExtractSemanticVersion(infoVersion);
                    if (!string.IsNullOrEmpty(semanticVersion))
                    {
                        return $"contentstack-delivery-dotnet/{semanticVersion}";
                    }
                    return $"contentstack-delivery-dotnet/{infoVersion}";
                }
            }
            catch
            {
                // Ignore exceptions and continue to fallback
            }

            // Final fallback - use a generic identifier that doesn't imply a specific version
            return "contentstack-delivery-dotnet/dev";
        }

        /// <summary>
        /// Extracts semantic version (Major.Minor.Patch) from informational version string.
        /// </summary>
        /// <param name="informationalVersion">The informational version string.</param>
        /// <returns>Semantic version in Major.Minor.Patch format.</returns>
        private static string ExtractSemanticVersion(string informationalVersion)
        {
            try
            {
                // Handle null or empty input
                if (string.IsNullOrWhiteSpace(informationalVersion))
                {
                    return null;
                }

                // Remove build metadata (everything after +)
                var versionWithoutMetadata = informationalVersion.Split('+')[0];

                // Handle case where version ends with + (e.g., "1.2.3+")
                if (string.IsNullOrWhiteSpace(versionWithoutMetadata))
                {
                    return null;
                }

                // Split by dots to get version parts
                var parts = versionWithoutMetadata.Split('.');

                // Ensure we have at least 3 parts (Major.Minor.Patch)
                if (parts.Length >= 3)
                {
                    // Validate that all parts are numeric or contain valid version identifiers
                    var major = parts[0].Trim();
                    var minor = parts[1].Trim();
                    var patch = parts[2].Trim();

                    // Check if we have valid version components
                    if (
                        !string.IsNullOrEmpty(major)
                        && !string.IsNullOrEmpty(minor)
                        && !string.IsNullOrEmpty(patch)
                    )
                    {
                        // Take only the first 3 parts (Major.Minor.Patch)
                        return $"{major}.{minor}.{patch}";
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
