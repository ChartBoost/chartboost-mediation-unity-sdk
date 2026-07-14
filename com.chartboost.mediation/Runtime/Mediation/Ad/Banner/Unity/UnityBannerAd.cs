using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost;
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
        /// <inheritdoc cref="IBannerAd.WillAppear"/>
        /// <remarks>
        /// Event type is <see cref="UnityBannerAdEvent"/> for Unity UI compatibility.
        /// </remarks>
        public event UnityBannerAdEvent WillAppear;

        /// <inheritdoc cref="IBannerAd.DidClick"/>
        /// <remarks>
        /// Event type is <see cref="UnityBannerAdEvent"/> for Unity UI compatibility.
        /// </remarks>
        public event UnityBannerAdEvent DidClick;

        /// <inheritdoc cref="IBannerAd.DidRecordImpression"/>
        /// <remarks>
        /// Event type is <see cref="UnityBannerAdEvent"/> for Unity UI compatibility.
        /// </remarks>
        public event UnityBannerAdEvent DidRecordImpression;

        /// <inheritdoc cref="IBannerAd.DidBeginDrag"/>
        /// <remarks>
        /// Event type is <see cref="UnityBannerAdDragEvent"/> for Unity UI compatibility.
        /// </remarks>
        public event UnityBannerAdDragEvent DidBeginDrag;

        /// <inheritdoc cref="IBannerAd.DidDrag"/>
        /// <remarks>
        /// Event type is <see cref="UnityBannerAdDragEvent"/> for Unity UI compatibility.
        /// </remarks>
        public event UnityBannerAdDragEvent DidDrag;

        /// <inheritdoc cref="IBannerAd.DidEndDrag"/>
        /// <remarks>
        /// Event type is <see cref="UnityBannerAdDragEvent"/> for Unity UI compatibility.
        /// </remarks>
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

        // Native banner backing this Unity variant, for automation slot hosting (HB-11391).
        internal IBannerAd NativeBannerAd => _bannerAd;

        // When hosted in the automation slot, drive the native container from these slot bounds
        // instead of this GameObject's RectTransform (parity with BannerVisualElement). The
        // RectTransform-driven sync settles slowly, which left the hosted banner blank for
        // seconds; the override applies the slot size/position immediately (HB-11391).
        public Vector2? ContainerPositionOverride { get; set; }
        public Vector2? ContainerSizeOverride { get; set; }
        private Vector2? _lastAppliedOverrideSize;
        private Vector2? _lastAppliedOverridePosition;

        private LayoutParams _lastLayoutParams = new();
        private RectTransform _rectTransform;
        private bool _isDragging;
        private bool _syncInProgress;
        
        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName
        {
            get => placementName;
            internal set => placementName = value;
        }
        
        /// <inheritdoc cref="IBannerAd.Draggable"/>
        public bool Draggable
        {
            get => draggable;
            set
            {
                draggable = value;
                if (_bannerAd != null)
                    _bannerAd.Draggable = value;
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

        private void OnEnable()
        {
            if (_bannerAd != null)
                _bannerAd.Visible = true;
        }

        private void Update()
        {
            if(!_isDragging && !_syncInProgress)
            {
                _syncInProgress = true;
                // Fire-and-forget pattern: start the async task without awaiting
                // We track the task to avoid overlapping calls with _syncInProgress flag
                // ContinueWithOnMainThread ensures execution on Unity main thread and handles exceptions
                SyncWithNativeContainerAsync().ContinueWithOnMainThread(task =>
                {
                    _syncInProgress = false;

                    if (task.IsFaulted && task.Exception != null)
                    {
                        LogController.LogException(task.Exception.InnerException ?? task.Exception);
                    }
                });
            }
        }

        private void OnDisable()
        {
            if (_bannerAd != null)
                _bannerAd.Visible = false;
        }

        public void OnDestroy()
        {
            UnsubscribeFromBannerEvents();
            BannerAd?.Dispose();
        }
        #endregion

        #region Public API
        
        /// <summary>
        /// Loads an ad inside this GameObject.
        /// Uses the size of this GameObject (width and height in pixels) to construct the
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
            set
            {
                if (BannerAd != null)
                    BannerAd.Keywords = value;
            }
        }

        /// <inheritdoc cref="IBannerAd.PartnerSettings"/>
        public IReadOnlyDictionary<string, string> PartnerSettings
        {
            get => BannerAd?.PartnerSettings;
            set
            {
                if (BannerAd != null)
                    BannerAd.PartnerSettings = value;
            }
        }

        /// <inheritdoc cref="IBannerAd.Request"/>
        public BannerAdLoadRequest Request => BannerAd?.Request;

        /// <inheritdoc cref="IBannerAd.WinningBidInfo"/>
        public BidInfo? WinningBidInfo => BannerAd?.WinningBidInfo;

        /// <inheritdoc cref="IBannerAd.LoadId"/>
        public string LoadId => BannerAd?.LoadId;

        /// <inheritdoc cref="IBannerAd.LoadMetrics"/>
        public Metrics? LoadMetrics => BannerAd?.LoadMetrics;

        /// <inheritdoc cref="IBannerAd.BannerSize"/>
        public BannerSize? BannerSize => BannerAd?.BannerSize;

        /// <inheritdoc cref="IBannerAd.HorizontalAlignment"/>
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

        /// <inheritdoc cref="IBannerAd.VerticalAlignment"/>
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
        
        /// <inheritdoc cref="IBannerAd.Load"/>
        public async Task<BannerAdLoadResult> Load(BannerAdLoadRequest loadRequest)
        {
            placementName = loadRequest.PlacementName;
            return await BannerAd.Load(loadRequest);
        }
        
        /// <inheritdoc cref="IBannerAd.Reset"/>
        public void Reset() => BannerAd?.Reset();
        
        #endregion

        /// <summary>
        /// Returns JSON representation of the object
        /// </summary>
        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(BannerAd, settings);
        }
        
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
            
            // x,y obtained from native is for the top left corner (x = 0,y = 1)
            // RectTransform pivot may or may not be top-left (it's usually at the center)
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
                _bannerAd.Draggable = draggable;
                _bannerAd.HorizontalAlignment = horizontalAlignment;
                _bannerAd.VerticalAlignment = verticalAlignment;
                return _bannerAd;
            }
        }

        private async Task SyncWithNativeContainerAsync()
        {
            try
            {
                // Hosted in the automation slot: drive the native container from the slot override
                // instead of the RectTransform, applied only when it changes (HB-11391).
                if (ContainerSizeOverride.HasValue || ContainerPositionOverride.HasValue)
                {
                    if (BannerAd != null)
                    {
                        if (ContainerSizeOverride.HasValue && ContainerSizeOverride != _lastAppliedOverrideSize)
                        {
                            BannerAd.ContainerSize = ContainerSize.FixedSize(
                                (int)ContainerSizeOverride.Value.x, (int)ContainerSizeOverride.Value.y);
                            _lastAppliedOverrideSize = ContainerSizeOverride;
                        }

                        if (ContainerPositionOverride.HasValue && ContainerPositionOverride != _lastAppliedOverridePosition)
                        {
                            BannerAd.Position = ContainerPositionOverride.Value;
                            _lastAppliedOverridePosition = ContainerPositionOverride;
                        }
                    }
                    return;
                }

                var layoutParams = UnityBannerTransform.LayoutParams();
                if (layoutParams.IsEqual(_lastLayoutParams))
                    return;

                if (BannerAd != null)
                {
                    var positionChanged = System.Math.Abs(layoutParams.x - _lastLayoutParams.x) >= 0.01f
                                       || System.Math.Abs(layoutParams.y - _lastLayoutParams.y) >= 0.01f;
                    var sizeChanged = System.Math.Abs(layoutParams.width - _lastLayoutParams.width) >= 0.01f
                                   || System.Math.Abs(layoutParams.height - _lastLayoutParams.height) >= 0.01f;

                    if (positionChanged)
                    {
                        var x = DensityConverters.PixelsToNative(layoutParams.x);
                        var y = DensityConverters.PixelsToNative(Screen.height - layoutParams.y);
                        BannerAd.Position = new Vector2(x, y);
                    }

                    if (sizeChanged)
                    {
                        var size = await GetTransformSize();
                        BannerAd.ContainerSize = ContainerSize.FixedSize((int)size.x, (int)size.y);
                    }
                }
                _lastLayoutParams = layoutParams;
            }
            catch (System.Exception ex)
            {
                LogController.Log($"Error syncing banner with native container: {ex.Message}", LogLevel.Error);
            }
        }
        
        private async Task<Vector2> GetTransformSize()
        {
            var layoutParams = UnityBannerTransform.LayoutParams();
            
            // Note: if rectTransform is part of a layout group, then we need to wait until the layout is created
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

        /// <summary>
        /// Unsubscribes from all banner ad events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromBannerEvents()
        {
            if (_bannerAd == null)
                return;

            _bannerAd.WillAppear -= OnWillAppear;
            _bannerAd.DidClick -= OnClick;
            _bannerAd.DidRecordImpression -= OnRecordImpression;
            _bannerAd.DidBeginDrag -= OnDragBegin;
            _bannerAd.DidDrag -= OnDrag;
            _bannerAd.DidEndDrag -= OnDragEnd;
        }
    }
}
