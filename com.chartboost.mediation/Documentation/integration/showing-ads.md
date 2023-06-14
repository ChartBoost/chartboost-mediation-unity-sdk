# Showing Ads

## Showing Fullscreen Placements

Fullscreen Placements must be first loaded, see section [Loading Ads](loading-ads.md) for more information.

Similar to the new load API. The new Fullscreen API utilizes C# async/await in order to request ad show. See below for details on implementation:

```c#
if (_fullscreenPlacement == null)
    return;

var adShowResult = await _fullscreenPlacement.Show();
var error = adShowResult.Error;

// Failed to Show
if (adShowResult.Error.HasValue)
{
    Debug.Log($"Fullscreen Failed to Show with Value: {adShowResult.Error?.code}, {adShowResult.Error?.message}");
    return;
}

// Successful Show
var metrics = adShowResult.metrics;
Debug.Log($"Fullscreen Ad Did Show: {JsonConvert.SerializeObject(metrics, Formatting.Indented)}");
```

## Showing Banner Ads
Banners are now automatically shown after load, see section [Loading Ads](loading-ads.md) for more information.

## Releasing Chartboost Mediation Ads

To clear resources used by Chartboost Mediation Ads, you can use the methods associated with the respective Ad you have used.

```c#
private void OnDestroy()
{
    if (_fullscreenPlacement != null)
    {
        _fullscreenPlacement.Invalidate();
        Debug.Log("Invalidated an existing fullscreen");
    }
    if (_bannerAd != null)
    {
        _bannerAd.ClearLoaded();
        _bannerAd.Destroy();
        Debug.Log("Destroyed an existing banner");
    }
}
```
