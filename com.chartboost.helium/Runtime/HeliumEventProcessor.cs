using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
                    if (!(HeliumJSON.Deserialize(dataString) is Dictionary<object, object> data)) 
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

        public static void ProcessHeliumEvent(int errorCode, string errorDescription, HeliumEvent heliumEvent)
        {
            if (heliumEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    var heliumError = HeliumError.ErrorFromIntString(errorCode, errorDescription);
                    heliumEvent(heliumError);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessHeliumPlacementEvent(string placementName, int errorCode, string errorDescription, HeliumPlacementEvent placementEvent)
        {
            if (placementEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    var heliumError = HeliumError.ErrorFromIntString(errorCode, errorDescription);
                    placementEvent(placementName, heliumError);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessHeliumBidEvent(string placementName, string auctionId, string partnerId, double price, HeliumBidEvent bidEvent)
        {
            if (bidEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    var heliumBid = new HeliumBidInfo(auctionId, partnerId, price);
                    bidEvent(placementName, heliumBid);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessHeliumRewardEvent(string placementName, int reward, HeliumRewardEvent rewardEvent)
        {
            if (rewardEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    rewardEvent(placementName, reward);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        private static void ReportUnexpectedSystemError(string message)
        {
            UnexpectedSystemErrorDidOccur?.Invoke(HeliumError.ErrorFromIntString(HeliumErrorCode.Unknown, message));
        }
    }
}
