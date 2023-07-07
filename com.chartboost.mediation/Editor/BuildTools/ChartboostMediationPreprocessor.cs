using System;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Chartboost.Editor.BuildTools
{
    internal class ChartboostMediationPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
                return;
            
            ValidateAndroidManifest();
        }

        private static void ValidateAndroidManifest()
        {
            const string pathToAndroidManifest = "Assets/Plugins/Android/AndroidManifest.xml";
            const string appLovinKeyIdentifier = "applovin.sdk.key";
            const string googleAppIdIdentifier = "com.google.android.gms.ads.APPLICATION_ID";
            
            var androidManifest = XDocument.Load(pathToAndroidManifest);
            var modified = IncludeElementsOrAdd(androidManifest, googleAppIdIdentifier, ChartboostMediationSettings.AndroidGoogleAppId);
            var appLovinAdded = IncludeElementsOrAdd(androidManifest, appLovinKeyIdentifier, ChartboostMediationSettings.AppLovinSDKKey);
            if (appLovinAdded && !modified)
                modified = true;

            if (modified)
            {
                androidManifest.Save(pathToAndroidManifest);
                AssetDatabase.Refresh();
            }

            static bool IncludeElementsOrAdd(XDocument androidManifest, string elementIdentifier, string elementValue)
            {
                if (string.IsNullOrEmpty(elementValue) || elementValue.Equals(ChartboostMediationSettings.DefaultSDKKeyValue))
                    return false;
            
                var targetElement = XElement.Parse($"<meta-data android:name=\"{elementIdentifier}\" android:value=\"{elementValue}\" xmlns:android=\"http://schemas.android.com/apk/res/android\"/>");

                try
                {
                    var targetElementInManifest = androidManifest.Descendants("meta-data").First(x => x.Attributes().Any(a => a.Value == elementIdentifier));
                    var targetElementMatch = targetElementInManifest.Attributes().Any(attribute => attribute.Value == elementValue);
                    if (targetElementMatch)
                        Debug.Log($"[ChartboostMediationPreprocessor] {elementIdentifier} Found in Android Manifest!");
                    else
                        Debug.LogError($"[ChartboostMediationPreprocessor] A {elementIdentifier} was found in manifest but did not match the {elementIdentifier} found in the ChartboostMediationSettings.");
                }
                catch (InvalidOperationException googleAppIdException)
                {
                    try
                    {
                        var applicationNode = androidManifest.Descendants("application").First();
                        targetElement.LastAttribute.Remove();
                        applicationNode.Add(targetElement);
                    }
                    catch (InvalidOperationException applicationNodeException)
                    {
                        Debug.LogError("[ChartboostMediationPreprocessor] Could not find an application element in AndroidManifest.xml, your AndroidManifest might be malformed.");
                    }
                }
                return true;
            }
        }
    }
}
