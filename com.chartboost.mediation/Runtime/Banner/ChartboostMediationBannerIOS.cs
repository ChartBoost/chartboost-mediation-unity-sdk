#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

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
            _uniqueId = _chartboostMediationGetBannerAd();
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

        public override void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment)
        {
            base.SetHorizontalAlignment(horizontalAlignment);
            _chartboostMediationBannerSetHorizontalAlignment(_uniqueId, (int)horizontalAlignment);
        }

        public override void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment)
        {
            base.SetVerticalAlignment(verticalAlignment);
            _chartboostMediationBannerSetVerticalAlignment(_uniqueId, (int)verticalAlignment);

        }

        public override ChartboostMediationBannerAdSize GetAdSize()
        {
            base.GetAdSize();
            var sizeJson = _chartboostMediationBannerGetAdSize(_uniqueId);
            var size = JsonConvert.DeserializeObject<ChartboostMediationBannerAdSize>(sizeJson);
            return size;
        }

        public override void Destroy()
        {
            base.Destroy();
            _chartboostMediationBannerRemove(_uniqueId);
            _chartboostMediationFreeAdObject(_uniqueId, PlacementName, true);
            IsValid = false;
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location)
        {
            base.Load(location);
            var sizeJson = JsonConvert.SerializeObject(Size);
            _chartboostMediationBannerAdLoad(_uniqueId, PlacementName, sizeJson, (int)location);
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
        private static extern IntPtr _chartboostMediationGetBannerAd();
        [DllImport("__Internal")]
        private static extern bool _chartboostMediationBannerSetKeyword(IntPtr uniqueId, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetHorizontalAlignment(IntPtr uniqueId, int verticalAlignment);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetVerticalAlignment(IntPtr uniqueId, int verticalAlignment);
        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerGetAdSize(IntPtr uniqueId);
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerAdLoad(IntPtr uniqueID, string placementName, string size, int screenLocation);
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
