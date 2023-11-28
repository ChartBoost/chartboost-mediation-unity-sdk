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
    public class BannerControllerAdBannerView : IBannerControllerAd
    {
        public event BannerControllerAdEvent DidLoad;
        public event BannerControllerAdEvent DidClick;
        public event BannerControllerAdEvent DidRecordImpression;
        public ChartboostMediationBannerAdLoadRequest Request => _bannerView?.Request;
        public string LoadId => _bannerView?.LoadId;
        public BidInfo? BidInfo => _bannerView?.WinningBidInfo;
        public ChartboostMediationBannerSize? ContainerSize => _bannerView?.ContainerSize;
        public ChartboostMediationBannerSize? AdSize => _bannerView?.AdSize;

        private GameObject _container;
        private ResizeOption _resizeOption;
        private IChartboostMediationBannerView _bannerView;
        
        public async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest loadRequest, RectTransform container)
        {
            _bannerView = GetBannerView();
            
            _container = container.gameObject;
            UpdateContainerSize(loadRequest.Size);
            
            var x = ChartboostMediationConverters.PixelsToNative(container.LayoutParams().x);
            var y = ChartboostMediationConverters.PixelsToNative(container.LayoutParams().y);
            
            #if UNITY_EDITOR
            await _bannerView.Load(loadRequest,x, y);
            
            OnLoad(_bannerView);
            return new ChartboostMediationBannerAdLoadResult("", null, null);
            #endif
            
            
            var result =  await _bannerView.Load(loadRequest,x, y);
            return result;
        }

        public async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest loadRequest, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            _bannerView = GetBannerView();
            _container = BannerController.sticky ? UIHelper.Instance.screenLocationStickyBannerContainer : UIHelper.Instance.screenLocationBannerContainer;
            
            if(0 <= (int)screenLocation && (int)screenLocation <= 2) // Top
                _container.transform.SetAsFirstSibling();
            
            if(4 <= (int)screenLocation && (int)screenLocation <= 6) // Bottom
                _container.transform.SetAsLastSibling();

            return await _bannerView.Load(loadRequest, screenLocation);
        }

        public void Reset()
        {
            _bannerView?.Reset();
            UpdateContainerSize(null);
        }

        public void Destroy()
        {
            if (_bannerView == null)
                return;

            _bannerView.DidLoad -= OnLoad;
            _bannerView.DidClick -= OnClick;
            _bannerView.DidRecordImpression -= OnRecordImpression;
            _bannerView.Destroy();

            UpdateContainerSize(null);
        }

        public void SetKeywords(Keyword[] keywords)
        {
            if(_bannerView == null)
                return;
            
            _bannerView.Keywords = keywords.ToDictionary(keyword => keyword.name, keyword => keyword.value);;
        }

        public void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment)
        {
            if(_bannerView == null)
                return;
            
            _bannerView.HorizontalAlignment = horizontalAlignment;
        }

        public void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment)
        {
            if(_bannerView == null)
                return;

            _bannerView.VerticalAlignment = verticalAlignment;
        }

        public void SetVisibility(bool isVisible)
        {
            _bannerView?.SetVisibility(isVisible);
            
            if (isVisible)
            {
                UpdateContainerSize(_bannerView?.AdSize ?? _bannerView?.Request?.Size);
            }
            else
            {
                UpdateContainerSize(null);
            }
        }

        public void SetDraggability(bool canDrag)
            => _bannerView?.SetDraggability(canDrag);

        public void SetResizeOption(ResizeOption resizeOption)
        {
            _resizeOption = resizeOption;
            Resize();
        }

        public void ToggleBackgroundColorVisibility(bool isVisible) { }

        private IChartboostMediationBannerView GetBannerView()
        {
            if (_bannerView != null)
            {
                _bannerView.DidLoad -= OnLoad;
                _bannerView.DidClick -= OnClick;
                _bannerView.DidRecordImpression -= OnRecordImpression;
                _bannerView.Destroy();
            }

            var bannerView = ChartboostMediation.GetBannerView();
            bannerView.DidLoad += OnLoad;
            bannerView.DidClick += OnClick;
            bannerView.DidRecordImpression += OnRecordImpression;

            return bannerView;
        }

        private void OnLoad(IChartboostMediationBannerView bannerView)
        {
            Resize();
            DidLoad?.Invoke(this);
        }

        private void OnClick(IChartboostMediationBannerView bannerView)
        {
            DidClick?.Invoke(this);
        }

        private void OnRecordImpression(IChartboostMediationBannerView bannerView)
        {
            DidRecordImpression?.Invoke(this);
        }

        private void Resize()
        {
            if (_resizeOption == ResizeOption.Disabled)
                return;

            var pivot = new Vector2(0, 1);  // top-left
            _bannerView?.ResizeToFit((ChartboostMediationBannerResizeAxis)_resizeOption, pivot);
            UpdateContainerSize(_bannerView?.AdSize);
        }

        private async void UpdateContainerSize(ChartboostMediationBannerSize? size)
        {
            var flexibleSpace = _container.AddOrGetComponent<LayoutElement>();
            var canvasScale = _container.GetComponentInParent<Canvas>().transform.localScale.x;
            var width = ChartboostMediationConverters.NativeToPixels(size?.Width ?? 0) / canvasScale;
            var height = ChartboostMediationConverters.NativeToPixels(size?.Height ?? 0) / canvasScale;
            
            await Task.Yield();

            flexibleSpace.minWidth = width;
            flexibleSpace.minHeight = height;

            var scrollView = _container.GetComponentInParent<ScrollRect>();
            scrollView.ScrollTo(_container.GetComponent<RectTransform>());
        }
    }
}
