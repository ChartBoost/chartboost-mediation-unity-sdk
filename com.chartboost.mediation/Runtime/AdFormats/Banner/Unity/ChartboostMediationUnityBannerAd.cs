using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner.Unity
{
    public delegate void ChartboostMediationUnityBannerAdEvent();
    public delegate void ChartboostMediationUnityBannerAdDragEvent(float x, float y);
    
    public partial class ChartboostMediationUnityBannerAd : MonoBehaviour
    {
        public ChartboostMediationUnityBannerAdEvent WillAppear;
        public ChartboostMediationUnityBannerAdEvent DidClick;
        public ChartboostMediationUnityBannerAdEvent DidRecordImpression;
        public ChartboostMediationUnityBannerAdDragEvent DidDrag;
        
        [SerializeField] private string _placementName;
        [SerializeField] private bool _draggable = true;
        [SerializeField] private bool _autoLoadOnInit = true;
        [SerializeField] private bool _resizeToFit;
        [SerializeField] private ChartboostMediationBannerHorizontalAlignment _horizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        [SerializeField] private ChartboostMediationBannerVerticalAlignment _verticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        
        private IChartboostMediationBannerView _bannerView;

        # region Unity Lifecycle
        private void Start()
        {
            ChartboostMediation.DidStart += ChartboostMediationOnDidStart;
        }

        private void OnEnable()
        {
            BannerView?.SetVisibility(true);
        }

        private void OnDisable()
        {
            BannerView?.SetVisibility(false);
        }

        public void OnDestroy() => BannerView?.Destroy();
        #endregion
        
        public string PlacementName
        {
            get => _placementName;
            internal set => _placementName = value;
        }
        public bool Draggable
        {
            get => _draggable;
            set
            {
                BannerView.SetDraggability(_draggable);
                _draggable = value;
            }
        }
        public bool ResizeToFit { get => _resizeToFit; set => _resizeToFit = value; }
        
        public async Task<ChartboostMediationBannerAdLoadResult> Load()
        {
            if (string.IsNullOrEmpty(_placementName))
            {
                const string error = "Placement Name is empty or not set in inspector";
                Logger.LogError("ChartboostMediationUnityBannerAd",  error);
                return new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError(error));
            }
            
            var recTransform = GetComponent<RectTransform>();
            var layoutParams = recTransform.LayoutParams();
            if (recTransform.GetComponentInParent<LayoutGroup>())
            {
                // Note : if rectTransform is part of a layoutgroup then we need wait until the layout is created
                // https://forum.unity.com/threads/solved-cant-get-the-rect-width-rect-height-of-an-element-when-using-layouts.377953/
                while (layoutParams.width == 0 && layoutParams.height == 0) // TODO: Find a better approach
                {
                    await Task.Yield();
                    layoutParams = recTransform.LayoutParams();
                }
            }
            
            var width = ChartboostMediationConverters.PixelsToNative(layoutParams.width);
            var height = ChartboostMediationConverters.PixelsToNative(layoutParams.height);
            var size = ChartboostMediationBannerAdSize.Adaptive(width, height);
            
            // TODO: Remove
            if (Math.Abs(width - 320) < .5f && Math.Abs(height - 50) < .5f)
            {
                size = ChartboostMediationBannerAdSize.Standard;    
            }
            else if (Math.Abs(width - 300) < .5f && Math.Abs(height - 250) < .5f)
            {
                size = ChartboostMediationBannerAdSize.MediumRect;    
            }
            else if (Math.Abs(width - 728) < .5f && Math.Abs(height - 90) < .5f)
            {
                size = ChartboostMediationBannerAdSize.Leaderboard;    
            }
            
            var loadRequest = new ChartboostMediationBannerAdLoadRequest(_placementName, size);
            
            var x = ChartboostMediationConverters.PixelsToNative(layoutParams.x);
            var y = ChartboostMediationConverters.PixelsToNative(layoutParams.y);
            
            return await BannerView.Load(loadRequest, x, y);
        }
        
        #region BannerView Wrap
        public Dictionary<string, string> Keywords => BannerView?.Keywords;
        public ChartboostMediationBannerAdLoadRequest Request => BannerView?.Request;
        public BidInfo? WinningBidInfo => BannerView?.WinningBidInfo;
        public string LoadId => BannerView?.LoadId;
        public ChartboostMediationBannerAdSize AdSize => BannerView?.AdSize;
        public ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                BannerView.HorizontalAlignment = value;
                _horizontalAlignment = value;
            }
        }
        public ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                BannerView.VerticalAlignment = value;
                _verticalAlignment = value;
            }
        }
        public void ResetAd() => BannerView.Reset();
        #endregion
        
        #region Events

        private void OnWillAppear(IChartboostMediationBannerView bannerView)
        {
            WillAppear?.Invoke();
            if (ResizeToFit)
            {
                var canvas = GetComponentInParent<Canvas>();
                var canvasScale = canvas.transform.localScale.x;
                var width = ChartboostMediationConverters.NativeToPixels(AdSize.Width)/canvasScale;
                var height = ChartboostMediationConverters.NativeToPixels(AdSize.Height)/canvasScale;
                var rect = GetComponent<RectTransform>();
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
        private void OnRecordImpression(IChartboostMediationBannerView bannerView) => DidRecordImpression?.Invoke();
        private void OnClick(IChartboostMediationBannerView bannerView) => DidClick?.Invoke();
        private async void ChartboostMediationOnDidStart(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                if (_autoLoadOnInit)
                {
                    await Load();
                }
            }
        }
        private void OnDrag(IChartboostMediationBannerView bannerView, float x, float y)
        {
            transform.position = new Vector3(x, y, 0);
            DidDrag?.Invoke(x, y);
        }

        #endregion 
        private IChartboostMediationBannerView BannerView
        {
            get
            {
                if (_bannerView == null)
                {
                    _bannerView = ChartboostMediation.GetBannerView();
                    _bannerView.WillAppear +=OnWillAppear;
                    _bannerView.DidClick += OnClick;
                    _bannerView.DidRecordImpression += OnRecordImpression;
                    _bannerView.DidDrag += OnDrag;
                }
        
                return _bannerView;
            }
        }
        
    }
}