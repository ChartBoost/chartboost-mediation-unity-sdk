#if UNITY_ANDROID
using System.Collections.Generic;
using Chartboost.Placements;
using Chartboost.Platforms.Android;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Utilities
{
    internal static class AndroidExtensions
    {
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
        
        public static BidInfo MapToWinningBidInfo(this AndroidJavaObject map)
        {
            var partnerId = map.Call<string>("get","partner_id");
            var auctionId = map.Call<string>("get", "auction-id");
            var lineItemId = map.Call<string>("get", "line_item_id");
            var lineItemName = map.Call<string>("get", "line_item_name");
            var price = map.Call<string>("get", "price");

            if (!double.TryParse(price, out var priceAsDouble))
                EventProcessor.ReportUnexpectedSystemError("[ToWinningBidInfo] failed to parse bid info price, defaulting to 0");
            var biddingInfo = new BidInfo(partnerId, auctionId, priceAsDouble, lineItemName, lineItemId);
            return biddingInfo;
        }

        public static string ImpressionDataToJsonString(this AndroidJavaObject impressionData)
        {
            var placementName = impressionData.Get<string>("placementId");
            var ilrdJson = impressionData.Get<AndroidJavaObject>("ilrdInfo").Call<string>("toString");
            return $"{{\"placementName\" : \"{placementName}\", \"ilrd\" : {ilrdJson}}}";
        }

        public static string PartnerInitializationDataToJsonString(this AndroidJavaObject partnerInitializationData) 
            => partnerInitializationData.Get<AndroidJavaObject>("data").Call<string>("toString");

        public static AndroidJavaObject ArrayToInitializationOptions(this string[] source)
        {
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            return unityBridge.CallStatic<AndroidJavaObject>("toInitializationOptions", string.Empty, source);
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

        public static int HashCode(this AndroidJavaObject source) => source.Call<int>("hashCode");
    }
}
#endif
