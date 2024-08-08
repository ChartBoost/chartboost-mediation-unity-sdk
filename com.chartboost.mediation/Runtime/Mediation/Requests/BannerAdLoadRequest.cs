using Chartboost.Mediation.Ad.Banner;

namespace Chartboost.Mediation.Requests
{
    /// <summary>
    /// Chartboost Mediation's <see cref="IBannerAd"/> load request.
    /// </summary>
    public sealed class BannerAdLoadRequest : AdLoadRequest
    {
        public BannerAdLoadRequest(string placementName, BannerSize size) : base(placementName) => Size = size;

        /// <summary>
        /// The <see cref="BannerSize"/> size for the request 
        /// </summary>
        public BannerSize Size { get; set; }
    }
}
