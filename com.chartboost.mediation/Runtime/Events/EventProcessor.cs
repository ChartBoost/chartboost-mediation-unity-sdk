using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;

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

        private static SynchronizationContext _context;
        
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event ChartboostMediationEvent UnexpectedSystemErrorDidOccur;

        /// <summary>
        /// Initializes Chartboost Mediation Event Processor, must be called from main thread.
        /// </summary>
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()=> _context ??= SynchronizationContext.Current;

        public static void ProcessEventWithILRD(string dataString, ChartboostMediationILRDEvent ilrdEvent)
        {
            if (ilrdEvent == null)
                return;
            
            _context.Post(o =>
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
            }, null);
        }

        public static void ProcessEventWithPartnerInitializationData(string dataString, ChartboostMediationPartnerInitializationEvent partnerInitializationEvent)
        {
            if (partnerInitializationEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    partnerInitializationEvent(dataString);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessChartboostMediationEvent(string error, ChartboostMediationEvent chartboostMediationEvent)
        {
            if (chartboostMediationEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    chartboostMediationEvent(error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }
        
        public static void ProcessChartboostMediationPlacementEvent(string placementName, string error, ChartboostMediationPlacementEvent placementEvent)
        {
            if (placementEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    placementEvent(placementName, error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessChartboostMediationLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error, ChartboostMediationPlacementLoadEvent bidEvent)
        {
            if (bidEvent == null)
                return;
            
            _context.Post(o =>
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
            }, null);
        }
        
        public static void ProcessEvent(Action customEvent)
        {
            _context.Post(o =>
            {
                customEvent?.Invoke();
            }, null);   
        }
        
        public static void ProcessFullscreenEvent(long adHashCode, int eventType, string code, string message)
        {
            _context.Post(o =>
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
                        break;
                    case FullscreenAdEvents.Click:
                        ad.Request?.OnClick(ad);
                        break;
                    case FullscreenAdEvents.Reward:
                        ad.Request?.OnReward(ad);
                        break;
                    case FullscreenAdEvents.Expire:
                        ad.Request?.OnExpire(ad);
                        CacheManager.ReleaseFullscreenAd(adHashCode);
                        break;
                    case FullscreenAdEvents.Close:
                        ad.Request?.OnClose(ad, error);
                        CacheManager.ReleaseFullscreenAd(adHashCode);
                        break;
                    default:
                        return;
                }
            }, null);
        }

        internal static void ReportUnexpectedSystemError(string message) 
            => _context.Post(o => UnexpectedSystemErrorDidOccur?.Invoke(message), null);
    }
}
