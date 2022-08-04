# Android Manifest

Add the following required permissions to your Android Manifest .xml file:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

> **_NOTE:_** For mediation, you will need to add the regular chartboost activity as well. This prevents any compilation errors when integrating with Helium SDK in which the Chartboost SDK is also used separately.

```xml
<activity android:name="com.chartboost_helium.sdk.CBImpressionActivity"
          android:excludeFromRecents="true"
          android:hardwareAccelerated="true"
          android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen"
          android:configChanges="keyboardHidden|orientation|screenSize"/>
```

After setup, your Android Manifest might look like the following:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="com.unity3d.player" xmlns:tools="http://schemas.android.com/tools" android:name="androidx.multidex.MultiDexApplication">
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
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

        <meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="GOOGLE_MOBILE_ADS_APPLICATION_ID" />

        <activity android:name="com.chartboost_helium.sdk.CBImpressionActivity"
                  android:excludeFromRecents="true"
                  android:hardwareAccelerated="true"
                  android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen"
                  android:configChanges="keyboardHidden|orientation|screenSize"/>

    </application>
</manifest>
```
