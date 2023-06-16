using UnityEngine;
using UnityEngine.UI;
using Chartboost.Utilities;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

#if UNITY_EDITOR
#endif

namespace Chartboost.Banner.Unity
{
    #region Editor Mode Only

    #if UNITY_ANDROID
    public enum EditorDPi
    {
        ldpi = 120,
        hdpi = 240,
        mdpi = 160,
        xhdpi = 320,
        dpi420 = 420,
        dpi440 = 440,
        xxhdpi = 480,
        dpi560 = 560,
        xxxhdpi = 640
    }
    #endif

    #if UNITY_IOS
    public enum EditorRetinaScale
    {
        One = 1,
        Two = 2,
        Three = 3
    }
    #endif
    
    #endregion
    
    
    /// <summary>
    /// A Wrapper MonoBehaviour for <see cref="ChartboostMediationBannerAd"/> with Drag and Visualization capabilities
    /// </summary>
    [RequireComponent(typeof(DragObject))]
    [RequireComponent(typeof(Image))]
    public partial class UnityBannerAd : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("Placement identifier for banner")]
        public string bannerPlacementName;

        [SerializeField] [Tooltip("Size of the banner")]
        private ChartboostMediationBannerAdSize _size;
        
        [SerializeField] [Tooltip("If enabled, this Gameobject can be dragged on screen")]
        private bool _draggable = true;

        [SerializeField][Tooltip("If enabled, the rect of this Gameobject's RectTransform will be visible on screen")]
        private bool _visualize = false;
        
        [Tooltip("Auto loads this ad after Chartboost Mediation SDK is initialized")]
        public bool autoLoadOnInit = true;

        [Header("Edit mode only")]
#if UNITY_ANDROID
        public EditorDPi referenceDpi = EditorDPi.dpi420;
#elif UNITY_IOS
    public EditorRetinaScale referenceRetinaScale = EditorRetinaScale.Three;
#endif

        private const string LogTag = "UnityBannerAd";

        private Canvas _canvas;
        private Image _visualizer;
        private DragObject _dragger;
        private RectTransform _rectTransform;
        private ChartboostMediationBannerAd _bannerAd;
        
        private bool _initialized = false;

        #region Unity Lifecycle Events

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _dragger = GetComponent<DragObject>();
            _visualizer = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();

            ChartboostMediation.DidStart += DidStart;
            ChartboostMediation.DidLoadBanner += DidLoadBanner;
        }

        private void Start()
        {
            Draggable = _draggable;
            Visualize = _visualize;
            AdjustSize();
        }

        private void OnDestroy()
        {
            ChartboostMediation.DidStart -= DidStart;
            ChartboostMediation.DidLoadBanner -= DidLoadBanner;
        }

        #endregion

        # region Public API

        /// <summary>
        /// Size of the Banner Ad
        /// </summary>
        public ChartboostMediationBannerAdSize Size
        {
            get => _size;
            set
            {
                _size = value;
                AdjustSize();
            }
        }

        /// <summary>
        /// Enables/Disables dragging capabilities of Banner Ad
        /// </summary>
        public bool Draggable
        {
            get => _draggable;
            set
            {
                _draggable = value;
                _dragger.enabled = value;

                if (_draggable)
                {
                    BannerAd?.EnableDrag(OnBannerDrag);
                }
                else
                {
                    BannerAd?.DisableDrag();
                }
            }
        }

        /// <summary>
        /// Enables/Disables visibility of the rect of this Gameobject's RectTransform
        /// </summary>
        public bool Visualize
        {
            get => _visualize;
            set
            {
                _visualize = value;
                _visualizer ??= GetComponent<Image>();
                _visualizer.enabled = value;
            }
        }
        
        /// <summary>
        /// Initializes UnityBannerAd
        /// </summary>
        /// <param name="bannerAd"> <see cref="ChartboostMediationBannerAd"/> ad object  </param>
        public void Init(ChartboostMediationBannerAd bannerAd)
        {
            _bannerAd = bannerAd;
            _initialized = true;
        }

        /// <summary>
        /// Loads a banner ad inside this gameobject
        /// </summary>
        public void Load()
        {
            if (!_initialized)
            {
                Logger.LogError(LogTag, "Cannot load ! UnityBannerAd not initialized");
                return;
            }
            
            var layoutParams = _rectTransform.LayoutParams();
            BannerAd.Load(layoutParams.x, layoutParams.y, layoutParams.width, layoutParams.height);
        }
        
        /// <summary>
        /// Calculates and sets width and height of this Gameobject based on its size />
        /// </summary>
        public void AdjustSize()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            
            _rectTransform.sizeDelta = new Vector2(Size.GetDimensions().Item1 * ScalingFactor,
                Size.GetDimensions().Item2 * ScalingFactor);
        }

        /// <summary>
        /// Set a keyword/value pair on the advertisement. If the keyword has previously been
        /// set, then the value will be replaced with the new value.  These values will be
        /// used upon the loading of the advertisement.
        /// </summary>
        /// <param name="keyword">The keyword (maximum of 64 characters)</param>
        /// <param name="value">The value (maximum of 256 characters)</param>
        /// <returns>true if the keyword was successfully set, else false</returns>
        public void SetKeyword(string keyword, string value) => BannerAd?.SetKeyword(keyword, value);
        
        /// <summary>
        /// Remove a keyword from the advertisement.
        /// </summary>
        /// <param name="keyword">The keyword to remove.</param>
        /// <returns>The currently set value, else null</returns>
        public string RemoveKeyword(string keyword) => BannerAd?.RemoveKeyword(keyword);
        
        /// <summary>This method changes the visibility of the banner ad.</summary>
        /// <param name="isVisible">Specify if the banner should be visible.</param>
        public void SetVisibility(bool isVisible) => BannerAd?.SetVisibility(isVisible);
        
        /// <summary>
        /// If an advertisement has been loaded, clear it. Once cleared, a new
        /// load can be performed.
        /// </summary>
        public void ClearLoaded() => BannerAd?.ClearLoaded();

        /// <summary>
        /// Remove the banner.
        /// </summary>
        public void Remove() => BannerAd?.Remove();
        
        /// <summary>
        /// Destroy the advertisement to free up memory resources.
        /// </summary>
        public void Destroy() => BannerAd?.Destroy();

        #endregion

        #region Callbacks

        private void DidStart(string error)
        {
            if (!string.IsNullOrEmpty(error))
                return;
            
            if (!_initialized)
            {
                Logger.Log(LogTag,"Creating banner on placement: " + bannerPlacementName);
                var bannerAd = ChartboostMediation.GetBannerAd(bannerPlacementName, _size);

                if (bannerAd == null)
                {
                    Logger.LogError(LogTag,"Banner Ad not found");
                    return;
                }

                Init(bannerAd);
            }

            if (autoLoadOnInit)
                Load();
        }

        private void DidLoadBanner(string placement, string loadId, BidInfo bidInfo, string error)
        {
            if (!string.IsNullOrEmpty(error))
                return;
            
            // TODO: Currently this is the only way to associate bannerAd object with load callback.
            // In future, the load call will be an async call which can be awaited. All the code below 
            // can then be moved after the await of load call
            if (placement == bannerPlacementName)
            {
                // Disable Visualize since it will be of no use after ad is loaded
                Visualize = false;
                
                if (_draggable)
                {
                    BannerAd?.EnableDrag(OnBannerDrag);
                }
            }
        }

        private void OnBannerDrag(float x, float y)
        {
            _canvas ??= GetComponentInParent<Canvas>();
            var rt = _rectTransform == null ? GetComponent<RectTransform>() : _rectTransform;

            // adjust x,y based on anchor position
            // TODO: Not handling the case when custom anchors are in use (i.e when anchorMin and anchorMax are not equal)
            if (rt.anchorMin == rt.anchorMax)
            {
                var anchor = rt.anchorMin;
                x -= Screen.width * anchor.x;
                y -= Screen.height * anchor.y;
            }

            // convert in canvas scale
            var canvasScale = _canvas.transform.localScale;
            x /= canvasScale.x;
            y /= canvasScale.x;

            // x,y obtained from native is for top left corner (x = 0,y = 1)
            // RectTransform pivot may or may not be top-left (it's usually at center)
            var pivot = rt.pivot;
            var newX = x + (_size.GetDimensions().Item1 * ScalingFactor * (pivot.x - 0f)); // top-left x is 0
            var newY = y + (_size.GetDimensions().Item2 * ScalingFactor * (pivot.y - 1f)); // top-left y is 1

            rt.anchoredPosition = new Vector2(newX, newY);
        }

        #endregion
        
        private ChartboostMediationBannerAd BannerAd
        {
            get
            {
                if (_bannerAd == null)
                {
                    Logger.LogError(LogTag,"Banner Ad is NULL");
                    return null;
                }
                
                if (!_initialized)
                {
                    Logger.LogError(LogTag,"Banner Not Initialized");
                    return null;
                }

                return _bannerAd;
            }
        }

        // Note : This is a temporary hack/workaround until we have adaptive banners
        // Banner sizes are usually in `dp` or `points` but Unity works with pixels so there is no way of determining the
        // exact size of banner in pixels. This is why the hack
        private float ScalingFactor
        {
            get
            {
                var scaleFactor = 2.5f;
                var canvasScaler = GetComponentInParent<CanvasScaler>();
                if (canvasScaler == null)
                    return scaleFactor;

#if UNITY_EDITOR
#if UNITY_IOS
            scaleFactor = (float)referenceRetinaScale;
#elif UNITY_ANDROID
                scaleFactor = (float)referenceDpi / 160f;
#endif
#else
            scaleFactor = ChartboostMediation.GetUIScaleFactor();
#endif
                if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    scaleFactor /= canvasScaler.transform.localScale.x;
                }

                return scaleFactor;
            }
        }
    }
}