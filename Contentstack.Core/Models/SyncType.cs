using System;
namespace Contentstack.Core.Models
{
    /// <summary>
    /// The different types of items you can request a sync for.
    /// </summary>
    public enum SyncType
    {
        /// <summary>
        /// Every type of item.
        /// </summary>
        All,

        /// <summary>
        /// Only Asset Published.
        /// </summary>
        asset_published,

        /// <summary>
        /// Only Entry Published.
        /// </summary>
        entry_published,

        /// <summary>
        /// Only Asset Unpublished.
        /// </summary>
        asset_unpublished,

        /// <summary>
        /// Only Asset Deleted.
        /// </summary>
        asset_deleted,

        /// <summary>
        /// Only Entry Deleted.
        /// </summary>
        entry_deleted,

        /// <summary>
        /// Only Deleted Content Type.
        /// </summary>
        content_type_deleted

    }

}
