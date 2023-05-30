#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Platforms.IOS;

namespace Chartboost.Placements
{
    public class ChartboostMediationFullscreenAdIOS : IChartboostMediationFullscreenAd
    {
        private readonly IntPtr _uniqueId;
        private string _customData;
        private bool _isValid = true;
        
        public ChartboostMediationFullscreenAdIOS(IntPtr uniqueID, string loadId, ChartboostMediationFullscreenAdLoadRequest request, BidInfo winningBid)
        {
            _uniqueId = uniqueID;
            LoadId = loadId;
            Request = request;
            WinningBidInfo = winningBid;
            CacheManager.TrackFullscreenAd(uniqueID.ToInt32(), this);
        }

        public ChartboostMediationFullscreenAdLoadRequest Request { get; }
        public string CustomData
        {
            get => _customData;
            set
            {
                _customData = value;
                _chartboostMediationFullscreenSetCustomData(_uniqueId, _customData);
            }
        }
        public string LoadId { get; }
        public BidInfo WinningBidInfo { get; }
        public async Task<ChartboostMediationAdShowResult> Show()
        {
            var (proxy, hashCode) = ChartboostMediationIOS._setupProxy<ChartboostMediationAdShowResult>();
            _chartboostMediationShowFullscreenAd(_uniqueId, hashCode, ChartboostMediationIOS.FullscreenAdShowResultCallbackProxy);
            return await proxy;
        }

        public void Invalidate()
        {
            if (!_isValid)
                return;

            _isValid = false;
            _chartboostMediationInvalidateFullscreenAd(_uniqueId);
        }

        ~ChartboostMediationFullscreenAdIOS()
        {
            Invalidate();
        }

        [DllImport("__Internal")] private static extern void _chartboostMediationFullscreenSetCustomData(IntPtr uniqueId, string customData);
        [DllImport("__Internal")] private static extern void _chartboostMediationInvalidateFullscreenAd(IntPtr uniqueId);
        [DllImport("__Internal")] private static extern void _chartboostMediationShowFullscreenAd(IntPtr uniqueId, int hashCode, ChartboostMediationIOS.ExternChartboostMediationFullscreenAdShowResultEvent callback);
    }
}
#endif
