#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// Chartboost Mediation interstitial object for iOS.
    /// </summary>
    public sealed class ChartboostMediationInterstitialIOS : ChartboostMediationFullScreenBase
    {
        private readonly IntPtr _uniqueId;

        public ChartboostMediationInterstitialIOS(string placementName) : base(placementName)
        {
            logTag = "ChartboostMediationInterstitial (iOS)";
            _uniqueId = _chartboostMediationGetInterstitialAd(placementName);
        }

        internal override bool IsValid { get; set; } = true;

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _chartboostMediationInterstitialSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _chartboostMediationInterstitialRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _chartboostMediationInterstitialAdLoad(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _chartboostMediationInterstitialAdShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _chartboostMediationInterstitialAdReadyToShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _chartboostMediationInterstitialClearLoaded(_uniqueId);
        }

        public override void Destroy()
        {
            base.Destroy();
            _chartboostMediationFreeAdObject(_uniqueId, placementName, false);
            IsValid = false;
        }

        #region External Methods
        [DllImport("__Internal")]
        private static extern IntPtr _chartboostMediationGetInterstitialAd(string placementName);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationInterstitialSetKeyword(IntPtr uniqueID, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _chartboostMediationInterstitialRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationInterstitialAdLoad(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationInterstitialClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationInterstitialAdShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationInterstitialAdReadyToShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationFreeAdObject(IntPtr uniqueID, string placementName, bool multiPlacementSupport);
        #endregion
    }
}
#endif
