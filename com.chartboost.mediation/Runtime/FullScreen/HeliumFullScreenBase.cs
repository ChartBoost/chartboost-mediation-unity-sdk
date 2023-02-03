using Helium.Interfaces;

namespace Helium.FullScreen
{
    /// <summary>
    /// Helium fullscreen ad base structure.
    /// </summary>
    public abstract class HeliumFullScreenBase : IHeliumAd, IHeliumFullScreenAd
    {
        protected static string LogTag = "HeliumFullScreen (Base)";
        protected readonly string PlacementName;

        protected HeliumFullScreenBase(string placementName)
            => PlacementName = placementName;

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            HeliumLogger.Log(LogTag, $"fullscreen: {PlacementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            HeliumLogger.Log(LogTag, $"fullscreen: {PlacementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IHeliumAd.Destroy"/>>
        public virtual void Destroy()
            => HeliumLogger.Log(LogTag, $"destroying fullscreen: {PlacementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.Load"/>>
        public virtual void Load()
            => HeliumLogger.Log(LogTag, $"loading fullscreen: {PlacementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.Show"/>>
        public virtual void Show()
            => HeliumLogger.Log(LogTag, $"showing fullscreen: {PlacementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.ReadyToShow"/>>
        public virtual bool ReadyToShow()
        {
            HeliumLogger.Log(LogTag, $"checking fullscreen: {PlacementName} availability");
            return false;
        }

        /// <inheritdoc cref="IHeliumFullScreenAd.ClearLoaded"/>>
        public virtual void ClearLoaded()
        {
            HeliumLogger.Log(LogTag, $"clearing fullscreen: {PlacementName}");
        }
    }
    
    /// <summary>
    /// Helium interstitial ad object for unsupported platforms.
    /// </summary>
    public class HeliumInterstitialUnsupported : HeliumFullScreenBase
    {
        public HeliumInterstitialUnsupported(string placementName) : base(placementName)
        {
            LogTag = "HeliumInterstitial (Unsupported)";
        }
    }
}
