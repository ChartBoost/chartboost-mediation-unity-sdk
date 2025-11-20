using System.Reflection;
using Chartboost.Mediation.Ad.Banner;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Utility class for triggering banner ad event handlers using reflection.
    /// Provides generic methods to invoke private event handlers for testing purposes.
    /// </summary>
    /// <typeparam name="TBanner">The banner type (BannerVisualElement or UnityBannerAd)</typeparam>
    public static class BannerEventTrigger<TBanner>
    {
        private const BindingFlags PrivateInstanceFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// Triggers the OnWillAppear event handler on the banner instance.
        /// </summary>
        /// <param name="bannerInstance">The banner instance (BannerVisualElement or UnityBannerAd)</param>
        /// <param name="bannerAd">The IBannerAd to pass to the event handler</param>
        /// <param name="methodNotFoundMessage">Error message if method not found (optional)</param>
        public static void TriggerOnWillAppear(TBanner bannerInstance, IBannerAd bannerAd, string methodNotFoundMessage = null)
        {
            var method = typeof(TBanner).GetMethod(TestConstants.Reflection.OnWillAppearMethod, PrivateInstanceFlags);
            Assert.IsNotNull(method, methodNotFoundMessage ?? $"{TestConstants.Reflection.OnWillAppearMethod} method not found");
            method.Invoke(bannerInstance, new object[] { bannerAd });
        }

        /// <summary>
        /// Triggers the OnClick event handler on the banner instance.
        /// </summary>
        /// <param name="bannerInstance">The banner instance (BannerVisualElement or UnityBannerAd)</param>
        /// <param name="bannerAd">The IBannerAd to pass to the event handler</param>
        /// <param name="methodNotFoundMessage">Error message if method not found (optional)</param>
        public static void TriggerOnClick(TBanner bannerInstance, IBannerAd bannerAd, string methodNotFoundMessage = null)
        {
            var method = typeof(TBanner).GetMethod(TestConstants.Reflection.OnClickMethod, PrivateInstanceFlags);
            Assert.IsNotNull(method, methodNotFoundMessage ?? $"{TestConstants.Reflection.OnClickMethod} method not found");
            method.Invoke(bannerInstance, new object[] { bannerAd });
        }

        /// <summary>
        /// Triggers the OnRecordImpression event handler on the banner instance.
        /// </summary>
        /// <param name="bannerInstance">The banner instance (BannerVisualElement or UnityBannerAd)</param>
        /// <param name="bannerAd">The IBannerAd to pass to the event handler</param>
        /// <param name="methodNotFoundMessage">Error message if method not found (optional)</param>
        public static void TriggerOnRecordImpression(TBanner bannerInstance, IBannerAd bannerAd, string methodNotFoundMessage = null)
        {
            var method = typeof(TBanner).GetMethod(TestConstants.Reflection.OnRecordImpressionMethod, PrivateInstanceFlags);
            Assert.IsNotNull(method, methodNotFoundMessage ?? $"{TestConstants.Reflection.OnRecordImpressionMethod} method not found");
            method.Invoke(bannerInstance, new object[] { bannerAd });
        }

        /// <summary>
        /// Triggers the OnDragBegin event handler on the banner instance.
        /// </summary>
        /// <param name="bannerInstance">The banner instance (BannerVisualElement or UnityBannerAd)</param>
        /// <param name="bannerAd">The IBannerAd to pass to the event handler</param>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="methodNotFoundMessage">Error message if method not found (optional)</param>
        public static void TriggerOnDragBegin(TBanner bannerInstance, IBannerAd bannerAd, float x, float y, string methodNotFoundMessage = null)
        {
            var method = typeof(TBanner).GetMethod(TestConstants.Reflection.OnDragBeginMethod, PrivateInstanceFlags);
            Assert.IsNotNull(method, methodNotFoundMessage ?? $"{TestConstants.Reflection.OnDragBeginMethod} method not found");
            method.Invoke(bannerInstance, new object[] { bannerAd, x, y });
        }

        /// <summary>
        /// Triggers the OnDrag event handler on the banner instance.
        /// </summary>
        /// <param name="bannerInstance">The banner instance (BannerVisualElement or UnityBannerAd)</param>
        /// <param name="bannerAd">The IBannerAd to pass to the event handler</param>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="methodNotFoundMessage">Error message if method not found (optional)</param>
        public static void TriggerOnDrag(TBanner bannerInstance, IBannerAd bannerAd, float x, float y, string methodNotFoundMessage = null)
        {
            var method = typeof(TBanner).GetMethod(TestConstants.Reflection.OnDragMethod, PrivateInstanceFlags);
            Assert.IsNotNull(method, methodNotFoundMessage ?? $"{TestConstants.Reflection.OnDragMethod} method not found");
            method.Invoke(bannerInstance, new object[] { bannerAd, x, y });
        }

        /// <summary>
        /// Triggers the OnDragEnd event handler on the banner instance.
        /// </summary>
        /// <param name="bannerInstance">The banner instance (BannerVisualElement or UnityBannerAd)</param>
        /// <param name="bannerAd">The IBannerAd to pass to the event handler</param>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="methodNotFoundMessage">Error message if method not found (optional)</param>
        public static void TriggerOnDragEnd(TBanner bannerInstance, IBannerAd bannerAd, float x, float y, string methodNotFoundMessage = null)
        {
            var method = typeof(TBanner).GetMethod(TestConstants.Reflection.OnDragEndMethod, PrivateInstanceFlags);
            Assert.IsNotNull(method, methodNotFoundMessage ?? $"{TestConstants.Reflection.OnDragEndMethod} method not found");
            method.Invoke(bannerInstance, new object[] { bannerAd, x, y });
        }
    }
}
