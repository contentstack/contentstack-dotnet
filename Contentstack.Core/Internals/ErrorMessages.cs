using System;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Contains all error message constants used throughout the SDK
    /// </summary>
    internal static class ErrorMessages
    {
        // Query and Filter related errors
        public const string QueryFilterError = "Please provide valid params.";
        public const string InvalidParamsError = "Invalid parameters provided. {0}";

        // Asset related errors
        public const string AssetJsonConversionError = "Failed to convert asset JSON. Please check the asset format and data integrity.";
        public const string AssetProcessingError = "An error occurred while processing the asset. {0}";
        public const string AssetLibraryRequestError = "Exception in {0}: {1}\nStackTrace: {2}";

        // Entry related errors
        public const string EntryProcessingError = "An error occurred while processing the entry. {0}";
        public const string EntryUidRequired = "Please set entry uid.";
        public const string EntryNotFoundInCache = "Entry is not present in cache";

        // Global Field related errors
        public const string GlobalFieldIdNullError = "GlobalFieldId required. This value cannot be null or empty, define it in your configuration.";
        public const string GlobalFieldProcessingError = "An error occurred while processing the globalField. {0}";
        public const string GlobalFieldQueryError = "Global field query failed. Check your query syntax and field schema before retrying.";

        // Live Preview related errors
        public const string LivePreviewTokenMissing = "Live Preview token missing. Add either a PreviewToken or a ManagementToken in the LivePreviewConfig.";

        // Client Request related errors
        public const string ContentstackClientRequestError = "Contentstack client request failed. Check your network settings or request parameters and try again: {0}";
        public const string ContentstackSyncRequestError = "An error occurred while processing the Contentstack client request: {0}";

        // Taxonomy related errors
        public const string TaxonomyProcessingError = "An error occurred while processing the taxonomy operation: {0}";

        // Content Type related errors
        public const string ContentTypeProcessingError = "Content type processing failed. Verify the schema and ensure all required fields are configured.";

        // Authentication and Configuration errors
        public const string StackApiKeyRequired = "Stack api key can not be null.";
        public const string AccessTokenRequired = "Access token can not be null.";
        public const string EnvironmentRequired = "Environment can not be null.";
        public const string AuthenticationNotPresent = "Authentication Not present.";
        public const string ContentTypeNameRequired = "Please set contentType name.";

        // JSON and Parsing errors
        public const string InvalidJsonFormat = "Please provide valid JSON.";
        public const string ParsingError = "Parsing Error.";

        // Network and Server errors
        public const string NoConnectionError = "Connection error";
        public const string ServerError = "Server interaction went wrong, Please try again.";
        public const string NetworkUnavailable = "Network not available.";
        public const string DefaultError = "Oops! Something went wrong. Please try again.";

        // Cache related errors
        public const string SavingNetworkCallResponseForCache = "Error while saving network call response.";

        // Initialization errors
        public const string ContentstackDefaultMethodNotCalled = "You must called Contentstack.stack() first";

        // Helper method to format exception details
        public static string FormatExceptionDetails(Exception ex)
        {
            return string.Format("Exception: {0}\nSource: {1}\nStackTrace: {2}",
                ex.Message,
                ex.Source ?? "Unknown",
                ex.StackTrace ?? "No stack trace available");
        }
    }
}