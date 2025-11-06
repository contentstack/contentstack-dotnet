using System;
using System.Linq;
using Contentstack.Core;
using Xunit;

namespace Contentstack.Core.Tests.UnitTests
{
    public class CSJsonConverterAttributeUnitTests
    {
        [Fact]
        public void CSJsonConverterAttribute_Constructor_WithName_SetsName()
        {
            // Arrange
            var name = "TestConverter";

            // Act
            var attribute = new CSJsonConverterAttribute(name);

            // Assert
            Assert.Equal(name, attribute.Name);
            Assert.True(attribute.IsAutoloadEnable);
        }

        [Fact]
        public void CSJsonConverterAttribute_Constructor_WithNameAndAutoload_SetsBoth()
        {
            // Arrange
            var name = "TestConverter";
            var isAutoloadEnable = false;

            // Act
            var attribute = new CSJsonConverterAttribute(name, isAutoloadEnable);

            // Assert
            Assert.Equal(name, attribute.Name);
            Assert.Equal(isAutoloadEnable, attribute.IsAutoloadEnable);
        }

        [Fact]
        public void CSJsonConverterAttribute_Constructor_WithAutoloadTrue_SetsAutoloadEnable()
        {
            // Arrange
            var name = "TestConverter";
            var isAutoloadEnable = true;

            // Act
            var attribute = new CSJsonConverterAttribute(name, isAutoloadEnable);

            // Assert
            Assert.True(attribute.IsAutoloadEnable);
        }

        [Fact]
        public void CSJsonConverterAttribute_Constructor_WithAutoloadFalse_SetsAutoloadDisable()
        {
            // Arrange
            var name = "TestConverter";
            var isAutoloadEnable = false;

            // Act
            var attribute = new CSJsonConverterAttribute(name, isAutoloadEnable);

            // Assert
            Assert.False(attribute.IsAutoloadEnable);
        }

        [Fact]
        public void GetCustomAttribute_WithValidType_ReturnsTypes()
        {
            // Arrange
            var attributeType = typeof(CSJsonConverterAttribute);

            // Act
            var result = CSJsonConverterAttribute.GetCustomAttribute(attributeType);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<System.Collections.Generic.IEnumerable<Type>>(result);
        }

        [Fact]
        public void GetCustomAttribute_MultipleCalls_ReturnsCachedResult()
        {
            // Arrange
            var attributeType = typeof(CSJsonConverterAttribute);

            // Act
            var result1 = CSJsonConverterAttribute.GetCustomAttribute(attributeType);
            var result2 = CSJsonConverterAttribute.GetCustomAttribute(attributeType);

            // Assert
            Assert.Equal(result1.ToArray(), result2.ToArray());
        }
    }
}


