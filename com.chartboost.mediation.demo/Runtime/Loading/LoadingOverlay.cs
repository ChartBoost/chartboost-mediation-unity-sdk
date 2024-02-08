using UnityEngine;

namespace Chartboost.Mediation.Demo.Loading
{
    /// <summary>
    /// Simple overlay to block input when "Loads" are happening.
    /// </summary>
    public class LoadingOverlay : MonoBehaviour
    {
        public static LoadingOverlay Instance;

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Toggles overlay active status.
        /// </summary>
        /// <param name="status">Status to be set.</param>
        public void ToggleLoadingOverlay(bool status)
        {
            gameObject.SetActive(status);
        }
    }
}
