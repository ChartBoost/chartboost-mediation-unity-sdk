# Using Proguard

If you are using a custom ProGuard file, make sure to add the following rule to the proguard file `Assets/Plugin/Android/proguard-user.txt` for Chartboost Helium:

> **_NOTE:_** Proguard contents can also be found into `Packages/com.chartboost.helium/Runtime/Plugins/Android/proguard-user.txt`

```java
# HeliumUnityBridge

-keep public class com.chartboost.heliumsdk.unity.HeliumUnityBridge {
    *;
}
-keep class com.chartboost.heliumsdk.unity.HeliumUnityBridge$* {
    *;
}

# Helium SDK

-repackageclasses 'com.chartboost.heliumsdk.impl'
-renamesourcefileattribute SourceFile
-keepattributes Exceptions,InnerClasses,Signature,Deprecated,SourceFile,LineNumberTable,EnclosingMethod

-optimizations !code/allocation/variable

-keepattributes *Annotation*
-keepclassmembers class ** {
    @org.greenrobot.eventbus.Subscribe <methods>;
}
-keep enum org.greenrobot.eventbus.ThreadMode { *; }

-keep public class com.chartboost.heliumsdk.HeliumSdk {
    public <methods>;
}

#Adapters won't be able to make bids. We need to check what to keep.
-keep public class com.chartboost.heliumsdk.domain.Bid {
    public <methods>;
    public <fields>;
}

-keep public class com.chartboost.heliumsdk.domain.Bids {
    public <methods>;
    public <fields>;
}

-keep public class com.chartboost.heliumsdk.domain.AdData {
    public *;
}

-keep public class com.chartboost.heliumsdk.domain.Ad$AdType {}
-keep public class com.chartboost.heliumsdk.domain.Ad$State {}

-keep public class com.chartboost.heliumsdk.ad.HeliumAdError {
    public <methods>;
    public <fields>;
}

-keep public class com.chartboost.heliumsdk.ad.HeliumAdError$Code {}


-keep public class com.chartboost.heliumsdk.proxies.* {
    public <methods>;
}

#Adapters won't start without this. But we may need to look as to what to keep.
-keep public class com.chartboost.heliumsdk.proxies.BasePartnerProxy {}

-keep public interface com.chartboost.heliumsdk.HeliumSdk$HeliumSdkListener {
    public <methods>;
    public <fields>;
}

-keep public interface com.chartboost.heliumsdk.utils.HeliumAdapter {
    public <methods>;
    public <fields>;
}

-keep public class com.chartboost.heliumsdk.ad.* {
    public <methods>;
    public <fields>;
}

# IronSource

-keepclassmembers class com.ironsource.sdk.controller.IronSourceWebView$JSInterface {
    public *;
}
-keepclassmembers class * implements android.os.Parcelable {
    public static final android.os.Parcelable$Creator *;
}
-keep public class com.google.android.gms.ads.** {
   public *;
}
-keep class com.ironsource.adapters.** { *;
}
-dontwarn com.ironsource.mediationsdk.**
-dontwarn com.ironsource.adapters.**
-dontwarn com.moat.**
-keep class com.moat.** { public protected private *; }

# Vungle

-keepclassmembers enum com.vungle.warren.** { *; }
-keep class com.moat.** { *; }
-dontwarn com.moat.**
-dontwarn org.codehaus.mojo.animal_sniffer.IgnoreJRERequirement
-keepattributes *Annotation*
-keepattributes Signature, InnerClasses
-dontwarn org.codehaus.mojo.animal_sniffer.IgnoreJRERequirement
-dontwarn javax.annotation.**
-dontwarn kotlin.Unit
-dontwarn retrofit2.-KotlinExtensions
-keepclassmembers,allowshrinking,allowobfuscation interface * { @retrofit2.http.* <methods>; }
-dontwarn okhttp3.** -dontwarn okio.**
-dontwarn javax.annotation.**
-dontwarn org.conscrypt.**
-keepnames class okhttp3.internal.publicsuffix.PublicSuffixDatabase
-keepclassmembers class * extends com.vungle.warren.persistence.Memorable { public <init>(byte[]); }
```