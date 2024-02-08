using System.Collections;
using Chartboost.Mediation.Demo.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace Chartboost.Mediation.Demo.Pages
{
    public class InitializationPage : MonoBehaviour
    {
        [SerializeField] private Text initializationStatusText;
        [SerializeField] private LoadingIcon loadingIconIndicator;
        
        private const string DefaultUserIdentifier = "123456";
        private const string InitializationText = "Initializing the Chartboost Mediation Unity SDK...";
        private const string InitializationSuccessText = "Chartboost Mediation Unity SDK Initialization Completed!";
        private const string InitializationFailedText = "Chartboost Mediation Unity SDK Failed to Initialize with Error:";
        private const float TransitionToNextPage = 0.75f;
        private const int TargetFramerate = 60;
        
        private void Awake()
        {
            Application.targetFrameRate = TargetFramerate;
            ChartboostMediation.DidStart += OnDidStart;
            ChangeInitializationStatusText(InitializationText);
            loadingIconIndicator.ToggleLoadingIcon(true);
            ChartboostMediation.StartWithOptions(ChartboostMediationSettings.AppId, ChartboostMediationSettings.AppSignature);
        }

        private void OnDidStart(string error)
        {
            loadingIconIndicator.ToggleLoadingIcon(false);
            ChartboostMediation.SetUserIdentifier(DefaultUserIdentifier);
            
            // If failed to initialize, report and return
            if (error != null)
            {
                ChangeInitializationStatusText($"{InitializationFailedText} {error}");
                #if UNITY_EDITOR
                StartCoroutine(MoveToAdFormats());
                #endif
                return;
            }

            ChartboostMediation.SetTestMode(true);
            ChartboostMediation.SetSubjectToGDPR(false);
            ChartboostMediation.SetSubjectToCoppa(false);
            ChartboostMediation.SetUserHasGivenConsent(true);
            ChangeInitializationStatusText(InitializationSuccessText);
            StartCoroutine(MoveToAdFormats());
        }

        private void ChangeInitializationStatusText(string text)
        {
            initializationStatusText.text = text;
        }

        private static IEnumerator MoveToAdFormats()
        {
            yield return new WaitForSeconds(TransitionToNextPage);
            PageController.MoveToPage(PageType.AdFormats);
        }
    }
}

