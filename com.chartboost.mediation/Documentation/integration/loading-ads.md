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

Since Chartboost Mediation Unity SDK 4.6.X we have deprecated the previous approach for loading banner placements. A new Banner API `ChartboostMedaitionBannerView` has been provided which makes use of C# asycn/await methods in order to await for load and show and also provides support to load adaptive size banners.

Another API `ChartboostMediationUnityBannerAd` is also provided which allows usage of unity gameobjects to load a banner ad within it.

A detailed example on the load logic for both of these APIs can be found below :

###  Using `ChartboostMediationBannerView` API

```c#
// Get a bannerView
ChartboostMediationBannerView bannerView = ChartboostMediation.GetBannerView();

// Determine the maximum size to load using width and height
var size = ChartboostMediationBannerSize.Adaptive(width, height);

// create a load request with size and your placementName
var loadRequest = new ChartboostMediationBannerAdLoadRequest(placementName, size)

// Determine where on the screen you want to place this bannerView
var screenLocation = ChartboostMediationAdScreenLocation.TopRight;

// Load the banner ad
var loadResult = await bannerView.Load(loadRequest, screenLocation);
if(!loadResult.Error.HasValue)
{
    // loaded successfullly
}
```
Banners are now shown automatically after load, you may choose to use a different `ChartboostMediationBannerAdScreenLocation` position when calling the load method:

| Banner Ad Location Enum                                  | Enum Value | Position                                                        |
|:---------------------------------------------------------| :---       | :---                                                            |
| `ChartboostMediationBannerAdScreenLocation.TopLeft`      | 0          | Positions the banner to the top-left screen of the device.      |
| `ChartboostMediationBannerAdScreenLocation.TopCenter`    | 1          | Positions the banner to the top-center screen of the device.    |
| `ChartboostMediationBannerAdScreenLocation.TopRight`     | 2          | Positions the banner to the top-right screen of the device.     |
| `ChartboostMediationBannerAdScreenLocation.Center`       | 3          | Positions the banner to the center screen of the device.        |
| `ChartboostMediationBannerAdScreenLocation.BottomLeft`   | 4          | Positions the banner to the bottom-left screen of the device.   |
| `ChartboostMediationBannerAdScreenLocation.BottomCenter` | 5          | Positions the banner to the bottom-center screen of the device. |
| `ChartboostMediationBannerAdScreenLocation.BottomRight`  | 6          | Positions the banner to the bottom-right screen of the device.  |


If you want to place the bannerView using a custom screen location then you can do so by providing a screen co-ordinate(x, y) which denotes where the top-left corner of this bannerView will be placed
```C#
// 400 pixels from left
float x = ChartboostMediationConverters.PixelsToNative(400); 
// 200 pixels from bottom
float y = ChartboostMediationConverters.PixelsToNative(200); 

// Load the banner ad
await bannerView.Load(loadRequest, x, y);
```

### Using `ChartboostMediationUnityBannerAd` API

`ChartboostMediationUnityBannerAd` API enables loading of a bannerAd within a unity gameobject. 
To create such gameobject you can simply right-click in hierarchy window and select `Chartboost Mediation/UnityBannerAd`
![Creating UnityBannerAd](../images/create-unity-banner-ad.png)


```C#
// Get reference to ChartboostMediationUnityBannerAd created in Editor
public ChartboostMediationUnityBannerAd unityBannerAd;

// Load the banner ad inside this gameobject
var loadResult = await unityBannerAd.Load();

if(!loadResult.Error.HasValue)
{
    // loaded successfullly
}
```
If you want to create this gameobject at runtime you can make use of `ChartboostMediation.GetUnityBannerAd()` 
```C#
// Get new unityBannerAd created as a child of provided canvas
var canvas = FindObjectOfType<Canvas>(); 
ChartboostMediationUnityBannerAd unityBannerAd = ChartboostMediation.GetUnityBannerAd(placementName, canvas.transform);

// Load the banner ad inside this gameobject
var loadResult = await unityBannerAd.Load();

if(!loadResult.Error.HasValue)
{
    // loaded successfullly
}

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

/// New bannerView API
_bannerView.Reset();

/// New unityBannerAd API
_unityBannerAd.Reset()

```

> **Warning** \
> `Invalidate` behaves similarly like `ClearLoaded` and `Destroy`. As such, once called, you must free your ad reference to avoid any possible issues. 
