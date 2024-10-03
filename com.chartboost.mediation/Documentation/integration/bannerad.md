# Chartboost Mediation - Banner Ad

Chartboost Mediation SDK 4.6.0 introduced a new Adaptive Banner ad format, capable of serving flexible and fixed sized ads in the placement. The new Adaptive Banner ad format has the following features:

* Publishers can choose whether to use Adaptive Ads or Fixed Ads in a given placement.
* Fixed Ads are supported in Adaptive Ad placements (backwards compatible).
* Publishers should know whether an ad is fixed or flexible and receive the dimensions of fixed ads.
* Publishers can align the ad horizontally and/or vertically.
* Publishers can resize the ad container to fit the ad or optionally discard oversized ads that are rendered in the container.
* The ad container can be in-line or on top of publisher content.

To use this new ad format, Publishers will need to create a new Adaptive Banner placement in their platform and integrate with the new Adaptive Banner APIs.

Another API `UnityBannerAd` is also provided which allows usage of Unity Gameobjects to load a banner ad within it.

# `BannerAdLoadRequest`

`BannerAdLoadRequest` objects contains publisher provided configurations for `IBannerAd` objects. It is used when calling `IBannerAd.Load`, as seen in the examples below.

```csharp
// Determine the maximum size to load using width and height
BannerSize size = BannerSize.Adaptive(100, 100);
BannerAdLoadRequest loadRequest = new BannerAdLoadRequest("BANNER_PLACEMENT_NAME", size);
```

# Loading `IBannerAd` Objects

A detailed example on the load logic for this API can be found below:

```csharp
// Get a bannerAd
IBannerAd bannerAd = ChartboostMediation.GetBannerAd();

// Place it at the center of screen
bannerAd.Position = new Vector2(
    DensityConverters.PixelsToNative(Screen.width/2f),
    DensityConverters.PixelsToNative(Screen.height/2f)
);

// Set pivot
bannerAd.Pivot = new Vector2(0.5f, 0.5f);

// Set banner ad callbacks
bannerAd.WillAppear += ad => Debug.Log($"BannerAd: {ad.LoadId} will appear.");
bannerAd.DidClick += ad => Debug.Log($"BannerAd: {ad.LoadId} was clicked.");
bannerAd.DidDrag += (ad,x,y) => Debug.Log($"BannerAd: {ad.LoadId} was dragged x:{x}/y:{y}.");
bannerAd.DidRecordImpression += ad => Debug.Log($"BannerAd: {ad.LoadId} was clicked.");


// create a load request with size and your placementName
var loadRequest = new BannerAdLoadRequest(
    "BANNER_PLACEMENT_NAME",
    BannerSize.Adaptive6X1(width)    // This can be any other size or the old non-adaptive size like `BannerSize.Standard`
);

...
```

# Keywords

Keywords are set after obtaining the IBannerAd object when calling ChartboostMediation.GetBannerAd. To remove keywords, pass a new set without those keywords. The list will be overridden. This is to facilitate the wrapping process between Unity and native platforms.

> **Warning** \
> Keywords has restrictions for setting keys and values. The maximum characters allowed for keys is 64 characters. The maximum characters for values is 256 characters.

```csharp
// Keywords to pass for banner ad
var keywords = new Dictionary<string, string>
{
    { "key", "value" },
    { "key_2", "value_2" }
};

// Get banner ad
IBannerAd bannerAd = ChartboostMediation.GetBannerAd();

// Set keywords
bannerAd.Keywords = keywords;
```

## Loading in async Context
```csharp
...
// Load the banner ad with our previously configured load request
BannerAdLoadResult loadResult = await bannerAd.Load(loadRequest);
if(loadResult.Error.HasValue)
{
    // report load error
    return;
}
// loaded successfully
```

## Loading in sync Context
A lot of APIs provided in the Chartboost Mediation Unity SDK utilize the async/await C# implementation. It is possible for developers to try to call the following code from a sync context where async/await might not be supported:

```csharp
// Show `IBannerAd` using async approach
bannerAd.Load(loadRequest).ContinueWithOnMainThread(continuation =>
{
    loadResult = continuation.Result;
});
```

# `BannerAdLoadResult`

`BannerAdLoadResult` contains information regarding the load for `IBannerAd` objects. Its result can be used as seen below:

```csharp
// Check if IBannerAd failed to load
var error = adShowResult.Error;

// Failed to load
if (error.HasValue)
{
    // Report load failure
    Debug.LogError($"`IBannerAd` Load failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Load succeeded

// Report metrics and show success
var loadId = loadResult.LoadId;
var metricsJson = JsonTools.SerializeObject(loadResult.Metrics, Formatting.Indented);
var winningBidInfo = JsonTools.SerializeObject(loadResult.WinningBidInfo, Formatting.Indented);
Debug.Log($"`IBannerAd` loaded successfully with:\n" +
          $"LoadId: {loadId}\n" +
          $"Metrics: {metricsJson}\n" +
          $"Winning Bid Info: {winningBidInfo}");

```

# Dispose

`IBannerAd` objects implement the [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0) to properly dispose of managed and unmanaged resources.

You can free up resources directly by calling `IBannerAd.Dispose`

```csharp
...
IBannerAd ad = ChartboostMediation.GetBannerAd();

// Do what we need to do with our IBannerAd

// Free up resources
ad.Dispose();
```

> **Note** \
> While this is not a necessary call, since `IBannerAd` objects will be disposed by the garbage collector once they are no longer referenced. It is still a good practice to disposed of unmanaged resources when no longer needed.
