// ReSharper disable InconsistentNaming
using UnityEngine.Scripting;

// ReSharper disable once CheckNamespace
namespace Chartboost
{
    /// <summary>
    /// All Chartboost Mediation SDK Lifecycle callbacks.
    /// </summary>
    public interface IChartboostMediationLifeCycle
    {
        /// <summary>
        /// Called after an the SDK has been initialized
        /// </summary>
        [Preserve]
        public event ChartboostMediationEvent DidStart;
        
        /// <summary>
        /// Called immediately when impression level revenue data has been received after an
        /// ad was displayed on the screen.
        /// </summary>
        [Preserve]
        public event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        
        /// <summary>
        /// Provides Chartboost Mediation initialization metrics data in a json format.
        /// </summary>
        [Preserve]
        public event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
    }
}
