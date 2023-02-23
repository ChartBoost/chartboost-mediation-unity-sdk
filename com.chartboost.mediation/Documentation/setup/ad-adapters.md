# Ad Network Adapters

## Dependency Resolution & Google External Dependency Manager (EDM)

The Chartboost Mediation Unity SDK does not embed Google’s EDM plugin.

If you want to integrate ad networks with other supported SDKs as well, you will need [Google's External Dependency Manager](https://developers.google.com/unity/archive#external_dependency_manager_for_unity). For more information see our recommended setup in [Google External Dependency Manager (EDM)](edm.md).

The Chartboost Mediation Unity SDK package contains samples with optional dependencies for ad adapters. Such samples can be found in the `Package Manager/Sample` sections.

Each Chartboost Mediation Unity SDK sample contains a .xml file with dependencies for the adapter available platforms (Android/iOS). In order for such dependencies to be included into the final build the [Google's External Dependency Manager](https://github.com/googlesamples/unity-jar-resolver) is needed.
The `EDM` makes sure to download and package any dependencies with your build.

## ChartboostMediationDependencies.xml

The `ChartboostMediationDependencies.xml` can be found in the `Package Manager/Sample` as seen in the following screenshot.

> **_NOTE:_** This is a mandatory dependency and must be imported for proper utilization of the Chartboost Mediation Unity SDK.

![ChartboostMediationDependencies.xml](../images/chartboost-mediation-dependencies.png)

## Ad Adapters as Optional Dependencies
In addition to the `ChartboostMediationDependencies.xml` file, the Chartboost Mediation Unity SDK includes optional dependencies based on the supported Ad Networks and its adapters.

They and can be found in the `Package Manager/Sample` area, and are labeled as follow:

`Optional-AdColonyDependencies.xml`

`Optional-AdMobDependencies.xml`

`Optional-AmazonPublisherServicesDependencies.xml`

`Optional-AppLovinDependencies.xml`

`Optional-GoogleBiddingDependencies.xml`

`Optional-MetaAudienceNetworkDependencies.xml`

`Optional-TapjoyDependencies.xml`

`Optional-VungleDependencies.xml`

`Optional-UnityAdsDependencies.xml`

`Optional-IronSourceDependencies.xml`

`Optional-DigitalTurbineExchangeDependencies.xml`

`Optional-InMobiDependencies.xml`

`Optional-MintegralDependencies.xml`

`Optional-YahooDependencies.xml`

They can be obtained by importing them as Samples with Unity's Package Manager. When updating the Chartboost Mediation Unity SDK Package through UPM. It is important to reimport any samples as to update your local dependency files.

![Reimport](../images/chartboost-mediation-dependencies-reimport.png)

> **_NOTE:_** When including 3rd-Party SDKs on Android, remember to update the manifest file according to the integrated SDK’s requirements as well.

### Adding Unity Ads SDK - Android

Including the `Optional-UnityAdsDependencies.xml` file will NOT automatically import the Unity Ads SDK to your Unity Android build (unlike the other dependency files which will do that automatically). You should manually include the correct version of the Unity Ads SDK by either:

* Uncommenting out the appropriate line in the `Optional-UnityAdsDependencies.xml` : `<!-- <androidPackage spec="com.unity3d.ads:unity-ads:4.4.1""/> -->`.

* Importing the correct version of the Unity Ads SDK via the built-in Unity Package Manager.

> **_NOTE:_** Only one of this solutions should be utilized, as utilizing both will create gradle compilation errors.
