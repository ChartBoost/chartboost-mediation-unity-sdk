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
    internal sealed class ChartboostMediationFullscreenAdIOS : ChartboostMediationFullscreenAdBase
    {
        public ChartboostMediationFullscreenAdIOS(IntPtr uniqueID, string loadId, ChartboostMediationFullscreenAdLoadRequest request, BidInfo winningBid) : base(uniqueId: uniqueID)
        {
            LoadId = loadId;
            Request = request;
            WinningBidInfo = winningBid;
            CacheManager.TrackFullscreenAd(uniqueID.ToInt64(), this);
        }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Request"/>
        public override ChartboostMediationFullscreenAdLoadRequest Request { get; }

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
        public override string LoadId { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.WinningBidInfo"/>
        public override BidInfo WinningBidInfo { get; }

        /// <inheritdoc cref="IChartboostMediationFullscreenAd.Show"/>
        public override async Task<ChartboostMediationAdShowResult> Show()
        {
            if (!isValid)
                return GetAdShowResultForInvalidAd();

            var (proxy, hashCode) = ChartboostMediationIOS._setupProxy<ChartboostMediationAdShowResult>();
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
    }
}
#endif
