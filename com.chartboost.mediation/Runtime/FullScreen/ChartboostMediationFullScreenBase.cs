using System;
using Chartboost.Interfaces;
using Chartboost.Utilities;

namespace Chartboost.FullScreen
{
    /// <summary>
    /// Chartboost Mediation fullscreen ad base structure.
    /// </summary>
    [Obsolete("ChartboostMediationFullScreenBase has been deprecated, use the new fullscreen API instead.")]
    public abstract class ChartboostMediationFullScreenBase : IChartboostMediationAd, IChartboostMediationFullScreenAdOld
    {
        protected static string logTag = "ChartboostMediationFullScreen (Base)";
        protected readonly string placementName;
        internal abstract bool IsValid { get; set; }

        protected ChartboostMediationFullScreenBase(string placementName) 
            => this.placementName = placementName;

        /// <inheritdoc cref="IChartboostMediationAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            Logger.Log(logTag, $"fullscreen: {placementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IChartboostMediationAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            Logger.Log(logTag, $"fullscreen: {placementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IChartboostMediationAd.Destroy"/>>
        public virtual void Destroy()
        {
            Logger.Log(logTag, $"destroying fullscreen: {placementName}");
        }

        /// <inheritdoc cref="IChartboostMediationFullScreenAdOld.Load"/>>
        public virtual void Load()
            => Logger.Log(logTag, $"loading fullscreen: {placementName}");

        /// <inheritdoc cref="IChartboostMediationFullScreenAdOld.Show"/>>
        public virtual void Show()
            => Logger.Log(logTag, $"showing fullscreen: {placementName}");

        /// <inheritdoc cref="IChartboostMediationFullScreenAdOld.ReadyToShow"/>>
        public virtual bool ReadyToShow()
        {
            Logger.Log(logTag, $"checking fullscreen: {placementName} availability");
            return false;
        }

        /// <inheritdoc cref="IChartboostMediationFullScreenAdOld.ClearLoaded"/>>
        public virtual void ClearLoaded()
        {
            Logger.Log(logTag, $"clearing fullscreen: {placementName}");
        }
    }
    
    /// <summary>
    /// Chartboost Mediation interstitial ad object for unsupported platforms.
    /// </summary>
    internal class ChartboostMediationInterstitialUnsupported : ChartboostMediationFullScreenBase
    {
        public ChartboostMediationInterstitialUnsupported(string placementName) : base(placementName)
        {
            logTag = "ChartboostMediationInterstitial (Unsupported)";
        }

        internal override bool IsValid { get; set; }
    }
}
