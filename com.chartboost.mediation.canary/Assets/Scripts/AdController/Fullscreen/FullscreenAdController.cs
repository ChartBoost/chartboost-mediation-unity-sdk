using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Chartboost;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;

namespace AdController.Fullscreen
{
    public class FullscreenAdController : CanaryAdController
    {
        /// The loaded ads.
        /// At most one if lifecycle type is "replacement", an unlimited number if lifecycle type is "queue".
        private List<IChartboostMediationFullscreenAd> _ads = new List<IChartboostMediationFullscreenAd>(4);

        /// <summary>
        /// Flag that indicates if the first ad in the _ads list has been shown or not.
        /// </summary>
        private bool DidShowFirstAdInList = false;
        
        private readonly Lazy<Environment> _environment = new Lazy<Environment>(() => Environment.Shared);
        private Environment Environment => _environment.Value;

        protected override void Awake()
        {
            base.Awake();

            var callbacks = new[] {
                CanaryConstants.Callbacks.DidLoad,
                CanaryConstants.Callbacks.DidShow,
                CanaryConstants.Callbacks.DidClose,
                CanaryConstants.Callbacks.DidClick,
                CanaryConstants.Callbacks.DidExpire,
                CanaryConstants.Callbacks.DidReceiveReward,
                CanaryConstants.Callbacks.DidRecordImpression,
                CanaryConstants.Callbacks.DidReceiveILRD
            };
            callbackPanel.AddCallbacks(callbacks);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearAllAds();
        }

        public override async void OnLoadButtonPushed()
        {
            base.OnLoadButtonPushed();

            var fullscreenKeywords = keywordsDataSource.Keywords.ToDictionary(keyword => 
                keyword.name, keyword => keyword.value);

            var loadRequest = new ChartboostMediationFullscreenAdLoadRequest(controllerConfiguration.placementName, fullscreenKeywords);
            Log($"Fullscreen load request created with placement : {loadRequest.PlacementName} & keywords count: {loadRequest.Keywords.Keys.Count}");

            loadRequest.DidClick += (fullscreenAd) 
                => DidClick(fullscreenAd.Request.PlacementName, null);
            
            loadRequest.DidClose += (fullscreenAd, error) 
                => DidClose(fullscreenAd.Request.PlacementName, !error.HasValue ? null : $"Code: {error?.Code}, Message: {error?.Message}");
        
            loadRequest.DidReward += fullscreenAd
                => DidReceiveReward(fullscreenAd.Request.PlacementName, null);
        
            loadRequest.DidRecordImpression += fullscreenAd 
                => DidRecordImpression(fullscreenAd.Request.PlacementName, null);
        
            loadRequest.DidExpire += fullscreenAd 
                => DidExpire(fullscreenAd.Request.PlacementName, null);

            var loadResult = await ChartboostMediation.LoadFullscreenAd(loadRequest);
            var loadDetails = new AdLoadDetails();
            
            // Failed to Load
            if (loadResult.Error.HasValue)
            {
                loadDetails.error = loadResult.Error;
                DidLoad(loadDetails);
                return;
            }

            // Loaded but AD is null?
            IChartboostMediationFullscreenAd ad = loadResult.Ad;
            if (ad == null)
            {
                loadDetails.error = new ChartboostMediationError("Fullscreen Ad is null but no error was found???");
                DidLoad(loadDetails);
                return;
            }

            if (controllerConfiguration.loadType == AdLoadType.Replace)
            {
                ClearAllAds();
            }
            else if (controllerConfiguration.loadType == AdLoadType.Queue && Environment.KeepFullscreenAdUntilShownThenLoad && _ads.Count > 0 && DidShowFirstAdInList)
            {
                RemoveFirstAd();
            }
            _ads.Add(ad);
            
            // DidLoad
            loadDetails.placementName = ad?.Request.PlacementName;
            loadDetails.loadId = ad?.LoadId;
            loadDetails.bidInfo = ad.WinningBidInfo;
            loadDetails.customData = ad.CustomData;
            loadDetails.metrics = loadResult.Metrics;
            DidLoad(loadDetails);
            Log(CacheManager.CacheInfo());
        }

        public override async void OnShowButtonPushed()
        {
            base.OnShowButtonPushed();
            
            if (_ads.Count == 0)
            {
                Log(HasNotBeenLoaded);
                return;
            }

            var ad = _ads[0];
            var adShowResult = await ad.Show();

            DidShow(ad.Request.PlacementName, adShowResult.Metrics, adShowResult.Error);
            DidShowFirstAdInList = true;

            if (Environment.Shared.AutoLoadOnShow && adShowResult.Error == null)
            {
                Log(AutoLoadingNextAd);
                OnLoadButtonPushed();
            }
        }

        public void OnInvalidateButtonPushed()
        {
            base.OnClearButtonPushed();
            if (_ads.Count == 0)
            {
                Log(HasNotBeenLoaded, null, LogType.Error);
                return;
            }

            var ad = _ads[0];
            ad.Invalidate();
            if (controllerConfiguration.loadType == AdLoadType.Queue)
            {
                RemoveFirstAd();
            }
            Log(HasBeenCleared);
            Log(CacheManager.CacheInfo());
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        private void ClearAllAds()
        {
            foreach (var ad in _ads)
            {
                ad.Invalidate();
            }
            _ads.Clear();
        }

        private void RemoveFirstAd()
        {
            DidShowFirstAdInList = false;
            if (_ads.Count > 1)
            {
                _ads.RemoveAt(0);
                Log("Removed ad from top of queue");
            }
        }

        protected override void DidClose(string placementName, string error)
        {
            base.DidClose(placementName, error);
            if (controllerConfiguration.loadType == AdLoadType.Queue && !Environment.KeepFullscreenAdUntilShownThenLoad)
            {
                RemoveFirstAd();
            }
        }
    }
}
