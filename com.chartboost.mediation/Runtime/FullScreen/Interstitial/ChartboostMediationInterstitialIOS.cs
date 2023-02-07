#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// Helium interstitial object for iOS.
    /// </summary>
    public class ChartboostMediationInterstitialIOS : ChartboostMediationFullScreenBase
    {
        private readonly IntPtr _uniqueId;

        public ChartboostMediationInterstitialIOS(string placementName) : base(placementName)
        {
            LogTag = "HeliumInterstitial (iOS)";
            _uniqueId = _heliumSdkGetInterstitialAd(placementName);
        }
        
        /// <inheritdoc cref="ChartboostMediationFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _heliumSdkInterstitialSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _heliumSdkInterstitialRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Load"/>>
        public override void Load()
        {
            base.Load();
            GC.Collect(); // make sure previous i12 ads get destructed if necessary
            _heliumSdkInterstitialAdLoad(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _heliumSdkInterstitialAdShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _heliumSdkInterstitialAdReadyToShow(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _heliumSdkInterstitialClearLoaded(_uniqueId);
        }

        ~ChartboostMediationInterstitialIOS() 
            => _heliumSdkFreeInterstitialAdObject(_uniqueId);

        #region External Methods
        [DllImport("__Internal")]
        private static extern IntPtr _heliumSdkGetInterstitialAd(string placementName);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkInterstitialSetKeyword(IntPtr uniqueID, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _heliumSdkInterstitialRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _heliumSdkInterstitialAdLoad(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkInterstitialClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _heliumSdkInterstitialAdShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkInterstitialAdReadyToShow(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _heliumSdkFreeInterstitialAdObject(IntPtr uniqueID);
        #endregion
    }
}
#endif
