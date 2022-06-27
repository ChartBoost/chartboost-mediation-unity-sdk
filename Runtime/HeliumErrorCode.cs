namespace Helium
{
    /// <summary>
    /// Returned to Helium Delegate methods to notify of Helium SDK errors.
    /// </summary>
    public enum HeliumErrorCode
    {
        /// No ad was available for the user from Helium
        NoAdFound = 0,
        /// No bid
        NoBid = 1,
        /// No internet connection was found
        NoNetwork = 2,
        /// An error occurred during network communication with the server
        ServerError = 3,
        /// An unknown or unexpected error
        Unknown = 4
    }
}
