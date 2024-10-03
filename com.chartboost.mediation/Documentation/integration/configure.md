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