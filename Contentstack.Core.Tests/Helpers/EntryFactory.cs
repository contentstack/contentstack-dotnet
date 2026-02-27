using System;
using System.Threading.Tasks;
using Contentstack.Core.Models;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Factory class for creating and fetching entries in tests
    /// Provides common patterns for entry retrieval
    /// </summary>
    public class EntryFactory
    {
        private readonly ContentstackClient _client;
        
        /// <summary>
        /// Initializes a new instance of EntryFactory
        /// </summary>
        /// <param name="client">Contentstack client instance</param>
        public EntryFactory(ContentstackClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        #region Single Entry Methods
        
        /// <summary>
        /// Fetches a single entry by UID
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="entryUid">Entry UID</param>
        /// <returns>Fetched entry</returns>
        public async Task<T> FetchEntryAsync<T>(string contentTypeUid, string entryUid) where T : Entry
        {
            return await _client
                .ContentType(contentTypeUid)
                .Entry(entryUid)
                .Fetch<T>();
        }
        
        /// <summary>
        /// Fetches a single entry with references
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="entryUid">Entry UID</param>
        /// <param name="referenceFields">Reference field UIDs to include</param>
        /// <returns>Fetched entry with references</returns>
        public async Task<T> FetchEntryWithReferencesAsync<T>(
            string contentTypeUid, 
            string entryUid, 
            params string[] referenceFields) where T : Entry
        {
            var entry = _client
                .ContentType(contentTypeUid)
                .Entry(entryUid);
            
            foreach (var refField in referenceFields)
            {
                entry.IncludeReference(refField);
            }
            
            return await entry.Fetch<T>();
        }
        
        /// <summary>
        /// Fetches a single entry with all options
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="entryUid">Entry UID</param>
        /// <param name="includeMetadata">Include metadata</param>
        /// <param name="includeBranch">Include branch</param>
        /// <param name="includeOwner">Include owner</param>
        /// <param name="locale">Locale code</param>
        /// <param name="includeFallback">Include fallback locale</param>
        /// <returns>Fetched entry</returns>
        public async Task<T> FetchEntryWithOptionsAsync<T>(
            string contentTypeUid, 
            string entryUid,
            bool includeMetadata = false,
            bool includeBranch = false,
            bool includeOwner = false,
            string locale = null,
            bool includeFallback = false) where T : Entry
        {
            var entry = _client
                .ContentType(contentTypeUid)
                .Entry(entryUid);
            
            if (includeMetadata)
                entry.IncludeMetadata();
            
            if (includeBranch)
                entry.IncludeBranch();
            
            if (includeOwner)
                entry.IncludeOwner();
            
            if (!string.IsNullOrEmpty(locale))
            {
                entry.SetLocale(locale);
                
                if (includeFallback)
                    entry.IncludeFallback();
            }
            
            return await entry.Fetch<T>();
        }
        
        #endregion
        
        #region Query Methods
        
        /// <summary>
        /// Creates a basic query for a content type
        /// </summary>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <returns>Query instance</returns>
        public Query CreateQuery(string contentTypeUid)
        {
            return _client.ContentType(contentTypeUid).Query();
        }
        
        /// <summary>
        /// Fetches all entries for a content type
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="limit">Optional limit</param>
        /// <returns>Collection of entries</returns>
        public async Task<ContentstackCollection<T>> FetchAllEntriesAsync<T>(
            string contentTypeUid, 
            int? limit = null) where T : Entry
        {
            var query = CreateQuery(contentTypeUid);
            
            if (limit.HasValue)
                query.Limit(limit.Value);
            
            return await query.Find<T>();
        }
        
        /// <summary>
        /// Fetches entries with pagination
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="skip">Number to skip</param>
        /// <param name="limit">Number to return</param>
        /// <returns>Collection of entries</returns>
        public async Task<ContentstackCollection<T>> FetchEntriesWithPaginationAsync<T>(
            string contentTypeUid, 
            int skip, 
            int limit) where T : Entry
        {
            return await CreateQuery(contentTypeUid)
                .Skip(skip)
                .Limit(limit)
                .Find<T>();
        }
        
        /// <summary>
        /// Fetches entries matching a field value
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <param name="fieldName">Field name to match</param>
        /// <param name="fieldValue">Field value to match</param>
        /// <returns>Collection of matching entries</returns>
        public async Task<ContentstackCollection<T>> FetchEntriesWhereAsync<T>(
            string contentTypeUid, 
            string fieldName, 
            object fieldValue) where T : Entry
        {
            return await CreateQuery(contentTypeUid)
                .Where(fieldName, fieldValue)
                .Find<T>();
        }
        
        #endregion
        
        #region Asset Methods
        
        /// <summary>
        /// Fetches a single asset by UID
        /// </summary>
        /// <param name="assetUid">Asset UID</param>
        /// <returns>Fetched asset</returns>
        public async Task<Asset> FetchAssetAsync(string assetUid)
        {
            return await _client.Asset(assetUid).Fetch();
        }
        
        /// <summary>
        /// Fetches all assets
        /// </summary>
        /// <param name="limit">Optional limit</param>
        /// <returns>Collection of assets</returns>
        public async Task<ContentstackCollection<Asset>> FetchAllAssetsAsync(int? limit = null)
        {
            var assetLibrary = _client.AssetLibrary();
            
            if (limit.HasValue)
                assetLibrary.Limit(limit.Value);
            
            return await assetLibrary.FetchAll();
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Fetches the first entry from a query (convenience method)
        /// FindOne returns a ContentstackCollection with limit=1
        /// </summary>
        /// <typeparam name="T">Entry model type</typeparam>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <returns>ContentstackCollection with one entry</returns>
        public async Task<ContentstackCollection<Entry>> FetchFirstEntryAsync(string contentTypeUid)
        {
            return await CreateQuery(contentTypeUid).FindOne<Entry>();
        }
        
        /// <summary>
        /// Counts entries in a content type
        /// </summary>
        /// <param name="contentTypeUid">Content type UID</param>
        /// <returns>Count result</returns>
        public async Task<Newtonsoft.Json.Linq.JObject> CountEntriesAsync(string contentTypeUid)
        {
            return await CreateQuery(contentTypeUid).Count();
        }
        
        #endregion
    }
}

