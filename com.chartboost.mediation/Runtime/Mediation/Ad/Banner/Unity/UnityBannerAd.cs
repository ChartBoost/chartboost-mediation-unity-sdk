using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Chartboost.Mediation.Ad.Banner.Unity
{
    /// <summary>
    /// Unity UI compatible <see cref="IBannerAd"/>.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public partial class UnityBannerAd : MonoBehaviour, IAd
    {
        /// <summary>
        /// Called when ad is loaded within this GameObject. This will be called for each refresh when auto-refresh is enabled.
        /// </summary>
        public event UnityBannerAdEvent WillAppear;
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        public event UnityBannerAdEvent DidClick;
        
        /// <summary>
        /// Called when the ad impression occurs.
        /// </summary>
        public event UnityBannerAdEvent DidRecordImpression;
        
        /// <summary>
        ///  Called when this GameObject has begun dragging on screen.
        /// </summary>
        public event UnityBannerAdDragEvent DidBeginDrag;
        
        /// <summary>
        ///  Called when this GameObject is dragged on screen.
        /// </summary>
        public event UnityBannerAdDragEvent DidDrag;
        
        /// <summary>
        ///  Called when this GameObject has finished dragging on screen.
        /// </summary>
        public event UnityBannerAdDragEvent DidEndDrag;
        
        [SerializeField] 
        private string placementName;
        [SerializeField] 
        private bool draggable;
        
        [SerializeField][HideInInspector] 
        private BannerHorizontalAlignment horizontalAlignment = BannerHorizontalAlignment.Center;
        [SerializeField][HideInInspector] 
        private BannerVerticalAlignment verticalAlignment = BannerVerticalAlignment.Center;
        
        private IBannerAd _bannerAd;
        private LayoutParams _lastLayoutParams = new();
        private RectTransform _rectTransform;
        private bool _isDragging;
        
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
            get => BannerAd.Draggable;
            set
            {
                BannerAd.Draggable = value;
                draggable = value;
            }
        }

        private RectTransform UnityBannerTransform
        {
            get
            {
                if (_rectTransform != null)
                    return _rectTransform;

                _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        #region Unity LifeCycle

        private void OnEnable() => BannerAd.Visible = true;
        
        private void Update()
        {
            if(!_isDragging)
                SyncWithNativeContainer();
        }
        
        private void OnDisable() => BannerAd.Visible = false;

        public void OnDestroy() => BannerAd?.Dispose();
        #endregion

        #region Public API
        
        /// <summary>
        /// Loads an ad inside this gameobject.
        /// Uses the size of this gameobject (width and height in pixels) to construct the
        /// <see cref="Banner.BannerSize"/> in load request 
        /// </summary>
        /// <returns></returns>
        public async Task<BannerAdLoadResult> Load()
        {
            if (string.IsNullOrEmpty(placementName))
                return new BannerAdLoadResult(new ChartboostMediationError(Errors.ErrorNotReady));

            var transformSize = await GetTransformSize();
            var size = Banner.BannerSize.Adaptive(transformSize.x, transformSize.y);
            var loadRequest = new BannerAdLoadRequest(placementName, size);
            return await BannerAd.Load(loadRequest);
        }

        #region BannerAd Wrap
        /// <inheritdoc cref="IBannerAd.Keywords"/>
        public IReadOnlyDictionary<string, string> Keywords
        {
            get => BannerAd?.Keywords;
            set => BannerAd.Keywords = value;
        }

        /// <inheritdoc cref="IBannerAd.PartnerSettings"/>
        public IReadOnlyDictionary<string, string> PartnerSettings
        {
            get => BannerAd?.PartnerSettings;
            set => BannerAd.PartnerSettings = value;
        }

        /// <summary>
        /// The publisher supplied request that was used to load the ad.
        /// </summary>
        public BannerAdLoadRequest Request => BannerAd?.Request;

        /// <summary>
        /// The winning bid info for the ad. Note that this will change with auto-refresh and will be notified in <see cref="WillAppear"/>
        /// </summary>
        public BidInfo? WinningBidInfo => BannerAd.WinningBidInfo;

        /// <summary>
        /// The identifier for this load call. Note that this will change with auto-refresh and will be notified in <see cref="WillAppear"/>
        /// </summary>
        public string LoadId => BannerAd?.LoadId;
        
        /// <summary>
        /// The load metrics for the most recent successful load operation, or Null if a banner is not loaded.
        /// If auto-refresh is enabled, this value will change over time. The <see cref="WillAppear"/> event will be called after this value changes.
        /// </summary>
        public Metrics? LoadMetrics => BannerAd?.LoadMetrics;

        /// <summary>
        /// The size of the loaded ad. Note that this will change with auto-refresh and will be notified in <see cref="WillAppear"/>
        /// </summary>
        public BannerSize? BannerSize => BannerAd.BannerSize;

        /// <summary>
        /// The horizontal alignment of the ad within this gameobject.
        /// </summary>
        public BannerHorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment;
            set
            {
                if(BannerAd != null)
                    BannerAd.HorizontalAlignment = value;
                horizontalAlignment = value;
            }
        }
        
        /// <summary>
        /// The vertical alignment of the ad within this gameobject.
        /// </summary>
        public BannerVerticalAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set
            {
                if(BannerAd != null)
                    BannerAd.VerticalAlignment = value;
                verticalAlignment = value;
            }
        }
        
        /// <summary>
        /// Loads an ad inside this gameobject.
        /// </summary>
        /// <param name="loadRequest"></param>
        /// <returns></returns>
        public async Task<BannerAdLoadResult> Load(BannerAdLoadRequest loadRequest)
        {
            placementName = loadRequest.PlacementName;
            return await BannerAd.Load(loadRequest);
        }
        
        /// <summary>
        /// Clears the loaded ad
        /// </summary>
        public void Reset() => BannerAd?.Reset();
        
        #endregion

        /// <summary>
        /// Returns json representation of current state of the object
        /// </summary>
        public override string ToString() => JsonConvert.SerializeObject(BannerAd);
        
        #endregion

        #region Events
        private void OnWillAppear(IBannerAd bannerView) => WillAppear?.Invoke(this);

        private void OnRecordImpression(IBannerAd bannerView) => DidRecordImpression?.Invoke(this);

        private void OnClick(IBannerAd bannerView) => DidClick?.Invoke(this);

        private void OnDragBegin(IBannerAd bannerAdd, float x, float y)
        {
            _isDragging = true;
            DidBeginDrag?.Invoke(this, x, y);
        }

        private void OnDrag(IBannerAd bannerView, float x, float y)
        {
            if (!_isDragging)
            {
                LogController.Log("The DidDrag event was triggered, but no preceding DidDragBegin event was detected.", LogLevel.Debug);
                return;
            }
            
            y = Screen.height - y;
            
            // x,y obtained from native is for top left corner (x = 0,y = 1)
            // RectTransform pivot may or may not be top-left (it's usually at center)
            var pivot = UnityBannerTransform.pivot;
            var widthInPixels = UnityBannerTransform.LayoutParams().width;
            var heightInPixels = UnityBannerTransform.LayoutParams().height;
            x += widthInPixels * pivot.x;
            y -= heightInPixels * pivot.y;
            
            transform.position = new Vector3(x, y, 0);
            DidDrag?.Invoke(this, x, y);
        }
        
        private void OnDragEnd(IBannerAd bannerAd, float x, float y)
        {
            _isDragging = false;
            DidEndDrag?.Invoke(this, x, y);
        }
        #endregion

        private IBannerAd BannerAd
        {
            get
            {
                if (_bannerAd != null)
                    return _bannerAd;

                _bannerAd = ChartboostMediation.GetBannerAd();
                _bannerAd.WillAppear += OnWillAppear;
                _bannerAd.DidClick += OnClick;
                _bannerAd.DidRecordImpression += OnRecordImpression;
                _bannerAd.DidBeginDrag += OnDragBegin;
                _bannerAd.DidDrag += OnDrag;
                _bannerAd.DidEndDrag += OnDragEnd;

                _bannerAd.Visible = gameObject.activeSelf;
                _bannerAd.Draggable = Draggable;
                _bannerAd.HorizontalAlignment = horizontalAlignment;
                _bannerAd.VerticalAlignment = verticalAlignment;
                return _bannerAd;
            }
        }

        private async void SyncWithNativeContainer()
        {
            var layoutParams = UnityBannerTransform.LayoutParams();
            if (layoutParams.IsEqual(_lastLayoutParams))
                return;

            if (BannerAd != null)
            {
                // Position
                var x = DensityConverters.PixelsToNative(layoutParams.x);
                var y = DensityConverters.PixelsToNative(Screen.height - layoutParams.y);
                BannerAd.Position = new Vector2(x, y);

                // Size
                var size = await GetTransformSize();
                BannerAd.ContainerSize = ContainerSize.FixedSize((int)size.x, (int)size.y);
            }
            _lastLayoutParams = layoutParams;
        }
        
        private async Task<Vector2> GetTransformSize()
        {
            var layoutParams = UnityBannerTransform.LayoutParams();
            
            // Note : if rectTransform is part of a layoutgroup then we need to wait until the layout is created
            // https://forum.unity.com/threads/solved-cant-get-the-rect-width-rect-height-of-an-element-when-using-layouts.377953/
            if (UnityBannerTransform.GetComponentInParent<LayoutGroup>())
            {
                // Wait a couple of frames
                await Task.Yield();
                await Task.Yield();
                layoutParams = UnityBannerTransform.LayoutParams();
            }
            
            var width = DensityConverters.PixelsToNative(layoutParams.width);
            var height = DensityConverters.PixelsToNative(layoutParams.height);
            return new Vector2(width, height);
        }
    }
}
