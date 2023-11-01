using System;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.Requests;

namespace Chartboost.Platforms
{
    [Obsolete("IChartboostMediationInterstitialEvents, IChartboostMediationRewardedEvents, IChartboostMediationBannerEvents has been deprecated, use the new fullscreen API instead.")]
    internal class ChartboostMediationUnsupported : ChartboostMediationExternal
    {
        private static string _userIdentifier;
        private static string _initializationError = "Chartboost Mediation is only supported in Android & iOS platforms.";
        
        public ChartboostMediationUnsupported()
        {
            LogTag = "ChartboostMediation (Unsupported)";
        }

        [Obsolete("Init has been deprecated and will be removed in future versions of the SDK.")]
        public override void Init()
        {
            base.Init();
            IsInitialized = true;
        }

        [Obsolete("InitWithAppIdAndSignature has been deprecated, please use StartWithOptions instead")]
        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            IsInitialized = true;
            DidStart?.Invoke(_initializationError);
        }

        public override void StartWithOptions(string appId, string appSignature, string[] initializationOptions = null)
        {
            base.StartWithOptions(appId, appSignature, initializationOptions);
            IsInitialized = true;
            DidStart?.Invoke(_initializationError);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            _userIdentifier = userIdentifier;
        }
        
        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return _userIdentifier;
        }

        public override Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest loadRequest) 
            => Task.FromResult(new ChartboostMediationFullscreenAdLoadResult(new ChartboostMediationError("")));

        public override IChartboostMediationBannerView GetBannerView()
        {
            return new ChartboostMediationBannerViewUnsupported();
        }

        public override event ChartboostMediationEvent DidStart;
    }
}
