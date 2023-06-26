#if UNITY_ANDROID
using Chartboost.Platforms.Android;
using Chartboost.Utilities;
using UnityEngine;

namespace Chartboost.FullScreen.Rewarded
{
    /// <summary>
    /// Chartboost Mediation rewarded ad object for Android.
    /// </summary>
    public sealed class ChartboostMediationRewardedAndroid : ChartboostMediationRewardedBase
    {
        private readonly AndroidJavaObject _androidAd;
        private readonly int _uniqueId;

        public ChartboostMediationRewardedAndroid(string placementName) : base(placementName)
        {
            logTag = "ChartboostMediationRewarded (Android)";
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            _androidAd = unityBridge.CallStatic<AndroidJavaObject>("getRewardedAd", placementName);
            _uniqueId = _androidAd.HashCode();
        }
        
        internal override bool IsValid { get; set; } = true;

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
            IsValid = false;
            _androidAd.Dispose();
            AndroidAdStore.ReleaseLegacyAd(_uniqueId);
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
