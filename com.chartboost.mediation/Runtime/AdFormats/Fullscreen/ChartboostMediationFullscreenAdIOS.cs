#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Platforms.IOS;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;

namespace Chartboost.AdFormats.Fullscreen
{
    /// <summary>
    /// IOS implementation of IChartboostMediationFullscreenAd
    /// </summary>
    internal sealed class ChartboostMediationFullscreenAdIOS : ChartboostMediationFullscreenAdBase
    {
        private ChartboostMediationFullscreenAdLoadRequest _request;
        private string _loadId;
        private BidInfo? _winningBidInfo;

        public ChartboostMediationFullscreenAdIOS(IntPtr uniqueID) : base(uniqueID)
        {
            CacheManager.TrackFullscreenAd(uniqueID.ToInt64(), this);
        }
        
        public ChartboostMediationFullscreenAdIOS(IntPtr uniqueID, ChartboostMediationFullscreenAdLoadRequest request) : base(uniqueId: uniqueID)
        {
            _request = request;
            CacheManager.TrackFullscreenAd(uniqueID.ToInt64(), this);
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Request"/>
        public override ChartboostMediationFullscreenAdLoadRequest Request
        {
            get
            {
                if (_request != null)
                    return _request;

                var requestJson = _chartboostMediationFullscreenAdRequest(uniqueId);
                _request = JsonConvert.DeserializeObject<ChartboostMediationFullscreenAdLoadRequest>(requestJson);
                return _request;
            }
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.CustomData"/>
        public override string CustomData
        {
            get => customData;
            set
            {
                customData = value;
                if (isValid)
                    _chartboostMediationFullscreenSetCustomData(uniqueId, value);
            }
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.LoadId"/>
        public override string LoadId => _chartboostMediationFullscreenAdLoadId(uniqueId);

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.WinningBidInfo"/>
        public override BidInfo WinningBidInfo
        {
            get
            {
                if (_winningBidInfo != null)
                    return _winningBidInfo.Value;
                
                var winningBidInfoJson = _chartboostMediationFullscreenAdWinningBidInfo(uniqueId);
                _winningBidInfo = JsonConvert.DeserializeObject<BidInfo>(winningBidInfoJson);
                return _winningBidInfo.Value;
            }
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Show"/>
        public override async Task<ChartboostMediationAdShowResult> Show()
        {
            if (!isValid)
                return GetAdShowResultForInvalidAd();

            var (proxy, hashCode) = AwaitableProxies.SetupProxy<ChartboostMediationAdShowResult>();
            _chartboostMediationShowFullscreenAd(uniqueId, hashCode, ChartboostMediationIOS.FullscreenAdShowResultCallbackProxy);
            return await proxy;
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Invalidate"/>
        public override void Invalidate()
        {
            if (!isValid)
                return;

            isValid = false;
            _chartboostMediationInvalidateFullscreenAd(uniqueId);
            CacheManager.ReleaseFullscreenAd(uniqueId.ToInt64());
        }

        ~ChartboostMediationFullscreenAdIOS() => Invalidate(true);

        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationFullscreenSetCustomData(IntPtr uniqueId, string customData);
        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationInvalidateFullscreenAd(IntPtr uniqueId);
        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationShowFullscreenAd(IntPtr uniqueId, int hashCode, ChartboostMediationIOS.ExternChartboostMediationFullscreenAdShowResultEvent callback);
        [DllImport(IOSConstants.Internal)] private static extern string _chartboostMediationFullscreenAdLoadId(IntPtr uniqueId);
        [DllImport(IOSConstants.Internal)] private static extern string _chartboostMediationFullscreenAdWinningBidInfo(IntPtr uniqueId);
        [DllImport(IOSConstants.Internal)] private static extern string _chartboostMediationFullscreenAdRequest(IntPtr uniqueId);
    }
}
#endif
