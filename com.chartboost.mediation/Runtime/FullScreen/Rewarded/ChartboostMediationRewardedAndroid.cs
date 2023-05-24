#if UNITY_ANDROID
using Chartboost.Platforms;
using UnityEngine;

namespace Chartboost.FullScreen.Rewarded
{
    /// <summary>
    /// Chartboost Mediation rewarded ad object for Android.
    /// </summary>
    public class ChartboostMediationRewardedAndroid : ChartboostMediationRewardedBase
    {
        private readonly AndroidJavaObject _androidAd;

        public ChartboostMediationRewardedAndroid(string placementName) : base(placementName)
        {
            logTag = "ChartboostMediationRewarded (Android)";
            _androidAd = ChartboostMediationAndroid.plugin().Call<AndroidJavaObject>("getRewardedAd", placementName);
        }
        
        // *NOTE* Implementation for Rewarded/FullScreen is very similar, and it could be simplified on a single file,
        // for now it will stay separated in case placement specific placement changes are required. This only applies for Android.

        /// <inheritdoc cref="ChartboostMediationRewardedBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _androidAd.Call("load");
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _androidAd.Call("show");
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _androidAd.Call<bool>("readyToShow");
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _androidAd.Call("clearLoaded");
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.SetCustomData"/>>
        public override void SetCustomData(string customData)
        {
            base.SetCustomData(customData);
            _androidAd.Call("setCustomData", customData);
        }
    }
}
#endif
