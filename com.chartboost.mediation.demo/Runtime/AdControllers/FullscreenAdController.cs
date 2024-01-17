using Chartboost.AdFormats.Fullscreen;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public sealed class FullscreenAdController : SimpleAdController
    {
        public FullscreenAdController(string placementIdentifier) : base(placementIdentifier) { }

        private IChartboostMediationFullscreenAd _fullscreenPlacement;

        public override async void Load()
        {
            if (_fullscreenPlacement != null)
                Invalidate();

            var fullscreenAdRequest = new ChartboostMediationFullscreenAdLoadRequest(PlacementIdentifier, DefaultKeywords);

            fullscreenAdRequest.DidRecordImpression += OnDidRecordImpression;
            fullscreenAdRequest.DidClick += OnDidClick;
            fullscreenAdRequest.DidReward += OnDidReward;
            fullscreenAdRequest.DidExpire += OnDidExpire;
            fullscreenAdRequest.DidClose += OnDidClose;
            
            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await ChartboostMediation.LoadFullscreenAd(fullscreenAdRequest);
            LoadingOverlay.Instance.ToggleLoadingOverlay(false);

            if (adLoadResult.Error.HasValue)
            {
                Debug.LogError($"Fullscreen Failed to Load with Error: {adLoadResult.Error.Value.Message}");
                return;
            }

            _fullscreenPlacement = adLoadResult.Ad;
            Debug.Log("Fullscreen Loaded!");
        }


        public override async void Show()
        {
            var adShowResult = await _fullscreenPlacement.Show();

            if (adShowResult.Error.HasValue)
            {
                Debug.LogError($"Fullscreen Failed to Show with Error: {adShowResult.Error.Value.Message}");
                return;
            }

            Debug.Log("Fullscreen Shown!");
        }

        public override void Invalidate()
        {
            _fullscreenPlacement.Invalidate();
            _fullscreenPlacement = null;
        }

        #region Fullscreen Callbacks

        private void OnDidRecordImpression(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen RecordImpression!");
        }

        private void OnDidClick(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen Clicked!");
        }

        private void OnDidReward(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen Rewarded!");
        }

        private void OnDidExpire(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen Expired!");
        }

        private void OnDidClose(IChartboostMediationFullscreenAd ad, ChartboostMediationError? error)
        {
            Debug.Log("Fullscreen Close!");
        }

        #endregion
    }
}
