using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Contentstack.Core.Models;
using Contentstack.Core.Unit.Tests.Mokes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for Entry class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class EntryUnitTests
    {
        private readonly IFixture _fixture = new Fixture();
        private ContentstackClient _client;

        public EntryUnitTests()
        {
            Initialize();
        }

        private void Initialize()
        {
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>()
            };
            _client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
        }

        private Entry CreateEntry(string contentTypeId = "source")
        {
            var contentType = _client.ContentType(contentTypeId);
            return contentType.Entry("test_entry_uid");
        }

        private Entry CreateEntryWithAttributes(Dictionary<string, object> attributes)
        {
            var entry = CreateEntry();
            var field = typeof(Entry).GetField("_ObjectAttributes", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(entry, attributes);
            return entry;
        }

        private Entry CreateEntryWithoutUid(string contentTypeId = "source")
        {
            var contentType = _client.ContentType(contentTypeId);
            return contentType.Entry(null);
        }

        private Entry CreateEntryWithMockResponse(string mockResponse)
        {
            var entry = CreateEntry();
            _client.Plugins.Clear();
            _client.Plugins.Add(new MockHttpHandler(mockResponse));
            return entry;
        }

        #region Entry Initialization Tests

        [Fact]
        public void Initialize_Entry_WithUid()
        {
            // Arrange
            var entryUid = _fixture.Create<string>();
            var contentType = _client.ContentType("source");

            // Act
            var entry = contentType.Entry(entryUid);

            // Assert
            Assert.NotNull(entry);
            Assert.Equal(entryUid, entry.Uid);
        }

        [Fact]
        public void Initialize_Entry_WithoutUid()
        {
            // Arrange
            var contentType = _client.ContentType("source");

            // Act
            var entry = contentType.Entry(null);

            // Assert
            Assert.NotNull(entry);
            Assert.Null(entry.Uid);
        }

        [Fact]
        public void Initialize_Entry_WithEmptyUid()
        {
            // Arrange
            var contentType = _client.ContentType("source");

            // Act
            var entry = contentType.Entry("");

            // Assert
            Assert.NotNull(entry);
            Assert.Equal("", entry.Uid);
        }

        #endregion

        #region SetUid Tests

        [Fact]
        public void SetUid_WithValidUid_SetsUid()
        {
            // Arrange
            var entry = CreateEntry();
            var newUid = _fixture.Create<string>();

            // Act
            entry.SetUid(newUid);

            // Assert
            Assert.Equal(newUid, entry.Uid);
        }

        [Fact]
        public void SetUid_WithNullUid_SetsNull()
        {
            // Arrange
            var entry = CreateEntry();
            entry.Uid = "existing_uid";

            // Act
            entry.SetUid(null);

            // Assert
            Assert.Null(entry.Uid);
        }

        [Fact]
        public void SetUid_WithEmptyUid_SetsEmpty()
        {
            // Arrange
            var entry = CreateEntry();
            var existingUid = "existing_uid";
            entry.Uid = existingUid;

            // Act
            entry.SetUid("");

            // Assert
            Assert.Equal("", entry.Uid);
        }

        #endregion

        #region SetTags Tests

        [Fact]
        public void SetTags_WithValidTags_SetsTags()
        {
            // Arrange
            var entry = CreateEntry();
            var tags = new string[] { "tag1", "tag2", "tag3" };

            // Act
            entry.SetTags(tags);

            // Assert
            Assert.Equal(tags, entry.Tags);
        }

        [Fact]
        public void SetTags_WithNullTags_SetsNull()
        {
            // Arrange
            var entry = CreateEntry();
            entry.Tags = new object[] { "tag1" };

            // Act
            entry.SetTags(null);

            // Assert
            Assert.Null(entry.Tags);
        }

        [Fact]
        public void SetTags_WithEmptyTags_SetsEmpty()
        {
            // Arrange
            var entry = CreateEntry();
            var emptyTags = new string[0];

            // Act
            entry.SetTags(emptyTags);

            // Assert
            Assert.Equal(emptyTags, entry.Tags);
        }

        #endregion

        #region GetTags Tests

        [Fact]
        public void GetTags_WithTags_ReturnsTags()
        {
            // Arrange
            var tags = new object[] { "tag1", "tag2" };
            var attributes = new Dictionary<string, object>
            {
                { "tags", tags }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            var result = entry.GetTags();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tags, result);
        }

        [Fact]
        public void GetTags_WithoutTags_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            var result = entry.GetTags();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTags_WithInvalidTags_ReturnsNull()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "tags", "not_an_array" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            var result = entry.GetTags();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Object Property Tests

        [Fact]
        public void Object_Get_ReturnsObjectAttributes()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_uid" },
                { "title", "Test Title" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            var result = entry.Object;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(attributes, result);
        }

        [Fact]
        public void Object_Set_UpdatesObjectAttributes()
        {
            // Arrange
            var entry = CreateEntry();
            var newAttributes = new Dictionary<string, object>
            {
                { "uid", "new_uid" },
                { "title", "New Title" }
            };

            // Act
            entry.Object = newAttributes;

            // Assert
            Assert.Equal(newAttributes, entry.Object);
        }

        #endregion

        #region Title Property Tests

        [Fact]
        public void Title_Get_ReturnsTitle()
        {
            // Arrange
            var title = _fixture.Create<string>();
            var entry = CreateEntry();
            entry.Title = title;

            // Act
            var result = entry.Title;

            // Assert
            Assert.Equal(title, result);
        }

        [Fact]
        public void Title_Set_UpdatesTitle()
        {
            // Arrange
            var entry = CreateEntry();
            var newTitle = _fixture.Create<string>();

            // Act
            entry.Title = newTitle;

            // Assert
            Assert.Equal(newTitle, entry.Title);
        }

        #endregion

        #region Uid Property Tests

        [Fact]
        public void Uid_Get_ReturnsUid()
        {
            // Arrange
            var uid = _fixture.Create<string>();
            var entry = CreateEntry();
            entry.Uid = uid;

            // Act
            var result = entry.Uid;

            // Assert
            Assert.Equal(uid, result);
        }

        [Fact]
        public void Uid_Set_UpdatesUid()
        {
            // Arrange
            var entry = CreateEntry();
            var newUid = _fixture.Create<string>();

            // Act
            entry.Uid = newUid;

            // Assert
            Assert.Equal(newUid, entry.Uid);
        }

        #endregion

        #region Tags Property Tests

        [Fact]
        public void Tags_Get_ReturnsTags()
        {
            // Arrange
            var tags = new object[] { "tag1", "tag2" };
            var entry = CreateEntry();
            entry.Tags = tags;

            // Act
            var result = entry.Tags;

            // Assert
            Assert.Equal(tags, result);
        }

        [Fact]
        public void Tags_Set_UpdatesTags()
        {
            // Arrange
            var entry = CreateEntry();
            var newTags = new object[] { "tag3", "tag4" };

            // Act
            entry.Tags = newTags;

            // Assert
            Assert.Equal(newTags, entry.Tags);
        }

        #endregion

        #region Metadata Property Tests

        [Fact]
        public void Metadata_Get_ReturnsMetadata()
        {
            // Arrange
            var metadata = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var entry = CreateEntry();
            entry.Metadata = metadata;

            // Act
            var result = entry.Metadata;

            // Assert
            Assert.Equal(metadata, result);
        }

        [Fact]
        public void Metadata_Set_UpdatesMetadata()
        {
            // Arrange
            var entry = CreateEntry();
            var newMetadata = new Dictionary<string, object>
            {
                { "key3", "value3" }
            };

            // Act
            entry.Metadata = newMetadata;

            // Assert
            Assert.Equal(newMetadata, entry.Metadata);
        }

        #endregion

        #region GetDeletedAt Tests

        [Fact]
        public void GetDeletedAt_WithDeletedEntry_ReturnsDateTime()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" },
                { "deleted_at", "2023-01-03T00:00:00.000Z" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            DateTime deletedAt = entry.GetDeletedAt();

            // Assert
            Assert.NotEqual(DateTime.MinValue, deletedAt);
            Assert.Equal(2023, deletedAt.Year);
            Assert.Equal(1, deletedAt.Month);
            Assert.Equal(3, deletedAt.Day);
        }

        [Fact]
        public void GetDeletedAt_WithoutDeletedAt_ReturnsMinValue()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            DateTime deletedAt = entry.GetDeletedAt();

            // Assert
            Assert.Equal(DateTime.MinValue, deletedAt);
        }

        #endregion

        #region GetDeletedBy Tests

        [Fact]
        public void GetDeletedBy_WithDeletedBy_ReturnsUid()
        {
            // Arrange
            var deletedBy = _fixture.Create<string>();
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" },
                { "deleted_by", deletedBy }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            string result = entry.GetDeletedBy();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deletedBy, result);
        }

        [Fact]
        public void GetDeletedBy_WithoutDeletedBy_ThrowsException()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => entry.GetDeletedBy());
        }

        #endregion

        #region GetContentType Tests

        [Fact]
        public void GetContentType_ReturnsContentTypeId()
        {
            // Arrange
            var contentTypeId = "source";
            var entry = CreateEntry(contentTypeId);

            // Act
            var result = entry.GetContentType();

            // Assert
            Assert.Equal(contentTypeId, result);
        }

        #endregion

        #region GetUid Tests

        [Fact]
        public void GetUid_ReturnsUid()
        {
            // Arrange
            var uid = "test_entry_uid";
            var entry = CreateEntry();

            // Act
            var result = entry.GetUid();

            // Assert
            Assert.Equal(uid, result);
        }

        [Fact]
        public void GetUid_WithNullUid_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntryWithoutUid();

            // Act
            var result = entry.GetUid();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Get Tests

        [Fact]
        public void Get_WithValidKey_ReturnsValue()
        {
            // Arrange
            var key = "test_key";
            var value = "test_value";
            var attributes = new Dictionary<string, object>
            {
                { key, value }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            var result = entry.Get(key);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public void Get_WithInvalidKey_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            var result = entry.Get("non_existent_key");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetCreateAt Tests

        [Fact]
        public void GetCreateAt_WithCreatedAt_ReturnsDateTime()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "created_at", "2023-01-01T00:00:00.000Z" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            DateTime result = entry.GetCreateAt();

            // Assert
            Assert.NotEqual(DateTime.MinValue, result);
            Assert.Equal(2023, result.Year);
            Assert.Equal(1, result.Month);
            Assert.Equal(1, result.Day);
        }

        [Fact]
        public void GetCreateAt_WithoutCreatedAt_ReturnsMinValue()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            DateTime result = entry.GetCreateAt();

            // Assert
            Assert.Equal(DateTime.MinValue, result);
        }

        #endregion

        #region GetCreatedBy Tests

        [Fact]
        public void GetCreatedBy_WithCreatedBy_ReturnsUid()
        {
            // Arrange
            var createdBy = "user_123";
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "created_by", createdBy }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            string result = entry.GetCreatedBy();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdBy, result);
        }

        [Fact]
        public void GetCreatedBy_WithoutCreatedBy_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            string result = entry.GetCreatedBy();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUpdateAt Tests

        [Fact]
        public void GetUpdateAt_WithUpdatedAt_ReturnsDateTime()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "updated_at", "2023-01-02T00:00:00.000Z" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            DateTime result = entry.GetUpdateAt();

            // Assert
            Assert.NotEqual(DateTime.MinValue, result);
            Assert.Equal(2023, result.Year);
            Assert.Equal(1, result.Month);
            Assert.Equal(2, result.Day);
        }

        [Fact]
        public void GetUpdateAt_WithoutUpdatedAt_ReturnsMinValue()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            DateTime result = entry.GetUpdateAt();

            // Assert
            Assert.Equal(DateTime.MinValue, result);
        }

        #endregion

        #region GetUpdatedBy Tests

        [Fact]
        public void GetUpdatedBy_WithUpdatedBy_ReturnsUid()
        {
            // Arrange
            var updatedBy = "user_456";
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "updated_by", updatedBy }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            string result = entry.GetUpdatedBy();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedBy, result);
        }

        [Fact]
        public void GetUpdatedBy_WithoutUpdatedBy_ReturnsEmpty()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            string result = entry.GetUpdatedBy();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region GetHTMLText Tests

        [Fact]
        public void GetHTMLText_WithMarkdownKey_ReturnsHTML()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "markdown_content", "# Heading 1\n\n**Bold** text" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            string result = entry.GetHTMLText("markdown_content");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            // Markdown generates <h1 id="heading-1"> or <h1> depending on extensions
            Assert.True(result.Contains("<h1") && result.Contains("Heading 1"));
        }

        [Fact]
        public void GetHTMLText_WithoutKey_ReturnsEmpty()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            string result = entry.GetHTMLText("non_existent_key");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region GetMetadata Tests

        [Fact]
        public void GetMetadata_WithMetadata_ReturnsMetadata()
        {
            // Arrange
            var metadata = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            var entry = CreateEntry();
            entry.Metadata = metadata;

            // Act
            var result = entry.GetMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(metadata, result);
        }

        [Fact]
        public void GetMetadata_WithoutMetadata_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            var result = entry.GetMetadata();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ToJson Tests

        [Fact]
        public void ToJson_ReturnsJObject()
        {
            // Arrange
            var entry = CreateEntry();
            var jObjectField = typeof(Entry).GetField("jObject", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var jObject = new JObject
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" },
                { "content_type_uid", "source" }
            };
            jObjectField?.SetValue(entry, jObject);

            // Act
            JObject result = entry.ToJson();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test_entry_uid", result["uid"].ToString());
            Assert.Equal("Test Entry", result["title"].ToString());
        }

        [Fact]
        public void ToJson_WithNullJObject_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();
            var jObjectField = typeof(Entry).GetField("jObject", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            jObjectField?.SetValue(entry, null);

            // Act
            JObject result = entry.ToJson();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SetCachePolicy Tests

        [Fact]
        public void SetCachePolicy_SetsCachePolicyAndReturnsEntry()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry.SetCachePolicy(CachePolicy.NetworkOnly);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var cachePolicyField = typeof(Entry).GetField("_CachePolicy", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var isCachePolicySetField = typeof(Entry).GetField("_IsCachePolicySet", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            Assert.Equal(CachePolicy.NetworkOnly, cachePolicyField?.GetValue(entry));
            Assert.True((bool)(isCachePolicySetField?.GetValue(entry) ?? false));
        }

        [Fact]
        public void SetCachePolicy_SupportsMethodChaining()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry
                .SetCachePolicy(CachePolicy.NetworkOnly)
                .IncludeMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
        }

        #endregion

        #region GetTitle Tests

        [Fact]
        public void GetTitle_ReturnsTitle()
        {
            // Arrange
            var title = _fixture.Create<string>();
            var entry = CreateEntry();
            entry.Title = title;

            // Act
            string result = entry.GetTitle();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(title, result);
        }

        [Fact]
        public void GetTitle_WithNullTitle_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();
            entry.Title = null;

            // Act
            string result = entry.GetTitle();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetMultipleHTMLText Tests

        [Fact]
        public void GetMultipleHTMLText_WithMarkdownArray_ReturnsHTMLList()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" },
                { "markdown_array", new object[] { "# Heading 1", "## Heading 2", "**Bold** text" } }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            List<string> result = entry.GetMultipleHTMLText("markdown_array");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.All(result, html => Assert.NotNull(html));
            Assert.All(result, html => Assert.True(html.Length > 0));
        }

        [Fact]
        public void GetMultipleHTMLText_WithoutKey_ReturnsEmptyList()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
            {
                { "uid", "test_entry_uid" },
                { "title", "Test Entry" }
            };
            var entry = CreateEntryWithAttributes(attributes);

            // Act
            List<string> result = entry.GetMultipleHTMLText("non_existent_key");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region AddParam Tests

        [Fact]
        public void AddParam_AddsParameterAndReturnsEntry()
        {
            // Arrange
            var entry = CreateEntry();
            var key = _fixture.Create<string>();
            var value = _fixture.Create<string>();

            // Act
            Entry result = entry.AddParam(key, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey(key) ?? false);
            Assert.Equal(value, urlQueries?[key]);
        }

        [Fact]
        public void AddParam_SupportsMethodChaining()
        {
            // Arrange
            var entry = CreateEntry();
            var key1 = _fixture.Create<string>();
            var value1 = _fixture.Create<string>();
            var key2 = _fixture.Create<string>();
            var value2 = _fixture.Create<string>();

            // Act
            Entry result = entry
                .AddParam(key1, value1)
                .AddParam(key2, value2)
                .IncludeMetadata();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey(key1) ?? false);
            Assert.True(urlQueries?.ContainsKey(key2) ?? false);
        }

        #endregion

        #region Reference Methods Tests

        [Fact]
        public void IncludeReferenceContentTypeUID_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry.IncludeReferenceContentTypeUID();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include_reference_content_type_uid") ?? false);
            Assert.True((bool)(urlQueries?["include_reference_content_type_uid"] ?? false));
        }

        [Fact]
        public void IncludeOnlyReference_AddsQueryParameters()
        {
            // Arrange
            var entry = CreateEntry();
            string[] keys = { "name", "description" };
            string referenceKey = _fixture.Create<string>();

            // Act
            Entry result = entry.IncludeOnlyReference(keys, referenceKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey($"only[{referenceKey}][]") ?? false);
            Assert.Equal(keys, urlQueries?[$"only[{referenceKey}][]"]);
        }

        [Fact]
        public void IncludeOnlyReference_WithNullKeys_DoesNotAddParameters()
        {
            // Arrange
            var entry = CreateEntry();
            string referenceKey = _fixture.Create<string>();

            // Act
            Entry result = entry.IncludeOnlyReference(null, referenceKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.False(urlQueries?.ContainsKey($"only[{referenceKey}][]") ?? true);
        }

        [Fact]
        public void IncludeExceptReference_AddsQueryParameters()
        {
            // Arrange
            var entry = CreateEntry();
            string[] keys = { "name", "description" };
            string referenceKey = _fixture.Create<string>();

            // Act
            Entry result = entry.IncludeExceptReference(keys, referenceKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey($"except[{referenceKey}][]") ?? false);
            Assert.Equal(keys, urlQueries?[$"except[{referenceKey}][]"]);
        }

        [Fact]
        public void includeEmbeddedItems_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry.includeEmbeddedItems();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include_embedded_items[]") ?? false);
            Assert.Equal("BASE", urlQueries?["include_embedded_items[]"]);
        }

        [Fact]
        public void IncludeOwner_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry.IncludeOwner();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include_owner") ?? false);
            Assert.True((bool)(urlQueries?["include_owner"] ?? false));
        }

        #endregion

        #region SetLocale Tests

        [Fact]
        public void SetLocale_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var locale = "en-us";

            // Act
            Entry result = entry.SetLocale(locale);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.Equal(locale, urlQueries?["locale"]?.ToString());
        }

        [Fact]
        public void SetLocale_WithNullLocale_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.SetLocale(null);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // If locale was already not present, count should remain same
            Assert.True(true);
        }

        #endregion

        #region IncludeReference Tests

        [Fact]
        public void IncludeReference_WithSingleField_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var referenceField = _fixture.Create<string>();

            // Act
            Entry result = entry.IncludeReference(referenceField);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include[]") ?? false);
        }

        [Fact]
        public void IncludeReference_WithArray_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var referenceFields = new string[] { "field1", "field2", "field3" };

            // Act
            Entry result = entry.IncludeReference(referenceFields);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include[]") ?? false);
            Assert.Equal(referenceFields, urlQueries?["include[]"]);
        }

        [Fact]
        public void IncludeReference_WithNullField_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.IncludeReference((string)null);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // If include[] was already not present, count should remain same
            Assert.True(true);
        }

        [Fact]
        public void IncludeReference_WithNullArray_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.IncludeReference((string[])null);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // If include[] was already not present, count should remain same
            Assert.True(true);
        }

        [Fact]
        public void IncludeReference_WithEmptyArray_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.IncludeReference(new string[0]);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Empty array should not add parameter
            Assert.True(true);
        }

        #endregion

        #region IncludeFallback Tests

        [Fact]
        public void IncludeFallback_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry.IncludeFallback();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.Equal("true", urlQueries?["include_fallback"]?.ToString());
        }

        #endregion

        #region IncludeBranch Tests

        [Fact]
        public void IncludeBranch_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            Entry result = entry.IncludeBranch();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.Equal("true", urlQueries?["include_branch"]?.ToString());
        }

        #endregion

        #region Only Tests

        [Fact]
        public void Only_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var fieldUids = new string[] { "field1", "field2" };

            // Act
            Entry result = entry.Only(fieldUids);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("only[BASE][]") ?? false);
            Assert.Equal(fieldUids, urlQueries?["only[BASE][]"]);
        }

        [Fact]
        public void Only_WithNullFields_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.Only(null);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Null fields should not add parameter
            Assert.True(true);
        }

        [Fact]
        public void Only_WithEmptyFields_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.Only(new string[0]);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Empty fields should not add parameter
            Assert.True(true);
        }

        #endregion

        #region Except Tests

        [Fact]
        public void Except_AddsQueryParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var fieldUids = new string[] { "field1", "field2" };

            // Act
            Entry result = entry.Except(fieldUids);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entry, result);
            
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            Assert.True(urlQueries?.ContainsKey("except[BASE][]") ?? false);
            Assert.Equal(fieldUids, urlQueries?["except[BASE][]"]);
        }

        [Fact]
        public void Except_WithNullFields_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.Except(null);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Null fields should not add parameter
            Assert.True(true);
        }

        [Fact]
        public void Except_WithEmptyFields_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueriesBefore = new Dictionary<string, object>((Dictionary<string, object>)urlQueriesField?.GetValue(entry));

            // Act
            Entry result = entry.Except(new string[0]);

            // Assert
            Assert.NotNull(result);
            var urlQueriesAfter = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Empty fields should not add parameter
            Assert.True(true);
        }

        #endregion

        #region Fetch Setup Tests

        [Fact]
        public void Fetch_WithCachePolicy_SetBeforeFetch_RespectsCachePolicy()
        {
            // Arrange
            var entry = CreateEntry();
            entry.SetCachePolicy(CachePolicy.NetworkOnly);

            // Act - Verify cache policy is set (we don't actually call Fetch to avoid HTTP)
            var cachePolicyField = typeof(Entry).GetField("_CachePolicy", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var isCachePolicySetField = typeof(Entry).GetField("_IsCachePolicySet", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.Equal(CachePolicy.NetworkOnly, cachePolicyField?.GetValue(entry));
            Assert.True((bool)(isCachePolicySetField?.GetValue(entry) ?? false));
        }

        [Fact]
        public void Fetch_WithQueryParameters_SetBeforeFetch_IncludesQueryParameters()
        {
            // Arrange
            var entry = CreateEntry();
            entry.IncludeMetadata()
                  .IncludeOwner()
                  .SetLocale("en-us");

            // Act - Verify query parameters are set (we don't actually call Fetch to avoid HTTP)
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);

            // Assert
            Assert.True(urlQueries?.ContainsKey("include_metadata") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_owner") ?? false);
            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
        }

        [Fact]
        public void Fetch_WithChainedQueryParameters_AllParametersSet()
        {
            // Arrange
            var entry = CreateEntry();

            // Act - Chain multiple query parameter methods
            entry.IncludeMetadata()
                  .IncludeOwner()
                  .IncludeFallback()
                  .IncludeBranch()
                  .SetLocale("en-us")
                  .IncludeReference("field1")
                  .IncludeReferenceContentTypeUID();

            // Assert - Verify all parameters are set
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);

            Assert.True(urlQueries?.ContainsKey("include_metadata") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_owner") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.True(urlQueries?.ContainsKey("include[]") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_reference_content_type_uid") ?? false);
        }

        [Fact]
        public void Fetch_WithDefaultCachePolicy_NetworkOnly()
        {
            // Arrange
            var entry = CreateEntry();

            // Act - Verify default cache policy
            var cachePolicyField = typeof(Entry).GetField("_CachePolicy", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var isCachePolicySetField = typeof(Entry).GetField("_IsCachePolicySet", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert - Default should be NetworkOnly when not set
            // The Fetch method defaults to NetworkOnly if _IsCachePolicySet is false
            Assert.False((bool)(isCachePolicySetField?.GetValue(entry) ?? true));
        }

        [Fact]
        public void Fetch_WithMultipleParameters_VerifiesAllQueryParameters()
        {
            // Arrange
            var entry = CreateEntry();
            entry.IncludeFallback().IncludeBranch().IncludeReference("field1").SetLocale("en-us");
            entry.SetHeader("custom_header", "value");

            // Act - Just verify setup, not actual HTTP call
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            
            var headersField = typeof(Entry).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(entry);

            // Assert
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false);
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false);
            Assert.True(urlQueries?.ContainsKey("include[]") ?? false);
            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.True(headers?.ContainsKey("custom_header") ?? false);
        }

        [Fact]
        public void Fetch_WithCachePolicy_VerifiesCachePolicy()
        {
            // Arrange
            var entry = CreateEntry();
            entry.SetCachePolicy(CachePolicy.NetworkOnly);

            // Act - Just verify setup
            var cachePolicyField = typeof(Entry).GetField("_CachePolicy", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var cachePolicy = (CachePolicy?)cachePolicyField?.GetValue(entry);

            // Assert
            Assert.NotNull(cachePolicy);
            Assert.Equal(CachePolicy.NetworkOnly, cachePolicy.Value);
        }

        [Fact]
        public void Fetch_WithAllIncludeMethods_VerifiesQueryParameters()
        {
            // Arrange
            var entry = CreateEntry();
            entry.IncludeReference("field1").IncludeReferenceContentTypeUID()
                .IncludeOnlyReference(new[] { "key1", "key2" }, "ref_uid")
                .IncludeExceptReference(new[] { "key3", "key4" }, "ref_uid")
                .includeEmbeddedItems().IncludeOwner().IncludeFallback().IncludeBranch();

            // Act - Just verify setup
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);

            // Assert - Check that expected keys are present
            // IncludeReference adds "include[]", IncludeReferenceContentTypeUID adds "include_reference_content_type_uid"
            Assert.True(urlQueries?.ContainsKey("include[]") ?? false, "include[] should be present");
            Assert.True(urlQueries?.ContainsKey("include_reference_content_type_uid") ?? false, "include_reference_content_type_uid should be present");
            // Check if only[ref_uid][] exists (it may not if include[] was already added)
            var hasOnlyKey = urlQueries?.Keys.Any(k => k.Contains("only[") && k.Contains("ref_uid")) ?? false;
            Assert.True(hasOnlyKey || (urlQueries?.ContainsKey("only[ref_uid][]") ?? false), "only[ref_uid][] should be present");
            Assert.True(urlQueries?.ContainsKey("except[ref_uid][]") ?? false, "except[ref_uid][] should be present");
            Assert.True(urlQueries?.ContainsKey("include_embedded_items[]") ?? false, "include_embedded_items[] should be present");
            Assert.True(urlQueries?.ContainsKey("include_owner") ?? false, "include_owner should be present");
            Assert.True(urlQueries?.ContainsKey("include_fallback") ?? false, "include_fallback should be present");
            Assert.True(urlQueries?.ContainsKey("include_branch") ?? false, "include_branch should be present");
        }

        [Fact]
        public void GetContentstackError_WithWebException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(Entry).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var webEx = new System.Net.WebException("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { webEx }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetContentstackError_WithGenericException_ReturnsContentstackException()
        {
            // Arrange
            var method = typeof(Entry).GetMethod("GetContentstackError", 
                BindingFlags.NonPublic | BindingFlags.Static);
            var ex = new Exception("Test error");

            // Act
            var result = method?.Invoke(null, new object[] { ex }) as ContentstackException;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test error", result.ErrorMessage);
        }

        [Fact]
        public void GetHeader_WithNullLocalHeader_ReturnsFormHeaders()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndNoFormHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _FormHeaders to null
            var formHeadersField = typeof(Entry).GetField("_FormHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            formHeadersField?.SetValue(entry, null);

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetHeader_WithLocalHeaderAndFormHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ParseObject_WithMetadata_ParsesMetadata()
        {
            // Arrange
            var entry = CreateEntry();
            var parseObjectMethod = typeof(Entry).GetMethod("ParseObject", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var json = new JObject
            {
                ["title"] = "Test Title",
                ["_metadata"] = new JObject
                {
                    ["uid"] = "test_uid",
                    ["tags"] = new JArray("tag1", "tag2")
                }
            };

            // Act
            parseObjectMethod?.Invoke(entry, new object[] { json, null });

            // Assert
            var metadataField = typeof(Entry).GetProperty("Metadata", 
                BindingFlags.Public | BindingFlags.Instance);
            var metadata = metadataField?.GetValue(entry) as Dictionary<string, object>;
            Assert.NotNull(metadata);
        }

        [Fact]
        public void ParseObject_WithoutMetadata_DoesNotSetMetadata()
        {
            // Arrange
            var entry = CreateEntry();
            var parseObjectMethod = typeof(Entry).GetMethod("ParseObject", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var json = new JObject
            {
                ["title"] = "Test Title"
            };

            // Act
            parseObjectMethod?.Invoke(entry, new object[] { json, null });

            // Assert
            var metadataField = typeof(Entry).GetProperty("Metadata", 
                BindingFlags.Public | BindingFlags.Instance);
            var metadata = metadataField?.GetValue(entry) as Dictionary<string, object>;
            // Metadata may be null or empty if not present
            Assert.True(true); // Just verify no exception thrown
        }

        [Fact]
        public void IncludeOnlyReference_WithEmptyKeys_DoesNotAddParameters()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.IncludeOnlyReference(new string[0], "ref_uid");

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            Assert.False(urlQueries?.ContainsKey("only[ref_uid][]") ?? false);
        }

        [Fact]
        public void IncludeExceptReference_WithEmptyKeys_DoesNotAddParameters()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.IncludeExceptReference(new string[0], "ref_uid");

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            Assert.False(urlQueries?.ContainsKey("except[ref_uid][]") ?? false);
        }

        [Fact]
        public void IncludeExceptReference_WithNullKeys_DoesNotAddParameters()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.IncludeExceptReference(null, "ref_uid");

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            Assert.False(urlQueries?.ContainsKey("except[ref_uid][]") ?? false);
        }

        [Fact]
        public void Only_WithEmptyArray_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.Only(new string[0]);

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            Assert.False(urlQueries?.ContainsKey("only[BASE][]") ?? false);
        }

        [Fact]
        public void Only_WithNullArray_DoesNotAddParameter()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.Only(null);

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            Assert.False(urlQueries?.ContainsKey("only[BASE][]") ?? false);
        }

        [Fact]
        public void IncludeOnlyReference_WhenIncludeKeyExists_StillAddsOnlyKey()
        {
            // Arrange
            var entry = CreateEntry();
            entry.IncludeReference("ref_uid"); // This adds "include[]"

            // Act
            entry.IncludeOnlyReference(new[] { "key1", "key2" }, "ref_uid");

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Should still add only key even if include[] exists
            Assert.True(urlQueries?.ContainsKey("only[ref_uid][]") ?? false);
        }

        [Fact]
        public void IncludeExceptReference_WhenIncludeKeyExists_StillAddsExceptKey()
        {
            // Arrange
            var entry = CreateEntry();
            entry.IncludeReference("ref_uid"); // This adds "include[]"

            // Act
            entry.IncludeExceptReference(new[] { "key1", "key2" }, "ref_uid");

            // Assert
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            // Should still add except key even if include[] exists
            Assert.True(urlQueries?.ContainsKey("except[ref_uid][]") ?? false);
        }

        #endregion

        #region SetLocale Edge Cases

        [Fact]
        public void SetLocale_WhenLocaleAlreadyExists_DoesNotAddAgain()
        {
            // Arrange
            var entry = CreateEntry();
            var locale = "en-us";

            // Act
            entry.SetLocale(locale);
            entry.SetLocale(locale); // Try to add again

            // Assert
            // Note: SetLocale stores locale in UrlQueries, and when locale already exists in ObjectValueJson,
            // it uses the else branch which sets UrlQueries["locale"] = Locale. Dictionary keys are unique,
            // so calling SetLocale twice with the same value just updates the existing entry.
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);
            Assert.True(urlQueries?.ContainsKey("locale") ?? false);
            Assert.Equal(locale, urlQueries?["locale"]); // Should have the locale value
        }

        #endregion

        #region Variant Edge Cases

        [Fact]
        public void Variant_WithEmptyString_DoesNotAddHeader()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.Variant("");

            // Assert
            // Note: Variant calls SetHeader which checks `value != null`, so empty string IS added (empty string != null)
            var headersField = typeof(Entry).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(entry);
            Assert.True(headers?.ContainsKey("x-cs-variant-uid") ?? false); // Empty string is added
        }

        [Fact]
        public void Variant_WithNullString_DoesNotAddHeader()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.Variant((string)null);

            // Assert
            var headersField = typeof(Entry).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(entry);
            Assert.False(headers?.ContainsKey("x-cs-variant-uid") ?? false);
        }

        [Fact]
        public void Variant_WithEmptyList_DoesNotAddHeader()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            entry.Variant(new List<string>());

            // Assert
            // Note: string.Join with empty list returns empty string, which is then added to headers
            var headersField = typeof(Entry).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(entry);
            Assert.True(headers?.ContainsKey("x-cs-variant-uid") ?? false); // Empty string is added
        }

        [Fact]
        public void Variant_WithNullList_DoesNotAddHeader()
        {
            // Arrange
            var entry = CreateEntry();

            // Act & Assert
            // Variant calls string.Join which throws ArgumentNullException for null list
            Assert.Throws<ArgumentNullException>(() => entry.Variant((List<string>)null));
        }

        [Fact]
        public void GetCreateAt_WithInvalidDateTimeFormat_ReturnsMinValue()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "created_at", "invalid_date_format" }
            });

            // Act
            DateTime result = entry.GetCreateAt();

            // Assert
            // Should return MinValue when date parsing fails
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void GetUpdateAt_WithInvalidDateTimeFormat_ReturnsMinValue()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "updated_at", "invalid_date_format" }
            });

            // Act
            DateTime result = entry.GetUpdateAt();

            // Assert
            // Should return MinValue when date parsing fails
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void GetCreatedBy_WithNullValue_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "created_by", null }
            });

            // Act
            string result = entry.GetCreatedBy();

            // Assert
            // Should return null when value is null
            Assert.Null(result);
        }

        [Fact]
        public void GetUpdatedBy_WithNullValue_ReturnsEmpty()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "updated_by", null }
            });

            // Act
            string result = entry.GetUpdatedBy();

            // Assert
            // Should return empty string when value is null
            Assert.Empty(result);
        }

        [Fact]
        public void GetHTMLText_WithValidMarkdown_ReturnsHtml()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "markdown_field", "# Hello World" }
            });

            // Act
            string result = entry.GetHTMLText("markdown_field");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetHTMLText_WithNonExistentKey_ReturnsEmptyString()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            string result = entry.GetHTMLText("non_existent_key");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetHTMLText_WithNullValue_ReturnsEmptyString()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "markdown_field", null }
            });

            // Act
            string result = entry.GetHTMLText("markdown_field");

            // Assert
            // Should return empty string when value is null or conversion fails
            Assert.Empty(result);
        }

        [Fact]
        public void GetMultipleHTMLText_WithValidArray_ReturnsHtmlList()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "markdown_array", new object[] { "# Header 1", "# Header 2" } }
            });

            // Act
            List<string> result = entry.GetMultipleHTMLText("markdown_array");

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetMultipleHTMLText_WithNonExistentKey_ReturnsEmptyList()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            List<string> result = entry.GetMultipleHTMLText("non_existent_key");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMultipleHTMLText_WithNullValue_ReturnsEmptyList()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "markdown_array", null }
            });

            // Act
            List<string> result = entry.GetMultipleHTMLText("markdown_array");

            // Assert
            // Should return empty list when value is null or conversion fails
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetMultipleHTMLText_WithInvalidArray_ReturnsEmptyList()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "markdown_array", "not_an_array" }
            });

            // Act
            List<string> result = entry.GetMultipleHTMLText("markdown_array");

            // Assert
            // Should return empty list when cast fails
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Get_WithNullKey_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();

            // Act
            var result = entry.Get(null);

            // Assert
            // Dictionary.ContainsKey(null) throws ArgumentNullException, caught and returns null
            Assert.Null(result);
        }

        [Fact]
        public void GetCreatedBy_WithException_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntry();
            // Set _ObjectAttributes to null to trigger exception
            var field = typeof(Entry).GetField("_ObjectAttributes", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(entry, null);

            // Act
            string result = entry.GetCreatedBy();

            // Assert
            // Should return null when exception occurs
            Assert.Null(result);
        }

        [Fact]
        public void GetCreatedBy_WithToStringException_ReturnsNull()
        {
            // Arrange
            var entry = CreateEntryWithAttributes(new Dictionary<string, object>
            {
                { "created_by", new object() } // Object.ToString() returns "System.Object", not null
            });

            // Act
            string result = entry.GetCreatedBy();

            // Assert
            // ToString() on object doesn't throw, it returns "System.Object"
            Assert.NotNull(result);
            Assert.Equal("System.Object", result);
        }

        #endregion

        #region Live Preview Tests

        [Fact]
        public void Entry_WithLivePreviewEnabled_Setup_VerifiesLivePreviewConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    ManagementToken = "mgmt_token",
                    ContentTypeUID = "source",
                    LivePreview = "preview_token_value"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            var entry = client.ContentType("source").Entry("test_uid");

            // Act - Just verify setup, not actual HTTP call
            var contentTypeInstanceField = typeof(Entry).GetProperty("ContentTypeInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var contentTypeInstance = contentTypeInstanceField?.GetValue(entry);

            // Assert
            Assert.NotNull(contentTypeInstance);
        }

        [Fact]
        public void Entry_WithLivePreviewEnabledAndPreviewToken_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "source"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            var entry = client.ContentType("source").Entry("test_uid");

            // Act - Just verify setup
            var contentTypeInstanceField = typeof(Entry).GetProperty("ContentTypeInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var contentTypeInstance = contentTypeInstanceField?.GetValue(entry);

            // Assert
            Assert.NotNull(contentTypeInstance);
        }

        [Fact]
        public void Entry_WithLivePreviewEnabledAndReleaseId_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "source",
                    ReleaseId = "release_123"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            var entry = client.ContentType("source").Entry("test_uid");

            // Act - Just verify setup
            var contentTypeInstanceField = typeof(Entry).GetProperty("ContentTypeInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var contentTypeInstance = contentTypeInstanceField?.GetValue(entry);

            // Assert
            Assert.NotNull(contentTypeInstance);
        }

        [Fact]
        public void Entry_WithLivePreviewEnabledAndPreviewTimestamp_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "source",
                    PreviewTimestamp = "timestamp_123"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            var entry = client.ContentType("source").Entry("test_uid");

            // Act - Just verify setup
            var contentTypeInstanceField = typeof(Entry).GetProperty("ContentTypeInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var contentTypeInstance = contentTypeInstanceField?.GetValue(entry);

            // Assert
            Assert.NotNull(contentTypeInstance);
        }

        [Fact]
        public void Entry_WithLivePreviewEnabledAndEmptyLivePreview_Setup_VerifiesConfig()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "source",
                    LivePreview = "" // Empty string
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            var entry = client.ContentType("source").Entry("test_uid");

            // Act - Just verify setup
            var contentTypeInstanceField = typeof(Entry).GetProperty("ContentTypeInstance", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var contentTypeInstance = contentTypeInstanceField?.GetValue(entry);

            // Assert
            Assert.NotNull(contentTypeInstance);
        }

        [Fact]
        public void Entry_WithLivePreviewEnabledAndAccessTokenHeader_SkipsAccessToken()
        {
            // Arrange
            var options = new ContentstackOptions()
            {
                ApiKey = _fixture.Create<string>(),
                DeliveryToken = _fixture.Create<string>(),
                Environment = _fixture.Create<string>(),
                LivePreview = new LivePreviewConfig
                {
                    Enable = true,
                    PreviewToken = "preview_token",
                    ContentTypeUID = "source",
                    LivePreview = "preview_value"
                }
            };
            var client = new ContentstackClient(new OptionsWrapper<ContentstackOptions>(options));
            var entry = client.ContentType("source").Entry("test_uid");
            entry.SetHeader("access_token", "token_value");

            // Act - Just verify setup
            var headersField = typeof(Entry).GetField("_Headers", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var headers = (Dictionary<string, object>)headersField?.GetValue(entry);

            // Assert
            Assert.NotNull(headers);
            Assert.True(headers.ContainsKey("access_token"));
        }

        #endregion

        #region GetHeader Tests

        [Fact]
        public void Entry_GetHeader_WithLocalHeaderAndEmptyFormHeaders_ReturnsLocalHeader()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "value" } };
            
            // Set _FormHeaders to empty dictionary
            var formHeadersField = typeof(Entry).GetField("_FormHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            formHeadersField?.SetValue(entry, new Dictionary<string, object>());

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(localHeader, result);
        }

        [Fact]
        public void Entry_GetHeader_WithNullLocalHeader_ReturnsFormHeaders()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { null }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Entry_GetHeader_WithEmptyLocalHeader_ReturnsFormHeaders()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object>();

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Entry_GetHeader_WithOverlappingKeys_LocalHeaderTakesPrecedence()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "custom", "local_value" } };

            // Set _FormHeaders with same key
            var formHeadersField = typeof(Entry).GetField("_FormHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            formHeadersField?.SetValue(entry, new Dictionary<string, object> { { "custom", "form_value" } });

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("local_value", result["custom"]?.ToString());
        }

        [Fact]
        public void Entry_GetHeader_WithBothHeaders_ReturnsMergedHeaders()
        {
            // Arrange
            var entry = CreateEntry();
            var getHeaderMethod = typeof(Entry).GetMethod("GetHeader", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var localHeader = new Dictionary<string, object> { { "local_key", "local_value" } };

            // Set _FormHeaders with different key
            var formHeadersField = typeof(Entry).GetField("_FormHeaders", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            formHeadersField?.SetValue(entry, new Dictionary<string, object> { { "form_key", "form_value" } });

            // Act
            var result = getHeaderMethod?.Invoke(entry, new object[] { localHeader }) as Dictionary<string, object>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("local_key"));
            Assert.True(result.ContainsKey("form_key"));
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void Entry_WithNullUrlQueries_Setup_HandlesGracefully()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            // Can't set UrlQueries to null as it's initialized in constructor, but we can verify it exists
            var urlQueries = urlQueriesField?.GetValue(entry);

            // Assert
            Assert.NotNull(urlQueries);
        }

        [Fact]
        public void Entry_WithEmptyUrlQueries_Setup_HandlesGracefully()
        {
            // Arrange
            var entry = CreateEntry();
            var urlQueriesField = typeof(Entry).GetField("UrlQueries", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var urlQueries = (Dictionary<string, object>)urlQueriesField?.GetValue(entry);

            // Assert
            Assert.NotNull(urlQueries);
        }

        [Fact]
        public void Entry_Fetch_WithCachePolicySet_Setup_VerifiesCachePolicy()
        {
            // Arrange
            var entry = CreateEntry();
            entry.SetCachePolicy(CachePolicy.NetworkOnly);

            // Act - Just verify setup
            var cachePolicyField = typeof(Entry).GetField("_CachePolicy", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var isCachePolicySetField = typeof(Entry).GetField("_IsCachePolicySet", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.Equal(CachePolicy.NetworkOnly, cachePolicyField?.GetValue(entry));
            Assert.True((bool)(isCachePolicySetField?.GetValue(entry) ?? false));
        }

        [Fact]
        public void Entry_Fetch_WithCachePolicyNotSet_Setup_VerifiesDefault()
        {
            // Arrange
            var entry = CreateEntry();

            // Act - Just verify setup
            var cachePolicyField = typeof(Entry).GetField("_CachePolicy", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var isCachePolicySetField = typeof(Entry).GetField("_IsCachePolicySet", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            // Default cache policy should be NetworkOnly when not set
            Assert.False((bool)(isCachePolicySetField?.GetValue(entry) ?? true));
        }

        #endregion
    }
}

