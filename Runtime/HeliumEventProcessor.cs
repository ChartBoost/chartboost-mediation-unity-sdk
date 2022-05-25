using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helium
{
    public class HeliumEventProcessor
    {
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// <param name="message">A message that describes the unexpected system error.</param>
        /// </summary>
        public static event Action<string> UnexpectedSystemErrorDidOccur;

        public void ProcessEventWithError(string dataString, Action<HeliumError> ev)
        {
            if (ev == null)
                return;  // only bother to do work if there are event listeners

            dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError(String.Format("Non JSON data received when processing event with error: {0}", dataString));
                return;
            }

            try
            {
                Dictionary<object, object> data = HeliumJSON.Deserialize(dataString) as Dictionary<object, object>;
                if (data == null)
                {
                    ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with error: {0}", dataString));
                    return;
                }

                object errorCode;
                data.TryGetValue("errorCode", out errorCode);
                object errorDescription;
                data.TryGetValue("errorDescription", out errorDescription);

                HeliumError error = ImpressionErrorFromIntString(errorCode, errorDescription as string);
                ev(error);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with error: {0}", e.Message));
            }
        }

        public bool ProcessEventWithPlacementAndError(string dataString, Action<string, HeliumError> ev)
        {
            if (ev == null)
                return false;  // only bother to do work if there are event listeners

            dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError(String.Format("Non JSON data received when processing event with placement: {0}", dataString));
                return false;
            }

            try
            {
                Dictionary<object, object> data = HeliumJSON.Deserialize(dataString) as Dictionary<object, object>;
                if (data == null)
                {
                    ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with placement: {0}", dataString));
                    return false;
                }

                object errorCode;
                data.TryGetValue("errorCode", out errorCode);
                object errorDescription;
                data.TryGetValue("errorDescription", out errorDescription);

                HeliumError error = ImpressionErrorFromIntString(errorCode, errorDescription as string);

                object placementName;
                data.TryGetValue("placementName", out placementName);
                if (placementName == null)
                {
                    ReportUnexpectedSystemError(String.Format("Placement name not provided at root of: {0}", dataString));
                    return false;
                }

                ev(placementName as string, error);

                return (error == null);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with placement: {0}", e.Message));
                return false;
            }
        }

        public void ProcessEventWithReward(string dataString, Action<string> ev)
        {
            if (ev == null)
                return;  // only bother to do work if there are event listeners

            dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError(String.Format("Non JSON data received when processing event with reward: {0}", dataString));
                return;
            }

            try
            {
                Dictionary<object, object> data = HeliumJSON.Deserialize(dataString) as Dictionary<object, object>;
                if (data == null)
                {
                    ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with reward: {0}", dataString));
                    return;
                }

                object reward;
                data.TryGetValue("reward", out reward);
                if (reward == null)
                {
                    ReportUnexpectedSystemError(String.Format("Reward object not included with JSON payload: {0}", dataString));
                    return;
                }

                ev(reward.ToString());
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with reward: {0}", e.Message));
            }
        }

        public void ProcessEventWithPlacementAndBidInfo(string dataString, Action<string, HeliumBidInfo> ev)
        {
            if (ev == null)
                return;  // only bother to do work if there are event listeners

            dataString.Trim();
            if (!dataString.StartsWith("{") || !dataString.EndsWith("}"))
            {
                ReportUnexpectedSystemError(String.Format("Non JSON data received when processing event with placement and bid info: {0}", dataString));
                return;
            }

            try
            {
                Dictionary<object, object> data = HeliumJSON.Deserialize(dataString) as Dictionary<object, object>;
                if (data == null)
                {
                    ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with placement and bid info: {0}", dataString));
                    return;
                }

                object placementName;
                data.TryGetValue("placementName", out placementName);
                if (placementName == null)
                {
                    ReportUnexpectedSystemError(String.Format("Placement name not provided at root of: {0}", dataString));
                    return;
                }

                object infoObject;
                data.TryGetValue("info", out infoObject);
                Newtonsoft.Json.Linq.JObject infoJObject = infoObject as Newtonsoft.Json.Linq.JObject;

                HeliumBidInfo info = new HeliumBidInfo();
                if (infoJObject != null)
                {
                    Newtonsoft.Json.Linq.JToken auctionID;
                    infoJObject.TryGetValue("auction-id", out auctionID);
                    if (auctionID != null)
                        info.AuctionId = auctionID.ToString();

                    Newtonsoft.Json.Linq.JToken price;
                    infoJObject.TryGetValue("price", out price);
                    if (price != null)
                        Double.TryParse(price.ToString(), out info.Price);

                    Newtonsoft.Json.Linq.JToken seat;
                    infoJObject.TryGetValue("seat", out seat);
                    if (seat != null)
                        info.Seat = seat.ToString();

                    Newtonsoft.Json.Linq.JToken partnerPlacementName;
                    infoJObject.TryGetValue("placementName", out partnerPlacementName);
                    if (partnerPlacementName != null)
                        info.PartnerPlacementName = partnerPlacementName.ToString();
                }
                ev(placementName as string, info);
            }
            catch (Exception e)
            {
                ReportUnexpectedSystemError(String.Format("Malformed data received when processing event with placement and bid info: {0}", e.Message));
            }
        }

        public void ProcessEventWithILRD(string dataString, Action<string, Hashtable> ev)
        {
            if (ev == null)
                return;

            Dictionary<object, object> data = HeliumJSON.Deserialize(dataString) as Dictionary<object, object>;
            if (data == null)
                return;

            object placementName;
            data.TryGetValue("placementName", out placementName);
            ev(placementName as string, new Hashtable(data));
        }

        private static void ReportUnexpectedSystemError(string message)
        {
            if (UnexpectedSystemErrorDidOccur == null)
                return;
            UnexpectedSystemErrorDidOccur(message);
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

            if (error == -1)
                return null;

            if (error < 0 || error > (int)HeliumErrorCode.Unknown) // out of bounds
                return new HeliumError(HeliumErrorCode.Unknown, null);
            else
                return new HeliumError((HeliumErrorCode)error, null);
        }

        private static HeliumError ImpressionErrorFromIntString(object errorObj, string errString)
        {
            HeliumError e = ImpressionErrorFromInt(errorObj);
            if (e != null)
                e.errorDescription = errString;
            return e;
        }
    }
}
