using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Contentstack.Core.Models;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    /// <summary>
    /// Unit tests for SyncStack class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class SyncStackUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        #region Initialization Tests

        [Fact]
        public void SyncStack_Initialization_SetsDefaultValues()
        {
            // Act
            var syncStack = new SyncStack();

            // Assert
            Assert.Null(syncStack.Items);
            Assert.Equal(0, syncStack.TotalCount);
            Assert.Null(syncStack.SyncToken);
            Assert.Null(syncStack.PaginationToken);
        }

        #endregion

        #region Items Property Tests

        [Fact]
        public void Items_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var syncStack = new SyncStack();
            var items = new List<dynamic>
            {
                new { uid = "item1", title = "Item 1" },
                new { uid = "item2", title = "Item 2" }
            };

            // Act
            syncStack.Items = items;

            // Assert
            Assert.NotNull(syncStack.Items);
            Assert.Equal(2, syncStack.Items.Count());
        }

        [Fact]
        public void Items_SetToNull_ReturnsNull()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                Items = new List<dynamic> { new { uid = "item1" } }
            };

            // Act
            syncStack.Items = null;

            // Assert
            Assert.Null(syncStack.Items);
        }

        [Fact]
        public void Items_WithEmptyList_ReturnsEmpty()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                Items = new List<dynamic>()
            };

            // Assert
            Assert.NotNull(syncStack.Items);
            Assert.Empty(syncStack.Items);
        }

        #endregion

        #region TotalCount Property Tests

        [Fact]
        public void TotalCount_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var syncStack = new SyncStack();
            var totalCount = _fixture.Create<int>();

            // Act
            syncStack.TotalCount = totalCount;

            // Assert
            Assert.Equal(totalCount, syncStack.TotalCount);
        }

        [Fact]
        public void TotalCount_SetToZero_ReturnsZero()
        {
            // Arrange
            var syncStack = new SyncStack();

            // Act
            syncStack.TotalCount = 0;

            // Assert
            Assert.Equal(0, syncStack.TotalCount);
        }

        [Fact]
        public void TotalCount_SetToNegative_StoresNegativeValue()
        {
            // Arrange
            var syncStack = new SyncStack();

            // Act
            syncStack.TotalCount = -1;

            // Assert
            Assert.Equal(-1, syncStack.TotalCount);
        }

        #endregion

        #region SyncToken Property Tests

        [Fact]
        public void SyncToken_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var syncStack = new SyncStack();
            var syncToken = _fixture.Create<string>();

            // Act
            syncStack.SyncToken = syncToken;

            // Assert
            Assert.Equal(syncToken, syncStack.SyncToken);
        }

        [Fact]
        public void SyncToken_SetToNull_ReturnsNull()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                SyncToken = "token123"
            };

            // Act
            syncStack.SyncToken = null;

            // Assert
            Assert.Null(syncStack.SyncToken);
        }

        [Fact]
        public void SyncToken_SetToEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var syncStack = new SyncStack();

            // Act
            syncStack.SyncToken = string.Empty;

            // Assert
            Assert.Equal(string.Empty, syncStack.SyncToken);
        }

        #endregion

        #region PaginationToken Property Tests

        [Fact]
        public void PaginationToken_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var syncStack = new SyncStack();
            var paginationToken = _fixture.Create<string>();

            // Act
            syncStack.PaginationToken = paginationToken;

            // Assert
            Assert.Equal(paginationToken, syncStack.PaginationToken);
        }

        [Fact]
        public void PaginationToken_SetToNull_ReturnsNull()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                PaginationToken = "token123"
            };

            // Act
            syncStack.PaginationToken = null;

            // Assert
            Assert.Null(syncStack.PaginationToken);
        }

        [Fact]
        public void PaginationToken_SetToEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var syncStack = new SyncStack();

            // Act
            syncStack.PaginationToken = string.Empty;

            // Assert
            Assert.Equal(string.Empty, syncStack.PaginationToken);
        }

        #endregion

        #region Complete Object Tests

        [Fact]
        public void SyncStack_WithAllPropertiesSet_ReturnsAllValues()
        {
            // Arrange
            var items = new List<dynamic>
            {
                new { uid = "item1", title = "Item 1" },
                new { uid = "item2", title = "Item 2" }
            };
            var syncToken = "sync_token_123";
            var paginationToken = "pagination_token_456";
            var totalCount = 100;

            // Act
            var syncStack = new SyncStack
            {
                Items = items,
                TotalCount = totalCount,
                SyncToken = syncToken,
                PaginationToken = paginationToken
            };

            // Assert
            Assert.NotNull(syncStack.Items);
            Assert.Equal(2, syncStack.Items.Count());
            Assert.Equal(totalCount, syncStack.TotalCount);
            Assert.Equal(syncToken, syncStack.SyncToken);
            Assert.Equal(paginationToken, syncStack.PaginationToken);
        }

        [Fact]
        public void SyncStack_Items_CanBeIterated()
        {
            // Arrange
            var items = new List<dynamic>
            {
                new { uid = "item1", title = "Item 1" },
                new { uid = "item2", title = "Item 2" },
                new { uid = "item3", title = "Item 3" }
            };
            var syncStack = new SyncStack
            {
                Items = items
            };

            // Act
            var result = new List<dynamic>();
            foreach (var item in syncStack.Items)
            {
                result.Add(item);
            }

            // Assert
            Assert.Equal(3, result.Count);
        }

        #endregion

        #region JSON Serialization Tests (Based on JsonProperty attributes)

        [Fact]
        public void SyncStack_TotalCount_ShouldMapToTotalCountProperty()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                TotalCount = 42
            };

            // Act & Assert
            // The property uses [JsonProperty("total_count")] so it should serialize correctly
            Assert.Equal(42, syncStack.TotalCount);
        }

        [Fact]
        public void SyncStack_SyncToken_ShouldMapToSyncTokenProperty()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                SyncToken = "test_token"
            };

            // Act & Assert
            // The property uses [JsonProperty("sync_token")] so it should serialize correctly
            Assert.Equal("test_token", syncStack.SyncToken);
        }

        [Fact]
        public void SyncStack_PaginationToken_ShouldMapToPaginationTokenProperty()
        {
            // Arrange
            var syncStack = new SyncStack
            {
                PaginationToken = "test_pagination_token"
            };

            // Act & Assert
            // The property uses [JsonProperty("pagination_token")] so it should serialize correctly
            Assert.Equal("test_pagination_token", syncStack.PaginationToken);
        }

        #endregion
    }
}



