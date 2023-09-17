using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner.Unity
{
    public enum UnityBannerAdSize
    {
        Adaptive,
        Standard,
        Medium,
        Leaderboard
    }
    
    public delegate void ChartboostMediationUnityBannerAdEvent();

    public delegate void ChartboostMediationUnityBannerAdDragEvent(float x, float y);
    
    public partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
        public ChartboostMediationUnityBannerAdEvent WillAppear;
        public ChartboostMediationUnityBannerAdEvent DidClick;
        public ChartboostMediationUnityBannerAdEvent DidRecordImpression;
        public ChartboostMediationUnityBannerAdDragEvent DidDrag;
        
        [SerializeField] 
        private bool autoLoadOnInit = true;
        [SerializeField] 
        private string placementName;
        [SerializeField] 
        private bool draggable = true;
        
        [SerializeField][HideInInspector] 
        private UnityBannerAdSize size;
        [SerializeField][HideInInspector] 
        private bool resizeToFit;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerHorizontalAlignment horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerVerticalAlignment verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        
        private IChartboostMediationBannerView _bannerView;

        # region Unity Lifecycle
        
        private void Start() => ChartboostMediation.DidStart += ChartboostMediationOnDidStart;

        private void OnEnable() => BannerView?.SetVisibility(true);

        private void OnDisable() => BannerView?.SetVisibility(false);

        public void OnDestroy() => BannerView?.Destroy();
        
        #endregion
        
        public string PlacementName
        {
            get => placementName;
            internal set => placementName = value;
        }

        public bool Draggable
        {
            get => draggable;
            set
            {
                BannerView.SetDraggability(value);
                draggable = value;
            }
        }
        
        public bool ResizeToFit { get => resizeToFit; set => resizeToFit = value; }

        public async Task<ChartboostMediationBannerAdLoadResult> Load()
        {
            if (string.IsNullOrEmpty(placementName))
            {
                const string error = "Placement Name is empty or not set in inspector";
                Logger.LogError("ChartboostMediationUnityBannerAd", error);
                return new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError(error));
            }
            
            var recTransform = GetComponent<RectTransform>();
            var layoutParams = recTransform.LayoutParams();
            if (recTransform.GetComponentInParent<LayoutGroup>())
            {
                // Note : if rectTransform is part of a layoutgroup then we need to wait until the layout is created
                // https://forum.unity.com/threads/solved-cant-get-the-rect-width-rect-height-of-an-element-when-using-layouts.377953/
                while (layoutParams.width == 0 && layoutParams.height == 0) // TODO: Find a better approach => use minWidth and minHeight instead of 0s ?
                {
                    await Task.Yield();
                    layoutParams = recTransform.LayoutParams();
                }
            }
            
            var width = ChartboostMediationConverters.PixelsToNative(layoutParams.width);
            var height = ChartboostMediationConverters.PixelsToNative(layoutParams.height);

            var bannerViewSize = this.size switch
            {
                UnityBannerAdSize.Standard => ChartboostMediationBannerAdSize.Standard,
                UnityBannerAdSize.Medium => ChartboostMediationBannerAdSize.MediumRect,
                UnityBannerAdSize.Leaderboard => ChartboostMediationBannerAdSize.Leaderboard,
                _ => ChartboostMediationBannerAdSize.Adaptive(width, height)
            };

            var loadRequest = new ChartboostMediationBannerAdLoadRequest(placementName, bannerViewSize);
            
            var x = ChartboostMediationConverters.PixelsToNative(layoutParams.x);
            var y = ChartboostMediationConverters.PixelsToNative(layoutParams.y);
            
            return await BannerView.Load(loadRequest, x, y);
        }
        
        #region BannerView Wrap
        
        public Dictionary<string, string> Keywords
        {
            get => BannerView?.Keywords;
            set => BannerView.Keywords = value;
        }

        public ChartboostMediationBannerAdLoadRequest Request => BannerView?.Request;

        public BidInfo? WinningBidInfo => BannerView?.WinningBidInfo;

        public string LoadId => BannerView?.LoadId;

        public ChartboostMediationBannerAdSize AdSize => BannerView?.AdSize;

        public ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                BannerView.HorizontalAlignment = value;
                horizontalAlignment = value;
            }
        }

        public ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                BannerView.VerticalAlignment = value;
                verticalAlignment = value;
            }
        }

        public void ResetAd() => BannerView.Reset();
        
        #endregion
        
        public override string ToString()
        {
            base.ToString();
            return JsonConvert.SerializeObject(BannerView);
        }

        #region Events
        
        private async void ChartboostMediationOnDidStart(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                if (autoLoadOnInit)
                {
                    await Load();
                }
            }
        }

        private void OnWillAppear(IChartboostMediationBannerView bannerView)
        {
            WillAppear?.Invoke();

            if (ResizeToFit)
            {
                var canvas = GetComponentInParent<Canvas>();
                var canvasScale = canvas.transform.localScale.x;
                var width = ChartboostMediationConverters.NativeToPixels(AdSize.Width) / canvasScale;
                var height = ChartboostMediationConverters.NativeToPixels(AdSize.Height) / canvasScale;
                var rect = GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }

        private void OnRecordImpression(IChartboostMediationBannerView bannerView) => DidRecordImpression?.Invoke();

        private void OnClick(IChartboostMediationBannerView bannerView) => DidClick?.Invoke();

        private void OnDrag(IChartboostMediationBannerView bannerView, float x, float y)
        {
            // x,y obtained from native is for top left corner (x = 0,y = 1)
            // RectTransform pivot may or may not be top-left (it's usually at center)
            var pivot = GetComponent<RectTransform>().pivot;
            var widthInPixels = ChartboostMediationConverters.NativeToPixels(AdSize.Width);
            var heightInPixels = ChartboostMediationConverters.NativeToPixels(AdSize.Height);
            x += widthInPixels * pivot.x; // top-left x is 0
            y += heightInPixels * (pivot.y - 1); // top-left y is 1

            transform.position = new Vector3(x, y, 0);
            DidDrag?.Invoke(x, y);
        }

        #endregion

        private void SetUnityBannerAdSize(UnityBannerAdSize size)
        {
            this.size = size;
        }

        private IChartboostMediationBannerView BannerView
        {
            get
            {
                if (_bannerView == null)
                {
                    _bannerView = ChartboostMediation.GetBannerView();
                    _bannerView.WillAppear += OnWillAppear;
                    _bannerView.DidClick += OnClick;
                    _bannerView.DidRecordImpression += OnRecordImpression;
                    _bannerView.DidDrag += OnDrag;
                    
                    _bannerView.SetVisibility(gameObject.activeSelf);
                    _bannerView.SetDraggability(Draggable);
                }
        
                return _bannerView;
            }
        }
        
    }
}
