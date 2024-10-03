using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Android.Ad.Banner;
using Chartboost.Mediation.Android.Ad.Fullscreen;
using Chartboost.Mediation.Android.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Android.ILRD;
using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;
using UnityEngine.Scripting;
using DensityConverters = Chartboost.Mediation.Utilities.DensityConverters;

namespace Chartboost.Mediation.Android
{
    /// <summary>
    /// Android's implementation of <see cref="ChartboostMediation"/>.
    /// </summary>
    internal partial class ChartboostMediation : ChartboostMediationBase
    {
        [Preserve]
        // ReSharper disable once InconsistentNaming
        internal static readonly UnityILRDConsumer UnityILRDConsumerInstance = new();
        
        /// <summary>
        /// Registers the class instance on start-up.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterInstance()
        {
            if (Application.isEditor)
                return;
            
            Chartboost.Mediation.ChartboostMediation.Instance = new ChartboostMediation();
            
            using var unityBridge = AndroidConstants.GetUnityBridge();
            DensityConverters.ScaleFactor = unityBridge.CallStatic<float>(AndroidConstants.FunctionGetUIScaleFactor);
            
            using var nativeSDK =  AndroidConstants.GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunctionSubscribeIlrd, new AndroidJavaObject(AndroidConstants.UnityILRDObserver));
            nativeSDK.CallStatic(AndroidConstants.FunctionSubscribePartnerAdapterInitializationResults, new PartnerAdapterInitializationResultsObserver());
        }

        /// <inheritdoc/>
        public override string CoreModuleId {
            get
            {
                using var native = AndroidConstants.GetNativeSDK();
                return native.GetStatic<string>(AndroidConstants.PropertyCoreModuleId);
            }
        }

        /// <inheritdoc/>
        public override string NativeSDKVersion
        {
            get
            {
                using var native = AndroidConstants.GetNativeSDK();
                return native.CallStatic<string>(AndroidConstants.FunctionGetVersion);
            }
        }

        /// <inheritdoc/>
        public override bool TestMode 
        {
            get
            {
                using var native = AndroidConstants.GetNativeSDK();
                return native.CallStatic<bool>(SharedAndroidConstants.FunctionGetTestMode);
            }
            set
            {
                using var native = AndroidConstants.GetNativeSDK();
                using var currentActivity = SharedAndroidConstants.UnityPlayerCurrentActivity();
                native.CallStatic(SharedAndroidConstants.FunctionSetTestMode, currentActivity, value);
            }
        }
        
        /// <inheritdoc/>
        public override LogLevel LogLevel 
        {
            get
            {
                using var bridge = AndroidConstants.GetUnityBridge();
                return (LogLevel)bridge.CallStatic<int>(SharedAndroidConstants.FunctionGetLogLevel);;
            }
            set
            {
                base.LogLevel = value;
                using var bridge = AndroidConstants.GetUnityBridge();
                bridge.CallStatic(AndroidConstants.FunctionSetLogLevel, (int)value);
            }
        }
        
        /// <inheritdoc/>
        public override bool DiscardOverSizedAds 
        {
            get
            {
                using var native = AndroidConstants.GetNativeSDK();
                return native.CallStatic<bool>(AndroidConstants.FunctionIsDiscardOversizedAdsEnabled);
            }
            set
            {
                using var native = AndroidConstants.GetNativeSDK();
                native.CallStatic(AndroidConstants.FunctionSetShouldDiscardOversizedAds, value);
            }
        }
        
        /// <inheritdoc/>
        public override AdapterInfo[] AdaptersInfo
        {
            get
            {
                using var native = AndroidConstants.GetNativeSDK();
                using var adaptersInfo = native.CallStatic<AndroidJavaObject>(AndroidConstants.FunctionGetAdapterInfo);
                return adaptersInfo.ToAdapterInfo();
            }
        }
        
        /// <inheritdoc/>
        public override ChartboostMediationError? SetPreInitializationConfiguration(ChartboostMediationPreInitializationConfiguration configuration)
        {
            base.SetPreInitializationConfiguration(configuration);
            using var nativePreInitializationOptions = configuration.SkippablePartnerIds.ToArray().ToInitializationOptions();
            using var nativeSDK = AndroidConstants.GetNativeSDK();
            using var error = nativeSDK.CallStatic<AndroidJavaObject>(SharedAndroidConstants.FunctionSetPreInitializationConfiguration, nativePreInitializationOptions);
            return error?.ToChartboostMediationError(AndroidConstants.PropertyChartboostMediationError);
        }
        
        /// <inheritdoc/>
        public override async Task<FullscreenAdLoadResult> LoadFullscreenAd(FullscreenAdLoadRequest request)
        {
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError(Errors.ErrorNotReady);
                var adLoadResult = new FullscreenAdLoadResult(error);
                return await Task.FromResult(adLoadResult);
            }

            var adLoadListenerAwaitableProxy = new FullscreenAd.FullscreenAdLoadListener();
            try
            {
                AdCache.TrackAdLoadRequest(adLoadListenerAwaitableProxy.hashCode(), request);
                using var nativeAdRequest = new AndroidJavaObject(AndroidConstants.ClassFullscreenAdLoadRequest, request.PlacementName, request.Keywords.ToKeywords(), new Dictionary<string, string>().ToKeyValuePair());
                using var bridge = AndroidConstants.GetUnityBridge();
                bridge.CallStatic(AndroidConstants.FunctionLoadFullscreenAd, nativeAdRequest, adLoadListenerAwaitableProxy, FullscreenAd.FullscreenAdListenerInstance);
            }
            catch (NullReferenceException exception)
            {
                LogController.LogException(exception);
            }
            return await adLoadListenerAwaitableProxy;
        }

        /// <inheritdoc/>
        public override IFullscreenAdQueue GetFullscreenAdQueue(string placementName)
        {
            // Queues are a "singleton per placement", meaning that if a publisher attempts to
            // create multiple queues with the same placement ID the same object will be returned each time.
            using var unityBridge = AndroidConstants.GetUnityBridge();
            var nativeQueue = unityBridge.CallStatic<AndroidJavaObject>(AndroidConstants.FunctionGetFullscreenAdQueue, placementName, FullscreenAdQueue.FullscreenAdQueueListenerInstance);
            var queue = (FullscreenAdQueue)AdCache.GetAd(nativeQueue.NativeHashCode());
            
            if (queue != null)
                return queue;
            
            queue = new FullscreenAdQueue(nativeQueue);
            return queue;
        }
        
        /// <inheritdoc/>
        public override IBannerAd GetBannerAd()
        {
            using var unityBridge = AndroidConstants.GetUnityBridge();
            var bannerAd = unityBridge.CallStatic<AndroidJavaObject>(AndroidConstants.FunctionLoadBannerAd, BannerAd.BannerAdListenerInstance);
            return new BannerAd(bannerAd);
        }
    }
}
