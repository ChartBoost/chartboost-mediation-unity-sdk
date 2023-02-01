using System.Collections;

namespace Helium
{
    /// <param name="error">The error encountered, if any.</param>
    public delegate void HeliumEvent(string error);

    /// <param name="placement">The placement name.</param>
    /// <param name="impressionData">The impression data delivered within a hashtable.</param>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public delegate void HeliumILRDEvent(string placement, Hashtable impressionData);

    /// <summary>
    /// <param name="partnerInitializationEventData">The partner initialization data delivered within a hashtable.</param>
    /// </summary>
    public delegate void HeliumPartnerInitializationEvent(string partnerInitializationEventData);

    /// <summary>
    /// <param name="placement">The placement name for the placement.</param>
    /// <param name="error">The error encountered, if any.</param>
    /// </summary>
    public delegate void HeliumPlacementEvent(string placement, string error);

    /// <summary>
    /// <param name="placement">The placement name for the placement.</param>
    /// /// <param name="loadId">The placement name for the placement.</param>
    /// <param name="error">The error encountered, if any.</param>
    /// </summary>
    public delegate void HeliumPlacementLoadEvent(string placement, string loadId, HeliumBidInfo bidInfo, string error);
}
