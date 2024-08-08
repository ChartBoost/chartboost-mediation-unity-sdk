using System;
using System.Collections.Generic;
using System.Xml;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Requests;

namespace Chartboost.Mediation.Utilities
{
    public static class AdCache
    {
        /// <summary>
        /// Weak reference cache to <see cref="IAd"/> ads.
        /// </summary>
        private static readonly Dictionary<long, WeakReference<IAd>> Ads = new();
        
        /// <summary>
        /// Publisher supplied <see cref="AdLoadRequests"/> requests.
        /// </summary>
        private static readonly Dictionary<long, AdLoadRequest> AdLoadRequests = new();

        /// <summary>
        /// Keeps track of a <see cref="IAd"/> with a weak reference so it can be disposed by GC.
        /// </summary>
        /// <param name="uniqueId">Associated unique identifier.</param>
        /// <param name="ad"><see cref="IAd"/> to cache.</param>
        public static void TrackAd(long uniqueId, IAd ad)
        {
            Ads[uniqueId] = new WeakReference<IAd>(ad, false);
            LogController.Log($"Tracking {ad.GetType()} with UniqueId: {ad}", LogLevel.Verbose);
        }

        /// <inheritdoc cref="TrackAd(long, IAd)"/>
        public static void TrackAd(IntPtr uniqueId, IAd ad)
            => TrackAd(uniqueId.ToInt64(), ad);

        /// <summary>
        /// Retrieves a <see cref="IAd"/> by unique identifier if any.
        /// </summary>
        /// <param name="uniqueId">Associated unique identifier.</param>
        /// <returns>Cached <see cref="IAd"/>.</returns>
        public static IAd GetAd(long uniqueId)
        {
            if (!Ads.TryGetValue(uniqueId, out var value))
            {
                LogController.Log($"Failed to get WeakReference<IAd> for: {uniqueId}, reference was most likely disposed, returning null.", LogLevel.Warning);
                return null;
            }

            var found = value.TryGetTarget(out var ad);
            return found ? ad : null;
        }
        
        /// <inheritdoc cref="GetAd(long)"/>
        public static IAd GetAd(IntPtr uniqueId)
            => GetAd(uniqueId.ToInt64());

        /// <summary>
        /// Releases a <see cref="IAd"/> from the cache.
        /// </summary>
        /// <param name="uniqueId">Associated unique identifier.</param>
        public static void ReleaseAd(long uniqueId)
        {
            if (!Ads.ContainsKey(uniqueId))
            {
                LogController.Log($"Attempted to release: {uniqueId} but no reference was found", LogLevel.Warning);
                return;
            }

            LogController.Log($"Releasing IAd reference for {uniqueId}", LogLevel.Verbose);
            Ads.Remove(uniqueId);
        }

        /// <inheritdoc cref="ReleaseAd(long)"/>
        public static void ReleaseAd(IntPtr uniqueId)
        {
            var hashCode = uniqueId.ToInt64();
            ReleaseAd(hashCode);
        }

        /// <summary>
        /// Keeps track of a publisher's <see cref="AdLoadRequest"/>.
        /// </summary>
        /// <param name="uniqueId">Associated hashCode</param>
        /// <param name="request">Publisher's fullscreen ad load request.</param>
        public static void TrackAdLoadRequest(long uniqueId, AdLoadRequest request)
        {
            LogController.Log($"Tracking AdLoadRequest with uniqueId: {uniqueId}", LogLevel.Verbose);
            request.AssociatedProxy = uniqueId;
            AdLoadRequests[uniqueId] = request;
        }
        
        /// <summary>
        /// Retrieves a <see cref="AdLoadRequest"/> by hashcode if any.
        /// </summary>
        /// <param name="uniqueId">Associated hashCode.</param>
        /// <returns>Cached <see cref="AdLoadRequest"/>.</returns>
        public static AdLoadRequest GetAdLoadRequest(long uniqueId)
        {
            if (!AdLoadRequests.TryGetValue(uniqueId, out var adLoadRequest))
            {
                LogController.Log($"Failed to get WeakReference<AdLoadRequest> for: {uniqueId}, reference was most likely disposed, returning null.", LogLevel.Warning);
                return null;
            }

            ReleaseAdLoadRequest(uniqueId);
            return adLoadRequest;
        }

        /// <summary>
        /// Releases a <see cref="AdLoadRequest"/> from the cache.
        /// </summary>
        /// <param name="uniqueId">Associated hashCode.</param>
        public static void ReleaseAdLoadRequest(long uniqueId)
        {
            if (!AdLoadRequests.ContainsKey(uniqueId))
                return;

            LogController.Log($"Releasing AdLoadRequest for {uniqueId}", LogLevel.Verbose);
            AdLoadRequests.Remove(uniqueId);
        }
        
        public static string CacheInfo() => $"CacheManager : \n" + $"Fullscreen Cache: {Ads.Count}, FullscreenAdLoadRequest: {AdLoadRequests.Count}";
    }
}
