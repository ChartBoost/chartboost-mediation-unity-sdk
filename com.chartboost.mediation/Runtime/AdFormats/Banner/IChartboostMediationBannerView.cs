using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Results;
using UnityEngine;

namespace Chartboost.AdFormats.Banner
{
    public delegate void ChartboostMediationBannerEvent(IChartboostMediationBannerView bannerView);
    public delegate void ChartboostMediationBannerDragEvent(IChartboostMediationBannerView bannerView, float x, float y);
    
    public enum ChartboostMediationBannerHorizontalAlignment
    {
        Left,
        Center,
        Right
    }
    
    public enum ChartboostMediationBannerVerticalAlignment
    {
        Top,
        Center,
        Bottom
    }
    
    public interface IChartboostMediationBannerView
    {
        abstract Dictionary<string, string> Keywords { get; set; }
        abstract ChartboostMediationBannerAdLoadRequest Request { get; }
        abstract BidInfo WinningBidInfo { get; }
        abstract string LoadId { get; }
        abstract ChartboostMediationBannerAdSize AdSize { get; }
        abstract ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        abstract ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        abstract Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation);
        abstract Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y);
        abstract void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both, Vector2 pivot = default);
        abstract void SetDraggability(bool canDrag);
        abstract void SetVisibility(bool visibility);
        abstract void Reset();
        abstract void Destroy();
        
        // ReSharper disable once InconsistentNaming
        abstract event ChartboostMediationBannerEvent WillAppear;
        abstract event ChartboostMediationBannerEvent DidClick;
        abstract event ChartboostMediationBannerEvent DidRecordImpression;
        abstract event ChartboostMediationBannerDragEvent DidDrag;

    }
}