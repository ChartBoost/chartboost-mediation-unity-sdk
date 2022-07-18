# Helium App ID & App Signature

In order to initialize the Helium SDK, you will need your Helium App ID & App Signature. This can be obtained in your [Helium Dashboard](https://helium.chartboost.com
).

There are 2 ways you can go about providing your App IDs to the Helium SDK.

## Scriptable Object & Automatic Initialization

Go to **Helium** > **Edit Settings**

Add your **Helium App ID** and **Helium App Signature**

Enable Automatic Initialization on the Helium Settings Scriptable object.

### Default Helium Settings

![](../images/helium-settings.png)

> **_NOTE:_** Make sure that these are the Helium AppId and App Signature values that you obtain directly from your Helium Dashboard for your app as opposed to credentials from Chartboost or any other Ad Network.

## Manual Initialization

If you would like to have more control on when to initialize the Helium SDK

You can call the following on your `Awake` method.

```c#
HeliumSDK.StartWithAppIdAndAppSignature(APP_ID, APP_SIGNATURE);
```

This will start the Helium SDK. For delegate information see section [Delegate Usage](com.chartboost.helium/Documentation/integration/delegate-usage.md)
