using System;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Helium
{
    public static class HeliumEventProcessor
    {
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event HeliumEvent UnexpectedSystemErrorDidOccur;
        public static void ProcessEventWithILRD(string dataString, HeliumILRDEvent ilrdEvent)
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
        }

        public static void ProcessHeliumEvent(int errorCode, string errorDescription, HeliumEvent heliumEvent)
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
        }

        public static void ProcessHeliumPlacementEvent(string placementName, int errorCode, string errorDescription, HeliumPlacementEvent placementEvent)
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
        }

        public static void ProcessHeliumBidEvent(string placementName, string auctionId, double price, string seat, HeliumBidEvent bidEvent)
        {
            try
            {
                if (bidEvent == null)
                    return;

                var heliumBid = new HeliumBidInfo(placementName, auctionId, price, seat);
                bidEvent(placementName, heliumBid);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError(e.ToString());
            }
        }

        public static void ProcessHeliumRewardEvent(string reward, HeliumRewardEvent rewardEvent)
        {
            try
            {
                if (rewardEvent == null)
                    return;
                rewardEvent(reward);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError(e.ToString());
            }
        }

        private static void ReportUnexpectedSystemError(string message)
        {
            UnexpectedSystemErrorDidOccur?.Invoke(HeliumError.ErrorFromIntString(4, message));
        }
    }
}
