using System;
using System.Collections;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Helium;
using Helium.Banner;
using Helium.FullScreen.Interstitial;
using Helium.FullScreen.Rewarded;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Demo : MonoBehaviour
{
#if UNITY_ANDROID
    private const string AppID = "5a4e797538a5f00cf60738d6";
    private const string AppSIG = "d29d75ce6213c746ba986f464e2b4a510be40399";
    private const string DefaultPlacementInterstitial = "CBInterstitial";
    private const string DefaultPlacementRewarded = "CBRewarded";
#else
    private const string AppID = "59c04299d989d60fc5d2c782";
    private const string AppSIG = "6deb8e06616569af9306393f2ce1c9f8eefb405c";
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
    private HeliumInterstitialAd _interstitialAd;

    // rewarded controls
    public InputField rewardedPlacementInputField;
    private HeliumRewardedAd _rewardedAd;

    // banner controls
    public InputField bannerPlacementInputField;
    public Dropdown bannerSizeDropdown;
    public Dropdown bannerLocationDropdown;
    private HeliumBannerAd _bannerAd;
    private bool _bannerAdIsVisible;

    public ScrollRect outputTextScrollRect;
    public Text outputText;

    public GameObject objectToDestroyForTest;

    #region Lifecycle

    private void Awake()
    {
        Application.targetFrameRate = 60;
        HeliumSDK.DidStart += DidStartHelium;
        HeliumSDK.DidReceivePartnerInitializationData += DidReceivePartnerInitializationData;
        HeliumSDK.DidReceiveImpressionLevelRevenueData += DidReceiveImpressionLevelRevenueData;
        HeliumSDK.UnexpectedSystemErrorDidOccur += UnexpectedSystemErrorDidOccur;
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

        HeliumSDK.StartWithAppIdAndAppSignature(AppID, AppSIG);
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

    private void DidStartHelium(string error)
    {
        Log($"DidStart: {error}");
        HeliumSDK.SetUserIdentifier(DefaultUserIdentifier);

        if (error != null) return;
        HeliumSDK.SetSubjectToGDPR(false);
        HeliumSDK.SetSubjectToCoppa(false);
        HeliumSDK.SetUserHasGivenConsent(true);
    }

    private void DidReceiveImpressionLevelRevenueData(string placement, Hashtable impressionData)
    {
        var json =  HeliumJson.Serialize(impressionData);
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
        HeliumSDK.DidLoadInterstitial += DidLoadInterstitial;
        HeliumSDK.DidShowInterstitial += DidShowInterstitial;
        HeliumSDK.DidCloseInterstitial += DidCloseInterstitial;
        HeliumSDK.DidClickInterstitial += DidClickInterstitial;
        HeliumSDK.DidRecordImpressionInterstitial += DidRecordImpressionInterstitial;
        HeliumSDK.DidWinBidInterstitial += DidWinBidInterstitial;
    }

    public void OnCacheInterstitialClick()
    {
        _interstitialAd = HeliumSDK.GetInterstitialAd(interstitialPlacementInputField.text);

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

    private void DidLoadInterstitial(string placementName, string loadId, string error) 
        => Log($"DidLoadInterstitial {placementName}: LoadId: ${loadId}. {error}");

    private  void DidShowInterstitial(string placementName, string error) 
        => Log($"DidShowInterstitial {placementName}: {error}");

    private void DidCloseInterstitial(string placementName, string error) 
        => Log($"DidCloseInterstitial {placementName}: {error}");
    
    private void DidWinBidInterstitial(string placementName, HeliumBidInfo info, string error) 
        => Log($"DidWinBidInterstitial {placementName}: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}: {error}");

    private void DidClickInterstitial(string placementName, string error) 
        => Log($"DidClickInterstitial {placementName}: {error}");

    private void DidRecordImpressionInterstitial(string placementName, string error) 
        => Log($"DidRecordImpressionInterstitial {placementName}: {error}");
    #endregion

    #region Rewarded

    private void SetupRewardedDelegates()
    {
        HeliumSDK.DidLoadRewarded += DidLoadRewarded;
        HeliumSDK.DidShowRewarded += DidShowRewarded;
        HeliumSDK.DidCloseRewarded += DidCloseRewarded;
        HeliumSDK.DidReceiveReward += DidReceiveReward;
        HeliumSDK.DidClickRewarded += DidClickRewarded;
        HeliumSDK.DidRecordImpressionRewarded += DidRecordImpressionRewarded;
        HeliumSDK.DidWinBidRewarded += DidWinBidRewarded;
    }

    public void OnCacheRewardedClick()
    {
        _rewardedAd = HeliumSDK.GetRewardedAd(rewardedPlacementInputField.text);
        
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

    private void DidLoadRewarded(string placementName, string loadId, string error)
        => Log($"DidLoadRewarded {placementName}, LoadId: {loadId}: {error}");

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

    private void DidWinBidRewarded(string placementName, HeliumBidInfo info, string error) 
        => Log($"DidWinBidRewarded {placementName}: {placementName}: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}: {error}");
    #endregion

    #region Banners
    private void SetupBannerDelegates()
    {
        HeliumSDK.DidLoadBanner += DidLoadBanner;
        HeliumSDK.DidClickBanner += DidClickBanner;
        HeliumSDK.DidRecordImpressionBanner += DidRecordImpressionBanner;
        HeliumSDK.DidWinBidBanner += DidWinBidBanner;
    }

    public void OnCreateBannerClick()
    {
        var size = bannerSizeDropdown.value switch
        {
            2 => HeliumBannerAdSize.Leaderboard,
            1 => HeliumBannerAdSize.MediumRect,
            _ => HeliumBannerAdSize.Standard
        };

        _bannerAd?.Remove();

        Log("Creating banner on placement: " + bannerPlacementInputField.text + " with size: " + size);
        _bannerAd = HeliumSDK.GetBannerAd(bannerPlacementInputField.text, size);
        
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
            0 => HeliumBannerAdScreenLocation.TopLeft,
            1 => HeliumBannerAdScreenLocation.TopCenter,
            2 => HeliumBannerAdScreenLocation.TopRight,
            3 => HeliumBannerAdScreenLocation.Center,
            4 => HeliumBannerAdScreenLocation.BottomLeft,
            5 => HeliumBannerAdScreenLocation.BottomCenter,
            6 => HeliumBannerAdScreenLocation.BottomRight,
            _ => HeliumBannerAdScreenLocation.TopCenter
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

    private void DidLoadBanner(string placementName, string loadId, string error)
    {
        _bannerAdIsVisible = true;
        Log($"DidLoadBanner{placementName}: LoadId: {loadId}, Error: {error}");
    }

    private void DidClickBanner(string placementName, string error) 
        => Log($"DidClickBanner {placementName}: {error}");

    private void DidRecordImpressionBanner(string placementName, string error) 
        => Log($"DidRecordImpressionBanner {placementName}: {error}");

    private void DidWinBidBanner(string placementName, HeliumBidInfo info, string error) 
        => Log($"DidWinBidBanner {placementName}: {placementName}: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}: {error}");
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
