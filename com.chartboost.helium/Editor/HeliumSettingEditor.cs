using UnityEditor;
using UnityEngine;

namespace Helium.Editor
{
	[CustomEditor(typeof(HeliumSettings))]
	public class HeliumSettingEditor : UnityEditor.Editor
	{
		private const string AppIdLink = "https://dashboard.chartboost.com/all/publishing";

		private readonly GUIContent _iOSAppIdLabel = new GUIContent("App Id [?]:", "Helium App Ids can be found at " + AppIdLink);
		private readonly GUIContent _androidAppIdLabel = new GUIContent("App Id [?]:", "Helium App Ids can be found at " + AppIdLink);
		private readonly GUIContent _iOSAppSigLabel = new GUIContent("App Signature [?]:", "Helium App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _androidAppSigLabel = new GUIContent("App Signature [?]:", "Helium App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _iOSLabel = new GUIContent("iOS");
		private readonly GUIContent _androidLabel = new GUIContent("Google Play");
		private readonly GUIContent _enableLoggingLabel = new GUIContent("Enable Logging for Debug Builds");
		private readonly GUIContent _enableAutomaticInitLabel = new GUIContent("Enable Automatic Initialization");
		private readonly GUIContent _enableLoggingToggle = new GUIContent("isLoggingEnabled");
		private readonly GUIContent _enableAutomaticInitToggle = new GUIContent("isAutomaticallyInitializing");

		private HeliumSettings _instance;

		public override void OnInspectorGUI()
		{
			_instance = (HeliumSettings)target;

			SetupUI();
		}

		private void SetupUI()
		{
			EditorGUILayout.HelpBox("Add the Helium App Id associated with this game", MessageType.None);

			// iOS
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_iOSLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_iOSAppIdLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.IOSAppId = EditorGUILayout.TextField(HeliumSettings.IOSAppId);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_iOSAppSigLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.IOSAppSignature = EditorGUILayout.TextField(HeliumSettings.IOSAppSignature);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Android
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidAppIdLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.AndroidAppId = EditorGUILayout.TextField(HeliumSettings.AndroidAppId);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidAppSigLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.AndroidAppSignature = EditorGUILayout.TextField(HeliumSettings.AndroidAppSignature);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Loggin toggle.
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_enableLoggingLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.IsLoggingEnabled = EditorGUILayout.Toggle(_enableLoggingToggle, HeliumSettings.IsLoggingEnabled);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			// automatic init toggle
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_enableAutomaticInitLabel);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			HeliumSettings.IsAutomaticInitializationEnabled = EditorGUILayout.Toggle(_enableAutomaticInitToggle, HeliumSettings.IsAutomaticInitializationEnabled);
			EditorGUILayout.EndHorizontal();
			
		}
	}
}
