using Chartboost.AdFormats.Banner;

namespace Chartboost.Requests
{
    /// <summary>
    /// Load request for banner ad.
    /// </summary>
    public class ChartboostMediationBannerAdLoadRequest : ChartboostMediationAdLoadRequest
    {
        public ChartboostMediationBannerAdLoadRequest(string placementName, ChartboostMediationBannerSize size) : base(placementName) => Size = size;

        /// <summary>
        /// The <see cref="ChartboostMediationBannerSize"/> size for the request 
        /// </summary>
        public ChartboostMediationBannerSize Size { get; set; }
    }
}
