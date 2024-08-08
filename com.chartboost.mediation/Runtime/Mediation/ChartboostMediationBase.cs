using System.Threading.Tasks;
using Chartboost.Core;
using Chartboost.Core.Initialization;
using Chartboost.Json;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;

namespace Chartboost.Mediation
{
    internal abstract class ChartboostMediationBase 
    {
        static ChartboostMediationBase()
        {
            ChartboostCore.ModuleInitializationCompleted += OnMediationInitialized;
        }

        private static void OnMediationInitialized(ModuleInitializationResult result)
        {
            if (result.ModuleId != ChartboostMediation.CoreModuleId)
                return;

            if (result.Error.HasValue)
            {
                LogController.Log($"Chartboost Mediation Failed to Initialize: {JsonTools.SerializeObject(result.Error.Value)}", LogLevel.Error);
                return;
            }
            IsInitialized = true;
            ChartboostCore.ModuleInitializationCompleted -= OnMediationInitialized;
        }

        /// <inheritdoc cref="ChartboostMediation.NativeSDKVersion"/>
        public abstract string CoreModuleId { get; }

        /// <inheritdoc cref="ChartboostMediation.NativeSDKVersion"/>
        public abstract string NativeSDKVersion { get; }

        /// <inheritdoc cref="ChartboostMediation.TestMode"/>
        public abstract bool TestMode { get; set; }
        
        /// <inheritdoc cref="ChartboostMediation.LogLevel"/>
        public virtual LogLevel LogLevel {
            get => LogController.LoggingLevel;
            set => LogController.LoggingLevel = value;
        }
        
        /// <inheritdoc cref="ChartboostMediation.DiscardOverSizedAds"/>
        public abstract bool DiscardOverSizedAds { get; set; }
        
        /// <inheritdoc cref="ChartboostMediation.AdaptersInfo"/>
        public abstract AdapterInfo[] AdaptersInfo { get; }


        /// <summary>
        /// Sets the Chartboost Mediation PreInitialization configuration. Setting this after initialization does nothing and returns an exception.
        /// </summary>
        public virtual ChartboostMediationError? SetPreInitializationConfiguration(ChartboostMediationPreInitializationConfiguration configuration)
        {
            LogController.Log($"SetPreInitializationConfiguration with Configuration: {JsonTools.SerializeObject(configuration)}", LogLevel.Info);
            return null;
        }

        /// <inheritdoc cref="ChartboostMediation.LoadFullscreenAd"/>
        public abstract Task<FullscreenAdLoadResult> LoadFullscreenAd(FullscreenAdLoadRequest request);
        
        /// <inheritdoc cref="ChartboostMediation.GetFullscreenAdQueue"/>
        public abstract IFullscreenAdQueue GetFullscreenAdQueue(string placementName);
        
        /// <inheritdoc cref="ChartboostMediation.GetBannerAd"/>
        public abstract IBannerAd GetBannerAd();
        
        /// <summary>
        /// Keeps track of initialization attempts.
        /// </summary>
        protected static bool IsInitialized { get; set; }
        
        /// <summary>
        /// Reusable method to check if we meet conditions to request <see cref="IAd"/> loads.
        /// </summary>
        /// <param name="placementName">Identifier for the Chartboost placement to load.</param>
        /// <returns><see cref="bool"/> validation status.</returns>
        internal static bool CanFetchAd(string placementName)
        {
            if (!IsInitialized)
            {
                LogController.Log("Unable to fetch Ads, Chartboost Mediation has not been initialized.", LogLevel.Info);
                return false;
            }

            if (!string.IsNullOrEmpty(placementName)) 
                return true;
            
            LogController.Log("Unable to fetch Ads, placement cannot be null or empty.", LogLevel.Error);
            return false;
        }
    }
}
