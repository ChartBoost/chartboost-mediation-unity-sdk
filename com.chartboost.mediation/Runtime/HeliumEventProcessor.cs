using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Helium
{
    public static class HeliumEventProcessor
    {
        private static SynchronizationContext _context;
        
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event HeliumEvent UnexpectedSystemErrorDidOccur;

        /// <summary>
        /// Initializes Helium Event Processor, must be called from main thread.
        /// </summary>
        internal static void Initialize()
        {
            _context = SynchronizationContext.Current;
        }

        public static void ProcessEventWithILRD(string dataString, HeliumILRDEvent ilrdEvent)
        {
            if (ilrdEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    if (!(HeliumJson.Deserialize(dataString) is Dictionary<object, object> data)) 
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

        public static void ProcessEventWithPartnerInitializationData(string dataString, HeliumPartnerInitializationEvent partnerInitializationEvent)
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

        public static void ProcessHeliumEvent(string error, HeliumEvent heliumEvent)
        {
            if (heliumEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    heliumEvent(error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }
        public static void ProcessHeliumPlacementEvent(string placementName, string error, HeliumPlacementEvent placementEvent)
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

        public static void ProcessHeliumLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string error, HeliumPlacementLoadEvent bidEvent)
        {
            if (bidEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    var bidInfo = new HeliumBidInfo(auctionId, partnerId, price);
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