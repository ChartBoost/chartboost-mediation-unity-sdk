# Chartboost Mediation - Fullscreen Ad Queue

As the name indicates, this is an managed queue for `IFullscreenAd` objects. Ad queueing is a new feature introduced in Chartboost Mediation `4.9.0.+` that allows queueing up multiple fullscreen ads to show in succession. This can reduce and potentially eliminate latency for ad experiences that require showing fullscreen ads back to back.

> **Note** \
> Banners and adaptive banners cannot be queued.

# IFullscreenAdQueue

`IFullscreenAdQueue` manages the pre-loading of fullscreen ads. Each `IFullscreenAdQueue` manages ads for one placement. To create a `IFullscreenAdQueue` object call `ChartboostMediation.GetFullscreenAdQueue` as seen in the example below:

```csharp
IFullscreenAdQueue adQueue = ChartboostMediation.GetFullscreenAdQueue("PLACEMENT_NAME");
```

# Keywords
The Chartboost Mediation SDKs introduces keywords: key-value pairs to enable real-time targeting on line items. Every load request made by the `IFullscreenAdQueue` object will include these keywords.

```csharp
... 
// Keywords to pass for IFullscreenAdQueue
var keywords = new Dictionary<string, string>
{
    { "key", "value" },
    { "key_2", "value_2" }
};

// assign Keywords to IFullscreenAdQueue
adQueue.Keywords = keywords;
```

> **Warning** \
> Keywords has restrictions for setting keys and values. The maximum characters allowed for keys is 64 characters. The maximum characters for values is 256 characters.

# Usage Example

```csharp
// Get Queue
IFullscreenAdQueue queue = ChartboostMediation.GetFullscreenAdQueue("PLACEMENT_NAME");
Debug.Log($"Queue capacity : {queue.QueueCapacity}");

// Monitor Queue
queue.DidUpdate += (adQueue, adLoadResult, numberOfAdsReady) => Debug.Log($"Queue Updated. NumberOfAdsReady : {numberOfAdsReady}");
queue.DidRemoveExpiredAd += (adQueue, numberOfAdsReady) => Debug.Log($"Removed expired ad. NumberOfAdsReady : {numberOfAdsReady}");

 // Start queue
 queue.Start();

// Wait for some time for the queue to load an ad or subscribe to `DidUpdate` 
// event as shown above to be notified when an ad is loaded into queue

// Load an ad from queue
if (queue.HasNextAd())
{
    // removes and returns the oldest ad in the queue 
    // and starts a new load request
    var fullscreenAd = queue.GetNextAd();   
 }

// The dashboard's queue size settings can be overridden at runtime.
// If this placement has a Queue Size setting of 2 and Max Queue Size
// is 4 or 5, this line of code will increase this queue's size to
// 4. But if Max Queue Size is smaller than the number passed to
// setQueueCapacity, the queue size will only be increased up to the
// allowed maximum. So if Max Queue Size is 3 then an error message
// will be logged and this queue's size will be changed to 3.
queue.SetCapacity(4);
  
// Stop queue
queue.Stop();

// Keywords can be set at any time, whether the queue is stopped or running.
// The next ad request sent by the running queue will use the keywords set here.
queue.Keywords = new Dictionary<string, string>()
{
    { "key", "value" }
};
```