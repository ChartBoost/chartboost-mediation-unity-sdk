#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Helium.Banner
{
    public class HeliumBannerIOS : HeliumBannerBase
    {
        private readonly IntPtr _uniqueId;

        public HeliumBannerIOS(string placement, HeliumBannerAdSize size) : base(placement, size)
        {
            LOGTag = "HeliumBanner (iOS)";
            _uniqueId = _heliumSdkGetBannerAd(placement, (int)size);
        }

        /// <inheritdoc cref="HeliumBannerBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _heliumSdkBannerSetKeyword(_uniqueId, keyword, value);
        }

        /// <inheritdoc cref="HeliumBannerBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _heliumSdkBannerRemoveKeyword(_uniqueId, keyword);
        }

        /// <inheritdoc cref="HeliumBannerBase.Load"/>>
        public override void Load(HeliumBannerAdScreenLocation location)
        {
            base.Load(location);
            _heliumSdkBannerAdLoad(_uniqueId, (int)location);
        }

        /// <inheritdoc cref="HeliumBannerBase.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
        {
            base.SetVisibility(isVisible);
            _heliumSdkBannerSetVisibility(_uniqueId, isVisible);
        }

        /// <inheritdoc cref="HeliumBannerBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _heliumSdkBannerClearLoaded(_uniqueId);
        }

        /// <inheritdoc cref="HeliumBannerBase.Remove"/>>
        public override void Remove()
        {
            base.Remove();
            _heliumSdkBannerRemove(_uniqueId);
        }

        ~HeliumBannerIOS()
            => _heliumSdkFreeBannerAdObject(_uniqueId);

        #region EXTERNAL_METHODS
        [DllImport("__Internal")]
        private static extern IntPtr _heliumSdkGetBannerAd(string placementName, int size);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkBannerSetKeyword(IntPtr uniqueId, string keyword, string value);
        [DllImport("__Internal")]
        private static extern string _heliumSdkBannerRemoveKeyword(IntPtr uniqueID, string keyword);
        [DllImport("__Internal")]
        private static extern void _heliumSdkBannerAdLoad(IntPtr uniqueID, int screenLocation);
        [DllImport("__Internal")]
        private static extern void _heliumSdkBannerClearLoaded(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkBannerRemove(IntPtr uniqueID);
        [DllImport("__Internal")]
        private static extern bool _heliumSdkBannerSetVisibility(IntPtr uniqueID, bool isVisible);
        [DllImport("__Internal")]
        private static extern void _heliumSdkFreeBannerAdObject(IntPtr uniqueID);
        #endregion
    }
}
#endif
