using System;
using Chartboost.Mediation.Demo.AdControllers;
using UnityEngine;
using UnityEngine.UI;

namespace Chartboost.Mediation.Demo.Pages
{
    [Serializable]
    public struct Placement
    {
        public string placementIdentifier;
        public PlacementType placementType;
        public Sprite placementSprite;

        public Placement(string placementIdentifier, PlacementType placementType)
        {
            this.placementIdentifier = placementIdentifier;
            this.placementType = placementType;
            placementSprite = null;
        }
    }

    public enum PlacementType
    {
        Banner,
        UnityBanner,
        Interstitial,
        Rewarded
    }

    public class PlacementPage : MonoBehaviour
    {
        [Header("Contents")]
        [SerializeField] private Image unityIcon;
        [SerializeField] private Image placementIcon;
        [SerializeField] private Text placementTypeText;
        [SerializeField] private Text preparationText;
        [SerializeField] private Text loadCompletionText;
        
        [Header("Buttons")]
        [SerializeField] private Button loadButton;
        [SerializeField] private Button showButton;
        [SerializeField] private Button backButton;
        
        [Header("Queueing")]
        [SerializeField] private Text queuePreparationText;
        [SerializeField] private Button queueButton;
        [SerializeField] private Toggle queueToggle;
        
        private Placement _currentPlacement;
        private SimpleAdController _currentController;
        
        private void Awake()
        {
            backButton.onClick.AddListener(MoveToAdFormats);
        }

        private void OnDestroy()
        {
            backButton.onClick.RemoveListener(MoveToAdFormats);
        }

        private void MoveToAdFormats()
        {
            PageController.MoveToPage(PageType.AdFormats);
            loadButton.onClick.RemoveListener(_currentController.Load);
            showButton.onClick.RemoveListener(_currentController.Show);
            _currentController?.Dispose();
        }

        public void Configure(Placement placement, string preparationTextContents, string loadCompletionTextContents, string queuePreparationTextContents = null)
        {
            var placementType = placement.placementType;
            placementTypeText.text = placementType.ToString();
            placementIcon.sprite = placement.placementSprite;
            preparationText.text = preparationTextContents;
            queuePreparationText.text = queuePreparationTextContents;
            loadCompletionText.text = loadCompletionTextContents;

            _currentController = placementType switch
            {
                PlacementType.Interstitial => new FullscreenAdControllerWithQueue(placement.placementIdentifier, queueToggle, queueButton, loadButton),
                PlacementType.Rewarded => new FullscreenAdControllerWithQueue(placement.placementIdentifier, queueToggle, queueButton, loadButton),
                PlacementType.Banner => new BannerAdController(placement.placementIdentifier),
                _ => new UnityBannerAdController(placement.placementIdentifier)
            };

            if (_currentController != null)
            {
                loadButton.onClick.AddListener(_currentController.Load);
                showButton.onClick.AddListener(_currentController.Show);
            }

            queueToggle.gameObject.SetActive(placementType == PlacementType.Interstitial || placementType == PlacementType.Rewarded);
            showButton.gameObject.SetActive(placementType == PlacementType.Interstitial || placementType == PlacementType.Rewarded);
            unityIcon.gameObject.SetActive(placementType == PlacementType.UnityBanner);
        }
    }
}
