namespace Chartboost.FullScreen
{
    /// <summary>
    /// Interface implemented by Chartboost Mediation fullscreen ads. 
    /// </summary>
    public interface IChartboostMediationFullScreenAdOld
    {
        /// <summary>
        /// Load the advertisement.
        /// </summary>
        void Load();

        /// <summary>
        /// Show a previously loaded advertisement.
        /// </summary>
        void Show();

        /// <summary>
        /// Indicates if an advertisement is ready to show.
        /// </summary>
        /// <returns>True if ready to show.</returns>
        bool ReadyToShow();

        /// <summary>
        /// If an advertisement has been loaded, clear it. Once cleared, a new
        /// load can be performed.
        /// </summary>
        /// <returns>true if successfully cleared</returns>
        void ClearLoaded();
    }
}
