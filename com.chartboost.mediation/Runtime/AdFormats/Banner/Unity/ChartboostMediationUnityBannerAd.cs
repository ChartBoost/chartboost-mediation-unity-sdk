using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using static Chartboost.Utilities.Constants;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner.Unity
{
    public delegate void ChartboostMediationUnityBannerAdEvent();

    public delegate void ChartboostMediationUnityBannerAdDragEvent(float x, float y);
    
    [RequireComponent(typeof(RectTransform))]
    public partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
        public ChartboostMediationUnityBannerAdEvent DidLoad;
        public ChartboostMediationUnityBannerAdEvent DidClick;
        public ChartboostMediationUnityBannerAdEvent DidRecordImpression;
        public ChartboostMediationUnityBannerAdDragEvent DidDrag;
        
        [SerializeField] 
        private bool autoLoadOnInit = true;
        [SerializeField] 
        private string placementName;
        [SerializeField] 
        private bool draggable = true;
        
        [SerializeField][HideInInspector][InspectorName("Size")] 
        private ChartboostMediationBannerSizeType sizeType;
        [SerializeField][HideInInspector] 
        private bool resizeToFit = false;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerResizeAxis resizeAxis;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerHorizontalAlignment horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerVerticalAlignment verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        
        private IChartboostMediationBannerView _bannerView;

        # region Unity Lifecycle
        private void Start()
        {
            if (sizeType != ChartboostMediationBannerSizeType.Adaptive)
            {
                LockToFixedSize(sizeType);
            }
            
            ChartboostMediation.DidStart += ChartboostMediationOnDidStart;
        }

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
        
        public ChartboostMediationBannerResizeAxis ResizeAxis { get => resizeAxis; set => resizeAxis = value; }
        
        public async Task<ChartboostMediationBannerAdLoadResult> Load()
        {
            if (string.IsNullOrEmpty(placementName))
            {
                const string error = "Placement Name is empty or not set in inspector";
                Logger.LogError("ChartboostMediationUnityBannerAd", error);
                return new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError(error));
            }
            
            var containerSize = sizeType switch
            {
                ChartboostMediationBannerSizeType.Standard => ChartboostMediationBannerAdSize.Standard,
                ChartboostMediationBannerSizeType.Medium => ChartboostMediationBannerAdSize.MediumRect,
                ChartboostMediationBannerSizeType.Leaderboard => ChartboostMediationBannerAdSize.Leaderboard,
                _ => await GetAdaptiveSize()
            };
            var loadRequest = new ChartboostMediationBannerAdLoadRequest(placementName, containerSize);
            var layoutParams = GetComponent<RectTransform>().LayoutParams();
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

        public ChartboostMediationBannerAdSize AdSize => BannerView?.AdSize ?? ChartboostMediationBannerAdSize.Adaptive(0,0);

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
        
        public void LockToFixedSize(ChartboostMediationBannerSizeType sizeType)
        {
            // ReSharper disable once PossibleNullReferenceException
            var canvas =  GetComponentInParent<Canvas>();
            var rect = GetComponent<RectTransform>();
            
            var canvasScale = canvas.transform.localScale.x;
            float width;
            float height;

            switch (sizeType)
            {
                case ChartboostMediationBannerSizeType.Adaptive: 
                    return;
                case ChartboostMediationBannerSizeType.Standard: 
                    width = ChartboostMediationConverters.NativeToPixels(BannerSize.STANDARD.Item1)/canvasScale;
                    height = ChartboostMediationConverters.NativeToPixels(BannerSize.STANDARD.Item2)/canvasScale;
                    break;
                case ChartboostMediationBannerSizeType.Medium:
                    width = ChartboostMediationConverters.NativeToPixels(BannerSize.MEDIUM.Item1)/canvasScale;
                    height = ChartboostMediationConverters.NativeToPixels(BannerSize.MEDIUM.Item2)/canvasScale;
                    break;
                case ChartboostMediationBannerSizeType.Leaderboard:
                    width = ChartboostMediationConverters.NativeToPixels(BannerSize.LEADERBOARD.Item1)/canvasScale;
                    height = ChartboostMediationConverters.NativeToPixels(BannerSize.LEADERBOARD.Item2)/canvasScale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        
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

        private void OnLoad(IChartboostMediationBannerView bannerView)
        {
            DidLoad?.Invoke();

            if (ResizeToFit)
            {
                var rect = GetComponent<RectTransform>();
                
                var canvasScale = GetComponentInParent<Canvas>().transform.localScale.x;
                var width = ChartboostMediationConverters.NativeToPixels(AdSize.Width)/canvasScale;
                var height = ChartboostMediationConverters.NativeToPixels(AdSize.Height)/canvasScale;
                switch (resizeAxis)
                {
                    case ChartboostMediationBannerResizeAxis.Horizontal:
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                        BannerView.VerticalAlignment = verticalAlignment;
                        break;
                    case ChartboostMediationBannerResizeAxis.Vertical:
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                        BannerView.HorizontalAlignment = horizontalAlignment;
                        break;
                    case ChartboostMediationBannerResizeAxis.Both:
                    default:
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                        break;
                }
                
                BannerView?.ResizeToFit(resizeAxis, rect.pivot);
            }
        }

        private void OnRecordImpression(IChartboostMediationBannerView bannerView) => DidRecordImpression?.Invoke();

        private void OnClick(IChartboostMediationBannerView bannerView) => DidClick?.Invoke();

        private void OnDrag(IChartboostMediationBannerView bannerView, float x, float y)
        {
            // x,y obtained from native is for top left corner (x = 0,y = 1)
            // RectTransform pivot may or may not be top-left (it's usually at center)
            var rect = GetComponent<RectTransform>();
            var pivot = rect.pivot;
            var widthInPixels = rect.LayoutParams().width;
            var heightInPixels = rect.LayoutParams().height;
            x += widthInPixels * pivot.x;
            y -= heightInPixels - heightInPixels * pivot.y;

            transform.position = new Vector3(x, y, 0);
            DidDrag?.Invoke(x, y);
        }

        #endregion

        private void SetSizeType(ChartboostMediationBannerSizeType sizeType)
        {
            this.sizeType = sizeType;
        }

        private IChartboostMediationBannerView BannerView
        {
            get
            {
                if (_bannerView == null)
                {
                    _bannerView = ChartboostMediation.GetBannerView();
                    _bannerView.DidLoad += OnLoad;
                    _bannerView.DidClick += OnClick;
                    _bannerView.DidRecordImpression += OnRecordImpression;
                    _bannerView.DidDrag += OnDrag;
                    
                    _bannerView.SetVisibility(gameObject.activeSelf);
                    _bannerView.SetDraggability(Draggable);
                }
        
                return _bannerView;
            }
        }

        private async Task<ChartboostMediationBannerAdSize> GetAdaptiveSize()
        {
            var recTransform = GetComponent<RectTransform>();
            var layoutParams = recTransform.LayoutParams();
            
            // Note : if rectTransform is part of a layoutgroup then we need to wait until the layout is created
            // https://forum.unity.com/threads/solved-cant-get-the-rect-width-rect-height-of-an-element-when-using-layouts.377953/
            if (recTransform.GetComponentInParent<LayoutGroup>())
            {
                // Wait a couple of frames
                await Task.Yield();
                await Task.Yield();
                layoutParams = recTransform.LayoutParams();
            }
            
            var width = ChartboostMediationConverters.PixelsToNative(layoutParams.width);
            var height = ChartboostMediationConverters.PixelsToNative(layoutParams.height);

            return ChartboostMediationBannerAdSize.Adaptive(width, height);
        }
        
    }
}
