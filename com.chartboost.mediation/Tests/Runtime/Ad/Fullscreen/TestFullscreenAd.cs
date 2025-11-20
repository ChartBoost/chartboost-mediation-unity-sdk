using System;
using System.Threading.Tasks;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;

namespace Chartboost.Tests.Runtime.Ad.Fullscreen
{
    /// <summary>
    /// Shared test helper class for FullscreenAdBase testing.
    /// Provides a concrete implementation of FullscreenAdBase with controllable properties and test hooks.
    /// </summary>
    internal class TestFullscreenAd : FullscreenAdBase
    {
        /// <summary>
        /// Tracks whether Dispose(bool) was called.
        /// </summary>
        public bool DisposeCalled { get; private set; }

        /// <summary>
        /// Tracks the disposing parameter value passed to Dispose(bool).
        /// </summary>
        public bool DisposingValue { get; private set; }

        /// <summary>
        /// Exposes the protected IsDisposed property for testing.
        /// </summary>
        public bool IsDisposedPublic => IsDisposed;

        /// <summary>
        /// Controls the result returned by Show().
        /// </summary>
        public AdShowResult? ShowResult { get; set; }

        /// <summary>
        /// Tracks whether DidRecordImpression event was triggered.
        /// </summary>
        public bool RecordImpressionCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidClick event was triggered.
        /// </summary>
        public bool ClickCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidReward event was triggered.
        /// </summary>
        public bool RewardCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidExpire event was triggered.
        /// </summary>
        public bool ExpireCalled { get; private set; }

        /// <summary>
        /// Tracks whether DidClose event was triggered.
        /// </summary>
        public bool CloseCalled { get; private set; }

        /// <summary>
        /// Stores the error passed to DidClose event.
        /// </summary>
        public ChartboostMediationError? CloseError { get; private set; }

        public TestFullscreenAd(IntPtr uniqueId) : base(uniqueId)
        {
            DidRecordImpression += _ => RecordImpressionCalled = true;
            DidClick += _ => ClickCalled = true;
            DidReward += _ => RewardCalled = true;
            DidExpire += _ => ExpireCalled = true;
            DidClose += (_, error) =>
            {
                CloseCalled = true;
                CloseError = error;
            };
        }

        public TestFullscreenAd(long uniqueId) : base(uniqueId)
        {
            DidRecordImpression += _ => RecordImpressionCalled = true;
            DidClick += _ => ClickCalled = true;
            DidReward += _ => RewardCalled = true;
            DidExpire += _ => ExpireCalled = true;
            DidClose += (_, error) =>
            {
                CloseCalled = true;
                CloseError = error;
            };
        }

        public override FullscreenAdLoadRequest Request => _request;

        public override string CustomData { get; set; }

        public override string LoadId => _loadId;

        public override BidInfo WinningBidInfo => _bidInfo ?? new BidInfo();

        public override Task<AdShowResult> Show()
        {
            if (IsDisposed)
                return Task.FromResult(InvalidAdShowResult);

            return Task.FromResult(ShowResult ?? new AdShowResult(new ChartboostMediationError(Errors.ShowFailureUnknown)));
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCalled = true;
            DisposingValue = disposing;
            IsDisposed = true;
        }

        // Expose internal methods for testing
        public void TestOnClick() => OnClick();
        public void TestOnClose(ChartboostMediationError? error) => OnClose(error);
        public void TestOnRecordImpression() => OnRecordImpression();
        public void TestOnExpire() => OnExpire();
        public void TestOnReward() => OnReward();

        // Helper methods to set backing fields for testing
        public void SetRequest(FullscreenAdLoadRequest request) => _request = request;
        public void SetLoadId(string loadId) => _loadId = loadId;
        public void SetBidInfo(BidInfo bidInfo) => _bidInfo = bidInfo;

    }
}
