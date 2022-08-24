# Configure Helium

## Test Mode
Helium 1.1.0 introduced a Test Mode method that will allow you to test your partner integrations and get their test ads. Please refer to the [Helium Android](https://developers.chartboost.com/docs/android-configure-helium#test-mode) and [Helium iOS](https://developers.chartboost.com/docs/ios-configure-helium-ad-settings#test-mode) integration documents on how to enable Test Mode.

## COPPA
COPPA Settings can be configured utilizing the following method:

```csharp
  HeliumSDK.SetSubjectToCoppa(true);
  // or
  HeliumSDK.SetSubjectToCoppa(false);
```

* By sending `setSubjectToCoppa (true)`, you indicate that you want your content treated as child-directed for purposes of COPPA. We will take steps to disable interest-based advertising for such ad requests.

* By sending `setSubjectToCoppa (false)`, you indicate that you don't want your content treated as child-directed for purposes of COPPA. You represent and warrant that your applications and services are not directed towards children and that you will not provide any information to Helium from a user under the age of 13.

## GDPR
```csharp
  HeliumSDK.SetSubjectToGDPR(true);
  // or
  HeliumSDK.SetSubjectToGDPR(false);
```

* By sending `setSubjectToGDPR (true)`, you indicate that GDPR is applied to this user from your application.

* By sending `setSubjectToGDPR (false)`, you indicate that GDPR is not applied to this user from your application.

```csharp
  HeliumSDK.SetUserHasGivenConsent(true);
  // or
  HeliumSDK.SetUserHasGivenConsent(false);
```

* By sending `setUserHasGivenConsent (true)`, you indicate that this user from your application has given consent to share personal data for behavioral targeted advertising.

* By sending `setUserHasGivenConsent (false)`, you indicate that this user from your application has not given consent to use its personal data for behavioral targeted advertising, so only contextual advertising is allowed.

## CCPA
```csharp
  HeliumSDK.SetCCPAConsent(true);
  // or
  HeliumSDK.SetCCPAConsent(false);
```

* By sending `setCCPAConsent (true)`, you indicate that this user from your application has given consent to share personal data for behavioral targeted advertising under CCPA regulation.

* By sending `setCCPAConsent (false)`, you indicate that this user from your application has not given consent to allow sharing personal data for behavioral targeted advertising under CCPA regulation.

> **_NOTE:_** Helium will send CCPA information to the bidding network and set the CCPA information for the adapters.

## Keywords
As of Helium 2.9.0, the Helium SDKs introduces keywords: key-value pairs to enable real-time targeting on line items.

### Set Keywords
To set keywords, you will need to first create a Helium ad object, then use the setKeyword method to add key-value keywords pair.

```csharp
// Create a Helium Ad object.
HeliumInterstitialAd interstitialAd = HeliumSdk.GetInterstitialAd(PLACEMENT_INTERSTITIAL);

HeliumRewardedAd rewardedAd = HeliumSdk.GetRewardedAd(PLACEMENT_REWARDED);

HeliumBannerAd bannerAd = HeliumSdk.GetBannerAd(PLACEMENT_BANNER, BANNER_SIZE);

// Set a Keyword
this.interstitialAd.SetKeyword("i12_keyword1", "i12_value1");
this.rewardedAd.SetKeyword("rwd_keyword1", "rwd_value1");
this.bannerAd.SetKeyword("bnr_keyword1", "bnr_value1");
```

### Remove Keywords
To remove keywords, simply use the removeKeyword method and pass the key you would like to remove.

```csharp
// Remove Keyword.
this.interstitialAd.RemoveKeyword("i12_keyword1");
this.rewardedAd.RemoveKeyword("rwd_keyword1");
this.bannerAd.RemoveKeyword("bnr_keyword1");
```

> **_NOTE:_** Keywords has restrictions for setting keys and values. The maximum characters allowed for keys is 64 characters. The maximum characters for values is 256 characters.
