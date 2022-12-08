using UnityEngine.Scripting;

namespace Helium.FullScreen
{
    public interface IHeliumFullScreenAd
    {
        /// <summary>
        /// Load the advertisement.
        /// </summary>
        [Preserve]
        void Load();

        /// <summary>
        /// Show a previously loaded advertisement.
        /// </summary>
        [Preserve]
        void Show();

        /// <summary>
        /// Indicates if an advertisement is ready to show.
        /// </summary>
        /// <returns>True if ready to show.</returns>
        [Preserve]
        bool ReadyToShow();

        /// <summary>
        /// If an advertisement has been loaded, clear it. Once cleared, a new
        /// load can be performed.
        /// </summary>
        /// <returns>true if successfully cleared</returns>
        [Preserve]
        bool ClearLoaded();
    }
}
