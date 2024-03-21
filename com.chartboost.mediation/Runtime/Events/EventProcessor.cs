using System;
using System.Collections;
using System.Collections.Generic;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.AdFormats.Fullscreen.Queue;
using Chartboost.Requests;
using Chartboost.Utilities;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Chartboost.Events
{
    public static class EventProcessor
    {
        internal enum FullscreenAdEvents
        {
            RecordImpression = 0,
            Click = 1,
            Reward = 2,
            Close = 3,
            Expire = 4
        }

        internal enum BannerAdEvents
        {
            Load = 0,
            Click = 1,
            RecordImpression = 2,
            Drag = 3
        }
        
        internal enum FullscreenAdQueueEvents
        {
            Update = 0,
            RemoveExpiredAd = 1
        }

        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event ChartboostMediationEvent UnexpectedSystemErrorDidOccur;
        
        public static void ProcessEventWithILRD(string dataString, ChartboostMediationILRDEvent ilrdEvent)
        {
            if (ilrdEvent == null)
                return;
            
            MainThreadDispatcher.Post(o =>
            {
                try
                {
                    if (!(JsonTools.Deserialize(dataString) is Dictionary<object, object> data))
                        return;

                    data.TryGetValue("placementName", out var placementName);
                    ilrdEvent(placementName as string, new Hashtable(data));
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            });
        }

        public static void ProcessEventWithPartnerInitializationData(string dataString, ChartboostMediationPartnerInitializationEvent partnerInitializationEvent)
        {
            if (partnerInitializationEvent == null)
                return;
            
            MainThreadDispatcher.Post(o =>
            {
                try
                {
                    partnerInitializationEvent(dataString);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            });
        }

        public static void ProcessChartboostMediationEvent(string error, ChartboostMediationEvent chartboostMediationEvent)
        {
            if (chartboostMediationEvent == null)
                return;
            
            MainThreadDispatcher.Post(o =>
            {
                try
                {
                    chartboostMediationEvent(error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            });
        }
        
        public static void ProcessChartboostMediationPlacementEvent(string placementName, string error, ChartboostMediationPlacementEvent placementEvent)
        {
            if (placementEvent == null)
                return;
            
            MainThreadDispatcher.Post(o =>
            {
                try
                {
                    placementEvent(placementName, error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            });
        }

        public static void ProcessChartboostMediationLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error, ChartboostMediationPlacementLoadEvent bidEvent)
        {
            if (bidEvent == null)
                return;
            
            MainThreadDispatcher.Post(o =>
            {
                try
                {
                    var bidInfo = new BidInfo(auctionId, partnerId, price, lineItemName, lineItemId);
                    bidEvent(placementName, loadId, bidInfo, error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            });
        }

        public static void ProcessChartboostMediationBannerEvent(long adHashCode, int eventType, float x = default, float y = default)
        {
            MainThreadDispatcher.Post(o =>
            {
                try
                {
                    var ad = (ChartboostMediationBannerViewBase)CacheManager.GetBannerAd(adHashCode);
                    
                    // Ad event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                    if (ad == null)
                        return;
                    
                    switch ((BannerAdEvents)eventType)
                    {
                        case BannerAdEvents.Load : 
                            ad.OnBannerDidLoad(ad);
                            break;
                        case BannerAdEvents.Click :
                            ad.OnBannerClick(ad);
                            break;
                        case BannerAdEvents.RecordImpression:
                            ad.OnBannerRecordImpression(ad);
                            break;
                        case BannerAdEvents.Drag:
                            ad.OnBannerDrag(ad, x, y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
                    }
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            });
        }
        
        public static void ProcessEvent(Action customEvent)
        {
            MainThreadDispatcher.Post(o =>
            {
                customEvent?.Invoke();
            });   
        }
        
        public static void ProcessFullscreenEvent(long adHashCode, int eventType, string code, string message)
        {
            MainThreadDispatcher.Post(o =>
            {
                var ad = CacheManager.GetFullscreenAd(adHashCode);

                // Ad event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                if (ad == null)
                    return;

                var type = (FullscreenAdEvents)eventType;
                
                ChartboostMediationError? error = null;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                    error = new ChartboostMediationError(code, message);
                
                switch (type)
                {
                    case FullscreenAdEvents.RecordImpression:
                        ad.Request?.OnRecordImpression(ad);
                        ((ChartboostMediationFullscreenAdBase)ad).OnRecordImpression(ad);
                        break;
                    case FullscreenAdEvents.Click:
                        ad.Request?.OnClick(ad);
                        ((ChartboostMediationFullscreenAdBase)ad).OnClick(ad);
                        break;
                    case FullscreenAdEvents.Reward:
                        ad.Request?.OnReward(ad);
                        ((ChartboostMediationFullscreenAdBase)ad).OnReward(ad);
                        break;
                    case FullscreenAdEvents.Expire:
                        ad.Request?.OnExpire(ad);
                        ((ChartboostMediationFullscreenAdBase)ad).OnExpire(ad);
                        CacheManager.ReleaseFullscreenAd(adHashCode);
                        break;
                    case FullscreenAdEvents.Close:
                        ((ChartboostMediationFullscreenAdBase)ad).OnClose(ad, error);
                        ad.Request?.OnClose(ad, error);
                        CacheManager.ReleaseFullscreenAd(adHashCode);
                        break;
                    default:
                        return;
                }
            });
        }
        
        public static void ProcessFullscreenAdQueueEvent(long queueHashCode, int eventType, ChartboostMediationAdLoadResult adLoadResult, int numberOfAdsReady)
        {
            MainThreadDispatcher.Post(o =>
            {
                var queue = CacheManager.GetFullscreenAdQueue(queueHashCode);

                // Queue event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                if (queue == null)
                    return;

                switch ((FullscreenAdQueueEvents)eventType)
                {
                    case FullscreenAdQueueEvents.Update :
                        ((ChartboostMediationFullscreenAdQueueBase)queue).OnFullscreenAdQueueUpdated(queue, adLoadResult, numberOfAdsReady);
                        break;
                    case FullscreenAdQueueEvents.RemoveExpiredAd:
                        ((ChartboostMediationFullscreenAdQueueBase)queue).OnFullscreenAdQueueDidRemoveExpiredAd(queue, numberOfAdsReady);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
                }
            });
        }

        internal static void ReportUnexpectedSystemError(string message) 
            => MainThreadDispatcher.Post(o => UnexpectedSystemErrorDidOccur?.Invoke(message));
    }
}
