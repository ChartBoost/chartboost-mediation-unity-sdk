using Chartboost.Caching;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Requests;

namespace Chartboost.Mediation.Utilities.Events
{
    /// <summary>
    /// Handles <see cref="IAd"/> events. Meant to be utilized across platforms, automatically send events to Unity's main thread.
    /// </summary>
    internal sealed class AdEventHandler
    {
        /// <summary>
        /// Utilized to dispatch <see cref="IFullscreenAd"/> events.
        /// </summary>
        /// <param name="uniqueId">Unique identifier for <see cref="IFullscreenAd"/>.</param>
        /// <param name="eventType"><see cref="FullscreenAdEvents"/> event to be dispatched.</param>
        /// <param name="code">Error <b>Code</b> found while triggering a <see cref="FullscreenAdEvents"/> event.</param>
        /// <param name="message">Error <b>Message</b> found while triggering a <see cref="FullscreenAdEvents"/> event.</param>
        public static void ProcessFullscreenEvent(long uniqueId, FullscreenAdEvents eventType, string code, string message)
        {
            MainThreadDispatcher.Post(o =>
            {
                var ad = (FullscreenAdBase)AdCache<IAd>.GetAd(uniqueId);

                // Ad event was fired but no reference for it exists. Developer did not set strong reference to it, so it was garbage collected.
                if (ad == null)
                {
                    LogController.Log($"Fullscreen Ad event: {eventType.ToString()} fired, but no ad reference for ad: {uniqueId} found.", LogLevel.Error);
                    return;
                }

                var error = NativeResults.ToError(code, message);

                switch (eventType)
                {
                    case FullscreenAdEvents.RecordImpression:
                        ad.OnRecordImpression();
                        break;
                    case FullscreenAdEvents.Click:
                        ad.OnClick();
                        break;
                    case FullscreenAdEvents.Reward:
                        ad.OnReward();
                        break;
                    case FullscreenAdEvents.Expire:
                        ad.OnExpire();
                        AdCache<IAd>.ReleaseAd(uniqueId);
                        break;
                    case FullscreenAdEvents.Close:
                        ad.OnClose(error);
                        AdCache<IAd>.ReleaseAd(uniqueId);
                        break;
                }
            });
        }
        
        /// <summary>
        /// Utilized to dispatch <see cref="IFullscreenAdQueue"/> events.
        /// </summary>
        /// <param name="uniqueId">Unique identifier for <see cref="IFullscreenAd"/>.</param>
        /// <param name="eventType"><see cref="FullscreenAdEvents"/> event to be dispatched.</param>
        /// <param name="adLoadResult">The associated <see cref="IAdLoadResult"/> data.</param>
        /// <param name="numberOfAdsReady">Number of <see cref="IFullscreenAd"/> ready.</param>
        public static void ProcessFullscreenAdQueueEvent(long uniqueId, FullscreenAdQueueEvents eventType, IAdLoadResult adLoadResult, int numberOfAdsReady)
        {
            MainThreadDispatcher.Post(_ =>
            {
                var queue = (FullscreenAdQueueBase)AdCache<IAd>.GetAd(uniqueId);

                // Queue event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                if (queue == null)
                {
                    LogController.Log($"Fullscreen Ad Queue event: {eventType.ToString()} fired, but no ad reference for ad: {uniqueId} found.", LogLevel.Error);
                    return;
                }

                switch (eventType)
                {
                    case FullscreenAdQueueEvents.Update :
                        queue.OnDidUpdate(adLoadResult, numberOfAdsReady);
                        break;
                    case FullscreenAdQueueEvents.RemoveExpiredAd:
                        queue.OnDidRemoveExpiredAd(numberOfAdsReady);
                        break;
                }
            });
        }

        /// <summary>
        /// Utilized to dispatch <see cref="IBannerAd"/> events.
        /// </summary>
        /// <param name="uniqueId">Unique identifier for <see cref="IBannerAd"/>.</param>
        /// <param name="eventType"><see cref="BannerAdEvents"/> event to be dispatched.</param>
        public static void ProcessBannerEvent(long uniqueId, BannerAdEvents eventType)
        {
            MainThreadDispatcher.Post(o =>
            {
                var ad = (BannerAdBase)AdCache<IAd>.GetAd(uniqueId);

                // Ad event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                if (ad == null) 
                {
                    LogController.Log($"Banner Ad event: {eventType.ToString()} fired, but no ad reference for ad: {uniqueId} found.", LogLevel.Error);
                    return;
                }

                switch (eventType)
                {
                    case BannerAdEvents.Load:
                        ad.OnWillAppear();
                        break;
                    case BannerAdEvents.Click:
                        ad.OnClick();
                        break;
                    case BannerAdEvents.RecordImpression:
                        ad.OnRecordImpression();
                        break;
                }
            });
        }
    }
}
