using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

// For internal Chartboost usage only.  Do not package for distribution.
// The purpose of this script is to add the GADApplicationIdentifier to the
// generated Xcode project plist file.

public class ChangeIOSBuildNumber
{
#if UNITY_IOS
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // Change value of CFBundleVersion in Xcode plist
            var buildKey = "GADApplicationIdentifier";
            rootDict.SetString(buildKey, "ca-app-pub-6548817822928201~9620807135");

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
#endif
}
