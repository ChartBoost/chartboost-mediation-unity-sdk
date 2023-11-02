#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using Chartboost.AdFormats.Banner;
using Chartboost.Events;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Utilities
{
    internal static class AndroidExtensions
    {
        #nullable enable
        public static ChartboostMediationError? ToChartboostMediationError(this AndroidJavaObject objectWithErrorField, string field = AndroidConstants.PropertyError)
        {
            var error = objectWithErrorField.Get<AndroidJavaObject>(field);
            if (error == null)
                return null;
            
            var code = error.Get<string>(AndroidConstants.PropertyCode);
            var message = error.Call<string>(AndroidConstants.FunToString);
            return new ChartboostMediationError(code, message);
        }
        #nullable  disable
        
        public static BidInfo MapToWinningBidInfo(this AndroidJavaObject map)
        {
            var partnerId = map.Call<string>(AndroidConstants.FunGet, AndroidConstants.PropertyPartnerId);
            var auctionId = map.Call<string>(AndroidConstants.FunGet, AndroidConstants.PropertyAuctionId);
            var lineItemId = map.Call<string>(AndroidConstants.FunGet, AndroidConstants.PropertyLineItemId);
            var lineItemName = map.Call<string>(AndroidConstants.FunGet, AndroidConstants.PropertyLineItemName);
            var price = map.Call<string>(AndroidConstants.FunGet, AndroidConstants.PropertyPrice);

            if (!double.TryParse(price, out var priceAsDouble))
                EventProcessor.ReportUnexpectedSystemError("[ToWinningBidInfo] failed to parse bid info price, defaulting to 0");
            var biddingInfo = new BidInfo(partnerId, auctionId, priceAsDouble, lineItemName, lineItemId);
            return biddingInfo;
        }

        public static ChartboostMediationBannerSize ToChartboostMediationBannerSize(this AndroidJavaObject source)
        {
            if (source == null)
                return new ChartboostMediationBannerSize();
            
            var name = source.Get<string>(AndroidConstants.PropertyName);
            var width = source.Get<int>(AndroidConstants.PropertyWidth);
            var height = source.Get<int>(AndroidConstants.PropertyHeight);
            var size = name switch
            {
                AndroidConstants.BannerSizeStandard => ChartboostMediationBannerSize.Standard, 
                AndroidConstants.BannerSizeMedium => ChartboostMediationBannerSize.MediumRect,
                AndroidConstants.BannerSizeLeaderboard => ChartboostMediationBannerSize.Leaderboard,
                AndroidConstants.BannerSizeAdaptive => ChartboostMediationBannerSize.Adaptive(width, height),
                _ => throw new ArgumentOutOfRangeException()
            };

            // if we get adaptive size of size 0X0 then it is undefined/unknown
            if (size is { SizeType: ChartboostMediationBannerSizeType.Adaptive, Width: 0, Height: 0 })
                size.SizeType = ChartboostMediationBannerSizeType.Unknown;
            
            return size;
        }
        
        public static string ImpressionDataToJsonString(this AndroidJavaObject impressionData)
        {
            var placementName = impressionData.Get<string>(AndroidConstants.PropertyPlacementId);
            var ilrdJson = impressionData.Get<AndroidJavaObject>(AndroidConstants.PropertyIlrdInfo).Call<string>(AndroidConstants.FunToString);
            return $"{{\"placementName\" : \"{placementName}\", \"ilrd\" : {ilrdJson}}}";
        }

        public static string PartnerInitializationDataToJsonString(this AndroidJavaObject partnerInitializationData) 
            => partnerInitializationData.Get<AndroidJavaObject>(AndroidConstants.PropertyData).Call<string>(AndroidConstants.FunToString);

        public static AndroidJavaObject ToInitializationOptions(this IEnumerable<string> options)
        {
            using var hashSet = new AndroidJavaObject(AndroidConstants.ClassHashSet);
            foreach (var option in options)
            {
                if (string.IsNullOrEmpty(option))
                    continue;
                hashSet.Call<bool>(AndroidConstants.FunAdd, option);
            }
            return new AndroidJavaObject(AndroidConstants.ClassHeliumInitializationOptions, hashSet);
        }

        public static ChartboostMediationAdapterInfo[] ToAdapterInfo(this AndroidJavaObject nativeAdapterInfo)
        {
            using var iterator = nativeAdapterInfo.Call<AndroidJavaObject>(AndroidConstants.FunIterator);
            var count = nativeAdapterInfo.Call<int>(AndroidConstants.FunSize);
            
            var ret = new ChartboostMediationAdapterInfo[count];

            if (count == 0)
                return ret;
            
            var index = 0;
            
            do {
                using var entry = iterator.Call<AndroidJavaObject>(AndroidConstants.FunNext);
                var adapterVersion = entry.Call<string>(AndroidConstants.FunGetAdapterVersion);
                var partnerVersion = entry.Call<string>(AndroidConstants.FunGetPartnerVersion);
                var partnerId = entry.Call<string>(AndroidConstants.FunGetPartnerId);
                var partnerDisplayName = entry.Call<string>(AndroidConstants.FunGetPartnerDisplayName);
                var adapterInfo = new ChartboostMediationAdapterInfo(adapterVersion, partnerVersion, partnerId, partnerDisplayName);
                ret[index] = adapterInfo;
                index++;
            } while (iterator.Call<bool>(AndroidConstants.FunHasNext));
            return ret;
        }

        #nullable enable
        public static Metrics? JsonObjectToMetrics(this AndroidJavaObject source)
        {
            var jsonString = source.Call<string>(AndroidConstants.FunToString);
            var metrics = JsonConvert.DeserializeObject<Metrics>(jsonString);
            return metrics;
        }
        #nullable disable

        public static AndroidJavaObject ToKeywords(this Dictionary<string, string> source)
        {
            var keywords = new AndroidJavaObject(AndroidConstants.ClassKeywords);
            if (source == null || source.Count == 0)
                return keywords;

            foreach (var kvp in source)
            {
               var isSet = keywords.Call<bool>(AndroidConstants.FunSet, kvp.Key, kvp.Value);
               if (!isSet)
                   EventProcessor.ReportUnexpectedSystemError($"[Keywords] failed to set the following keyword: {kvp.Key}, with value: {kvp.Value}");
            }
            
            return keywords;
        }

        public static int HashCode(this AndroidJavaObject source) => source.Call<int>(AndroidConstants.FunHashCode);
    }
}
#endif
