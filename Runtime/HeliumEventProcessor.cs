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
        private static readonly SynchronizationContext _context;
        
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event HeliumEvent UnexpectedSystemErrorDidOccur;

        static HeliumEventProcessor()
        {
            _context = SynchronizationContext.Current;
        }
        
        public static void ProcessEventWithILRD(string dataString, HeliumILRDEvent ilrdEvent)
        {
            _context.Post(o =>
            {
                try
                {
                    if (ilrdEvent == null)
                        return;
                    if (HeliumJSON.Deserialize(dataString) is not Dictionary<object, object> data)
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

        public static void ProcessHeliumEvent(int errorCode, string errorDescription, HeliumEvent heliumEvent)
        {
            _context.Post(o =>
            {
                try
                {
                    if (heliumEvent == null)
                        return;
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
            _context.Post(o =>
            {
                try
                {
                    if (placementEvent == null)
                        return;
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
            _context.Post(o =>
            {
                try
                {
                    if (bidEvent == null)
                        return;
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
            _context.Post(o =>
            {
                try
                {
                    if (rewardEvent == null)
                        return;
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
