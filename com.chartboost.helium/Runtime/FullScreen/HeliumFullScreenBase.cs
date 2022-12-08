namespace Helium.FullScreen
{
    public abstract class HeliumFullScreenBase : IHeliumAd, IHeliumFullScreenAd
    {
        protected static string LOGTag = "HeliumFullScreen (Base)";
        private readonly string _placementName;

        protected HeliumFullScreenBase(string placementName)
            => _placementName = placementName;

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            HeliumLogger.Log(LOGTag, $"fullscreen: {_placementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            HeliumLogger.Log(LOGTag, $"fullscreen: {_placementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IHeliumAd.Destroy"/>>
        public virtual void Destroy()
            => HeliumLogger.Log(LOGTag, $"destroying fullscreen: {_placementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.Load"/>>
        public virtual void Load()
            => HeliumLogger.Log(LOGTag, $"loading fullscreen: {_placementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.Show"/>>
        public virtual void Show()
            => HeliumLogger.Log(LOGTag, $"showing fullscreen: {_placementName}");

        /// <inheritdoc cref="IHeliumFullScreenAd.ReadyToShow"/>>
        public virtual bool ReadyToShow()
        {
            HeliumLogger.Log(LOGTag, $"checking fullscreen: {_placementName} availability");
            return false;
        }

        /// <inheritdoc cref="IHeliumFullScreenAd.ClearLoaded"/>>
        public virtual bool ClearLoaded()
        {
            HeliumLogger.Log(LOGTag, $"clearing fullscreen: {_placementName}");
            return true;
        }
    }

    public class HeliumInterstitialUnsupported : HeliumFullScreenBase
    {
        public HeliumInterstitialUnsupported(string placementName) : base(placementName)
        {
            LOGTag = "HeliumInterstitial (Unsupported)";
        }
    }
}
