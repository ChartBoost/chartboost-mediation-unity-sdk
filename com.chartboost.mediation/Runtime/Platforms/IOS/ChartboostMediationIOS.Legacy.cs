#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using Chartboost.Events;
using Chartboost.Utilities;

// ReSharper disable InconsistentNaming
namespace Chartboost.Platforms.IOS
{
    internal sealed partial class ChartboostMediationIOS
    {
        [DllImport(IOSConstants.Internal)]
        private static extern void _setInterstitialCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback,
            ExternChartboostMediationPlacementEvent DidShowCallback, ExternChartboostMediationPlacementEvent DidCloseCallback,
            ExternChartboostMediationPlacementEvent DidClickCallback, ExternChartboostMediationPlacementEvent DidRecordImpression);

        [DllImport(IOSConstants.Internal)]
        private static extern void _setRewardedCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback,
            ExternChartboostMediationPlacementEvent DidShowCallback, ExternChartboostMediationPlacementEvent DidCloseCallback, ExternChartboostMediationPlacementEvent DidClickCallback,
            ExternChartboostMediationPlacementEvent DidRecordImpression, ExternChartboostMediationPlacementEvent DidReceiveReward);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _setBannerCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback, ExternChartboostMediationPlacementEvent DidRecordImpression, ExternChartboostMediationPlacementEvent DidClickCallback);
        
        #region Interstitial Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error)
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, lineItemName, lineItemId, error, _instance.DidLoadInterstitial);

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
        
        [Obsolete("DidLoadInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        [Obsolete("DidShowInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        [Obsolete("DidCloseInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial;
        [Obsolete("DidClickInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        [Obsolete("DidRecordImpressionInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error) 
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, lineItemName, lineItemId, error, _instance.DidLoadRewarded);

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
        
        [Obsolete("DidLoadRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        [Obsolete("DidShowRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        [Obsolete("DidCloseRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        [Obsolete("DidClickRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        [Obsolete("DidRecordImpressionRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        [Obsolete("DidReceiveReward has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion
        
        #region Banner Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error) 
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, lineItemName, lineItemId, error, _instance.DidLoadBanner);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickBanner(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickBanner);
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionBanner(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error,  _instance.DidRecordImpressionBanner);

        [Obsolete("DidLoadBanner has been deprecated, use the new ChartboostMediationBannerView API instead.")]
        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        [Obsolete("DidClickBanner has been deprecated, use the new ChartboostMediationBannerView API instead.")]
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        [Obsolete("DidRecordImpressionBanner has been deprecated, use the new ChartboostMediationBannerView API instead.")]
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif
