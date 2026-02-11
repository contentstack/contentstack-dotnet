using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Contentstack.Core.Models;
using Contentstack.Core.Internals;
using Contentstack.Core.Tests.Helpers;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Helper class for common test assertions
    /// Provides reusable assertion logic to keep tests DRY
    /// </summary>
    public static class AssertionHelper
    {
        #region Entry Assertions
        
        /// <summary>
        /// Asserts that an entry has all basic required fields populated
        /// </summary>
        public static void AssertEntryBasicFields(Entry entry, string expectedUid = null)
        {
            TestAssert.NotNull(entry);
            TestAssert.NotNull(entry.Uid);
            TestAssert.NotEmpty(entry.Uid);
            
            if (!string.IsNullOrEmpty(expectedUid))
            {
                TestAssert.Equal(expectedUid, entry.Uid);
            }
            
            // Title is usually required
            TestAssert.NotNull(entry.Title);
        }
        
        /// <summary>
        /// Asserts that an entry has metadata populated
        /// </summary>
        public static void AssertEntryMetadata(Entry entry)
        {
            TestAssert.NotNull(entry);
            
            var metadata = entry.GetMetadata();
            TestAssert.NotNull(metadata);
            
            // Metadata should be a dictionary (even if empty)
            TestAssert.IsType<Dictionary<string, object>>(metadata);
        }
        
        /// <summary>
        /// Asserts that a list of entries is not empty and valid
        /// </summary>
        public static void AssertEntriesValid<T>(IEnumerable<T> entries, int? expectedMinCount = null) where T : Entry
        {
            TestAssert.NotNull(entries);
            
            var entriesList = entries.ToList();
            TestAssert.NotEmpty(entriesList);
            
            if (expectedMinCount.HasValue)
            {
                TestAssert.True(entriesList.Count >= expectedMinCount.Value, 
                    $"Expected at least {expectedMinCount.Value} entries, but got {entriesList.Count}");
            }
            
            // All entries should have UIDs
            TestAssert.All(entriesList, entry => TestAssert.NotNull(entry.Uid));
        }
        
        #endregion

        #region Reference Assertions
        
        /// <summary>
        /// Asserts that references are populated at the specified level
        /// </summary>
        public static void AssertReferencesPopulated(Entry entry, string referenceFieldName, int expectedMinCount = 1)
        {
            TestAssert.NotNull(entry);
            
            var referenceField = entry.Get(referenceFieldName);
            TestAssert.NotNull(referenceField);
            
            if (referenceField is List<Entry> refList)
            {
                TestAssert.NotEmpty(refList);
                TestAssert.True(refList.Count >= expectedMinCount, 
                    $"Expected at least {expectedMinCount} references in '{referenceFieldName}', but got {refList.Count}");
                TestAssert.All(refList, refEntry => TestAssert.NotNull(refEntry.Uid));
            }
            else if (referenceField is Entry singleRef)
            {
                TestAssert.NotNull(singleRef.Uid);
            }
            else
            {
                TestAssert.Fail($"Reference field '{referenceFieldName}' is not of expected type (Entry or List<Entry>)");
            }
        }
        
        /// <summary>
        /// Asserts that a reference chain is populated to the specified depth
        /// </summary>
        public static void AssertReferenceChainDepth(Entry entry, string[] referenceFieldPath)
        {
            TestAssert.NotNull(entry);
            TestAssert.NotEmpty(referenceFieldPath);
            
            object current = entry;
            
            foreach (var fieldName in referenceFieldPath)
            {
                if (current is Entry currentEntry)
                {
                    var field = currentEntry.Get(fieldName);
                    TestAssert.NotNull(field);
                    current = field;
                }
                else if (current is List<Entry> entryList)
                {
                    TestAssert.NotEmpty(entryList);
                    current = entryList.First();
                    var field = ((Entry)current).Get(fieldName);
                    TestAssert.NotNull(field);
                    current = field;
                }
                else
                {
                    TestAssert.Fail($"Unexpected type in reference chain: {current.GetType().Name}");
                }
            }
        }
        
        #endregion

        #region Asset Assertions
        
        /// <summary>
        /// Asserts that an asset has all required fields populated
        /// </summary>
        public static void AssertAssetValid(Asset asset, string expectedUid = null)
        {
            TestAssert.NotNull(asset);
            TestAssert.NotNull(asset.Uid);
            TestAssert.NotEmpty(asset.Uid);
            TestAssert.NotNull(asset.Url);
            TestAssert.NotEmpty(asset.Url);
            TestAssert.NotNull(asset.FileName);
            TestAssert.NotEmpty(asset.FileName);
            
            if (!string.IsNullOrEmpty(expectedUid))
            {
                TestAssert.Equal(expectedUid, asset.Uid);
            }
        }
        
        /// <summary>
        /// Asserts that a collection of assets is valid
        /// </summary>
        public static void AssertAssetsValid(IEnumerable<Asset> assets, int? expectedMinCount = null)
        {
            TestAssert.NotNull(assets);
            
            var assetsList = assets.ToList();
            TestAssert.NotEmpty(assetsList);
            
            if (expectedMinCount.HasValue)
            {
                TestAssert.True(assetsList.Count >= expectedMinCount.Value, 
                    $"Expected at least {expectedMinCount.Value} assets, but got {assetsList.Count}");
            }
            
            TestAssert.All(assetsList, asset => AssertAssetValid(asset));
        }
        
        #endregion

        #region Query Result Assertions
        
        /// <summary>
        /// Asserts that a ContentstackCollection result is valid
        /// </summary>
        public static void AssertQueryResultValid<T>(ContentstackCollection<T> result, int? expectedMinCount = null) where T : Entry
        {
            TestAssert.NotNull(result);
            TestAssert.NotNull(result.Items);
            
            var items = result.Items.ToList();
            
            if (expectedMinCount.HasValue)
            {
                TestAssert.True(items.Count >= expectedMinCount.Value, 
                    $"Expected at least {expectedMinCount.Value} items, but got {items.Count}");
            }
        }
        
        /// <summary>
        /// Asserts that query results are sorted correctly
        /// </summary>
        public static void AssertSortedAscending<T, TKey>(IEnumerable<T> items, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            var itemsList = items.ToList();
            TestAssert.True(itemsList.Count >= 2, "Need at least 2 items to verify sorting");
            
            for (int i = 0; i < itemsList.Count - 1; i++)
            {
                var current = keySelector(itemsList[i]);
                var next = keySelector(itemsList[i + 1]);
                
                TestAssert.True(current.CompareTo(next) <= 0, 
                    $"Items are not sorted ascending at index {i}. Current: {current}, Next: {next}");
            }
        }
        
        /// <summary>
        /// Asserts that query results are sorted descending
        /// </summary>
        public static void AssertSortedDescending<T, TKey>(IEnumerable<T> items, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            var itemsList = items.ToList();
            TestAssert.True(itemsList.Count >= 2, "Need at least 2 items to verify sorting");
            
            for (int i = 0; i < itemsList.Count - 1; i++)
            {
                var current = keySelector(itemsList[i]);
                var next = keySelector(itemsList[i + 1]);
                
                TestAssert.True(current.CompareTo(next) >= 0, 
                    $"Items are not sorted descending at index {i}. Current: {current}, Next: {next}");
            }
        }
        
        #endregion

        #region Field Projection Assertions
        
        /// <summary>
        /// Asserts that only specified fields are present
        /// </summary>
        public static void AssertOnlyFieldsPresent(Entry entry, string[] expectedFields)
        {
            TestAssert.NotNull(entry);
            TestAssert.NotNull(expectedFields);
            
            // UID is always present
            var allowedFields = new List<string>(expectedFields) { "uid" };
            
            foreach (var key in entry.Object.Keys)
            {
                // Skip internal fields that start with underscore
                if (key.StartsWith("_"))
                    continue;
                    
                TestAssert.Contains(key, allowedFields);
            }
        }
        
        /// <summary>
        /// Asserts that specified fields are excluded
        /// </summary>
        public static void AssertFieldsExcluded(Entry entry, string[] excludedFields)
        {
            TestAssert.NotNull(entry);
            TestAssert.NotNull(excludedFields);
            
            foreach (var field in excludedFields)
            {
                TestAssert.Null(entry.Get(field));
            }
        }
        
        #endregion

        #region Date/Time Assertions
        
        /// <summary>
        /// Asserts that a date string is valid and parseable
        /// </summary>
        public static void AssertValidDate(string dateString)
        {
            TestAssert.NotNull(dateString);
            TestAssert.NotEmpty(dateString);
            TestAssert.True(DateTime.TryParse(dateString, out _), 
                $"'{dateString}' is not a valid date");
        }
        
        /// <summary>
        /// Asserts that a date is within an expected range
        /// </summary>
        public static void AssertDateInRange(DateTime date, DateTime minDate, DateTime maxDate)
        {
            TestAssert.True(date >= minDate && date <= maxDate, 
                $"Date {date} is not between {minDate} and {maxDate}");
        }
        
        #endregion

        #region Error Assertions
        
        /// <summary>
        /// Asserts that an exception is thrown with a specific error code
        /// </summary>
        public static void AssertContentstackException(Action action, int? expectedErrorCode = null)
        {
            var exception = TestAssert.Throws<ContentstackException>(action);
            
            if (expectedErrorCode.HasValue)
            {
                TestAssert.Equal(expectedErrorCode.Value, exception.ErrorCode);
            }
        }
        
        /// <summary>
        /// Asserts that an async exception is thrown with a specific error code
        /// </summary>
        public static async System.Threading.Tasks.Task AssertContentstackExceptionAsync(
            Func<System.Threading.Tasks.Task> action, 
            int? expectedErrorCode = null)
        {
            var exception = await TestAssert.ThrowsAsync<ContentstackException>(action);
            
            if (expectedErrorCode.HasValue)
            {
                TestAssert.Equal(expectedErrorCode.Value, exception.ErrorCode);
            }
        }
        
        #endregion
        
        #region Asset Assertions
        
        /// <summary>
        /// Asserts that an asset has all basic required fields populated
        /// </summary>
        public static void AssertAssetBasicFields(Asset asset, string expectedUid = null)
        {
            TestAssert.NotNull(asset);
            TestAssert.NotNull(asset.Uid);
            TestAssert.NotEmpty(asset.Uid);
            
            if (!string.IsNullOrEmpty(expectedUid))
            {
                TestAssert.Equal(expectedUid, asset.Uid);
            }
            
            // Required fields for assets
            TestAssert.NotNull(asset.Url);
            TestAssert.NotEmpty(asset.Url);
            TestAssert.NotNull(asset.FileName);
            TestAssert.NotEmpty(asset.FileName);
        }
        
        /// <summary>
        /// Asserts that an asset URL is valid and accessible
        /// </summary>
        public static void AssertAssetUrl(Asset asset)
        {
            TestAssert.NotNull(asset);
            TestAssert.NotNull(asset.Url);
            TestAssert.NotEmpty(asset.Url);
            
            // Verify it's a valid URL
            TestAssert.True(Uri.TryCreate(asset.Url, UriKind.Absolute, out var uri),
                $"Asset URL should be a valid absolute URL: {asset.Url}");
            TestAssert.True(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps,
                $"Asset URL should use HTTP or HTTPS: {asset.Url}");
        }
        
        #endregion
        
        #region Stack/Client Assertions
        
        /// <summary>
        /// Asserts that a ContentstackClient is properly configured with given options
        /// </summary>
        public static void AssertStackConfiguration(
            ContentstackClient client, 
            Configuration.ContentstackOptions options)
        {
            TestAssert.NotNull(client);
            TestAssert.NotNull(options);
            
            // Verify core configuration
            TestAssert.Equal(options.ApiKey, client.GetApplicationKey());
            TestAssert.Equal(options.DeliveryToken, client.GetAccessToken());
            
            // Version should always be available
            var version = client.GetVersion();
            TestAssert.NotNull(version);
            TestAssert.NotEmpty(version);
        }
        
        #endregion
    }
}

