using System.Collections.Generic;
using Chartboost.Mediation.Ad.Fullscreen;

namespace Chartboost.Mediation.Requests
{
    /// <summary>
    /// Chartboost Mediation's <see cref="IFullscreenAd"/> load request.
    /// </summary>
    public sealed class FullscreenAdLoadRequest : AdLoadRequest
    {
        public FullscreenAdLoadRequest(string placementName, Dictionary<string, string> keywords = null, Dictionary<string, string> partnerSettings = null) : base(placementName)
        {
            Keywords = keywords ?? new Dictionary<string, string>();
            PartnerSettings = partnerSettings;
        }

        /// <summary>
        /// The keywords targeted for the ad.
        /// </summary>
        public IReadOnlyDictionary<string, string> Keywords { get;  }

        /// <summary>
        /// An optional <see cref="IDictionary{TKey,TValue}"/> that a publisher would like to send to all partners.
        /// </summary>
        public IReadOnlyDictionary<string, string> PartnerSettings { get; }
    }
}
