using System.Collections;
using System.Collections.Generic;
using Chartboost;
using Chartboost.Banner;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UnityBannerAd : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ChartboostMediationBannerAdSize size;
    public Text bannerPlacementName;

    private ChartboostMediationBannerAd _bannerAd;

    public void LoadBanner()
    {
        _bannerAd?.Remove();

        Debug.Log("Creating banner on placement: " + bannerPlacementName.text);
        _bannerAd = ChartboostMediation.GetBannerAd(bannerPlacementName.text, size);

        if (_bannerAd == null)
        {
            Debug.Log("Banner not found");
            return;
        }

        LayoutParams layoutParams = GetComponent<RectTransform>().LayoutParams();
        _bannerAd.Load(layoutParams.x, layoutParams.y, layoutParams.width, layoutParams.height);
    }        

    public void OnBeginDrag(PointerEventData eventData)
    {        
        
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LayoutParams layoutParams = GetComponent<RectTransform>().LayoutParams();
        _bannerAd.SetParams(layoutParams.x, layoutParams.y, layoutParams.width, layoutParams.height);
    }   
}

#if UNITY_EDITOR

[CustomEditor(typeof(UnityBannerAd))]
public class UnityBannerEditor : Editor
{
    private float _scalingFactor = 2.65f;
    private ChartboostMediationBannerAdSize _size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var banner = target as UnityBannerAd;

        _size = banner.size;

        var rt = banner.GetComponent<RectTransform>();
        switch (_size)
        {
            case ChartboostMediationBannerAdSize.Standard:
                rt.sizeDelta = new Vector2(320f * ScalingFactor, 50f * _scalingFactor);
                break;

            case ChartboostMediationBannerAdSize.MediumRect:
                rt.sizeDelta = new Vector2(300f * ScalingFactor, 250f * _scalingFactor);
                break;

            case ChartboostMediationBannerAdSize.Leaderboard:
                rt.sizeDelta = new Vector2(728f * ScalingFactor, 90f * _scalingFactor);
                break;

        }
    }


    // Note : This is a temporary hack/workaround until we have adaptive banners
    // Banner sizes are usually in `dp` but Unity works with pixels so there is no way of determining the
    // exact size of banner in pixels. This is why the hack
    private float ScalingFactor
    {
        get
        {
            var banner = target as UnityBannerAd;            
            float scaleFactor = 2.5f;

            var canvasScaler = banner.GetComponentInParent<CanvasScaler>();

            if (canvasScaler == null)
                return scaleFactor;


            Vector2 resolution;

            switch (canvasScaler.uiScaleMode)
            {
                case CanvasScaler.ScaleMode.ConstantPixelSize:
                    resolution = new Vector2(Screen.width, Screen.height);
                    break;
                case CanvasScaler.ScaleMode.ScaleWithScreenSize:
                    resolution = canvasScaler.referenceResolution;
                    break;
                default:
                    resolution = new Vector2(1920, 1080);    
                    break;
            }

            if (resolution.x <= 680)
            {
                scaleFactor = 1f;
            }
            else if (resolution.x > 680 && resolution.x <= 1344)
            {
                scaleFactor = 2f;
            }
            else if (resolution.x > 1344 && resolution.x <= 1920)
            {
                scaleFactor = 2.5f;
            }
            else if (resolution.x > 1920 && resolution.x <= 2400)
            {
                scaleFactor = 2.625f;
            }
            else if (resolution.x > 2400)
            {
                scaleFactor = 2.65f;
            }


            return scaleFactor;

        }
    }
}
#endif