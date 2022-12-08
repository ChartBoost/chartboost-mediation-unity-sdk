namespace Helium.FullScreen.Rewarded
{
    public class HeliumRewardedBase : HeliumFullScreenBase, IHeliumRewardedAd
    {
        public HeliumRewardedBase(string placementName) : base(placementName) 
            => LOGTag = "HeliumRewarded (Base)";

        /// <inheritdoc cref="IHeliumRewardedAd.SetCustomData"/>>
        public virtual void SetCustomData(string customData) => HeliumLogger.Log(LOGTag, $"rewarded: {PlacementName}, setting custom data: {customData}");
    }
    
    public class HeliumRewardedUnsupported : HeliumRewardedBase
    {
        public HeliumRewardedUnsupported(string placementName) : base(placementName) => LOGTag = "HeliumRewarded (Unsupported)";
    }
}
