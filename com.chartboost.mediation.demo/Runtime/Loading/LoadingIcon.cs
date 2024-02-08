using UnityEngine;

namespace Chartboost.Mediation.Demo.Loading
{
    /// <summary>
    /// Loading indicator icon.
    /// </summary>
    public class LoadingIcon : MonoBehaviour
    {
        [SerializeField] private RectTransform mainTransform;
        [SerializeField] private float repeatRate = -0.05f;
        [SerializeField] private float oneStepAngle = -18f;

        private float _startTime;

        /// <summary>
        /// Toggles loading icon active status.
        /// </summary>
        /// <param name="status">Status to be set.</param>
        public void ToggleLoadingIcon(bool status)
        {
            gameObject.SetActive(status);
        }

        private void OnEnable()
        {
            // Call rotation method based on parameters. More "expensive", but simpler to implement
            InvokeRepeating(nameof(RotateIcon), 0, repeatRate);
        }
        
        private void OnDisable()
        {
            // Disable invoke when status is disabled
            CancelInvoke(nameof(RotateIcon));
        }

        private void RotateIcon()
        {
            var angle = mainTransform.localEulerAngles;
            angle.z += oneStepAngle;
            mainTransform.localEulerAngles = angle;
        }
    }
}
