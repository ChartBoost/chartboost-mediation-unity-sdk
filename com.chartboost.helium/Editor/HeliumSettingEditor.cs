using System;
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
		private GUIStyle _title;

		public override void OnInspectorGUI()
		{
			_instance = (HeliumSettings)target;
			_title = new GUIStyle {
				fontSize = 16,
				fontStyle = FontStyle.Bold,
				normal =
				{
					textColor = Color.white
				}
			};
			SetupUI();
		}



		private void SetupUI()
		{
			// partner kill-switch
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Partner Kill Switch", _title);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Select partners to disable their initialization.", MessageType.Info);
			HeliumSettings.PartnerKillSwitch = (HeliumPartners)EditorGUILayout.EnumFlagsField(HeliumSettings.PartnerKillSwitch);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Platform IDs", _title);
			
			EditorGUILayout.HelpBox("Add the Helium App Id & App Signature associated with this game.", MessageType.Info);
			// iOS
			EditorGUILayout.LabelField("iOS", _title);

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
			EditorGUILayout.LabelField("Android", _title);
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
			
			EditorGUILayout.LabelField("Debugging", _title);
			EditorGUILayout.BeginHorizontal();
			HeliumSettings.IsLoggingEnabled = EditorGUILayout.Toggle(_enableLoggingToggle, HeliumSettings.IsLoggingEnabled);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Automatic Initialization", _title);
			EditorGUILayout.BeginHorizontal();
			HeliumSettings.IsAutomaticInitializationEnabled = EditorGUILayout.Toggle(_enableAutomaticInitToggle, HeliumSettings.IsAutomaticInitializationEnabled);
			EditorGUILayout.EndHorizontal();
			
		}
	}
}
