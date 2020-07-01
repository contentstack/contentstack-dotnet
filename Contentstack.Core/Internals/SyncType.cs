using System;
namespace Contentstack.Core.Internals
{
    /// <summary>
    /// The different types of items you can request a sync for.
    /// </summary>
    [Flags]
    public enum SyncType : byte
    {
        /// <summary>
        /// This will bring all published entries and published assets
        /// </summary>
        Default = 0,

        /// <summary>
        /// Only published entries and assets
        /// </summary>
        All = 1,

        /// <summary>
        /// Only published assets.
        /// </summary>
        AssetPublished = 2,

        /// <summary>
        /// Only published entries.
        /// </summary>
        EntryPublished = 4,

        /// <summary>
        /// Only unpublished assets.
        /// </summary>
        AssetUnpublished = 8,

        /// <summary>
        /// Only unpublished entries.
        /// </summary>
        EntryUnpublished = 16,

        /// <summary>
        /// Only deleted assets.
        /// </summary>
        AssetDeleted = 32,

        /// <summary>
        /// Only deleted entries.
        /// </summary>
        EntryDeleted = 64,

        /// <summary>
        /// Only deleted Content-Types entries.
        /// </summary>
        ContentTypeDeleted = 128,
    }

}
