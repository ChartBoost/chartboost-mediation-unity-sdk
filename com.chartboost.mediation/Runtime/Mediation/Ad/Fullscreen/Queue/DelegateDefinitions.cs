using Chartboost.Mediation.Requests;

namespace Chartboost.Mediation.Ad.Fullscreen.Queue
{
    /// <summary>
    /// <see cref="IFullscreenAdQueue"/> update event. Triggers when <see cref="IFullscreenAdQueue"/> is modified.
    /// <param name="adQueue">The <see cref="IFullscreenAdQueue"/> triggering the event.</param>
    /// <param name="adLoadResult">The associated <see cref="IAdLoadResult"/> data.</param>
    /// <param name="numberOfAdsReady">Number of <see cref="IFullscreenAd"/> ready.</param>
    /// </summary>
    public delegate void FullscreenAdQueueUpdateEvent(IFullscreenAdQueue adQueue, IAdLoadResult adLoadResult, int numberOfAdsReady);
    
    /// <summary>
    /// Indicates when a <see cref="IFullscreenAd"/> expires inside a <see cref="IFullscreenAdQueue"/>.
    /// <param name="adQueue">The <see cref="IFullscreenAdQueue"/> triggering the event.</param>
    /// <param name="numberOfAdsReady">Number of <see cref="IFullscreenAd"/> ready.</param>
    /// </summary>
    public delegate void FullscreenAdQueueRemoveExpiredAdEvent(IFullscreenAdQueue adQueue, int numberOfAdsReady);
}
