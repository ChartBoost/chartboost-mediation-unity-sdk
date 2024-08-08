using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using UnityEngine;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Defines the contract for a UI container that manages and displays banner advertisements.
    /// </summary>
    public interface IBannerAd : IAd, IDisposable
    {
        /// <summary>
        /// The keywords targeted for the ad.
        /// </summary>
        IReadOnlyDictionary<string, string> Keywords { get; set; }

        /// <summary>
        /// Optional partner-specific settings that can be associated with the advertisement placement.
        /// </summary>
        IReadOnlyDictionary<string, string> PartnerSettings { get; set; }
        
        /// <summary>
        /// The position of the <see cref="Pivot"/> of this container on the screen in the Screen Coordinate System.
        /// X-Axis: The horizontal axis, with positive values increasing to the right.
        /// Y-Axis: The vertical axis, with positive values increasing downward.
        /// </summary>
        Vector2 Position { get; set; }
        
        /// <summary>
        /// Gets or sets the Pivot property, which defines the normalized pivot point of this container within the screen coordinate system 
        /// from which all transformations and layout computations, such as width and height, are based.
        /// This property functions similarly to the pivot in Unity's RectTransform
        /// - X-Axis: Horizontal position within the container, normalized to the container's width. 
        ///   A value of 0 aligns the pivot with the left edge, while a value of 1 aligns with the right edge.
        /// - Y-Axis: Vertical position within the container, normalized to the container's height.
        ///   A value of 0 aligns the pivot with the top edge, while a value of 1 aligns with the bottom edge.
        /// Examples:
        /// - (0, 0): Pivot at the top-left corner of the container.
        /// - (0.5, 0.5): Pivot at the center of the container.
        /// - (1, 1): Pivot at the bottom-right corner of the container.
        /// </summary>
        Vector2 Pivot { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating the visibility of the container.
        /// </summary>
        bool Visible { get; set;}
        
        /// <summary>
        /// Gets or sets a value indicating whether this container can be dragged on the screen.
        /// </summary>
        bool Draggable { get; set; }
        
        /// <summary>
        /// Gets or sets the size of the container.
        /// </summary>
        ContainerSize ContainerSize { get; set; }
        
        /// <summary>
        /// Gets the size of the banner ad loaded inside this container.
        /// Note that this will change with auto-refresh and will be notified in <see cref="WillAppear"/>
        /// </summary>
        BannerSize? BannerSize { get; }

        /// <summary>
        /// The publisher supplied request that was used to load the ad.
        /// </summary>
        BannerAdLoadRequest Request { get; }
        
        /// <summary>
        /// The winning bid info for the ad. Note that this will change with auto-refresh and will be notified in <see cref="WillAppear"/>
        /// </summary>
        BidInfo WinningBidInfo { get; }
        
        /// <summary>
        /// The identifier for this load call. Note that this will change with auto-refresh and will be notified in <see cref="WillAppear"/>
        /// </summary>
        string LoadId { get; }
        
        /// <summary>
        /// The load metrics for the most recent successful load operation, or Null if a banner is not loaded.
        /// If auto-refresh is enabled, this value will change over time. The <see cref="WillAppear"/> event will be called after this value changes.
        /// </summary>
        Metrics? LoadMetrics { get; }

        /// <summary>
        /// The horizontal alignment of the ad within its container.
        /// </summary>
        BannerHorizontalAlignment HorizontalAlignment { get; set; }
        
        /// <summary>
        /// The vertical alignment of the ad within its container.
        /// </summary>
        BannerVerticalAlignment VerticalAlignment { get; set; }
        
        /// <summary>
        /// Loads an ad.
        /// </summary>
        /// <param name="request"> The ChartboostMediationBannerAdLoadRequest request</param>
        /// <returns></returns>
        Task<BannerAdLoadResult> Load(BannerAdLoadRequest request);

        /// <summary>
        /// Clears the loaded ad
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Disposes the ad along with its container. This should be called when the ad is no longer needed.
        /// </summary>
        new void Dispose();
        
        /// <summary>
        /// Called when ad is loaded within its container. This will be called for each refresh when auto-refresh is enabled.
        /// </summary>
        event BannerAdEvent WillAppear;
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        event BannerAdEvent DidClick;
        
        /// <summary>
        /// Called when the ad impression occurs.
        /// </summary>
        event BannerAdEvent DidRecordImpression;
        
        /// <summary>
        /// Called when the ad container is dragged on screen.
        /// </summary>
        event BannerAdDragEvent DidDrag;
    }
}
