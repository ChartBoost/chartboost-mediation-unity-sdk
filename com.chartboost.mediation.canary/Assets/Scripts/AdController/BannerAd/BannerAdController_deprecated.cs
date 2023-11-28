using System;
using Chartboost;
using Chartboost.Banner;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The controller for a specific banner ad placement.
/// </summary>
public class BannerAdController_deprecated : CanaryAdController
{
    /// <summary>
    /// The dropdown that consists of the possible banner sizes.
    /// </summary>
    public Dropdown sizeDropdown;

    /// <summary>
    /// The dropdown that consists of the possible locations on the screen
    /// to place the banner.
    /// </summary>
    public Dropdown locationDropdown;    

    /// <summary>
    /// The button that allows toggling between visiblity of banner
    /// </summary>
    public Button setInvisibleButton;

    /// <summary>
    /// The LayoutElement whose height is adjusted within vertical layout to
    /// accommodate Banner Ad
    /// </summary>
    private LayoutElement _bannerFlexibleSpace;

    /// <summary>
    /// The banner ad that is being managed by this controller.
    /// </summary>
    private ChartboostMediationBannerAd _bannerAd;

    /// <summary>
    /// Name of Banner Flexible Space LayoutElement gameobject 
    /// </summary>
    private string BannerFlexibleSpaceName = "_bannerFlexibleSpace";

    /// <summary>
    /// Current banner ad visibility status
    /// </summary>
    private bool _bannerAdIsVisible = true;		// default is visible

    /// <summary>
    /// Screenlocation where bannerAd is loaded and displayed
    /// </summary>
    private ChartboostMediationBannerAdScreenLocation _bannerScreenLocation;

    #region Sticky Banner

    /// <summary>
    /// The togggle that allows toggling between usage of sticky banner
    /// </summary>
    public Toggle stickyBannerToggle;

    /// <summary>
    /// Name of Sticky Banner Flexible Space LayoutElement gameobject 
    /// </summary>
    private static string StickyBannerFlexibleSpaceName = "_stickyBannerFlexibleSpace";

    /// <summary>
    /// The LayoutElement whose height is adjusted within vertical layout to
    /// accommodate Sticky Banner Ad
    /// </summary>
    private static LayoutElement _stickyBannerFlexibleSpace;

    /// <summary>
    /// Static banner Ad that persists throughout the session
    /// and can only be cleared by explicitly calling Remove or Clear
    /// </summary>
    private static ChartboostMediationBannerAd _stickyBannerAd;    

    /// <summary>
    /// Current state of Sticky Banner Ad
    /// </summary>
    private static bool _useStickyBanner = false;

    /// <summary>
    /// Screenlocation where StickyBannerAd is loaded and displayed
    /// </summary>
    private static ChartboostMediationBannerAdScreenLocation _stickyBannerScreenLocation;

    #endregion


    /// <inheritdoc cref="CanaryAdController.Configure"/>
    public override void Configure(AdControllerConfiguration configuration)
    {
        base.Configure(configuration);
        locationDropdown.SetValueWithoutNotify((int)ChartboostMediationBannerAdScreenLocation.TopCenter);
    }

    /// <inheritdoc cref="CanaryAdController.Awake"/>
    protected override void Awake()
    {
        base.Awake();
        ChartboostMediation.DidLoadBanner += DidLoad;
        ChartboostMediation.DidClickBanner += DidClick;
        ChartboostMediation.DidRecordImpressionBanner += DidRecordImpression;

        string[] callbacks = new string[] {
            CanaryConstants.Callbacks.DidLoad,
            CanaryConstants.Callbacks.DidClick,
            CanaryConstants.Callbacks.DidRecordImpression,
            CanaryConstants.Callbacks.DidReceiveILRD
        };
        callbackPanel.AddCallbacks(callbacks);

        stickyBannerToggle.onValueChanged.AddListener(isOn => _useStickyBanner = isOn);       
    }

    /// <summary>
    /// Standard Unity Destroy handler.
    /// </summary>
    protected override void OnDestroy()
    {
        _bannerAdIsVisible = false;      
        _bannerAd?.Destroy();
        ChartboostMediation.DidLoadBanner -= DidLoad;
        ChartboostMediation.DidClickBanner -= DidClick;
        ChartboostMediation.DidRecordImpressionBanner -= DidRecordImpression;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        stickyBannerToggle.isOn = _useStickyBanner;

        SetVisibility(true, _bannerAd);
    }

    protected override void OnDisable()
    {
        // Remove visibility only for regular banner( not sticky banner)
        SetVisibility(false, _bannerAd);
    }

    /// <inheritdoc cref="CanaryAdController.OnLoadButtonPushed"/>
    public override void OnLoadButtonPushed()
    {
        base.OnLoadButtonPushed();
        Load();
    }

    /// <inheritdoc cref="CanaryAdController.OnClearButtonPushed"/>
    public override void OnClearButtonPushed()
    {
        base.OnClearButtonPushed();
        Clear(BannerAd);
    }

    /// <inheritdoc cref="CanaryAdController.OnDestroyButtonPushed"/>
    public override void OnDestroyButtonPushed()
    {
        base.OnDestroyButtonPushed();

        if (BannerAd == null)
        {
            Log(HasNotBeenLoaded, null, LogType.Error);
            return;
        }

        SetVisibility(false, BannerAd);
        BannerAd?.Destroy();
        Log(HasBeenDestroyed);
    }

    /// <inheritdoc cref="CanaryAdController.OnToggleVisibilityButtonPushed"/>
    public override void OnToggleVisibilityButtonPushed()
    {
        if (BannerAd == null)
        {
            Log(ToggleVisibilityFailed);
            return;
        }

        _bannerAdIsVisible = !_bannerAdIsVisible;

        SetVisibility(_bannerAdIsVisible, BannerAd);
        Log(ToggleVisibility);

        setInvisibleButton.GetComponentInChildren<Text>().text = _bannerAdIsVisible ? "Set Invisible" : "Set Visible";
    }

    protected override void DidRecordImpression(string placementName, string error)
    {
        SetVisibility(true, BannerAd);
        _bannerAdIsVisible = true;
        base.DidRecordImpression(placementName, error);
    }

    /// <summary>
    /// Current active bannerAd that responds to UI buttons in this controlller
    /// </summary>
    private ChartboostMediationBannerAd BannerAd
    {
        get => _useStickyBanner ? _stickyBannerAd : _bannerAd;
        set
        {
            if (_useStickyBanner)
            {
                _stickyBannerAd = value;
            }
            else
            {
                _bannerAd = value;
            }
        }
    }

    private void Load()
    {       
        var size = sizeDropdown.value switch
        {
            2 => ChartboostMediationBannerAdSize.Leaderboard,
            1 => ChartboostMediationBannerAdSize.MediumRect,
            _ => ChartboostMediationBannerAdSize.Standard
        };        

        var screenPos = locationDropdown.value switch
        {
            0 => ChartboostMediationBannerAdScreenLocation.TopLeft,
            1 => ChartboostMediationBannerAdScreenLocation.TopCenter,
            2 => ChartboostMediationBannerAdScreenLocation.TopRight,
            3 => ChartboostMediationBannerAdScreenLocation.Center,
            4 => ChartboostMediationBannerAdScreenLocation.BottomLeft,
            5 => ChartboostMediationBannerAdScreenLocation.BottomCenter,
            6 => ChartboostMediationBannerAdScreenLocation.BottomRight,
            _ => ChartboostMediationBannerAdScreenLocation.TopCenter
        };
        

        var bannerAd = ChartboostMediation.GetBannerAd(controllerConfiguration.placementName, size);

        if (bannerAd == null)
        {
            Log(NotFound, null, LogType.Error);
            return;
        }

        foreach(var keyword in keywordsDataSource.Keywords)
            bannerAd?.SetKeyword(keyword.name, keyword.value);

        Log(RequestingLoad);
        
        bannerAd.Load(screenPos);

        // Set currently loaded ad to appropriate bannerAd object
        if (_useStickyBanner)
        {
            _stickyBannerAd = bannerAd;
            _stickyBannerScreenLocation = screenPos;
        }
        else
        {
            _bannerAd = bannerAd;
            _bannerScreenLocation = screenPos;
        }

        SetVisibility(true, bannerAd);
    }

    private void Clear(ChartboostMediationBannerAd bannerAd)
    {
        if (bannerAd == null)
        {
            Log(HasNotBeenLoaded);
            return;
        }

        SetVisibility(false, bannerAd);

        bannerAd.ClearLoaded();
        Log(HasBeenCleared);
    }

    private void SetVisibility(bool visible, ChartboostMediationBannerAd bannerAd)
    {
        if (bannerAd == null)        
            return;
        
        bannerAd.SetVisibility(visible);

        var flexibleSpace = bannerAd == _bannerAd ? BannerFlexibleSpace : StickyBannerFlexibleSpace;

        if (visible)
            CalculateFlexibleSpace();
        else
            flexibleSpace.minHeight = 0;

        void CalculateFlexibleSpace()
        {
            // Reset the height of flexible spaces to 0 if sticky banner and
            // regular banner both contest for same screen location (in y axis)
            Reset();
 
            var screenLocation = bannerAd == _bannerAd ? _bannerScreenLocation : _stickyBannerScreenLocation;

            // Top
            switch (screenLocation)
            {
                case ChartboostMediationBannerAdScreenLocation.TopCenter:
                case ChartboostMediationBannerAdScreenLocation.TopLeft:
                case ChartboostMediationBannerAdScreenLocation.TopRight:

                    // Applicable for any of the above three cases
                    CalculateSpace();
                    flexibleSpace.transform.SetAsFirstSibling();

                    break;
            }

            // Bottom
            switch (screenLocation)
            {
                case ChartboostMediationBannerAdScreenLocation.BottomCenter:
                case ChartboostMediationBannerAdScreenLocation.BottomLeft:
                case ChartboostMediationBannerAdScreenLocation.BottomRight:

                    // Applicable for any of the above three cases
                    CalculateSpace();
                    flexibleSpace.transform.SetAsLastSibling();

                    break;
            }

            void CalculateSpace()
            {
                var adSize = sizeDropdown.value switch
                {
                    2 => ChartboostMediationBannerAdSize.Leaderboard,
                    1 => ChartboostMediationBannerAdSize.MediumRect,
                    _ => ChartboostMediationBannerAdSize.Standard
                };

                var finalSize = adSize switch
                {
                    ChartboostMediationBannerAdSize.Leaderboard => 90,
                    ChartboostMediationBannerAdSize.MediumRect => 250,
                    ChartboostMediationBannerAdSize.Standard => 50,
                    _ => throw new ArgumentOutOfRangeException()
                };

                // unity UI to native scale factor
                const float scaleFactor = 2.5f;
                flexibleSpace.minHeight = finalSize * scaleFactor;
            }

            void Reset()
            {
                var bsl = (int)_bannerScreenLocation;
                var ssl = (int)_stickyBannerScreenLocation;
                if ((0 <= bsl && bsl <= 2) && ((0 <= ssl && ssl <= 2)) ||    // Top
                    (4 <= bsl && bsl <= 6) && ((4 <= ssl && ssl <= 6)))     // Bottom
                {
                    BannerFlexibleSpace.minHeight = 0;
                    StickyBannerFlexibleSpace.minHeight = 0;
                }
            }
        }
    }

    private LayoutElement BannerFlexibleSpace
    {
        get
        {
            if (_bannerFlexibleSpace == null)
            {
                _bannerFlexibleSpace = GameObject.Find(BannerFlexibleSpaceName)?.GetComponent<LayoutElement>();

                if(_bannerFlexibleSpace == null)
                {
                    // create
                    _bannerFlexibleSpace = new GameObject(BannerFlexibleSpaceName).AddComponent<LayoutElement>();
                    _bannerFlexibleSpace.transform.parent = GameObject.Find("Holder").transform;    // TODO: should use a variable instead ?
                    _bannerFlexibleSpace.transform.localScale = Vector3.one;
                }
            }

            return _bannerFlexibleSpace;
        }
    }

    private static LayoutElement StickyBannerFlexibleSpace
    {
        get
        {
            if (_stickyBannerFlexibleSpace == null)
            {
                _stickyBannerFlexibleSpace = GameObject.Find(StickyBannerFlexibleSpaceName)?.GetComponent<LayoutElement>();

                if(_stickyBannerFlexibleSpace == null)
                {
                    // Create
                    _stickyBannerFlexibleSpace = new GameObject(StickyBannerFlexibleSpaceName).AddComponent<LayoutElement>();
                    _stickyBannerFlexibleSpace.transform.parent = GameObject.Find("Holder").transform;  // TODO: Should use a variable instead ?
                    _stickyBannerFlexibleSpace.transform.localScale = Vector3.one;
                }
            }
            return _stickyBannerFlexibleSpace;
        }
    }
}
