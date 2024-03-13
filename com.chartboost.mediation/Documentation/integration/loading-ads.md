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

### Queueing Fullscreen Ads
Ad queueing is a new feature for SDK 4.9.0+ that builds upon the existing fullscreen ad experience that allows publishers to queue up multiple fullscreen ads and show them in succession. This can reduce and potentially eliminate latency for ad experiences that require showing fullscreen ads back to back. 



 Queues are a "singleton per placement", meaning that if attempted to create multiple queues with the same placement ID the same object will be returned each time.

```csharp

// Get Queue
var queue = ChartboostMediationFullscreenAdQueue.Queue("placementName");
Debug.Log($"Queue capacity : {queue.QueueCapacity}");

// Monitor Queue
queue.DidUpdate += (adQueue, adLoadResult, numberOfAdsReady)
     => Debug.Log($"Queue Updated. NumberOfAdsReady : {numberOfAdsReady}");
 queue.DidRemoveExpiredAd += (adQueue, numberOfAdsReady)
     => Debug.Log($"Removed expired ad. NumberOfAdsReady : {numberOfAdsReady}");

 // Start queue
 queue.Start();

// Wait for some time for the queue to load an ad or subscribe to `DidUpdate` event as shown above
// to be notified when an ad is loaded into queue

// Load an ad from queue
if (queue.HasNextAd())
{
    // removes and returns the oldest ad in the queue 
    // and starts a new load request
    var fullscreenAd = queue.GetNextAd();   
 }
  
// Stop queue
queue.Stop();

```

## Banner Ad Objects

Mediation SDK 4.6.0 introduces a new [Adaptive Banner](https://docs.chartboost.com/en/mediation/ad-types/#adaptive-banner/) ad format, capable of serving flexible and fixed sized ads in the placement. The new [Adaptive Banner]([/en/mediation/ad-types/#adaptive-banner/](https://docs.chartboost.com/en/mediation/ad-types/#adaptive-banner/)) ad format has the following features:
- Publishers can choose whether to use Adaptive Ads or Fixed Ads in a given placement.
- Fixed Ads are supported in Adaptive Ad placements (backwards compatible).
- Publishers should know whether an ad is fixed or flexible and receive the dimensions of fixed ads.
- Publishers can align the ad horizontally and/or vertically.
- Publishers can resize the ad container to fit the ad or optionally discard oversized ads that are rendered in the container.
- The ad container can be in-line or on top of publisher content.

To use this new ad format, Publishers will need to create a new Adaptive Banner placement in their platform and integrate with the new Adaptive Banner APIs.

> **Caution** \
> Google Bidding does not support 0 height adaptive banner sizes and will result in no fill.


We have added a new Banner API `ChartboostMedaitionBannerView` to allow usage of adaptive banners. The previous `ChartboostMedaitionBannerAd` API has now been deprecated.


Another API `ChartboostMediationUnityBannerAd` is also provided which allows usage of unity gameobjects to load a banner ad within it.

The API class `ChartboostMediationBannerSize` will support both fixed and adaptive banners:

|Field|Description|
|--|--|
|`static ChartboostMediationBannerSize Standard`|Static constant that returns a fixed `STANDARD` banner with a size of 320x50.|
|`static ChartboostMediationBannerSize MediumRect`|Static constant that returns a fixed MREC `MEDIUM` banner with a size of 300x250.|
|`static ChartboostMediationBannerSize Leaderboard`|Static constant that returns a fixed `LEADERBOARD` banner with a size of 728x90.|
|`ChartboostMediationBannerType BannerType`|An enum specifying that the ad size is fixed (size cannot change), or adaptive (size can change but must maintain aspectRatio). This is an integer based enum.|
|`float Width`|The width of the ad.|
|`float Height`|The height of the ad.|
|`float AspectRatio`|The aspect ratio of the ad. This can be derived from the size. Will be 0 if either the width or the height are <= 0.|
|`static ChartboostMediationBannerSize Adaptive(float width)`|Creates a flexible/adaptive banner size with 0 height.|
|`static ChartboostMediationBannerSize Adaptive(float width, float height)`|Creates a flexible/adaptive banner size with a width and max height. Used either when the height of an inline ad should be capped, or when requesting an anchored banner.|
|`<additional conveniences>`|This provides additional conveniences to create sizes based on the IAB ratios (e.g. 6:1, 1:1) with a width. For example, using the 6:1 convenience with a 600 width would return a size of 600x100. Note that these are max sizes, therefore smaller sizes or other aspect ratios may be served.|


### Loading Banner ads
A detailed example on the load logic for both of these APIs can be found below :

####  Using `ChartboostMediationBannerView` API

```c#
// Get a bannerView
IChartboostMediationBannerView bannerView = ChartboostMediation.GetBannerView();

// Determine the maximum size to load using width and height
var size = ChartboostMediationBannerSize.Adaptive(width, height);

// create a load request with size and your placementName
var loadRequest = new ChartboostMediationBannerAdLoadRequest(placementName, size)

// Determine where on the screen you want to place this bannerView
var screenLocation = ChartboostMediationBannerAdScreenLocation.TopRight;

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


To place the `bannerView` using a custom screen location, provide a screen coordinate (x, y) which denotes where the top-left corner of this bannerView will be placed.
```C#
// 400 pixels from left
float x = ChartboostMediationConverters.PixelsToNative(400); 
// 200 pixels from bottom
float y = ChartboostMediationConverters.PixelsToNative(200); 

// Load the banner ad
await bannerView.Load(loadRequest, x, y);
```

#### Using `ChartboostMediationUnityBannerAd` API

`ChartboostMediationUnityBannerAd` API enables loading of a bannerAd within a unity gameobject. 
To create this gameobject, right-click in the hierarchy window and select `Chartboost Mediation/UnityBannerAd`
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

### Resizing Adaptive Banner Container

#### Using `ChartboostMediationBannerView` API

```C#
bannerView.DidLoad += (banner) => 
{
    // Determine the axis on which you want the bannerView to resize
    var resizeAxis = ChartboostMediationBannerResizeAxis.Vertical; 
    
    // Make the resize call
    bannerView.ResizeToFit(resizeAxis);
}

```

#### Using `ChartboostMediationUnityBannerAd` API
```C#
// Determine the resizeOption you want to set on this gameobject
ResizeOption resizeOption = ResizeOption.FitVertical;

// Update the resizeOption 
unityBannerAd.ResizeOption = resizeOption

```

### Discarding Oversized Ads

```C#
// To drop oversized ads
ChartboostMediation.DiscardOversizedAds(true)

// To allow oversized ads
ChartboostMediation.DiscardOversizedAds(false)

```

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
