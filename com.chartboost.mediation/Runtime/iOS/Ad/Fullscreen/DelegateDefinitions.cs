using System;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Utilities.Events;

namespace Chartboost.Mediation.iOS.Ad.Fullscreen
{
    /// <summary>
    /// Extern delegate definition for iOS's <see cref="IFullscreenAd"/> loads completions.
    /// </summary>
    internal delegate void ExternFullscreenAdLoadResultEvent(int hashCode, IntPtr adHashCode, string loadId, string bidInfoJson, string metricsJson, string code, string message);
    
    /// <summary>
    /// Extern delegate definition for iOS's <see cref="IFullscreenAd"/> show completions.
    /// </summary>
    internal delegate void ExternFullscreenAdShowResultEvent(int hashCode, string metricsJson, string code, string message);
    
    /// <summary>
    /// Extern delegate definition for iOS's <see cref="IFullscreenAd"/> <see cref="FullscreenAdEvents"/>.
    /// </summary>
    internal delegate void ExternFullscreenAdEvent(long adHashCode, int eventType, string code, string error);
}
