# Frecuently Asked Questions

## Is the minimum iOS version the same for all ad adapters?

No, each adapter might have a different minimum iOS version. Depending on your adapters, you might need to modify this value accordingly. For information on how to set this values for Unity, visit the [Unity Documentation](https://docs.unity3d.com/Manual/class-PlayerSettingsiOS.html).

## Encountering duplicate Android libraries

Chartboost Mediation provides multiple partner integrations with ease, we make sure that every possible integration is as easy as it can be. As such, we try to avoid integrations that might cause duplicate libraries for Android, since not many Unity developers might be familiar on how to resolve this issue. 

It is possible, that some integrations might collide with native plugins and dependencies of your own. Below, there is an example on how to add a fix to your mainTemplate.gradle in Unity to fix duplicate library issues.

The example below, utilizes exclusion to remove duplicate libraries that might otherwise collide with newer versions of the same library. For more information, visit [Gradle Docs](https://docs.gradle.org/current/userguide/resolution_rules.html#excluding_a_dependency_from_a_configuration_completely).

```groovy
configurations {
    all { 
        exclude group: "org.jetbrains.kotlin", module: "kotlin-stdlib-jdk8"
    }
}
```