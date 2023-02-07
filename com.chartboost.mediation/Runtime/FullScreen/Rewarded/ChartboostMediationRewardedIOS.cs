#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Chartboost.FullScreen.Rewarded
{
    public class ChartboostMediationRewardedIOS : ChartboostMediationRewardedBase
    {
        private readonly IntPtr _uniqueId;

        public ChartboostMediationRewardedIOS(string placementName) : base(placementName)
        {
            LogTag = "HeliumRewarded (iOS)";
            _uniqueId = _heliumSdkGetRewardedAd(placementName);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _heliumSdkRewardedSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _heliumSdkRewardedRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _heliumSdkRewardedAdLoad(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _heliumSdkRewardedAdShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _heliumSdkRewardedAdReadyToShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _heliumSdkRewardedClearLoaded(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.SetCustomData"/>>
        public override void SetCustomData(string customData)
        {
            base.SetCustomData(customData);
            _heliumSdkRewardedAdSetCustomData(_uniqueId, customData);
        }

        ~ChartboostMediationRewardedIOS()
            => _heliumSdkFreeRewardedAdObject(_uniqueId);

        #region External Methods
        [DllImport("__Internal")]
        private static extern IntPtr _heliumSdkGetRewardedAd(string placementName);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkRewardedSetKeyword(IntPtr uniqueID, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _heliumSdkRewardedRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _heliumSdkRewardedAdLoad(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkRewardedClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _heliumSdkRewardedAdShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkRewardedAdReadyToShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _heliumSdkRewardedAdSetCustomData(IntPtr uniqueID, string customData);
        [DllImport("__Internal")]
        private static extern void _heliumSdkFreeRewardedAdObject(IntPtr uniqueID);
        #endregion
    }
}
#endif
