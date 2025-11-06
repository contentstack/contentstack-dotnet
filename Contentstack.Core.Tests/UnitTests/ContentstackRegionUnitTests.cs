using System;
using System.Linq;
using AutoFixture;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for ContentstackRegion enum - uses mocks and AutoFixture, no real API calls
    /// </summary>
    public class ContentstackRegionUnitTests
    {
        private readonly IFixture _fixture = new Fixture();

        #region Enum Values Tests

        [Fact]
        public void ContentstackRegion_US_HasCorrectValue()
        {
            // Act
            var region = ContentstackRegion.US;

            // Assert
            Assert.Equal(0, (int)region);
        }

        [Fact]
        public void ContentstackRegion_EU_HasCorrectValue()
        {
            // Act
            var region = ContentstackRegion.EU;

            // Assert
            Assert.Equal(1, (int)region);
        }

        [Fact]
        public void ContentstackRegion_AZURE_EU_HasCorrectValue()
        {
            // Act
            var region = ContentstackRegion.AZURE_EU;

            // Assert
            Assert.Equal(2, (int)region);
        }

        [Fact]
        public void ContentstackRegion_AZURE_NA_HasCorrectValue()
        {
            // Act
            var region = ContentstackRegion.AZURE_NA;

            // Assert
            Assert.Equal(3, (int)region);
        }

        [Fact]
        public void ContentstackRegion_GCP_NA_HasCorrectValue()
        {
            // Act
            var region = ContentstackRegion.GCP_NA;

            // Assert
            Assert.Equal(4, (int)region);
        }

        [Fact]
        public void ContentstackRegion_AU_HasCorrectValue()
        {
            // Act
            var region = ContentstackRegion.AU;

            // Assert
            Assert.Equal(5, (int)region);
        }

        #endregion

        #region Enum Parsing Tests

        [Fact]
        public void ContentstackRegion_Parse_WithValidString_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<ContentstackRegion>("US");

            // Assert
            Assert.Equal(ContentstackRegion.US, result);
        }

        [Fact]
        public void ContentstackRegion_Parse_WithEU_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<ContentstackRegion>("EU");

            // Assert
            Assert.Equal(ContentstackRegion.EU, result);
        }

        [Fact]
        public void ContentstackRegion_Parse_WithAZURE_EU_ReturnsCorrectValue()
        {
            // Act
            var result = Enum.Parse<ContentstackRegion>("AZURE_EU");

            // Assert
            Assert.Equal(ContentstackRegion.AZURE_EU, result);
        }

        [Fact]
        public void ContentstackRegion_Parse_WithInvalidString_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Enum.Parse<ContentstackRegion>("INVALID"));
        }

        [Fact]
        public void ContentstackRegion_TryParse_WithValidString_ReturnsTrue()
        {
            // Act
            var result = Enum.TryParse<ContentstackRegion>("US", out var region);

            // Assert
            Assert.True(result);
            Assert.Equal(ContentstackRegion.US, region);
        }

        [Fact]
        public void ContentstackRegion_TryParse_WithInvalidString_ReturnsFalse()
        {
            // Act
            var result = Enum.TryParse<ContentstackRegion>("INVALID", out var region);

            // Assert
            Assert.False(result);
            Assert.Equal(default(ContentstackRegion), region);
        }

        #endregion

        #region Enum Usage Tests

        [Fact]
        public void ContentstackRegion_CanBeUsedInSwitchStatement()
        {
            // Arrange
            var region = ContentstackRegion.US;
            var result = "";

            // Act
            switch (region)
            {
                case ContentstackRegion.US:
                    result = "US";
                    break;
                case ContentstackRegion.EU:
                    result = "EU";
                    break;
                case ContentstackRegion.AZURE_EU:
                    result = "AZURE_EU";
                    break;
                case ContentstackRegion.AZURE_NA:
                    result = "AZURE_NA";
                    break;
                case ContentstackRegion.GCP_NA:
                    result = "GCP_NA";
                    break;
                case ContentstackRegion.AU:
                    result = "AU";
                    break;
            }

            // Assert
            Assert.Equal("US", result);
        }

        [Fact]
        public void ContentstackRegion_GetValues_ReturnsAllRegions()
        {
            // Act
            var values = Enum.GetValues(typeof(ContentstackRegion));

            // Assert
            Assert.NotNull(values);
            Assert.Equal(6, values.Length);
            var regionArray = values.Cast<ContentstackRegion>().ToArray();
            Assert.Contains(ContentstackRegion.US, regionArray);
            Assert.Contains(ContentstackRegion.EU, regionArray);
            Assert.Contains(ContentstackRegion.AZURE_EU, regionArray);
            Assert.Contains(ContentstackRegion.AZURE_NA, regionArray);
            Assert.Contains(ContentstackRegion.GCP_NA, regionArray);
            Assert.Contains(ContentstackRegion.AU, regionArray);
        }

        #endregion
    }
}

