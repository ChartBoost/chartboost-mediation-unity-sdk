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

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Keywords"/>
        public override Dictionary<string, string> Keywords { get; set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Request"/>
        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.WinningBidInfo"/>
        public override BidInfo WinningBidInfo { get; protected set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.LoadId"/>
        public override string LoadId { get; protected set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.LoadMetrics"/>
        public override Metrics? LoadMetrics { get; protected set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.AdSize"/>
        public override ChartboostMediationBannerSize? AdSize { get; protected set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.ContainerSize"/>
        public override ChartboostMediationBannerSize? ContainerSize { get; protected set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.HorizontalAlignment"/>
        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.VerticalAlignment"/>
        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,Chartboost.Banner.ChartboostMediationBannerAdScreenLocation)"/>
        public override Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            base.Load(request, screenLocation);
            return Task.FromResult(new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError("Unsupported Platform")));
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,float, float)"/>
        public override Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y)
        {
            base.Load(request, x, y);
            return Task.FromResult(new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError("Unsupported Platform")));
        }

    }
}
