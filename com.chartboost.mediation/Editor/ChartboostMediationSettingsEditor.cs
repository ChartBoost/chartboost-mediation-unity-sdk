using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
	[CustomEditor(typeof(ChartboostMediationSettings))]
	public class HeliumSettingEditor : UnityEditor.Editor
	{
		private const string AppIdLink = "https://dashboard.chartboost.com/all/publishing";

		private readonly GUIContent _partnerKilLSwitchTitle = new GUIContent("Partner Kill Switch");
		private readonly GUIContent _platformsIdsLabel = new GUIContent("Platform IDs");
		private readonly GUIContent _iOSAppIdLabel = new GUIContent("App Id [?]:", "Helium App Ids can be found at " + AppIdLink);
		private readonly GUIContent _androidAppIdLabel = new GUIContent("App Id [?]:", "Helium App Ids can be found at " + AppIdLink);
		private readonly GUIContent _iOSAppSigLabel = new GUIContent("App Signature [?]:", "Helium App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _androidAppSigLabel = new GUIContent("App Signature [?]:", "Helium App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _iOSLabel = new GUIContent("iOS");
		private readonly GUIContent _androidLabel = new GUIContent("Android");
		private readonly GUIContent _debuggingTitle = new GUIContent("Debugging");
		private readonly GUIContent _automaticInitLabel = new GUIContent("Automatic Initialization");
		private readonly GUIContent _enableLoggingToggle = new GUIContent("Logging Enabled");
		private readonly GUIContent _enableAutomaticInitToggle = new GUIContent("Initialize Helium Automatically");
		private readonly GUIContent _skAdNetworkLabel = new GUIContent("SKAdNetwork");
		private readonly GUIContent _skAdNetworkToggle = new GUIContent("Use Helium SKAdNetwork Identifier Resolution");

		private ChartboostMediationSettings _instance;
		private GUIStyle _title;

		public override void OnInspectorGUI()
		{
			_instance = (ChartboostMediationSettings)target;
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
			EditorGUILayout.LabelField(_partnerKilLSwitchTitle, _title);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Select partners to disable their initialization.", MessageType.Info);
			ChartboostMediationSettings.PartnerKillSwitch = (ChartboostMediationPartners)EditorGUILayout.EnumFlagsField(ChartboostMediationSettings.PartnerKillSwitch);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(_platformsIdsLabel, _title);
			
			EditorGUILayout.HelpBox("Add the Helium App Id & App Signature associated with this game.", MessageType.Info);
			// iOS
			EditorGUILayout.LabelField(_iOSLabel, _title);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_iOSAppIdLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.IOSAppId = EditorGUILayout.TextField(ChartboostMediationSettings.IOSAppId);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_iOSAppSigLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.IOSAppSignature = EditorGUILayout.TextField(ChartboostMediationSettings.IOSAppSignature);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Android
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidLabel, _title);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidAppIdLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.AndroidAppId = EditorGUILayout.TextField(ChartboostMediationSettings.AndroidAppId);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(_androidAppSigLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.AndroidAppSignature = EditorGUILayout.TextField(ChartboostMediationSettings.AndroidAppSignature);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(_debuggingTitle, _title);
			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.IsLoggingEnabled = EditorGUILayout.Toggle(_enableLoggingToggle, ChartboostMediationSettings.IsLoggingEnabled);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			EditorGUILayout.LabelField(_automaticInitLabel, _title);
			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.IsAutomaticInitializationEnabled = EditorGUILayout.Toggle(_enableAutomaticInitToggle, ChartboostMediationSettings.IsAutomaticInitializationEnabled);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(_skAdNetworkLabel, _title);
			EditorGUILayout.BeginHorizontal();
			ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = EditorGUILayout.Toggle(_skAdNetworkToggle, ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled);
			EditorGUILayout.EndHorizontal();
		}
	}
}
