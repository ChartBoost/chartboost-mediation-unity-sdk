# Initialization

## Adding the import header

Add the following import header to the top of any class file that will be using a Helium class.

```c#
using Helium;
```

## Initializing Helium SDK

In order to initialize the Helium SDK, you will need your Helium App ID & App Signature. This can be obtained in your [Helium Dashboard](https://helium.chartboost.com).
There are 2 ways you can go about providing your App IDs to the Helium SDK.

### Helium Settings & Automatic Initialization

Go to **Helium** > **Edit Settings**

Add your **Helium App ID** and **Helium App Signature**

Enable Automatic Initialization on the Helium Settings Scriptable object.

#### Helium Settings Static Accessors

HeliumSettings parameters can be modified via the available static accessors. As follows:

```c#
// IOS IDs Accessors
HeliumSettings.IOSAppId = "SAMPLE_APP_ID_IOS";
HeliumSettings.IOSAppSignature = "SAMPLE_APP_SIGNATURE_IOS";

// Android IDs Accessors
HeliumSettings.AndroidAppId = "SAMPLE_APP_ID_ANDROID";
HeliumSettings.AndroidAppSignature = "SAMPLE_APP_SIGNATURE_ANDROID";


// Logging Status Accessor
HeliumSettings.IsLoggingEnabled = false;

// Automatic Initialization Accessor
HeliumSettings.IsAutomaticInitializationEnabled = false;

// Partner Kill Switch Accessor
HeliumSettings.PartnersKillSwitch = HeliumPartners.None;
```

The Helium SDK automatically uses such parameters to make decisions, the APIs are exposed for ease of usage.

### Default Helium Settings

![Helium Settings](../images/helium-settings.png)

> **_NOTE:_** Make sure that these are the Helium AppId and App Signature values that you obtain directly from your Helium Dashboard for your app as opposed to credentials from Chartboost or any other Ad Network.

### Manual Initialization

If you would like to have more control on when to initialize the Helium SDK

You can call the following on your `Awake` method.

```c#
var appID = "SAMPLE_APP_ID";
var appSignature = "SAMPLE_APP_SIGNATURE";

HeliumSDK.StartWithAppIdAndAppSignature(appID, appSignature);
```

This will start the Helium SDK. For delegate information see section [Delegate Usage](delegate-usage.md)

> **_NOTE:_** Failing to remove fillers such as HELIUM_APP_ID and HELIUM_APP_SIGNATURE will result in an error.

Once the Helium SDK has successfully started, you can start requesting ads.

### Partner Kill Switch

From Helium 3.3.0 forward, the Helium SDK initialization method has been expanded to take in optional initialization parameters. One of those parameters is a set of partner adapter identifiers to skip initialization for the session.

In Unity, Helium partners are identified through the HeliumPartners enum. HeliumPartners implements enum flags, which means flags can be raised before initialization as needed.

By default, the Partner Kill Switch can be easily modified through the HeliumSettings scriptable object. By doing so, you can only enable/disable partner before builds are compiled. However, if your game utilizes a remote config supported service such as Leanplum, Swrve, etc. you can utilize remote config variables to enable/disable partners in deployed builds.

The following example disables AdMob, Facebook, and TapJoy before Helium initialization. As long as the `HeliumSettings.PartnersKillSwitch` parameter is modified before initialization, the partner networks will be skipped during the initialization process.


```c#
HeliumSettings.PartnersKillSwitch = HeliumPartners.AdMob | HeliumPartners.Facebook | HeliumPartners.TapJoy;

var appID = "SAMPLE_APP_ID";
var appSignature = "SAMPLE_APP_SIGNATURE";

HeliumSDK.StartWithAppIdAndAppSignature(appID, appSignature);
```
For more information on how to corroborate partner initialization data visit [Delegate Usage](delegate-usage.md)
