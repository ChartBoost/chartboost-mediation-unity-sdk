#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Helium.FullScreen.Rewarded
{
    public class HeliumRewardedIOS : HeliumRewardedBase
    {
        private readonly IntPtr _uniqueId;

        public HeliumRewardedIOS(string placementName) : base(placementName)
        {
            LogTag = "HeliumRewarded (iOS)";
            _uniqueId = _heliumSdkGetRewardedAd(placementName);
        }

        /// <inheritdoc cref="HeliumRewardedBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _heliumSdkRewardedSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="HeliumRewardedBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _heliumSdkRewardedRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="HeliumRewardedBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _heliumSdkRewardedAdLoad(_uniqueId);
        }

        /// <inheritdoc cref="HeliumRewardedBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _heliumSdkRewardedAdShow(_uniqueId);
        }

        /// <inheritdoc cref="HeliumRewardedBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _heliumSdkRewardedAdReadyToShow(_uniqueId);
        }

        /// <inheritdoc cref="HeliumRewardedBase.ClearLoaded"/>>
        public override bool ClearLoaded()
        {
            base.ClearLoaded();
            return _heliumSdkRewardedClearLoaded(_uniqueId);
        }

        /// <inheritdoc cref="HeliumRewardedBase.SetCustomData"/>>
        public override void SetCustomData(string customData)
        {
            base.SetCustomData(customData);
            _heliumSdkRewardedAdSetCustomData(_uniqueId, customData);
        }

        ~HeliumRewardedIOS()
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
