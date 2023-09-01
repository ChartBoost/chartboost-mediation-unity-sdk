using System;
using Chartboost.Interfaces;
using Chartboost.Utilities;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner base structure.
    /// </summary>
    public abstract class ChartboostMediationBannerBase : IChartboostMediationAd, IChartboostMediationBannerAd
    {
        protected static string LogTag = "ChartboostMediationBanner (Base)";
        protected readonly string placementName;
        protected readonly ChartboostMediationBannerAdSize size;
        internal abstract bool IsValid { get; set; }

        protected ChartboostMediationBannerBase(string placementName, ChartboostMediationBannerAdSize size)
        {
            this.placementName = placementName;
            this.size = size;
        }

        /// <inheritdoc cref="IChartboostMediationAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            Logger.Log(LogTag, $"banner: {placementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IChartboostMediationAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            Logger.Log(LogTag, $"banner: {placementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IChartboostMediationAd.Destroy"/>>
        public virtual void Destroy()
        {
            Logger.Log(LogTag, $"destroying banner: {placementName}");
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load"/>>
        public virtual void Load(ChartboostMediationBannerAdScreenLocation location)
            => Logger.Log(LogTag, $"loading banner: {placementName} with size: {size} at {location}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetVisibility"/>>
        public virtual void SetVisibility(bool isVisible)
            => Logger.Log(LogTag, $"setting visibility: {isVisible} for banner: {placementName}");
       
        /// <inheritdoc cref="IChartboostMediationBannerAd.ClearLoaded"/>>
        public virtual void ClearLoaded()
            => Logger.Log(LogTag, $"clearing banner: {placementName}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.Remove"/>>
        [Obsolete("Remove has been deprecated, please use Destroy instead.")]
        public virtual void Remove()
            => Logger.Log(LogTag, $"removing banner: {placementName}");
    }

    /// <summary>
    /// Chartboost Mediation banner object for unsupported platforms.
    /// </summary>
    public sealed class ChartboostMediationBannerUnsupported : ChartboostMediationBannerBase
    {
        public ChartboostMediationBannerUnsupported(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size) 
            => LogTag = "ChartboostMediationBanner (Unsupported)";

        internal override bool IsValid { get; set; }
    }
}
