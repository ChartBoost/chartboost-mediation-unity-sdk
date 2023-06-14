# Loading Ads

## Loading Fullscreen Placements

Since Chartboost Mediation Unity SDK 4.3.X we have deprecated the previous approach for loading fullscreen placements. A new Fullscreen API has been provided for `interstitials`, `rewarded videos` and `rewarded interstitials`, the new API makes use of C# asycn/await methods in order to await for load and show. A detailed example on the load logic for fullscreen placements can be found below:

```c#
// Strong reference to cached ad.
private IChartboostMediationFullscreenAd _fullscreenPlacement;

...
// keywords are optional
var keywords = new Dictionary<string, string> { { "i12_keyword1", "i12_value1" } };

// Create a Fullscreen Ad Load Request
var loadRequest = new ChartboostMediationFullscreenAdLoadRequest(FULLSCREEN_PLACEMENT, keywords);

// Subscribing Instance Delegates
loadRequest.DidClick += fullscreenAd => Log($"DidClick Name: {fullscreenAd.Request.PlacementName}");

loadRequest.DidClose += (fullscreenAd, error) => 
Debug.Log(!error.HasValue ? $"DidClose Name: {fullscreenAd.Request.PlacementName}"
: $"DidClose with Error. Name: {fullscreenAd.Request.PlacementName}, Code: {error?.Code}, Message: {error?.Message}");

loadRequest.DidReward += fullscreenAd => Log($"DidReward Name: {fullscreenAd.Request.PlacementName}");

loadRequest.DidRecordImpression += fullscreenAd => Log($"DidImpressionRecorded Name: {fullscreenAd.Request.PlacementName}");

loadRequest.DidExpire += fullscreenAd => Log($"DidExpire Name: {fullscreenAd.Request.PlacementName}");

// Await on FullscreenAd Load
var loadResult = await ChartboostMediation.LoadFullscreenAd(loadRequest);


// Failed to Load
if (loadResult.Error.HasValue)
{
    Debug.Log($"Fullscreen Failed to Load: {loadResult.Error?.Code}, message: {loadResult.Error?.Message}");
    return;
}

// Successful Load!

// Set strong reference to cached ad
_fullscreenPlacement = loadResult.AD;

// Set custom data before show
_fullscreenPlacement.CustomData = "CUSTOM DATA HERE!";

var placementName = _fullscreenAd?.Request?.PlacementName;
Debug.Log($"Fullscreen Placement Loaded with PlacementName: {placementName}")
```

> **Note** \
> The new fullscreen API supports multiple placement loads of the same placement. It is important to properly manage your ad instances if you are planning to create an Ad Queue system.

> **Warning** \
> The new fullscreen API utilizes instance based callbacks to notify information regarding the advertisement life-cycle. You must take this into account when migrating from the old API static callbacks.

## Creating Banner Ad Objects

To show a banner ad, first declare a variable to hold a reference to the Banner Ad. Supply the corresponding Placement Name and the Banner Size.

> **Note** \
> The following banner sizes can be passed down. Some partners may not fill for some banner sizes.

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

> **Note** \
> Any auto refresh changes made on the dashboard will take approximately one hour to take effect and the SDK must be rebooted in order to pick up the changes once they are available.

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
// Old API
_interstitialAd.ClearLoaded();
_rewardedAd.ClearLoaded();

_bannerAd.ClearLoaded();
```

```c#
/// New fullscreen API
_fullscreenPlacement.Invalidate();
```

> **Warning** \
> `Invalidate` behaves similarly like `ClearLoaded` and `Destroy`. As such, once called, you must free your ad reference to avoid any possible issues. 
