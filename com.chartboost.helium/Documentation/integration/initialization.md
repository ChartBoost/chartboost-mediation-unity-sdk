# Initialization

## Adding the import header

Add the following import header to the top of any class file that will be using a Helium class.

```c#
using Helium;
```

## Initializing Helium SDK

Using the Helium AppId and AppSignature from your dashboard setup, initialize the Helium SDK with the following code. We recommend initializing in the void Start() method:

```c#
HeliumSDK.StartWithAppIdAndAppSignature(APP_ID, APP_SIGNATURE);
```

Additionally, Helium Unity SDK includes an automatic initialization toggle in the HeliumSettings ScriptableObject.

To enabled automatic initialization check the `isAutomaticallyInitializing` toggle from `HeliumSettings.asset`


> **_NOTE 1:_** Make sure that these are the Helium AppId and AppSignature values that you obtain directly from your Helium Dashboard for your app as opposed to credentials from Chartboost or any other Ad Network.

> **_NOTE 2:_** Failing to remove fillers such as HELIUM_APP_ID and HELIUM_APP_SIGNATURE will result in an error.

Once the Helium SDK has successfully started, you can start showing ads.
