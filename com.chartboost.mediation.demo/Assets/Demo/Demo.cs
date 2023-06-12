using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Chartboost;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Banner;
using Chartboost.Requests;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Demo : MonoBehaviour
{
    private const string DefaultPlacementFullscreen = "CBRewarded";
    private const string DefaultPlacementBanner = "AllNetworkBanner";
    private const string DefaultUserIdentifier = "123456";
    private const string DefaultFullscreenAdCustomData = "{\"testkey\":\"testvalue\"}";

    // advertisement type selection
    public GameObject fullScreenPanel;
    public GameObject bannerPanel;

    // interstitial controls
    public InputField fullscreenPlacementInputField;
    private IChartboostMediationFullscreenAd _fullscreenAd;
    
    // banner controls
    public InputField bannerPlacementInputField;
    public Dropdown bannerSizeDropdown;
    public Dropdown bannerLocationDropdown;
    private ChartboostMediationBannerAd _bannerAd;
    private bool _bannerAdIsVisible;

    public ScrollRect outputTextScrollRect;
    public Text outputText;

    public GameObject objectToDestroyForTest;

    #region Lifecycle
    private void Awake()
    {
        Application.targetFrameRate = 60;
        ChartboostMediation.DidStart += DidStart;
        ChartboostMediation.DidReceivePartnerInitializationData += DidReceivePartnerInitializationData;
        ChartboostMediation.DidReceiveImpressionLevelRevenueData += DidReceiveImpressionLevelRevenueData;
        ChartboostMediation.UnexpectedSystemErrorDidOccur += UnexpectedSystemErrorDidOccur;
        SetupBannerDelegates();
    }
    
    private void Start()
    {
        if (outputText != null)
            outputText.text = string.Empty;
        fullScreenPanel.SetActive(true);
        bannerPanel.SetActive(false);

        fullscreenPlacementInputField.SetTextWithoutNotify(DefaultPlacementFullscreen);
        bannerPlacementInputField.SetTextWithoutNotify(DefaultPlacementBanner);

        ChartboostMediation.StartWithAppIdAndAppSignature(ChartboostMediationSettings.AppId, ChartboostMediationSettings.AppSignature);
    }

    private void OnDestroy()
    {
        OnInvalidateFullscreenClick();

        if (_bannerAd == null) 
            return;
        
        _bannerAd.ClearLoaded();
        _bannerAd.Destroy();
        Log("destroyed an existing banner");
    }

    private void DidStart(string error)
    {
        Log($"DidStart: {error}");
        ChartboostMediation.SetUserIdentifier(DefaultUserIdentifier);

        Log($"User Identifier Set: {ChartboostMediation.GetUserIdentifier()}");

        if (error != null) return;
        ChartboostMediation.SetSubjectToGDPR(false);
        ChartboostMediation.SetSubjectToCoppa(false);
        ChartboostMediation.SetUserHasGivenConsent(true);
    }

    private void DidReceiveImpressionLevelRevenueData(string placement, Hashtable impressionData)
    {
        var json =  JsonTools.Serialize(impressionData);
        Log($"DidReceiveImpressionLevelRevenueData {placement}: {JsonPrettify(json)}");
    }

    private void DidReceivePartnerInitializationData(string partnerInitializationData)
    {
        Log($"DidReceivePartnerInitializationData: ${JsonPrettify(partnerInitializationData)}");
    }

    public void OnSelectFullScreenClicked()
    {
        fullScreenPanel.SetActive(true);
        bannerPanel.SetActive(false);
    }

    public void OnSelectBannersClicked()
    {
        fullScreenPanel.SetActive(false);
        bannerPanel.SetActive(true);
    }

    #endregion

    #region Fullscreen
    public async void OnLoadFullscreenClick()
    {
        var keywords = new Dictionary<string, string>()
        {
            { "i12_keyword1", "i12_value1" },
            { "i12_keyword2", "i12_value2" }
        };

        var loadRequest = new ChartboostMediationFullscreenAdLoadRequest(fullscreenPlacementInputField.text, keywords);

        loadRequest.DidClick += fullscreenAd => Log($"DidClick Name: {fullscreenAd.Request.PlacementName}");

        loadRequest.DidClose += (fullscreenAd, error) => Log(!error.HasValue
            ? $"DidClose Name: {fullscreenAd.Request.PlacementName}"
            : $"DidClose Name: {fullscreenAd.Request.PlacementName}, Code: {error?.code}, Message: {error?.message}");

        loadRequest.DidReward += fullscreenAd => Log($"DidReward Name: {fullscreenAd.Request.PlacementName}");

        loadRequest.DidRecordImpression += fullscreenAd => Log($"DidImpressionRecorded Name: {fullscreenAd.Request.PlacementName}");

        loadRequest.DidExpire += fullscreenAd => Log($"DidExpire Name: {fullscreenAd.Request.PlacementName}");

        var loadResult = await ChartboostMediation.LoadFullscreenAd(loadRequest);
        
        // Failed to Load
        if (loadResult.Error.HasValue)
        {
            var error = loadResult.Error.Value;
            Log($"Fullscreen Failed to Load: {error.code}, message: {error.message}");
            return;
        }

        // Loaded but AD is null?
        _fullscreenAd = loadResult.Ad;
        if (_fullscreenAd == null)
        {
            Log("Fullscreen Ad is null but no error was found???");
            return;
        }

        // DidLoad
        _fullscreenAd.CustomData = DefaultFullscreenAdCustomData;
        var customData = _fullscreenAd.CustomData;
        var adLoadId = _fullscreenAd.LoadId;
        var bidInfo = _fullscreenAd.WinningBidInfo;
        var placementName = _fullscreenAd?.Request?.PlacementName;
        var loadId = loadResult.LoadId;
        var metrics = loadResult.Metrics;
        Log($"Fullscreen: {placementName} Loaded with: \nAdRequestId {adLoadId} \nRequestID {loadId} \nBidInfo: {JsonConvert.SerializeObject(bidInfo, Formatting.Indented)} \n Metrics:{JsonConvert.SerializeObject(metrics, Formatting.Indented)} \n Custom Data: {customData}");
    }

    public void OnInvalidateFullscreenClick()
    {
        if (_fullscreenAd == null)
        {
            Log("fullscreen ad does not exist");
            return;
        }

        _fullscreenAd.Invalidate();
        Log("interstitial ad does not exist");
    }

    public async void OnShowFullscreenClick()
    {
        if (_fullscreenAd == null)
            return;

        var adShowResult = await _fullscreenAd.Show();
        var error = adShowResult.error;
        
        if (adShowResult.error.HasValue)
        {
            Log($"Fullscreen Failed to Show with Value: {error.Value.code}, {error.Value.message}");
            return;
        }

        var metrics = adShowResult.metrics;
        Log($"Fullscreen Ad Did Show: {JsonConvert.SerializeObject(metrics, Formatting.Indented)}");
    }
    #endregion
    
    #region Banners
    private void SetupBannerDelegates()
    {
        ChartboostMediation.DidLoadBanner += DidLoadBanner;
        ChartboostMediation.DidClickBanner += DidClickBanner;
        ChartboostMediation.DidRecordImpressionBanner += DidRecordImpressionBanner;
    }

    public void OnCreateBannerClick()
    {
        var size = bannerSizeDropdown.value switch
        {
            2 => ChartboostMediationBannerAdSize.Leaderboard,
            1 => ChartboostMediationBannerAdSize.MediumRect,
            _ => ChartboostMediationBannerAdSize.Standard
        };

        _bannerAd?.Remove();

        Log("Creating banner on placement: " + bannerPlacementInputField.text + " with size: " + size);
        _bannerAd = ChartboostMediation.GetBannerAd(bannerPlacementInputField.text, size);
        
        if (_bannerAd == null)
        {
            Log("Banner not found");
            return;
        }

        // example keywords usage
        _bannerAd.SetKeyword("bnr_keyword1", "bnr_value1"); // accepted set
        _bannerAd.SetKeyword("bnr_keyword2", "bnr_value2"); // accepted set
        _bannerAd.SetKeyword(GenerateRandomString(65), "bnr_value2"); // rejected set
        _bannerAd.SetKeyword("bnr_keyword3", GenerateRandomString(257)); // rejected set
        _bannerAd.SetKeyword("bnr_keyword4", "bnr_value4"); // accepted set
        var keyword4 = this._bannerAd.RemoveKeyword("bnr_keyword4"); // removal of existing
        _bannerAd.RemoveKeyword("bnr_keyword4"); // removal of non-existing
        _bannerAd.SetKeyword("bnr_keyword5", keyword4); // accepted set using prior value
        _bannerAd.SetKeyword("bnr_keyword6", "bnr_value6"); // accepted set
        _bannerAd.SetKeyword("bnr_keyword6", "bnr_value6_replaced"); // accepted replace
        var screenPos = bannerLocationDropdown.value switch
        {
            0 => ChartboostMediationBannerAdScreenLocation.TopLeft,
            1 => ChartboostMediationBannerAdScreenLocation.TopCenter,
            2 => ChartboostMediationBannerAdScreenLocation.TopRight,
            3 => ChartboostMediationBannerAdScreenLocation.Center,
            4 => ChartboostMediationBannerAdScreenLocation.BottomLeft,
            5 => ChartboostMediationBannerAdScreenLocation.BottomCenter,
            6 => ChartboostMediationBannerAdScreenLocation.BottomRight,
            _ => ChartboostMediationBannerAdScreenLocation.TopCenter
        };
        _bannerAd.Load(screenPos);
    }

    public void OnRemoveBannerClick()
    {
        _bannerAd?.Remove();
        _bannerAd = null;
        Log("Banner Removed");
    }

    public void OnClearBannerClick()
    {
        if (_bannerAd == null)
        {
            Log("banner ad does not exist");
            return;
        }
        _bannerAd.ClearLoaded();
        Log("banner ad has been cleared");
    }

    public void OnToggleBannerVisibilityClick()
    {
        if (_bannerAd != null)
        {
            _bannerAdIsVisible = !_bannerAdIsVisible;
            _bannerAd.SetVisibility(_bannerAdIsVisible);
        }
        Log("Banner Visibility Toggled");
    }

    private void DidLoadBanner(string placementName, string loadId, BidInfo info, string error)
    {
        _bannerAdIsVisible = true;
        Log($"DidLoadBanner{placementName}: \nLoadId: ${loadId} \nPrice: ${info.Price:F4} \nAuction Id: {info.AuctionId} \nPartner Id: {info.PartnerId} \nError: {error}");
    }

    private void DidClickBanner(string placementName, string error) 
        => Log($"DidClickBanner {placementName}: {error}");

    private void DidRecordImpressionBanner(string placementName, string error) 
        => Log($"DidRecordImpressionBanner {placementName}: {error}");
    #endregion

    #region Utility

    private void Log(string text)
    {
        Debug.Log(text);
        if (outputText != null)
            outputText.text += $"{text}\n";
    }

    public void OnTestDestroyClick()
    {
        if (objectToDestroyForTest != null)
            Destroy(objectToDestroyForTest);
    }

    public void OnClearLogClick()
    {
        outputText.text = null;
        outputTextScrollRect.normalizedPosition = new Vector2(0, 1);
    }

    /// <summary>
    /// Generates a random string to help demonstrate the designed constraints
    /// of setting keywords with a word that is too long or a value that is too long.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <returns>A random string of the specified length.</returns>
    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new System.Random();
        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(stringChars);
    }

    private static void UnexpectedSystemErrorDidOccur(string error)
    {
        Debug.LogErrorFormat(error);
    }
    
    public static string JsonPrettify(string json)
    {
        
        using var stringReader = new StringReader(json);
        using var stringWriter = new StringWriter();
        var jsonReader = new JsonTextReader(stringReader);
        var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
        jsonWriter.WriteToken(jsonReader);
        return stringWriter.ToString();
    }
    #endregion
}
