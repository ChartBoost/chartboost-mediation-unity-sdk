# Ad Network Adapters

## Dependency Resolution & Google External Dependency Manager (EDM)

The Chartboost Mediation Unity SDK does not embed Google’s EDM plugin.

If you want to integrate ad networks with other supported SDKs as well, you will need [Google's External Dependency Manager](https://developers.google.com/unity/archive#external_dependency_manager_for_unity). For more information see our recommended setup in [Google External Dependency Manager (EDM)](edm.md).

## Adding Dependencies

Starting with Chartboost Mediation 5.X series, Unity adapters are now distributed individually in [npm](https://www.npmjs.com) and [NuGet](https://www.nuget.org)

All available adapters can be found in [Chartboost Mediation Adapters Page](https://adapters.chartboost.com), with corresponding instructions as to how to add them to your project.

## Dependency.xml Files

All `Dependency.xml` files are now embedded in each package `Editor` directory. This means they can't be direclty overwritten. However, this can bypassed as needed by creating a duplicate of such file under your `Assets` directory where modifications can be done as needed.