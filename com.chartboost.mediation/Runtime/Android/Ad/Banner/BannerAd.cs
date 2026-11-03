using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndroidBannerContainers = Chartboost.Banners.AndroidBannerContainers;
using Chartboost.Caching;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Android.Ad.Banner
{
    /// <summary>
    /// Android's implementation of <see cref="IBannerAd"/>.
    /// </summary>
    internal partial class BannerAd : NativeBannerAdBase
    {
        private readonly AndroidJavaObject _nativeBannerAd;
        internal static readonly BannerAdListener BannerAdListenerInstance = new();

        internal BannerAd(AndroidJavaObject bannerAd) : base(new IntPtr(bannerAd.NativeHashCode()))
        {
            _nativeBannerAd = bannerAd;
            // Wrap owns the container reference.
            Container = AndroidBannerContainers.Wrap(bannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionCreateContainer));
        }
        
        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> Keywords 
        {
            get
            {
                var keywordsAsMap = _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetKeywords).Call<AndroidJavaObject>(SharedAndroidConstants.FunctionGet);
                return keywordsAsMap.MapToDictionary();
            }
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetKeywords, value.ToKeywords());
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> PartnerSettings
        {
            get
            {
                var partnerSettingsAsMap = _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetPartnerSettings).Call<AndroidJavaObject>(SharedAndroidConstants.FunctionGet);
                return partnerSettingsAsMap.MapToDictionary();
            }
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetPartnerSettings, value.DictionaryToMap());
        }

        /// <inheritdoc />
        public override BannerAdLoadRequest Request => _request;
        
        /// <inheritdoc />
        public override BidInfo? WinningBidInfo
        {
            get
            {
                var nativeWinningBidInfo = _nativeBannerAd?.Call<AndroidJavaObject>(AndroidConstants.FunctionGetWinningBidInfo);
                return nativeWinningBidInfo?.MapToWinningBidInfo();
            }
        }
        
        /// <inheritdoc />
        public override string LoadId => _nativeBannerAd.Call<string>(AndroidConstants.FunctionGetLoadId);
        
        /// <inheritdoc />
        public override Metrics? LoadMetrics => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetMetrics)?.JsonObjectToMetrics();

        /// <inheritdoc />
        public override BannerSize? BannerSize
        {
            get
            {
                using var size = _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetBannerSize);
                return size?.ToBannerSize();
            }
        }

        /// <inheritdoc />
        protected override void SetContentSize(ContainerSize size)
            => _nativeBannerAd.Call(AndroidConstants.FunctionSetContainerSize, size.Width, size.Height);

        /// <inheritdoc />
        public override BannerHorizontalAlignment HorizontalAlignment
        {
            get => (BannerHorizontalAlignment)_nativeBannerAd.Call<int>(AndroidConstants.FunctionGetHorizontalAlignment);
            set => _nativeBannerAd?.Call(AndroidConstants.FunctionSetHorizontalAlignment, (int)value);
        }
        
        /// <inheritdoc />
        public override BannerVerticalAlignment VerticalAlignment
        {
            get => (BannerVerticalAlignment)_nativeBannerAd.Call<int>(AndroidConstants.FunctionGetVerticalAlignment);
            set => _nativeBannerAd?.Call(AndroidConstants.FunctionSetVerticalAlignment, (int)value);
        }

        /// <inheritdoc />
        public override async Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
        {
            await base.Load(request);
            _request = request;

            // The native wrapper throws on NaN, which kills the app.
            if (float.IsNaN(request.Size.Width) || float.IsNaN(request.Size.Height))
                return new BannerAdLoadResult(new ChartboostMediationError(Errors.InvalidBannerSize));

            var loadResultAwaitableProxy = new BannerAdLoadListener();
            try
            {
                AdLoadRequestCache.TrackAdLoadRequest(loadResultAwaitableProxy.hashCode(), request);
                _nativeBannerAd.Call(AndroidConstants.FunctionLoad, request.PlacementName,
                    (int)request.Size.SizeType, request.Size.Width, request.Size.Height, loadResultAwaitableProxy);
            }
            catch (Exception exception)
            {
                LogController.LogException(exception);
                throw;
            }
            
            return await loadResultAwaitableProxy;
        }

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            _nativeBannerAd.Call(AndroidConstants.FunctionReset);
        }

        /// <inheritdoc />
        protected override void DestroyNative(bool disposing)
        {
            // Release managed resources
            if (disposing)
                _nativeBannerAd?.Dispose();
            
            // Release unmanaged resources
            AndroidAdStore.ReleaseBannerAd(UniqueId);
            AdCache<IAd>.ReleaseAd(UniqueId);
        }

        /// <inheritdoc />
        internal override void SetContainerBackgroundColor(Color color)
        {
            base.SetContainerBackgroundColor(color);
            var colorArray = new[] { color.r, color.g, color.b, color.a };
            _nativeBannerAd.Call(AndroidConstants.FunctionSetContainerBackgroundColor, colorArray);
        }

        /// <inheritdoc />
        internal override void SetAdBackgroundColor(Color color)
        {
            base.SetAdBackgroundColor(color);
            var colorArray = new[] { color.r, color.g, color.b, color.a };
            _nativeBannerAd.Call(AndroidConstants.FunctionSetAdBackgroundColor, colorArray);
        }

        /// <inheritdoc />
        internal override Vector2 AdRelativePosition
        {
            get => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetAdRelativePosition).PointFToVector2();
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetAdRelativePosition, value.x, value.y);
        }
    }
}
