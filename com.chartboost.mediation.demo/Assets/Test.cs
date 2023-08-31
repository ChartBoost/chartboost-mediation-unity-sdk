using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost;
using Chartboost.AdFormats.Banner;
using Chartboost.Banner;
using Chartboost.Requests;
using Newtonsoft.Json;
using UnityEngine;

public class Test : MonoBehaviour
{
    private IChartboostMediationBannerView _bannerView;
    private void Start()
    {
        ChartboostMediation.StartWithOptions(ChartboostMediationSettings.AppId, ChartboostMediationSettings.AppSignature);
#if UNITY_EDITOR
        TestDemo("");
#else
        ChartboostMediation.DidStart += TestDemo;
        // ChartboostMediation.DidStart += TestDemoLegacy;
#endif
    }

    private void TestDemoLegacy(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError($" Chartboost SDK failed to initialize : {error}");
        }

        ChartboostMediation.DidLoadBanner += (placement, id, info, s) =>
        {
            Debug.Log($"DidLoadBanner => {placement}");
        };
        
        var bannerAd = ChartboostMediation.GetBannerAd("AllNetworkBanner", ChartboostMediationBannerAdSize.Standard);
        bannerAd.Load(ChartboostMediationBannerAdScreenLocation.Center);

    }
    
    private async void TestDemo(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError($" Chartboost SDK failed to initialize : {error}");
        }

        _bannerView = ChartboostMediation.GetBannerView();
        _bannerView.Keywords = new Dictionary<string, string> { { "foo", "bar" } };
        
        var request =
            new ChartboostMediationBannerAdLoadRequest("AllNetworkBanner", ChartboostMediationBannerAdSize.Leaderboard);

        _bannerView.WillAppear += view =>
        {
            // Debug.Log($" ON Banner Show : {JsonConvert.SerializeObject(view)}");
            Debug.Log($"winingBid : {JsonConvert.SerializeObject(view.WinningBidInfo)}");
            Debug.Log($"load metrics : {JsonConvert.SerializeObject(view.LoadMetrics)}");
            Debug.Log($" request : {JsonConvert.SerializeObject(view.Request)}");
            Debug.Log($"Keywords : {JsonConvert.SerializeObject(view.Keywords)}");
            Debug.Log($"Size Width : {view.AdSize.Width}, height : {view.AdSize.Height}");
            Debug.Log($"size json : {JsonConvert.SerializeObject( view.AdSize)}");
            Debug.Log($" Horizontal alignment of view {view} : {view.HorizontalAlignment}");
            Debug.Log($" Vertical alignment of view {view} : {view.VerticalAlignment}");
        };


        var result = await _bannerView.Load(request, ChartboostMediationBannerAdScreenLocation.Center);
        if (result.Error != null)
        {
            Debug.LogError($"Load Error : {result.Error?.Code} : {result.Error?.Message}");
        }
        
        Debug.Log($"load successful in test");

        // string json = JsonConvert.SerializeObject(_bannerView, Formatting.Indented);
        // Debug.Log(json);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _bannerView.HorizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Left;
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            _bannerView.HorizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Right;
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            _bannerView.VerticalAlignment = ChartboostMediationBannerVerticalAlignment.Top;
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            _bannerView.VerticalAlignment = ChartboostMediationBannerVerticalAlignment.Bottom;
        }
        
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            _bannerView.HorizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            _bannerView.VerticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        }
    }

    private void OnDestroy()
    {
        _bannerView.Reset();
    }
}
