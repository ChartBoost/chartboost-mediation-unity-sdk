# Loading Ads

## Creating Interstitial & Rewarded Ad Objects

To show an Interstitial or Rewarded Ads, first declare a variable to hold a reference to either the Interstitial or Rewarded Chartboost Mediation Ad. Supply the corresponding Placement Name you set up on your dashboard as the argument for each of these functions:

```c#
// Interstitial Ad
private ChartboostMediationInterstitialAd _interstitialAd;

// Rewarded Ad
private ChartboostMediationRewardedAd _rewardedAd;

...
_interstitialAd = ChartboostMediation.GetInterstitialAd(PLACEMENT_INTERSTITIAL);
_rewardedAd = ChartboostMediation.GetRewardedAd(PLACEMENT_REWARDED);
```

## Loading Interstitial & Rewarded Ads

You will need to create an instance for each Placement Name you want to use. Finally, make the call to load the ad:

```c#
_interstitialAd.Load();
_rewardedAd.Load();
```

You can implement delegates in your class to receive notifications about the success or failure of the ad loading process for both Interstitial and Rewarded formats. See section [Delegate Usage](delegate-usage.md) for more details.

## Creating Banner Ad Objects

To show a banner ad, first declare a variable to hold a reference to the Banner Ad. Supply the corresponding Placement Name and the Banner Size.

> **_NOTE:_** The following banner sizes can be passed down. Some partners may not fill for some banner sizes.

| Banner Enum   | Dimensions (Width x Height) |
| :---          | :---                        |
| `Standard`    | 320 x 50                    |
| `Medium`      | 300 x 250                   |
| `Leaderboard` | 728 x 90                    |

```c#
private ChartboostMediationBannerAd _bannerAd;

if (_bannerAd != null)
  return;

/*
  The following Banner enum Sizes can be passed down:
  ChartboostMediationBannerAdSize.Standard
  ChartboostMediationBannerAdSize.MediumRect
  ChartboostMediationBannerAdSize.Leaderboard
*/
ChartboostMediationBannerAdSize BANNER_SIZE = ChartboostMediationBannerAdSize.Standard;
_bannerAd = ChartboostMediation.GetBannerAd(PLACEMENT_BANNER, BANNER_SIZE);
```

Banners are now shown automatically after load, as such you will need to pass a `ChartboostMediationBannerAdScreenLocation` position when calling the load method:


| Banner Ad Location Enum                                  | Enum Value | Position                                                        |
|:---------------------------------------------------------| :---       | :---                                                            |
| `ChartboostMediationBannerAdScreenLocation.TopLeft`      | 0          | Positions the banner to the top-left screen of the device.      |
| `ChartboostMediationBannerAdScreenLocation.TopCenter`    | 1          | Positions the banner to the top-center screen of the device.    |
| `ChartboostMediationBannerAdScreenLocation.TopRight`     | 2          | Positions the banner to the top-right screen of the device.     |
| `ChartboostMediationBannerAdScreenLocation.Center`       | 3          | Positions the banner to the center screen of the device.        |
| `ChartboostMediationBannerAdScreenLocation.BottomLeft`   | 4          | Positions the banner to the bottom-left screen of the device.   |
| `ChartboostMediationBannerAdScreenLocation.BottomCenter` | 5          | Positions the banner to the bottom-center screen of the device. |
| `ChartboostMediationBannerAdScreenLocation.BottomRight`  | 6          | Positions the banner to the bottom-right screen of the device.  |

If you enable auto-refresh for a banner placement in the dashboard, then the Chartboost Mediation Unity SDK will apply that setting when the placement is shown.

> **_NOTE:_** Any auto refresh changes made on the dashboard will take approximately one hour to take effect and the SDK must be rebooted in order to pick up the changes once they are available.

You will need to create an instance for each Placement Name you want to use. Finally, make the call to load the ad:

```c#

/* All possible banner locations
 * ChartboostMediationBannerAdScreenLocation.TopLeft,
 * ChartboostMediationBannerAdScreenLocation.TopCenter,
 * ChartboostMediationBannerAdScreenLocation.TopRight,
 * ChartboostMediationBannerAdScreenLocation.Center,
 * ChartboostMediationBannerAdScreenLocation.BottomLeft,
 * ChartboostMediationBannerAdScreenLocation.BottomCenter,
 * ChartboostMediationBannerAdScreenLocation.BottomRight,
 * ChartboostMediationBannerAdScreenLocation.TopCenter
 */

// Load a banner on the top center location
_bannerAd.Load(ChartboostMediationBannerAdScreenLocation.TopCenter);
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
