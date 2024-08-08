using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Utilities;

namespace Chartboost.Mediation.Requests
{
    /// <summary>
    /// Parent class utilized by all <see cref="IAd"/> load request.
    /// </summary>
    public class AdLoadRequest
    {
        /// <summary>
        /// Base constructor for all load requests.
        /// </summary>
        /// <param name="placementName"></param>
        internal AdLoadRequest(string placementName) => PlacementName = placementName;

        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName { get; }

        internal long AssociatedProxy { get; set; }
        
        ~AdLoadRequest()
            => AdCache.ReleaseAdLoadRequest(AssociatedProxy);
    }
}
