using System.Collections;
using System.Collections.Generic;
using Chartboost;
using UnityEngine;

public class UnityBannerDemo : MonoBehaviour
{
    public UnityBannerAd[] bannerAds;

    private int _index;

    private void Start()
    {
        ChartboostMediation.DidStart += ChartboostMediation_DidStart;
    }

    public void OnToggleDrag(bool drag)
    {
        bannerAds[_index].Draggable = drag;

        if (drag)
        {
            _index++;
        }

        _index %= bannerAds.Length;
    }

    private void ChartboostMediation_DidStart(string error)
    {
        foreach (var bannerAd in bannerAds)
        {
            bannerAd.LoadBanner();
        }
    }
}
