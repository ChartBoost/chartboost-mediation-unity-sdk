using Chartboost.AdFormats.Fullscreen;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public class FullscreenAdController : SimpleAdController
    {
        protected IChartboostMediationFullscreenAd FullscreenPlacement;

        public FullscreenAdController(string placementIdentifier) : base(placementIdentifier) { }

        public override async void Load()
        {
            if (FullscreenPlacement != null)
                Invalidate();

            var fullscreenAdRequest = new ChartboostMediationFullscreenAdLoadRequest(PlacementIdentifier, DefaultKeywords);

            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await ChartboostMediation.LoadFullscreenAd(fullscreenAdRequest);
            LoadingOverlay.Instance.ToggleLoadingOverlay(false);

            if (adLoadResult.Error.HasValue)
            {
                Debug.LogError($"Fullscreen Failed to Load with Error: {adLoadResult.Error.Value.Message}");
                return;
            }

            FullscreenPlacement = adLoadResult.Ad;

            if (FullscreenPlacement != null)
            {
                FullscreenPlacement.DidRecordImpression += OnDidRecordImpression;
                FullscreenPlacement.DidClick += OnDidClick;
                FullscreenPlacement.DidReward += OnDidReward;
                FullscreenPlacement.DidExpire += OnDidExpire;
                FullscreenPlacement.DidClose += OnDidClose;
            }

            Debug.Log("Fullscreen Loaded!");
        }

        public override async void Show()
        {
            var adShowResult = await FullscreenPlacement.Show();

            if (adShowResult.Error.HasValue)
            {
                Debug.LogError($"Fullscreen Failed to Show with Error: {adShowResult.Error.Value.Message}");
                return;
            }

            Debug.Log("Fullscreen Shown!");
        }

        public override void Invalidate()
        {
            FullscreenPlacement?.Invalidate();
            FullscreenPlacement = null;
        }

        #region Fullscreen Callbacks

        protected void OnDidRecordImpression(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen RecordImpression!");
        }

        protected void OnDidClick(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen Clicked!");
        }

        protected void OnDidReward(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen Rewarded!");
        }

        protected void OnDidExpire(IChartboostMediationFullscreenAd ad)
        {
            Debug.Log("Fullscreen Expired!");
        }

        protected void OnDidClose(IChartboostMediationFullscreenAd ad, ChartboostMediationError? error)
        {
            Debug.Log("Fullscreen Close!");
        }

        #endregion
    }
}
