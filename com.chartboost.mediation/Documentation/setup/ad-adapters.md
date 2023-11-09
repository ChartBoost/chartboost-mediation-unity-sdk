# Ad Network Adapters

## Dependency Resolution & Google External Dependency Manager (EDM)

The Chartboost Mediation Unity SDK does not embed Google’s EDM plugin.

If you want to integrate ad networks with other supported SDKs as well, you will need [Google's External Dependency Manager](https://developers.google.com/unity/archive#external_dependency_manager_for_unity). For more information see our recommended setup in [Google External Dependency Manager (EDM)](edm.md).

## Chartboost Mediation Adapters Window

Since Chartboost Mediation 4.X Adapters are no longer released at the cadence as the Chartboost Mediation SDK, it is now possible to receive adapter updates in between SDK releases. 

Until Chartboost Mediation Unity SDK 4.1.0, adapters were added through the UPM Samples capability; however, this limited the ability to provide adapter updates in between SDK releases. As such, from Chartboost Mediation Unity SDK 4.2.0, we have created a brand new Editor Window. This will allow users to fetch Ad Adapter updates on demand. *see screenshot below*

### ***Adapters Window Default State***

The Adapters window can be accessed through the following unity menu: ***Chartboost Mediation/Configure***

![Chartboost Mediation Settings](../images/adapters-window-default.png)

As seen, in the screenshot above, in its default state, the Adapters Window does not select any adapters, and there are multiple elements grabbing your attention. For a detailed step-by-step instruction on how to use the window see below:

### ChartboostMediationDependencies.xml

Although Chartboost Mediation Unity SDK can be initialized without adapters. You still need to have a reference to the Chartboost Mediation Native libraries. Whenever such dependencies are missing or miss-matching with your currently implemented version, the following warning button will show up. ***see below***

![Chartboost Mediation Settings](../images/adapters-window-warning.png)

If you wish to know more details, you can always hover over the warning to see more details. In order to resolve the warnings, you just need to press the warning button itself. In most scenarios, this will add or update your `ChartboostMediationDependencies.xml` dependency file. 

After resolving the warnings, the `ChartboostMediationDependencies.xml` dependency file can be found in the following path 
`Assets/com.chartboost.mediation/Editor/ChartboostMediationDependencies.xml`

> **Note** \
> In the past, dependencies used to live under the `Assets/Samples/Chartboost Mediation/4.X` path. However, they will now be under the `Assets/com.chartboost.mediation/Editor` directory.

### Adding Ad Adapters

In order to add adapters, you only need to select a version from the platform specific dropdowns. ***see below*** 

![Chartboost Mediation Settings](../images/adapters-window-save.png)

As seen in the screenshot above, whenever changes are pending to be saved, the save button will be displayed. In order to save your adapter selections you must click on the save button. After saving, the corresponding dependencies for the Ad Adapter selections will be saved in the following path `Assets/com.chartboost.mediation/Editor/Adapters`.

> **Note** \
> If you wish to see all of your Ad Adapter selections in your project, they can be found in the following file `Assets/com.chartboost.mediation/Editor/selections.json`. If you ever need to provide support with information regarding your Ad Adapter selections, you can use this file.

> **Note** \
> Ad Networks can be implemented entirely (Android, IOS) or partially, only one platform. 

### Window Utilities

#### **Upgrade All Selections**

Once you have all of your selections, you can always manually check for updates by pressing the upgrades button. Found in the top right corner. Using the upgrade button will compare your current selections with the most up to date adapter releases. If any changes are found, you will be notified and asked to save such changes.

![Chartboost Mediation Settings](../images/adapters-window-upgrade.png)

#### **Refresh**

Adapter information is fetched automatically on Unity Editor's startup. If you wish you check for updates on demand, you can use the refresh button. ***see below*** 

![Chartboost Mediation Settings](../images/adapters-window-refresh.png)

Using the refresh button will check for new adapter releases, update your cached adapter info, and repaint the Adapters Window if necessary.

## Adapters Window Unity Editor C# API

Along with the Editor Window, we have exposed a few C# methods that can be utilized in a CI/CD environment to keep your adapters up to date.

Below is a demonstration on how to use such API:

```csharp

// AdapterDataSource is in charge of fetching adapter updates, runs once on Editor startup, but you will need to call it manually if running in batchmode
AdapterDataSource.Update();

// Loads current project adapter selections
AdaptersWindow.LoadSelections();

// Perform Ad Adapter upgrades, platform flags available for customization
var upgrades = AdaptersWindow.UpgradePlatformToLatest(Platform.Android | Platform.IOS);

// Depending on upgrade results, information can be logged.
Console.WriteLine(upgrades.Count > 0 ? $"[Adapters] Upgraded: \n {JsonConvert.SerializeObject(upgrades, Formatting.Indented)}" : "[Adapters] No Upgrades.");

// Ad newly found Ad Adapter networks, by default any partially implemented or newly found networks will be added, but such behavior can be customized.
var newNetworks = AdaptersWindow.AddNewNetworks(Platform.Android | Platform.IOS);

// Manually save selections
AdaptersWindow.SaveSelections();

// Depending on new networks result, information can be logged.
Console.WriteLine(newNetworks.Count > 0 ? $"[Adapters] New Networks: \n {JsonConvert.SerializeObject(newNetworks, Formatting.Indented)}" :  "[Adapters] No New Networks");

// This will resolve any issues with your Chartboost Mediation dependency, e.g if your package does not match your current dependency file, this method makes sure to update the file as needed.
var changed = AdaptersWindow.CheckChartboostMediationVersion();
Console.WriteLine(changed ? "[Adapters] Chartboost Mediation Version Has Been Updated" :  "[Adapters] Chartboost Mediation Version is Up to Date");
```

### Adding Networks through the C# API

As mentioned in the previous section, networks can be added based on specific conditions.

#### Default Network Addition Condition

```csharp
/// <summary>
/// Default network add condition. This will add any entirely missing or partially implemented networks
/// </summary>
/// <param name="id">network id</param>
/// <param name="currentSelections">current selections</param>
/// <returns></returns>
private static bool DefaultAddCondition(string id, Dictionary<string, AdapterSelection> currentSelections) => !selections.ContainsKey(id) || selections[id].android == Constants.Unselected || selections[id].ios == Constants.Unselected;
```
Such condition is checked automatically when running `AdaptersWindow.AddNewNetworks`. However, it can be customized to fit your own needs. ***see below*** 

```csharp

// The method below will only add brand new networks and implemented them as needed.
private bool CustomCondition(string id, Dictionary<string, AdapterSelection> currentSelections) => !selections.ContainsKey(id);

// Addding networks only if they are entirely new
AdaptersWindow.AddNewNetworks(Platform.Android | Platform.IOS, CustomCondition);

// Another example, adding only brand new networks for Android
AdaptersWindow.AddNewNetworks(Platform.Android, CustomCondition);
```

## Adapters Related Runtime C# APIs

Starting Chartboost Mediation 4.7.0 we have included a few APIs pertinent to ad adapter partners. See below for integration details:

### Partner Consent

Consent now can be set on an case by case basis, to do so refer to the `Partners.cs` data class containing an up to date group of all of the supported Partner ids. If you do not see your Partner there contact support for details. 

#### Setting Partner Consents
Partner consent can be set on an individual basis using the following API:

```csharp
// In this example we set AdMob's Consent to True. Granted!
ChartboostMediation.PartnerConsents.SetPartnerConsent(Partners.AdMob, true);

// Set AdColony's Consent to False. Denied!
ChartboostMediation.PartnerConsents.SetPartnerConsent(Partners.AdColony, false);
```

Partner consent can also be set as a predefined collection using the following API:

```csharp
var consents = new Dictionary<string, bool>
{
    { Partners.AdMob, true },
    { Partners.AdColony, false }
};
ChartboostMediation.PartnerConsents.AddPartnerConsents(consents);
```

> **Note** \
> Both examples above achieve the same result.

#### Getting Current Partner Consents

Consents can be fetched utilzing the following API.

```csharp
var consents = ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy();
// Base on the examples provided before, this would outout "Current Consent: { "admob" : "true", "adcolony" : "false" }".
Debug.Log($"Current Consent: {JsonConvert.SerializeObject(consents)}");
```

> **Note** \
> Partner consent persists across sessions.

### Removing Partner Consents

Consent can be removed on a case by case basis with the following API:

```csharp
// Remove AdMob Consent, RemovePartnerConsent returns the value attached to the Partner. Base on the previous examples this would be `true`.
var adMobConsent = ChartboostMediation.PartnerConsents.RemovePartnerConsent(Partners.AdMob);

// Remove AdColony Consent, RemovePartnerConsent returns the value attached to the Partner. Base on the previous examples this would be `false`.
var adColonyConsent = ChartboostMediation.PartnerConsents.RemovePartnerConsent(Partners.AdColony);

// Removing consent for a network without consent set returns `null`.
var vungleConsent = ChartboostMediation.PartnerConsents.RemovePartnerConsent(Partners.Vungle);
```

If you wish to remove consent for all partners, please use:

```csharp
ChartboostMediation.PartnerConsents.ClearConsents();
```

### Replacing Consent

If you wish to replace consent with an entirely new set of values , use:

```csharp
var consents = new Dictionary<string, bool>
{
    { Partners.Vungle, true },
    { Partners.Mintegral, true }
};

ChartboostMediation.PartnerConsents.ReplacePartnerConsents(consents);

var consents = ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy();
// Base on the examples provided before, the new output would be "Current Consent: { "vungle" : "true", "mintegra" : "true" }".
Debug.Log($"Current Consent: {JsonConvert.SerializeObject(consents)}");
```

### Adapters Information

Partners ad adapter information can now be fetched utilizing the following API:

```csharp

// Adapter information is conformed to the `AdapterInfo.cs` data class.
Debug.Log($"Current Adapters: {JsonConvert.SerializeObject(ChartboostMediation.AdaptersInfo)}");

// Logging each adapter information.
foreach (var adapter in ChartboostMediation.AdaptersInfo)
{
    Debug.Log($"Logging Adapter v-{adapter.AdapterVersion}, partner v-{adapter.PartnerVersion}, partner id:{adapter.PartnerIdentifier}, partner display:{adapter.PartnerDisplayName}");
}
```

You can use the two APIs above to manage your consent easily, for example:

```csharp

// Get your current user consent status.
var myUserConsentStatus = FetchUserConsent();

foreach (var adapter in ChartboostMediation.AdaptersInfo)
    ChartboostMediation.PartnerConsents.SetPartnerConsent(adapter.PartnerIdentifier, myUserConsentStatus);
```

> **Note** \
> The code block above is a mere example of what can be done with the APIs, user consent should be managed with care.