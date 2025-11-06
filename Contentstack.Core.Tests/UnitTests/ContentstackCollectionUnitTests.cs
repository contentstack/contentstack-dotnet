using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Contentstack.Core.Models;
using Xunit;

namespace Contentstack.Core.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for ContentstackCollection class - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class ContentstackCollectionUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        #region Initialization Tests

        [Fact]
        public void ContentstackCollection_Initialization_SetsDefaultValues()
        {
            // Act
            var collection = new ContentstackCollection<string>();

            // Assert
            Assert.Equal(0, collection.Skip);
            Assert.Equal(0, collection.Limit);
            Assert.Equal(0, collection.Count);
            Assert.Null(collection.Items);
        }

        [Fact]
        public void ContentstackCollection_Initialization_WithItems_SetsItems()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };

            // Act
            var collection = new ContentstackCollection<string>
            {
                Items = items
            };

            // Assert
            Assert.NotNull(collection.Items);
            Assert.Equal(3, collection.Items.Count());
        }

        #endregion

        #region Properties Tests

        [Fact]
        public void Skip_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var collection = new ContentstackCollection<string>();
            var skipValue = _fixture.Create<int>();

            // Act
            collection.Skip = skipValue;

            // Assert
            Assert.Equal(skipValue, collection.Skip);
        }

        [Fact]
        public void Limit_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var collection = new ContentstackCollection<string>();
            var limitValue = _fixture.Create<int>();

            // Act
            collection.Limit = limitValue;

            // Assert
            Assert.Equal(limitValue, collection.Limit);
        }

        [Fact]
        public void Count_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var collection = new ContentstackCollection<string>();
            var countValue = _fixture.Create<int>();

            // Act
            collection.Count = countValue;

            // Assert
            Assert.Equal(countValue, collection.Count);
        }

        [Fact]
        public void Items_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var collection = new ContentstackCollection<string>();
            var items = new List<string> { "item1", "item2" };

            // Act
            collection.Items = items;

            // Assert
            Assert.NotNull(collection.Items);
            Assert.Equal(items, collection.Items);
        }

        [Fact]
        public void Items_SetToNull_ReturnsNull()
        {
            // Arrange
            var collection = new ContentstackCollection<string>
            {
                Items = new List<string> { "item1" }
            };

            // Act
            collection.Items = null;

            // Assert
            Assert.Null(collection.Items);
        }

        #endregion

        #region GetEnumerator Tests

        [Fact]
        public void GetEnumerator_WithItems_ReturnsEnumerator()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new ContentstackCollection<string>
            {
                Items = items
            };

            // Act
            var enumerator = collection.GetEnumerator();

            // Assert
            Assert.NotNull(enumerator);
        }

        [Fact]
        public void GetEnumerator_WithItems_IteratesThroughAllItems()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new ContentstackCollection<string>
            {
                Items = items
            };

            // Act
            var result = new List<string>();
            foreach (var item in collection)
            {
                result.Add(item);
            }

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("item1", result[0]);
            Assert.Equal("item2", result[1]);
            Assert.Equal("item3", result[2]);
        }

        [Fact]
        public void GetEnumerator_WithNullItems_ThrowsException()
        {
            // Arrange
            var collection = new ContentstackCollection<string>
            {
                Items = null
            };

            // Act & Assert
            Assert.Throws<NullReferenceException>(() =>
            {
                foreach (var item in collection)
                {
                    // This should throw
                }
            });
        }

        [Fact]
        public void GetEnumerator_WithEmptyItems_ReturnsEmptyEnumerator()
        {
            // Arrange
            var collection = new ContentstackCollection<string>
            {
                Items = new List<string>()
            };

            // Act
            var result = new List<string>();
            foreach (var item in collection)
            {
                result.Add(item);
            }

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region IEnumerable.GetEnumerator Tests

        [Fact]
        public void IEnumerable_GetEnumerator_ReturnsEnumerator()
        {
            // Arrange
            var items = new List<string> { "item1", "item2" };
            var collection = new ContentstackCollection<string>
            {
                Items = items
            };
            IEnumerable enumerable = collection;

            // Act
            var enumerator = enumerable.GetEnumerator();

            // Assert
            Assert.NotNull(enumerator);
        }

        [Fact]
        public void IEnumerable_GetEnumerator_IteratesThroughAllItems()
        {
            // Arrange
            var items = new List<string> { "item1", "item2", "item3" };
            var collection = new ContentstackCollection<string>
            {
                Items = items
            };
            IEnumerable enumerable = collection;

            // Act
            var result = new List<object>();
            foreach (var item in enumerable)
            {
                result.Add(item);
            }

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("item1", result[0]);
            Assert.Equal("item2", result[1]);
            Assert.Equal("item3", result[2]);
        }

        #endregion

        #region Complex Type Tests

        [Fact]
        public void ContentstackCollection_WithComplexType_WorksCorrectly()
        {
            // Arrange
            var items = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "Test1" },
                new TestModel { Id = 2, Name = "Test2" }
            };

            // Act
            var collection = new ContentstackCollection<TestModel>
            {
                Items = items,
                Skip = 0,
                Limit = 10,
                Count = 2
            };

            // Assert
            Assert.Equal(2, collection.Count);
            Assert.Equal(0, collection.Skip);
            Assert.Equal(10, collection.Limit);
            Assert.Equal(2, collection.Items.Count());
        }

        [Fact]
        public void ContentstackCollection_WithComplexType_IteratesCorrectly()
        {
            // Arrange
            var items = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "Test1" },
                new TestModel { Id = 2, Name = "Test2" }
            };
            var collection = new ContentstackCollection<TestModel>
            {
                Items = items
            };

            // Act
            var result = new List<TestModel>();
            foreach (var item in collection)
            {
                result.Add(item);
            }

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Test1", result[0].Name);
            Assert.Equal(2, result[1].Id);
            Assert.Equal("Test2", result[1].Name);
        }

        #endregion
    }

    /// <summary>
    /// Test model for complex type testing
    /// </summary>
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}


