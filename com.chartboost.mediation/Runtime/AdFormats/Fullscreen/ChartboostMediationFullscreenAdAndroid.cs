#if UNITY_ANDROID
using System.Threading.Tasks;
using Chartboost.Placements;
using Chartboost.Platforms.Android;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;

namespace Chartboost.AdFormats.Fullscreen
{
    public sealed class ChartboostMediationFullscreenAdAndroid : IChartboostMediationFullscreenAd
    {
        private readonly int _hashCode;
        private bool _isValid = true;
        private readonly AndroidJavaObject _chartboostMediationFullscreenAd;
        
        public ChartboostMediationFullscreenAdAndroid(AndroidJavaObject fullscreenAd, ChartboostMediationFullscreenAdLoadRequest request)
        {
            _hashCode = fullscreenAd.HashCode();
            _chartboostMediationFullscreenAd = fullscreenAd;
            var bidInfoMap = _chartboostMediationFullscreenAd.Get<AndroidJavaObject>("winningBidInfo");
            if (bidInfoMap != null)
                WinningBidInfo = bidInfoMap.MapToWinningBidInfo();
            Request = request;
            LoadId = _chartboostMediationFullscreenAd.Get<string>("loadId");
            CacheManager.TrackFullscreenAd(_hashCode, this);
        }

        public ChartboostMediationFullscreenAdLoadRequest Request { get; }

        public string CustomData
        {
            get => _chartboostMediationFullscreenAd.Get<string>("customData");
            set => _chartboostMediationFullscreenAd.Set("customData", value);
        }
        
        public string LoadId { get ;}

        public BidInfo WinningBidInfo { get; }

        public async Task<ChartboostMediationAdShowResult> Show()
        {
            var adShowListenerAwaitableProxy = new ChartboostMediationAndroid.ChartboostMediationFullscreenAdShowListener();
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            unityBridge.CallStatic("showFullscreenAd", _chartboostMediationFullscreenAd, adShowListenerAwaitableProxy);
            return await adShowListenerAwaitableProxy;
        }

        public void Invalidate()
        {
            if (!_isValid)
                return;

            _isValid = true;
            _chartboostMediationFullscreenAd.Call("invalidate");
            _chartboostMediationFullscreenAd.Dispose();
            CacheManager.ReleaseFullscreenAd(_hashCode);
        }

        ~ChartboostMediationFullscreenAdAndroid()
        {
            Invalidate();
        }
    }
}
#endif
