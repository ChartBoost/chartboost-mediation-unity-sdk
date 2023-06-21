using System;
using System.Collections.Generic;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Requests;

namespace Chartboost.Utilities
{
    public static class CacheManager
    {
        /// <summary>
        /// Weak reference cache to fullscreen ads. if publishers do not keep a strong ref, this will make sure they get disposed as needed.
        /// </summary>
        private static readonly Dictionary<long, WeakReference<IChartboostMediationFullscreenAd>> FullscreenCache;
        
        /// <summary>
        /// Publisher supplied fullscreen ad load requests.
        /// </summary>
        private static readonly Dictionary<long, ChartboostMediationFullscreenAdLoadRequest> FullscreenAdLoadRequests;

        static CacheManager()
        {
            FullscreenCache = new Dictionary<long, WeakReference<IChartboostMediationFullscreenAd>>();
            FullscreenAdLoadRequests = new Dictionary<long, ChartboostMediationFullscreenAdLoadRequest>();
        }

        /// <summary>
        /// Keeps track of a <see cref="IChartboostMediationFullscreenAd"/> with a weak reference so it can be disposed by GC.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        /// <param name="ad">Fullscreen ad to cache.</param>
        public static void TrackFullscreenAd(long hashCode, IChartboostMediationFullscreenAd ad) 
            => FullscreenCache[hashCode] = new WeakReference<IChartboostMediationFullscreenAd>(ad, false);

        /// <summary>
        /// Retrieves a <see cref="IChartboostMediationFullscreenAd"/> by hashcode if any.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        /// <returns>Cached <see cref="IChartboostMediationFullscreenAd"/>.</returns>
        public static IChartboostMediationFullscreenAd GetFullscreenAd(long hashCode)
        {
            if (!FullscreenCache.ContainsKey(hashCode))
                return null;
            
            var ad = FullscreenCache[hashCode].TryGetTarget(out var fullscreenAd);
            return ad ? fullscreenAd : null;
        }

        /// <summary>
        /// Releases a <see cref="IChartboostMediationFullscreenAd"/> from the cache.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        public static void ReleaseFullscreenAd(long hashCode)
        {
            if (FullscreenCache.ContainsKey(hashCode))
                FullscreenCache.Remove(hashCode);
        }

        /// <summary>
        /// Keeps track of a publisher's <see cref="ChartboostMediationFullscreenAdLoadRequest"/>.
        /// </summary>
        /// <param name="hashCode">Associated hashCode</param>
        /// <param name="request">Publisher's fullscreen ad load request.</param>
        public static void TrackFullscreenAdLoadRequest(long hashCode, ChartboostMediationFullscreenAdLoadRequest request)
        {
            request.AssociatedProxy = hashCode;
            FullscreenAdLoadRequests[hashCode] = request;
        }
        
        /// <summary>
        /// Retrieves a <see cref="ChartboostMediationFullscreenAdLoadRequest"/> by hashcode if any.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        /// <returns>Cached <see cref="ChartboostMediationFullscreenAdLoadRequest"/>.</returns>
        public static ChartboostMediationFullscreenAdLoadRequest GetFullScreenAdLoadRequest(long hashCode)
        {
            if (!FullscreenAdLoadRequests.TryGetValue(hashCode, out var fullscreenAdLoadRequest)) 
                return null;

            ReleaseFullscreenAdLoadRequest(hashCode);
            return fullscreenAdLoadRequest;
        }

        /// <summary>
        /// Releases a <see cref="ChartboostMediationFullscreenAdLoadRequest"/> from the cache.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        public static void ReleaseFullscreenAdLoadRequest(long hashCode)
        {
            if (FullscreenAdLoadRequests.ContainsKey(hashCode))
                FullscreenAdLoadRequests.Remove(hashCode);
        }
    }
}
