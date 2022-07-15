# Showing Ads

## Showing Interstitial and Rewarded Ads

When you are ready to show a Rewarded or Interstitial ad, you can check that it is ready to show and then display it like so:

### Interstitial Ad

```c#
// Showing a Helium Interstitial Ad
if (_interstitialAd.ReadyToShow())
    _interstitialAd.Show();
```

### Rewarded Ad

```c#
//Showing a Helium Rewarded Ad
if (_rewardedAd.ReadyToShow()){
  _rewardedAd.show();
```

## Showing Banner Ads
When you are ready to show a banner ad, you can check that it is ready to show and then display it by also passing a `HeliumBannerAdScreenLocation` position:


| Banner Ad Location Enum                   | Enum Value | Position                                                        |
| :---                                      | :---       | :---                                                            |
| HeliumBannerAdScreenLocation.TopLeft      | 0          | Positions the banner to the top-left screen of the device.      |
| HeliumBannerAdScreenLocation.TopCenter    | 1          | Positions the banner to the top-center screen of the device.    |
| HeliumBannerAdScreenLocation.TopRight     | 2          | Positions the banner to the top-right screen of the device.     |
| HeliumBannerAdScreenLocation.Center       | 3          | Positions the banner to the center screen of the device.        |
| HeliumBannerAdScreenLocation.BottomLeft   | 4          | Positions the banner to the bottom-left screen of the device.   |
| HeliumBannerAdScreenLocation.BottomCenter | 5          | Positions the banner to the bottom-center screen of the device. |
| HeliumBannerAdScreenLocation.BottomRight  | 6          | Positions the banner to the bottom-right screen of the device.  |

```c#
if (!_bannerAd.ReadyToShow())
  return;
_bannerAd.Show(HeliumBannerAdScreenLocation.TopCenter);
```

If you enable auto-refresh for a banner placement in the dashboard, then the Helium SDK will apply that setting when the placement is shown.

> **_NOTE:_** Any auto refresh changes made on the dashboard will take approximately one hour to take effect and the SDK must be rebooted in order to pick up the changes once they are available.

## Releasing Helium Ads

To clear resources used by Helium Ads, you can use the destroy method associated with the respective Helium Ad you have used.

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
