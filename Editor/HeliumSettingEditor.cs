using UnityEditor;
using UnityEngine;

namespace Helium.Editor
{
	[CustomEditor(typeof(HeliumSettings))]
	public class HeliumSettingEditor : UnityEditor.Editor
	{
		private const string AppIdLink = "https://dashboard.chartboost.com/all/publishing";

		private readonly GUIContent _iOSAppIdLabel = new("App Id [?]:", "Helium App Ids can be found at " + AppIdLink);
		private readonly GUIContent _androidAppIdLabel = new("App Id [?]:", "Helium App Ids can be found at " + AppIdLink);
		private readonly GUIContent _iOSAppSigLabel = new("App Signature [?]:", "Helium App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _androidAppSigLabel = new("App Signature [?]:", "Helium App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _iOSLabel = new("iOS");
		private readonly GUIContent _androidLabel = new("Google Play");
		private readonly GUIContent _enableLoggingLabel = new("Enable Logging for Debug Builds");
		private readonly GUIContent _enableLoggingToggle = new("isLoggingEnabled");

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
		}
	}
}
