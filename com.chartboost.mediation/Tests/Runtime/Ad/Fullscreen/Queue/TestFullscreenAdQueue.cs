using System;
using System.Collections.Generic;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Requests;

namespace Chartboost.Tests.Runtime.Ad.Fullscreen.Queue
{
    /// <summary>
    /// Shared test helper class for FullscreenAdQueueBase testing.
    /// Provides a concrete implementation of FullscreenAdQueueBase with controllable properties and test hooks.
    /// </summary>
    internal class TestFullscreenAdQueue : FullscreenAdQueueBase
    {
        /// <summary>
        /// Exposes the protected UniqueId for testing.
        /// </summary>
        public IntPtr UniqueIdPublic => UniqueId;

        /// <summary>
        /// Controls the value returned by NumberOfAdsReady.
        /// </summary>
        public int NumberOfAdsReadyValue { get; set; }

        /// <summary>
        /// Controls the value returned by IsRunning.
        /// </summary>
        public bool IsRunningValue { get; set; }

        /// <summary>
        /// Controls the ad returned by GetNextAd.
        /// </summary>
        public IFullscreenAd NextAd { get; set; }

        /// <summary>
        /// Controls the value returned by HasNextAd.
        /// </summary>
        public bool HasNextAdValue { get; set; }

        /// <summary>
        /// Tracks whether Start was called.
        /// </summary>
        public bool StartCalled { get; private set; }

        /// <summary>
        /// Tracks whether Stop was called.
        /// </summary>
        public bool StopCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidUpdate event was triggered.
        /// </summary>
        public bool DidUpdateCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidRemoveExpiredAd event was triggered.
        /// </summary>
        public bool DidRemoveExpiredAdCalled { get; private set; }

        /// <summary>
        /// Stores the IAdLoadResult passed to DidUpdate event.
        /// </summary>
        public IAdLoadResult UpdateLoadResult { get; private set; }

        /// <summary>
        /// Stores the numberOfAdsReady parameter from DidUpdate event.
        /// </summary>
        public int UpdateNumberOfAdsReady { get; private set; }

        /// <summary>
        /// Stores the numberOfAdsReady parameter from DidRemoveExpiredAd event.
        /// </summary>
        public int RemoveExpiredAdNumberOfAdsReady { get; private set; }

        public TestFullscreenAdQueue(IntPtr uniqueId) : base(uniqueId)
        {
            DidUpdate += (_, adLoadResult, numberOfAdsReady) =>
            {
                DidUpdateCalled = true;
                UpdateLoadResult = adLoadResult;
                UpdateNumberOfAdsReady = numberOfAdsReady;
            };
            DidRemoveExpiredAd += (_, numberOfAdsReady) =>
            {
                DidRemoveExpiredAdCalled = true;
                RemoveExpiredAdNumberOfAdsReady = numberOfAdsReady;
            };
        }

        public override Dictionary<string, string> Keywords { get; set; }

        public override int QueueCapacity { get; set; }

        public override int NumberOfAdsReady => NumberOfAdsReadyValue;

        public override bool IsRunning => IsRunningValue;

        public override IFullscreenAd GetNextAd()
        {
            base.GetNextAd();
            return NextAd;
        }

        public override bool HasNextAd()
        {
            base.HasNextAd();
            return HasNextAdValue;
        }

        public override void Start()
        {
            base.Start();
            StartCalled = true;
            IsRunningValue = true;
        }

        public override void Stop()
        {
            base.Stop();
            StopCalled = true;
            IsRunningValue = false;
        }

        // Expose internal methods for testing
        public void TestOnDidUpdate(IAdLoadResult adLoadResult, int numberOfAdsReady)
            => OnDidUpdate(adLoadResult, numberOfAdsReady);

        public void TestOnDidRemoveExpiredAd(int numberOfAdsReady)
            => OnDidRemoveExpiredAd(numberOfAdsReady);

    }
}
