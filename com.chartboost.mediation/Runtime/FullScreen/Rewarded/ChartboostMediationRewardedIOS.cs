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
            LogTag = "ChartboostMediationRewarded (iOS)";
            _uniqueId = _chartboostMediationGetRewardedAd(placementName);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _chartboostMediationRewardedSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _chartboostMediationRewardedRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _chartboostMediationRewardedAdLoad(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _chartboostMediationRewardedAdShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _chartboostMediationRewardedAdReadyToShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _chartboostMediationRewardedClearLoaded(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationRewardedBase.SetCustomData"/>>
        public override void SetCustomData(string customData)
        {
            base.SetCustomData(customData);
            _chartboostMediationRewardedAdSetCustomData(_uniqueId, customData);
        }

        ~ChartboostMediationRewardedIOS()
            => _chartboostMediationFreeRewardedAdObject(_uniqueId);

        #region External Methods
        [DllImport("__Internal")]
        private static extern IntPtr _chartboostMediationGetRewardedAd(string placementName);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationRewardedSetKeyword(IntPtr uniqueID, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _chartboostMediationRewardedRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationRewardedAdLoad(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationRewardedClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationRewardedAdShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationRewardedAdReadyToShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationRewardedAdSetCustomData(IntPtr uniqueID, string customData);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationFreeRewardedAdObject(IntPtr uniqueID);
        #endregion
    }
}
#endif
