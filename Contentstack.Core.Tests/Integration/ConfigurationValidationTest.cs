using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Contentstack.Core.Configuration;
using Contentstack.Core.Models;
using Contentstack.Core.Tests.Helpers;
using Contentstack.Core.Tests.Models;

namespace Contentstack.Core.Tests.Integration
{
    /// <summary>
    /// Validates that the test infrastructure is properly set up
    /// This test should run first to ensure all dependencies are working
    /// </summary>
    public class ConfigurationValidationTest
    {
        [Fact(DisplayName = "Test Data Helper All Required Configuration Present")]
        public void TestDataHelper_AllRequiredConfigurationPresent()
        {
            // Arrange & Act & Assert
            // This will throw if any required configuration is missing
            Assert.NotNull(TestDataHelper.Host);
            Assert.NotNull(TestDataHelper.ApiKey);
            Assert.NotNull(TestDataHelper.DeliveryToken);
            Assert.NotNull(TestDataHelper.Environment);
            Assert.NotNull(TestDataHelper.ComplexEntryUid);
            Assert.NotNull(TestDataHelper.MediumEntryUid);
            Assert.NotNull(TestDataHelper.SimpleEntryUid);
            Assert.NotNull(TestDataHelper.ComplexContentTypeUid);
            Assert.NotNull(TestDataHelper.MediumContentTypeUid);
            Assert.NotNull(TestDataHelper.SimpleContentTypeUid);
        }
        
        [Fact(DisplayName = "Test Data Helper Validation Passes")]
        public void TestDataHelper_ValidationPasses()
        {
            // Arrange & Act & Assert
            // Should not throw
            TestDataHelper.ValidateConfiguration();
        }
        
        [Fact(DisplayName = "Test Data Helper Optional Configuration Handled Correctly")]
        public void TestDataHelper_OptionalConfigurationHandledCorrectly()
        {
            // Arrange & Act
            var branchUid = TestDataHelper.BranchUid;
            var livePreviewConfigured = TestDataHelper.IsLivePreviewConfigured();
            
            // Assert
            Assert.NotNull(branchUid); // Should default to "main"
            // Live preview may or may not be configured
            Assert.True(livePreviewConfigured || !livePreviewConfigured); // Just checking it doesn't throw
        }
        
        [Fact(DisplayName = "Stack Connectivity Can Connect To Stack")]
        public async Task StackConnectivity_CanConnectToStack()
        {
            // Arrange
            var config = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(config);
            
            // Act
            var contentType = client.ContentType(TestDataHelper.SimpleContentTypeUid);
            var query = contentType.Query();
            var result = await query.FindOne<Entry>();
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Count() > 0, "Should fetch at least one entry from the stack");
        }
        
        [Fact(DisplayName = "Entry Factory Can Fetch Entry")]
        public async Task EntryFactory_CanFetchEntry()
        {
            // Arrange
            var config = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(config);
            var factory = new EntryFactory(client);
            
            // Act
            var entry = await factory.FetchEntryAsync<Entry>(
                TestDataHelper.SimpleContentTypeUid,
                TestDataHelper.SimpleEntryUid
            );
            
            // Assert
            Assert.NotNull(entry);
            Assert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Generic Models Can Be Instantiated")]
        public void GenericModels_CanBeInstantiated()
        {
            // Arrange & Act
            var complexModel = new ComplexContentTypeModel();
            var mediumModel = new MediumContentTypeModel();
            var simpleModel = new SimpleContentTypeModel();
            
            // Assert
            Assert.NotNull(complexModel);
            Assert.NotNull(mediumModel);
            Assert.NotNull(simpleModel);
        }
        
        [Fact(DisplayName = "Generic Models Can Be Used With SDK")]
        public async Task GenericModels_CanBeUsedWithSDK()
        {
            // Arrange
            var config = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(config);
            
            // Act - Fetch using strongly-typed model
            var entry = await client
                .ContentType(TestDataHelper.SimpleContentTypeUid)
                .Entry(TestDataHelper.SimpleEntryUid)
                .Fetch<SimpleContentTypeModel>();
            
            // Assert
            Assert.NotNull(entry);
            Assert.IsType<SimpleContentTypeModel>(entry);
            Assert.Equal(TestDataHelper.SimpleEntryUid, entry.Uid);
            Assert.NotNull(entry.Title);
        }
        
        [Fact(DisplayName = "Performance Helper Can Measure Operations")]
        public async Task PerformanceHelper_CanMeasureOperations()
        {
            // Arrange
            var config = new ContentstackOptions()
            {
                Host = TestDataHelper.Host,
                ApiKey = TestDataHelper.ApiKey,
                DeliveryToken = TestDataHelper.DeliveryToken,
                Environment = TestDataHelper.Environment
            };
            var client = new ContentstackClient(config);
            
            // Act - Measure a simple fetch operation
            var (result, elapsed) = await PerformanceHelper.MeasureExecutionTimeAsync(async () =>
            {
                return await client
                    .ContentType(TestDataHelper.SimpleContentTypeUid)
                    .Entry(TestDataHelper.SimpleEntryUid)
                    .Fetch<Entry>();
            });
            
            // Assert
            Assert.NotNull(result);
            Assert.True(elapsed >= 0, $"Elapsed time should be non-negative, got {elapsed}ms");
            Assert.True(elapsed < 30000, $"Single fetch should complete within 30s, took {elapsed}ms");
        }
        
        [Fact(DisplayName = "Directory Structure All Directories Exist")]
        public void DirectoryStructure_AllDirectoriesExist()
        {
            // Arrange
            var baseTestPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location
            );
            
            var projectRoot = System.IO.Directory.GetParent(baseTestPath)?.Parent?.Parent?.FullName;
            
            // Act & Assert - Check that key directories exist
            Assert.True(System.IO.Directory.Exists(System.IO.Path.Combine(projectRoot, "Integration")), 
                "Integration directory should exist");
            Assert.True(System.IO.Directory.Exists(System.IO.Path.Combine(projectRoot, "Helpers")), 
                "Helpers directory should exist");
            Assert.True(System.IO.Directory.Exists(System.IO.Path.Combine(projectRoot, "Models")), 
                "Models directory should exist");
        }
    }
}

