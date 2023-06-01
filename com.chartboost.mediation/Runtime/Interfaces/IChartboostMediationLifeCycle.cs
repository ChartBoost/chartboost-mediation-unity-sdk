// ReSharper disable once CheckNamespace
// ReSharper disable EventNeverSubscribedTo.Global
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
        public event ChartboostMediationEvent DidStart;
        
        /// <summary>
        /// Called immediately when impression level revenue data has been received after an
        /// ad was displayed on the screen.
        /// </summary>
        public event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        
        /// <summary>
        /// Provides Chartboost Mediation initialization metrics data in a json format.
        /// </summary>
        public event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
    }
}
