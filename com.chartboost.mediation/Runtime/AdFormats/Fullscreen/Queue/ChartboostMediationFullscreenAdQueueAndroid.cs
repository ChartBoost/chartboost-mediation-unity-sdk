#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using Chartboost.Events;
using Chartboost.Platforms.Android;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Fullscreen.Queue
{
    internal sealed class ChartboostMediationFullscreenAdQueueAndroid : ChartboostMediationFullscreenAdQueueBase
    {
        private readonly AndroidJavaObject _nativeQueue;
        private Dictionary<string,string> _keywords = new Dictionary<string, string>();

        private ChartboostMediationFullscreenAdQueueAndroid(AndroidJavaObject nativeQueue) : base(new IntPtr(nativeQueue.HashCode()))
        {
            LogTag = "ChartboostMediationFullscreenAdQueue (Android)";
            _nativeQueue = nativeQueue;
        }

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Keywords"/>
        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                try
                {
                    _nativeQueue.Call(AndroidConstants.FunSetKeywords, value.ToKeywords());
                    _keywords = value;
                }
                catch (Exception e)
                {
                    EventProcessor.ReportUnexpectedSystemError($"Error setting keywords => {e.Message}");
                }
            }
        }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.QueueCapacity"/>
        public override int QueueCapacity => _nativeQueue.Get<int>(AndroidConstants.PropertyQueueCapacity);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.NumberOfAdsReady"/>
        public override int NumberOfAdsReady => _nativeQueue.Call<int>(AndroidConstants.PropertyNumberOfAdsReady);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.IsRunning"/>
        public override bool IsRunning => _nativeQueue.Get<bool>(AndroidConstants.PropertyIsRunning);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.HasNextAd"/>
        public override bool HasNextAd()
            => _nativeQueue.Call<bool>(AndroidConstants.FunHasNextAd);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Start"/>
        public override void Start()
            => _nativeQueue.Call(AndroidConstants.FunStart);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Stop"/>
        public override void Stop()
            => _nativeQueue.Call(AndroidConstants.FunStop);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.SetCapacity"/>
        public override void SetCapacity(int capacity)
            => _nativeQueue.Set(AndroidConstants.PropertyQueueCapacity, capacity);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.GetNextAd"/>
        public override IChartboostMediationFullscreenAd GetNextAd()
        {
            var nativeAd = _nativeQueue.Call<AndroidJavaObject>(AndroidConstants.FunGetNextAd);

            if (nativeAd == null)
            {
                Logger.Log(LogTag, "GetNextAd() :: No more ads left in queue returning NULL");
                return null;
            }
            
            nativeAd.Call(AndroidConstants.FunChartboostMediationFullscreenAdListener, ChartboostMediationAndroid.ChartboostMediationFullscreenAdListener.Instance);
            var nativeRequest = nativeAd.Get<AndroidJavaObject>(AndroidConstants.PropertyRequest);
            var placement = nativeRequest.Get<string>(AndroidConstants.PropertyPlacementName);
            var loadRequest = new ChartboostMediationFullscreenAdLoadRequest(placement, Keywords);  
            
            return new ChartboostMediationFullscreenAdAndroid(nativeAd, loadRequest);
        }

        internal static ChartboostMediationFullscreenAdQueueAndroid Queue(string placementName)
        {
            // Queues are a "singleton per placement", meaning that if a publisher attempts to
            // create multiple queues with the same placement ID the same object will be returned each time.
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            var nativeQueue = unityBridge.CallStatic<AndroidJavaObject>(AndroidConstants.FunGetFullscreenAdQueue, placementName, ChartboostMediationAndroid.ChartboostMediationFullscreenAdQueueListener.Instance);

            var queue = (ChartboostMediationFullscreenAdQueueAndroid)CacheManager.GetFullscreenAdQueue(nativeQueue.HashCode());
            if (queue != null)
                return queue;

            queue = new ChartboostMediationFullscreenAdQueueAndroid(nativeQueue);
            return queue;
        }
    }
}
#endif
