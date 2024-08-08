using System;
using System.Collections.Generic;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Android.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Android.Ad.Fullscreen.Queue
{
    /// <summary>
    /// Android's implementation of <see cref="FullscreenAdQueueBase"/>.
    /// </summary>
    internal partial class FullscreenAdQueue : FullscreenAdQueueBase
    {
        private readonly AndroidJavaObject _nativeFullscreenAdQueue;
        internal static readonly FullscreenAdQueueListener FullscreenAdQueueListenerInstance = new();

        internal FullscreenAdQueue(AndroidJavaObject nativeQueue) : base(new IntPtr(nativeQueue.NativeHashCode())) => _nativeFullscreenAdQueue = nativeQueue;

        /// <inheritdoc cref="FullscreenAdQueueBase.Keywords"/>
        public override Dictionary<string, string> Keywords
        {
            get
            {
                var keywordsAsMap = _nativeFullscreenAdQueue.Call<AndroidJavaObject>(AndroidConstants.FunctionGetKeywords).Call<AndroidJavaObject>(SharedAndroidConstants.FunctionGet);
                return keywordsAsMap.MapToDictionary();
            }
            set => _nativeFullscreenAdQueue.Call(AndroidConstants.FunctionSetKeywords, value.ToKeywords());
        }
        
        /// <inheritdoc cref="FullscreenAdQueueBase.QueueCapacity"/>
        public override int QueueCapacity
        {
            get => _nativeFullscreenAdQueue.Get<int>(AndroidConstants.PropertyQueueCapacity);
            set => _nativeFullscreenAdQueue.Set(AndroidConstants.PropertyQueueCapacity, value);
        }
        
        /// <inheritdoc cref="FullscreenAdQueueBase.NumberOfAdsReady"/>
        public override int NumberOfAdsReady => _nativeFullscreenAdQueue.Call<int>(AndroidConstants.PropertyNumberOfAdsReady);
        
        /// <inheritdoc cref="FullscreenAdQueueBase.IsRunning"/>
        public override bool IsRunning => _nativeFullscreenAdQueue.Get<bool>(AndroidConstants.PropertyIsRunning);
        
        /// <inheritdoc cref="FullscreenAdQueueBase.GetNextAd"/>
        public override IFullscreenAd GetNextAd()
        {
            base.GetNextAd();

            try
            {
                var nativeAd = _nativeFullscreenAdQueue.Call<AndroidJavaObject>(AndroidConstants.FunctionGetNextAd);

                if (nativeAd == null)
                {
                    LogController.Log($"{FullscreenAdQueue}/GetNextAd no more ads left in queue returning null.", LogLevel.Debug);
                    return null;
                }
            
                nativeAd.Call(AndroidConstants.FunctionSetListener, FullscreenAd.FullscreenAdListenerInstance);
                return new FullscreenAd(nativeAd);
            }
            catch (Exception exception)
            {
               LogController.LogException(exception);
               return null;
            }
        }
        
        /// <inheritdoc cref="FullscreenAdQueueBase.HasNextAd"/>
        public override bool HasNextAd()
        {
            base.HasNextAd();
            return _nativeFullscreenAdQueue.Call<bool>(AndroidConstants.FunctionHasNextAd);
        }

        /// <inheritdoc cref="FullscreenAdQueueBase.Start"/>
        public override void Start()
        {
            base.Start();
            _nativeFullscreenAdQueue.Call(SharedAndroidConstants.FunctionStart);
        }

        /// <inheritdoc cref="FullscreenAdQueueBase.Stop"/>
        public override void Stop()
        {
            base.Stop();
            _nativeFullscreenAdQueue.Call(AndroidConstants.FunctionStop);
        }
    }
}
