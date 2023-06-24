#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner object for iOS.
    /// </summary>
    public sealed class ChartboostMediationBannerIOS : ChartboostMediationBannerBase
    {
        private readonly IntPtr _uniqueId;

        public ChartboostMediationBannerIOS(string placement, ChartboostMediationBannerAdSize size) : base(placement, size)
        {
            LogTag = "ChartboostMediation Banner (iOS)";
            _uniqueId = _chartboostMediationGetBannerAd(placement, (int)size);
        }

        internal override bool IsValid { get; set; } = true;

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _chartboostMediationBannerSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _chartboostMediationBannerRemoveKeyword(_uniqueId, keyword);
        }

        public override void Destroy()
        {
            base.Destroy();
            _chartboostMediationBannerRemove(_uniqueId);
            _chartboostMediationFreeAdObject(_uniqueId, placementName, true);
            IsValid = false;
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location)
        {
            base.Load(location);
            _chartboostMediationBannerAdLoad(_uniqueId, (int)location);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
        {
            base.SetVisibility(isVisible);
            _chartboostMediationBannerSetVisibility(_uniqueId, isVisible);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _chartboostMediationBannerClearLoaded(_uniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Remove"/>>
        public override void Remove()
        {
            base.Remove();
            Destroy();
        }

        #region External Methods
        [DllImport("__Internal")]
        private static extern IntPtr _chartboostMediationGetBannerAd(string placementName, int size);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationBannerSetKeyword(IntPtr uniqueId, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerAdLoad(IntPtr uniqueID, int screenLocation);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerRemove(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationBannerSetVisibility(IntPtr uniqueID, bool isVisible);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationFreeAdObject(IntPtr uniqueID, string placementName, bool multiPlacementSupport);
        #endregion
    }
}
#endif
