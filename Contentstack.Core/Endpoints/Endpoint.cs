#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Contentstack.Core.Endpoints
{
    /// <summary>
    /// Resolves Contentstack service URLs for any supported region.
    ///
    /// All public methods are static — no instantiation required.
    ///
    /// Example:
    /// <code>
    /// // Full URL
    /// string url = Endpoint.GetContentstackEndpoint("na", "contentDelivery");
    /// // → "https://cdn.contentstack.io"
    ///
    /// // Host only (omit https://) — useful for SDK host configuration
    /// string host = Endpoint.GetContentstackEndpoint("eu", "contentDelivery", omitHttps: true);
    /// // → "eu-cdn.contentstack.com"
    ///
    /// // All endpoints for a region
    /// Dictionary&lt;string, string&gt; all = Endpoint.GetContentstackEndpoint("azure-na");
    /// // → { "contentDelivery": "...", "contentManagement": "...", ... }
    /// </code>
    /// </summary>
    public static class Endpoint
    {
        private const string RegionsUrl = "https://artifacts.contentstack.com/regions.json";

        // Module-level cache — loaded once per process, shared across all calls.
        private static JsonElement[]? _regionsData;
        private static readonly object _cacheLock = new object();
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        /// <summary>
        /// Resolves a single Contentstack service endpoint URL for the given region.
        /// </summary>
        /// <param name="region">
        /// Region ID or alias (case-insensitive). Examples: "na", "us", "eu", "AWS-NA", "azure_eu", "gcp-na".
        /// </param>
        /// <param name="service">
        /// Service key. Valid keys include: "contentDelivery", "contentManagement", "auth",
        /// "graphqlDelivery", "preview", "images", "assets", "automate", "launch",
        /// "developerHub", "brandKit", "genAI", "personalizeManagement",
        /// "personalizeEdge", "composableStudio", "assetManagement".
        /// </param>
        /// <param name="omitHttps">
        /// When true, strips the https:// scheme from the returned URL.
        /// Useful when passing the host to an SDK that constructs its own URLs.
        /// </param>
        /// <returns>The endpoint URL string for the specified service.</returns>
        /// <exception cref="ArgumentException">If region is empty or whitespace.</exception>
        /// <exception cref="KeyNotFoundException">If region or service is not found.</exception>
        /// <exception cref="InvalidOperationException">If the registry cannot be read or is corrupt.</exception>
        public static string GetContentstackEndpoint(string region, string service, bool omitHttps = false)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Empty region provided. Please put valid region.", nameof(region));

            var regions = LoadRegions();
            string normalized = region.Trim().ToLowerInvariant();
            var regionEl = FindRegion(regions, normalized);

            if (regionEl == null)
                throw new KeyNotFoundException($"Invalid region: {region}");

            var endpoints = regionEl.Value.GetProperty("endpoints");
            if (!endpoints.TryGetProperty(service, out var urlEl))
            {
                string regionId = regionEl.Value.GetProperty("id").GetString()!;
                throw new KeyNotFoundException($"Service \"{service}\" not found for region \"{regionId}\"");
            }

            string url = urlEl.GetString()!;
            return omitHttps ? StripHttps(url) : url;
        }

        /// <summary>
        /// Returns all service endpoint URLs for the given region.
        /// </summary>
        /// <param name="region">Region ID or alias (case-insensitive).</param>
        /// <param name="omitHttps">When true, strips the https:// scheme from all returned URLs.</param>
        /// <returns>Dictionary mapping service keys to endpoint URLs.</returns>
        /// <exception cref="ArgumentException">If region is empty or whitespace.</exception>
        /// <exception cref="KeyNotFoundException">If region is not found.</exception>
        /// <exception cref="InvalidOperationException">If the registry cannot be read or is corrupt.</exception>
        public static Dictionary<string, string> GetContentstackEndpoint(string region, bool omitHttps = false)
        {
            if (string.IsNullOrWhiteSpace(region))
                throw new ArgumentException("Empty region provided. Please put valid region.", nameof(region));

            var regions = LoadRegions();
            string normalized = region.Trim().ToLowerInvariant();
            var regionEl = FindRegion(regions, normalized);

            if (regionEl == null)
                throw new KeyNotFoundException($"Invalid region: {region}");

            var result = new Dictionary<string, string>();
            var endpoints = regionEl.Value.GetProperty("endpoints");
            foreach (var ep in endpoints.EnumerateObject())
            {
                string url = ep.Value.GetString()!;
                result[ep.Name] = omitHttps ? StripHttps(url) : url;
            }
            return result;
        }

        /// <summary>
        /// Clears the in-memory region cache. Intended for testing only — forces the
        /// next call to re-read regions.json from disk or re-download from CDN.
        /// </summary>
        public static void ResetCache()
        {
            lock (_cacheLock)
            {
                _regionsData = null;
            }
        }

        // ------------------------------------------------------------------
        // Internal helpers
        // ------------------------------------------------------------------

        /// <summary>
        /// Load and cache the regions registry.
        ///
        /// Resolution order:
        ///   1. In-memory cache    — zero I/O after the first call in a process
        ///   2. Local file on disk — Assets/regions.json next to the DLL
        ///                          (written by DownloadAndSave or refresh-region.cs)
        ///   3. CDN download       — fetches from artifacts.contentstack.com,
        ///                          writes to disk for future calls (silent on failure)
        /// </summary>
        private static JsonElement[] LoadRegions()
        {
            lock (_cacheLock)
            {
                if (_regionsData != null)
                    return _regionsData;

                string localFile = GetLocalFilePath();

                // Step 2 — local file on disk
                string? json = ReadLocalFile(localFile);

                // Step 3 — CDN download, writes to disk so next startup skips this step
                if (json == null)
                    json = DownloadAndSave(localFile);

                if (json == null)
                    throw new InvalidOperationException(
                        "contentstack_csharp: regions.json not found and could not be downloaded. " +
                        "Run 'dotnet run Scripts/refresh-region.cs' and ensure network access.");

                JsonDocument doc;
                try
                {
                    doc = JsonDocument.Parse(json);
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException(
                        "contentstack_csharp: regions.json is corrupt. " +
                        "Run 'dotnet run Scripts/refresh-region.cs' to re-download it.", ex);
                }

                if (!doc.RootElement.TryGetProperty("regions", out var regionsEl) ||
                    regionsEl.ValueKind != JsonValueKind.Array)
                {
                    throw new InvalidOperationException(
                        "contentstack_csharp: regions.json is corrupt. " +
                        "Run 'dotnet run Scripts/refresh-region.cs' to re-download it.");
                }

                var list = new List<JsonElement>();
                foreach (var r in regionsEl.EnumerateArray())
                    list.Add(r.Clone());

                _regionsData = list.ToArray();
                return _regionsData;
            }
        }

        /// <summary>
        /// Returns the path to regions.json next to the DLL.
        /// </summary>
        internal static string GetLocalFilePath()
        {
            string assemblyDir = Path.GetDirectoryName(typeof(Endpoint).Assembly.Location)
                                 ?? AppContext.BaseDirectory;
            return Path.Combine(assemblyDir, "Assets", "regions.json");
        }

        /// <summary>
        /// Reads regions.json from disk. Returns null if the file does not exist.
        /// </summary>
        private static string? ReadLocalFile(string path)
        {
            if (!File.Exists(path))
                return null;
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Downloads regions.json from the CDN and writes it to disk so that future
        /// process startups read from the local file instead of downloading again.
        /// Silent on all failures (network error, permission denied).
        /// </summary>
        private static string? DownloadAndSave(string dest)
        {
            try
            {
                var task = _httpClient.GetStringAsync(RegionsUrl);
                task.Wait();
                string data = task.Result;

                using var doc = JsonDocument.Parse(data);
                if (!doc.RootElement.TryGetProperty("regions", out _))
                    return null;

                // Write to disk — next startup reads from local file (Step 2).
                // Silent on PermissionError or read-only filesystem.
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    File.WriteAllText(dest, data);
                }
                catch { }

                return data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Two-pass region lookup: ID match wins over alias match.
        /// Input must already be lowercased and trimmed.
        /// </summary>
        private static JsonElement? FindRegion(JsonElement[] regions, string normalized)
        {
            // Pass 1 — exact id match
            foreach (var r in regions)
            {
                if (r.TryGetProperty("id", out var id) &&
                    id.GetString()?.ToLowerInvariant() == normalized)
                    return r;
            }

            // Pass 2 — alias match
            foreach (var r in regions)
            {
                if (!r.TryGetProperty("alias", out var aliases)) continue;
                foreach (var alias in aliases.EnumerateArray())
                {
                    if (alias.GetString()?.ToLowerInvariant() == normalized)
                        return r;
                }
            }

            return null;
        }

        private static string StripHttps(string url)
        {
            return Regex.Replace(url, @"^https?://", string.Empty);
        }
    }
}
