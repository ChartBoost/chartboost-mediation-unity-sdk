#if UNITY_ANDROID
using Helium.Platforms;
using UnityEngine;

namespace Helium.Banner
{
    public class HeliumBannerAndroid : HeliumBannerBase
    {
        private readonly AndroidJavaObject _androidAd;

        public HeliumBannerAndroid(string placementName, HeliumBannerAdSize size) : base(placementName, size)
            => _androidAd = HeliumAndroid.plugin().Call<AndroidJavaObject>("getBannerAd", placementName, (int)size);

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
            => _androidAd.Call<bool>("setKeyword", keyword, value);

        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
            => _androidAd.Call<string>("removeKeyword", keyword);

        /// <inheritdoc cref="IHeliumAd.Destroy"/>>
        public override void Destroy()
            => _androidAd.Call("destroy");

        /// <inheritdoc cref="IHeliumBannerAd.Load"/>>
        public override void Load(HeliumBannerAdScreenLocation location)
            => _androidAd.Call("load", (int)location);

        /// <inheritdoc cref="IHeliumBannerAd.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
            => _androidAd.Call("setBannerVisibility", isVisible);

        /// <inheritdoc cref="IHeliumBannerAd.ClearLoaded"/>>
        public override void ClearLoaded()
            => _androidAd.Call("clearLoaded");

        /// <inheritdoc cref="IHeliumBannerAd.Remove"/>>
        public override void Remove()
            //android doesn't have a remove method. Instead, calling destroy
            => Destroy();
    }
}
#endif
