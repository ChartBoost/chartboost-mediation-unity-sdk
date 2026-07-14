using System.Collections;
using Chartboost.Core;
using Chartboost.Core.Initialization;
using Chartboost.Mediation;
using NUnit.Framework;
using UnityEngine;

namespace Chartboost.Tests.Runtime
{
    /// <summary>
    /// Helper class to initialize Chartboost Mediation SDK for tests on native platforms (Android/iOS).
    /// Tests that need native SDK functionality should explicitly call Initialize() in their SetUp.
    /// This is not a SetUpFixture to avoid interfering with SetPreInitializationConfiguration tests.
    /// </summary>
    public static class TestInitializer
    {
#if UNITY_ANDROID
        private const string TestAppId = "5ce82fbffde3570afb4647bc";
#elif UNITY_IOS
        private const string TestAppId = "59c2b75ed7d75f0da04c452f";
#else
        private const string TestAppId = "test_app_id";
#endif

        private static bool _isMediationInitialized;
        private static bool _initializationStarted;
        private static readonly object InitializationLock = new object();

        /// <summary>
        /// Returns true if we should initialize the SDK (on Android/iOS devices, not in Editor).
        /// </summary>
        private static bool ShouldInitializeSDK => (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !Application.isEditor;

        /// <summary>
        /// Initializes the Chartboost Mediation SDK if on a native platform and not already initialized.
        /// Call this from test SetUp methods that require native SDK functionality.
        /// </summary>
        public static void Initialize()
        {
            if (!ShouldInitializeSDK)
            {
                Debug.Log("[TestInitializer] Running in Editor or non-native platform - skipping SDK initialization");
                return;
            }

            // On native platforms, we need to initialize the SDK
            lock (InitializationLock)
            {
                if (!_initializationStarted)
                {
                    Debug.Log($"[TestInitializer] Initializing Chartboost Mediation SDK with test app ID: {TestAppId}");

                    // Subscribe to module initialization events
                    ChartboostCore.ModuleInitializationCompleted += OnModuleInitializationCompleted;

                    // Initialize Chartboost Core, which will initialize Mediation
                    var sdkConfig = new SDKConfiguration(TestAppId, null);
                    ChartboostCore.Initialize(sdkConfig);

                    // Mark as started to prevent duplicate initialization
                    _initializationStarted = true;

                    Debug.Log("[TestInitializer] Chartboost Mediation SDK initialization started");
                }
            }
        }

        /// <summary>
        /// Cleans up SDK initialization. Call this from test TearDown if needed.
        /// </summary>
        public static void Cleanup()
        {
            if (ShouldInitializeSDK)
            {
                // Unsubscribe from events
                ChartboostCore.ModuleInitializationCompleted -= OnModuleInitializationCompleted;
            }
            Debug.Log("[TestInitializer] Cleanup completed");
        }

        /// <summary>
        /// Called when a ChartboostCore module completes initialization.
        /// We use this to track when Mediation is ready.
        /// </summary>
        private static void OnModuleInitializationCompleted(ModuleInitializationResult result)
        {
            // Check if this is the Chartboost Mediation module
            if (result.ModuleId == ChartboostMediation.CoreModuleId)
            {
                if (result.Error == null)
                {
                    _isMediationInitialized = true;
                    Debug.Log($"[TestInitializer] Chartboost Mediation initialized successfully (Duration: {result.Duration}ms)");
                }
                else
                {
                    Debug.LogError($"[TestInitializer] Chartboost Mediation initialization failed: {result.Error}");
                }
            }
        }

        /// <summary>
        /// Coroutine to wait for Mediation SDK initialization to complete.
        /// Call this from UnityTest methods that need the SDK to be initialized.
        /// </summary>
        public static IEnumerator WaitForInitialization()
        {
            if (!ShouldInitializeSDK)
            {
                yield break;
            }

            // If already initialized, return immediately
            if (_isMediationInitialized)
            {
                yield break;
            }

            var maxWaitTime = 15f; // 15 seconds max wait
            var startTime = Time.realtimeSinceStartup;

            Debug.Log("[TestInitializer] Waiting for Chartboost Mediation initialization...");

            // Wait until Mediation is initialized or timeout
            while (!_isMediationInitialized && Time.realtimeSinceStartup - startTime < maxWaitTime)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (_isMediationInitialized)
            {
                Debug.Log("[TestInitializer] Chartboost Mediation is ready");
            }
            else
            {
                Assert.Inconclusive($"[TestInitializer] Chartboost Mediation initialization timed out after {maxWaitTime} seconds. " +
                    "The SDK did not complete initialization — subsequent tests that depend on it will be skipped.");
            }
        }
    }
}
