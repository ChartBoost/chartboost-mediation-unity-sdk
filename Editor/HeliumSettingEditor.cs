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
			HeliumSettings.SetIOSAppId(EditorGUILayout.TextField(_instance.iOSAppId));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_iOSAppSigLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.SetiOSAppSignature(EditorGUILayout.TextField(_instance.iOSAppSignature));
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
			HeliumSettings.SetAndroidAppId(EditorGUILayout.TextField(_instance.androidAppId));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidAppSigLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.SetAndroidAppSignature(EditorGUILayout.TextField(_instance.androidAppSignature));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Loggin toggle.
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_enableLoggingLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			HeliumSettings.EnableLogging(EditorGUILayout.Toggle(_enableLoggingToggle, _instance.isLoggingEnabled));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			// automatic init toggle
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_enableAutomaticInitLabel);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			HeliumSettings.EnableAutomaticInit(EditorGUILayout.Toggle(_enableAutomaticInitToggle, _instance.isAutomaticInitEnabled));
			EditorGUILayout.EndHorizontal();
			
		}
	}
}
