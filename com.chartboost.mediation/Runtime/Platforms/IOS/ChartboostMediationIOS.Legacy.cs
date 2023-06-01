#if UNITY_IOS
using System.Runtime.InteropServices;
using AOT;
// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms.IOS
{
    public sealed partial class ChartboostMediationIOS
    {
        [DllImport("__Internal")]
        private static extern void _setInterstitialCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback,
            ExternChartboostMediationPlacementEvent DidShowCallback, ExternChartboostMediationPlacementEvent DidCloseCallback,
            ExternChartboostMediationPlacementEvent DidClickCallback, ExternChartboostMediationPlacementEvent DidRecordImpression);

        [DllImport("__Internal")]
        private static extern void _setRewardedCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback,
            ExternChartboostMediationPlacementEvent DidShowCallback, ExternChartboostMediationPlacementEvent DidCloseCallback, ExternChartboostMediationPlacementEvent DidClickCallback,
            ExternChartboostMediationPlacementEvent DidRecordImpression, ExternChartboostMediationPlacementEvent DidReceiveReward);
        
        #region Interstitial Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string error)
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidShowInterstitial(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidCloseInterstitial(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickInterstitial(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionInterstitial(string placementName, string error)
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidRecordImpressionInterstitial);
        
        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial;
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidShowRewarded(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidCloseRewarded(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseRewarded);
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickRewarded(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionRewarded(string placementName, string error)
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidRecordImpressionRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidReceiveReward(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidReceiveReward);
        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion
    }
}
#endif
