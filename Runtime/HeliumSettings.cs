using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Helium {

	public class HeliumSettings : ScriptableObject
	{
		private const string Package = "com.chartboost.helium";
		private const string CbSettingsAssetName = "HeliumSettings";
		private const string CbSettingsPath = Package + "/Resources";
		private const string CbSettingsAssetExtension = ".asset";

	    private const string IOSExampleAppIDLabel = "HE_IOS_APP_ID";
	    private const string IOSExampleAppSignatureLabel = "HE_IOS_APP_SIGNATURE";
	    private const string IOSExampleAppID = "59c04299d989d60fc5d2c782";
	    private const string IOSExampleAppSignature = "";

	    private const string AndroidExampleAppIDLabel = "HE_ANDROID_APP_ID";
	    private const string AndroidExampleAppSignatureLabel = "HE_ANDROID_APP_SIGNATURE";
	    private const string AndroidExampleAppID = "4f7b433509b6025804000002";
	    private const string AndroidExampleAppSignature = "";

	    private const string CredentialsWarningDefaultFormat = "You are using the Helium SDK {0} example {1}! Go to the Helium SDK dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";
	    private const string CredentialsWarningEmptyFormat = "You are using an empty string for the {0} {1}! Go to the Helium SDK dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";
	    private const string CredentialsWarningIOS = "IOS";
	    private const string CredentialsWarningAndroid = "Android";
	    private const string CredentialsWarningAppID = "App ID";
	    private const string CredentialsWarningAppSignature = "App Signature";

        private static bool _credentialsWarning = false;

	    private static HeliumSettings _instance;

	    private static HeliumSettings Instance
	    {
	        get
	        {
		        if (_instance != null) 
			        return _instance;
		        _instance = Resources.Load(CbSettingsAssetName) as HeliumSettings;
		        if (_instance != null)
			        return _instance;
		        // If not found, autocreate the asset object.
	            _instance = CreateInstance<HeliumSettings>();
#if UNITY_EDITOR
	            if (!Directory.Exists(Path.Combine(Application.dataPath, Package)))
	            {
		            AssetDatabase.CreateFolder("Assets", Package);
	            }
	            if (!Directory.Exists(Path.Combine(Application.dataPath, CbSettingsPath)))
	            {
		            AssetDatabase.CreateFolder("Assets/" + Package, "Resources");
	            }

	            var fullPath = Path.Combine(Path.Combine("Assets", CbSettingsPath),
		            CbSettingsAssetName + CbSettingsAssetExtension
	            );
	            AssetDatabase.CreateAsset(_instance, fullPath);
#endif
		        return _instance;
	        }
	    }

#if UNITY_EDITOR
	    [MenuItem("Helium/Edit Settings")]
	    public static void Edit()
	    {
	        Selection.activeObject = Instance;
	    }

	    [MenuItem("Helium/Documentation")]
	    public static void OpenDocumentation()
	    {
		    const string url = "https://answers.chartboost.com/en-us/child_article/unity";
		    Application.OpenURL(url);
	    }
#endif

        #region App Settings
        [SerializeField]
        public string iOSAppId = IOSExampleAppIDLabel;
        [SerializeField]
        public string iOSAppSignature = IOSExampleAppSignatureLabel;
        [SerializeField]
        public string androidAppId = AndroidExampleAppIDLabel;
        [SerializeField]
        public string androidAppSignature = AndroidExampleAppSignatureLabel;
        [SerializeField]
		public bool isLoggingEnabled = false;

        // allow mediation partners to set the appId and appSignature from code
        // if set, overrides the values set in the editor
        public static void SetAppId(string appId)
        {
#if UNITY_IPHONE
            Debug.Log("Overriding IOS AppId: " + appId);
            SetIOSAppId(appId);
#elif UNITY_ANDROID
            Debug.Log("Overriding Google AppId: " + appId);
            SetAndroidAppId(appId);
#endif
        }

        public static void SetAppSignature(string appSignature)
        {
#if UNITY_IPHONE
            Debug.Log("Overriding IOS AppSignature: " + appSignature);
            SetiOSAppSignature(appSignature);
#elif UNITY_ANDROID
            Debug.Log("Overriding Google AppSignature: " + appSignature);
            SetAndroidAppSignature(appSignature);
#endif
        }

        // iOS
        public static void SetIOSAppId(string id)
        {
	        if (Instance.iOSAppId.Equals(id)) 
		        return;
	        Instance.iOSAppId = id;
	        DirtyEditor();
        }

        public static string GetIOSAppId()
        {
            switch (Instance.iOSAppId)
            {
	            case IOSExampleAppID:
		            CredentialsWarning(CredentialsWarningDefaultFormat, CredentialsWarningIOS, CredentialsWarningAppID);
		            return IOSExampleAppID;
	            default:
		            CredentialsWarning(CredentialsWarningEmptyFormat, CredentialsWarningIOS, CredentialsWarningAppID);
		            // use it anyway
		            break;
            }
            return Instance.iOSAppId;
        }


        public static void SetiOSAppSignature(string signature)
        {
	        if (Instance.iOSAppSignature.Equals(signature))
		        return;
	        Instance.iOSAppSignature = signature;
	        DirtyEditor();
        }

        public static string GetIOSAppSignature()
        {
	        switch (Instance.iOSAppSignature)
	        {
		        case IOSExampleAppSignature:
			        CredentialsWarning(CredentialsWarningDefaultFormat, CredentialsWarningIOS, CredentialsWarningAppSignature);

			        return IOSExampleAppSignature;
		        default:
			        CredentialsWarning(CredentialsWarningEmptyFormat, CredentialsWarningIOS, CredentialsWarningAppSignature);
			        // use it anyway
			        break;
	        }
            return Instance.iOSAppSignature;
        }

        // Android
        public static void SetAndroidAppId(string id)
        {
	        if (Instance.androidAppId.Equals(id)) 
		        return;
	        Instance.androidAppId = id;
	        DirtyEditor();
        }

		public static string GetAndroidAppId()
		{
			switch (Instance.androidAppId)
			{
				case AndroidExampleAppID:
					CredentialsWarning(CredentialsWarningDefaultFormat, CredentialsWarningAndroid, CredentialsWarningAppID);
					return AndroidExampleAppID;
				case "":
					CredentialsWarning(CredentialsWarningEmptyFormat, CredentialsWarningAndroid, CredentialsWarningAppID);
					// use it anyway
					break;
			}
			return Instance.androidAppId;
		}

        public static void SetAndroidAppSignature(string signature)
        {
	        if (Instance.androidAppSignature.Equals(signature))
		        return;
	        Instance.androidAppSignature = signature;
	        DirtyEditor();
        }

        public static string GetAndroidAppSignature()
        {
	        switch (Instance.androidAppSignature)
	        {
		        case AndroidExampleAppSignature:
			        CredentialsWarning(CredentialsWarningDefaultFormat, CredentialsWarningAndroid, CredentialsWarningAppSignature);

			        return IOSExampleAppSignature;
		        default:
			        CredentialsWarning(CredentialsWarningEmptyFormat, CredentialsWarningAndroid, CredentialsWarningAppSignature);
			        // use it anyway
			        break;
	        }
	        return Instance.androidAppSignature;
        }

        public static void EnableLogging(bool enabled)
		{
			Instance.isLoggingEnabled = enabled;
			DirtyEditor();
		}

		public static bool IsLogging()
		{
			return Instance.isLoggingEnabled;
		}

	    private static void DirtyEditor()
	    {
#if UNITY_EDITOR
	        EditorUtility.SetDirty(Instance);
#endif
	    }

	    private static void CredentialsWarning(string warning, string platform, string field)
	    {
		    if (_credentialsWarning) 
			    return;
		    _credentialsWarning = true;
		    // Substitute the platform name in the warning
		    Debug.LogWarning( string.Format(warning, platform, field));
	    }

		public static void ResetSettings()
		{
			// iOS
			if(Instance.iOSAppId.Equals(IOSExampleAppID))
				SetIOSAppId(IOSExampleAppIDLabel);

			// Android
			if(Instance.androidAppId.Equals(AndroidExampleAppID))
				SetAndroidAppId(AndroidExampleAppIDLabel);
		}

	    #endregion
	}
}
