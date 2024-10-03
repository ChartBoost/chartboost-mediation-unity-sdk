using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Core;
using Chartboost.Core.Initialization;
using Chartboost.Json;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Default;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;
using UnityEngine;

namespace Chartboost.Mediation
{
    /// <summary>
    /// The main interface to the Chartboost Mediation SDK.
    /// </summary>
    public sealed class ChartboostMediation
    {
        internal static ChartboostMediationBase Instance = new ChartboostMediationDefault();
        
        /// The module ID of the <see cref="ChartboostCore"/> module that represents <see cref="ChartboostMediation"/>.
        /// <para>
        /// <see cref="ChartboostCore.Initialize"/> initializes <see cref="ChartboostMediation"/> by default. In order to skip
        /// <see cref="ChartboostMediation"/> initialization, provide this module ID to <see cref="SDKConfiguration.SkippedModuleIdentifiers"/>
        /// when calling <see cref="ChartboostCore.Initialize"/>.
        /// </para>
        public static string CoreModuleId 
            => Instance.CoreModuleId;
        
        /// <summary>
        /// The Chartboost Mediation SDK version. The value is a semantic versioning compliant string.
        /// </summary>
        public const string SDKVersion = "5.1.0";

        /// <summary>
        /// The native Chartboost Mediation SDK version.
        /// The value is a semantic versioning compliant string.
        /// </summary>
        public static string NativeSDKVersion 
            => Instance.NativeSDKVersion;

        /// <summary>
        /// A <see cref="bool"/> flag for setting test mode.
        /// <para/>
        /// warning: Do not enable test mode in production builds.
        /// </summary>
        public static bool TestMode
        {
            get => Instance.TestMode;
            set => Instance.TestMode = value;
        }
        
        /// <summary>
        /// Sets the log level. Anything of that log level and lower will be emitted. Set this to <see cref="LogLevel.Disabled"/> for no logs.
        /// </summary>
        public static LogLevel LogLevel
        {
            get => Instance.LogLevel;
            set => Instance.LogLevel = value;
        }

        /// <summary>
        /// <see cref="bool"/> value indicating that ads returned from adapters that are larger than the requested size should be discarded.
        /// An ad is defined as too large if either the width or the height of the resulting ad is larger than the requested ad size
        /// (unless the height of the requested ad size is 0, as is the case when using
        /// <see cref="BannerSizeType.Adaptive"/>, in this case,an error will be returned.
        /// This currently only applies to <see cref="IBannerAd"/>. Defaults to <b>false</b>.
        /// </summary>
        public static bool DiscardOverSizedAds
        {
            get => Instance.DiscardOverSizedAds;
            set => Instance.DiscardOverSizedAds = value;
        }

        /// <summary>
        /// An array of all initialized adapters, or an empty array if the SDK is not initialized.
        /// </summary>
        public static AdapterInfo[] AdaptersInfo => Instance.AdaptersInfo;

        /// <summary>
        /// Sets the Chartboost Mediation PreInitialization configuration. Setting this after initialization does nothing and returns an exception.
        /// </summary>
        public static ChartboostMediationError? SetPreInitializationConfiguration(ChartboostMediationPreInitializationConfiguration configuration) 
            => Instance.SetPreInitializationConfiguration(configuration);

        /// <summary>
        /// Loads a <see cref="IFullscreenAd"/> using the information provided in the request.
        /// Chartboost Mediation may return the same ad from a previous successful load if it was never shown nor invalidated before it got discarded.
        /// <param name="request">A request containing the information used to load the ad.</param>
        /// \param request .
        ///
        /// \param completion A closure executed when the load operation is done.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/> with <see cref="FullscreenAdLoadResult"/>.</returns>>
        public static async Task<FullscreenAdLoadResult> LoadFullscreenAd(FullscreenAdLoadRequest request) 
            => await Instance.LoadFullscreenAd(request);

        /// <summary>
        /// Returns a <see cref="IFullscreenAdQueue"/>. Queue will not begin loading ads until `Start()` is called.
        /// Calling <see cref="ChartboostMediationBase.GetFullscreenAdQueue"/> more than once with the same placement ID returns the same object each time.
        /// </summary>
        /// <param name="placementName">Identifier for the Chartboost placement this queue should load ads from.</param>
        /// <returns><see cref="IFullscreenAd"/> instance.</returns>
        public static IFullscreenAdQueue GetFullscreenAdQueue(string placementName) 
            => Instance.GetFullscreenAdQueue(placementName);

        /// <summary>
        /// Returns a new <see cref="IBannerAd"/> unit that can be used to load and display <see cref="IBannerAd"/> ads.
        /// </summary>
        public static IBannerAd GetBannerAd() 
            => Instance.GetBannerAd();
        
        /// <summary>
        /// Returns a new <see cref="UnityBannerAd"/> <see cref="GameObject"/> that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement name for this <see cref="IBannerAd"/> ad.</param>
        /// <param name="parent">The parent <see cref="Transform"/> under which this <see cref="GameObject"/> will be created.</param>
        /// <param name="size">size of the <see cref="GameObject"/>.</param>
        
        public static UnityBannerAd GetUnityBannerAd(string placementName, Transform parent = null, BannerSize? size = null)
        {
            var unityBannerAd = UnityBannerAd.InstantiateUnityBannerAd(parent, size);
            unityBannerAd.PlacementName = placementName;
            return unityBannerAd; 
        }
        
        /// <summary>
        /// Event for receiving ILRD(Impression Level Revenue Data) events.
        /// </summary>
        public static event ChartboostMediationImpressionLevelRevenueDataEvent DidReceiveImpressionLevelRevenueData;
        
        /// <summary>
        /// Event for receiving partner initialization result events.
        /// </summary>
        public static event ChartboostMediationPartnerAdapterInitializationEvent DidReceivePartnerAdapterInitializationData;

        internal static void OnDidReceiveImpressionLevelRevenueData(string impressionDataJson)
        {
            MainThreadDispatcher.Post(_ =>
            {
                if (string.IsNullOrEmpty(impressionDataJson))
                {
                    LogController.Log("ILRD is null or empty, this is not correct and likely a broken ILRD listener.", LogLevel.Error);
                    return;
                }

                if (impressionDataJson.DeserializeObject() is not Dictionary<object, object> data)
                {
                    LogController.Log($"ILRD: {impressionDataJson} does not match the Dictionary<object, object> format.", LogLevel.Error);
                    return;
                }

                data.TryGetValue("placement", out var placementName);
                DidReceiveImpressionLevelRevenueData?.Invoke(placementName as string, new Hashtable(data));
            });
        }

        internal static void OnDidReceivePartnerAdapterInitializationData(string partnerInitializationData) 
            => MainThreadDispatcher.Post(_ => DidReceivePartnerAdapterInitializationData?.Invoke(partnerInitializationData));
    }
}
