using System.Threading.Tasks;
using Chartboost.Platforms.Android;
using Chartboost.Utilities;
using UnityEngine;

namespace Chartboost.Placements
{
    public sealed class ChartboostMediationFullscreenAdAndroid : IChartboostMediationFullscreenAd
    {
        public ChartboostMediationFullscreenAdAndroid(AndroidJavaObject fullscreenAd, ChartboostMediationFullscreenAdLoadRequest request)
        {
            _chartboostMediationFullscreenAd = fullscreenAd;
            var bidInfoMap = _chartboostMediationFullscreenAd.Get<AndroidJavaObject>("winningBidInfo");
            if (bidInfoMap != null)
                WinningBidInfo = bidInfoMap.ToWinningBidInfo();
            Request = request;
        }

        private readonly AndroidJavaObject _chartboostMediationFullscreenAd;

        public ChartboostMediationFullscreenAdLoadRequest Request { get; }

        public string AuctionId => _chartboostMediationFullscreenAd.Get<AndroidJavaObject>("cachedAd").Get<string>("auctionId");

        public string CustomData
        {
            get => _chartboostMediationFullscreenAd.Get<string>("customData");
            set => _chartboostMediationFullscreenAd.Set("customData", value);
        }
        
        public string RequestId => _chartboostMediationFullscreenAd.Get<string>("requestId");
        public BidInfo WinningBidInfo { get; }
        public async Task<ChartboostMediationAdShowResult> Show()
        {
            var awaitableProxy = new ChartboostMediationAndroid.CMAdShowResultHandler();
            ChartboostMediationAndroid.UnityBridge.Call("showFullscreenAd", _chartboostMediationFullscreenAd, awaitableProxy);
            return await awaitableProxy;
        }

        public void Invalidate()
        {
            _chartboostMediationFullscreenAd.Call("invalidate");
        }
    }
}
