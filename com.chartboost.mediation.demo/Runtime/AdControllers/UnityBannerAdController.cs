using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Mediation.Demo.Pages;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
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
            _unityBanner.DidBeginDrag += OnDidBeginDragBanner;
            _unityBanner.DidDrag += OnDidDragBanner;
            _unityBanner.DidEndDrag += OnDidEndDragBanner;
            _unityBanner.Keywords = DefaultKeywords;

            // Bottom-center
            _unityBanner.transform.position = new Vector3(Screen.width / 2f, 0, 0);
            _unityBanner.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);

            var adLoadRequest = new BannerAdLoadRequest(PlacementIdentifier, BannerSize.Standard);

            LoadingOverlay.Instance.ToggleLoadingOverlay(true);
            var adLoadResult = await _unityBanner.Load(adLoadRequest);
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
            
            // Update size
            var canvasScale = _unityBanner.GetComponentInParent<Canvas>().transform.localScale.x;
            var width = DensityConverters.NativeToPixels(_unityBanner.BannerSize?.Width ?? 0)/canvasScale;
            var height = DensityConverters.NativeToPixels(_unityBanner.BannerSize?.Height ?? 0)/canvasScale;
            var rectTransform = _unityBanner.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        private void OnDidClickBanner(UnityBannerAd unityBannerAd)
        {
            Debug.Log("Unity Banner Clicked");
        }

        private void OnDidBeginDragBanner(UnityBannerAd unityBannerAd, float x, float y)
        {
            Debug.Log("Unity Banner Drag Begin");
        }
        
        private void OnDidDragBanner(UnityBannerAd unityBannerAd, float x, float y)
        {
            Debug.Log("Unity Banner Drag");
        }
        
        private void OnDidEndDragBanner(UnityBannerAd unityBannerAd, float x, float y)
        {
            Debug.Log("Unity Banner Drag End");
        }
        #endregion
    }
}
