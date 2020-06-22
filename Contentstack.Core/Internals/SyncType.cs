using System;
namespace Contentstack.Core.Internals
{
    /// <summary>
    /// The different types of items you can request a sync for.
    /// </summary>
    public enum SyncType
    {
        /// <summary>
        /// Only published entries and assets
        /// </summary>
        All,

        /// <summary>
        /// Only published assets.
        /// </summary>
        AssetPublished,

        /// <summary>
        /// Only published entries.
        /// </summary>
        EntryPublished,

        /// <summary>
        /// Only unpublished assets.
        /// </summary>
        AssetUnpublished,

        /// <summary>
        /// Only unpublished entries.
        /// </summary>
        EntryUnpublished,

        /// <summary>
        /// Only deleted assets.
        /// </summary>
        AssetDeleted,

        /// <summary>
        /// Only deleted entries.
        /// </summary>
        EntryDeleted,

        /// <summary>
        /// Only deleted Content-Types entries.
        /// </summary>
        ContentTypeDeleted
    }

}
