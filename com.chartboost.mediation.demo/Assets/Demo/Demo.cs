using System;
using System.Collections;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Chartboost;
using Chartboost.Banner;
using Chartboost.FullScreen.Interstitial;
using Chartboost.FullScreen.Rewarded;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Demo : MonoBehaviour
{
#if UNITY_ANDROID
    private const string DefaultPlacementInterstitial = "CBInterstitial";
    private const string DefaultPlacementRewarded = "CBRewarded";
#else
    private const string DefaultPlacementInterstitial = "Startup";
    private const string DefaultPlacementRewarded = "Startup-Rewarded";
#endif
    private const string DefaultPlacementBanner = "AllNetworkBanner";
    private const string DefaultUserIdentifier = "123456";
    private const string DefaultRewardedAdCustomData = "{\"testkey\":\"testvalue\"}";

    // advertisement type selection
    public GameObject fullScreenPanel;
    public GameObject bannerPanel;

    // interstitial controls
    public InputField interstitialPlacementInputField;
    private ChartboostMediationInterstitialAd _interstitialAd;

    // rewarded controls
    public InputField rewardedPlacementInputField;
    private ChartboostMediationRewardedAd _rewardedAd;

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
        SetupInterstitialDelegates();
        SetupRewardedDelegates();
        SetupBannerDelegates();
    }

    private void Start()
    {
        if (outputText != null)
            outputText.text = string.Empty;
        fullScreenPanel.SetActive(true);
        bannerPanel.SetActive(false);

        interstitialPlacementInputField.SetTextWithoutNotify(DefaultPlacementInterstitial);
        rewardedPlacementInputField.SetTextWithoutNotify(DefaultPlacementRewarded);
        bannerPlacementInputField.SetTextWithoutNotify(DefaultPlacementBanner);

        ChartboostMediation.StartWithAppIdAndAppSignature(ChartboostMediationSettings.AppId, ChartboostMediationSettings.AppSignature);
    }

    private void OnDestroy()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.ClearLoaded();
            _interstitialAd.Destroy();
            Log("destroyed an existing interstitial");
        }
        if (_rewardedAd != null)
        {
            _rewardedAd.ClearLoaded();
            _rewardedAd.Destroy();
            Log("destroyed an existing rewarded");
        }
        if (_bannerAd != null)
        {
            _bannerAd.ClearLoaded();
            _bannerAd.Destroy();
            Log("destroyed an existing banner");
        }
    }

    private void DidStart(string error)
    {
        Log($"DidStart: {error}");
        ChartboostMediation.SetUserIdentifier(DefaultUserIdentifier);

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

    #region Interstitials

    private void SetupInterstitialDelegates()
    {
        ChartboostMediation.DidLoadInterstitial += DidLoadInterstitial;
        ChartboostMediation.DidShowInterstitial += DidShowInterstitial;
        ChartboostMediation.DidCloseInterstitial += DidCloseInterstitial;
        ChartboostMediation.DidClickInterstitial += DidClickInterstitial;
        ChartboostMediation.DidRecordImpressionInterstitial += DidRecordImpressionInterstitial;
    }

    public void OnCacheInterstitialClick()
    {
        _interstitialAd = ChartboostMediation.GetInterstitialAd(interstitialPlacementInputField.text);

        if (_interstitialAd == null)
        {
            Log("Interstitial Ad not found");
            return;
        }

        // example keywords usage
        _interstitialAd.SetKeyword("i12_keyword1", "i12_value1"); // accepted set
        _interstitialAd.SetKeyword("i12_keyword2", "i12_value2"); // accepted set
        _interstitialAd.SetKeyword(GenerateRandomString(65), "i12_value2"); // rejected set
        _interstitialAd.SetKeyword("i12_keyword3", GenerateRandomString(257)); // rejected set
        _interstitialAd.SetKeyword("i12_keyword4", "i12_value4"); // accepted set
        var keyword4 = this._interstitialAd.RemoveKeyword("i12_keyword4"); // removal of existing
        _interstitialAd.RemoveKeyword("i12_keyword4"); // removal of non-existing
        _interstitialAd.SetKeyword("i12_keyword5", keyword4); // accepted set using prior value
        _interstitialAd.SetKeyword("i12_keyword6", "i12_value6"); // accepted set
        _interstitialAd.SetKeyword("i12_keyword6", "i12_value6_replaced"); // accepted replace

        _interstitialAd.Load();
    }

    public void OnClearInterstitialClick()
    {
        if (_interstitialAd == null)
        {
            Log("interstitial ad does not exist");
            return;
        }

        _interstitialAd.ClearLoaded();
        Log("interstitial ad has been cleared");
    }

    public void OnShowInterstitialClick()
    {
        if (_interstitialAd.ReadyToShow())
            _interstitialAd.Show();
    }

    private void DidLoadInterstitial(string placementName, string loadId, BidInfo info, string error) 
        => Log($"DidLoadInterstitial {placementName}: \nLoadId: ${loadId} \nPrice: ${info.Price:F4} \nAuction Id: {info.AuctionId} \nPartner Id: {info.PartnerId} \nError: {error}");

    private  void DidShowInterstitial(string placementName, string error) 
        => Log($"DidShowInterstitial {placementName}: {error}");

    private void DidCloseInterstitial(string placementName, string error) 
        => Log($"DidCloseInterstitial {placementName}: {error}");
    
    private void DidClickInterstitial(string placementName, string error) 
        => Log($"DidClickInterstitial {placementName}: {error}");

    private void DidRecordImpressionInterstitial(string placementName, string error) 
        => Log($"DidRecordImpressionInterstitial {placementName}: {error}");
    #endregion

    #region Rewarded

    private void SetupRewardedDelegates()
    {
        ChartboostMediation.DidLoadRewarded += DidLoadRewarded;
        ChartboostMediation.DidShowRewarded += DidShowRewarded;
        ChartboostMediation.DidCloseRewarded += DidCloseRewarded;
        ChartboostMediation.DidReceiveReward += DidReceiveReward;
        ChartboostMediation.DidClickRewarded += DidClickRewarded;
        ChartboostMediation.DidRecordImpressionRewarded += DidRecordImpressionRewarded;
    }

    public void OnCacheRewardedClick()
    {
        _rewardedAd = ChartboostMediation.GetRewardedAd(rewardedPlacementInputField.text);
        
        if (_rewardedAd == null)
        {
            Log("Rewarded Ad not found");
            return;
        }

        // example keywords usage
        _rewardedAd.SetKeyword("rwd_keyword1", "rwd_value1"); // accepted set
        _rewardedAd.SetKeyword("rwd_keyword2", "rwd_value2"); // accepted set
        _rewardedAd.SetKeyword(GenerateRandomString(65), "rwd_value2"); // rejected set
        _rewardedAd.SetKeyword("rwd_keyword3", GenerateRandomString(257)); // rejected set
        _rewardedAd.SetKeyword("rwd_keyword4", "rwd_value4"); // accepted set
        var keyword4 = this._rewardedAd.RemoveKeyword("rwd_keyword4"); // removal of existing
        _rewardedAd.RemoveKeyword("rwd_keyword4"); // removal of non-existing
        _rewardedAd.SetKeyword("rwd_keyword5", keyword4); // accepted set using prior value
        _rewardedAd.SetKeyword("rwd_keyword6", "rwd_value6"); // accepted set
        _rewardedAd.SetKeyword("rwd_keyword6", "rwd_value6_replaced"); // accepted replace

        // example custom data usage
        var bytesToEncode = Encoding.UTF8.GetBytes(DefaultRewardedAdCustomData);
        var encodedText = Convert.ToBase64String(bytesToEncode);
        _rewardedAd.SetCustomData(encodedText);

        _rewardedAd.Load();
    }

    public void OnClearRewardedClick()
    {
        if (_rewardedAd == null)
        {
            Log("rewarded ad does not exist");
            return;
        }
        _rewardedAd.ClearLoaded();
        Log("rewarded ad has been cleared");
    }

    public void OnShowRewardedClick()
    {
        if (_rewardedAd.ReadyToShow())
            _rewardedAd.Show();
    }

    private void DidLoadRewarded(string placementName, string loadId, BidInfo info, string error)
        => Log($"DidLoadRewarded {placementName} \nLoadId: ${loadId} \nPrice: ${info.Price:F4} \nAuction Id: {info.AuctionId} \nPartner Id: {info.PartnerId} \nError: {error}");

    private void DidShowRewarded(string placementName, string error) 
        => Log($"DidShowRewarded {placementName}: {error}");

    private void DidCloseRewarded(string placementName, string error) 
        => Log($"DidCloseRewarded {placementName}: {error}");

    private void DidClickRewarded(string placementName, string error) 
        => Log($"DidClickRewarded {placementName}: {error}");

    private void DidRecordImpressionRewarded(string placementName, string error) 
        => Log($"DidRecordImpressionRewarded {placementName}: {error}");
    
    private void DidReceiveReward(string placementName, string error) 
        => Log($"DidReceiveReward {placementName}: {error}");
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
