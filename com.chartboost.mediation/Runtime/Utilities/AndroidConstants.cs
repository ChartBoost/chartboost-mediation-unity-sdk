namespace Chartboost.Utilities
{
    internal static class AndroidConstants
    {
        internal const string GameEngine = "unity";

        internal const string NamespaceChartboostMediationBridge = "com.chartboost.mediation";
        internal const string NamespaceNativeChartboostMediationSDK = "com.chartboost.heliumsdk";
        
        internal const string ClassUnityBridge = "UnityBridge";
        internal const string ClassHeliumSdk = "HeliumSdk";
        internal const string ClassUnityPlayer = "com.unity3d.player.UnityPlayer";
        internal const string ClassChartboostMediationAdLoadRequest = "ChartboostMediationAdLoadRequest";
        internal const string ClassHeliumSDKListener = "HeliumSdk$HeliumSdkListener";
        internal const string ClassHeliumIlrdObserver = "HeliumIlrdObserver";
        internal const string ClassPartnerInitializationResultsObserver = "PartnerInitializationResultsObserver";
        internal const string ClassChartboostMediationFullscreenAdLoadListener = "ChartboostMediationFullscreenAdLoadListener";
        internal const string ClassChartboostMediationFullscreenAdShowListener = "ChartboostMediationFullscreenAdShowListener";
        internal const string ClassChartboostMediationFullscreenAdListener = "ChartboostMediationFullscreenAdListener";
        internal const string ClassChartboostMediationBannerViewListener = "ChartboostMediationBannerViewListener";
        internal const string ClassAdStore = "com.chartboost.mediation.unity.AdStore";
        internal const string ClassKeywords = "com.chartboost.heliumsdk.domain.Keywords";
        internal const string ClassHeliumInitializationOptions = "com.chartboost.heliumsdk.HeliumInitializationOptions";

        internal const string ClassHashSet = "java.util.HashSet";
        internal const string ClassHashMap = "java.util.HashMap";
        internal const string ClassString = "java.lang.String";
        internal const string ClassBoolean = "java.lang.Boolean";

        internal const string FunSetKeywords = "setKeywords";
        internal const string FunGetAdSize = "getAdSize";
        internal const string FunGetContainerSize = "getContainerSize";
        internal const string FunGetHorizontalAlignment = "getHorizontalAlignment";
        internal const string FunSetHorizontalAlignment = "setHorizontalAlignment";
        internal const string FunGetVerticalAlignment = "getVerticalAlignment";
        internal const string FunSetVerticalAlignment = "setVerticalAlignment";
        internal const string FunLoad = "load";
        internal const string FunResizeToFit = "resizeToFit";
        internal const string FunSetDraggability = "setDraggability";
        internal const string FunSetVisibility = "setVisibility";
        internal const string FunReset = "reset";
        internal const string FunDestroy = "destroy";
        internal const string FunMoveTo = "moveTo";
        internal const string FunSetGameEngine = "setGameEngine";
        internal const string FunSubscribeIlrd = "subscribeIlrd";
        internal const string FunSubscribeInitializationResults = "subscribeInitializationResults";
        internal const string FunShowFullscreenAd = "showFullscreenAd";
        internal const string FunSetupEventListeners = "setupEventListeners";
        internal const string FunStart = "start";
        internal const string FunSetSubjectToCoppa = "setSubjectToCoppa";
        internal const string FunSetSubjectToGDPR = "setSubjectToGDPR";
        internal const string FunSetUserHasGivenConsent = "setUserHasGivenConsent";
        internal const string FunSetCCPAConsent = "setCCPAConsent";
        internal const string FunSetUserIdentifier = "setUserIdentifier";
        internal const string FunGetUserIdentifier = "getUserIdentifier";
        internal const string FunSetTestMode = "setTestMode";
        internal const string FunSetShouldDiscardOversizedAds = "setShouldDiscardOversizedAds";
        internal const string FunUnsubscribeILRDObserver = "unsubscribeILRDObserver";
        internal const string FunLoadFullscreenAd = "loadFullscreenAd";
        internal const string FunLoadBannerAd = "loadBannerAd";
        internal const string FunGetUIScaleFactor = "getUIScaleFactor";
        internal const string FunReleaseLegacyAd = "releaseLegacyAd";
        internal const string FunReleaseFullscreenAd = "releaseFullscreenAd";
        internal const string FunReleaseBannerAd = "releaseBannerAd";
        internal const string FunAdStoreInfo = "storeInfo";
        internal const string FunToString = "toString";
        internal const string FunHashCode = "hashCode";
        internal const string FunGet = "get";
        internal const string FunSet = "set";
        internal const string FunPut = "put";
        internal const string FunAdd = "add";
        internal const string FunSize = "size";
        internal const string FunAdapterInfo = "getAdapterInfo";
        internal const string FunIterator = "iterator";
        internal const string FunNext = "next";
        internal const string FunHasNext = "hasNext";
        internal const string FunGetAdapterVersion = "getAdapterVersion";
        internal const string FunGetPartnerVersion = "getPartnerVersion";
        internal const string FunGetPartnerId = "getPartnerId";
        internal const string FunGetPartnerDisplayName = "getPartnerDisplayName";
        internal const string FunGetPartnerConsents = "getPartnerConsents";
        internal const string FunGetPartnerIdToConsentGivenMapCopy = "getPartnerIdToConsentGivenMapCopy";
        internal const string FunSetPartnerConsent = "setPartnerConsent";
        internal const string FunSetPartnerConsents = "setPartnerConsents";
        internal const string FunRemovePartnerConsent = "removePartnerConsent";
        internal const string FunBooleanValue = "booleanValue";
        internal const string FunEntrySet = "entrySet";
        internal const string FunGetKey = "getKey";
        internal const string FunGetValue = "getValue";
        
        internal const string PropertyChartboostMediationError = "chartboostMediationError";
        internal const string PropertyAd = "ad";
        internal const string PropertyMetrics = "metrics";
        internal const string PropertyWinningBidInfo = "winningBidInfo";
        internal const string PropertyLoadId = "loadId";
        internal const string PropertyCustomData = "customData";
        internal const string PropertyCurrentActivity = "currentActivity";
        internal const string PropertyCode = "code";
        internal const string PropertyError = "error";
        internal const string PropertyPartnerId = "partner_id";
        internal const string PropertyAuctionId = "auction-id";
        internal const string PropertyLineItemId = "line_item_id";
        internal const string PropertyLineItemName = "line_item_name";
        internal const string PropertyPrice = "price";
        internal const string PropertyName = "name";
        internal const string PropertyWidth = "width";
        internal const string PropertyHeight = "height";
        internal const string PropertyPlacementId = "placementId";
        internal const string PropertyIlrdInfo = "ilrdInfo";
        internal const string PropertyData = "data";

        internal const string BannerSizeStandard = "STANDARD";
        internal const string BannerSizeMedium = "MEDIUM";
        internal const string BannerSizeLeaderboard = "LEADERBOARD";
        internal const string BannerSizeAdaptive = "ADAPTIVE";
    }
}
