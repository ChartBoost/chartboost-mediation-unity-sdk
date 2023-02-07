using System;
using System.ComponentModel;
using UnityEngine;
using System.IO;
using Chartboost;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Chartboost
{
    /// <summary>
    /// List of officially supported Chartboost Mediation mediation partners and their identifiers
    /// </summary>
    [Flags]
    public enum ChartboostMediationPartners
    {
        [Description("none")] None = 0,
        [Description("adcolony")] AdColony = 1,
        [Description("admob")] AdMob = 2,
        [Description("amazon_aps")] Amazon = 4,
        [Description("applovin")] AppLovin = 8,
        [Description("facebook")] Facebook = 16,
        [Description("fyber")] Fyber = 32,
        [Description("google_googlebidding")] GoogleBidding = 64,
        [Description("inmobi")] InMobi = 128,
        [Description("ironsource")] IronSource = 256,
        [Description("mintegral")] Mintegral = 512,
        [Description("pangle")] Pangle = 1024,
        [Description("tapjoy")] TapJoy = 2048,
        [Description("unity")] UnityAds = 4096,
        [Description("vungle")] Vungle = 8192,
        [Description("yahoo")] Yahoo = 16384
    }

    /// <summary>
    /// Chartboost Mediation Unity SDK Settings as an scriptable object with accessors
    /// </summary>
    public class ChartboostMediationSettings : ScriptableObject
    {
        private const string Package = "com.chartboost.mediation";
        private const string CbSettingsAssetName = "ChartboostMediationSettings";
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

        private const string CredentialsWarningDefaultFormat =
            "You are using the Chartboost Mediation SDK {0} example {1}! Go to the Chartboost Mediation dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";

        private const string CredentialsWarningEmptyFormat =
            "You are using an empty string for the {0} {1}! Go to the Chartboost Mediation dashboard and replace these with an App ID & App Signature from your account! If you need help, check out answers.chartboost.com";

        private const string CredentialsWarningIOS = "IOS";
        private const string CredentialsWarningAndroid = "Android";
        private const string CredentialsWarningAppID = "App ID";
        private const string CredentialsWarningAppSignature = "App Signature";

        private static bool _credentialsWarning = false;

        private static ChartboostMediationSettings _instance;

        /// <summary>
        /// Creates or Fetches a ChartboostMediationSettings instance.
        /// </summary>
        internal static ChartboostMediationSettings Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                _instance = Resources.Load(CbSettingsAssetName) as ChartboostMediationSettings;
                if (_instance != null)
                    return _instance;
                // If not found, auto-create the asset object.
                _instance = CreateInstance<ChartboostMediationSettings>();
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
        [MenuItem("Chartboost Mediation/Edit Settings")]
        public static void Edit()
        {
            Selection.activeObject = Instance;
        }

        [MenuItem("Chartboost Mediation/Documentation")]
        public static void OpenDocumentation()
        {
            const string url = "https://developers.chartboost.com/docs/mediation-unity-get-started";
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
        [SerializeField] private bool isSkAdNetworkResolutionEnabled;
        [SerializeField] private ChartboostMediationPartners partnerKillSwitch = ChartboostMediationPartners.None;

        /// <summary>
        /// Accessor for partnerKillSwitch. 
        /// </summary>
        public static ChartboostMediationPartners PartnerKillSwitch
        {
            get => Instance.partnerKillSwitch;
            set => Instance.partnerKillSwitch = value;
        }

        /// <summary>
        /// Accessor for androidAppId.
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
        /// Accessor for androidAppSignature.
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
        /// Accessor for iOSAppId.
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
        /// Accessor for iOSAppSignature.
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
        /// Accessor for isLoggingEnabled.
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

        /// <summary>
        /// Accessor for isSkAdNetworkResolutionEnabled.
        /// </summary>
        public static bool IsSkAdNetworkResolutionEnabled
        {
            get => Instance.isSkAdNetworkResolutionEnabled;
            set
            {
                Instance.isSkAdNetworkResolutionEnabled = value;
                DirtyEditor();
            }
        }

        private static void DirtyEditor()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(Instance);
#endif
        }

        private static string EvaluateCredential(string credential, string exampleCredential, string platform,
            string field)
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

namespace Helium
{
    [Obsolete("HeliumSettings has been replaced with ChartboostMediationSettings, and will be removed in the future.")]
    public class HeliumSettings : ScriptableObject
    {
        private static ChartboostMediationSettings _instance;

        private static ChartboostMediationSettings Instance = ChartboostMediationSettings.Instance;

        public static ChartboostMediationPartners PartnerKillSwitch
        {
            get => ChartboostMediationSettings.PartnerKillSwitch;
            set => ChartboostMediationSettings.PartnerKillSwitch = value;
        }

        public static string AndroidAppId
        {
            get => ChartboostMediationSettings.AndroidAppId;
            set => ChartboostMediationSettings.AndroidAppId = value;
        }

        public static string AndroidAppSignature
        {
            get => ChartboostMediationSettings.AndroidAppSignature;
            set => ChartboostMediationSettings.AndroidAppSignature = value;
        }

        public static string IOSAppId
        {
            get => ChartboostMediationSettings.IOSAppId;
            set => ChartboostMediationSettings.IOSAppId = value;
        }

        public static string IOSAppSignature
        {
            get => ChartboostMediationSettings.IOSAppSignature;
            set => ChartboostMediationSettings.IOSAppSignature = value;
        }

        public static bool IsLoggingEnabled
        {
            get => ChartboostMediationSettings.IsLoggingEnabled;
            set => ChartboostMediationSettings.IsLoggingEnabled = value;
        }

        public static bool IsAutomaticInitializationEnabled
        {
            get => ChartboostMediationSettings.IsAutomaticInitializationEnabled;
            set => ChartboostMediationSettings.IsAutomaticInitializationEnabled = value;
        }

        public static bool IsSkAdNetworkResolutionEnabled
        {
            get => ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled;
            set => ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = value;
        }
    }
}
