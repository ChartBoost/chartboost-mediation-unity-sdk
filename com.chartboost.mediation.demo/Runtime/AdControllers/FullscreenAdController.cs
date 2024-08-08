using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Demo.AdControllers
{
    public class FullscreenAdController : SimpleAdController
    {
        protected IFullscreenAd FullscreenPlacement;

        public FullscreenAdController(string placementIdentifier) : base(placementIdentifier) { }

        public override async void Load()
        {
            if (FullscreenPlacement != null)
                Dispose();

            var fullscreenAdRequest = new FullscreenAdLoadRequest(PlacementIdentifier, DefaultKeywords);

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

        public override void Dispose()
        {
            FullscreenPlacement?.Dispose();
            FullscreenPlacement = null;
        }

        #region Fullscreen Callbacks

        protected void OnDidRecordImpression(IFullscreenAd ad)
        {
            Debug.Log("Fullscreen RecordImpression!");
        }

        protected void OnDidClick(IFullscreenAd ad)
        {
            Debug.Log("Fullscreen Clicked!");
        }

        protected void OnDidReward(IFullscreenAd ad)
        {
            Debug.Log("Fullscreen Rewarded!");
        }

        protected void OnDidExpire(IFullscreenAd ad)
        {
            Debug.Log("Fullscreen Expired!");
        }

        protected void OnDidClose(IFullscreenAd ad, ChartboostMediationError? error)
        {
            Debug.Log("Fullscreen Close!");
        }

        #endregion
    }
}
