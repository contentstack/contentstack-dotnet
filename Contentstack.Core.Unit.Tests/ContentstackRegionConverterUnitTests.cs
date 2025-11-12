using System;
using System.ComponentModel;
using System.Globalization;
using Contentstack.Core.Configuration;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    public class ContentstackRegionConverterUnitTests
    {
        private readonly ContentstackRegionConverter _converter = new ContentstackRegionConverter();

        [Fact]
        public void CanConvertFrom_String_ReturnsTrue()
        {
            // Act
            var result = _converter.CanConvertFrom(null, typeof(string));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanConvertFrom_NonString_ReturnsFalse()
        {
            // Act
            var result = _converter.CanConvertFrom(null, typeof(int));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ConvertFrom_ValidRegionString_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "US";

            // Act & Assert
            // The converter only parses when nonDigitIndex > 0 (i.e., there's a prefix)
            // For "US", nonDigitIndex is 0, so it doesn't parse and throws
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Fact]
        public void ConvertFrom_ValidRegionStringWithPrefix_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "eu-EU";

            // Act & Assert
            // The converter looks for first letter ('e') which is at index 0
            // nonDigitIndex is 0, which is NOT > 0, so it doesn't parse and throws
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Fact]
        public void ConvertFrom_ValidRegionStringLowerCase_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "us";

            // Act & Assert
            // For "us", nonDigitIndex is 0, so it doesn't parse and throws
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Fact]
        public void ConvertFrom_ValidRegionStringMixedCase_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "Eu";

            // Act & Assert
            // For "Eu", nonDigitIndex is 0, so it doesn't parse and throws
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Fact]
        public void ConvertFrom_InvalidRegionString_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "INVALID";

            // Act & Assert
            // When nonDigitIndex <= 0 or parsing fails, it calls base.ConvertFrom which throws
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Fact]
        public void ConvertFrom_EmptyString_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "";

            // Act & Assert
            // Empty string results in null, which calls base.ConvertFrom and throws
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Fact]
        public void ConvertFrom_Null_ThrowsNotSupportedException()
        {
            // Act & Assert
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, null));
        }

        [Fact]
        public void ConvertFrom_StringWithOnlyDigits_ThrowsNotSupportedException()
        {
            // Arrange
            var regionString = "123";

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => 
                _converter.ConvertFrom(null, CultureInfo.InvariantCulture, regionString));
        }

        [Theory]
        [InlineData("123-US", ContentstackRegion.US)]
        [InlineData("123-EU", ContentstackRegion.EU)]
        [InlineData("123-AZURE_EU", ContentstackRegion.AZURE_EU)]
        [InlineData("123-AZURE_NA", ContentstackRegion.AZURE_NA)]
        [InlineData("123-GCP_NA", ContentstackRegion.GCP_NA)]
        [InlineData("123-AU", ContentstackRegion.AU)]
        public void ConvertFrom_AllRegionsWithNumericPrefix_ReturnsCorrectRegion(string input, ContentstackRegion expected)
        {
            // Act
            // The converter finds first letter which is after the digits
            // nonDigitIndex > 0, so it parses from there
            var result = _converter.ConvertFrom(null, CultureInfo.InvariantCulture, input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}

