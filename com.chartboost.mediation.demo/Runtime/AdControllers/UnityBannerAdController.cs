using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Mediation.Demo.Pages;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public class UnityBannerAdController : SimpleAdController
    {
        private ChartboostMediationUnityBannerAd _unityBanner;

        public UnityBannerAdController(string placementIdentifier) : base(placementIdentifier) { }
        
        public override async void Load()
        {
            if (_unityBanner != null)
                Invalidate();

            _unityBanner = ChartboostMediation.GetUnityBannerAd(PlacementIdentifier, PageController.Instance.Root, ChartboostMediationBannerSize.Standard, ChartboostMediationBannerAdScreenLocation.BottomCenter);
            _unityBanner.DidRecordImpression += OnDidRecordImpressionBanner;
            _unityBanner.DidLoad += OnDidLoadBanner;
            _unityBanner.DidClick += OnDidClickBanner;
            _unityBanner.DidDrag += OnDidDragBanner;
            _unityBanner.Keywords = DefaultKeywords;

            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await _unityBanner.Load();
            LoadingOverlay.Instance.ToggleLoadingOverlay(false);
            
            if (adLoadResult.Error.HasValue)
            {
                Debug.LogError($"Ad Failed to Load with Error: {adLoadResult.Error.Value.Message}");
                return;
            }

            Debug.Log("Banner Loaded!");
        }

        public override void Show()
        {
            // Do nothing, banners show automatically after load, this button will be hidden for banners.
        }

        public override void Invalidate()
        {
            if (_unityBanner != null && _unityBanner.gameObject != null)
                Object.Destroy(_unityBanner.gameObject);
        }

        #region UnityBanner Callbacks
        private void OnDidRecordImpressionBanner(ChartboostMediationUnityBannerAd unitybannerad)
        {
            Debug.Log("Unity Banner RecordImpression");
        }

        private void OnDidLoadBanner(ChartboostMediationUnityBannerAd unitybannerad)
        {
            Debug.Log("Unity Banner Loaded");
        }

        private void OnDidClickBanner(ChartboostMediationUnityBannerAd unitybannerad)
        {
            Debug.Log("Unity Banner Clicked");
        }

        private void OnDidDragBanner(ChartboostMediationUnityBannerAd unitybannerad, float x, float y)
        {
            Debug.Log("Unity Banner Drag");
        }
        #endregion
    }
}
