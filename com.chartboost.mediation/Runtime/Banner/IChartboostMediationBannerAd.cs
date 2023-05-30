using System;
using UnityEngine.Scripting;

namespace Chartboost.Banner
{
    /// <summary>
    /// Interface implemented by all Chartboost Mediation banners. 
    /// </summary>
    public interface IChartboostMediationBannerAd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        [Preserve]
        void Load(ChartboostMediationBannerAdScreenLocation location);
        
        /// <summary>
        /// Loads the banner ad at specified position and size
        /// </summary>
        /// <param name="x"> Distance from left of screen in pixels </param>
        /// <param name="y"> Distance from bottom of screen in pixels </param>
        /// <param name="width"> width in pixels </param>
        /// <param name="height"> height in pixels </param>
        [Preserve]
        void Load(float x, float y, int width, int height); // use 2 Vector2(position and size) instead ?

        /// <summary>This method changes the visibility of the banner ad.</summary>
        /// <param name="isVisible">Specify if the banner should be visible.</param>
        [Preserve]
        void SetVisibility(bool isVisible);
        
        /// <summary>
        /// If an advertisement has been loaded, clear it. Once cleared, a new
        /// load can be performed.
        /// </summary>
        [Preserve]
        void ClearLoaded();
        
        /// <summary>
        /// Remove the banner.
        /// </summary>
        [Preserve]
        void Remove();

        /// <summary>
        /// Enables dragging of this banner ad object
        /// </summary>
        /// <param name="onDrag"> Callback that receives screen coordinates of ad object, when dragged</param>
        [Preserve]
        void EnableDrag(Action<float, float> onDrag = null);

        /// <summary>
        /// Disables dragging of this banner ad object
        /// </summary>
        [Preserve]
        void DisableDrag();
    }
}
