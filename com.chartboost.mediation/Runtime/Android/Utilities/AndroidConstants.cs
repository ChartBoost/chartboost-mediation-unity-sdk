using UnityEngine;

namespace Chartboost.Mediation.Android.Utilities
{
    /// <summary>
    /// Chartboost Mediation Android's Constants
    /// </summary>
    internal sealed class AndroidConstants
    {
        internal static AndroidJavaObject GetUnityBridge() => new(ClassUnityBridge);
        
        internal static AndroidJavaClass GetNativeSDK() => new(ClassChartboostMediationSdk);
        
        public const string FunctionGetRequest = "getRequest";
        public const string FunctionGetPlacementName = "getPlacementName";
        public const string FunctionGetCustomData = "getCustomData";
        public const string FunctionSetCustomData = "setCustomData";
        public const string FunctionGetPosition = "getPosition";
        public const string FunctionSetPosition = "setPosition";
        public const string FunctionGetPivot = "getPivot";
        public const string FunctionSetPivot = "setPivot";
        public const string FunctionGetAd = "getAd";
        public const string FunctionGetLoadId = "getLoadId";
        public const string FunctionGetMetrics = "getMetrics";
        public const string FunctionGetWinningBidInfo= "getWinningBidInfo";
        public const string FunctionGetChartboostMediationError = "getChartboostMediationError";
        public const string FunctionGetCode = "getCode";
        public const string FunctionReleaseFullscreenAd = "releaseFullscreenAd";
        public const string FunctionReleaseBannerAd = "releaseBannerAd";
        public const string FunctionAdStoreInfo = "adStoreInfo";
        public const string FunctionToString = "toString";
        public const string FunctionShowFullscreenAd = "showFullscreenAd";
        public const string FunctionSubscribeIlrd= "subscribeIlrd";
        public const string FunctionSubscribePartnerAdapterInitializationResults = "subscribePartnerAdapterInitializationResults";
        public const string FunctionGetAdapterInfo = "getAdapterInfo";
        public const string FunctionGetAdapterVersion = "getAdapterVersion";
        public const string FunctionGetPartnerVersion = "getPartnerVersion";
        public const string FunctionGetPartnerId = "getPartnerId";
        public const string FunctionGetPartnerDisplayName = "getPartnerDisplayName";
        public const string FunctionIsDiscardOversizedAdsEnabled = "isDiscardOversizedAdsEnabled";
        public const string FunctionSetShouldDiscardOversizedAds = "setShouldDiscardOversizedAds";
        public const string FunctionGetVersion = "getVersion";
        public const string FunctionSetLogLevel = "setLogLevel";
        public const string FunctionLoadFullscreenAd = "loadFullscreenAd";
        public const string FunctionLoadBannerAd = "loadBannerAd";
        public const string FunctionGetKeywords = "getKeywords";
        public const string FunctionSetKeywords = "setKeywords";
        public const string FunctionGetPartnerSettings = "getPartnerSettings";
        public const string FunctionSetPartnerSettings = "setPartnerSettings";
        public const string FunctionGetBannerSize = "getBannerSize";
        public const string FunctionGetWidth = "getWidth";
        public const string FunctionGetHeight = "getHeight";
        public const string FunctionGetContainerSize = "getContainerSize";
        public const string FunctionGetHorizontalAlignment = "getHorizontalAlignment";
        public const string FunctionSetHorizontalAlignment = "setHorizontalAlignment";
        public const string FunctionGetVerticalAlignment = "getVerticalAlignment";
        public const string FunctionSetVerticalAlignment = "setVerticalAlignment";
        public const string FunctionLoad = "load";
        public const string FunctionResizeToFit = "resizeToFit";
        public const string FunctionGetDraggability = "getDraggability";
        public const string FunctionSetDraggability = "setDraggability";
        public const string FunctionGetVisibility = "getVisibility";
        public const string FunctionSetVisibility = "setVisibility";
        public const string FunctionReset = "reset";
        public const string FunctionDestroy = "destroy";
        public const string FunctionSetContainerPosition = "setContainerPosition";
        public const string FunctionSetContainerSize = "setContainerSize";
        public const string FunctionGetUIScaleFactor = "getUIScaleFactor";
        public const string FunctionGetFullscreenAdQueue = "getFullscreenAdQueue";
        public const string FunctionHasNextAd = "hasNextAd";
        public const string FunctionGetNextAd = "getNextAd";
        public const string FunctionStop = "stop";
        public const string FunctionSetListener = "setListener";

        public const string PropertyCoreModuleId = "CORE_MODULE_ID";
        public const string PropertyPartnerId = "partner_id";
        public const string PropertyAuctionId = "auction_id";
        public const string PropertyLineItemId = "line_item_id";
        public const string PropertyLineItemName = "line_item_name";
        public const string PropertyPrice = "price";
        public const string PropertyError = "getError";
        public const string PropertyPlacementId = "placementId";
        public const string PropertyIlrdInfo = "ilrdInfo";
        public const string PropertyData = "data";
        public const string PropertyChartboostMediationError = "getChartboostMediationError";
        public const string PropertyName = "name";
        public const string PropertyWidth = "width";
        public const string PropertyHeight = "height";
        public const string PropertyQueueCapacity = "queueCapacity";
        public const string PropertyNumberOfAdsReady = "getNumberOfAdsReady";
        public const string PropertyIsRunning = "isRunning";
        
        public const string BannerSizeStandard = "STANDARD";
        public const string BannerSizeMedium = "MEDIUM";
        public const string BannerSizeLeaderboard = "LEADERBOARD";
        public const string BannerSizeAdaptive = "ADAPTIVE";
        
        public static readonly string ClassChartboostMediationSdk = GetNativeSDKClass("ChartboostMediationSdk");
   
        public static readonly string ClassKeywords =  GetNativeSDKClass("domain.Keywords");
        public static readonly string ClassAdLoadRequest = GetNativeSDKAdClass("ChartboostMediationAdLoadRequest");
        public static readonly string ClassFullscreenAdLoadRequest = GetNativeSDKAdClass("ChartboostMediationFullscreenAdLoadRequest");
        public static readonly string ClassFullscreenAdLoadListener = GetNativeSDKAdClass("ChartboostMediationFullscreenAdLoadListener");
        public static readonly string ClassFullscreenAdShowListener = GetNativeSDKAdClass("ChartboostMediationFullscreenAdShowListener");
        public static readonly string ClassFullscreenAdListener = GetNativeSDKAdClass("ChartboostMediationFullscreenAdListener");
        public static readonly string ClassBannerAdListener = GetBannerWrappingType("ChartboostMediationBannerAdListener");
        public static readonly string ClassBannerAdLoadListener = GetNativeSDKAdClass("ChartboostMediationBannerAdLoadListener");
        public static readonly string ClassFullscreenAdQueueListener = GetNativeSDKAdClass("ChartboostMediationFullscreenAdQueueListener");
        public static readonly string ClassChartboostMediationPreInitializationConfiguration = GetNativeSDKClass("ChartboostMediationPreinitializationConfiguration");
        public static readonly string ClassPartnerAdapterInitializationResultsObserver = GetNativeSDKClass("PartnerAdapterInitializationResultsObserver");
        
        public static readonly string ClassUnityBridge = GetBridgeType("BridgeCBM");
        public static readonly string ClassAdStore = GetUtilsType("AdStore");

        private const string NamespaceMediationUnity = "com.chartboost.mediation.unity";
        
        public static readonly string UnityILRDObserver = GetILRDType("UnityILRDObserver");
        public static readonly string UnityILRDConsumer = GetILRDType("UnityILRDConsumer");

        public const string FunctionSetUnityILRDProxy = "setUnityILRDProxy";
        public const string FunctionRetrieveImpressionData = "retrieveImpressionData";
        
        private static string GetILRDType(string className) => $"{NamespaceMediationUnity}.ilrd.{className}";
        private static string GetBridgeType(string className) => $"{NamespaceMediationUnity}.bridge.{className}";

        private static string GetUtilsType(string className) => $"{NamespaceMediationUnity}.utils.{className}";
        private static string GetBannerWrappingType(string className) => $"{NamespaceMediationUnity}.banner.{className}";

        private static string GetNativeSDKAdClass(string className) => GetNativeSDKClass($"ad.{className}");

        private static string GetNativeSDKClass(string className)
        {
            return $"com.chartboost.chartboostmediationsdk.{className}";
        }
    }
}
