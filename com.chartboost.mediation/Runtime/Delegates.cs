using System.Collections;

namespace Chartboost
{
    /// <param name="error">The error encountered, if any.</param>
    public delegate void ChartboostMediationEvent(string error);

    /// <param name="placement">The placement name.</param>
    /// <param name="impressionData">The impression data delivered within a hashtable.</param>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public delegate void ChartboostMediationILRDEvent(string placement, Hashtable impressionData);

    /// <summary>
    /// <param name="partnerInitializationEventData">The partner initialization data delivered within a hashtable.</param>
    /// </summary>
    public delegate void ChartboostMediationPartnerInitializationEvent(string partnerInitializationEventData);

    /// <summary>
    /// <param name="placement">The placement name for the placement.</param>
    /// <param name="error">The error encountered, if any.</param>
    /// </summary>
    public delegate void ChartboostMediationPlacementEvent(string placement, string error);

    /// <summary>
    /// <param name="placement">The placement name for the placement.</param>
    /// /// <param name="loadId">The placement name for the placement.</param>
    /// <param name="error">The error encountered, if any.</param>
    /// </summary>
    public delegate void ChartboostMediationPlacementLoadEvent(string placement, string loadId, BidInfo bidInfo, string error);
}
