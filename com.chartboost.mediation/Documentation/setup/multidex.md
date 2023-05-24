# Multidex Support

Since the Chartboost Mediation Unity SDK provides the option to include multiple Ad Adapters and their own SDKs, this means it is very likely for an Android Application to require multidex support for proper compilation.

For more information about multidex please visit the [Official Android Documentation](https://developer.android.com/studio/build/multidex).

For proper Unity multidex support, you will need to enable Unity's `Custom Launcher Gradle Template`. The toggle to enable it can be found under `Project Settings/Player/Android/Publishing Settings`

When enabled, a gradle file will be created at the following location: `Assets/Plugins/Android/launcherTemplate.gradle`

In order to enable multidex, the default config section inside the gradle file needs to be changed from

### Multidex Disabled in Launcher Template
```gradle
  defaultConfig {
      minSdkVersion **MINSDKVERSION**
      targetSdkVersion **TARGETSDKVERSION**
      ...
  }
```

### Multidex Enabled in Launcher Template
```gradle
  defaultConfig {
      multiDexEnabled true
      minSdkVersion **MINSDKVERSION**
      targetSdkVersion **TARGETSDKVERSION**
      ...
  }
```

> **Note** \
> Notice that (...) merely represents continuation of contents and not the actual contents inside the gradle file

> **Warning** \
> Updating your Unity project will require to regenerate this launcherTemplate.gradle file as its contents tend to variate from Unity version to Unity version. This is because newer Unity versions tend to support higher Gradle versions.

## Adding Multidex Dependency to Project

Now that multidex is enabled on the gradle files, you will need to add its corresponding library as well for proper Android compilation.
To add multidex, create a new file into your `Assets` folder and name it `MultidexDependencies.xml` with the following contents:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<dependencies>
    <androidPackages>
        <androidPackage spec="androidx.multidex:multidex:2.0.1" />    
    </androidPackages>
</dependencies>
```

Now, resolve your dependencies utilizing Google's External Dependency Manager (EDM).

For more information about EDM, see section [EDM](edm.md)

## Application Class & Android Manifest

If you override your Application's class for Android, you will need to deal with further steps, more information about it can be found in the following [Official Android Documentation](https://developer.android.com/studio/build/multidex#mdex-gradle)

This step is important for proper functionality.
