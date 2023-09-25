using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;

namespace Chartboost.AdFormats.Banner
{
    internal class ChartboostMediationBannerViewUnsupported : ChartboostMediationBannerViewBase
    {
        public ChartboostMediationBannerViewUnsupported()
        {
            LogTag = "ChartboostMediationBanner (Unsupported)";
        }

        public override Dictionary<string, string> Keywords { get; set; }
        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }
        public override BidInfo WinningBidInfo { get; protected set; }
        public override string LoadId { get; protected set; }
        public override Metrics? LoadMetrics { get; protected set; }
        public override ChartboostMediationBannerSize? AdSize { get; protected set; }
        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        public override Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            base.Load(request, screenLocation);
            return Task.FromResult(new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError("Unsupported Platform")));
        }
    }
}
