using System.Threading.Tasks;

namespace Chartboost.Placements
{
    public interface IChartboostMediationFullscreenAd
    {
        public abstract ChartboostMediationFullscreenAdLoadRequest LoadRequest { get;  }
        
        public abstract string AuctionId { get; }

        public abstract string CustomData { get; set; }
        
        public abstract string RequestId { get; }

        public abstract BidInfo WinningBidInfo { get; }

        public abstract Task<ChartboostMediationAdShowResult> Show();

        public abstract void Invalidate();
    }
}
