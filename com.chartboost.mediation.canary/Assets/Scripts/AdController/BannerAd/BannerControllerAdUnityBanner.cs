using System.Linq;
using System.Threading.Tasks;
using Chartboost;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace AdController.BannerAd
{
    public class BannerControllerAdUnityBanner : IBannerControllerAd
    {
        public event BannerControllerAdEvent DidLoad;
        public event BannerControllerAdEvent DidClick;
        public event BannerControllerAdEvent DidRecordImpression;
        public ChartboostMediationBannerAdLoadRequest Request => _unityBannerAd!= null ? _unityBannerAd.Request : null;
        public string LoadId => _unityBannerAd!= null ? _unityBannerAd.LoadId : null;
        public BidInfo? BidInfo => _unityBannerAd!= null ? _unityBannerAd.WinningBidInfo : null;
        public ChartboostMediationBannerSize? ContainerSize => GetContainerSize();
        public ChartboostMediationBannerSize? AdSize => _unityBannerAd!= null ? _unityBannerAd.AdSize : null;

        private ChartboostMediationUnityBannerAd _unityBannerAd;

        public async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest loadRequest, RectTransform container)
        {
            // Combination of VerticalLayoutGroup and ContentSizeFitter will ensure that the container
            // resizes based on the size of it's children
            var verticalLayoutGroup = container.gameObject.AddOrGetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            
            var contentSizeFitter = container.gameObject.AddOrGetComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;

            _unityBannerAd = GetUnityBannerAd(loadRequest.PlacementName, container, loadRequest.Size);
            #if UNITY_EDITOR
            await _unityBannerAd.Load();
            OnLoad(_unityBannerAd);
            return new ChartboostMediationBannerAdLoadResult("", null, null);
            #endif
            return await _unityBannerAd.Load();
        }

        public Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest loadRequest, ChartboostMediationBannerAdScreenLocation screenLocation)
            => throw new System.NotImplementedException();

        public void Reset()
        {
            if(_unityBannerAd != null)
                _unityBannerAd.ResetAd();
        }

        public void Destroy()
        {
            if (_unityBannerAd == null)
                return;

            _unityBannerAd.DidLoad -= OnLoad;
            _unityBannerAd.DidClick -= OnClick;
            _unityBannerAd.DidRecordImpression -= OnRecordImpression;
                
            Object.Destroy(_unityBannerAd.gameObject);
        }

        public void SetKeywords(Keyword[] keywords)
        {
            if (_unityBannerAd != null)
                _unityBannerAd.Keywords = keywords.ToDictionary(keyword => keyword.name, keyword => keyword.value);
        }

        public void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment)
        {
            if (_unityBannerAd != null)
                _unityBannerAd.HorizontalAlignment = horizontalAlignment;
        }

        public void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment)
        {
            if(_unityBannerAd != null)
                _unityBannerAd.VerticalAlignment = verticalAlignment;
        }

        public void SetVisibility(bool isVisible)
        {
            if(_unityBannerAd != null)
                _unityBannerAd.gameObject.SetActive(isVisible);
        }

        public void SetDraggability(bool canDrag)
        {
            if (_unityBannerAd != null)
                _unityBannerAd.Draggable = canDrag;
        }

        public void SetResizeOption(ResizeOption resizeOption)
        {
            if (_unityBannerAd != null)
                _unityBannerAd.ResizeOption = resizeOption;
        }

        public void ToggleBackgroundColorVisibility(bool isVisible)
        {
            if (_unityBannerAd == null)
                return;

            var backgroundImage = _unityBannerAd.gameObject.AddOrGetComponent<Image>();
            backgroundImage.color = new Color(0, 01, 0, 0.25f);
            backgroundImage.enabled = isVisible;
        }

        private ChartboostMediationUnityBannerAd GetUnityBannerAd(string placementName, Transform parent, ChartboostMediationBannerSize size)
        {
            if (_unityBannerAd != null)
            {
                _unityBannerAd.DidLoad -= OnLoad;
                _unityBannerAd.DidClick -= OnClick;
                _unityBannerAd.DidRecordImpression -= OnRecordImpression;
                
                Object.Destroy(_unityBannerAd.gameObject);
            }
            var unityBannerAd = ChartboostMediation.GetUnityBannerAd(placementName, parent, size);
            unityBannerAd.DidLoad += OnLoad;
            unityBannerAd.DidClick += OnClick;
            unityBannerAd.DidRecordImpression += OnRecordImpression;

            return unityBannerAd;
        }

        private void OnLoad(ChartboostMediationUnityBannerAd unityBannerAd)
        {
            DidLoad?.Invoke(this);
        }

        private void OnClick(ChartboostMediationUnityBannerAd unityBannerAd)
        {
            DidClick?.Invoke(this);
        }

        private void OnRecordImpression(ChartboostMediationUnityBannerAd unityBannerAd)
        {
            DidRecordImpression?.Invoke(this);
        }
        
        private ChartboostMediationBannerSize? GetContainerSize()
        {
            if (_unityBannerAd == null)
                return null;
            
            var size = _unityBannerAd.Request?.Size ?? new ChartboostMediationBannerSize();
            var layoutParams = _unityBannerAd.GetComponent<RectTransform>().LayoutParams();
            
            size.Width = ChartboostMediationConverters.PixelsToNative(layoutParams.width);
            size.Height = ChartboostMediationConverters.PixelsToNative(layoutParams.height);
            
            return size;
        }

    }
}
