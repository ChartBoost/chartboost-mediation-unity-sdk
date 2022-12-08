#if UNITY_ANDROID
using Helium.Platforms;
using UnityEngine;

namespace Helium.FullScreen.Rewarded
{
    /// <summary>
    /// Helium rewarded ad object for Android.
    /// </summary>
    public class HeliumRewardedAndroid : HeliumRewardedBase
    {
        private readonly AndroidJavaObject _androidAd;

        public HeliumRewardedAndroid(string placementName) : base(placementName)
        {
            LogTag = "HeliumRewarded (Android)";
            _androidAd = HeliumAndroid.plugin().Call<AndroidJavaObject>("getRewardedAd", placementName);
        }
        
        // *NOTE* Implementation for Rewarded/FullScreen is very similar, and it could be simplified on a single file,
        // for now it will stay separated in case placement specific placement changes are required. This only applies for Android.

        /// <inheritdoc cref="HeliumRewardedBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="HeliumRewardedBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="HeliumRewardedBase.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
        }

        /// <inheritdoc cref="HeliumRewardedBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _androidAd.Call("load");
        }

        /// <inheritdoc cref="HeliumRewardedBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _androidAd.Call("show");
        }

        /// <inheritdoc cref="HeliumRewardedBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _androidAd.Call<bool>("readyToShow");
        }

        /// <inheritdoc cref="HeliumRewardedBase.ClearLoaded"/>>
        public override bool ClearLoaded()
        {
            base.ClearLoaded();
            return _androidAd.Call<bool>("clearLoaded");
        }

        /// <inheritdoc cref="HeliumRewardedBase.SetCustomData"/>>
        public override void SetCustomData(string customData)
        {
            base.SetCustomData(customData);
            _androidAd.Call("setCustomData", customData);
        }
    }
}
#endif
