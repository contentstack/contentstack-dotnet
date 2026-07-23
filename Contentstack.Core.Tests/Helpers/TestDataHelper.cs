using System;
using System.Configuration;

namespace Contentstack.Core.Tests.Helpers
{
    /// <summary>
    /// Helper class to retrieve test data from app.config
    /// Ensures no hardcoded values in tests - all data comes from configuration
    /// </summary>
    public static class TestDataHelper
    {
        static TestDataHelper()
        {
            // Initialize configuration similar to StackConfig
            var currentConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var assemblyConfiguration = ConfigurationManager.OpenExeConfiguration(
                new Uri(uriString: typeof(TestDataHelper).Assembly.Location).LocalPath
            );
            
            if (assemblyConfiguration.HasFile && 
                string.Compare(assemblyConfiguration.FilePath, currentConfiguration.FilePath, true) != 0)
            {
                assemblyConfiguration.SaveAs(currentConfiguration.FilePath);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
        #region Entry UIDs
        
        /// <summary>
        /// Gets the UID for a complex entry with multiple field types, deep references, etc.
        /// </summary>
        public static string ComplexEntryUid => 
            GetRequiredConfig("COMPLEX_ENTRY_UID");
        
        /// <summary>
        /// Gets the UID for a medium complexity entry
        /// </summary>
        public static string MediumEntryUid => 
            GetRequiredConfig("MEDIUM_ENTRY_UID");
        
        /// <summary>
        /// Gets the UID for a simple entry with basic fields
        /// </summary>
        public static string SimpleEntryUid => 
            GetRequiredConfig("SIMPLE_ENTRY_UID");
        
        /// <summary>
        /// Gets the UID for a self-referencing entry
        /// </summary>
        public static string SelfRefEntryUid => 
            GetRequiredConfig("SELF_REF_ENTRY_UID");
        
        /// <summary>
        /// Gets the UID for an entry with complex modular blocks
        /// </summary>
        public static string ComplexBlocksEntryUid => 
            GetRequiredConfig("COMPLEX_BLOCKS_ENTRY_UID");
        
        #endregion

        #region Content Type UIDs
        
        /// <summary>
        /// Gets the UID for a complex content type with all field types
        /// </summary>
        public static string ComplexContentTypeUid => 
            GetRequiredConfig("COMPLEX_CONTENT_TYPE_UID");
        
        /// <summary>
        /// Gets the UID for a medium complexity content type
        /// </summary>
        public static string MediumContentTypeUid => 
            GetRequiredConfig("MEDIUM_CONTENT_TYPE_UID");
        
        /// <summary>
        /// Gets the UID for a simple content type
        /// </summary>
        public static string SimpleContentTypeUid => 
            GetRequiredConfig("SIMPLE_CONTENT_TYPE_UID");
        
        /// <summary>
        /// Gets the UID for a self-referencing content type
        /// </summary>
        public static string SelfRefContentTypeUid => 
            GetRequiredConfig("SELF_REF_CONTENT_TYPE_UID");
        
        #endregion

        #region Asset UIDs
        
        /// <summary>
        /// Gets the UID for an image asset (for image transformation tests)
        /// </summary>
        public static string ImageAssetUid => 
            GetRequiredConfig("IMAGE_ASSET_UID");
        
        #endregion

        #region Variant UIDs
        
        /// <summary>
        /// Gets the UID for a variant
        /// </summary>
        public static string VariantUid => 
            GetRequiredConfig("VARIANT_UID");
        
        #endregion

        #region Branch
        
        /// <summary>
        /// Gets the branch UID (defaults to "main" if not specified)
        /// </summary>
        public static string BranchUid => 
            GetOptionalConfig("BRANCH_UID", "main");
        
        #endregion

        #region Taxonomy (Entry Query Operators — existing)

        /// <summary>
        /// Gets the taxonomy term for USA state (e.g., "california").
        /// Used by entry-level taxonomy query operator tests (Taxonomies()).
        /// </summary>
        public static string TaxUsaState =>
            GetRequiredConfig("TAX_USA_STATE");

        /// <summary>
        /// Gets the taxonomy term for India state (e.g., "maharashtra").
        /// Used by entry-level taxonomy query operator tests (Taxonomies()).
        /// </summary>
        public static string TaxIndiaState =>
            GetRequiredConfig("TAX_INDIA_STATE");

        #endregion

        #region Taxonomy CDA (new Publishing & Delivery endpoints)

        /// <summary>
        /// Gets the UID of a taxonomy that has been published to the test environment.
        /// Used by Taxonomy CDA endpoint tests (stack.Taxonomy(uid)).
        /// Configure in app.config: <c>&lt;add key="TAXONOMY_CDA_UID" value="your_taxonomy_uid" /&gt;</c>
        /// </summary>
        public static string TaxonomyCdaUid =>
            GetRequiredConfig("TAXONOMY_CDA_UID");

        /// <summary>
        /// Gets the UID of a term within <see cref="TaxonomyCdaUid"/> that has been published
        /// to the test environment. Used for term-level CDA endpoint tests.
        /// Configure in app.config: <c>&lt;add key="TAXONOMY_CDA_TERM_UID" value="your_term_uid" /&gt;</c>
        /// </summary>
        public static string TaxonomyCdaTermUid =>
            GetRequiredConfig("TAXONOMY_CDA_TERM_UID");

        /// <summary>
        /// Gets the locale used for Taxonomy CDA locale-specific tests (e.g., "fr-fr").
        /// The taxonomy must be published in this locale on the test environment.
        /// Configure in app.config: <c>&lt;add key="TAXONOMY_CDA_LOCALE" value="fr-fr" /&gt;</c>
        /// </summary>
        public static string TaxonomyCdaLocale =>
            GetRequiredConfig("TAXONOMY_CDA_LOCALE");

        #endregion

        #region Live Preview
        
        /// <summary>
        /// Gets the preview token for Live Preview tests
        /// </summary>
        public static string PreviewToken => 
            GetOptionalConfig("PREVIEW_TOKEN");
        
        /// <summary>
        /// Gets the Live Preview host
        /// </summary>
        public static string LivePreviewHost => 
            GetOptionalConfig("LIVE_PREVIEW_HOST");
        
        /// <summary>
        /// Gets the management token (for some Live Preview scenarios)
        /// </summary>
        public static string ManagementToken => 
            GetOptionalConfig("MANAGEMENT_TOKEN");
        
        #endregion

        #region Core Configuration
        
        /// <summary>
        /// Gets the Contentstack host
        /// </summary>
        public static string Host => 
            GetRequiredConfig("HOST");
        
        /// <summary>
        /// Gets the API key
        /// </summary>
        public static string ApiKey => 
            GetRequiredConfig("API_KEY");
        
        /// <summary>
        /// Gets the delivery token
        /// </summary>
        public static string DeliveryToken => 
            GetRequiredConfig("DELIVERY_TOKEN");
        
        /// <summary>
        /// Gets the environment name
        /// </summary>
        public static string Environment => 
            GetRequiredConfig("ENVIRONMENT");
        
        #endregion

        #region Helper Methods
        
        /// <summary>
        /// Gets a required configuration value and throws if not found
        /// </summary>
        /// <param name="configKey">Configuration key name</param>
        /// <returns>Configuration value</returns>
        /// <exception cref="InvalidOperationException">Thrown when configuration is missing</exception>
        private static string GetRequiredConfig(string configKey)
        {
            var value = ConfigurationManager.AppSettings[configKey];
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(
                    $"Required configuration '{configKey}' is missing from app.config. " +
                    $"Please ensure all required keys are present in the <appSettings> section.");
            }
            return value;
        }
        
        /// <summary>
        /// Gets an optional configuration value with a default
        /// </summary>
        /// <param name="configKey">Configuration key name</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Configuration value or default</returns>
        private static string GetOptionalConfig(string configKey, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[configKey] ?? defaultValue;
        }
        
        /// <summary>
        /// Validates that all required configuration is present
        /// Call this at the start of test runs to fail fast if config is incomplete
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when configuration validation fails</exception>
        public static void ValidateConfiguration()
        {
            try
            {
                // Test all required configs by accessing them
                var _ = ComplexEntryUid;
                var __ = MediumEntryUid;
                var ___ = SimpleEntryUid;
                var ____ = ComplexContentTypeUid;
                var _____ = MediumContentTypeUid;
                var ______ = SimpleContentTypeUid;
                var _______ = Host;
                var ________ = ApiKey;
                var _________ = DeliveryToken;
                var __________ = Environment;
                var ___________ = ImageAssetUid;
                var ____________ = VariantUid;
                var _____________ = SelfRefContentTypeUid;
                var ______________ = SelfRefEntryUid;
                var _______________ = ComplexBlocksEntryUid;
                var ________________ = TaxUsaState;
                var _________________ = TaxIndiaState;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Configuration validation failed. Please check app.config and ensure all required keys are present. " +
                    "See TEST-SUITE-DOCUMENTATION.md for the complete list of required environment variables.", 
                    ex);
            }
        }
        
        /// <summary>
        /// Checks if Live Preview configuration is available
        /// </summary>
        /// <returns>True if Live Preview can be tested</returns>
        public static bool IsLivePreviewConfigured()
        {
            return !string.IsNullOrEmpty(PreviewToken) && 
                   !string.IsNullOrEmpty(LivePreviewHost);
        }
        
        #endregion
    }
}

