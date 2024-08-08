using Chartboost.Mediation.Ad.Fullscreen.Queue;

namespace Chartboost.Mediation.iOS.Ad.Fullscreen.Queue
{
    /// <summary>
    /// Extern definition for <see cref="FullscreenAdQueueUpdateEvent"/>
    /// </summary>
    internal delegate void ExternFullscreenAdQueueUpdateEvent(long hashCode, string loadId, string metricsJson, string winningBidInfoJson, string code, string message, int numberOfAdsReady);
    
    /// <summary>
    /// Extern definition for <see cref="FullscreenAdQueueRemoveExpiredAdEvent"/>
    /// </summary>
    internal delegate void ExternFullscreenAdQueueRemoveExpiredAdEvent(long hashCode, int numberOfAdsReady);
}
