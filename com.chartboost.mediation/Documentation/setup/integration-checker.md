# Chartboost Mediation Integration Checker

`ChartboostMediationIntegrationChecker.cs` is a utility class introduced in Chartboost Mediation Unity 3.3.0. The `ChartboostMediationIntegrationChecker` provides common utility methods to make developers life easier when integrating, updating, and testing the Chartboost Mediation Unity SDK. The following functionality can be found in the Editor, and it can be triggered through Unity Context Menus:

* *`Chartboost Mediation/Integration/Status Check`*

This will run a series of checks and tests inside the Chartboost Mediation Unity SDK. It attempts to corroborate that any common integration issues are surfaced to developers with a popup window. It is recommended for developers to run this method once after every Chartboost Mediation Unity SDK update, as it will arise any issues that might need correction after the package update has completed.

* *`Chartboost Mediation/Integration/UnityAds Check`*

This will run a UnityAds integration specific check, it is completely optional and its only purpose is to make sure your integration is aligned with Chartboost Mediation / UnityAds integration standards. Whether you integrate the UnityAds SDK through the Unity Package Manager, or through the Chartboost Mediation Optional dependencies, this check will make sure that any warnings are shown to developers through a popup window.

* *`Chartboost Mediation/Integration/Force Reimport Adapters`*

This will automatically reimport all of your existing Ad Adapters. This utility function will double-check this is an intended action. Reimporting Ad Adapters is particularly useful in scenarios where you update your Chartboost Mediation Unity SDK and want to make sure all of your ad adapters are up to date. On the other hand, if you were to downgrade your Chartboost Mediation Unity SDK, this could help you get that version specific dependencies in a much more efficient manner.

## C# API

Here are some of the Utility methods accessible through the C# API for the `ChartboostMediationIntegrationChecker`

```csharp
// Used to attempt to update all existing Ad Adapters without extra input. Good for CI/CD usage and update of Adapters.
ChartboostMediationIntegrationChecker.ReimportExistingAdapters();

// Re-imports a series of Samples based of a collection of Samples names.
// This is to only update what it's currently in place regardless of the version.
var version = "4.0.0";
ChartboostMediationIntegrationChecker.ReimportExistingSamplesSet(["Chartboost Mediation", "AdMob", "Vungle"], version);


// Imports a sample from the Chartboost Mediation Unity SDK package
ChartboostMediationIntegrationChecker.ImportSample("ADAPTER_NAME", version)

// Uncomment UnityAds dependency on Optional-UnityAdsDependencies.xml if present.
ChartboostMediationIntegrationChecker.UncommentUnityAdsDependency(bool skipDialog = false)
```
