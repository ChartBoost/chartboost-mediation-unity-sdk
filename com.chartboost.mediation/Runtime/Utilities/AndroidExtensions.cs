#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using Chartboost.AdFormats.Banner;
using Chartboost.Events;
using Chartboost.Platforms.Android;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Utilities
{
    internal static class AndroidExtensions
    {
        private const string FunToString = "toString";
        private const string FunHashCode = "hashCode";
        private const string FunGet = "get";
        private const string FunSet = "set";
        private const string FunToInitializationOptions = "toInitializationOptions";
        
        private const string PropertyCode = "code";
        private const string PropertyError = "error";
        private const string PropertyPartnerId = "partner_id";
        private const string PropertyAuctionId = "auction-id";
        private const string PropertyLineItemId = "line_item_id";
        private const string PropertyLineItemName = "line_item_name";
        private const string PropertyPrice = "line_item_name";
        private const string PropertyName = "name";
        private const string PropertyWidth = "width";
        private const string PropertyHeight = "height";
        private const string PropertyPlacementId = "placementId";
        private const string PropertyILRDInfo = "ilrdInfo";
        private const string PropertyData = "data";

        private const string BannerSizeStandard = "STANDARD";
        private const string BannerSizeMedium = "MEDIUM";
        private const string BannerSizeLeaderboard = "LEADERBOARD";
        private const string BannerSizeAdaptive = "ADAPTIVE";
        
        private const string NativeClassKeywords = "com.chartboost.heliumsdk.domain.Keywords";
        
        #nullable enable
        public static ChartboostMediationError? ToChartboostMediationError(this AndroidJavaObject objectWithErrorField, string field = PropertyError)
        {
            var error = objectWithErrorField.Get<AndroidJavaObject>(field);
            if (error == null)
                return null;
            
            var code = error.Get<string>(PropertyCode);
            var message = error.Call<string>(FunToString);
            return new ChartboostMediationError(code, message);
        }
        #nullable  disable
        
        public static BidInfo MapToWinningBidInfo(this AndroidJavaObject map)
        {
            var partnerId = map.Call<string>(FunGet,PropertyPartnerId);
            var auctionId = map.Call<string>(FunGet, PropertyAuctionId);
            var lineItemId = map.Call<string>(FunGet, PropertyLineItemId);
            var lineItemName = map.Call<string>(FunGet, PropertyLineItemName);
            var price = map.Call<string>(FunGet, PropertyPrice);

            if (!double.TryParse(price, out var priceAsDouble))
                EventProcessor.ReportUnexpectedSystemError("[ToWinningBidInfo] failed to parse bid info price, defaulting to 0");
            var biddingInfo = new BidInfo(partnerId, auctionId, priceAsDouble, lineItemName, lineItemId);
            return biddingInfo;
        }

        public static ChartboostMediationBannerSize ToChartboostMediationBannerSize(this AndroidJavaObject source)
        {
            if (source == null)
                return new ChartboostMediationBannerSize();
            
            var name = source.Get<string>(PropertyName);
            var width = source.Get<int>(PropertyWidth);
            var height = source.Get<int>(PropertyHeight);
            var size = name switch
            {
                BannerSizeStandard => ChartboostMediationBannerSize.Standard, 
                BannerSizeMedium => ChartboostMediationBannerSize.MediumRect,
                BannerSizeLeaderboard => ChartboostMediationBannerSize.Leaderboard,
                BannerSizeAdaptive => ChartboostMediationBannerSize.Adaptive(width, height),
                _ => throw new ArgumentOutOfRangeException()
            };

            // if we get adaptive size of size 0X0 then it is undefined/unknown
            if (size is { SizeType: ChartboostMediationBannerSizeType.Adaptive, Width: 0, Height: 0 })
                size.SizeType = ChartboostMediationBannerSizeType.Unknown;
            
            return size;
        }
        
        public static string ImpressionDataToJsonString(this AndroidJavaObject impressionData)
        {
            var placementName = impressionData.Get<string>(PropertyPlacementId);
            var ilrdJson = impressionData.Get<AndroidJavaObject>(PropertyILRDInfo).Call<string>(FunToString);
            return $"{{\"placementName\" : \"{placementName}\", \"ilrd\" : {ilrdJson}}}";
        }

        public static string PartnerInitializationDataToJsonString(this AndroidJavaObject partnerInitializationData) 
            => partnerInitializationData.Get<AndroidJavaObject>(PropertyData).Call<string>(FunToString);

        public static AndroidJavaObject ArrayToInitializationOptions(this string[] source)
        {
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            return unityBridge.CallStatic<AndroidJavaObject>(FunToInitializationOptions, string.Empty, source);
        }

        #nullable enable
        public static Metrics? JsonObjectToMetrics(this AndroidJavaObject source)
        {
            var jsonString = source.Call<string>(FunToString);
            var metrics = JsonConvert.DeserializeObject<Metrics>(jsonString);
            return metrics;
        }
        #nullable disable

        public static AndroidJavaObject ToKeywords(this Dictionary<string, string> source)
        {
            var keywords = new AndroidJavaObject(NativeClassKeywords);
            if (source == null || source.Count == 0)
                return keywords;

            foreach (var kvp in source)
            {
               var isSet = keywords.Call<bool>(FunSet, kvp.Key, kvp.Value);
               if (!isSet)
                   EventProcessor.ReportUnexpectedSystemError($"[Keywords] failed to set the following keyword: {kvp.Key}, with value: {kvp.Value}");
            }
            
            return keywords;
        }

        public static int HashCode(this AndroidJavaObject source) => source.Call<int>(FunHashCode);
    }
}
#endif
