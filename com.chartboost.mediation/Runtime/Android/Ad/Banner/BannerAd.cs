using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Android.Ad.Banner
{
    /// <summary>
    /// Android's implementation of <see cref="IBannerAd"/>.
    /// </summary>
    internal partial class BannerAd : BannerAdBase
    {
        private readonly AndroidJavaObject _nativeBannerAd;
        internal static readonly BannerAdListener BannerAdListenerInstance = new();
        internal BannerAd(AndroidJavaObject bannerAd) : base(new IntPtr(bannerAd.NativeHashCode())) => _nativeBannerAd = bannerAd;
        
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
        public override Vector2 Position
        {
            get => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetPosition).PointFToVector2();
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetPosition, value.x, value.y);
        }
        
        /// <inheritdoc />
        public override Vector2 Pivot
        {
            get => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetPivot).PointFToVector2();
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetPivot, value.x, value.y);
        }

        /// <inheritdoc />
        public override bool Visible
        {
            get => _nativeBannerAd.Call<bool>(AndroidConstants.FunctionGetVisibility);
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetVisibility, value);
        }
        
        /// <inheritdoc />
        public override bool Draggable
        {
            get => _nativeBannerAd.Call<bool>(AndroidConstants.FunctionGetDraggability);
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetDraggability, value);
        }

        /// <inheritdoc />
        public override BannerAdLoadRequest Request => _request;
        
        /// <inheritdoc />
        public override BidInfo WinningBidInfo
        {
            get
            {
                var nativeWinningBidInfo = _nativeBannerAd?.Call<AndroidJavaObject>(AndroidConstants.FunctionGetWinningBidInfo);
                return nativeWinningBidInfo?.MapToWinningBidInfo() ?? new BidInfo();
            }
        }
        
        /// <inheritdoc />
        public override string LoadId => _nativeBannerAd.Call<string>(AndroidConstants.FunctionGetLoadId);
        
        /// <inheritdoc />
        public override Metrics? LoadMetrics => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetMetrics)?.JsonObjectToMetrics();

        /// <inheritdoc />
        public override BannerSize? BannerSize 
            => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetBannerSize)?.ToBannerSize();

        /// <inheritdoc />
        public override ContainerSize ContainerSize
        {
            get => _nativeBannerAd.Call<AndroidJavaObject>(AndroidConstants.FunctionGetContainerSize).ToContainerSize();
            set => _nativeBannerAd.Call(AndroidConstants.FunctionSetContainerSize,value.Width, value.Height);
        }

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

            var loadResultAwaitableProxy = new BannerAdLoadListener();
            try
            {
                AdCache.TrackAdLoadRequest(loadResultAwaitableProxy.hashCode(), request);
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
        protected override void Dispose(bool disposing)
        {
            if(IsDisposed) 
                return;
            IsDisposed = true;

            // Release managed resources
            if (disposing)
                _nativeBannerAd?.Dispose();
            
            // Release unmanaged resources
            AndroidAdStore.ReleaseBannerAd(UniqueId);
        }
    }
}
