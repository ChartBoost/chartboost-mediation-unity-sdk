#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Helium.Banner
{
    public class HeliumBannerIOS : HeliumBannerBase
    {
        private readonly IntPtr _uniqueId;

        public HeliumBannerIOS(string placement, HeliumBannerAdSize size) : base(placement, size)
            => _uniqueId = _heliumSdkGetBannerAd(placement, (int)size);

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
            => _heliumSdkBannerSetKeyword(_uniqueId, keyword, value);

        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
            => _heliumSdkBannerRemoveKeyword(_uniqueId, keyword);

        /// <inheritdoc cref="IHeliumBannerAd.Load"/>>
        public override void Load(HeliumBannerAdScreenLocation location)
            => _heliumSdkBannerAdLoad(_uniqueId, (int)location);

        /// <inheritdoc cref="IHeliumBannerAd.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
            => _heliumSdkBannerSetVisibility(_uniqueId, isVisible);

        /// <inheritdoc cref="IHeliumBannerAd.ClearLoaded"/>>
        public override void ClearLoaded()
            => _heliumSdkBannerClearLoaded(_uniqueId);

        /// <inheritdoc cref="IHeliumBannerAd.Remove"/>>
        public override void Remove()
            => _heliumSdkBannerRemove(_uniqueId);

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
