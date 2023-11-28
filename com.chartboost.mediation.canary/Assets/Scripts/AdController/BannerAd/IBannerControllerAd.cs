using System.Threading.Tasks;
using Chartboost;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Requests;
using UnityEngine;

namespace AdController.BannerAd
{
    public delegate void BannerControllerAdEvent(IBannerControllerAd bannerControllerAd);
    
    /// <summary>
    /// Interface for BannerController's banner ad consisting of methods that
    /// correspond to banner controller's UI  
    /// </summary>
    public interface IBannerControllerAd
    {
        /// <summary>
        /// Event triggered when banner ad is loaded.
        /// </summary>
        event BannerControllerAdEvent DidLoad;
        
        /// <summary>
        /// Event triggered when banner ad is clicked.
        /// </summary>
        event BannerControllerAdEvent DidClick;
        
        /// <summary>
        /// Event triggered when banner ad recorded an impression.
        /// </summary>
        event BannerControllerAdEvent DidRecordImpression;
        
        /// <summary>
        /// The request that was used to load the banner ad.
        /// </summary>
        ChartboostMediationBannerAdLoadRequest Request { get; }
        
        /// <summary>
        /// The identifier for this load call. 
        /// </summary>
        string LoadId { get; }
        
        /// <summary>
        /// The winning bid info for the ad. 
        /// </summary>
        BidInfo? BidInfo { get; }
        
        /// <summary>
        /// The size of the container view loading the ad.
        /// </summary>
        ChartboostMediationBannerSize? ContainerSize { get; }
        
        /// <summary>
        /// The size of the ad view within the container.
        /// </summary>
        ChartboostMediationBannerSize? AdSize { get; }

        // Metrics? Metrics { get; }
        
        /// <summary>
        /// Loads a new banner ad within the provided container.
        /// </summary>
        /// <param name="loadRequest"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest loadRequest, RectTransform container);
        
        /// <summary>
        /// Loads a new banner ad at the provided screen location.
        /// </summary>
        /// <param name="loadRequest"></param>
        /// <param name="screenLocation"></param>
        /// <returns></returns>
        Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest loadRequest, ChartboostMediationBannerAdScreenLocation screenLocation);
        
        /// <summary>
        /// Resets the currently loaded banner ad.
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Destroys the currently loaded banner ad.
        /// </summary>
        void Destroy();
        
        /// <summary>
        /// Sets keywords on the currently loaded banner ad.
        /// </summary>
        /// <param name="keywords"></param>
        void SetKeywords(Keyword[] keywords);
        
        /// <summary>
        /// Sets horizontal alignment of currently loaded banner ad.
        /// </summary>
        /// <param name="horizontalAlignment"></param>
        void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment);
        
        /// <summary>
        /// Sets vertical alignment of currently loaded banner ad.
        /// </summary>
        /// <param name="verticalAlignment"></param>
        void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment);
        
        /// <summary>
        /// Sets background color visibility of banner ad container, if available. 
        /// </summary>
        /// <param name="isVisible"></param>
        void SetVisibility(bool isVisible);
        
        /// <summary>
        /// Sets the ability to drag the currently loaded banner ad.
        /// </summary>
        /// <param name="canDrag"></param>
        void SetDraggability(bool canDrag);
        
        /// <summary>
        /// Sets resize option of currently loaded banner ad.
        /// </summary>
        /// <param name="resizeOption"></param>
        void SetResizeOption(ResizeOption resizeOption);
        
        /// <summary>
        /// Toggles background color visibility of banner ad container, if available. 
        /// </summary>
        /// <param name="isVisible"></param>
        void ToggleBackgroundColorVisibility(bool isVisible);
    }

}
