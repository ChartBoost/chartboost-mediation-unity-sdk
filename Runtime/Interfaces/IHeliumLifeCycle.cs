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
        ///   Called after an the SDK has been initialized
        /// </summary>
        [Preserve]
        public event HeliumEvent DidStart;
        
        /// <summary>
        ///   Called immediately when impression level revenue data has been received after an
        ///   ad was displayed on the screen. This may be called in a background thread. This event
        ///   is sent natively from iOS and Android.
        /// </summary>
        [Preserve]
        public event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
    }
}
