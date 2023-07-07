using System;
using System.Threading.Tasks;
using Chartboost.Requests;

namespace Chartboost.Platforms
{
    internal class ChartboostMediationUnsupported : ChartboostMediationExternal
    {
        private static string _userIdentifier;
        
        public ChartboostMediationUnsupported()
        {
            LogTag = "ChartboostMediation (Unsupported)";
        }

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
        }

        public override void StartWithOptions(string appId, string appSignature, string[] initializationOptions = null)
        {
            base.StartWithOptions(appId, appSignature, initializationOptions);
            IsInitialized = true;
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
            => Task.FromResult(new ChartboostMediationFullscreenAdLoadResult(new ChartboostMediationError("Unsupported Platform")));
    }
}
