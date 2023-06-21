using System;
using System.Threading.Tasks;
using Chartboost.Requests;

namespace Chartboost.AdFormats.Fullscreen
{
    public abstract class ChartboostMediationFullscreenAdBase : IChartboostMediationFullscreenAd
    {
        protected readonly IntPtr uniqueId;
        protected bool isValid = true;
        protected string customData;

        protected ChartboostMediationFullscreenAdBase(int uniqueId) => this.uniqueId = new IntPtr(uniqueId);
        protected ChartboostMediationFullscreenAdBase(IntPtr uniqueId) => this.uniqueId = uniqueId;

        public abstract ChartboostMediationFullscreenAdLoadRequest Request { get; }
        public abstract string CustomData { get; set; }
        public abstract string LoadId { get; }
        public abstract BidInfo WinningBidInfo { get; }
        public abstract Task<ChartboostMediationAdShowResult> Show();
        public abstract void Invalidate();

        protected static ChartboostMediationAdShowResult GetAdShowResultForInvalidAd()
        {
            var error = new ChartboostMediationError("Fullscreen Ad is not valid, reference should be disposed.");
            return new ChartboostMediationAdShowResult(error);
        }
    }
}
