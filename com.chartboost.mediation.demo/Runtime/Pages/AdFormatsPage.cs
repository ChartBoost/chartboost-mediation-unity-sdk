using UnityEngine;
using UnityEngine.UI;

namespace Chartboost.Mediation.Demo.Pages
{
    public class AdFormatsPage : MonoBehaviour
    {
        [Header("Placements")]
        [SerializeField] private Placement bannerPlacement = new Placement(DefaultEnvironment.BannerPlacement, PlacementType.Banner);
        [SerializeField] private Placement unityBannerPlacement = new Placement(DefaultEnvironment.BannerPlacement, PlacementType.UnityBanner);
        [SerializeField] private Placement interstitialPlacement = new Placement(DefaultEnvironment.InterstitialPlacement, PlacementType.Interstitial);
        [SerializeField] private Placement rewardedPlacement = new Placement(DefaultEnvironment.RewardedPlacement, PlacementType.Rewarded);

        [Header("Buttons")]
        [SerializeField] private Button bannerPlacementButton;
        [SerializeField] private Button unityBannerPlacementButton;
        [SerializeField] private Button interstitialPlacementButton;
        [SerializeField] private Button rewardedPlacementButton;

        private const string BannerPreparationText = "A Banner advertisement must first be loaded.";
        private const string UnityBannerPreparationText = "A UnityBanner advertisement must first be loaded.";
        private const string InterstitialPreparationText = "A fullscreen Interstitial advertisement must first be loaded.";
        private const string RewardedPreparationText = "A fullscreen Rewarded advertisement must first be loaded.";
        
        private const string FullscreenLoadCompletionText = "After it has been successfully loaded it can then be shown.";
        private const string BannerLoadCompletionText = "After it has been successfully loaded it will then be automatically shown in the specified location.";

        private void Awake()
        {
            bannerPlacementButton.onClick.AddListener(MoveToBannerPlacement);
            unityBannerPlacementButton.onClick.AddListener(MoveToUnityBannerPlacement);
            interstitialPlacementButton.onClick.AddListener(MoveToInterstitialPlacement);
            rewardedPlacementButton.onClick.AddListener(MoveToRewardedPlacement);
        }

        private void OnDestroy()
        {
            bannerPlacementButton.onClick.RemoveListener(MoveToBannerPlacement);
            unityBannerPlacementButton.onClick.RemoveListener(MoveToUnityBannerPlacement);
            interstitialPlacementButton.onClick.RemoveListener(MoveToInterstitialPlacement);
            rewardedPlacementButton.onClick.RemoveListener(MoveToRewardedPlacement);
        }

        private void MoveToBannerPlacement() => MoveToPlacementPage(bannerPlacement, BannerPreparationText, BannerLoadCompletionText);
        
        private void MoveToUnityBannerPlacement() => MoveToPlacementPage(unityBannerPlacement, UnityBannerPreparationText, BannerLoadCompletionText);

        private void MoveToInterstitialPlacement() => MoveToPlacementPage(interstitialPlacement, InterstitialPreparationText, FullscreenLoadCompletionText);

        private void MoveToRewardedPlacement() => MoveToPlacementPage(rewardedPlacement, RewardedPreparationText, FullscreenLoadCompletionText);

        private void MoveToPlacementPage(Placement placement, string preparationText, string loadCompletionText)
        {
            var pageInstanceGameObject = PageController.MoveToPage(PageType.Placement);
            var placementPageInstance = pageInstanceGameObject.GetComponent<PlacementPage>();

            if (placementPageInstance == null) 
                Debug.LogError("PlacementPage instance was not found.");
            
            placementPageInstance.Configure(placement,preparationText, loadCompletionText);
        }
    }
}
