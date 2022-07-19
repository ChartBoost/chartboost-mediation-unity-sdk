# Loading Ads

## Creating Interstitial & Rewarded Ad Objects

To show an Interstitial or Rewarded Ads, first declare a variable to hold a reference to either the Interstitial or Rewarded Helium Ad. Supply the corresponding Placement Name you set up on your dashboard as the argument for each of these functions:

```c#
//Helium Interstitial Ad
private HeliumInterstitialAd _interstitialAd;

//Helium Rewarded Ad
private HeliumRewardedAd _rewardedAd;

...
_interstitialAd = HeliumSDK.GetInterstitialAd(PLACEMENT_INTERSTITIAL);
_rewardedAd = HeliumSDK.GetRewardedAd(PLACEMENT_REWARDED);
```

## Loading Interstitial & Rewarded Ads

You will need to create an instance for each Placement Name you want to use. Finally, make the call to load the ad:

```c#
_interstitialAd.Load();
_rewardedAd.Load();
```

You can implement delegates in your class to receive notifications about the success or failure of the ad loading process for both Interstitial and Rewarded formats. See section [Delegate Usage](delegate-usage.md) for more details.

## Creating Banner Ad Objects

To show a banner ad, first declare a variable to hold a reference to the Banner Helium Ad. Supply the corresponding Placement Name and the Banner Size.

> **_NOTE:_** The following banner sizes can be passed down. Some partners may not fill for some banner sizes.

| Banner Enum | Dimensions (Width x Height) |
| :---        | :---                        |
| Standard    | 320 x 50                    |
| Medium      | 300 x 250                   |
| Leaderboard | 728 x 90                    |

```c#
private HelliumBannerAd _bannerAd;

if (_bannerAd != null)
  return;

/*
  The following Banner enum Sizes can be passed down:
  HeliumBannerAdSize.Standard
  HeliumBannerAdSize.MediumRect
  HeliumBannerAdSize.Leaderboard
*/
HeliumBannerAdSize BANNER_SIZE = HeliumBannerAdSize.Standard;
_bannerAd = HeliumSDK.GetBannerAd(PLACEMENT_BANNER, BANNER_SIZE);
```

## Loading Banner Ads

You will need to create an instance for each Placement Name you want to use. Finally, make the call to load the ad:

```c#
_bannerAd.Load();
```

You can implement delegates in your class to receive notifications about the success or failure of the ad loading process for Banner formats. See section [Delegate Usage](delegate-usage.md) for more details.

## Clearing Loaded Ads

Sometimes, you may need to clear loaded ads on existing placements to request another ad (i.e. for an in-house programmatic auction). To do this:

```c#
_interstitialAd.ClearLoaded();
_rewardedAd.ClearLoaded();
_bannerAd.ClearLoaded();
```

The clearLoaded API returns a boolean and indicates if the ad object has been cleared and is ready for another load call.
