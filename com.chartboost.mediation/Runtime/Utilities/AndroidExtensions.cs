using System.Collections.Generic;
using Chartboost.Placements;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Utilities
{
    internal static class AndroidExtensions
    {
        
        public static IChartboostMediationFullscreenAd FullscreenFromAdResult(this AndroidJavaObject androidJavaObject, ChartboostMediationFullscreenAdLoadRequest request)
        {
            var nativeFullScreenAd = androidJavaObject.Get<AndroidJavaObject>("ad");
            return nativeFullScreenAd == null ? null : new ChartboostMediationFullscreenAdAndroid(nativeFullScreenAd, request);
        }
        
        #nullable enable

        public static ChartboostMediationError? ToChartboostMediationError(this AndroidJavaObject objectWithErrorField, string field = "error")
        {
            var error = objectWithErrorField.Get<AndroidJavaObject>(field);
            if (error == null)
                return null;
            
            var code = error.Get<string>("code");
            var message = error.Call<string>("toString");
            return new ChartboostMediationError(code, message);
        }
        #nullable  disable
        
        public static BidInfo ToWinningBidInfo(this AndroidJavaObject map)
        {
            var partnerId = map.Call<string>("get","partner_id");
            var auctionId = map.Call<string>("get", "auction-id");
            var price = map.Call<string>("get", "price");

            if (!double.TryParse(price, out var priceAsDouble))
                EventProcessor.ReportUnexpectedSystemError("[ToWinningBidInfo] failed to parse bid info price, defaulting to 0");
            var biddingInfo = new BidInfo(partnerId, auctionId, priceAsDouble);
            return biddingInfo;
        }
        
        #nullable enable
        public static Metrics? JsonObjectToMetrics(this AndroidJavaObject source)
        {
            var jsonString = source.Call<string>("toString");
            var metrics = JsonConvert.DeserializeObject<Metrics>(jsonString);
            return metrics;
        }
        #nullable disable

        public static AndroidJavaObject ToKeywords(this Dictionary<string, string> source)
        {
            var keywords = new AndroidJavaObject("com.chartboost.heliumsdk.domain.Keywords");
            if (source == null || source.Count == 0)
                return keywords;

            foreach (var kvp in source)
            {
               var isSet = keywords.Call<bool>("set", kvp.Key, kvp.Value);
               if (!isSet)
                   EventProcessor.ReportUnexpectedSystemError($"[Keywords] failed to set the following keyword: {kvp.Key}, with value: {kvp.Value}");
            }

            return keywords;
        }
    }
}
