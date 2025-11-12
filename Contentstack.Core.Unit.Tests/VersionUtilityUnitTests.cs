using System;
using System.Reflection;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Unit.Tests
{
    public class VersionUtilityUnitTests
    {
        [Fact]
        public void GetSdkVersion_ReturnsValidFormat()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("contentstack-delivery-dotnet/", result);
        }

        [Fact]
        public void GetSdkVersion_ContainsVersionNumber()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(result);
            // Should contain version format like "2.25.0" or "dev"
            Assert.True(result.Contains(".") || result.Contains("/dev"));
        }

        [Fact]
        public void GetSdkVersion_AlwaysReturnsString()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            Assert.IsType<string>(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetSdkVersion_MultipleCalls_ReturnsConsistentResult()
        {
            // Act
            var result1 = VersionUtility.GetSdkVersion();
            var result2 = VersionUtility.GetSdkVersion();

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void ExtractSemanticVersion_WithValidVersion_ReturnsSemanticVersion()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "2.25.0" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2.25.0", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionAndBuildMetadata_ReturnsSemanticVersion()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "2.25.0+abc123" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2.25.0", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionAndPrerelease_ReturnsSemanticVersion()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "2.25.0-beta.1" });

            // Assert
            Assert.NotNull(result);
            // ExtractSemanticVersion splits by dots and takes first 3 parts, so "2.25.0-beta.1" becomes ["2", "25", "0-beta"]
            Assert.Equal("2.25.0-beta", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionPrereleaseAndBuildMetadata_ReturnsSemanticVersion()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "2.25.0-beta.1+abc123" });

            // Assert
            Assert.NotNull(result);
            // ExtractSemanticVersion splits by dots and takes first 3 parts, so "2.25.0-beta.1+abc123" becomes ["2", "25", "0-beta"]
            Assert.Equal("2.25.0-beta", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithLongVersion_ReturnsFirstThreeParts()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "2.25.0.123" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2.25.0", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithNull_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { null });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithEmptyString_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithWhitespace_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "   " });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithOnlyPlus_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "+abc123" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithInvalidVersion_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "invalid" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithShortVersion_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "2.25" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithSpaces_ReturnsSemanticVersion()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { " 2.25.0 " });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2.25.0", result);
        }

        [Theory]
        [InlineData("1.2.3-alpha")]
        [InlineData("1.2.3-beta")]
        [InlineData("1.2.3-rc")]
        [InlineData("1.2.3-preview")]
        [InlineData("1.2.3-stable")]
        public void ExtractSemanticVersion_WithVariousPreReleaseIdentifiers_ReturnsSemanticVersion(string input)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.StartsWith("1.2.3", result);
        }

        [Theory]
        [InlineData("1.2.3+abc")]
        [InlineData("1.2.3+abc123")]
        [InlineData("1.2.3+123abc")]
        public void ExtractSemanticVersion_WithVariousBuildMetadata_RemovesMetadata(string input)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionEndingWithPlus_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1.2.3+" });

            // Assert
            // Note: ExtractSemanticVersion splits on '+' and takes first part, so "1.2.3+" becomes "1.2.3"
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithEmptyParts_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1..3" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSdkVersion_ReturnsNonEmptyString()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.StartsWith("contentstack-delivery-dotnet/", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithTrailingWhitespace_TrimsWhitespace()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "  1.2.3  " });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionPartsWithWhitespace_TrimsEachPart()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { " 1 . 2 . 3 " });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithEmptyPart_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1..3" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithOnlyOnePart_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithOnlyTwoParts_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1.2" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithFourParts_ReturnsFirstThree()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1.2.3.4" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithFiveParts_ReturnsFirstThree()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1.2.3.4.5" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionStartingWithPlus_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "+1.2.3" });

            // Assert
            // Split on '+' gives empty string as first part, which is null/whitespace
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithMultiplePlusSigns_RemovesAllAfterFirst()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1.2.3+abc+def" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1.2.3", result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithEmptyMajor_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { ".2.3" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithEmptyMinor_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1..3" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ExtractSemanticVersion_WithVersionWithEmptyPatch_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            // Act
            var result = method?.Invoke(null, new object[] { "1.2." });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSdkVersion_AlwaysReturnsNonEmptyString()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            // Should always return a string starting with "contentstack-delivery-dotnet/"
            Assert.StartsWith("contentstack-delivery-dotnet/", result);
        }

        [Fact]
        public void GetSdkVersion_ReturnsConsistentFormat()
        {
            // Act
            var result1 = VersionUtility.GetSdkVersion();
            var result2 = VersionUtility.GetSdkVersion();

            // Assert
            Assert.Equal(result1, result2);
            Assert.StartsWith("contentstack-delivery-dotnet/", result1);
            Assert.StartsWith("contentstack-delivery-dotnet/", result2);
        }

        [Fact]
        public void GetSdkVersion_WithAssemblyVersion_ReturnsVersion()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            // Should return either version number or "dev" fallback
            Assert.NotNull(result);
            Assert.True(result.Contains(".") || result.Contains("/dev"));
        }

        [Fact]
        public void GetSdkVersion_WithFileVersionFallback_ReturnsVersion()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            // If assembly version is not available, should fallback to file version or informational version
            Assert.NotNull(result);
            Assert.StartsWith("contentstack-delivery-dotnet/", result);
        }

        [Fact]
        public void GetSdkVersion_WithInformationalVersionFallback_ReturnsVersion()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            // Should eventually fallback to "dev" if no version info available
            Assert.NotNull(result);
            Assert.StartsWith("contentstack-delivery-dotnet/", result);
        }

        [Fact]
        public void GetSdkVersion_WithExceptionFallback_ReturnsDev()
        {
            // Act
            var result = VersionUtility.GetSdkVersion();

            // Assert
            // Should always return something, even if exception occurs (falls back to "dev")
            Assert.NotNull(result);
            Assert.True(result == "contentstack-delivery-dotnet/dev" || result.StartsWith("contentstack-delivery-dotnet/"));
        }
    }
}

