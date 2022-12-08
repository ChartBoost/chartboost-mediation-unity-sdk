using Helium.Interfaces;

namespace Helium.FullScreen
{
    /// <summary>
    /// Helium fullscreen ad base structure.
    /// </summary>
    public abstract class HeliumFullScreenBase : IHeliumAd, IHeliumFullScreenAd
    {
        protected static string LOGTag = "HeliumFullScreen (Base)";
        protected readonly string PlacementName;

        protected HeliumFullScreenBase(string placementName)
            => PlacementName = placementName;

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            HeliumLogger.Log(LOGTag, $"fullscreen: {PlacementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            HeliumLogger.Log(LOGTag, $"fullscreen: {PlacementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IHeliumAd.Destroy"/>>
        public virtual void Destroy()
            => HeliumLogger.Log(LOGTag, $"destroying fullscreen: {PlacementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.Load"/>>
        public virtual void Load()
            => HeliumLogger.Log(LOGTag, $"loading fullscreen: {PlacementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.Show"/>>
        public virtual void Show()
            => HeliumLogger.Log(LOGTag, $"showing fullscreen: {PlacementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.ReadyToShow"/>>
        public virtual bool ReadyToShow()
        {
            HeliumLogger.Log(LOGTag, $"checking fullscreen: {PlacementName} availability");
            return false;
        }

        /// <inheritdoc cref="IHeliumFullScreenAd.ClearLoaded"/>>
        public virtual bool ClearLoaded()
        {
            HeliumLogger.Log(LOGTag, $"clearing fullscreen: {PlacementName}");
            return true;
        }
    }
    
    /// <summary>
    /// Helium interstitial ad object for unsupported platforms.
    /// </summary>
    public class HeliumInterstitialUnsupported : HeliumFullScreenBase
    {
        public HeliumInterstitialUnsupported(string placementName) : base(placementName)
        {
            LOGTag = "HeliumInterstitial (Unsupported)";
        }
    }
}
