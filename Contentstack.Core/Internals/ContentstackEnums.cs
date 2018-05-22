namespace Contentstack.Core.Internals
{
    /// <summary>
    /// Set of allowed cache policies.
    /// </summary>
    public enum CachePolicy
    {
        /// <summary>
        /// This is to handle exception response, which are not identified while programming.
        /// </summary>
        None,

        /// <summary>
        /// To fetch data from cache if data not available in cache then it will send a network call and response will be saved in cache.
        /// </summary>
        CacheElseNetwork,

        /// <summary>
        /// To fetch data from cache.
        /// </summary>
        CacheOnly,

        /// <summary>
        /// To fetch data from cache and send a network call and response will be saved in cache.
        /// </summary>
        CacheThenNetwork,

        /// <summary>
        /// To fetch data from network call and response will not be saved cache.
        /// </summary>
        IgnoreCache,

        /// <summary>
        /// To fetch data from network and response will be saved in cache ; if network not available then it will fetch data from cache.
        /// </summary>
        NetworkElseCache,

        /// <summary>
        /// To fetch data from network and response will be saved in cache.
        /// </summary>
        NetworkOnly
    }

    /// <summary>
    /// Set of allowed order by.
    /// </summary>
    public enum OrderBy
    {
        /// <summary>
        /// order by desceding
        /// </summary>
        OrderByAscending,

        /// <summary>
        /// order by descending
        /// </summary>
        OrderByDescending
    }

    /// <summary>
    /// Response type of success query and also response type for error.
    /// </summary>
    public enum ResponseType
    {
        /// <summary>
        /// Response receive through Cache.
        /// </summary>
        Cache,

        /// <summary>
        /// Response receive through Network.
        /// </summary>
        Network,

        /// <summary>
        /// This is to handle exception response, which are not identified while programming.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// This defines different types of network availability status.
    /// </summary>
    public enum NetworkStatus
    {
        /// <summary>
        /// If network is not reachable.
        /// </summary>
        NotReachable,
        /// <summary>
        /// If network is available on mobile data.
        /// </summary>
        ReachableViaCarrierDataNetwork,
        /// <summary>
        /// If network is available on WiFi.
        /// </summary>
        ReachableViaWiFiNetwork
    }

    /// <summary>
    /// Http methods.
    /// </summary>
    public enum HttpMethods
    {
        /// <summary>
        /// To specify get request.
        /// </summary>
        Get,
        /// <summary>
        /// To specify post request.
        /// </summary>
        Post,
        /// <summary>
        /// To specify put request.
        /// </summary>
        Put,
        /// <summary>
        /// To specify delete request.
        /// </summary>
        Delete
    }
}
