using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Mediation.Demo.Pages;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public class UnityBannerAdController : SimpleAdController
    {
        private UnityBannerAd _unityBanner;

        public UnityBannerAdController(string placementIdentifier) : base(placementIdentifier) { }
        
        public override async void Load()
        {
            if (_unityBanner != null)
                Dispose();

            _unityBanner = ChartboostMediation.GetUnityBannerAd(PlacementIdentifier, PageController.Instance.Root);
            _unityBanner.DidRecordImpression += OnDidRecordImpressionBanner;
            _unityBanner.WillAppear += OnWillAppearBanner;
            _unityBanner.DidClick += OnDidClickBanner;
            _unityBanner.DidDrag += OnDidDragBanner;
            _unityBanner.Keywords = DefaultKeywords;

            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await _unityBanner.Load();
            LoadingOverlay.Instance.ToggleLoadingOverlay(false);
            
            if (adLoadResult.Error.HasValue)
            {
                Debug.LogError($"Ad Failed to Load with Code: {adLoadResult.Error.Value.Code} Error: {adLoadResult.Error.Value.Message}");
                return;
            }

            Debug.Log("Banner Loaded!");
        }

        public override void Show()
        {
            // Do nothing, banners show automatically after load, this button will be hidden for banners.
        }

        public override void Dispose()
        {
            if (_unityBanner != null)
                Object.Destroy(_unityBanner.gameObject);
        }

        #region UnityBanner Callbacks
        private void OnDidRecordImpressionBanner(UnityBannerAd unityBannerAd)
        {
            Debug.Log("Unity Banner RecordImpression");
        }

        private void OnWillAppearBanner(UnityBannerAd unityBannerAd)
        {
            Debug.Log("Unity Banner Loaded");
        }

        private void OnDidClickBanner(UnityBannerAd unityBannerAd)
        {
            Debug.Log("Unity Banner Clicked");
        }

        private void OnDidDragBanner(UnityBannerAd unityBannerAd, float x, float y)
        {
            Debug.Log("Unity Banner Drag");
        }
        #endregion
    }
}
