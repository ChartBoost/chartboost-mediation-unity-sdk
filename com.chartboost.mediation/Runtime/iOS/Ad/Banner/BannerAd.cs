using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IOSBannerContainers = Chartboost.Banners.IOSBannerContainers;
using Chartboost.Caching;
using Chartboost.Constants;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Chartboost.Mediation.iOS.Ad.Banner
{
    /// <summary>
    /// iOS's implementation of <see cref="BannerAdBase"/>.
    /// </summary>
    internal partial class BannerAd : NativeBannerAdBase
    {
        /// <summary>
        /// Register callbacks to native observer.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterCallbacks()
        {
            if (Application.isEditor)
                return;
            
            _CBMBannerAdSetCallbacks(BannerAdEvent);
        }

        internal BannerAd() : base(_CBMGetBannerAd())
        {
            var container = _CBMBannerAdCreateContainer(UniqueId);
            if (container != IntPtr.Zero)
                Container = IOSBannerContainers.Wrap(container);
        }
        
        // The store frees the native wrapper on destroy, so calls after Dispose pass null (a logged no-op) instead of a freed pointer.
        private IntPtr NativeId => IsDisposed ? IntPtr.Zero : UniqueId;

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> Keywords
        {
            get
            {
                var keywordsJson = _CBMBannerAdGetKeywords(NativeId);
                return string.IsNullOrEmpty(keywordsJson) ? new Dictionary<string, string>() : keywordsJson.ToDictionary();
            }
            set => _CBMBannerAdSetKeywords(NativeId, value == null? null : JsonConvert.SerializeObject(value));
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> PartnerSettings 
        {
            get
            {
                var partnerSettingsJson = _CBMBannerAdGetPartnerSettings(NativeId);
                return string.IsNullOrEmpty(partnerSettingsJson) ? new Dictionary<string, string>() : partnerSettingsJson.ToDictionary();
            }
            set => _CBMBannerAdSetPartnerSettings(NativeId, value == null? null : JsonConvert.SerializeObject(value));
        }

        /// <inheritdoc />
        public override BannerAdLoadRequest Request
        {
            get
            {
                if (_request != null)
                    return _request;

                _request = _CBMBannerAdGetRequest(NativeId).ToBannerAdLoadRequest();
                return _request;
            }
        }
        
        /// <inheritdoc />
        public override BidInfo? WinningBidInfo => _CBMBannerAdGetBidInfo(NativeId).ToBidInfo();

        /// <inheritdoc />
        public override string LoadId => _CBMBannerAdGetLoadId(NativeId) ?? string.Empty;

        /// <inheritdoc />
        public override Metrics? LoadMetrics => _CBMBannerAdGetLoadMetrics(NativeId).ToMetrics();

        /// <inheritdoc />
        public override BannerSize? BannerSize => _CBMBannerAdGetBannerSize(NativeId).ToBannerSize();
        
        /// <inheritdoc />
        protected override void SetContentSize(ContainerSize size) => _CBMBannerAdSetContainerSize(NativeId, size.Width, size.Height);

        /// <inheritdoc />
        public override BannerHorizontalAlignment HorizontalAlignment 
        {
            get =>  (BannerHorizontalAlignment)_CBMBannerAdGetHorizontalAlignment(NativeId);
            set => _CBMBannerAdSetHorizontalAlignment(NativeId, (int)value);
        }
        
        /// <inheritdoc />
        public override BannerVerticalAlignment VerticalAlignment 
        {
            get =>  (BannerVerticalAlignment)_CBMBannerAdGetVerticalAlignment(NativeId);
            set => _CBMBannerAdSetVerticalAlignment(NativeId, (int)value);
        }
        
        /// <inheritdoc />
        public override async Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
        {
            await base.Load(request);
            _request = request;
            var (proxy, hashCode) = AwaitableProxies.SetupProxy<BannerAdLoadResult>();
            AdLoadRequestCache.TrackAdLoadRequest(hashCode, request);

            _CBMBannerAdLoadAd(NativeId, request.PlacementName, (int)request.Size.SizeType, request.Size.Width, request.Size.Height, hashCode, BannerAdLoadResultCallbackProxy);
            return await proxy;
        }

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            _CBMBannerAdReset(NativeId);
        }

        /// <inheritdoc />
        protected override void DestroyNative(bool disposing)
        {
            _CBMBannerAdDestroy(UniqueId);
            AdCache<IAd>.ReleaseAd(UniqueId);
        }

        /// <inheritdoc />
        internal override Vector2 AdRelativePosition
        {
            get => _CBMBannerAdGetAdRelativePosition(NativeId).ToVector2();
            set => _CBMBannerAdSetAdRelativePosition(NativeId, value.x, value.y);
        }
        
        /// <inheritdoc />
        internal override void SetContainerBackgroundColor(Color color)
        {
            base.SetContainerBackgroundColor(color);
            _CBMBannerAdSetContainerBackgroundColor(NativeId, color.r, color.g, color.b, color.a);
        }

        /// <inheritdoc />
        internal override void SetAdBackgroundColor(Color color)
        {
            base.SetAdBackgroundColor(color);
            _CBMBannerAdSetAdBackgroundColor(NativeId, color.r, color.g, color.b, color.a);
        }

        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetCallbacks(ExternBannerAdEvent bannerAdEvents);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern IntPtr _CBMGetBannerAd();
        [DllImport(SharedIOSConstants.DLLImport)] private static extern IntPtr _CBMBannerAdCreateContainer(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetKeywords(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetKeywords(IntPtr uniqueId, string keywordsJson);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetPartnerSettings(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetPartnerSettings(IntPtr uniqueId, string partnerSettingsJson);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetRequest(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetBidInfo(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetLoadId(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetLoadMetrics(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetBannerSize(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMBannerAdGetHorizontalAlignment(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetHorizontalAlignment(IntPtr uniqueId, int horizontalAlignment );
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMBannerAdGetVerticalAlignment(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetVerticalAlignment(IntPtr uniqueId, int verticalAlignment );
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdLoadAd(IntPtr uniqueId, string placementName, int sizeType, float width, float height, int hashCode, ExternBannerAdLoadResultEvent callback);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdReset(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdDestroy(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetContainerSize(IntPtr uniqueId, float width, float height);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetAdRelativePosition(IntPtr uniqueId, float x, float y);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetAdRelativePosition(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetContainerBackgroundColor(IntPtr uniqueId, float r, float g, float b, float a);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetAdBackgroundColor(IntPtr uniqueId, float r, float g, float b, float a);
    }
}
