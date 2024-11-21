using System;
using System.Collections.Generic;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Mediation.Android.Utilities
{
    /// <summary>
    /// <see cref="AndroidJavaObject"/> extensions for Unity C# compatible objects.
    /// </summary>
    public static class AndroidExtensions
    {
#nullable enable
        public static ChartboostMediationError? ToChartboostMediationError(this AndroidJavaObject objectWithErrorField, string field = AndroidConstants.PropertyError)
        {
            var error = objectWithErrorField.Call<AndroidJavaObject>(field);
            if (error == null)
                return null;

            var code = error.Call<string>(AndroidConstants.FunctionGetCode);
            var message = error.Call<string>(AndroidConstants.FunctionToString);
            return new ChartboostMediationError(code, message);
        }

        public static Metrics? JsonObjectToMetrics(this AndroidJavaObject source)
        {
            var jsonString = source.Call<string>(AndroidConstants.FunctionToString);
            var metrics = JsonConvert.DeserializeObject<Metrics>(jsonString);
            return metrics;
        }
#nullable disable

        public static BidInfo MapToWinningBidInfo(this AndroidJavaObject map)
        {
            var partnerId = map.Call<string>(SharedAndroidConstants.FunctionGet, AndroidConstants.PropertyPartnerId);
            var auctionId = map.Call<string>(SharedAndroidConstants.FunctionGet, AndroidConstants.PropertyAuctionId);
            var lineItemId = map.Call<string>(SharedAndroidConstants.FunctionGet, AndroidConstants.PropertyLineItemId);
            var lineItemName = map.Call<string>(SharedAndroidConstants.FunctionGet, AndroidConstants.PropertyLineItemName);
            var price = map.Call<string>(SharedAndroidConstants.FunctionGet, AndroidConstants.PropertyPrice);

            if (!double.TryParse(price, out var priceAsDouble))
                LogController.Log("Failed to parse bid info price, defaulting to 0.", LogLevel.Error);
            
            return new BidInfo(auctionId, partnerId, priceAsDouble, lineItemName, lineItemId);
        }

        public static AndroidJavaObject ToInitializationOptions(this IEnumerable<string> options)
        {
            using var hashSet = new AndroidJavaObject(SharedAndroidConstants.ClassHashSet);
            foreach (var option in options)
            {
                if (string.IsNullOrEmpty(option))
                    continue;
                hashSet.Call<bool>(SharedAndroidConstants.FunctionAdd, option);
            }
            return new AndroidJavaObject(AndroidConstants.ClassChartboostMediationPreInitializationConfiguration, hashSet);
        }

        public static string ImpressionDataToJsonString(this AndroidJavaObject impressionData)
        {
            var placementName = impressionData.Get<string>(AndroidConstants.PropertyPlacementId);
            var ilrdJson = impressionData.Get<AndroidJavaObject>(AndroidConstants.PropertyIlrdInfo).Call<string>(SharedAndroidConstants.FunctionToString);
            return $"{{\"placementName\" : \"{placementName}\", \"ilrd\" : {ilrdJson}}}";
        }

        public static AdapterInfo[] ToAdapterInfo(this AndroidJavaObject nativeAdapterInfo)
        {
            using var iterator = nativeAdapterInfo.Call<AndroidJavaObject>(SharedAndroidConstants.FunctionIterator);
            var count = nativeAdapterInfo.Call<int>(SharedAndroidConstants.FunctionSize);

            if (count == 0)
                return Array.Empty<AdapterInfo>();

            var ret = new AdapterInfo[count];

            var index = 0;

            do
            {
                using var entry = iterator.Call<AndroidJavaObject>(SharedAndroidConstants.FunctionNext);
                var adapterVersion = entry.Call<string>(AndroidConstants.FunctionGetAdapterVersion);
                var partnerVersion = entry.Call<string>(AndroidConstants.FunctionGetPartnerVersion);
                var partnerId = entry.Call<string>(AndroidConstants.FunctionGetPartnerId);
                var partnerDisplayName = entry.Call<string>(AndroidConstants.FunctionGetPartnerDisplayName);
                var adapterInfo = new AdapterInfo(adapterVersion, partnerVersion, partnerId, partnerDisplayName);
                ret[index] = adapterInfo;
                index++;
            } while (iterator.Call<bool>(SharedAndroidConstants.FunctionHasNext));
            return ret;
        }

        public static AndroidJavaObject ToKeywords(this IReadOnlyDictionary<string, string> source)
        {
            var keywords = new AndroidJavaObject(AndroidConstants.ClassKeywords);
            if (source == null || source.Count == 0)
                return keywords;

            foreach (var kvp in source)
            {
                var key = kvp.Key;
                var value = kvp.Value;
                var isSet = keywords.Call<bool>(SharedAndroidConstants.FunctionSet, key, value);
                if (!isSet)
                    LogController.Log($"Failed to set keyword for Key: {key} and Value: {value}", LogLevel.Warning);
            }

            return keywords;
        }


        public static AndroidJavaObject ToKeyValuePair(this IReadOnlyDictionary<string, string> source)
        {
            var map = new AndroidJavaObject("java.util.HashMap");
            foreach (var kvp in source)
                map.Call<AndroidJavaObject>("put", kvp.Key, kvp.Value);

            return map;
        }

        public static BannerSize ToBannerSize(this AndroidJavaObject source)
        {
            if (source == null)
                return new BannerSize();

            var name = source.Get<string>(AndroidConstants.PropertyName);
            var width = source.Get<int>(AndroidConstants.PropertyWidth);
            var height = source.Get<int>(AndroidConstants.PropertyHeight);
            var size = name switch
            {
                AndroidConstants.BannerSizeStandard => BannerSize.Standard,
                AndroidConstants.BannerSizeMedium => BannerSize.MediumRect,
                AndroidConstants.BannerSizeLeaderboard => BannerSize.Leaderboard,
                AndroidConstants.BannerSizeAdaptive => BannerSize.Adaptive(width, height),
                _ => throw new ArgumentOutOfRangeException()
            };

            // if we get adaptive size of size 0X0 then it is undefined/unknown
            if (size is { SizeType: BannerSizeType.Adaptive, Width: 0, Height: 0 })
                size.SizeType = BannerSizeType.Unknown;

            return size;
        }

        public static ContainerSize ToContainerSize(this AndroidJavaObject source)
        {
            if (source == null)
                return new ContainerSize();

            var width = source.Call<int>(AndroidConstants.FunctionGetWidth);
            var height = source.Call<int>(AndroidConstants.FunctionGetHeight);

            return ContainerSize.FixedSize(width, height);
        }
    }
}
