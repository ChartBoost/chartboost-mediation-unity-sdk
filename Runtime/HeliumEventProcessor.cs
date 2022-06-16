using System;
using System.Collections;
using System.Collections.Generic;

namespace Helium
{
    public static class HeliumEventProcessor
    {
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// <param name="message">A message that describes the unexpected system error.</param>
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event Action<string> UnexpectedSystemErrorDidOccur;

        public static void ProcessEventWithError(string dataString, Action<HeliumError> ev)
        {
            if (ev == null)
                return;  // only bother to do work if there are event listeners

            dataString = dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError($"Non JSON data received when processing event with error: {dataString}");
                return;
            }

            try
            {
                if (HeliumJSON.Deserialize(dataString) is not Dictionary<object, object> data)
                {
                    ReportUnexpectedSystemError($"Malformed data received when processing event with error: {dataString}");
                    return;
                }

                data.TryGetValue("errorCode", out var errorCode);
                data.TryGetValue("errorDescription", out var errorDescription);

                var error = ImpressionErrorFromIntString(errorCode, errorDescription as string);
                ev(error);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError($"Malformed data received when processing event with error: {e.Message}");
            }
        }

        public static bool ProcessEventWithPlacementAndError(string dataString, Action<string, HeliumError> ev)
        {
            if (ev == null)
                return false;  // only bother to do work if there are event listeners

            dataString = dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError($"Non JSON data received when processing event with placement: {dataString}");
                return false;
            }

            try
            {
                if (HeliumJSON.Deserialize(dataString) is not Dictionary<object, object> data)
                {
                    ReportUnexpectedSystemError($"Malformed data received when processing event with placement: {dataString}");
                    return false;
                }

                data.TryGetValue("errorCode", out var errorCode);
                data.TryGetValue("errorDescription", out var errorDescription);

                var error = ImpressionErrorFromIntString(errorCode, errorDescription as string);

                if (!data.TryGetValue("placementName", out var placementName))
                {
                    ReportUnexpectedSystemError($"Placement name not provided at root of: {dataString}");
                    return false;
                }

                ev(placementName as string, error);

                return (error == null);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError($"Malformed data received when processing event with placement: {e.Message}");
                return false;
            }
        }

        public static void ProcessEventWithReward(string dataString, Action<string> ev)
        {
            if (ev == null)
                return;  // only bother to do work if there are event listeners

            dataString = dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError($"Non JSON data received when processing event with reward: {dataString}");
                return;
            }

            try
            {
                if (HeliumJSON.Deserialize(dataString) is not Dictionary<object, object> data)
                {
                    ReportUnexpectedSystemError($"Malformed data received when processing event with reward: {dataString}");
                    return;
                }

                if (!data.TryGetValue("reward", out var reward))
                {
                    ReportUnexpectedSystemError($"Reward object not included with JSON payload: {dataString}");
                    return;
                }

                ev(reward.ToString());
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError($"Malformed data received when processing event with reward: {e.Message}");
            }
        }

        public static void ProcessEventWithPlacementAndBidInfo(string dataString, Action<string, HeliumBidInfo> ev)
        {
            if (ev == null)
                return;  // only bother to do work if there are event listeners

            dataString = dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError($"Non JSON data received when processing event with placement and bid info: {dataString}");
                return;
            }

            try
            {
                if (HeliumJSON.Deserialize(dataString) is not Dictionary<object, object> data)
                {
                    ReportUnexpectedSystemError($"Malformed data received when processing event with placement and bid info: {dataString}");
                    return;
                }

                if (!data.TryGetValue("placementName", out var placementName))
                {
                    ReportUnexpectedSystemError($"Placement name not provided at root of: {dataString}");
                    return;
                }

                data.TryGetValue("info", out var infoObject);
                var infoJObject = infoObject as Newtonsoft.Json.Linq.JObject;

                var info = new HeliumBidInfo();
                if (infoJObject != null)
                {
                    infoJObject.TryGetValue("auction-id", out var auctionID);
                    if (auctionID != null)
                        info.AuctionId = auctionID.ToString();

                    infoJObject.TryGetValue("price", out var price);
                    if (price != null)
                        double.TryParse(price.ToString(), out info.Price);

                    infoJObject.TryGetValue("seat", out var seat);
                    if (seat != null)
                        info.Seat = seat.ToString();

                    infoJObject.TryGetValue("placementName", out var partnerPlacementName);
                    if (partnerPlacementName != null)
                        info.PartnerPlacementName = partnerPlacementName.ToString();
                }
                ev(placementName as string, info);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError($"Malformed data received when processing event with placement and bid info: {e.Message}");
            }
        }

        public static void ProcessEventWithILRD(string dataString, Action<string, Hashtable> ev)
        {
            if (ev == null)
                return;

            if (HeliumJSON.Deserialize(dataString) is not Dictionary<object, object> data)
                return;

            data.TryGetValue("placementName", out var placementName);
            ev(placementName as string, new Hashtable(data));
        }

        private static void ReportUnexpectedSystemError(string message)
        {
            UnexpectedSystemErrorDidOccur?.Invoke(message);
        }

        private static HeliumError ImpressionErrorFromInt(object errorObj)
        {
            int error;
            try
            {
                error = Convert.ToInt32(errorObj);
            }
            catch
            {
                error = (int)HeliumErrorCode.Unknown;
            }

            switch (error)
            {
                case -1:
                    return null;
                case < 0:
                // out of bounds
                case > (int)HeliumErrorCode.Unknown:
                    return new HeliumError(HeliumErrorCode.Unknown, null);
                default:
                    return new HeliumError((HeliumErrorCode)error, null);
            }
        }

        private static HeliumError ImpressionErrorFromIntString(object errorObj, string errString)
        {
            var e = ImpressionErrorFromInt(errorObj);
            if (e != null)
                e.errorDescription = errString;
            return e;
        }
    }
}
