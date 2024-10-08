using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public sealed class BannerAdController : SimpleAdController
    {
        public BannerAdController(string placementIdentifier) : base(placementIdentifier) { }

        private IBannerAd _banner;

        public override async void Load()
        {
            if (_banner != null)
                Dispose();

            _banner = ChartboostMediation.GetBannerAd();

            if (_banner == null)
            {
                Debug.LogError("Banner view failed to be obtained.");
                return;
            }

            _banner.WillAppear += OnWillAppear;
            _banner.DidRecordImpression += OnDidRecordImpression;
            _banner.DidClick += OnDidClick;
            _banner.DidBeginDrag += OnDidBeginDrag;
            _banner.DidDrag += OnDidDrag;
            _banner.DidEndDrag += OnDidEndDrag;
            _banner.Keywords = DefaultKeywords;

            // Bottom-center
            _banner.Position = new Vector2( DensityConverters.PixelsToNative(Screen.width / 2f), DensityConverters.PixelsToNative(Screen.height));
            _banner.Pivot = new Vector2(0.5f, 1);

            var adLoadRequest = new BannerAdLoadRequest(PlacementIdentifier, BannerSize.Standard);
            
            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await _banner.Load(adLoadRequest);
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
            _banner?.Dispose();
        }

        private void OnWillAppear(IBannerAd bannerAd)
        {
            Debug.Log("Banner Reloaded!");
        }

        private void OnDidRecordImpression(IBannerAd bannerAd)
        {
            Debug.Log("Banner Record Impression!");
        }

        private void OnDidClick(IBannerAd bannerAd)
        {
            Debug.Log("Banner Clicked!");
        }

        private void OnDidEndDrag(IBannerAd bannerAd, float x, float y)
        {
            Debug.Log("Banner Drag Begin!");
        }

        private void OnDidDrag(IBannerAd bannerAd, float x, float y)
        {
            Debug.Log("Banner Dragged!");
        }

        private void OnDidBeginDrag(IBannerAd bannerAd, float x, float y)
        {
            Debug.Log("Banner Drag End!");
        }
    }
}
