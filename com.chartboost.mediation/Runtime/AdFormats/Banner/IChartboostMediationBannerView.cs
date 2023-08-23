using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Results;

namespace Chartboost.AdFormats.Banner
{
    public delegate void ChartboostMediationBannerEvent(IChartboostMediationBannerView bannerView);
    
    public interface IChartboostMediationBannerView
    {
        abstract Dictionary<string, string> Keywords { get; set; }
        abstract ChartboostMediationBannerAdLoadRequest Request { get; }
        abstract BidInfo WinningBidInfo { get; }
        abstract Metrics? LoadMetrics { get; }
        abstract ChartboostMediationBannerSize Size { get; }
        abstract ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        abstract ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        abstract Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation);
        abstract void Reset();
        
        // ReSharper disable once InconsistentNaming
        abstract event ChartboostMediationBannerEvent WillAppear;
        abstract event ChartboostMediationBannerEvent DidClick;
        abstract event ChartboostMediationBannerEvent DidRecordImpression;
    }
}