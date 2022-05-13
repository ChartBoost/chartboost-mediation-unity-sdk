using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor;
using Helium;

[CustomEditor(typeof(HeliumSettings))]
public class HeliumSettingEditor : Editor {
	private const string appIdLink = "https://dashboard.chartboost.com/all/publishing";

    GUIContent iOSAppIdLabel = new GUIContent("App Id [?]:", "Helium App Ids can be found at " + appIdLink);
    GUIContent androidAppIdLabel = new GUIContent("App Id [?]:", "Helium App Ids can be found at " + appIdLink);
    GUIContent iOSAppSigLabel = new GUIContent("App Signature [?]:", "Helium App Signatures can be found at " + appIdLink);
    GUIContent androidAppSigLabel = new GUIContent("App Signature [?]:", "Helium App Signatures can be found at " + appIdLink);
    GUIContent iOSLabel = new GUIContent("iOS");
	GUIContent androidLabel = new GUIContent("Google Play");
	GUIContent enableLoggingLabel = new GUIContent("Enable Logging for Debug Builds");
	GUIContent enableLoggingToggle = new GUIContent("isLoggingEnabled");

	private HeliumSettings instance;

	public override void OnInspectorGUI() {
		instance = (HeliumSettings)target;

		SetupUI();
	}

	private void SetupUI() {
		EditorGUILayout.HelpBox("Add the Helium App Id associated with this game", MessageType.None);

		// iOS
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(iOSLabel);
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(iOSAppIdLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        instance.SetIOSAppId(EditorGUILayout.TextField(instance.iOSAppId));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(iOSAppSigLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        instance.SetiOSAppSignature(EditorGUILayout.TextField(instance.iOSAppSignature));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Android
        EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(androidLabel);
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(androidAppIdLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        instance.SetAndroidAppId(EditorGUILayout.TextField(instance.androidAppId));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(androidAppSigLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        instance.SetAndroidAppSignature(EditorGUILayout.TextField(instance.androidAppSignature));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Loggin toggle.
        EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(enableLoggingLabel);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		HeliumSettings.enableLogging(EditorGUILayout.Toggle(enableLoggingToggle, instance.isLoggingEnabled));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
	}
}
