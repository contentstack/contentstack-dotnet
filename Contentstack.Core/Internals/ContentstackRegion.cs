using System.Collections.Generic;

namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Contentstack region.
    /// </summary>
    public enum ContentstackRegion
    {
        /// <summary>
        /// To specify US region.
        /// </summary>
        US,
        /// <summary>
        /// To specify EU region.
        /// </summary>
        EU,
        /// <summary>
        /// To specify AZURE_EU region.
        /// </summary>
        AZURE_EU,

        /// <summary>
        /// To specify AZURE_NA region.
        /// </summary>
        AZURE_NA,

        /// <summary>
        /// To specify GCP_NA region.
        /// </summary>
        GCP_NA,

        /// <summary>
        /// To specify AWS_AU region.
        /// </summary>
        AU,

        /// <summary>
        /// To specify GCP_EU region.
        /// </summary>
        GCP_EU
    }


    internal enum ContentstackRegionCode
    {
        us,

        eu,

        azure_eu,

        azure_na,

        gcp_na,

        au,

        gcp_eu,
    }

    /// <summary>
    /// Maps <see cref="ContentstackRegion"/> enum values to the region ID strings
    /// used by the Contentstack regions registry (artifacts.contentstack.com/regions.json).
    /// </summary>
    internal static class ContentstackRegionMap
    {
        internal static readonly Dictionary<ContentstackRegion, string> RegionIdMap = new Dictionary<ContentstackRegion, string>
        {
            { ContentstackRegion.US,       "na"       },
            { ContentstackRegion.EU,       "eu"       },
            { ContentstackRegion.AZURE_EU, "azure-eu" },
            { ContentstackRegion.AZURE_NA, "azure-na" },
            { ContentstackRegion.GCP_NA,   "gcp-na"   },
            { ContentstackRegion.AU,       "au"       },
            { ContentstackRegion.GCP_EU,   "gcp-eu"   },
        };
    }
}
