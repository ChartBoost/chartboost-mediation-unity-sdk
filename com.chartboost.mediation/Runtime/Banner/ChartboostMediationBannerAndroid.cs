#if UNITY_ANDROID
using System;
using Chartboost.Platforms;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner object for Android.
    /// </summary>
    public class ChartboostMediationBannerAndroid : ChartboostMediationBannerBase
    {
        private readonly AndroidJavaObject _androidAd;

        public ChartboostMediationBannerAndroid(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size)
        {
            LogTag = "ChartboostMediationBanner (Android)";
            _androidAd = ChartboostMediationAndroid.plugin().Call<AndroidJavaObject>("getBannerAd", placementName, (int)size);
        }

        /// <inheritdoc cref="IChartboostMediationAd.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="IChartboostMediationAd.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="IChartboostMediationAd.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location)
        {
            base.Load(location);
            _androidAd.Call("load", (int)location);
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load(float, float, int, int)"/>>
        public override void Load(float x, float y, int width, int height)
        {
            base.Load(x, y, width, height);
            _androidAd.Call("load",x, Screen.height - y, width, height);    // Android measures pixels from top whereas Unity provides measurement from bottom of screen
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
        {
            base.SetVisibility(isVisible);
            _androidAd.Call("setBannerVisibility", isVisible);
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _androidAd.Call("clearLoaded");
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Remove"/>>
        public override void Remove()
        {
            //android doesn't have a remove method. Instead, calling destroy
            Destroy();
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetParams(float, float, int, int)"/>>
        public override void SetParams(float x, float y, int width, int height)
        {
            base.SetParams(x, y, width, height);
            _androidAd.Call("setParams", x, Screen.height - y, width, height);  // Android measures pixels from top whereas Unity provides measurement from bottom of screen
        }        

        public override void EnableDrag(Action<float, float> didDrag = null)
        {
            base.EnableDrag(didDrag);

            var dragListener = new BannerDragEventListener();
            dragListener.Init(didDrag);

            _androidAd.Call("enableDrag", dragListener);
        }

        public override void DisableDrag()
        {
            base.DisableDrag();

            _androidAd.Call("disableDrag");
        }
    }
}


[Preserve]
public class BannerDragEventListener : AndroidJavaProxy
{
    public BannerDragEventListener() : base("com.chartboost.mediation.unity.IBannerDragEventListener") { }

    //public static readonly BannerDragEventListener Instance = new BannerDragEventListener();


    private Action<float, float> _didDrag;
    public void Init(Action<float, float> didDrag)
    {
        _didDrag = didDrag;
    }

    [Preserve]
    private void DidDrag(float x, float y)
    {
        _didDrag?.Invoke(x, Screen.height - y);
    }
}
#endif
