using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner.Unity
{
    public delegate void ChartboostMediationUnityBannerAdEvent(ChartboostMediationUnityBannerAd unityBannerAd);

    public delegate void ChartboostMediationUnityBannerAdDragEvent(ChartboostMediationUnityBannerAd unityBannerAd, float x, float y);

    /// <summary>
    /// The ResizeOption enum 
    /// </summary>
    public enum ResizeOption
    {
        FitHorizontal,
        FitVertical,
        FitBoth,
        Disabled
    }
    
    [RequireComponent(typeof(RectTransform))]
    public sealed partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
        /// <summary>
        /// Called when ad is loaded within this GameObject. This will be called for each refresh when auto-refresh is enabled.
        /// </summary>
        public event ChartboostMediationUnityBannerAdEvent DidLoad;
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        public event ChartboostMediationUnityBannerAdEvent DidClick;
        
        /// <summary>
        /// Called when the ad impression occurs.
        /// </summary>
        public event ChartboostMediationUnityBannerAdEvent DidRecordImpression;
        
        /// <summary>
        ///  Called when this GameObject is dragged on screen.
        /// </summary>
        public event ChartboostMediationUnityBannerAdDragEvent DidDrag;
        
        [SerializeField] 
        private string placementName;
        [SerializeField] 
        private bool draggable;
        
        [SerializeField][HideInInspector]
        private ChartboostMediationBannerSizeType sizeType;
        [SerializeField][HideInInspector]
        private ResizeOption resizeOption = ResizeOption.FitBoth;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerHorizontalAlignment horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        [SerializeField][HideInInspector] 
        private ChartboostMediationBannerVerticalAlignment verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        
        private IChartboostMediationBannerView _bannerView;
        private Vector2 _lastPosition;
        private RectTransform _rectTransform;

        # region Unity Lifecycle
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (sizeType != ChartboostMediationBannerSizeType.Adaptive)
            {
                LockToFixedSize(sizeType);
            }
        }

        private void OnEnable() => BannerView?.SetVisibility(true);

        private void Update()
        {
            if (Draggable)
                return;
            
            // if this gameobject is moved by any other means except drag then
            // we should also move the corresponding view on native
            var distance = Vector2.Distance(_lastPosition, transform.position);
            if (distance > 0)
            {
                var x = ChartboostMediationConverters.PixelsToNative(_rectTransform.LayoutParams().x);
                var y = ChartboostMediationConverters.PixelsToNative(_rectTransform.LayoutParams().y);
                ((ChartboostMediationBannerViewBase)BannerView)?.MoveTo(x, y);
            }
            
            _lastPosition = transform.position;
        }

        private void OnDisable() => BannerView?.SetVisibility(false);

        public void OnDestroy() => BannerView?.Destroy();
        
        #endregion
        
        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName
        {
            get => placementName;
            internal set => placementName = value;
        }
        
        /// <summary>
        /// The ability of this gameobject to drag
        /// </summary>
        public bool Draggable
        {
            get => draggable;
            set
            {
                BannerView.SetDraggability(value);
                draggable = value;
            }
        }
        
        /// <summary>
        /// The resize option for this gameobject
        /// </summary>
        public ResizeOption ResizeOption
        {
            get => resizeOption;
            set
            {
                resizeOption = value;
                Resize();
            }
        }
        
        /// <summary>
        /// Loads an ad inside this gameobject
        /// </summary>
        /// <returns></returns>
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
                ChartboostMediationBannerSizeType.Standard => ChartboostMediationBannerSize.Standard,
                ChartboostMediationBannerSizeType.Medium => ChartboostMediationBannerSize.MediumRect,
                ChartboostMediationBannerSizeType.Leaderboard => ChartboostMediationBannerSize.Leaderboard,
                _ => await GetAdaptiveSize()
            };
            var loadRequest = new ChartboostMediationBannerAdLoadRequest(placementName, containerSize);
            var layoutParams = GetComponent<RectTransform>().LayoutParams();
            var x = ChartboostMediationConverters.PixelsToNative(layoutParams.x);
            var y = ChartboostMediationConverters.PixelsToNative(layoutParams.y);
        
            return await BannerView.Load(loadRequest, x, y);
        }
        
        #region BannerView Wrap
        
        /// <summary>
        /// The keywords targeted for the ad.
        /// </summary>
        public Dictionary<string, string> Keywords
        {
            get => BannerView?.Keywords;
            set => BannerView.Keywords = value;
        }

        /// <summary>
        /// The publisher supplied request that was used to load the ad.
        /// </summary>
        public ChartboostMediationBannerAdLoadRequest Request => BannerView?.Request;

        /// <summary>
        /// The winning bid info for the ad. Note that this will change with auto-refresh and will be notified in <see cref="DidLoad"/>
        /// </summary>
        public BidInfo? WinningBidInfo => BannerView?.WinningBidInfo;

        /// <summary>
        /// The identifier for this load call. Note that this will change with auto-refresh and will be notified in <see cref="DidLoad"/>
        /// </summary>
        public string LoadId => BannerView?.LoadId;

        /// <summary>
        /// The size of the loaded ad. Note that this will change with auto-refresh and will be notified in <see cref="DidLoad"/>
        /// </summary>
        public ChartboostMediationBannerSize? AdSize => BannerView?.AdSize;

        /// <summary>
        /// The horizontal alignment of the ad within this gameobject.
        /// </summary>
        public ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if(BannerView != null)
                    BannerView.HorizontalAlignment = value;
                horizontalAlignment = value;
            }
        }
        
        /// <summary>
        /// The vertical alignment of the ad within this gameobject.
        /// </summary>
        public ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                if(BannerView != null)
                    BannerView.VerticalAlignment = value;
                verticalAlignment = value;
            }
        }

        /// <summary>
        /// Clears the loaded ad
        /// </summary>
        public void ResetAd() => BannerView?.Reset();

        #endregion
        
        /// <summary>
        /// Locks the size of this GameObject based on the provided fixed type. Works only for fixed size types (Standard, Medium, Leaderboard)  
        /// </summary>
        /// <param name="sizeType"></param>
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
        
        /// <summary>
        /// Returns json representation of current state of the object
        /// </summary>
        public override string ToString() => JsonConvert.SerializeObject(BannerView);

        #region Events

        private void OnLoad(IChartboostMediationBannerView bannerView)
        {
            DidLoad?.Invoke(this);
            Resize();
        }

        private void OnRecordImpression(IChartboostMediationBannerView bannerView) => DidRecordImpression?.Invoke(this);

        private void OnClick(IChartboostMediationBannerView bannerView) => DidClick?.Invoke(this);

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
            DidDrag?.Invoke(this, x, y);
        }

        #endregion

        private void SetSizeType(ChartboostMediationBannerSizeType sizeType)
        {
            this.sizeType = sizeType;
        }
        
        private void Resize()
        {
            // Cannot resize until BannerView is loaded with Ad
            var adSize = AdSize ?? ChartboostMediationBannerSize.Adaptive(0, 0);
            if (Request?.Size.BannerType == ChartboostMediationBannerType.Fixed || adSize.SizeType == ChartboostMediationBannerSizeType.Unknown ||
                adSize is { Width: 0, Height: 0 })
                return;

            var rect = GetComponent<RectTransform>();
            var canvasScale = GetComponentInParent<Canvas>().transform.localScale.x;
            var width = ChartboostMediationConverters.NativeToPixels(adSize.Width)/canvasScale;
            var height = ChartboostMediationConverters.NativeToPixels(adSize.Height)/canvasScale;
            switch (resizeOption)
            {
                case ResizeOption.FitHorizontal:
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    break;
                case ResizeOption.FitVertical:
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    break;
                case ResizeOption.FitBoth:
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                    break;
                case ResizeOption.Disabled:
                default:
                    return;
            }
                
            BannerView?.ResizeToFit((ChartboostMediationBannerResizeAxis)resizeOption, rect.pivot);
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
                    
                    _bannerView.SetDraggability(Draggable);
                }
        
                return _bannerView;
            }
        }

        private async Task<ChartboostMediationBannerSize> GetAdaptiveSize()
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

            return ChartboostMediationBannerSize.Adaptive(width, height);
        }
        
    }
}
