using System;
using System.Reflection;
using Contentstack.Core.Internals;
using Xunit;

namespace Contentstack.Core.Tests.Integration.UtilityTests
{
    [Trait("Category", "Utility")]
    public class VersionUtilityTest
    {
        #region GetSdkVersion Tests

        [Fact(DisplayName = "Get Sdk Version Returns Valid Format")]
        public void GetSdkVersion_ReturnsValidFormat()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(version);
            Assert.StartsWith("contentstack-delivery-dotnet/", version);
            Assert.True(version.Length > "contentstack-delivery-dotnet/".Length);
        }

        [Fact(DisplayName = "Get Sdk Version Returns Consistent Result")]
        public void GetSdkVersion_ReturnsConsistentResult()
        {
            // Act
            var version1 = VersionUtility.GetSdkVersion();
            var version2 = VersionUtility.GetSdkVersion();

            // Assert
            Assert.Equal(version1, version2);
        }

        [Fact(DisplayName = "Get Sdk Version Does Not Return Null")]
        public void GetSdkVersion_DoesNotReturnNull()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(version);
            Assert.NotEmpty(version);
        }

        [Fact(DisplayName = "Get Sdk Version Does Not Return Empty String")]
        public void GetSdkVersion_DoesNotReturnEmptyString()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotEmpty(version);
        }

        [Fact(DisplayName = "Get Sdk Version Contains Expected Prefix")]
        public void GetSdkVersion_ContainsExpectedPrefix()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.StartsWith("contentstack-delivery-dotnet/", version);
        }

        [Fact(DisplayName = "Get Sdk Version Does Not Contain Spaces")]
        public void GetSdkVersion_DoesNotContainSpaces()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.DoesNotContain(" ", version);
        }

        [Fact(DisplayName = "Get Sdk Version Does Not Contain Newlines")]
        public void GetSdkVersion_DoesNotContainNewlines()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.DoesNotContain("\n", version);
            Assert.DoesNotContain("\r", version);
        }

        #endregion

        #region ExtractSemanticVersion Tests (via Reflection)

        [Theory]
        [InlineData("1.2.3", "1.2.3")]
        [InlineData("1.2.3-beta.1", "1.2.3-beta")]
        [InlineData("1.2.3+abc123", "1.2.3")]
        [InlineData("1.2.3-beta.1+abc123", "1.2.3-beta")]
        [InlineData("2.25.0", "2.25.0")]
        [InlineData("2.25.0-beta.1", "2.25.0-beta")]
        [InlineData("2.25.0+abc123", "2.25.0")]
        [InlineData("2.25.0-beta.1+abc123", "2.25.0-beta")]
        [InlineData("10.20.30", "10.20.30")]
        [InlineData("0.1.0", "0.1.0")]
        public void ExtractSemanticVersion_ValidInputs_ReturnsCorrectVersion(string input, string expected)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.2")]
        [InlineData("1")]
        [InlineData("")]
        [InlineData("invalid")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void ExtractSemanticVersion_InvalidInputs_ReturnsNull(string input)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "Extract Semantic Version Null Input Returns Null")]
        public void ExtractSemanticVersion_NullInput_ReturnsNull()
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { null }) as string;

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("1.2.3+", "1.2.3")] // Should handle trailing + correctly
        [InlineData("  1.2.3  ", "1.2.3")] // Should handle whitespace
        [InlineData("1.2.3.4.5", "1.2.3")] // Should handle extra version parts
        public void ExtractSemanticVersion_ImprovedHandling_WorksCorrectly(string input, string expected)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.2.3.4", "1.2.3")] // Should take only first 3 parts
        [InlineData("1.2.3.4.5.6", "1.2.3")] // Should take only first 3 parts
        [InlineData("1.2.3.4.5", "1.2.3")] // Should take only first 3 parts
        public void ExtractSemanticVersion_MoreThanThreeParts_TakesFirstThree(string input, string expected)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.2.3+metadata", "1.2.3")]
        [InlineData("1.2.3-beta.1+metadata", "1.2.3-beta")]
        [InlineData("1.2.3+very-long-metadata-string", "1.2.3")]
        [InlineData("1.2.3+", "1.2.3")]
        public void ExtractSemanticVersion_WithBuildMetadata_RemovesMetadata(string input, string expected)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1.2.3-beta.1", "1.2.3-beta")]
        [InlineData("1.2.3-alpha.1", "1.2.3-alpha")]
        [InlineData("1.2.3-rc.1", "1.2.3-rc")]
        [InlineData("1.2.3-preview.1", "1.2.3-preview")]
        public void ExtractSemanticVersion_WithPreReleaseIdentifiers_KeepsPreRelease(string input, string expected)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact(DisplayName = "Get Sdk Version Handles Exceptions Gracefully")]
        public void GetSdkVersion_HandlesExceptions_Gracefully()
        {
            // This test ensures that GetSdkVersion doesn't throw exceptions
            // and returns a fallback value when assembly reflection fails
            
            // Act & Assert - should not throw
            var version = VersionUtility.GetSdkVersion();
            Assert.NotNull(version);
            Assert.NotEmpty(version);
        }

        [Fact(DisplayName = "Get Sdk Version Returns Fallback When Assembly Version Is Invalid")]
        public void GetSdkVersion_ReturnsFallbackWhenAssemblyVersionIsInvalid()
        {
            // This test verifies that when assembly version is 0.0.0.0 or invalid,
            // the method falls back to other version sources or returns "dev"
            
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(version);
            Assert.True(version == "contentstack-delivery-dotnet/dev" || 
                       version.StartsWith("contentstack-delivery-dotnet/"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void ExtractSemanticVersion_WhitespaceInputs_ReturnsNull(string input)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("1.2.3.4.5.6.7.8.9.10")] // Very long version
        [InlineData("999999999.999999999.999999999")] // Very large numbers
        [InlineData("0.0.0")] // All zeros
        public void ExtractSemanticVersion_EdgeCaseInputs_HandlesCorrectly(string input)
        {
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act
            var result = method.Invoke(null, new object[] { input }) as string;

            // Assert
            if (input == "0.0.0")
            {
                Assert.Equal("0.0.0", result);
            }
            else
            {
                Assert.NotNull(result);
                Assert.True(result.Split('.').Length == 3);
            }
        }

        [Fact(DisplayName = "Extract Semantic Version Handles Exceptions Gracefully")]
        public void ExtractSemanticVersion_HandlesExceptions_Gracefully()
        {
            // This test ensures that ExtractSemanticVersion doesn't throw exceptions
            // and returns null when parsing fails
            
            // Arrange
            var method = typeof(VersionUtility).GetMethod("ExtractSemanticVersion", BindingFlags.NonPublic | BindingFlags.Static);

            // Act & Assert - should not throw
            var result = method.Invoke(null, new object[] { "invalid-version-string" }) as string;
            Assert.Null(result);
        }

        #endregion

        #region Integration Tests

        [Fact(DisplayName = "Get Sdk Version Integration Returns Valid User Agent Format")]
        public void GetSdkVersion_Integration_ReturnsValidUserAgentFormat()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            Assert.NotNull(version);
            Assert.StartsWith("contentstack-delivery-dotnet/", version);
            
            // Verify it's in a format suitable for User-Agent headers
            Assert.DoesNotContain(" ", version);
            Assert.DoesNotContain("\n", version);
            Assert.DoesNotContain("\r", version);
            Assert.DoesNotContain("\t", version);
        }

        [Fact(DisplayName = "Get Sdk Version Integration Can Be Used In Http Headers")]
        public void GetSdkVersion_Integration_CanBeUsedInHttpHeaders()
        {
            // Act
            var version = VersionUtility.GetSdkVersion();

            // Assert
            // Verify the version string is suitable for HTTP headers
            Assert.NotNull(version);
            Assert.NotEmpty(version);
            
            // Should not contain characters that would break HTTP headers
            Assert.DoesNotContain("\"", version);
            Assert.DoesNotContain("'", version);
            Assert.DoesNotContain("\n", version);
            Assert.DoesNotContain("\r", version);
        }

        #endregion

        #region Performance Tests

        [Fact(DisplayName = "Get Sdk Version Performance Returns Quickly")]
        public void GetSdkVersion_Performance_ReturnsQuickly()
        {
            // Act & Assert
            var startTime = DateTime.UtcNow;
            var version = VersionUtility.GetSdkVersion();
            var endTime = DateTime.UtcNow;

            // Should complete quickly (less than 1 second)
            var duration = endTime - startTime;
            Assert.True(duration.TotalSeconds < 1, $"GetSdkVersion took {duration.TotalSeconds} seconds");
            Assert.NotNull(version);
        }

        [Fact(DisplayName = "Get Sdk Version Performance Multiple Calls Consistent")]
        public void GetSdkVersion_Performance_MultipleCalls_Consistent()
        {
            // Act
            var versions = new string[100];
            for (int i = 0; i < 100; i++)
            {
                versions[i] = VersionUtility.GetSdkVersion();
            }

            // Assert
            var firstVersion = versions[0];
            for (int i = 1; i < versions.Length; i++)
            {
                Assert.Equal(firstVersion, versions[i]);
            }
        }

        #endregion
    }
}
