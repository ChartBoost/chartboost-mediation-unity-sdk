using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
	[CustomEditor(typeof(ChartboostMediationSettings))]
	public class ChartboostMediationSettingEditor : UnityEditor.Editor
	{
		private const string AppIdLink = "https://dashboard.chartboost.com/all/publishing";

		private readonly GUIContent _partnerKilLSwitchTitle = new GUIContent("Partner Kill Switch");
		private readonly GUIContent _platformsIdsLabel = new GUIContent("Platform IDs");
		private readonly GUIContent _iOSAppIdLabel = new GUIContent("App Id [?]:", "Chartboost Mediation App Ids can be found at " + AppIdLink);
		private readonly GUIContent _androidAppIdLabel = new GUIContent("App Id [?]:", "Chartboost Mediation App Ids can be found at " + AppIdLink);
		private readonly GUIContent _iOSAppSigLabel = new GUIContent("App Signature [?]:", "Chartboost Mediation App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _androidAppSigLabel = new GUIContent("App Signature [?]:", "Chartboost Mediation App Signatures can be found at " + AppIdLink);
		private readonly GUIContent _iOSLabel = new GUIContent("iOS");
		private readonly GUIContent _androidLabel = new GUIContent("Android");
		private readonly GUIContent _debuggingTitle = new GUIContent("Debugging");
		private readonly GUIContent _automaticInitLabel = new GUIContent("Automatic Initialization");
		private readonly GUIContent _enableLoggingToggle = new GUIContent("Logging Enabled");
		private readonly GUIContent _enableAutomaticInitToggle = new GUIContent("Initialize Chartboost Mediation Automatically");
		private readonly GUIContent _skAdNetworkLabel = new GUIContent("SKAdNetwork");
		private readonly GUIContent _skAdNetworkToggle = new GUIContent("Use Chartboost Mediation SKAdNetwork Identifier Resolution");
		private readonly GUIContent _disableBitCodeLabel = new GUIContent("BitCode");
		private readonly GUIContent _disableBitCodeToggle = new GUIContent("Disable BitCode for XCode Projects/Workspaces");
		private readonly GUIContent _sdkKeysLabel = new GUIContent("SDK Keys");
		private readonly GUIContent _googleLabelAndroid = new GUIContent("Google App Id - Android [?]:");
		private readonly GUIContent _googleLabelIOS = new GUIContent("Google App Id - IOS [?]:");
		private readonly GUIContent _applovinLabel = new GUIContent("AppLovin SDK Key [?]:");

		private GUIStyle _title;

		public override void OnInspectorGUI()
		{
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
			EditorGUILayout.HelpBox("Select partners to disable their initialization.\n(Enum flag has been deprecated and will be removed in future versions, please use the StartWithOptions API instead)", MessageType.Info);
			ChartboostMediationSettings.PartnerKillSwitch = (ChartboostMediationPartners)EditorGUILayout.EnumFlagsField(ChartboostMediationSettings.PartnerKillSwitch);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(_platformsIdsLabel, _title);
			EditorGUILayout.HelpBox("Add the Chartboost Mediation App Id & App Signature associated with this game.", MessageType.Info);
			EditorGUILayout.Space();

			// iOS
			EditorGUILayout.LabelField(_iOSLabel, _title);
			EditorGUILayout.LabelField(_iOSAppIdLabel);
			ChartboostMediationSettings.IOSAppId = EditorGUILayout.TextField(ChartboostMediationSettings.IOSAppId);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField(_iOSAppSigLabel);
			ChartboostMediationSettings.IOSAppSignature = EditorGUILayout.TextField(ChartboostMediationSettings.IOSAppSignature);
			EditorGUILayout.Space();

			// Android
			EditorGUILayout.LabelField(_androidLabel, _title);
			EditorGUILayout.LabelField(_androidAppIdLabel);
			ChartboostMediationSettings.AndroidAppId = EditorGUILayout.TextField(ChartboostMediationSettings.AndroidAppId);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField(_androidAppSigLabel);
			ChartboostMediationSettings.AndroidAppSignature = EditorGUILayout.TextField(ChartboostMediationSettings.AndroidAppSignature);
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField(_sdkKeysLabel, _title);
			EditorGUILayout.LabelField(_googleLabelAndroid);
			ChartboostMediationSettings.AndroidGoogleAppId = EditorGUILayout.TextField(ChartboostMediationSettings.AndroidGoogleAppId);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(_googleLabelIOS);
			ChartboostMediationSettings.IOSGoogleAppId = EditorGUILayout.TextField(ChartboostMediationSettings.IOSGoogleAppId);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(_applovinLabel);
			ChartboostMediationSettings.AppLovinSDKKey = EditorGUILayout.TextField(ChartboostMediationSettings.AppLovinSDKKey);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField(_debuggingTitle, _title);
			ChartboostMediationSettings.IsLoggingEnabled = EditorGUILayout.Toggle(_enableLoggingToggle, ChartboostMediationSettings.IsLoggingEnabled);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField(_automaticInitLabel, _title);
			ChartboostMediationSettings.IsAutomaticInitializationEnabled = EditorGUILayout.Toggle(_enableAutomaticInitToggle, ChartboostMediationSettings.IsAutomaticInitializationEnabled);
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(_skAdNetworkLabel, _title);
			ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = EditorGUILayout.Toggle(_skAdNetworkToggle, ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled);
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField(_disableBitCodeLabel, _title);
			ChartboostMediationSettings.DisableBitCode = EditorGUILayout.Toggle(_disableBitCodeToggle, ChartboostMediationSettings.DisableBitCode);
		}
	}
}
