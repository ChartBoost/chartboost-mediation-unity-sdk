# Android Manifest

## Permissions
Add the following required permissions to your Android Manifest .xml file:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

> **_NOTE:_** It is important to notice that permissions might increase depending on your Ad adapter integrations. Always refer to such ad adapter's SDK integration guides for complete details. 

## Special Cases

### AdMob & GoogleBidding Application Id

When integrating AdMob or Google Bidding ad adapter, You will be required to add your APPLICATION_ID to the AndroidManifest.xml. Without this, your application will crash. Always refer to [Google's Documentation](https://developers.google.com/admob/android/quick-start) for the most up to date information.

```xml
<manifest>
    <application>
        <!-- Sample AdMob app ID: ca-app-pub-3940256099942544~3347511713 -->
        <meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy"/>
    </application>
</manifest>
```

### AppLovin SDK Key

When integrating AppLovin ad adapter, You will be required to add the AppLovin SDK Key yo the AndroidManifest.xml. Unlike Admob, AppLovin will not crash your application upon initialization, but it will fail to initialize the ad adapter. Always refer to [AppLovin's Documentation](https://dash.applovin.com/documentation/mediation/android/getting-started/integration) for the most up to date information.

```xml
<manifest>
    <application>
        <meta-data android:name="applovin.sdk.key" android:value="YOUR_SDK_KEY_HERE"/>
    </application>
</manifest>
```

## Example Manifest
After setup, your Android Manifest might look like the following:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="com.unity3d.player" xmlns:tools="http://schemas.android.com/tools" android:name="androidx.multidex.MultiDexApplication">
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

    <!-- Additional permissions might increase based on adapter integrations. e.g look below. -->
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.READ_PHONE_STATE" />

    <application android:debuggable="true">
        <activity android:name="com.unity3d.player.UnityPlayerActivity" android:theme="@style/UnityThemeSelector">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
            <meta-data android:name="unityplayer.ForwardNativeEventsToDalvik" android:value="true" />
        </activity>

        <!-- AdMob's APPLICATION_ID in Manifest -->
        <meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy"/>

        <!-- AppLovin's SDK Key in Manifest -->
        <meta-data android:name="applovin.sdk.key" android:value="YOUR_SDK_KEY_HERE"/>
    </application>
</manifest>
```
