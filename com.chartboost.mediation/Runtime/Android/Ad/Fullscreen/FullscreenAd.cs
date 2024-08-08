using System;
using System.Threading.Tasks;
using Chartboost.Constants;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Android.Ad.Fullscreen
{
    /// <summary>
    /// Android's implementation of <see cref="FullscreenAdBase"/>.
    /// </summary>
    internal partial class FullscreenAd : FullscreenAdBase
    {
        private readonly AndroidJavaObject _nativeFullscreenAd;
        internal static readonly FullscreenAdListener FullscreenAdListenerInstance = new();
        internal FullscreenAd(AndroidJavaObject fullscreenAd, FullscreenAdLoadRequest request = null) : base(fullscreenAd.NativeHashCode())
        {
            _nativeFullscreenAd = fullscreenAd;
            _request = request;
        }

        /// <inheritdoc cref="FullscreenAdBase.Request"/>
        public override FullscreenAdLoadRequest Request
        {
            get
            {
                if (_request != null)
                    return _request;

                var nativeRequest = _nativeFullscreenAd?.Call<AndroidJavaObject>(AndroidConstants.FunctionGetRequest);

                if (nativeRequest == null)
                {
                    LogController.Log("Fullscreen native request not found, returning null.", LogLevel.Warning);
                    return null;
                }

                var placementName = nativeRequest.Call<string>(AndroidConstants.FunctionGetPlacementName);
                var keywordsAsMap = nativeRequest.Call<AndroidJavaObject>(AndroidConstants.FunctionGetKeywords).Call<AndroidJavaObject>(SharedAndroidConstants.FunctionGet);
                _request = new FullscreenAdLoadRequest(placementName, keywordsAsMap.MapToDictionary());
                return _request;
            }
        }

        /// <inheritdoc cref="FullscreenAdBase.CustomData"/>
        public override string CustomData 
        {
            get => _nativeFullscreenAd?.Call<string>(AndroidConstants.FunctionGetCustomData);
            set => _nativeFullscreenAd?.Call(AndroidConstants.FunctionSetCustomData, value);
        }
        
        /// <inheritdoc cref="FullscreenAdBase.LoadId"/>
        public override string LoadId
        {
            get
            {
                if (_loadId != null)
                    return _loadId;
                
                _loadId = _nativeFullscreenAd?.Call<string>(AndroidConstants.FunctionGetLoadId);
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
                
                var nativeWinningBidInfo = _nativeFullscreenAd?.Call<AndroidJavaObject>(AndroidConstants.FunctionGetWinningBidInfo);
                _bidInfo = nativeWinningBidInfo.MapToWinningBidInfo();
                return _bidInfo.Value;
            }
        }

        /// <inheritdoc cref="FullscreenAdBase.Show"/>
        public override async Task<AdShowResult> Show()
        {
            if (IsDisposed)
                return await Task.FromResult(InvalidAdShowResult);
            
            var adShowListenerAwaitableProxy = new FullscreenAdShowListener();
            try
            {
                using var unityBridge = AndroidConstants.GetUnityBridge();
                unityBridge.CallStatic(AndroidConstants.FunctionShowFullscreenAd, _nativeFullscreenAd, adShowListenerAwaitableProxy);
            }
            catch (NullReferenceException exception)
            {
                LogController.LogException(exception);
            }
            return await adShowListenerAwaitableProxy;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            // Dispose managed resources
            if (disposing)
                _nativeFullscreenAd?.Dispose();
            
            // Dispose unmanaged resources
            AndroidAdStore.ReleaseFullscreenAd(UniqueId);
            AdCache.ReleaseAd(UniqueId);
        }
    }
}
