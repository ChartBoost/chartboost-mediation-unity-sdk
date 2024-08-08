using System;
using Chartboost.Json;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using System.Collections.Generic;
using Chartboost.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Utilities
{
    /// <summary>
    /// <see cref="JsonTools"/> extensions for Chartboost Mediation SDK operations with JSON objects.
    /// </summary>
    public static class JsonExtensions 
    {
        private const string JsonExtensionsTag = "[JsonExtensions]";
        
        public static ChartboostMediationError? ToChartboostMediationError(this string mediationErrorJson)
            => JsonTools.DeserializeNullableObject<ChartboostMediationError>(mediationErrorJson);
        
        public static BidInfo ToBidInfo(this string bidInfoJson)
            => JsonTools.DeserializeObject<BidInfo>(bidInfoJson);

        public static Metrics? ToMetrics(this string metricsJson)
            => JsonTools.DeserializeNullableObject<Metrics>(metricsJson);
        
        public static FullscreenAdLoadRequest ToFullscreenAdLoadRequest(this string fullscreenAdLoadRequestJson) 
            => JsonTools.DeserializeObject<FullscreenAdLoadRequest>(fullscreenAdLoadRequestJson);

        public static AdapterInfo[] ToAdaptersInfo(this string adaptersInfoJson) 
            => JsonTools.DeserializeObject<AdapterInfo[]>(adaptersInfoJson);
        
        public static BannerAdLoadRequest ToBannerAdLoadRequest(this string fullscreenAdLoadRequestJson) 
            => JsonTools.DeserializeObject<BannerAdLoadRequest>(fullscreenAdLoadRequestJson);

        public static Dictionary<string, string> ToDictionary(this string collectionJson) 
            => JsonTools.DeserializeObject<Dictionary<string, string>>(collectionJson);

        public static List<string> ToList(this string collectionJson)
            => JsonTools.DeserializeObject<List<string>>(collectionJson);

        public static BannerSize? ToBannerSize(this string bannerSizeJson)
            => JsonTools.DeserializeNullableObject<BannerSize>(bannerSizeJson);
        
        public static ContainerSize ToContainerSize(this string containerSizeJson)
            => JsonTools.DeserializeObject<ContainerSize>(containerSizeJson);
        
        public static Vector2 ToVector2(this string vector2Json)
        {
            if (string.IsNullOrWhiteSpace(vector2Json))
            {
                LogController.Log($"{JsonExtensionsTag}/The input JSON string cannot be null or empty.", LogLevel.Warning);
                return Vector2.zero;
            }
            try
            {
                var jObj = JObject.Parse(vector2Json);
                var x = jObj["x"]?.ToObject<float>() ?? 0;
                var y = jObj["y"]?.ToObject<float>() ?? 0;

                return new Vector2(x, y);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("The input string is not a valid JSON.", nameof(vector2Json), ex);
            }   
        }
    }
}
