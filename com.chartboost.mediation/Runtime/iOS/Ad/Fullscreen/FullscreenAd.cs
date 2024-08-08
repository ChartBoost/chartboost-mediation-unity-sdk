using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Constants;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.iOS.Ad.Fullscreen
{
    /// <summary>
    /// iOS's implementation of <see cref="FullscreenAdBase"/>.
    /// </summary>
    internal partial class FullscreenAd : FullscreenAdBase
    {
        /// <summary>
        /// Register callbacks to native observer.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterCallbacks()
        {
            if (Application.isEditor)
                return;
            
            _CBMFullscreenAdSetCallbacks(FullscreenAdEvent);
        }

        internal FullscreenAd(IntPtr uniqueID, FullscreenAdLoadRequest request = null) : base(uniqueId: uniqueID) => _request = request;

        /// <inheritdoc cref="FullscreenAdBase.Request"/>
        public override FullscreenAdLoadRequest Request
        {
            get
            {
                if (_request != null)
                    return _request;

                _request = _CBMFullscreenAdGetRequest(UniqueId).ToFullscreenAdLoadRequest();
                return _request;
            }
        }
        
        /// <inheritdoc cref="FullscreenAdBase.CustomData"/>
        public override string CustomData
        {
            get => _CBMFullscreenAdGetCustomData(UniqueId);
            set => _CBMFullscreenAdSetCustomData(UniqueId, value);
        }
        
        /// <inheritdoc cref="FullscreenAdBase.LoadId"/>
        public override string LoadId
        {
            get
            {
                if (_loadId != null)
                    return _loadId;
                
                _loadId = _CBMFullscreenAdGetLoadId(UniqueId);
                return _loadId;
            }
        }

        /// <inheritdoc cref="FullscreenAdBase.WinningBidInfo"/>
        public override BidInfo WinningBidInfo
        {
            get
            {
                if (_bidInfo != null)
                    return _bidInfo.Value;
                
                _bidInfo = _CBMFullscreenAdGetBidInfo(UniqueId).ToBidInfo();
                return _bidInfo.Value;
            }
        }
        
        /// <inheritdoc cref="FullscreenAdBase.Show"/>
        public override async Task<AdShowResult> Show()
        {
            if (IsDisposed)
                return await Task.FromResult(InvalidAdShowResult);
            
            var (proxy, hashCode) = AwaitableProxies.SetupProxy<AdShowResult>();
            _CBMFullscreenAdShow(UniqueId, hashCode, FullscreenAdShowResultCallbackProxy);
            return await proxy;
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
            _CBMFullscreenAdInvalidate(UniqueId);
            AdCache.ReleaseAd(UniqueId);
        }

        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdSetCallbacks(ExternFullscreenAdEvent fullscreenAdEvents);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMFullscreenAdGetRequest(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMFullscreenAdGetLoadId(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMFullscreenAdGetCustomData(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdSetCustomData(IntPtr uniqueId, string customData);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern string _CBMFullscreenAdGetBidInfo(IntPtr uniqueId);
        [DllImport(SharedIOSConstants.DLLImport)] internal static extern void _CBMLoadFullscreenAd(string placementName, string keywords, int hashCode, ExternFullscreenAdLoadResultEvent callback);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdShow(IntPtr uniqueId, int hashCode, ExternFullscreenAdShowResultEvent callback);
        [DllImport(SharedIOSConstants.DLLImport)] private static extern void _CBMFullscreenAdInvalidate(IntPtr uniqueId);
    }
}
