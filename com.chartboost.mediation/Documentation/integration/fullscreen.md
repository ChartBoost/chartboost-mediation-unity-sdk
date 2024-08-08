# Chartboost Mediation - Fullscreen Ads

As the name indicates, this kind of ad typically take over the entire screen of the mobile device. For more information on available formats for `IFullscreenAds` vist [Ad Formats](https://docs.chartboost.com/en/mediation/ad-formats/)

# `FullscreenAdLoadRequest`

`FullscreenAdLoadRequest` objects contains publisher provided configurations for `IFullscreenAds`. It is used when calling `ChartboostMediation.LoadFullscreenAd`, as seen in the examples below.

```csharp
// Create FullscreenAdLoadRequest with no keywords nor partner settings
FullscreenAdLoadRequest fullscreenAdRequest = new FullscreenAdLoadRequest("FULLSCREEN_PLACEMENT_ID");
```

## Keywords 
As of Chartboost Mediation 2.9.0, the Chartboost Mediation SDKs introduces keywords: key-value pairs to enable real-time targeting on line items.
For `IFullscreenAd` objects, keywords are passed in the `FullscreenAdLoadRequest` object when calling `ChartboostMediation.LoadFullscreenAd`, seen in the example below:

```csharp
// Keywords to pass for fullscreen ad
var keywords = new Dictionary<string, string>
{
    { "key", "value" },
    { "key_2", "value_2" }
};

// Create FullscreenAdLoadRequest with keywords
FullscreenAdLoadRequest fullscreenAdRequest = new FullscreenAdLoadRequest("FULLSCREEN_PLACEMENT_ID", keywords);
```

> **Warning** \
> Keywords has restrictions for setting keys and values. The maximum characters allowed for keys is 64 characters. The maximum characters for values is 256 characters.

## Partner Settings 
An optional `IDictionary<string, string>` that a publisher would like to send to all partners.

```csharp
// Partner settings
var partnerSettings = new Dictionary<string, string>
{
    { "setting_1", "value" },
    { "setting2", "value_2" }
};

// Create FullscreenAdLoadRequest with keywords previous example and a partnersettings.
FullscreenAdLoadRequest fullscreenAdRequest = new FullscreenAdLoadRequest("FULLSCREEN_PLACEMENT_ID", keywords, partnerSettings);
```

# Load

In order to load `IFullscreenAd` objects, you will need to call `ChartboostMediation.LoadFullscreenAd`, as seen in the examples below:

## Load in async Context

```csharp
// Load `IFullscreenAd` using async approach and our previously built FullscreenAdLoadRequest
FullscreenAdLoadResult fullscreenAdLoadResult = await ChartboostMediation.LoadFullscreenAd(fullscreenAdRequest);
...
```

## Load in sync Context
A lot of APIs provided in the Chartboost Mediation Unity SDK utilize the async/await C# implementation. It is possible for developers to try to call the following code from a sync context where async/await might not be supported:

```csharp

ChartboostMediation.LoadFullscreenAd(fullscreenAdRequest).ContinueWithOnMainThread(continuation =>
{
    FullscreenAdLoadResult fullscreenAdLoadResult = continuation.Result;
    ... 
});
```

## `FullscreenAdLoadResult`

The `FullscreenAdLoadResult` object contains information for the requested `IFullscreenAd` load request. The information inside can be used as follows:

```csharp
// Check if an error occurred
ChartboostMediationError? error = fullscreenAdLoadResult.Error;

// Load failed
if (error.HasValue)
{
    // Report load failure
    Debug.LogError($"`IFullscreenAd` Load failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Load succeeded 

// Parse load result data
var loadId = fullscreenAdLoadResult.LoadId;
var winningBidInfoJson = JsonTools.SerializeObject(fullscreenAdLoadResult.WinningBidInfo, Formatting.Indented);
var metricsJson = JsonTools.SerializeObject(fullscreenAdLoadResult.Metrics, Formatting.Indented);

// Report fullscreen ad load result information
Debug.Log($"`IFullscreenAd` load completed with: LoadId{loadId} WinningBidInfo: {winningBidInfoJson}, Metrics: {metricsJson}");

// Obtain IFullscreenAd
IFullscreenAd ad = fullscreenAdLoadResult.Ad;

// Subscribing IFullscreenAd Instance Delegates
ad.DidClick += fullscreenAd => Log($"DidClick Name: {fullscreenAd.Request.PlacementName}");

ad.DidClose += (fullscreenAd, error) => Debug.Log(!error.HasValue ? $"DidClose Name: {fullscreenAd.Request.PlacementName}" : $"DidClose with Error. Name: {fullscreenAd.Request.PlacementName}, Code: {error?.Code}, Message: {error?.Message}");

ad.DidReward += fullscreenAd => Log($"DidReward Name: {fullscreenAd.Request.PlacementName}");

ad.DidRecordImpression += fullscreenAd => Log($"DidImpressionRecorded Name: {fullscreenAd.Request.PlacementName}");

ad.DidExpire += fullscreenAd => Log($"DidExpire Name: {fullscreenAd.Request.PlacementName}");
```

> **Note** \
> The fullscreen API supports multiple placement loads of the same placement. It is important to properly manage your ad instances if you are planning to create an Ad Queue system.

> **Warning** \
> The fullscreen API utilizes instance based callbacks to notify information regarding the advertisement life-cycle.

## Custom Data

Custom data may be set at any time before calling `Show()`

```csharp
...
IFullscreenAd ad = fullscreenAdLoadResult.Ad;
// Set custom data
ad.CustomData = "SOME_CUSTOM_DATA";
```

> **Warning** \
> The `CustomData` property is found on the `IFullsreenAd` instance, and has a maximum character limit of `1000` characters. In the event that the limit is exceeded, the `CustomData` property will be set to null.

# Show

Once the load process for the `IFullscreenAd` object has been completed, you will need to call `IFullscreenAd.Show`, as seen in the examples below:

## Show in async Context
```csharp
...
// Show `IFullscreenAd` using async approach
AdShowResult adShowResult = await ad.Show();
...
```

## Show in sync Context
A lot of APIs provided in the Chartboost Mediation Unity SDK utilize the async/await C# implementation. It is possible for developers to try to call the following code from a sync context where async/await might not be supported:

```csharp
...
// Show `IFullscreenAd` using async approach
ad.Show().ContinueWithOnMainThread(continuation =>
{
    AdShowResult adShowResult = continuation.Result;
    ...
});
```

### `AdShowResult`
The `AdShowResult` object contains information for the requested `IFullscreenAd` show result. The information inside can be used as follows:

```csharp
...
// Check if IFullscreenAd failed to show
var error = adShowResult.Error;

// Failed to show
if (error.HasValue)
{
    // Report show failure
    Debug.LogError($"`IFullscreenAd` Show failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Show succeeded

// Report metrics and show success
var metricsJson = JsonTools.SerializeObject(adShowResult.Metrics, Formatting.Indented);
Debug.Log($"`IFullscreenAd` show completed with: Metrics: {metricsJson}");
...
```

# Dispose

`IFullscreenAd` objects implement the [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0) to properly dispose of managed and unmanaged resources.

You can free up resources directly by calling `IFullscreenAd.Dispose`

```csharp
...
IFullscreenAd ad = fullscreenAdLoadResult.Ad;

// Do what we need to do with our IFullscreenAd

// Free up resources
ad.Dispose();
```

> **Note** \
> While this is not a necessary call, since `IFullscreenAd` objects will be disposed by the garbage collector once they are no longer referenced. It is still a good practice to disposed of unmanaged resources when no longer needed.
