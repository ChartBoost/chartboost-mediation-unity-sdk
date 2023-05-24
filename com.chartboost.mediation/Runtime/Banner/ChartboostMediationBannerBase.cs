using System;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner base structure.
    /// </summary>
    public abstract class ChartboostMediationBannerBase : IChartboostMediationAd, IChartboostMediationBannerAd
    {
        protected static string LogTag = "ChartboostMediationBanner (Base)";
        protected readonly string placementName;
        private readonly ChartboostMediationBannerAdSize _size;

        protected ChartboostMediationBannerBase(string placementName, ChartboostMediationBannerAdSize size)
        {
            this.placementName = placementName;
            _size = size;
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
            => Logger.Log(LogTag, $"destroying banner: {placementName}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load"/>>
        public virtual void Load(ChartboostMediationBannerAdScreenLocation location)
            => Logger.Log(LogTag, $"loading banner: {placementName} with size: {_size} at {location}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load(float, float, int, int)"/>
        public virtual void Load(float x, float y, int width, int height)
            => Logger.Log(LogTag, $"loading banner: {placementName} at position :{x},{y} and size : {width},{height}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetVisibility"/>>
        public virtual void SetVisibility(bool isVisible)
            => Logger.Log(LogTag, $"setting visibility: {isVisible} for banner: {placementName}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.ClearLoaded"/>>
        public virtual void ClearLoaded()
            => Logger.Log(LogTag, $"clearing banner: {placementName}");

        /// <inheritdoc cref="IChartboostMediationBannerAd.Remove"/>>
        public virtual void Remove()
            => Logger.Log(LogTag, $"removing banner: {placementName}");

        public virtual void EnableDrag(Action<float, float> onDrag = null)
            => Logger.Log(LogTag, $"Dragging enabled");

        public virtual void DisableDrag()
            => Logger.Log(LogTag, $"Dragging disabled");
    }

    /// <summary>
    /// Chartboost Mediation banner object for unsupported platforms.
    /// </summary>
    public class ChartboostMediationBannerUnsupported : ChartboostMediationBannerBase
    {
        public ChartboostMediationBannerUnsupported(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size)
        {
            LogTag = "ChartboostMediationBanner (Unsupported)";
        }
    }
}
