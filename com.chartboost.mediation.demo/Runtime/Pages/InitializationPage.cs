using System.Collections;
using Chartboost.Core;
using Chartboost.Core.Initialization;
using Chartboost.Mediation.Demo.Loading;
using Chartboost.Mediation.Error;
using UnityEngine;
using UnityEngine.UI;

namespace Chartboost.Mediation.Demo.Pages
{
    public class InitializationPage : MonoBehaviour
    {
        [SerializeField] private Text initializationStatusText;
        [SerializeField] private LoadingIcon loadingIconIndicator;
        
        private const string InitializationText = "Initializing the Chartboost Mediation Unity SDK...";
        private const string InitializationSuccessText = "Chartboost Mediation Unity SDK Initialization Completed!";
        private const string InitializationFailedText = "Chartboost Mediation Unity SDK Failed to Initialize with Error:";
        private const float TransitionToNextPage = 0.75f;
        private const int TargetFramerate = 60;
        
        private void Awake()
        {
            Application.targetFrameRate = TargetFramerate;

            ChartboostCore.ModuleInitializationCompleted += OnModuleInitializationResult;
            ChangeInitializationStatusText(InitializationText);
            loadingIconIndicator.ToggleLoadingIcon(true);

            var appId = string.Empty;
            #if UNITY_ANDROID
            appId = DefaultEnvironment.AndroidAppId;
            #elif UNITY_IOS
            appId = DefaultEnvironment.IOSAppId;
            #endif
            
           ChartboostCore.Initialize(new SDKConfiguration(appId, null));
        }

        private void OnModuleInitializationResult(ModuleInitializationResult result)
        {
            if (result.ModuleId != ChartboostMediation.CoreModuleId)
                return;

            var error = result.Error;
            loadingIconIndicator.ToggleLoadingIcon(false);
            
            // If failed to initialize, report and return
            if (error.HasValue)
            {
                ChangeInitializationStatusText($"{InitializationFailedText} {error.Value.Message}");
                #if UNITY_EDITOR
                StartCoroutine(MoveToAdFormats());
                #endif
                return;
            }

            ChartboostMediation.TestMode = true;
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

