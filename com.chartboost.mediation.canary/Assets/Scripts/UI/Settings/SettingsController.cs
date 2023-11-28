using System;
using System.Collections.Generic;
using Chartboost;
using Chartboost.AppTrackingTransparency;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utilities;

/// <summary>
/// The controller for the settings.
/// </summary>
public class SettingsController : MonoBehaviour
{
    /// <summary>
    /// The `ScrollRect` game object in the transform hierarchy that is the scroll view for this UI.
    /// </summary>
    public ScrollRect scrollView;

    /// <summary>
    /// The `RectTransform` of the game object where scrollable content will be placed under.
    /// </summary>
    public RectTransform scrollViewContent;

    /// <summary>
    /// The prefab for headers in each section.
    /// </summary>
    public GameObject headingItemPrefab;

    /// <summary>
    /// The prefab for the backend endpoint dropdown.
    /// </summary>
    public GameObject backendDropdownItemPrefab;

    /// <summary>
    /// The prefab for the kill switch dropdown.
    /// </summary>
    public GameObject killSwitchDropdownItemPrefab;

    /// <summary>
    /// The prefab for read-only data.
    /// </summary>
    public GameObject readOnlyItemPrefab;

    /// <summary>
    /// The prefab for rows with a button.
    /// </summary>
    public GameObject buttonItemPrefab;

    /// <summary>
    /// The prefab for rows with a text field.
    /// </summary>
    public GameObject textFieldItemPrefab;

    /// <summary>
    /// The prefab for rows with a toggle.
    /// </summary>
    public GameObject toggleItemPrefab;

    /// <summary>
    /// The prefab for rows with a dropdown
    /// </summary>
    public GameObject dropdownItemPrefab;

    /// <summary>
    /// Prefab utilized to visualize different kinds of data
    /// </summary>
    public DataVisualizer dataVisualizerPrefab;
    
    private DataVisualizer _visualizerInstance;

    private const float PaddingTop = 16;
    private const float SectionSpacing = 32;
    private const float HeadingPaddingBottom = 16;
    private const float ContentPaddingBottom = 200;

    private readonly Lazy<Environment> _environment = new Lazy<Environment>(() => Environment.Shared);
    private Environment Environment => _environment.Value;

    public static string PartnerInitializationData { get; set; } = string.Empty;

    private void Start()
    {
        GenerateSettingsList();
        ChartboostMediation.DidStart += error => GenerateSettingsList();
    }

    private void GenerateSettingsList()
    {
        foreach (Transform child in scrollViewContent.transform)
            Destroy(child.gameObject);

        scrollViewContent.sizeDelta = Vector2.zero;

        var yOffset = PaddingTop;
        yOffset = GenerateConfigurationSection(yOffset);
        yOffset = GenerateTestSettingsSection(yOffset);
        yOffset = GenerateUserInformationSection(yOffset);
        yOffset = GenerateCanaryOptionsSection(yOffset);
        yOffset = GenerateBannerOptionsSection(yOffset);
        // Ability to change game engine name and version not yet available in the Unity SDK
        //yOffset = GenerateGameInformationSection(yOffset);
        yOffset = GeneratePrivacyAndConsentSection(yOffset);
        yOffset = GenerateTestModeSection(yOffset);
        
        scrollViewContent.offsetMax += new Vector2(x: 0, y: ContentPaddingBottom);
        scrollView.normalizedPosition = new Vector2(x: 0, y: 1);
    }

    private float GenerateConfigurationSection(float yOffset)
    {
        yOffset = GenerateHeading("CHARTBOOST MEDIATION SDK CONFIGURATION", yOffset);

        var appIdentifier = GenerateReadOnlyItem("App Identifier", yOffset);
        yOffset = appIdentifier.Item1;
        appIdentifier.Item2.valueLabel.text = Environment.AppIdentifier;

        var appSignature = GenerateReadOnlyItem("App Signature", yOffset);
        yOffset = appSignature.Item1;
        appSignature.Item2.valueLabel.text = Environment.AppSignature;

        var signInButton = GenerateButtonItem("Change Sign In", "Change Sign In", yOffset);
        yOffset = signInButton.Item1;
        signInButton.Item2.pushedDelegate = delegate
        {
            Environment.ForceKillAfterSignIn = true;
            SceneManager.LoadScene("Scenes/SignIn", LoadSceneMode.Single);
        };

        var initialized = GenerateToggleItem("Automatic Initialization on Startup", yOffset);
        initialized.Item2.toggle.SetIsOnWithoutNotify(Environment.AutomaticInitialization);
        initialized.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.AutomaticInitialization = isOn;
            Environment.Save();
            Console.Out.Write($"updated automatic initialization: {isOn}");
        };
        
        yOffset = initialized.Item1;
        
        var partnersConsent = GenerateButtonItem("Per Partner Consent", "Configure", yOffset);
        partnersConsent.Item2.pushedDelegate += delegate
        {
            ConsentVisualizer.Instance.OpenVisualizer();
        };

        yOffset = partnersConsent.Item1;

        var adaptersData = GenerateButtonItem("Adapters Info", "Show", yOffset);
        adaptersData.Item2.pushedDelegate += delegate
        {
            DataVisualizer.Instance.UpdateContents("Adapters Information", JsonConvert.SerializeObject(ChartboostMediation.AdaptersInfo));
            DataVisualizer.Instance.OpenVisualizer();
        };
        
        yOffset = adaptersData.Item1;

        var networkKilLSwitch = GenerateKillSection(yOffset, "Partner Initialization Data", "Show");
        networkKilLSwitch.Item3.pushedDelegate += id =>
        {
            DataVisualizer.Instance.UpdateContents("Partner Initialization Data", PartnerInitializationData);
            DataVisualizer.Instance.OpenVisualizer();
        };

        yOffset = networkKilLSwitch.Item1;

        if (!ChartboostMediationInitializer.Instance.Initialized && !Environment.AutomaticInitialization)
        {
            var iniButton = GenerateButtonItem("Manual Initialization", "Initialize", yOffset);
            yOffset = iniButton.Item1;
            iniButton.Item2.pushedDelegate = delegate
            {
                ChartboostMediationInitializer.Instance.Initialize();
            };
        }

        if (ChartboostMediationInitializer.Instance.Initialized)
        {
            var cacheButton = GenerateButtonItem("Placement Caching", "Update", yOffset);
            yOffset = cacheButton.Item1;
            cacheButton.Item2.pushedDelegate = delegate
            {
                ChartboostMediationPlacementDataSource.Instance.ExpirePlacementCache(Environment.AppIdentifier);
                ChartboostMediationPlacementDataSource.Instance.LoadPlacementCache(Environment.AppIdentifier);
            };
        }

        var versionItem = GenerateReadOnlyItem("Build Version", yOffset);
        yOffset = versionItem.Item1;
        versionItem.Item2.valueLabel.text = Application.version;
        
        var buildNumber = GenerateReadOnlyItem("Build Number", yOffset);
        yOffset = buildNumber.Item1;
        var numberAsset = Resources.Load<TextAsset>("buildcode");
        if (numberAsset != null)
        {
            var number = numberAsset.text;
            buildNumber.Item2.valueLabel.text = number;
        }

        return yOffset + SectionSpacing;
    }

    private float GenerateTestSettingsSection(float yOffset)
    {
        yOffset = GenerateHeading("CHARTBOOST MEDIATION SDK TEST SETTINGS", yOffset);

        var selectedBackend = GenerateBackendDropdownItem(yOffset);
        yOffset = selectedBackend.Item1;
        var backend = new Backend(Environment.Backend);
        selectedBackend.Item2.dropdown.value = backend.Index;
        selectedBackend.Item2.changedDelegate = delegate (int newIndex)
        {
            var newBackendFromIndex = Backend.EndpointFromIndex(newIndex);
            var newBackend = new Backend(newBackendFromIndex);
            Environment.Backend = newBackend.StringRepresentation;
            Environment.Save();
            ToastManager.ShowMessage($"Updating Endpoints.\nSDK Domain: {newBackend.SdkDomainName}.\nRTB Domain: {newBackend.RtbDomainName}");
            Console.Out.Write($"updated backend: {Environment.Backend}");
        };

        return yOffset + SectionSpacing;
    }
    
    private float GenerateUserInformationSection(float yOffset)
    {
        yOffset = GenerateHeading("USER INFORMATION", yOffset);

        var identifier = GenerateTextFieldItem("Identifier", yOffset);
        yOffset = identifier.Item1;
        identifier.Item2.inputField.text = Environment.UserIdentifier;
        identifier.Item2.changedDelegate = delegate (string newValue)
        {
            Environment.UserIdentifier = newValue;
            Environment.Save();
            Console.Out.Write($"updated user identifier: {newValue}");
        };

        return yOffset + SectionSpacing;
    }

    private float GenerateCanaryOptionsSection(float yOffset)
    {
        yOffset = GenerateHeading("CANARY OPTIONS", yOffset);

        var autoLoadOnShow = GenerateToggleItem("Auto-Load On Show", yOffset);
        yOffset = autoLoadOnShow.Item1;
        autoLoadOnShow.Item2.toggle.SetIsOnWithoutNotify(Environment.AutoLoadOnShow);
        autoLoadOnShow.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.AutoLoadOnShow = isOn;
            Environment.Save();
        };
        
        var keepFullscreenAdUntilShownThenLoad = GenerateToggleItem("Keep Fullscreen Ad Until Shown Then Load", yOffset);
        yOffset = keepFullscreenAdUntilShownThenLoad.Item1;
        keepFullscreenAdUntilShownThenLoad.Item2.toggle.SetIsOnWithoutNotify(Environment.KeepFullscreenAdUntilShownThenLoad);
        keepFullscreenAdUntilShownThenLoad.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.KeepFullscreenAdUntilShownThenLoad = isOn;
            Environment.Save();
        };

        var newFullscreenAPIToggle = GenerateToggleItem("Use new Fullscreen API", yOffset);
        yOffset = newFullscreenAPIToggle.Item1;
        newFullscreenAPIToggle.Item2.toggle.SetIsOnWithoutNotify(Environment.UseNewFullscreenAPI);
        newFullscreenAPIToggle.Item2.changedDelegate = delegate(bool isOn)
        {
            Environment.UseNewFullscreenAPI = isOn;
            Environment.Save();
            
            APISwitcher.Instance.UseNewFullscreenAPI(isOn);
        };
        
        return yOffset + SectionSpacing;
    }

    private float GenerateBannerOptionsSection(float yOffset)
    {
        yOffset = GenerateHeading("BANNER OPTIONS", yOffset);

        var newBannerAPIToggle = GenerateToggleItem("Use new Banner API", yOffset);
        yOffset = newBannerAPIToggle.Item1;
        newBannerAPIToggle.Item2.toggle.SetIsOnWithoutNotify(Environment.UseNewBannerAPI);
        newBannerAPIToggle.Item2.changedDelegate = delegate(bool isOn)
        {
            Environment.UseNewBannerAPI = isOn;
            Environment.Save();
            
            APISwitcher.Instance.UseNewBannerAPI(isOn);
        };
        
        var discardOverSizedAds = GenerateToggleItem("Discard Oversized Ads", yOffset);
        yOffset = discardOverSizedAds.Item1;
        discardOverSizedAds.Item2.toggle.SetIsOnWithoutNotify(Environment.DiscardOversizedBannerAds);
        discardOverSizedAds.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.DiscardOversizedBannerAds = isOn;
            Environment.Save();

            ChartboostMediation.DiscardOversizedAds(Environment.DiscardOversizedBannerAds);
        };
        
        return yOffset + SectionSpacing;
    }
    
    private float GenerateGameInformationSection(float yOffset)
    {
        yOffset = GenerateHeading("GAME INFORMATION", yOffset);

        var gameEngineName = GenerateTextFieldItem("Game Engine Name", yOffset);
        yOffset = gameEngineName.Item1;
        gameEngineName.Item2.inputField.text = Environment.GameEngineName;
        gameEngineName.Item2.changedDelegate = delegate (string newValue)
        {
            Environment.GameEngineName = newValue;
            Environment.Save();
        };

        var gameEngineVersion = GenerateTextFieldItem("Game Engine Version", yOffset);
        yOffset = gameEngineVersion.Item1;
        gameEngineVersion.Item2.inputField.text = Environment.GameEngineVersion;
        gameEngineVersion.Item2.changedDelegate = delegate (string newValue)
        {
            Environment.GameEngineVersion = newValue;
            Environment.Save();
        };
        
        return yOffset + SectionSpacing;
    }

    private float GeneratePrivacyAndConsentSection(float yOffset)
    {
        yOffset = GenerateHeading("PRIVACY AND CONSENT", yOffset);
        
        if (AppTrackingTransparency.IsSupported)
        {
            var att = GenerateButtonItem("App Tracking Transparency", "Request Permission", yOffset);
            yOffset = att.Item1;
            
            var idfaUI = GenerateReadOnlyItem("IDFA", yOffset);
            yOffset = idfaUI.Item1;
            var deviceIdfa = string.Empty;
            #if UNITY_IOS
            deviceIdfa = UnityEngine.iOS.Device.advertisingIdentifier;
            #endif
            idfaUI.Item2.valueLabel.text = deviceIdfa;
            
            att.Item2.pushedDelegate = delegate
            {
                if (AppTrackingTransparency.CurrentAuthorizationStatus != AuthorizationStatus.NotDetermined)
                    return;
                
                AppTrackingTransparency.OnAuthResponse += (newStatus, idfa) =>
                {
                    if (newStatus == AuthorizationStatus.Authorized)
                        idfaUI.Item2.valueLabel.text = idfa;
                };
                AppTrackingTransparency.RequestAuthorization();
            };
        }
        
        var idfvLabel = "IDFV";
        #if UNITY_ANDROID
        idfvLabel = "Android ID";
        #endif
        var idfv = GenerateReadOnlyItem(idfvLabel, yOffset);
        yOffset = idfv.Item1;
        idfv.Item2.valueLabel.text = SystemInfo.deviceUniqueIdentifier;

        var coppaSubject = GenerateToggleItem("Is Subject to COPPA", yOffset);
        yOffset = coppaSubject.Item1;
        coppaSubject.Item2.toggle.SetIsOnWithoutNotify(Environment.IsSubjectToCoppa);
        coppaSubject.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.IsSubjectToCoppa = isOn;
            Environment.Save();

            ChartboostMediation.SetSubjectToCoppa(Environment.IsSubjectToCoppa);
        };

        var ccpaConsent = GenerateToggleItem("CCPA Consent", yOffset);
        yOffset = ccpaConsent.Item1;
        ccpaConsent.Item2.toggle.SetIsOnWithoutNotify(Environment.CcpaConsent);
        ccpaConsent.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.CcpaConsent = isOn;
            Environment.Save();

            ChartboostMediation.SetCCPAConsent(Environment.CcpaConsent);
        };

        var gdprSubject = GenerateToggleItem("Is Subject to GDPR", yOffset);
        yOffset = gdprSubject.Item1;
        gdprSubject.Item2.toggle.SetIsOnWithoutNotify(Environment.IsSubjectToGdpr);
        gdprSubject.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.IsSubjectToGdpr = isOn;
            Environment.Save();

            ChartboostMediation.SetSubjectToGDPR(Environment.IsSubjectToGdpr);
        };

        var gdprConsent = GenerateToggleItem("GDPR Consent", yOffset);
        yOffset = gdprConsent.Item1;
        gdprConsent.Item2.toggle.SetIsOnWithoutNotify(Environment.GdprConsent);
        gdprConsent.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.GdprConsent = isOn;
            Environment.Save();

            ChartboostMediation.SetUserHasGivenConsent(Environment.GdprConsent);
        };

        var tcString = GenerateTextFieldItem("TC String", yOffset);
        yOffset = tcString.Item1;
        tcString.Item2.inputField.text = Environment.TCString;
        tcString.Item2.changedDelegate = delegate (string newValue)
        {
            Environment.TCString = newValue;
            Environment.Save();
            Console.Out.Write($"updated TC string: {newValue}");
        };

        return yOffset + SectionSpacing;
    }

    private float GenerateTestModeSection(float yOffset)
    {
        yOffset = GenerateHeading("TEST MODE", yOffset);

        var amazon = GenerateToggleItem("Amazon APS", yOffset);
        yOffset = amazon.Item1;
        amazon.Item2.toggle.SetIsOnWithoutNotify(Environment.IsAmazonTestModeEnabled);
        amazon.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.IsAmazonTestModeEnabled = isOn;
            Environment.Save();
        };

        var facebook = GenerateToggleItem("Meta Audience Network", yOffset);
        yOffset = facebook.Item1;
        facebook.Item2.toggle.SetIsOnWithoutNotify(Environment.IsFacebookAudienceNetworkTestModeEnabled);
        facebook.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.IsFacebookAudienceNetworkTestModeEnabled = isOn;
            Environment.Save();
        };

        var helium = GenerateToggleItem("Chartboost Mediation", yOffset);
        yOffset = helium.Item1;
        helium.Item2.toggle.SetIsOnWithoutNotify(Environment.IsChartboostMediationTestModeEnabled);
        helium.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.IsChartboostMediationTestModeEnabled = isOn;
            Environment.Save();
        };

        var applovin = GenerateToggleItem("AppLovin", yOffset);
        yOffset = applovin.Item1;
        applovin.Item2.toggle.SetIsOnWithoutNotify(Environment.IsAppLovinTestModeEnabled);
        applovin.Item2.changedDelegate = delegate (bool isOn)
        {
            Environment.IsAppLovinTestModeEnabled = isOn;
            Environment.Save();
        };

        return yOffset + SectionSpacing;
    }

    private float GenerateHeading(string title, float yOffset)
    {
        var heading = Instantiate(headingItemPrefab, scrollViewContent, false);
        var rectTransform = heading.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var headingItem = heading.GetComponent<HeadingItem>();
        headingItem.titleLabel.text = title;
        yOffset += rect.height;
        return yOffset + HeadingPaddingBottom;
    }

    private (float, SettingsTextFieldItem) GenerateTextFieldItem(string title, float yOffset)
    {
        var instance = Instantiate(textFieldItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var textFieldItem = instance.GetComponent<SettingsTextFieldItem>();
        textFieldItem.titleLabel.text = title;
        yOffset += rect.height;
        return (yOffset, textFieldItem);
    }

    private (float, SettingsReadOnlyItem) GenerateReadOnlyItem(string title, float yOffset)
    {
        var instance = Instantiate(readOnlyItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var readOnlyItem = instance.GetComponent<SettingsReadOnlyItem>();
        readOnlyItem.titleLabel.text = title;
        yOffset += rect.height;
        return (yOffset, readOnlyItem);
    }

    private (float, SettingsButtonItem) GenerateButtonItem(string title, string buttonTitle, float yOffset)
    {
        var instance = Instantiate(buttonItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var buttonItem = instance.GetComponent<SettingsButtonItem>();
        buttonItem.titleLabel.text = title;
        var buttonText = buttonItem.button.GetComponentInChildren<Text>();
        buttonText.text = buttonTitle;
        yOffset += rect.height;
        return (yOffset, buttonItem);
    }

    private (float, SettingsBackendDropdownItem) GenerateBackendDropdownItem(float yOffset)
    {
        var instance = Instantiate(backendDropdownItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var dropdownItem = instance.GetComponent<SettingsBackendDropdownItem>();
        yOffset += rect.height;
        return (yOffset, dropdownItem);
    }

    private (float, Dropdown) GenerateDropdownItem(float yOffset, List<Dropdown.OptionData> options)
    {
        var instance = Instantiate(dropdownItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var dropdownItem = instance.GetComponentInChildren<Dropdown>();
        dropdownItem.AddOptions(options);
        dropdownItem.RefreshShownValue();
        yOffset += rect.height;
        return (yOffset, dropdownItem);
    }

    private (float, SettingsKillSwitchTogglesItem, SettingsButtonItem) GenerateKillSection(float yOffset, string labelTitle, string buttonContent)
    {
        var instance = Instantiate(killSwitchDropdownItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var dropdownItem = instance.GetComponent<SettingsKillSwitchTogglesItem>();
        yOffset += rect.height;
        
        var buttonInstance = Instantiate(buttonItemPrefab, scrollViewContent, false);
        var buttonRectTransform = instance.GetComponent<RectTransform>();
        var buttonRect = buttonRectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + buttonRect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: buttonRect.height);
        var buttonItem = buttonInstance.GetComponent<SettingsButtonItem>();
        buttonItem.titleLabel.text = labelTitle;
        var buttonText = buttonItem.button.GetComponentInChildren<Text>();
        buttonText.text = buttonContent;
        yOffset += buttonRect.height;
        return (yOffset, dropdownItem, buttonItem);
    }

    private (float, SettingsToggleItem) GenerateToggleItem(string title, float yOffset)
    {
        var instance = Instantiate(toggleItemPrefab, scrollViewContent, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
        var toggleItem = instance.GetComponent<SettingsToggleItem>();
        var toggleLabel = toggleItem.GetComponentInChildren<Text>();
        toggleLabel.text = title;
        yOffset += rect.height;
        return (yOffset, toggleItem);
    }
}
