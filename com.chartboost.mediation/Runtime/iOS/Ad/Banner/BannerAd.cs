using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Constants;
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
    internal partial class BannerAd : BannerAdBase
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

        internal BannerAd() : base(_CBMGetBannerAd(BannerAdDragEvent)) { }
        
        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> Keywords
        {
            get
            {
                var keywordsJson = _CBMBannerAdGetKeywords(UniqueId);
                return string.IsNullOrEmpty(keywordsJson) ? new Dictionary<string, string>() : keywordsJson.ToDictionary();
            }
            set => _CBMBannerAdSetKeywords(UniqueId, value == null? null : JsonConvert.SerializeObject(value));
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<string, string> PartnerSettings 
        {
            get
            {
                var partnerSettingsJson = _CBMBannerAdGetPartnerSettings(UniqueId);
                return string.IsNullOrEmpty(partnerSettingsJson) ? new Dictionary<string, string>() : partnerSettingsJson.ToDictionary();
            }
            set => _CBMBannerAdSetPartnerSettings(UniqueId, value == null? null : JsonConvert.SerializeObject(value));
        }

        /// <inheritdoc />
        public override BannerAdLoadRequest Request
        {
            get
            {
                if (_request != null)
                    return _request;

                _request = _CBMBannerAdGetRequest(UniqueId).ToBannerAdLoadRequest();
                return _request;
            }
        }
        
        /// <inheritdoc />
        public override BidInfo WinningBidInfo => _CBMBannerAdGetBidInfo(UniqueId).ToBidInfo();

        /// <inheritdoc />
        public override string LoadId =>
            // TODO: why metrics is a list ?
            LoadMetrics?.metrics != null ? LoadMetrics?.metrics.FirstOrDefault().loadId : string.Empty;

        /// <inheritdoc />
        public override Metrics? LoadMetrics => _CBMBannerAdGetLoadMetrics(UniqueId).ToMetrics();

        /// <inheritdoc />
        public override BannerSize? BannerSize => _CBMBannerAdGetBannerSize(UniqueId).ToBannerSize();
        
        /// <inheritdoc />
        public override ContainerSize ContainerSize
        {
            get => _CBMBannerAdGetContainerSize(UniqueId).ToContainerSize();
            set => _CBMBannerAdSetContainerSize(UniqueId, value.Width, value.Height);
        }
        
        /// <inheritdoc />
        public override Vector2 Position
        {
            get => _CBMBannerAdGetPosition(UniqueId).ToVector2();
            set => _CBMBannerAdSetPosition(UniqueId, value.x, value.y);
        }
        
        /// <inheritdoc />
        public override Vector2 Pivot
        {
            get => _CBMBannerAdGetPivot(UniqueId).ToVector2();
            set => _CBMBannerAdSetPivot(UniqueId, value.x, value.y);
        }
        
        /// <inheritdoc />
        public override bool Visible
        {
            get => _CBMBannerAdGetVisibility(UniqueId);
            set => _CBMBannerAdSetVisibility(UniqueId, value);
        }
        
        /// <inheritdoc />
        public override bool Draggable
        {
            get => _CBMBannerAdGetDraggability(UniqueId);
            set => _CBMBannerAdSetDraggability(UniqueId, value);
        }

        /// <inheritdoc />
        public override BannerHorizontalAlignment HorizontalAlignment 
        {
            get =>  (BannerHorizontalAlignment)_CBMBannerAdGetHorizontalAlignment(UniqueId);
            set => _CBMBannerAdSetHorizontalAlignment(UniqueId, (int)value);
        }
        
        /// <inheritdoc />
        public override BannerVerticalAlignment VerticalAlignment 
        {
            get =>  (BannerVerticalAlignment)_CBMBannerAdGetVerticalAlignment(UniqueId);
            set => _CBMBannerAdSetVerticalAlignment(UniqueId, (int)value);
        }
        
        /// <inheritdoc />
        public override async Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
        {
            await base.Load(request);
            _request = request;
            var (proxy, hashCode) = AwaitableProxies.SetupProxy<BannerAdLoadResult>();
            AdCache.TrackAdLoadRequest(hashCode, request);

            _CBMBannerAdLoadAd(UniqueId, request.PlacementName, (int)request.Size.SizeType, request.Size.Width, request.Size.Height, hashCode, BannerAdLoadResultCallbackProxy);
            return await proxy;
        }

        /// <inheritdoc />
        public override void Reset()
        {
            base.Reset();
            _CBMBannerAdReset(UniqueId);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if(IsDisposed) 
                return;
            IsDisposed = true;
            
            // Release managed resources
            if(disposing) 
            { 
                // no managed resources to release
            }
                
            // Release unmanaged resources
            _CBMBannerAdDestroy(UniqueId);
        }

        /// <inheritdoc />
        internal override Vector2 AdRelativePosition
        {
            get => _CBMBannerAdGetAdRelativePosition(UniqueId).ToVector2();
            set => _CBMBannerAdSetAdRelativePosition(UniqueId, value.x, value.y);
        }
        
        /// <inheritdoc />
        internal override void SetContainerBackgroundColor(Color color)
        {
            base.SetContainerBackgroundColor(color);
            _CBMBannerAdSetContainerBackgroundColor(UniqueId, color.r, color.g, color.b, color.a);
        }

        /// <inheritdoc />
        internal override void SetAdBackgroundColor(Color color)
        {
            base.SetAdBackgroundColor(color);
            _CBMBannerAdSetAdBackgroundColor(UniqueId, color.r, color.g, color.b, color.a);
        }

        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetCallbacks(ExternBannerAdEvent bannerAdEvents);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern IntPtr _CBMGetBannerAd(ExternBannerAdDragEvent dragListener);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetKeywords(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetKeywords(IntPtr uniqueId, string keywordsJson);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetPartnerSettings(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetPartnerSettings(IntPtr uniqueId, string partnerSettingsJson);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetPosition(IntPtr uniqueId, float x, float y);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetPosition(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetPivot(IntPtr uniqueId, float x, float y);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetPivot(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetRequest(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetBidInfo(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetLoadMetrics(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetBannerSize(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetContainerSize(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMBannerAdGetHorizontalAlignment(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetHorizontalAlignment(IntPtr uniqueId, int horizontalAlignment );
        [DllImport(SharedIOSConstants.DLLImport)] private static extern int _CBMBannerAdGetVerticalAlignment(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetVerticalAlignment(IntPtr uniqueId, int verticalAlignment );
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdLoadAd(IntPtr uniqueId, string placementName, int sizeType, float width, float height, int hashCode, ExternBannerAdLoadResultEvent callback);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern bool _CBMBannerAdGetDraggability(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetDraggability(IntPtr uniqueId, bool canDrag );
        [DllImport(SharedIOSConstants.DLLImport)] private static extern bool _CBMBannerAdGetVisibility(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetVisibility(IntPtr uniqueId, bool visible);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdReset(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdDestroy(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetContainerSize(IntPtr uniqueId, float width, float height);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetAdRelativePosition(IntPtr uniqueId, float x, float y);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMBannerAdGetAdRelativePosition(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetContainerBackgroundColor(IntPtr uniqueId, float r, float g, float b, float a);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMBannerAdSetAdBackgroundColor(IntPtr uniqueId, float r, float g, float b, float a);
    }
}
