# Showing Ads

## Showing Interstitial and Rewarded Ads

When you are ready to show a Rewarded or Interstitial ad, you can check that it is ready to show and then display it like so:

### Interstitial Ad

```c#
// Showing a Interstitial Ad
if (_interstitialAd.ReadyToShow())
    _interstitialAd.Show();
```

### Rewarded Ad

```c#
//Showing a Rewarded Ad
if (_rewardedAd.ReadyToShow()){
  _rewardedAd.show();
```

## Showing Banner Ads
Banners are now automatically shown after load, see section [Loading Ads](loading-ads.md) for more information.

## Releasing Charboost Mediation Ads

To clear resources used by Charboost Mediation Ads, you can use the destroy method associated with the respective Ad you have used.

```c#
private void OnDestroy()
{
    if (_interstitialAd != null)
    {
        _interstitialAd.ClearLoaded();
        _interstitialAd.Destroy();
        Debug.Log("Destroyed an existing interstitial");
    }
    if (_rewardedAd != null)
    {
        _rewardedAd.ClearLoaded();
        _rewardedAd.Destroy();
        Debug.Log("Destroyed an existing rewarded");
    }
    if (_bannerAd != null)
    {
        _bannerAd.ClearLoaded();
        _bannerAd.Destroy();
        Debug.Log("Destroyed an existing banner");
    }
}

```
