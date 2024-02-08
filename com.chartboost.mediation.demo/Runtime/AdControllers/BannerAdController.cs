using Chartboost.AdFormats.Banner;
using Chartboost.Banner;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public sealed class BannerAdController : SimpleAdController
    {
        public BannerAdController(string placementIdentifier) : base(placementIdentifier) { }

        private IChartboostMediationBannerView _banner;

        public override async void Load()
        {
            if (_banner != null)
                Invalidate();

            _banner = ChartboostMediation.GetBannerView();

            if (_banner == null)
            {
                Debug.LogError("Banner view failed to be obtained.");
                return;
            }

            _banner.DidLoad += OnDidLoad;
            _banner.DidRecordImpression += OnDidRecordImpression;
            _banner.DidClick += OnDidClick;
            _banner.DidDrag += OnDidDrag;
            _banner.Keywords = DefaultKeywords;

            var adLoadRequest = new ChartboostMediationBannerAdLoadRequest(PlacementIdentifier, ChartboostMediationBannerSize.Standard);
            
            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await _banner.Load(adLoadRequest, ChartboostMediationBannerAdScreenLocation.BottomCenter);
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
            _banner?.Destroy();
        }

        private void OnDidLoad(IChartboostMediationBannerView bannerview)
        {
            Debug.Log("Banner Reloaded!");
        }

        private void OnDidRecordImpression(IChartboostMediationBannerView bannerview)
        {
            Debug.Log("Banner Record Impression!");
        }

        private void OnDidClick(IChartboostMediationBannerView bannerview)
        {
            Debug.Log("Banner Clicked!");
        }

        private void OnDidDrag(IChartboostMediationBannerView bannerview, float x, float y)
        {
            Debug.Log("Banner Dragged!");
        }
    }
}
