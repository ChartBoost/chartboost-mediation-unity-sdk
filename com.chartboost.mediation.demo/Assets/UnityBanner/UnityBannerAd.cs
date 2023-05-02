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

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnityBannerAd : MonoBehaviour, IEndDragHandler
{

    public string bannerPlacementName;
    public ChartboostMediationBannerAdSize size;
    
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
        AdjustSize();
    }

    private void OnValidate()
    {
        AdjustSize();
    }

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

    public void OnEndDrag(PointerEventData eventData)
    {
        _bannerAd?.SetVisibility(true);

        LayoutParams layoutParams = _rectTransform.LayoutParams();
        _bannerAd.SetParams(layoutParams.x, layoutParams.y, layoutParams.width, layoutParams.height);
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