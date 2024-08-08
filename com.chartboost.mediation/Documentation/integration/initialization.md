# Initialization

## Chartboost Mediation as a Chartboost Core Module

Starting Chartboost Mediation 5.X series, Chartboost Mediation has become a module for [Chartboost Core Unity SDK](https://github.com/ChartBoost/chartboost-core-unity-sdk). Initialization is automatically handled when `ChartboostCore.Initialize` is called. Please refer to [Chartboost Core README.md](https://github.com/ChartBoost/chartboost-core-unity-sdk) on details for how to manage the Chartboost Core initialization processs.

## CoreModuleId

The `ChartboostMediation.CoreModuleId` can be used to identify the Chartboost Mediation Core `Module` initialization status. To be notified of Chartboost's Mediation initialization status, you can utilize the `ChartboostCore.ModuleInitializationCompleted` as seen below:

```csharp

```csharp
ChartboostCore.ModuleInitializationCompleted += result =>
{
    // If not Chartboost' Mediation, then ignore since we only care about CBMediation in this case.
    if (result.ModuleId != ChartboostMediation.CoreModuleId)
            return;

    Debug.Log($"Received initialization result for: {result.ModuleId} start:{result.Start}, end:{result.End} with duration: {result.Duration}");

    // Module failed to initialize module
    if (result.Error.HasValue) 
        Debug.LogError($"Module: {result.ModuleId} failed to initialize with error: {JsonTools.SerializeObject(result.Error.Value)}");
    // Modue succeeded to initialize, add to list of modules to skip to pass on the next ChartboostCore.Initialize call.
    else
        modulesToSkip.Add(result.ModuleId);
};
```

## SetPreInitializationConfiguration

Sets the Chartboost Mediation PreInitialization configuration. Setting this after initialization does nothing and returns an exception. This can be utilized to skip partner initialization, see example below:

```csharp
// List of partners ids to skip initialization
HashSet<string> skippablePartnerIds = new HashSet<string>
{
    ChartboostAdapter.PartnerIdentifier,
    MetaAudienceNetworkAdapter.PartnerIdentifier
};

// Create ChartboostMediationPreInitializationConfiguration object
ChartboostMediationPreInitializationConfiguration preinitializatioOptions = new ChartboostMediationPreInitializationConfiguration(skippablePartnerIds);

// Set ChartboostMediationPreInitializationConfiguration object
ChartboostMediationError? ChartboostMediation.SetPreInitializationConfiguration(preinitializatioOptions);

// Report if failed to set ChartboostMediationPreInitializationConfiguration object
if (error.HasValue) 
    Debug.LogError($"Failed to set PreInitializationConfiguration: {JsonTools.SerializeObject(error.Value)}");
```

In the example above, both `Chartboost` and `MetaAudienceNetwork` adapters are added to the `skippablePartnerIds` object. This will cause initialization for this ad adapters to be skipped. 