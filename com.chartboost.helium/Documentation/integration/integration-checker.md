# Helium Integration Checker

`HeliumIntegrationChecker.cs` is a utility class introduced in Helium Unity 3.3.0. The `HeliumIntegrationChecker` provides common utility methods to make developers life easier when integrating, updating, and testing the Helium Unity SDK. The following functionality can be found in it, and it is accessible through Unity Context Menus:

* *`Helium/Integration/Status Check`*

This will run a series of checks and tests inside the Helium Unity SDK. It attempts to corroborate that any common integration issues are surfaced to developers with a popup window. It is recommended for developers to run this method once after every Helium Unity SDK update, as it will arise any issues that might need correction after the package update has completed.

* *`Helium/Integration/UnityAds Check`*

This will run a UnityAds integration specific check, it is completely optional and its only purpose is to make sure your integration is aligned with Helium / UnityAds integration standards. Whether you integrate the UnityAds SDK through the Unity Package Manager, or through the Helium Optional dependency, this check will make sure that any warnings are shown to developers through a popup window.

* *`Helium/Integration/Force Reimport Adapters`*

This will automatically reimport all of your existing Ad Adapters. This utility function will double-check this is an intended action. Reimporting Ad Adapters is particularly useful in scenarios where you update your Helium Unity SDK and want to make sure all of your ad adapters are up to date. On the other hand, if you were to downgrade your Helium Unity SDK, this could help you get that version specific dependencies in a much more efficient manner.

## C# API

Here are some of the Utility methods accessible through the C# API for the `HeliumIntegrationChecker`

```csharp
// Used to attempt to update all existing Ad Adapters without extra input. Good for CI/CD usage and update of Adapters.
HeliumIntegrationChecker.ReimportExistingAdapters();

// Re-imports a series of Samples based of a collection of Samples names.
// This is to only update what it's currently in place regardless of the version.
var version = "3.3.0";
HeliumIntegrationChecker.ReimportExistingHeliumSamples(["Helium", "AdMob", "Vungle"], version);


// Imports a sample from the Helium Unity SDK package
HeliumIntegrationChecker.ImportSample("ADAPTER_NAME", version)

// Uncomment UnityAds dependency on Optional-HeliumUnityAdsDependencies.xml if present.
HeliumIntegrationChecker.UncommentUnityAdsDependency(bool skipDialog = false)
```
