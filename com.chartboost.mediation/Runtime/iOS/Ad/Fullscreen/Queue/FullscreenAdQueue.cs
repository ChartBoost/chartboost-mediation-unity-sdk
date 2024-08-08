using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace Chartboost.Mediation.iOS.Ad.Fullscreen.Queue
{
    /// <summary>
    /// iOS's implementation of <see cref="FullscreenAdQueueBase"/>.
    /// </summary>
    internal partial class FullscreenAdQueue : FullscreenAdQueueBase
    {
        /// <summary>
        /// Register callbacks to native observer.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterCallbacks()
        {
            if (Application.isEditor)
                return;
            
            _CBMFullscreenAdQueueSetCallbacks(FullscreenAdQueueUpdateEvent, FullscreenAdQueueRemoveExpiredAdEvent);
        }

        internal FullscreenAdQueue(IntPtr uniqueId) : base(uniqueId) { }
        
        /// <inheritdoc cref="FullscreenAdQueueBase.Keywords"/>
        public override Dictionary<string, string> Keywords
        {
            get
            {
                var keywordsJson = _CBMFullscreenAdQueueGetKeywords(UniqueId);
                return string.IsNullOrEmpty(keywordsJson) ? new Dictionary<string, string>() : keywordsJson.ToDictionary();
            }
            set
            {
                if (value == null || value.Count == 0)
                    return;

                _CBMFullscreenAdQueueSetKeywords(UniqueId, JsonConvert.SerializeObject(value));
            }
        }
        
        /// <inheritdoc cref="FullscreenAdQueueBase.QueueCapacity"/>
        public override int QueueCapacity
        {
            get => _CBMFullscreenAdQueueGetQueueCapacity(UniqueId);
            set => _CBMFullscreenAdQueueSetCapacity(UniqueId, value);
        }

        /// <inheritdoc cref="FullscreenAdQueueBase.NumberOfAdsReady"/>
        public override int NumberOfAdsReady => _CBMFullscreenAdQueueGetNumberOfAdsReady(UniqueId);

        /// <inheritdoc cref="FullscreenAdQueueBase.IsRunning"/>
        public override bool IsRunning => _CBMFullscreenAdQueueIsRunning(UniqueId);
        
        /// <inheritdoc cref="FullscreenAdQueueBase.GetNextAd"/>
        public override IFullscreenAd GetNextAd()
        {
            base.GetNextAd();
            try
            {
                return new FullscreenAd(_CBMFullscreenAdQueueGetNextAd(UniqueId));
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
            return _CBMFullscreenAdQueueHasNextAd(UniqueId);
        }

        /// <inheritdoc cref="FullscreenAdQueueBase.Start"/>
        public override void Start()
        {
            base.Start();
            _CBMFullscreenAdQueueStart(UniqueId);
        }

        /// <inheritdoc cref="FullscreenAdQueueBase.Stop"/>
        public override void Stop()
        {
            base.Stop();
            _CBMFullscreenAdQueueStop(UniqueId);
        }

        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdQueueSetCallbacks(ExternFullscreenAdQueueUpdateEvent updateEvent, ExternFullscreenAdQueueRemoveExpiredAdEvent removeExpiredAdEvent);
        [DllImport(SharedIOSConstants.DLLImport)] internal static extern IntPtr _CBMFullscreenAdQueueGetQueue(string placementName);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMFullscreenAdQueueGetKeywords(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdQueueSetKeywords(IntPtr uniqueId, string keywordsJson);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMFullscreenAdQueueGetQueueCapacity(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMFullscreenAdQueueGetNumberOfAdsReady(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern bool _CBMFullscreenAdQueueIsRunning(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern IntPtr _CBMFullscreenAdQueueGetNextAd(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern bool _CBMFullscreenAdQueueHasNextAd(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdQueueStart(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdQueueStop(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdQueueSetCapacity(IntPtr uniqueId, int capacity);
    }
}
