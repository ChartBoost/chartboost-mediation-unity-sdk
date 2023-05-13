namespace Chartboost.FullScreen.Rewarded
{
    /// <summary>
    /// Chartboost Mediation rewarded ad base structure.
    /// </summary>
    public class ChartboostMediationRewardedBase : ChartboostMediationFullScreenBase, IChartBoostMediationRewardedAd
    {
        public ChartboostMediationRewardedBase(string placementName) : base(placementName) => logTag = "ChartboostMediationRewarded (Base)";

        /// <inheritdoc cref="IChartBoostMediationRewardedAd.SetCustomData"/>>
        public virtual void SetCustomData(string customData) => Logger.Log(logTag, $"rewarded: {placementName}, setting custom data: {customData}");
    }
    
    /// <summary>
    /// Chartboost Mediation rewarded ad object for unsupported platforms.
    /// </summary>
    public class ChartboostMediationRewardedUnsupported : ChartboostMediationRewardedBase
    {
        public ChartboostMediationRewardedUnsupported(string placementName) : base(placementName) => logTag = "ChartboostMediationRewarded (Unsupported)";
    }
}
