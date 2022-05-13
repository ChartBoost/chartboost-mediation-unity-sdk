using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Helium {

#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class HeliumSettings : ScriptableObject
	{
		const string package = "com.chartboost.helium";
		const string cbSettingsAssetName = "HeliumSettings";
	    const string cbSettingsPath = package + "/Resources";
	    const string cbSettingsAssetExtension = ".asset";

	    const string iOSExampleAppIDLabel = "HE_IOS_APP_ID";
	    const string iOSExampleAppSignatureLabel = "HE_IOS_APP_SIGNATURE";
        const string iOSExampleAppID = "59c04299d989d60fc5d2c782";
        const string iOSExampleAppSignature = "";

        const string androidExampleAppIDLabel = "HE_ANDROID_APP_ID";
	    const string androidExampleAppSignatureLabel = "HE_ANDROID_APP_SIGNATURE";
        const string androidExampleAppID = "4f7b433509b6025804000002";
        const string androidExampleAppSignature = "";

        const string credentialsWarningDefaultFormat = "You are using the Helium SDK {0} example {1}! Go to the Helium SDK dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";
	    const string credentialsWarningEmptyFormat = "You are using an empty string for the {0} {1}! Go to the Helium SDK dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";
	    const string credentialsWarningIOS = "IOS";
	    const string credentialsWarningAndroid = "Android";
        const string credentialsWarningAppID = "App ID";
        const string credentialsWarningAppSignature = "App Signature";

        private static bool credentialsWarning = false;

	    private static HeliumSettings instance;

	    static HeliumSettings Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
	                instance = Resources.Load(cbSettingsAssetName) as HeliumSettings;
	                if (instance == null)
	                {
	                    // If not found, autocreate the asset object.
	                    instance = CreateInstance<HeliumSettings>();
#if UNITY_EDITOR
						if (!Directory.Exists(Path.Combine(Application.dataPath, package)))
                        {
							AssetDatabase.CreateFolder("Assets", package);
						}
	                    if (!Directory.Exists(Path.Combine(Application.dataPath, cbSettingsPath)))
	                    {
	                        AssetDatabase.CreateFolder("Assets/" + package, "Resources");
	                    }

	                    string fullPath = Path.Combine(Path.Combine("Assets", cbSettingsPath),
	                                                   cbSettingsAssetName + cbSettingsAssetExtension
	                                                  );
	                    AssetDatabase.CreateAsset(instance, fullPath);
#endif
	                }
	            }
	            return instance;
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
	        string url = "https://answers.chartboost.com/en-us/child_article/unity";
	        Application.OpenURL(url);
	    }
#endif

        #region App Settings
        [SerializeField]
        public string iOSAppId = iOSExampleAppIDLabel;
        [SerializeField]
        public string iOSAppSignature = iOSExampleAppSignatureLabel;
        [SerializeField]
        public string androidAppId = androidExampleAppIDLabel;
        [SerializeField]
        public string androidAppSignature = androidExampleAppSignatureLabel;
        [SerializeField]
		public bool isLoggingEnabled = false;

        // allow mediation partners to set the appId and appSignature from code
        // if set, overrides the values set in the editor
        public static void setAppId(string appId)
        {
#if UNITY_IPHONE
            Debug.Log("Overriding IOS AppId: " + appId);
            Instance.SetIOSAppId(appId);
#elif UNITY_ANDROID
            Debug.Log("Overriding Google AppId: " + appId);
            Instance.SetAndroidAppId(appId);
#endif
        }

        public static void setAppSignature(string appSignature)
        {
#if UNITY_IPHONE
            Debug.Log("Overriding IOS AppSignature: " + appSignature);
            Instance.SetiOSAppSignature(appSignature);
#elif UNITY_ANDROID
            Debug.Log("Overriding Google AppSignature: " + appSignature);
            Instance.SetAndroidAppSignature(appSignature);
#endif
        }

        // iOS
        public void SetIOSAppId(string id)
        {
            if (!Instance.iOSAppId.Equals(id))
            {
                Instance.iOSAppId = id;
                DirtyEditor();
            }
        }

        public static string getIOSAppId()
        {
            if (Instance.iOSAppId.Equals(iOSExampleAppID))
            {
                CredentialsWarning(credentialsWarningDefaultFormat, credentialsWarningIOS, credentialsWarningAppID);

                return iOSExampleAppID;
            }
            if (Instance.iOSAppId.Equals(""))
            {
                CredentialsWarning(credentialsWarningEmptyFormat, credentialsWarningIOS, credentialsWarningAppID);
                // use it anyway
            }
            return Instance.iOSAppId;
        }


        public void SetiOSAppSignature(string signature)
        {
            if (!Instance.iOSAppSignature.Equals(signature))
            {
                Instance.iOSAppSignature = signature;
                DirtyEditor();
            }
        }

        public static string getIOSAppSignature()
        {
            if (Instance.iOSAppSignature.Equals(iOSExampleAppSignature))
            {
                CredentialsWarning(credentialsWarningDefaultFormat, credentialsWarningIOS, credentialsWarningAppSignature);

                return iOSExampleAppSignature;
            }
            if (Instance.iOSAppSignature.Equals(""))
            {
                CredentialsWarning(credentialsWarningEmptyFormat, credentialsWarningIOS, credentialsWarningAppSignature);
                // use it anyway
            }
            return Instance.iOSAppSignature;
        }

        // Android
        public void SetAndroidAppId(string id)
		{
			if (!Instance.androidAppId.Equals(id))
			{
				Instance.androidAppId = id;
				DirtyEditor();
			}
		}

		public static string getAndroidAppId()
		{
			if(Instance.androidAppId.Equals(androidExampleAppID))
			{
				CredentialsWarning(credentialsWarningDefaultFormat, credentialsWarningAndroid, credentialsWarningAppID);

				return androidExampleAppID;
			}
			if(Instance.androidAppId.Equals(""))
			{
				CredentialsWarning(credentialsWarningEmptyFormat, credentialsWarningAndroid, credentialsWarningAppID);
				// use it anyway
			}

			return Instance.androidAppId;
		}

        public void SetAndroidAppSignature(string signature)
        {
            if (!Instance.androidAppSignature.Equals(signature))
            {
                Instance.androidAppSignature = signature;
                DirtyEditor();
            }
        }

        public static string getAndroidAppSignature()
        {
            if (Instance.androidAppSignature.Equals(androidExampleAppSignature))
            {
                CredentialsWarning(credentialsWarningDefaultFormat, credentialsWarningAndroid, credentialsWarningAppSignature);

                return androidExampleAppSignature;
            }
            if (Instance.androidAppSignature.Equals(""))
            {
                CredentialsWarning(credentialsWarningEmptyFormat, credentialsWarningAndroid, credentialsWarningAppSignature);
                // use it anyway
            }
            return Instance.androidAppSignature;
        }

        public static void enableLogging(bool enabled)
		{
			Instance.isLoggingEnabled = enabled;

			DirtyEditor();
		}

		public static bool isLogging()
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
	    	if(credentialsWarning == false)
	    	{
				credentialsWarning = true;

				// Substitute the platform name in the warning
				Debug.LogWarning( string.Format(warning, platform, field));
			}
	    }

		public static void resetSettings()
		{
			// iOS
			if(Instance.iOSAppId.Equals(iOSExampleAppID))
			{
				Instance.SetIOSAppId(iOSExampleAppIDLabel);
			}

			// Android
			if(Instance.androidAppId.Equals(androidExampleAppID))
			{
				Instance.SetAndroidAppId(androidExampleAppIDLabel);
			}
		}

	    #endregion
	}
}
