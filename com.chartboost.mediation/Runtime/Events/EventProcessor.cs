using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.Utilities;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Chartboost.Events
{
    public static class EventProcessor
    {
        internal enum FullscreenAdEvents
        {
            RecordImpression = 0,
            Click = 1,
            Reward = 2,
            Close = 3,
            Expire = 4
        }

        internal enum BannerAdEvents
        {
            Load = 0,
            Click = 1,
            RecordImpression = 2,
            Drag = 3
        }

        private static SynchronizationContext _context;
        private static TaskScheduler _unityScheduler;
        
        /// <summary>
        /// Called when an unexpected system error occurred.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static event ChartboostMediationEvent UnexpectedSystemErrorDidOccur;

        /// <summary>
        /// Initializes Chartboost Mediation Event Processor, must be called from main thread.
        /// </summary>
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            _context ??= SynchronizationContext.Current;
            _unityScheduler ??= TaskScheduler.FromCurrentSynchronizationContext();
        }
        
        /// <summary>
        /// Creates a continuation that executes asynchronously, on the Unity main thread, when the target <see cref="Task{T}"/> completes.
        /// </summary>
        /// <param name="task">Target <see cref="Task"/>.</param>
        /// <param name="continuation">An action to run when the <see cref="Task"/> completes. </param>
        /// <typeparam name="T">The type of the result produced by the <see cref="Task"/>.</typeparam>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        public static Task ContinueWithOnMainThread<T>(this Task<T> task, Action<Task<T>> continuation)
        {
            var ret = Task.Factory.StartNew(async () =>
            {
                await task;
                continuation.Invoke(task);
            }, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            
            ret.AppendExceptionLogging();
            return ret;
        }

        /// <summary>
        /// Creates a continuation that executes asynchronously, on the Unity main thread, when the target <see cref="Task"/> completes.
        /// </summary>
        /// <param name="task">Target <see cref="Task"/>.</param>
        /// <param name="continuation">An action to run when the <see cref="Task"/> completes. </param>
        /// <typeparam name="T">The type of the result produced by the <see cref="Task"/>.</typeparam>
        /// <returns>A new continuation <see cref="Task"/>.</returns>
        public static Task ContinueWithOnMainThread(this Task task, Action<Task> continuation)
        {
            var ret = Task.Factory.StartNew(async () =>
            {
                await task;
                continuation.Invoke(task);
            }, CancellationToken.None, TaskCreationOptions.None, _unityScheduler).Unwrap();
            ret.AppendExceptionLogging();
            return ret;
        }
        
        private static void AppendExceptionLogging(this Task inputTask) 
            => inputTask.ContinueWith(faultedTask => Debug.LogException(faultedTask.Exception), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

        public static void ProcessEventWithILRD(string dataString, ChartboostMediationILRDEvent ilrdEvent)
        {
            if (ilrdEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    if (!(JsonTools.Deserialize(dataString) is Dictionary<object, object> data))
                        return;

                    data.TryGetValue("placementName", out var placementName);
                    ilrdEvent(placementName as string, new Hashtable(data));
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessEventWithPartnerInitializationData(string dataString, ChartboostMediationPartnerInitializationEvent partnerInitializationEvent)
        {
            if (partnerInitializationEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    partnerInitializationEvent(dataString);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessChartboostMediationEvent(string error, ChartboostMediationEvent chartboostMediationEvent)
        {
            if (chartboostMediationEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    chartboostMediationEvent(error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }
        
        public static void ProcessChartboostMediationPlacementEvent(string placementName, string error, ChartboostMediationPlacementEvent placementEvent)
        {
            if (placementEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    placementEvent(placementName, error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessChartboostMediationLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error, ChartboostMediationPlacementLoadEvent bidEvent)
        {
            if (bidEvent == null)
                return;
            
            _context.Post(o =>
            {
                try
                {
                    var bidInfo = new BidInfo(auctionId, partnerId, price, lineItemName, lineItemId);
                    bidEvent(placementName, loadId, bidInfo, error);
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }

        public static void ProcessChartboostMediationBannerEvent(long adHashCode, int eventType, float x = default, float y = default)
        {
            _context.Post(o =>
            {
                try
                {
                    var ad = (ChartboostMediationBannerViewBase)CacheManager.GetBannerAd(adHashCode);
                    
                    // Ad event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                    if (ad == null)
                        return;
                    
                    switch ((BannerAdEvents)eventType)
                    {
                        case BannerAdEvents.Load : 
                            ad.OnBannerDidLoad(ad);
                            break;
                        case BannerAdEvents.Click :
                            ad.OnBannerClick(ad);
                            break;
                        case BannerAdEvents.RecordImpression:
                            ad.OnBannerRecordImpression(ad);
                            break;
                        case BannerAdEvents.Drag:
                            ad.OnBannerDrag(ad, x, y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
                    }
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(e.ToString());
                }
            }, null);
        }
        
        public static void ProcessEvent(Action customEvent)
        {
            _context.Post(o =>
            {
                customEvent?.Invoke();
            }, null);   
        }
        
        public static void ProcessFullscreenEvent(long adHashCode, int eventType, string code, string message)
        {
            _context.Post(o =>
            {
                var ad = CacheManager.GetFullscreenAd(adHashCode);

                // Ad event was fired but no reference for it exists. Developer did not set strong reference to it so it was gc.
                if (ad == null)
                    return;

                var type = (FullscreenAdEvents)eventType;
                
                ChartboostMediationError? error = null;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                    error = new ChartboostMediationError(code, message);
                
                switch (type)
                {
                    case FullscreenAdEvents.RecordImpression:
                        ad.Request?.OnRecordImpression(ad);
                        break;
                    case FullscreenAdEvents.Click:
                        ad.Request?.OnClick(ad);
                        break;
                    case FullscreenAdEvents.Reward:
                        ad.Request?.OnReward(ad);
                        break;
                    case FullscreenAdEvents.Expire:
                        ad.Request?.OnExpire(ad);
                        CacheManager.ReleaseFullscreenAd(adHashCode);
                        break;
                    case FullscreenAdEvents.Close:
                        ad.Request?.OnClose(ad, error);
                        CacheManager.ReleaseFullscreenAd(adHashCode);
                        break;
                    default:
                        return;
                }
            }, null);
        }

        internal static void ReportUnexpectedSystemError(string message) 
            => _context.Post(o => UnexpectedSystemErrorDidOccur?.Invoke(message), null);
    }
}
