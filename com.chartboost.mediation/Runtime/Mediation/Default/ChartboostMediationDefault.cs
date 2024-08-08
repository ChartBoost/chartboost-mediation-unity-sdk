using System;
using System.Threading.Tasks;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Default.Ad.Banner;
using Chartboost.Mediation.Default.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;

namespace Chartboost.Mediation.Default
{
    /// <summary>
    /// Default implementation of <see cref="ChartboostMediationBase"/> for any unsupported platforms.
    /// </summary>
    internal sealed class ChartboostMediationDefault : ChartboostMediationBase
    {
        public override string CoreModuleId => "chartboost_mediation";

        /// <inheritdoc cref="ChartboostMediationBase.NativeSDKVersion"/>
        public override string NativeSDKVersion => string.Empty;

        /// <inheritdoc cref="ChartboostMediationBase.TestMode"/>
        public override bool TestMode { get; set; }
        
        /// <inheritdoc cref="ChartboostMediationBase.DiscardOverSizedAds"/>
        public override bool DiscardOverSizedAds { get; set; }

        /// <inheritdoc cref="ChartboostMediationBase.AdaptersInfo"/>
        public override AdapterInfo[] AdaptersInfo => Array.Empty<AdapterInfo>();

        /// <inheritdoc cref="ChartboostMediation.SetPreInitializationConfiguration"/>
        public override ChartboostMediationError? SetPreInitializationConfiguration(ChartboostMediationPreInitializationConfiguration configuration)
        {
            base.SetPreInitializationConfiguration(configuration);
            return null;
        }

        /// <inheritdoc cref="ChartboostMediationBase.LoadFullscreenAd"/>
        public override Task<FullscreenAdLoadResult> LoadFullscreenAd(FullscreenAdLoadRequest request) 
            => Task.FromResult(new FullscreenAdLoadResult(new ChartboostMediationError(Errors.ErrorNotReady)));

        /// <inheritdoc cref="ChartboostMediationBase.GetFullscreenAdQueue"/>
        public override IFullscreenAdQueue GetFullscreenAdQueue(string placementName)
        {
            var nativeQueue = IntPtr.Zero;
            var queue = (FullscreenAdQueueDefault)AdCache.GetAd(nativeQueue.ToInt64());
            if (queue != null)
                return queue;

            queue = new FullscreenAdQueueDefault(nativeQueue);
            return queue;
        }
        
        /// <inheritdoc cref="ChartboostMediationBase.LoadFullscreenAd"/>
        public override IBannerAd GetBannerAd()
            => new BannerAdDefault();
    }
}
