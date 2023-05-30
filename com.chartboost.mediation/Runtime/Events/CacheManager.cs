using System;
using System.Collections.Generic;

namespace Chartboost.Placements
{
    public static class CacheManager
    {
        private static readonly Dictionary<int, WeakReference<IChartboostMediationFullscreenAd>> FullscreenCache;
        private static readonly Dictionary<int, ChartboostMediationFullscreenAdLoadRequest> FullscreenAdLoadRequests;

        static CacheManager()
        {
            FullscreenCache = new Dictionary<int, WeakReference<IChartboostMediationFullscreenAd>>();
            FullscreenAdLoadRequests = new Dictionary<int, ChartboostMediationFullscreenAdLoadRequest>();
        }

        // Number of items in the cache.
        public static int Count => FullscreenCache.Count;

        // Retrieve a data object from the cache.
        public static IChartboostMediationFullscreenAd GetFullscreenAd(int hashCode)
        {
            if (!FullscreenCache.ContainsKey(hashCode))
                return null;
            
            var ad = FullscreenCache[hashCode].TryGetTarget(out var fullscreenAd);
            return ad ? fullscreenAd : null;
        }

        public static void TrackFullscreenAd(int hashCode, IChartboostMediationFullscreenAd ad)
        {
            FullscreenCache[hashCode] = new WeakReference<IChartboostMediationFullscreenAd>(ad, false);
        }

        public static void ReleaseFullscreenAd(int hashCode)
        {
            if (FullscreenCache.ContainsKey(hashCode))
                FullscreenCache.Remove(hashCode);
        }

        public static void TrackFullscreenAdLoadRequest(int hashCode, ChartboostMediationFullscreenAdLoadRequest request)
        {
            request.AssociatedProxy = hashCode;
            FullscreenAdLoadRequests[hashCode] = request;
        }
        
        public static ChartboostMediationFullscreenAdLoadRequest GetFullScreenAdLoadRequest(int hashCode)
        {
            if (!FullscreenAdLoadRequests.TryGetValue(hashCode, out var fullscreenAdLoadRequest)) 
                return null;

            ReleaseFullscreenAdLoadRequest(hashCode);
            return fullscreenAdLoadRequest;
        }

        public static void ReleaseFullscreenAdLoadRequest(int hashCode)
        {
            if (FullscreenAdLoadRequests.ContainsKey(hashCode))
                FullscreenAdLoadRequests.Remove(hashCode);
        }
    }
}
