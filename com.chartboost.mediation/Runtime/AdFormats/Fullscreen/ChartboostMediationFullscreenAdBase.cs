using System;
using System.Threading.Tasks;
using Chartboost.Events;
using Chartboost.Requests;

namespace Chartboost.AdFormats.Fullscreen
{
    internal abstract class ChartboostMediationFullscreenAdBase : IChartboostMediationFullscreenAd
    {
        protected readonly IntPtr uniqueId;
        protected bool isValid = true;
        protected string customData;
        private const string InvalidAdError = "Fullscreen Ad is not valid, reference should be disposed.";

        protected ChartboostMediationFullscreenAdBase(long uniqueId) => this.uniqueId = new IntPtr(uniqueId);
        protected ChartboostMediationFullscreenAdBase(IntPtr uniqueId) => this.uniqueId = uniqueId;

        public abstract ChartboostMediationFullscreenAdLoadRequest Request { get; }
        public abstract string CustomData { get; set; }
        public abstract string LoadId { get; }
        public abstract BidInfo WinningBidInfo { get; }
        public abstract Task<ChartboostMediationAdShowResult> Show();
        public abstract void Invalidate();
        public event ChartboostMediationFullscreenAdEvent DidClick;
        public event ChartboostMediationFullscreenAdEventWithError DidClose;
        public event ChartboostMediationFullscreenAdEvent DidExpire;
        public event ChartboostMediationFullscreenAdEvent DidRecordImpression;
        public event ChartboostMediationFullscreenAdEvent DidReward;

        protected void Invalidate(bool isCollected)
        {
            // only report error if the ad is being disposed by gc and it has not been manually invalidated.
            if (isCollected && isValid)
                EventProcessor.ReportUnexpectedSystemError($"Fullscreen Ad with placement: {Request.PlacementName} and LoadId: {LoadId}, got GC. Make sure to properly dispose of ads utilizing Invalidate for the best integration experience.");
            Invalidate();
        }

        protected static ChartboostMediationAdShowResult GetAdShowResultForInvalidAd()
        {
            var error = new ChartboostMediationError(InvalidAdError);
            return new ChartboostMediationAdShowResult(error);
        }
        
        internal void OnClick(IChartboostMediationFullscreenAd ad) => DidClick?.Invoke(ad);

        internal void OnClose(IChartboostMediationFullscreenAd ad, ChartboostMediationError? error) => DidClose?.Invoke(ad, error);

        internal void OnRecordImpression(IChartboostMediationFullscreenAd ad) => DidRecordImpression?.Invoke(ad);

        internal void OnExpire(IChartboostMediationFullscreenAd ad) => DidExpire?.Invoke(ad);

        internal void OnReward(IChartboostMediationFullscreenAd ad) => DidReward?.Invoke(ad);
    }
}
