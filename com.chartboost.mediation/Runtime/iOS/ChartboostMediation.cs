using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using BannerAd = Chartboost.Mediation.iOS.Ad.Banner.BannerAd;
using FullscreenAd = Chartboost.Mediation.iOS.Ad.Fullscreen.FullscreenAd;
using FullscreenAdQueue = Chartboost.Mediation.iOS.Ad.Fullscreen.Queue.FullscreenAdQueue;

namespace Chartboost.Mediation.iOS
{
    /// <summary>
    /// iOS's implementation of <see cref="ChartboostMediation"/>.
    /// </summary>
    internal sealed partial class ChartboostMediation : ChartboostMediationBase
    {
        /// <summary>
        /// Registers the class instance on start-up.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterInstance()
        {
            if (Application.isEditor)
                return;
            
            Chartboost.Mediation.ChartboostMediation.Instance = new ChartboostMediation();
            _CBMSetLifeCycleCallbacks(ExternDidReceivePartnerInitializationData, ExternDidReceiveImpressionLevelRevenueData);
            DensityConverters.ScaleFactor = _CBMGetUIScaleFactor();
        }

        public override string CoreModuleId => _CBMCoreModuleId();

        /// <inheritdoc cref="NativeSDKVersion"/>
        public override string NativeSDKVersion => _CBMGetVersion();

        /// <inheritdoc cref="ChartboostMediation.TestMode"/>
        public override bool TestMode
        {
            get => _CBMGetTestMode();
            set => _CBMSetTestMode(value);
        }

        public override LogLevel LogLevel
        {
            get => (LogLevel)_CBMGetLogLevel();
            set {
                base.LogLevel = value;
                _CBMSetLogLevel((int)value);
            }
        }

        /// <inheritdoc cref="ChartboostMediation.DiscardOverSizedAds"/>
        public override bool DiscardOverSizedAds
        {
            get => _CBMGetDiscardOverSizedAds();
            set => _CBMSetDiscardOverSizedAds(value);
        }

        /// <inheritdoc cref="ChartboostMediation.AdaptersInfo"/>
        public override AdapterInfo[] AdaptersInfo => _CBMGetAdaptersInfo().ToAdaptersInfo();

        /// <inheritdoc cref="ChartboostMediation.SetPreInitializationConfiguration"/>
        public override ChartboostMediationError? SetPreInitializationConfiguration(ChartboostMediationPreInitializationConfiguration configuration)
        {
            base.SetPreInitializationConfiguration(configuration);
            var error = _CMBSetPreInitializationConfiguration(configuration.SkippablePartnerIds.ToArray(), configuration.SkippablePartnerIds.Count);
            return error.ToChartboostMediationError();
        }

        /// <inheritdoc cref="ChartboostMediation.LoadFullscreenAd"/>
        public override async Task<FullscreenAdLoadResult> LoadFullscreenAd(FullscreenAdLoadRequest request)
        {
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError(Errors.ErrorNotReady);
                var adLoadResult = new FullscreenAdLoadResult(error);
                return await Task.FromResult(adLoadResult);
            }

            var (proxy, hashCode) = AwaitableProxies.SetupProxy<FullscreenAdLoadResult>();
            AdCache.TrackAdLoadRequest(hashCode, request);
            var keywordsJson = string.Empty;
            if (request.Keywords.Count > 0)
                keywordsJson = JsonConvert.SerializeObject(request.Keywords);

            FullscreenAd._CBMLoadFullscreenAd(request.PlacementName, keywordsJson, hashCode, FullscreenAd.FullscreenAdLoadResultCallbackProxy);
            return await proxy;
        }

        /// <inheritdoc cref="ChartboostMediation.GetFullscreenAdQueue"/>
        public override IFullscreenAdQueue GetFullscreenAdQueue(string placementName)
        {
            // Queues are a "singleton per placement", meaning that if a publisher attempts to
            // create multiple queues with the same placement ID the same object will be returned each time.
            var nativeQueue =  FullscreenAdQueue._CBMFullscreenAdQueueGetQueue(placementName);
            var queue = (FullscreenAdQueue)AdCache.GetAd(nativeQueue.ToInt64());
            if (queue != null)
                return queue;
            
            queue = new FullscreenAdQueue(nativeQueue);
            return queue;
        }
        
        /// <inheritdoc cref="ChartboostMediation.GetBannerAd"/>
        public override IBannerAd GetBannerAd()
            => new BannerAd();

        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMCoreModuleId();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMGetVersion();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern bool _CBMGetTestMode();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMSetTestMode(bool enabled);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMGetLogLevel();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMSetLogLevel(int logLevel);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern bool _CBMGetDiscardOverSizedAds();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMSetDiscardOverSizedAds(bool shouldDiscard);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMGetAdaptersInfo();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CMBSetPreInitializationConfiguration(string[] skippedPartnerIds, int skippedPartnerIdsSize);
    }
}
