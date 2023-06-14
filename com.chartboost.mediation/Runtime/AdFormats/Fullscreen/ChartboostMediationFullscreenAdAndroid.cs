#if UNITY_ANDROID
using System;
using System.Threading.Tasks;
using Chartboost.Events;
using Chartboost.Platforms.Android;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;

namespace Chartboost.AdFormats.Fullscreen
{
    /// <summary>
    /// Android implementation of IChartboostMediationFullscreenAd
    /// </summary>
    public sealed class ChartboostMediationFullscreenAdAndroid : IChartboostMediationFullscreenAd
    {
        private readonly int _hashCode;
        private bool _isValid = true;
        private readonly AndroidJavaObject _chartboostMediationFullscreenAd;
        
        public ChartboostMediationFullscreenAdAndroid(AndroidJavaObject fullscreenAd, ChartboostMediationFullscreenAdLoadRequest request)
        {
            _hashCode = fullscreenAd.HashCode();
            _chartboostMediationFullscreenAd = fullscreenAd;
            var bidInfoMap = _chartboostMediationFullscreenAd.Get<AndroidJavaObject>("winningBidInfo");
            if (bidInfoMap != null)
                WinningBidInfo = bidInfoMap.MapToWinningBidInfo();
            Request = request;
            LoadId = _chartboostMediationFullscreenAd.Get<string>("loadId");
            CacheManager.TrackFullscreenAd(_hashCode, this);
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Request"/>
        public ChartboostMediationFullscreenAdLoadRequest Request { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.CustomData"/>
        public string CustomData
        {
            get => _chartboostMediationFullscreenAd.Get<string>("customData");
            set => _chartboostMediationFullscreenAd.Set("customData", value);
        }
        
        /// <inheritdoc cref="IChartboostMediationFullscreenAd.LoadId"/>
        public string LoadId { get ;}

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.WinningBidInfo"/>
        public BidInfo WinningBidInfo { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Show"/>
        public async Task<ChartboostMediationAdShowResult> Show()
        {
            var adShowListenerAwaitableProxy = new ChartboostMediationAndroid.ChartboostMediationFullscreenAdShowListener();
            try
            {
                using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
                unityBridge.CallStatic("showFullscreenAd", _chartboostMediationFullscreenAd, adShowListenerAwaitableProxy);
            }
            catch (NullReferenceException exception)
            {
                EventProcessor.ReportUnexpectedSystemError(exception.ToString());
            }
            return await adShowListenerAwaitableProxy;
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Invalidate"/>
        public void Invalidate()
        {
            if (!_isValid)
                return;

            _isValid = false;
            _chartboostMediationFullscreenAd.Call("invalidate");
            _chartboostMediationFullscreenAd.Dispose();
            CacheManager.ReleaseFullscreenAd(_hashCode);
        }

        ~ChartboostMediationFullscreenAdAndroid() => Invalidate();
    }
}
#endif
