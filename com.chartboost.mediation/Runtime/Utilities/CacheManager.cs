using System;
using System.Collections.Generic;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Interfaces;
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
        
        /// <summary>
        /// Weak reference cache to banner ads. if publishers do not keep a strong ref, this will make sure they get disposed as needed.
        /// </summary>
        private static readonly Dictionary<long, WeakReference<IChartboostMediationBannerView>> BannerCache;
        
        /// <summary>
        /// Publisher supplied banner ad load requests.
        /// </summary>
        private static readonly Dictionary<long, ChartboostMediationBannerAdLoadRequest> BannerAdLoadRequests;

        static CacheManager()
        {
            FullscreenCache = new Dictionary<long, WeakReference<IChartboostMediationFullscreenAd>>();
            FullscreenAdLoadRequests = new Dictionary<long, ChartboostMediationFullscreenAdLoadRequest>();

            BannerCache = new Dictionary<long, WeakReference<IChartboostMediationBannerView>>();
            BannerAdLoadRequests = new Dictionary<long, ChartboostMediationBannerAdLoadRequest>();
        }

        #region Fullscreen
        
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
        

        #endregion
        
        #region Banner
        
        /// <summary>
        /// Keeps track of a <see cref="IChartboostMediationBannerView"/> with a weak reference so it can be disposed by GC.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        /// <param name="ad">Fullscreen ad to cache.</param>
        public static void TrackBannerAd(long hashCode, IChartboostMediationBannerView ad) 
            => BannerCache[hashCode] = new WeakReference<IChartboostMediationBannerView>(ad, false);

        /// <summary>
        /// Retrieves a <see cref="IChartboostMediationBannerView"/> by hashcode if any.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        /// <returns>Cached <see cref="IChartboostMediationBannerView"/>.</returns>
        public static IChartboostMediationBannerView GetBannerAd(long hashCode)
        {
            if (!BannerCache.ContainsKey(hashCode))
                return null;
            
            var ad = BannerCache[hashCode].TryGetTarget(out var bannerAd);
            return ad ? bannerAd : null;
        }

        /// <summary>
        /// Releases a <see cref="IChartboostMediationBannerView"/> from the cache.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        public static void ReleaseBannerAd(long hashCode)
        {
            if (BannerCache.ContainsKey(hashCode))
                BannerCache.Remove(hashCode);
        }

        /// <summary>
        /// Keeps track of a publisher's <see cref="ChartboostMediationBannerAdLoadRequest"/>.
        /// </summary>
        /// <param name="hashCode">Associated hashCode</param>
        /// <param name="request">Publisher's banner ad load request.</param>
        public static void TrackBannerAdLoadRequest(long hashCode, ChartboostMediationBannerAdLoadRequest request)
        {
            request.AssociatedProxy = hashCode;
            BannerAdLoadRequests[hashCode] = request;
        }
        
        /// <summary>
        /// Retrieves a <see cref="ChartboostMediationBannerAdLoadRequest"/> by hashcode if any.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        /// <returns>Cached <see cref="ChartboostMediationBannerAdLoadRequest"/>.</returns>
        public static ChartboostMediationBannerAdLoadRequest GetBannerAdLoadRequest(long hashCode)
        {
            if (!BannerAdLoadRequests.TryGetValue(hashCode, out var bannerAdLoadRequest)) 
                return null;

            ReleaseBannerAdLoadRequest(hashCode);
            return bannerAdLoadRequest;
        }

        /// <summary>
        /// Releases a <see cref="ChartboostMediationBannerAdLoadRequest"/> from the cache.
        /// </summary>
        /// <param name="hashCode">Associated hashCode.</param>
        public static void ReleaseBannerAdLoadRequest(long hashCode)
        {
            if (BannerAdLoadRequests.ContainsKey(hashCode))
                BannerAdLoadRequests.Remove(hashCode);
        }
        

        #endregion
        
        public static string CacheInfo() => $"CacheManager : \n" +
            $"Fullscreen Cache: {FullscreenCache.Count}, FullscreenAdLoadRequest: {FullscreenAdLoadRequests.Count}\n" +
            $"Banner Cache: {BannerCache.Count}, BannerAdLoadRequest: {BannerAdLoadRequests.Count}\n";
    }
}
