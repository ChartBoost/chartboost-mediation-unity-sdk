namespace Helium.FullScreen.Rewarded
{
    /// <summary>
    /// Helium rewarded ad base structure.
    /// </summary>
    public class HeliumRewardedBase : HeliumFullScreenBase, IHeliumRewardedAd
    {
        public HeliumRewardedBase(string placementName) : base(placementName) 
            => LogTag = "HeliumRewarded (Base)";

        /// <inheritdoc cref="IHeliumRewardedAd.SetCustomData"/>>
        public virtual void SetCustomData(string customData) => HeliumLogger.Log(LogTag, $"rewarded: {PlacementName}, setting custom data: {customData}");
    }
    
    /// <summary>
    /// Helium rewarded ad object for unsupported platforms.
    /// </summary>
    public class HeliumRewardedUnsupported : HeliumRewardedBase
    {
        public HeliumRewardedUnsupported(string placementName) : base(placementName) => LogTag = "HeliumRewarded (Unsupported)";
    }
}
