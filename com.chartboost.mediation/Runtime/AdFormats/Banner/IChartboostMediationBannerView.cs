using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using UnityEngine;

namespace Chartboost.AdFormats.Banner
{
    /// <summary>
    /// <param name="bannerView"> The bannerView that triggers the event</param>
    /// </summary>
    public delegate void ChartboostMediationBannerEvent(IChartboostMediationBannerView bannerView);
    
    /// <summary>
    /// <param name="bannerView"> The bannerView that triggers the drag event</param>
    /// </summary>
    public delegate void ChartboostMediationBannerDragEvent(IChartboostMediationBannerView bannerView, float x, float y);
    
    /// <summary>
    /// The horizontal alignment enum.
    /// </summary>
    public enum ChartboostMediationBannerHorizontalAlignment
    {
        Left,
        Center,
        Right
    }
    
    /// <summary>
    /// The vertical alignment enum.
    /// </summary>
    public enum ChartboostMediationBannerVerticalAlignment
    {
        Top,
        Center,
        Bottom
    }
    
    /// <summary>
    /// Base public interface for BannerView API.
    /// </summary>
    public interface IChartboostMediationBannerView
    {
        /// <summary>
        /// The keywords targeted for the ad.
        /// </summary>
        abstract Dictionary<string, string> Keywords { get; set; }
        
        /// <summary>
        /// The publisher supplied request that was used to load the ad.
        /// </summary>
        abstract ChartboostMediationBannerAdLoadRequest Request { get; }
        
        /// <summary>
        /// The winning bid info for the ad. Note that this will change with auto-refresh and will be notified in <see cref="DidLoad"/>>
        /// </summary>
        abstract BidInfo WinningBidInfo { get; }
        
        /// <summary>
        /// The identifier for this load call. Note that this will change with auto-refresh and will be notified in <see cref="DidLoad"/>>
        /// </summary>
        abstract string LoadId { get; }
        
        /// <summary>
        /// The size of the loaded ad. Note that this will change with auto-refresh and will be notified in <see cref="DidLoad"/>>
        /// </summary>
        abstract ChartboostMediationBannerSize? AdSize { get; }
        
        /// <summary>
        /// The horizontal alignment of the ad within its container.
        /// </summary>
        abstract ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        
        /// <summary>
        /// The vertical alignment of the ad within its container.
        /// </summary>
        abstract ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        
        /// <summary>
        /// Loads an ad at the specified screen-location.
        /// </summary>
        /// <param name="request"> The ChartboostMediationBannerAdLoadRequest request</param>
        /// <param name="screenLocation"> pre-defined location on screen </param>
        /// <returns></returns>
        abstract Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation);
        
        /// <summary>
        /// Loads an ad at a custom screen location
        /// </summary>
        /// <param name="request"> The ChartboostMediationBannerAdLoadRequest request</param>
        /// <param name="x"> x co-ordinate of top-left corner of bannerView container in native unit</param>
        /// <param name="y"> y co-ordinate of top-left corner of bannerView container in native unit</param>
        /// <returns></returns>
        abstract Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y);
        
        /// <summary>
        /// Resizes the container at the specified axis to fit the loaded ad 
        /// </summary>
        /// <param name="axis"> The axis at which the container will be resized to fit the loaded ad </param>
        /// <param name="pivot"> The pivot within the container around which the container will be resized</param>
        abstract void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both, Vector2 pivot = default);
        
        /// <summary>
        /// Sets the drag capability of the container
        /// </summary>
        /// <param name="canDrag"></param>
        abstract void SetDraggability(bool canDrag);
        
        /// <summary>
        /// Sets the visibility of container
        /// </summary>
        /// <param name="visibility"></param>
        abstract void SetVisibility(bool visibility);
        
        /// <summary>
        /// Clears the loaded ad
        /// </summary>
        abstract void Reset();
        
        /// <summary>
        /// Destroys the ad along with its container
        /// </summary>
        abstract void Destroy();
        
        /// <summary>
        /// Called when ad is loaded within it's container. This will be called for each refresh when auto-refresh is enabled.
        /// </summary>
        abstract event ChartboostMediationBannerEvent DidLoad;
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        abstract event ChartboostMediationBannerEvent DidClick;
        
        /// <summary>
        /// Called when the ad impression occurs. This signal is when Chartboost Mediation fires an impression and is independent of any partner impression.
        /// </summary>
        abstract event ChartboostMediationBannerEvent DidRecordImpression;
        
        /// <summary>
        /// Called when the ad container is dragged on screen.
        /// </summary>
        abstract event ChartboostMediationBannerDragEvent DidDrag;
    }
}
