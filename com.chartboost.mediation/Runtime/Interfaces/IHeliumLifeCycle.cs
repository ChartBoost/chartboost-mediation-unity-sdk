// ReSharper disable InconsistentNaming
using UnityEngine.Scripting;

namespace Helium.Interfaces
{
    /// <summary>
    /// All Helium SDK Lifecycle callbacks.
    /// </summary>
    public interface IHeliumLifeCycle
    {
        /// <summary>
        /// Called after an the SDK has been initialized
        /// </summary>
        [Preserve]
        public event HeliumEvent DidStart;
        
        /// <summary>
        /// Called immediately when impression level revenue data has been received after an
        /// ad was displayed on the screen.
        /// </summary>
        [Preserve]
        public event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        
        /// <summary>
        /// Provides Helium initialization metrics data in a json format.
        /// </summary>
        public event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData;
    }
}
