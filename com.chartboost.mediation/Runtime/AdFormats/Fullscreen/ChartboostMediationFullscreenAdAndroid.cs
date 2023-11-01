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
    internal sealed class ChartboostMediationFullscreenAdAndroid : ChartboostMediationFullscreenAdBase
    {
        private readonly AndroidJavaObject _chartboostMediationFullscreenAd;

        public ChartboostMediationFullscreenAdAndroid(AndroidJavaObject fullscreenAd, ChartboostMediationFullscreenAdLoadRequest request) : base(uniqueId: fullscreenAd.HashCode())
        {
            _chartboostMediationFullscreenAd = fullscreenAd;
            var bidInfoMap = _chartboostMediationFullscreenAd.Get<AndroidJavaObject>(AndroidConstants.PropertyWinningBidInfo);
            if (bidInfoMap != null)
                WinningBidInfo = bidInfoMap.MapToWinningBidInfo();
            Request = request;
            LoadId = _chartboostMediationFullscreenAd.Get<string>(AndroidConstants.PropertyLoadId);
            CacheManager.TrackFullscreenAd(uniqueId.ToInt64(), this);
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Request"/>
        public override ChartboostMediationFullscreenAdLoadRequest Request { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.CustomData"/>
        public override string CustomData
        {
            get => customData;
            set
            {
                if (!isValid) 
                    return;
                customData = value;
                _chartboostMediationFullscreenAd.Set(AndroidConstants.PropertyCustomData, value);
            }
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.LoadId"/>
        public override string LoadId { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.WinningBidInfo"/>
        public override BidInfo WinningBidInfo { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Show"/>
        public override async Task<ChartboostMediationAdShowResult> Show()
        {
            if (!isValid)
                return GetAdShowResultForInvalidAd();

            var adShowListenerAwaitableProxy = new ChartboostMediationAndroid.ChartboostMediationFullscreenAdShowListener();
            try
            {
                using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
                unityBridge.CallStatic(AndroidConstants.FunShowFullscreenAd, _chartboostMediationFullscreenAd, adShowListenerAwaitableProxy);
            }
            catch (NullReferenceException exception)
            {
                EventProcessor.ReportUnexpectedSystemError(exception.ToString());
            }
            return await adShowListenerAwaitableProxy;
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Invalidate"/>
        public override void Invalidate()
        {
            if (!isValid)
                return;

            isValid = false;
            _chartboostMediationFullscreenAd.Dispose();
            AndroidAdStore.ReleaseFullscreenAd(uniqueId.ToInt32());
            CacheManager.ReleaseFullscreenAd(uniqueId.ToInt64());
        }

        ~ChartboostMediationFullscreenAdAndroid() => Invalidate(true);
    }
}
#endif
