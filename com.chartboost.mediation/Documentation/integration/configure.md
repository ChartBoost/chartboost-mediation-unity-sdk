# Configure Chartboost Mediation

## TestMode
A `bool` flag for setting test mode in Chartboost Mediation. 

```csharp
ChartboostMediation.TestMode = true;
// or
ChartboostMediation.TestMode = false;
```

> **Warning** \
> Do not enable test mode in production builds.

## LogLevel
Sets the log level. Anything of that log level and lower will be emitted. Set this to `LogLevel.Disabled` for no logs.

```csharp
// No Logs
ChartboostMediation.LogLevel = LogLevel.Disable;

// All Logs
ChartboostMediation.LogLevel = LogLevel.Verbose;
```

## DiscardOverSizedAds
`bool` value indicating that ads returned from adapters that are larger than the requested size should be discarded. An ad is defined as too large if either the width or the height of the resulting ad is larger than the requested ad size unless the height of the requested ad size is 0, as is the case when using `BannerSizeType.Adaptive`, in this case, an error will be returned. This currently only applies to `IBannerAd`. Defaults to `false`.

```csharp
ChartboostMediation.DiscardOverSizedAds = true;
// or
ChartboostMediation.DiscardOverSizedAds = false;
```

## AdaptersInfo
An array of all initialized adapters, or an empty array if the SDK is not initialized.

```csharp
Debug.Log($"Printing Adapter Info: {JsonConvert.SerializeObject(ChartboostMediation.AdaptersInfo, Formatting.Indented)}");
```

## Callbacks

### DidReceiveImpressionLevelRevenueData
Event for receiving ILRD(Impression Level Revenue Data) events.

```csharp
ChartboostMediation.DidReceiveImpressionLevelRevenueData += (placement, impressionData) => Debug.Log($"Received ILRD for: {placement}, ILRD: {JsonTools.SerializeObject(impressionData, Formatting.Indented)}");
```

### DidReceivePartnerAdapterInitializationData
Event for receiving partner initialization result events.

```csharp
ChartboostMediation.DidReceivePartnerAdapterInitializationData += partnerInitializationData => Debug.Log($"Received Partner Initialization Data {JsonTools.SerializeObject(partnerInitializationData, Formatting.Indented)}");
```

#### Banner Ads

##### IBannerAd
For `IBannerAd` objects, keywords are set after obtaining the `IBannerAd` object when calling `ChartboostMediation.GetBannerAd`, seen in the example below:

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

##### UnityBannerAd

```csharp
// Keywords to pass for unity banner ad
var keywords = new Dictionary<string, string>
{
    { "key", "value" },
    { "key_2", "value_2" }
};

// Get UnityBannerAd
UnityBannerAd unityBannerAd = ChartboostMediation.GetUnityBannerAd("PLACEMENT_NAME");

// Set keywords
unityBannerAd.Keywords = keywords;
```

### Remove Keywords
To remove keywords, simply you will need to pass a new set without the key you want to remove, this is to facilitate the wrapping process between Unity and native platforms.


> **Warning** \
> Keywords has restrictions for setting keys and values. The maximum characters allowed for keys is 64 characters. The maximum characters for values is 256 characters.

```