namespace Chartboost.FullScreen
{
    /// <summary>
    /// Helium fullscreen ad base structure.
    /// </summary>
    public abstract class ChartboostMediationFullScreenBase : IChartboostMediationAd, IChartboostMediationFullScreenAd
    {
        protected static string LogTag = "HeliumFullScreen (Base)";
        protected readonly string PlacementName;

        protected ChartboostMediationFullScreenBase(string placementName)
            => PlacementName = placementName;

        /// <inheritdoc cref="IChartboostMediationAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            Logger.Log(LogTag, $"fullscreen: {PlacementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IChartboostMediationAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            Logger.Log(LogTag, $"fullscreen: {PlacementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IChartboostMediationAd.Destroy"/>>
        public virtual void Destroy()
            => Logger.Log(LogTag, $"destroying fullscreen: {PlacementName}");

        /// <inheritdoc cref="IChartboostMediationFullScreenAd.Load"/>>
        public virtual void Load()
            => Logger.Log(LogTag, $"loading fullscreen: {PlacementName}");

        /// <inheritdoc cref="IChartboostMediationFullScreenAd.Show"/>>
        public virtual void Show()
            => Logger.Log(LogTag, $"showing fullscreen: {PlacementName}");

        /// <inheritdoc cref="IChartboostMediationFullScreenAd.ReadyToShow"/>>
        public virtual bool ReadyToShow()
        {
            Logger.Log(LogTag, $"checking fullscreen: {PlacementName} availability");
            return false;
        }

        /// <inheritdoc cref="IChartboostMediationFullScreenAd.ClearLoaded"/>>
        public virtual void ClearLoaded()
        {
            Logger.Log(LogTag, $"clearing fullscreen: {PlacementName}");
        }
    }
    
    /// <summary>
    /// Helium interstitial ad object for unsupported platforms.
    /// </summary>
    public class ChartboostMediationInterstitialUnsupported : ChartboostMediationFullScreenBase
    {
        public ChartboostMediationInterstitialUnsupported(string placementName) : base(placementName)
        {
            LogTag = "HeliumInterstitial (Unsupported)";
        }
    }
}
