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
        protected readonly string PlacementName;
        protected readonly ChartboostMediationBannerAdSize Size;
        internal abstract bool IsValid { get; set; }

        protected ChartboostMediationBannerBase(string placementName, ChartboostMediationBannerAdSize size)
        {
            PlacementName = placementName;
            Size = size;
        }

        /// <inheritdoc cref="IChartboostMediationAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            Logger.Log(LogTag, $"banner: {PlacementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IChartboostMediationAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            Logger.Log(LogTag, $"banner: {PlacementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IChartboostMediationAd.Destroy"/>>
        public virtual void Destroy()
        {
            Logger.Log(LogTag, $"destroying banner: {PlacementName}");
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load"/>>
        public virtual void Load(ChartboostMediationBannerAdScreenLocation location)
            => Logger.Log(LogTag, $"loading banner: {PlacementName} with size: {Size} at {location}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetVisibility"/>>
        public virtual void SetVisibility(bool isVisible)
            => Logger.Log(LogTag, $"setting visibility: {isVisible} for banner: {PlacementName}");

        public virtual void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment) 
            => Logger.Log(LogTag, $"setting horizontal alignment: {horizontalAlignment} for banner: {PlacementName}");

        public virtual void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment)
            => Logger.Log(LogTag, $"setting vertical alignment: {verticalAlignment} for banner: {PlacementName}");

        public virtual ChartboostMediationBannerAdSize GetAdSize()
        {
            Logger.Log(LogTag, $"getting ad size for banner: {PlacementName}");
            return null;
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.ClearLoaded"/>>
        public virtual void ClearLoaded()
            => Logger.Log(LogTag, $"clearing banner: {PlacementName}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.Remove"/>>
        [Obsolete("Remove has been deprecated, please use Destroy instead.")]
        public virtual void Remove()
            => Logger.Log(LogTag, $"removing banner: {PlacementName}");
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
