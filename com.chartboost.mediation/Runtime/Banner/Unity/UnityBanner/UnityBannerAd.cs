using UnityEngine;
using UnityEngine.UI;
using Chartboost.Utilities;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

#if UNITY_EDITOR
#endif

namespace Chartboost.Banner.Unity
{
#if UNITY_ANDROID
    public enum EditorDPi
    {
        ldpi = 120,
        mdpi = 160,
        xdpi = 320,
        dpi420 = 420,
        xxdpi = 480,
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
    
    [RequireComponent(typeof(DragObject))]
    [RequireComponent(typeof(Image))]
    public partial class UnityBannerAd : MonoBehaviour
    {
        [Tooltip("Placement identifier for banner")]
        public string bannerPlacementName;

        [SerializeField] [Tooltip("Size of the banner")]
        private ChartboostMediationBannerAdSize _size;
        
        [SerializeField] [Tooltip("If enabled, this gameobject can be dragged on screen")]
        private bool _draggable = true;

        [SerializeField][Tooltip("If enabled, this gameobject will be visible on screen")]
        private bool _visualize = false;
        
        [Tooltip("Auto loads this ad after Chartboost Mediation SDK is initialized")]
        public bool autoLoadOnInit = true;

        [Header("Edit mode only")]
#if UNITY_ANDROID
        public EditorDPi referenceDpi = EditorDPi.dpi420;
#elif UNITY_IOS
    public EditorRetinaScale referenceRetinaScale = EditorRetinaScale.Three;
#endif

        private string TAG = "UnityBannerAd";

        private Canvas _canvas;
        private Image _visualizer;
        private DragObject _dragger;
        private RectTransform _rectTransform;
        private ChartboostMediationBannerAd _bannerAd;
        
        private bool _initialized = false;

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
            if(Application.isPlaying){}
            {
                Draggable = _draggable;
                Visualize = _visualize;
                AdjustSize();
            }
        }

        private void OnDestroy()
        {
            ChartboostMediation.DidStart -= DidStart;
            ChartboostMediation.DidLoadBanner -= DidLoadBanner;
        }

        # region Public API

        public ChartboostMediationBannerAdSize Size
        {
            get => _size;
            set
            {
                _size = value;
                AdjustSize();
            }
        }

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
        
        public void Init(ChartboostMediationBannerAd bannerAd)
        {
            _bannerAd = bannerAd;
            _initialized = true;
        }

        public void Load()
        {
            var layoutParams = _rectTransform.LayoutParams();
            BannerAd.Load(layoutParams.x, layoutParams.y, layoutParams.width, layoutParams.height);
        }

        public void SetKeyword(string keyword, string value) => BannerAd?.SetKeyword(keyword, value);
        
        public string RemoveKeyword(string keyword) => BannerAd?.RemoveKeyword(keyword);
        
        public void SetVisibility(bool isVisible) => BannerAd?.SetVisibility(isVisible);
        
        public void ClearLoaded() => BannerAd?.ClearLoaded();

        public void Remove() => BannerAd?.Remove();
        
        public void Destroy() => BannerAd?.Destroy();

        #endregion
        
        public void AdjustSize()
        {
            _rectTransform ??= GetComponent<RectTransform>();
            _rectTransform.sizeDelta = new Vector2(_size.GetDimensions().Item1 * ScalingFactor,
                _size.GetDimensions().Item2 * ScalingFactor);
        }
        
        private void DidStart(string error)
        {
            if (!string.IsNullOrEmpty(error))
                return;
            
            if (!_initialized)
            {
                Logger.Log(TAG,"Creating banner on placement: " + bannerPlacementName);
                var bannerAd = ChartboostMediation.GetBannerAd(bannerPlacementName, _size);

                if (bannerAd == null)
                {
                    Debug.Log("Banner Ad not found");
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

        private ChartboostMediationBannerAd BannerAd
        {
            get
            {
                if (_bannerAd == null)
                {
                    Logger.LogError(TAG,"Banner Ad is NULL");
                    return null;
                }
                
                if (!_initialized)
                {
                    Logger.LogError(TAG,"Banner Not Initialized");
                    return null;
                }

                return _bannerAd;
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