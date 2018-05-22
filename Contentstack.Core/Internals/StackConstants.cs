using System;

namespace Contentstack.Core.Internals
{
    internal class StackConstants
    {
        public const string GreaterThan = "$gt";
        public const string GreaterThanEqualTo = "$gte";
        public const string LessThan = "$lt";
        public const string LessThanEqualTo = "$lte";
        public const string Where = "$where";
        public const string NotEqualTo = "$ne";
        public const string ContainedIn = "$in";
        public const string NotContainedIn = "$nin";
        public const string Exists = "$exists";
        public const string Ascending = "\"asc\":\"";
        public const string Descending = "\"desc\":\"";
        public const string BeforeUid = "\"before_uid\":\"";
        public const string AfterUid = "\"after_uid\":\"";
        public const string Skip = "\"skip\":\"";
        public const string Limit = "\"limit\":\"";
        public const string Count = "\"count\":";
        public const string IncludeCount = "\"include_count\":";
        public const string And = "$and";
        public const string Or = "$or";
        public const string IncludeSchema = "\"include_schema\":";
        public const string IncludeOwner = "\"include_owner\":";
        public const string Only = "\"only\":\"";
        public const string Except = "\"except\":\"";
        public const string Regex = "$regex";

        public static String ErrorMessage_JsonNotProper = "Please provide valid JSON.";        
        public static String ErrorMessage_StackApiKey = "Stack api key can not be null.";
        public static String ErrorMessage_FormName = "Please set contentType name.";
        public static String ErrorMessage_EntryUID = "Please set entry uid.";
        public static String ErrorMessage_Stack_AccessToken_IsNull = "Access token can not be null.";
        public static String ErrorMessage_Stack_Environment_IsNull = "Environment can not be null.";
        public static String ErrorMessage_NoConnectionError = "Connection error";
        public static String ErrorMessage_AuthFailureError = "Authentication Not present.";
        public static String ErrorMessage_ParseError = "Parsing Error.";
        public static String ErrorMessage_ServerError = "Server interaction went wrong, Please try again.";
        public static String ErrorMessage_Default = "Oops! Something went wrong. Please try again.";
        public static String ErrorMessage_NoNetwork = "Network not available.";
        public static String ErrorMessage_CalledContentstackDefaultMethod = "You must called Contentstack.stack() first";
        public static String ErrorMessage_QueryFilterException = "Please provide valid params.";
        public static String ErrorMessage_EntryNotFoundInCache = "Entry is not present in cache";
        public static String ErrorMessage_SavingNetworkCallResponseForCache = "Error while saving network call response.";
    }
}
