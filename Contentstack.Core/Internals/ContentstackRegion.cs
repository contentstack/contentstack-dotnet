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
        AU
    }


    internal enum ContentstackRegionCode
    {
        us,

        eu,

        azure_eu,

        azure_na,

        gcp_na,

        au,
        
    }
}
