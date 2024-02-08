using System.ComponentModel;
using Chartboost;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace AdController.BannerAd
{
    public class BannerController : CanaryAdController
    {
        public TMP_Dropdown sizeDropdown;
        public TMP_Dropdown locationDropdown;
        public TMP_Dropdown resizeOptionDropdown;
        public TMP_Dropdown horizontalAlignmentDropdown;
        public TMP_Dropdown verticalAlignmentDropdown;

        public Toggle useScreenLocation;
        public Toggle visibilityToggle;
        public Toggle dragToggle;
        public Toggle backgroundColorToggle;
        public Toggle stickyToggle;

        public GameObject content;
        public GameObject screenLocationPanel;
        public RectTransform adaptiveBannerContainer;
        public RectTransform stickyBannerContainer;
        
        // custom size
        public GameObject customSizePanel;
        public TMP_InputField customSizeWidth;
        public TMP_InputField customSizeHeight;
        
        private Vector2 _customSize;
        private IBannerControllerAd _bannerAd;
        private static IBannerControllerAd _stickyBannerAd;

        public static bool sticky;
        
        private IBannerControllerAd BannerControllerAd
        {
            get => sticky ? _stickyBannerAd : _bannerAd;
            set
            {
                if (sticky)
                    _stickyBannerAd = value;
                else
                    _bannerAd = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            stickyBannerContainer = UIHelper.Instance.stickyBannerContainer.GetComponent<RectTransform>();
            
            var callbacks = new[]
            {
                CanaryConstants.Callbacks.DidLoad, 
                CanaryConstants.Callbacks.DidClick, 
                CanaryConstants.Callbacks.DidRecordImpression,
                CanaryConstants.Callbacks.DidReceiveILRD
            };
            callbackPanel.AddCallbacks(callbacks);

            sizeDropdown.onValueChanged.AddListener(OnSizeDropdownChange);
            resizeOptionDropdown.onValueChanged.AddListener(OnResizeOptionDropdownChange);
            horizontalAlignmentDropdown.onValueChanged.AddListener(OnHorizontalAlignmentDropdownChange);
            verticalAlignmentDropdown.onValueChanged.AddListener(OnVerticalAlignmentDropdownChange);
            
            visibilityToggle.onValueChanged.AddListener(OnVisibilityToggle);
            dragToggle.onValueChanged.AddListener(OnDragToggle);
            backgroundColorToggle.onValueChanged.AddListener(OnBackgroundColorToggle);
            stickyToggle.onValueChanged.AddListener(OnStickyToggle);
            
            customSizeWidth.onValueChanged.AddListener(OnCustomSizeWidthEdit);
            customSizeHeight.onValueChanged.AddListener(OnCustomSizeHeightEdit);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            stickyToggle.SetIsOnWithoutNotify(sticky);
            
            _bannerAd?.SetVisibility(visibilityToggle.isOn);
        }

        protected override void OnDisable()
        {
            _bannerAd?.SetVisibility(false);
        }

        protected override void OnDestroy()
        {
            if (_bannerAd == null)
                return;

            _bannerAd.DidLoad -= OnLoad;
            _bannerAd.DidClick -= OnClick;
            _bannerAd.DidRecordImpression -= OnRecordImpression;
            _bannerAd.Destroy();

            _bannerAd = null;
        }

        public override void Configure(AdControllerConfiguration configuration)
        {
            base.Configure(configuration);
            screenLocationPanel.SetActive(configuration.chartboostMediationAPI == ChartboostMediationAPI.BannerView);
            backgroundColorToggle.gameObject.SetActive(configuration.chartboostMediationAPI == ChartboostMediationAPI.UnityBanner);
        }

        public override async void OnLoadButtonPushed()
        {
            base.OnLoadButtonPushed();
            
            var placement = controllerConfiguration.placementName;
            var size = GetSizeFromDropdown();
            var loadRequest = new ChartboostMediationBannerAdLoadRequest(placement, size);
            
            BannerControllerAd = GetBannerAd();
            if (BannerControllerAd == null)
            {
                Log(NotFound);
                return;
            }
            
            Log(RequestingLoad);

            ChartboostMediationBannerAdLoadResult loadResult;

            if (controllerConfiguration.chartboostMediationAPI == ChartboostMediationAPI.BannerView && useScreenLocation.isOn)
            {
                loadResult = await BannerControllerAd.Load(loadRequest, (ChartboostMediationBannerAdScreenLocation)locationDropdown.value);
            }
            else
            {
                var container = sticky ? stickyBannerContainer : adaptiveBannerContainer;
                loadResult = await BannerControllerAd.Load(loadRequest, container);
            }
            
            Log(" LoadResult size ",$" {JsonConvert.SerializeObject(loadResult.Size)}");
            
            if (loadResult.Error.HasValue)
            {
                var errorLoadDetails = new AdLoadDetails(loadResult.Error);
                DidLoad(errorLoadDetails);
                return;
            }
            
            // Set initial state of banner ad object based on what we have on UI
            BannerControllerAd.SetKeywords(keywordsDataSource.Keywords);
            BannerControllerAd.SetHorizontalAlignment((ChartboostMediationBannerHorizontalAlignment)horizontalAlignmentDropdown.value);
            BannerControllerAd.SetVerticalAlignment((ChartboostMediationBannerVerticalAlignment)verticalAlignmentDropdown.value);
            BannerControllerAd.SetResizeOption((ResizeOption)resizeOptionDropdown.value);
            BannerControllerAd.SetDraggability(dragToggle.isOn);
            BannerControllerAd.ToggleBackgroundColorVisibility(backgroundColorToggle.isOn);
        }

        public override void OnClearButtonPushed()
        {
            base.OnClearButtonPushed();
            if (BannerControllerAd == null)
            {
                Log(HasNotBeenLoaded);
                return;
            }
            
            BannerControllerAd?.Reset();
        }

        public override void OnDestroyButtonPushed()
        {
            base.OnDestroyButtonPushed();
            
            if (BannerControllerAd == null)
            {
                Log(HasNotBeenLoaded);
                return;
            }

            BannerControllerAd.DidLoad -= OnLoad;
            BannerControllerAd.DidClick -= OnClick;
            BannerControllerAd.DidRecordImpression -= OnRecordImpression;
            BannerControllerAd.Destroy();

            BannerControllerAd = null;
        }

        private void OnCustomSizeWidthEdit(string value)
        {
            _customSize.x = float.Parse(value);
            var customSizeIndex = sizeDropdown.options.Count - 1;
            
            var text = $"Custom ({_customSize.x} X {_customSize.y})";
            sizeDropdown.options[customSizeIndex].text = text;
            sizeDropdown.captionText.text = text;
        }

        private void OnCustomSizeHeightEdit(string value)
        {
            _customSize.y = float.Parse(value);
            var customSizeIndex = sizeDropdown.options.Count - 1;
            
            var text = $"Custom ({_customSize.x} X {_customSize.y})";
            sizeDropdown.options[customSizeIndex].text = text;
            sizeDropdown.captionText.text = text;
        }
        
        private void OnSizeDropdownChange(int value)
        {
            var customSizeIndex = sizeDropdown.options.Count - 1;
            customSizePanel.SetActive(value == customSizeIndex);
        }
        
        private void OnResizeOptionDropdownChange(int value)
        {
            BannerControllerAd?.SetResizeOption((ResizeOption)value);
        }

        private void OnHorizontalAlignmentDropdownChange(int value)
        {
            BannerControllerAd?.SetHorizontalAlignment((ChartboostMediationBannerHorizontalAlignment)value);
        }

        private void OnVerticalAlignmentDropdownChange(int value)
        {
            BannerControllerAd?.SetVerticalAlignment((ChartboostMediationBannerVerticalAlignment)value);
        }

        private void OnVisibilityToggle(bool isOn)
        {
            BannerControllerAd?.SetVisibility(isOn);
        }

        private void OnDragToggle(bool isOn)
        {
            BannerControllerAd?.SetDraggability(isOn);
        }

        private void OnBackgroundColorToggle(bool isOn)
        {
            BannerControllerAd?.ToggleBackgroundColorVisibility(isOn);
        }

        private static void OnStickyToggle(bool isOn)
        {
            sticky = !sticky;
        }

        private IBannerControllerAd GetBannerAd()
        {
            var chartboostMediationAPI = controllerConfiguration.chartboostMediationAPI;
            if (chartboostMediationAPI != ChartboostMediationAPI.BannerView && chartboostMediationAPI != ChartboostMediationAPI.UnityBanner)
            {
                throw new InvalidEnumArgumentException($"Invalid API for banner Controller");
            }

            if (BannerControllerAd != null)
            {
                BannerControllerAd.DidLoad -= OnLoad;
                BannerControllerAd.DidClick -= OnClick;
                BannerControllerAd.DidRecordImpression -= OnRecordImpression;
                BannerControllerAd.Destroy();

                BannerControllerAd = null;
            }
            
            var bannerAd = chartboostMediationAPI == ChartboostMediationAPI.BannerView ? (IBannerControllerAd)new BannerControllerAdBannerView() : new BannerControllerAdUnityBanner();
            bannerAd.DidLoad += OnLoad;
            bannerAd.DidClick += OnClick;
            bannerAd.DidRecordImpression += OnRecordImpression;
            return bannerAd;
        }

        private void OnLoad(IBannerControllerAd bannerControllerAd)
        {
            var loadDetails = new BannerAdLoadDetails
            {
                placementName = bannerControllerAd.Request.PlacementName,
                bidInfo = bannerControllerAd.BidInfo ?? new BidInfo(),
                loadId = bannerControllerAd.LoadId,
                // metrics = bannerAd.Metrics,
                containerSize = bannerControllerAd.ContainerSize ?? new ChartboostMediationBannerSize(),
                adSize = bannerControllerAd.AdSize ?? new ChartboostMediationBannerSize()
            };
            
            DidLoad(loadDetails);
        }

        private void OnClick(IBannerControllerAd bannerControllerAd)
        {
            DidClick(bannerControllerAd.Request.PlacementName, null);
        }

        private void OnRecordImpression(IBannerControllerAd bannerControllerAd)
        {
            DidRecordImpression(bannerControllerAd.Request.PlacementName, null);
        }

        private ChartboostMediationBannerSize GetSizeFromDropdown()
        {
            var padding = content.GetComponent<LayoutGroup>().padding.horizontal;
            var width = ChartboostMediationConverters.PixelsToNative(content.GetComponent<RectTransform>().LayoutParams().width) - padding;
            var size = sizeDropdown.value switch
            {
                // horizontal
                0 => ChartboostMediationBannerSize.Standard,
                1 => ChartboostMediationBannerSize.MediumRect,
                2 => ChartboostMediationBannerSize.Leaderboard,
                3 => ChartboostMediationBannerSize.Adaptive2X1(width),
                4 => ChartboostMediationBannerSize.Adaptive4X1(width),
                5 => ChartboostMediationBannerSize.Adaptive6X1(width),
                6 => ChartboostMediationBannerSize.Adaptive8X1(width),
                7 => ChartboostMediationBannerSize.Adaptive10X1(width),

                // vertical
                8 => ChartboostMediationBannerSize.Adaptive1X2(width),
                9 => ChartboostMediationBannerSize.Adaptive1X3(width),
                10 => ChartboostMediationBannerSize.Adaptive1X4(width),
                11 => ChartboostMediationBannerSize.Adaptive9X16(width),

                // custom
                _ => ChartboostMediationBannerSize.Adaptive(_customSize.x, _customSize.y)
            };
            return size;
        }
    }
}
