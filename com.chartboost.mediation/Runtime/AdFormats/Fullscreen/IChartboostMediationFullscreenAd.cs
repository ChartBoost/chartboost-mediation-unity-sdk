using System.Threading.Tasks;
using Chartboost.Requests;

namespace Chartboost.AdFormats.Fullscreen
{
    public interface IChartboostMediationFullscreenAd
    {
        public abstract ChartboostMediationFullscreenAdLoadRequest Request { get; }

        public abstract string CustomData { get; set; }
        
        public abstract string LoadId { get; }

        public abstract BidInfo WinningBidInfo { get; }

        public abstract Task<ChartboostMediationAdShowResult> Show();

        public abstract void Invalidate();
    }
}
