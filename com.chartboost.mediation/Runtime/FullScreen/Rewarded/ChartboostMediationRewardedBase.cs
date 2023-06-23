using System;
using Chartboost.Utilities;

namespace Chartboost.FullScreen.Rewarded
{
    /// <summary>
    /// Chartboost Mediation rewarded ad base structure.
    /// </summary>
    [Obsolete("ChartboostMediationRewardedBase has been deprecated, use the new fullscreen API instead.")]
    public abstract class ChartboostMediationRewardedBase : ChartboostMediationFullScreenBase, IChartBoostMediationRewardedAd
    {
        public ChartboostMediationRewardedBase(string placementName) : base(placementName) => logTag = "ChartboostMediationRewarded (Base)";

        /// <inheritdoc cref="IChartBoostMediationRewardedAd.SetCustomData"/>>
        public virtual void SetCustomData(string customData) => Logger.Log(logTag, $"rewarded: {placementName}, setting custom data: {customData}");

    }
    
    /// <summary>
    /// Chartboost Mediation rewarded ad object for unsupported platforms.
    /// </summary>
    public sealed class ChartboostMediationRewardedUnsupported : ChartboostMediationRewardedBase
    {
        internal override bool IsValid { get; set; }

        public ChartboostMediationRewardedUnsupported(string placementName) : base(placementName) => logTag = "ChartboostMediationRewarded (Unsupported)";
    }
}
