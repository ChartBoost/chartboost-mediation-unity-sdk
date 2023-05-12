using System.Collections;
using System.Collections.Generic;
using Chartboost;
using Chartboost.Banner;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;
using Chartboost.Platforms;
using UnityEngine.Scripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class UnityBannerAd : MonoBehaviour
{
    [Tooltip("Placement identifier for banner")]
    public string bannerPlacementName;
    [Tooltip("Banner size")]
    public ChartboostMediationBannerAdSize size;
    [Tooltip("Auto loads this ad after Chartboost Mediation SDK is initialized")]
    public bool autoLoadOnInit = true;
    [SerializeField][Tooltip("If enabled, bannerAd can be dragged on screen")]
    private bool _draggable = true;

#if UNITY_ANDROID
    public EditorDPi referenceDpi = EditorDPi.dpi420;
#elif UNITY_IOS
    public EditorRetinaScale referenceRetinaScale = EditorRetinaScale.Three;
#endif

    private RectTransform _rectTransform;
    private ChartboostMediationBannerAd _bannerAd;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if(autoLoadOnInit)
            ChartboostMediation.DidStart += ChartboostMediation_DidStart;

        ChartboostMediation.DidLoadBanner += ChartboostMediation_DidLoadBanner;
        AdjustSize();
    }    

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            AdjustSize();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnBannerDrag(74.84229f, 2210.409f);
        }
    }
#endif

    public void LoadBanner()
    {
        _bannerAd?.Remove();

        Debug.Log("Creating banner on placement: " + bannerPlacementName);
        _bannerAd = ChartboostMediation.GetBannerAd(bannerPlacementName, size);

        if (_bannerAd == null)
        {
            Debug.Log("Banner not found");
            return;
        }

        LayoutParams layoutParams = _rectTransform.LayoutParams();
        _bannerAd.Load(layoutParams.x, layoutParams.y, layoutParams.width, layoutParams.height);

    }

    public bool Draggable
    {
        get
        {
            return _draggable;
        }

        set
        {
            _draggable = value;

            if (_draggable)
            {
                _bannerAd.EnableDrag(OnBannerDrag);
            }
            else
            {
                _bannerAd.DisableDrag();
            }

        }
    }

    private void ChartboostMediation_DidStart(string error)
    {
        LoadBanner();
    }

    private void ChartboostMediation_DidLoadBanner(string placement, string loadId, BidInfo bidInfo, string error)
    {
        // TODO: Currently this is the only way to associate bannerAd object with load callback.
        // In future, the load call will be an async call which can be awaited. All the code below 
        // can then be moved after the await of load call
        if (placement == bannerPlacementName)
        {
            if (_draggable)
            {
                Debug.Log($"Enabling drag on placement : {placement} bannerPlacementName : {bannerPlacementName}");
                _bannerAd.EnableDrag(OnBannerDrag);
            }
        }
    }

    private void OnBannerDrag(float x, float y)
    {
        Debug.Log($"Unity Drag X : {x}, Y: {y} bannerplacementName : {bannerPlacementName}");

        // Set x,y of this rect transform
        var rt = _rectTransform == null ? GetComponent<RectTransform>() : _rectTransform;


        var canvas = GetComponentInParent<Canvas>();

        // adjust x,y based on anchor position
        // TODO: Not handling the case when custom anchors are in use
        x -= Screen.width * rt.anchorMin.x;
        y -= Screen.height * rt.anchorMin.y;

        // convert in canvas scale
        x /= canvas.transform.localScale.x;
        y /= canvas.transform.localScale.x;


        Vector2 rectSize = new Vector2();
        switch (size)
        {
            case ChartboostMediationBannerAdSize.Standard:
                rectSize = new Vector2(320, 50);
                break;
            case ChartboostMediationBannerAdSize.MediumRect:
                rectSize = new Vector2(300, 250);
                break;
            case ChartboostMediationBannerAdSize.Leaderboard:
                rectSize = new Vector2(728, 90);
                break;           
        }

        // x,y obtained from native is for top left corner (x = 0,y = 1)
        // RectTransform pivot may or may not be top-left (it's usually at center)

        float newX = x + (rectSize.x * ScalingFactor * (rt.pivot.x - 0f));
        float newY = y + (rectSize.y * ScalingFactor * (rt.pivot.y - 1f));

        rt.anchoredPosition = new Vector2(newX, newY);
    }

    private void AdjustSize()
    {
        var rt = _rectTransform == null ? GetComponent<RectTransform>() : _rectTransform;

        switch (size)
        {
            case ChartboostMediationBannerAdSize.Standard:
                rt.sizeDelta = new Vector2(320f * ScalingFactor, 50f * ScalingFactor);
                break;

            case ChartboostMediationBannerAdSize.MediumRect:
                rt.sizeDelta = new Vector2(300f * ScalingFactor, 250f * ScalingFactor);
                break;

            case ChartboostMediationBannerAdSize.Leaderboard:
                rt.sizeDelta = new Vector2(728f * ScalingFactor, 90f * ScalingFactor);
                break;

        }
    }

    // Note : This is a temporary hack/workaround until we have adaptive banners
    // Banner sizes are usually in `dp` or `points` but Unity works with pixels so there is no way of determining the
    // exact size of banner in pixels. This is why the hack
    private float ScalingFactor
    {
        get
        {
            float scaleFactor = 2.5f;
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

//#if UNITY_EDITOR

//[CustomEditor(typeof(UnityBannerAd))]
//[CanEditMultipleObjects]
//public class UnityBannerEditor : Editor
//{
//    override on

//    public void OnEnable()
//    {
//        var banner = target as UnityBannerAd;
//        banner.AdjustSize();
//    }
//}
//#endif