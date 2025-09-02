using System;
using System.Collections.Generic;
using System.Net;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Base exception class for all Contentstack exceptions
    /// </summary>
    public class ContentstackException : Exception
    {
        public int ErrorCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, object> Errors { get; set; }

        public ContentstackException() : base()
        {
        }

        public ContentstackException(string message) : base(message)
        {
        }

        public ContentstackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ContentstackException(Exception ex) : base(ex.Message, ex)
        {
        }
    }

    /// <summary>
    /// Exception thrown when there are issues with query filters or parameters
    /// </summary>
    public class QueryFilterException : ContentstackException
    {
        public QueryFilterException() : base(ErrorMessages.QueryFilterError)
        {
        }

        public QueryFilterException(string message) : base(message)
        {
        }

        public QueryFilterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static QueryFilterException Create(Exception innerException = null)
        {
            return new QueryFilterException(
                string.Format(ErrorMessages.InvalidParamsError, 
                innerException?.Message ?? ErrorMessages.QueryFilterError),
                innerException);
        }
    }

    /// <summary>
    /// Exception thrown when there are asset-related errors
    /// </summary>
    public class AssetException : ContentstackException
    {
        public AssetException(string message) : base(message)
        {
        }

        public AssetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static AssetException CreateForJsonConversionError()
        {
            return new AssetException(ErrorMessages.AssetJsonConversionError);
        }

        public static AssetException CreateForProcessingError(Exception innerException)
        {
            return new AssetException(
                string.Format(ErrorMessages.AssetProcessingError, 
                ErrorMessages.FormatExceptionDetails(innerException)),
                innerException);
        }
    }

    /// <summary>
    /// Exception thrown when there are live preview-related errors
    /// </summary>
    public class LivePreviewException : ContentstackException
    {
        public LivePreviewException() : base(ErrorMessages.LivePreviewTokenMissing)
        {
        }

        public LivePreviewException(string message) : base(message)
        {
        }

        public LivePreviewException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when there are global field-related errors
    /// </summary>
    public class GlobalFieldException : ContentstackException
    {
        public GlobalFieldException(string message) : base(message)
        {
        }

        public GlobalFieldException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static GlobalFieldException CreateForProcessingError(Exception innerException)
        {
            return new GlobalFieldException(
                string.Format(ErrorMessages.GlobalFieldProcessingError, 
                ErrorMessages.FormatExceptionDetails(innerException)),
                innerException);
        }

        public static GlobalFieldException CreateForIdNull()
        {
            return new GlobalFieldException(ErrorMessages.GlobalFieldIdNullError);
        }
    }

    /// <summary>
    /// Exception thrown when there are entry-related errors
    /// </summary>
    public class EntryException : ContentstackException
    {
        public EntryException(string message) : base(message)
        {
        }

        public EntryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static EntryException CreateForProcessingError(Exception innerException)
        {
            return new EntryException(
                string.Format(ErrorMessages.EntryProcessingError, 
                ErrorMessages.FormatExceptionDetails(innerException)),
                innerException);
        }
    }
}