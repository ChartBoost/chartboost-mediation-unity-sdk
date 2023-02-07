using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Chartboost
{
    public static class EventProcessor
    {
        private static SynchronizationContext _context;
        
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event ChartboostMediationEvent UnexpectedSystemErrorDidOccur;

        /// <summary>
        /// Initializes Helium Event Processor, must be called from main thread.
        /// </summary>
        internal static void Initialize()
        {
            _context = SynchronizationContext.Current;
        }

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

        public static void ProcessHeliumEvent(string error, ChartboostMediationEvent chartboostMediationEvent)
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
        public static void ProcessHeliumPlacementEvent(string placementName, string error, ChartboostMediationPlacementEvent placementEvent)
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

        public static void ProcessHeliumLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string error, ChartboostMediationPlacementLoadEvent bidEvent)
        {
            if (bidEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    var bidInfo = new BidInfo(auctionId, partnerId, price);
                    bidEvent(placementName, loadId, bidInfo, error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        private static void ReportUnexpectedSystemError(string message)
        {
            UnexpectedSystemErrorDidOccur?.Invoke(message);
        }
    }
}
