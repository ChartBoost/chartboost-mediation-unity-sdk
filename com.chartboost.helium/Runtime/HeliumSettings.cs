using System;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Helium
{
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
        
        /// <summary>
        /// Creates or Fetches a HeliumSettings instance.
        /// </summary>
        private static HeliumSettings Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                _instance = Resources.Load(CbSettingsAssetName) as HeliumSettings;
                if (_instance != null)
                    return _instance;
                // If not found, auto-create the asset object.
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
            const string url = "https://developers.chartboost.com/docs/unity-add-the-helium-sdk";
            Application.OpenURL(url);
        }
#endif

        #region App Settings
        [SerializeField] private string iOSAppId = IOSExampleAppIDLabel;
        [SerializeField] private string iOSAppSignature = IOSExampleAppSignatureLabel;
        [SerializeField] private string androidAppId = AndroidExampleAppIDLabel;
        [SerializeField] private string androidAppSignature = AndroidExampleAppSignatureLabel;
        [SerializeField] private bool isLoggingEnabled;
        [SerializeField] private bool isAutomaticInitEnabled;

        /// <summary>
        /// Accessor for androidAppId
        /// </summary>
        public static string AndroidAppId
        {
            get => EvaluateCredential(Instance.androidAppId, AndroidExampleAppID, CredentialsWarningAndroid,
                CredentialsWarningAppID);
            set
            {
                if (Instance.androidAppId.Equals(value))
                    return;
                Instance.androidAppId = value;
                DirtyEditor();
            }
        }

        /// <summary>
        /// Accessor for androidAppSignature
        /// </summary>
        public static string AndroidAppSignature
        {
            get => EvaluateCredential(Instance.androidAppSignature, AndroidExampleAppSignature,
                CredentialsWarningAndroid, CredentialsWarningAppSignature);
            set
            {
                if (Instance.androidAppSignature.Equals(value))
                    return;
                Instance.androidAppSignature = value;
                DirtyEditor();
            }
        }

        /// <summary>
        /// Accessor for iOSAppId
        /// </summary>
        public static string IOSAppId
        {
            get => EvaluateCredential(Instance.iOSAppId, IOSExampleAppID, CredentialsWarningIOS,
                CredentialsWarningAppID);
            set
            {
                if (Instance.iOSAppId.Equals(value))
                    return;
                Instance.iOSAppId = value;
                DirtyEditor();
            }
        }

        /// <summary>
        /// Accessor for iOSAppSignature
        /// </summary>
        public static string IOSAppSignature
        {
            get => EvaluateCredential(Instance.iOSAppSignature, IOSExampleAppSignature, CredentialsWarningIOS,
                CredentialsWarningAppSignature);
            set
            {
                if (Instance.iOSAppSignature.Equals(value))
                    return;
                Instance.iOSAppSignature = value;
                DirtyEditor();
            }
        }

        /// <summary>
        /// Accessor for LoggingEnabled settings
        /// </summary>
        public static bool IsLoggingEnabled
        {
            get => Instance.isLoggingEnabled;

            set
            {
                Instance.isLoggingEnabled = value;
                DirtyEditor();
            }
        }

        /// <summary>
        /// Accessor for isAutomaticInitEnabled.
        /// </summary>
        public static bool IsAutomaticInitializationEnabled
        {
            get => Instance.isAutomaticInitEnabled;
            set
            {
                Instance.isAutomaticInitEnabled = value;
                DirtyEditor();
            }
        }

        #region Obsolete

        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use platform specific accessors instead.")]
        public static void SetAppId(string appId)
        {
#if UNITY_IPHONE
            Debug.Log("Overriding IOS AppId: " + appId);
            IOSAppId = appId;
#elif UNITY_ANDROID
            Debug.Log("Overriding Google AppId: " + appId);
            AndroidAppId = appId;
#endif
        }

        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use platform specific accessors instead.")]
        public static void SetAppSignature(string appSignature)
        {
#if UNITY_IPHONE
            Debug.Log("Overriding IOS AppSignature: " + appSignature);
            IOSAppSignature = appSignature;
#elif UNITY_ANDROID
            Debug.Log("Overriding Google AppSignature: " + appSignature);
            AndroidAppSignature = appSignature;
#endif
        }

        // iOS
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IOSAppId accessor instead.")]
        public static void SetIOSAppId(string id) => IOSAppId = id;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IOSAppSignature accessor instead.")]
        public static void SetiOSAppSignature(string signature) => IOSAppSignature = signature;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IOSAppId accessor instead.")]
        public static string GetIOSAppId() => IOSAppId;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IOSAppSignature accessor instead.")]
        public static string GetIOSAppSignature() => IOSAppSignature;
        // Android
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use AndroidAppId accessor instead.")]
        public static void SetAndroidAppId(string id) => AndroidAppId = id;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use AndroidAppSignature accessor instead.")]
        public static void SetAndroidAppSignature(string signature) => AndroidAppSignature = signature;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use AndroidAppId accessor instead.")]
        public static string GetAndroidAppId() => AndroidAppId;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use AndroidAppSignature accessor instead.")]
        public static string GetAndroidAppSignature() => AndroidAppSignature;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IsLoggingEnabled accessor instead.")]
        public static void EnableLogging(bool enabled) => IsLoggingEnabled = enabled;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IsLoggingEnabled accessor instead.")]
        public static bool IsLogging() => IsLoggingEnabled;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IsAutomaticInitializationEnabled accessor instead.")]
        public static void EnableAutomaticInit(bool enabled) => IsAutomaticInitializationEnabled = enabled;
        [Obsolete("Method will be removed in future versions of Helium Unity SDK. Use IsAutomaticInitializationEnabled accessor instead.")]
        public static bool IsAutomaticInit() => IsAutomaticInitializationEnabled;
        #endregion

        private static void DirtyEditor()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }

        private static string EvaluateCredential(string credential, string exampleCredential, string platform, string field)
        {
            if (_credentialsWarning)
                return credential;

            _credentialsWarning = true;
            
            if (credential == exampleCredential)
            {
                Debug.LogWarning(string.Format(CredentialsWarningDefaultFormat, platform, field));
                return exampleCredential;
            }
            if (string.IsNullOrEmpty(credential))
                Debug.LogWarning(string.Format(CredentialsWarningEmptyFormat, platform, field));
            return credential;
        }

        public static void ResetSettings()
        {
            // iOS
            if (Instance.iOSAppId.Equals(IOSExampleAppID))
                IOSAppId = IOSExampleAppIDLabel;
            // Android
            if (Instance.androidAppId.Equals(AndroidExampleAppID))
                AndroidAppId = AndroidExampleAppIDLabel;
        }

        #endregion
    }
}
