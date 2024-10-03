using System;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Utilities.Events;

namespace Chartboost.Mediation.iOS.Ad.Banner
{
    /// <summary>
    /// Extern delegate definition for iOS's <see cref="IBannerAd"/> loads completions.
    /// </summary>
    internal delegate void ExternBannerAdLoadResultEvent(int hashCode, IntPtr adHashCode, string loadId, string metricsJson, string winningBidJson, float sizeWidth, float sizeHeight, string code, string message);
    
    /// <summary>
    /// Extern definition for <see cref="BannerAdDragEvent"/>
    /// </summary>
    internal delegate void ExternBannerAdDragEvent(long adHasCode, int eventType, float x, float y);
    
    /// <summary>
    /// Extern delegate definition for iOS's <see cref="IBannerAd"/> <see cref="BannerAdEvents"/>.
    /// </summary>
    internal delegate void ExternBannerAdEvent(long adHashCode, int eventType);
}
