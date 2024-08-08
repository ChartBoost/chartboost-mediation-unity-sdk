using System.Collections;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation
{
    /// <param name="placement">The placement name.</param>
    /// <param name="impressionData">The impression data delivered within a hashtable.</param>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once IdentifierTypo
    public delegate void ChartboostMediationImpressionLevelRevenueDataEvent(string placement, Hashtable impressionData);

    /// <summary>
    /// <param name="partnerInitializationEventData">The partner initialization data delivered within a hashtable.</param>
    /// </summary>
    public delegate void ChartboostMediationPartnerAdapterInitializationEvent(string partnerInitializationEventData);

}
