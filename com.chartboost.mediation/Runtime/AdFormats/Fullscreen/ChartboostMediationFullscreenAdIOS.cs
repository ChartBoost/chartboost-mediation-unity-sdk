#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Platforms.IOS;
using Chartboost.Requests;
using Chartboost.Utilities;

namespace Chartboost.AdFormats.Fullscreen
{
    /// <summary>
    /// IOS implementation of IChartboostMediationFullscreenAd
    /// </summary>
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

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Request"/>
        public ChartboostMediationFullscreenAdLoadRequest Request { get; }
        
        /// <inheritdoc cref="IChartboostMediationFullscreenAd.CustomData"/>
        public string CustomData
        {
            get => _customData;
            set
            {
                _customData = value;
                _chartboostMediationFullscreenSetCustomData(_uniqueId, _customData);
            }
        }
        
        /// <inheritdoc cref="IChartboostMediationFullscreenAd.LoadId"/>
        public string LoadId { get; }
        
        /// <inheritdoc cref="IChartboostMediationFullscreenAd.WinningBidInfo"/>
        public BidInfo WinningBidInfo { get; }
        
        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Show"/>
        public async Task<ChartboostMediationAdShowResult> Show()
        {
            var (proxy, hashCode) = ChartboostMediationIOS._setupProxy<ChartboostMediationAdShowResult>();
            _chartboostMediationShowFullscreenAd(_uniqueId, hashCode, ChartboostMediationIOS.FullscreenAdShowResultCallbackProxy);
            return await proxy;
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Invalidate"/>
        public void Invalidate()
        {
            if (!_isValid)
                return;

            _isValid = false;
            _chartboostMediationInvalidateFullscreenAd(_uniqueId);
        }

        ~ChartboostMediationFullscreenAdIOS() => Invalidate();

        [DllImport("__Internal")] private static extern void _chartboostMediationFullscreenSetCustomData(IntPtr uniqueId, string customData);
        [DllImport("__Internal")] private static extern void _chartboostMediationInvalidateFullscreenAd(IntPtr uniqueId);
        [DllImport("__Internal")] private static extern void _chartboostMediationShowFullscreenAd(IntPtr uniqueId, int hashCode, ChartboostMediationIOS.ExternChartboostMediationFullscreenAdShowResultEvent callback);
    }
}
#endif
