using UnityEngine;
using System;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
///
/// </summary>
public class SignInController : MonoBehaviour
{
    /// <summary>
    /// The `RectTransform` of the game object where content will be placed under.
    /// </summary>
    public RectTransform content;

    /// <summary>
    /// The prefab for headers in each section.
    /// </summary>
    public GameObject headingItemPrefab;

    /// <summary>
    /// The prefab for rows with a text field.
    /// </summary>
    public GameObject textFieldItemPrefab;
    
    /// <summary>
    /// The prefab for rows with a button.
    /// </summary>
    public GameObject buttonItemPrefab;

    private const float PaddingTop = 16;
    private const float HeadingPaddingBottom = 16;

    private SettingsTextFieldItem _appIdentifierTextFieldItem;
    private SettingsTextFieldItem _appSignatureTextFieldItem;

    /// <summary>
    /// Potential AppIdentifier and AppSignature values.
    /// </summary>
    private string AppIdentifier { get; set; }
    private string AppSignature { get; set; }
    
    /// <summary>
    /// Environment settings.
    /// </summary>
    private readonly Lazy<Environment> _environment = new Lazy<Environment>(() => Environment.Shared);
    private Environment Environment => _environment.Value;

    /// <summary>
    /// Standard Unity Awake handler.
    /// </summary>
    private void Awake()
    {
        AppIdentifier = DefaultAppIdentifer;
        AppSignature = DefaultAppSignature;
        GenerateLoginForm();
    }

    private void GenerateLoginForm()
    {
        var yOffset = PaddingTop;
        yOffset = GenerateHeading("APPLICATION SIGN IN", yOffset);
        
        // App Identifier
        var appIdentifier = GenerateTextFieldItem("App Identifier", yOffset);
        _appIdentifierTextFieldItem = appIdentifier.Item2;
        yOffset = appIdentifier.Item1;
        appIdentifier.Item2.inputField.text = AppIdentifier;
        appIdentifier.Item2.changedDelegate = delegate(string newValue)
        {
            AppIdentifier = newValue;
        };
        
        // App Signature
        var appSignature = GenerateTextFieldItem("App Signature", yOffset);
        _appSignatureTextFieldItem = appSignature.Item2;
        yOffset = appSignature.Item1;
        appSignature.Item2.inputField.text = AppSignature;
        appSignature.Item2.changedDelegate = newValue =>
        {
            AppSignature = newValue;
        };
        
        // Sign In button
        var signInButtonTitle = Environment.ForceKillAfterSignIn ? "Sign In and Restart" : "Sign In";
        var signInButton = GenerateButtonItem("", signInButtonTitle, yOffset);
        yOffset = signInButton.Item1;
        signInButton.Item2.pushedDelegate = delegate
        {
            Environment.AppIdentifier = AppIdentifier;
            Environment.AppSignature = AppSignature;
            Environment.Save();
            
            // Enable the Chartboost Mediation Initializer
            var chartboostMediation = FindObjectOfType<ChartboostMediationInitializer>();
            chartboostMediation.enabled = true;

            if (Environment.ForceKillAfterSignIn)
            {
                Environment.ForceKillAfterSignIn = false;
                Application.Quit();
            }
            else
            {
                LoadRoot();
            }
        };
        
        // Scan QR Code button
        var scanQrCodeButton = GenerateButtonItem("", "Scan QR Code", yOffset);
        yOffset = scanQrCodeButton.Item1;
        scanQrCodeButton.Item2.pushedDelegate = delegate
        {
            QrCodeScanner.PresentQrCodeScanner();
        };
        QrCodeScanner.DidScanQrCode += DidScanQrCodeHandler;
    }

    private void DidScanQrCodeHandler(string appId, string appSignature)
    {
        AppIdentifier = appId;
        _appIdentifierTextFieldItem.inputField.text = appId;
        
        AppSignature = appSignature;
        _appSignatureTextFieldItem.inputField.text = appSignature;
    }
    
    private float GenerateHeading(string title, float yOffset)
    {
        var heading = Instantiate(headingItemPrefab, content, false);
        var rectTransform = heading.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        content.offsetMax += new Vector2(x: 0, y: rect.height);
        var headingItem = heading.GetComponent<HeadingItem>();
        headingItem.titleLabel.text = title;
        yOffset += rect.height;
        return yOffset + HeadingPaddingBottom;
    }

    private (float, SettingsTextFieldItem) GenerateTextFieldItem(string title, float yOffset)
    {
        var instance = Instantiate(textFieldItemPrefab, content, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        content.offsetMax += new Vector2(x: 0, y: rect.height);
        var textFieldItem = instance.GetComponent<SettingsTextFieldItem>();
        textFieldItem.titleLabel.text = title;
        yOffset += rect.height;
        return (yOffset, textFieldItem);
    }

    private (float, SettingsButtonItem) GenerateButtonItem(string title, string buttonTitle, float yOffset)
    {
        var instance = Instantiate(buttonItemPrefab, content, false);
        var rectTransform = instance.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        rectTransform.anchoredPosition = new Vector3(x: 0, y: -(yOffset + rect.height), z: 0);
        content.offsetMax += new Vector2(x: 0, y: rect.height);
        var buttonItem = instance.GetComponent<SettingsButtonItem>();
        buttonItem.titleLabel.text = title;
        var buttonText = buttonItem.button.GetComponentInChildren<Text>();
        buttonText.text = buttonTitle;
        yOffset += rect.height;
        return (yOffset, buttonItem);
    }

    private void LoadRoot()
    {
        SceneManager.LoadScene("Scenes/Root", LoadSceneMode.Single);
    }

    private static string DefaultAppIdentifer => Debug.isDebugBuild ? DefaultValue.AppIdentifier : null;
    private static string DefaultAppSignature => Debug.isDebugBuild ? DefaultValue.AppSignature : null;
}
