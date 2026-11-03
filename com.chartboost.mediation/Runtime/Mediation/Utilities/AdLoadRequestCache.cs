using System.Collections.Concurrent;
using Chartboost.Logging;
using Chartboost.Mediation.Requests;

namespace Chartboost.Mediation.Utilities
{
    /// <summary>
    /// Cache for publisher-supplied <see cref="AdLoadRequest"/> objects, keyed by the native load
    /// listener's <c>hashCode()</c> (Android) or pointer (iOS). Bridges the request from the load
    /// call to the native load callback so the resulting ad can be paired with its originating request.
    /// </summary>
    public static class AdLoadRequestCache
    {
        /// <summary>
        /// Publisher supplied <see cref="AdLoadRequest"/> requests.
        /// Thread-safe dictionary to handle concurrent access from native callbacks and Unity main thread.
        /// </summary>
        private static readonly ConcurrentDictionary<long, AdLoadRequest> AdLoadRequests = new();

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
        /// Retrieves and removes a <see cref="AdLoadRequest"/> by hashcode (get-once); subsequent
        /// calls for the same id return <c>null</c>.
        /// </summary>
        /// <param name="uniqueId">Associated hashCode.</param>
        /// <returns>Cached <see cref="AdLoadRequest"/>, or <c>null</c> if not present.</returns>
        public static AdLoadRequest TakeAdLoadRequest(long uniqueId)
        {
            // Get-once: retrieve and remove in a single lookup.
            if (!AdLoadRequests.TryRemove(uniqueId, out var adLoadRequest))
            {
                LogController.Log($"Failed to get AdLoadRequest for: {uniqueId}, request was not found in cache, returning null.", LogLevel.Warning);
                return null;
            }

            return adLoadRequest;
        }

        /// <summary>
        /// Releases a <see cref="AdLoadRequest"/> from the cache.
        /// </summary>
        /// <param name="uniqueId">Associated hashCode.</param>
        public static void ReleaseAdLoadRequest(long uniqueId)
        {
            AdLoadRequests.TryRemove(uniqueId, out _);
        }

        /// <summary>
        /// Human-readable snapshot of the request cache size, for diagnostics.
        /// </summary>
        public static string CacheInfo() => $"AdLoadRequestCache: {AdLoadRequests.Count} tracked";

        /// <summary>Number of tracked requests. Test/diagnostics seam, not part of the public API.</summary>
        internal static int Count => AdLoadRequests.Count;

        /// <summary>Removes all tracked requests. Test-only seam.</summary>
        internal static void ClearForTesting() => AdLoadRequests.Clear();
    }
}
