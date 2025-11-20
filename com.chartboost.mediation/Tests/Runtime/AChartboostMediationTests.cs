using System.Collections;
using System.Reflection;
using Chartboost.Logging;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Default;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// ReSharper disable UnusedMember.Local

namespace Chartboost.Tests.Runtime
{
    /// <summary>
    /// Tests for ChartboostMediation static class.
    /// Class name starts with "A" to ensure it runs first before other tests that might initialize the SDK.
    /// </summary>
    public class AChartboostMediationTests
    {
        private ChartboostMediationBase _originalInstance;
        private bool _instanceCaptured;

        /// <summary>
        /// Helper method to get the Instance property via reflection.
        /// </summary>
        private PropertyInfo GetInstanceProperty()
        {
            var instanceType = typeof(ChartboostMediation);
            var instanceProperty = instanceType.GetProperty(TestConstants.ChartboostMediation.InstancePropertyName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(instanceProperty, $"Property '{TestConstants.ChartboostMediation.InstancePropertyName}' not found via reflection");
            return instanceProperty;
        }

        /// <summary>
        /// Helper method to get the current Instance value.
        /// </summary>
        private ChartboostMediationBase GetInstanceValue()
        {
            return (ChartboostMediationBase)GetInstanceProperty().GetValue(null);
        }

        /// <summary>
        /// Helper method to set the Instance value.
        /// </summary>
        private void SetInstanceValue(ChartboostMediationBase value)
        {
            GetInstanceProperty().SetValue(null, value);
        }

        [SetUp]
        public void SetUp()
        {
            // Store the original instance to restore it after each test
            _originalInstance = GetInstanceValue();
            _instanceCaptured = true;

            // Verify we captured a valid instance
            Assert.IsNotNull(_originalInstance, "Original instance is null - cannot ensure test isolation");
        }

        [TearDown]
        public void TearDown()
        {
            // Restore the original instance after each test
            if (!_instanceCaptured)
                return;

            SetInstanceValue(_originalInstance);

            // Verify restoration
            var restoredInstance = GetInstanceValue();
            Assert.AreSame(_originalInstance, restoredInstance, "Failed to restore original instance");
        }

        [Test]
        public void InstanceIsInitializedCorrectly()
        {
            var instanceProperty = GetInstanceProperty();
            Assert.IsNotNull(instanceProperty);

            var instance = GetInstanceValue();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ChartboostMediationBase>(instance);
        }

        [Test]
        public void InstanceGetterReturnsValue()
        {
            var instanceProperty = GetInstanceProperty();
            Assert.IsNotNull(instanceProperty.GetMethod, "Instance property should have a getter");

            var instance = GetInstanceValue();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ChartboostMediationBase>(instance);
        }

        [Test]
        public void InstanceSetterChangesValue()
        {
            var instanceProperty = GetInstanceProperty();
            Assert.IsNotNull(instanceProperty.SetMethod, "Instance property should have a setter");

            var newInstance = new ChartboostMediationDefault();
            SetInstanceValue(newInstance);

            var retrievedInstance = GetInstanceValue();
            Assert.AreSame(newInstance, retrievedInstance);
        }

        [Test]
        public void InstanceSetterAcceptsNullValue()
        {
            var instanceProperty = GetInstanceProperty();
            Assert.IsNotNull(instanceProperty.SetMethod, "Instance property should have a setter");

            // Set instance to null (this should be allowed for auto-properties)
            SetInstanceValue(null);

            var retrievedInstance = GetInstanceValue();
            Assert.IsNull(retrievedInstance, "Instance should be null after setting to null");
        }

        [Test]
        public void InstanceTypeMatchesPlatform()
        {
            var instance = GetInstanceValue();
            Assert.IsNotNull(instance);

#if UNITY_EDITOR
            // In editor, should always be ChartboostMediationDefault
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedDefaultInstanceName, instance.GetType().Name);
#elif UNITY_ANDROID
            // On Android devices, should be Android.ChartboostMediation
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedPlatformInstanceName, instance.GetType().Name);
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedAndroidNamespace, instance.GetType().Namespace);
#elif UNITY_IOS
            // On iOS devices, should be iOS.ChartboostMediation
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedPlatformInstanceName, instance.GetType().Name);
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedIOSNamespace, instance.GetType().Namespace);
#else
            // On unsupported platforms, should fall back to the default implementation
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedDefaultInstanceName, instance.GetType().Name, "Expected ChartboostMediationDefault on unsupported platforms");
#endif
        }

        [Test]
        public void ChartboostMediationDefaultCanBeInstantiated()
        {
            // This test ensures that ChartboostMediationDefault constructor is covered
            // The static field initializer uses: new ChartboostMediationDefault()
            var defaultInstance = new ChartboostMediationDefault();

            Assert.IsNotNull(defaultInstance);
            Assert.IsInstanceOf<ChartboostMediationBase>(defaultInstance);
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedCoreModuleId, defaultInstance.CoreModuleId);
            Assert.AreEqual(string.Empty, defaultInstance.NativeSDKVersion);
            Assert.IsNotNull(defaultInstance.AdaptersInfo);
            Assert.AreEqual(0, defaultInstance.AdaptersInfo.Length);
        }

        [Test]
        public void LoggingLevel()
        {
            var initial = ChartboostMediation.LogLevel;

            ChartboostMediation.LogLevel = LogLevel.Disabled;
            Assert.AreEqual(LogLevel.Disabled, ChartboostMediation.LogLevel);

            ChartboostMediation.LogLevel = LogLevel.Error;
            Assert.AreEqual(LogLevel.Error, ChartboostMediation.LogLevel);

            ChartboostMediation.LogLevel = LogLevel.Warning;
            Assert.AreEqual(LogLevel.Warning, ChartboostMediation.LogLevel);

            ChartboostMediation.LogLevel = LogLevel.Info;
            Assert.AreEqual(LogLevel.Info, ChartboostMediation.LogLevel);

            ChartboostMediation.LogLevel = LogLevel.Debug;
            Assert.AreEqual(LogLevel.Debug, ChartboostMediation.LogLevel);

            ChartboostMediation.LogLevel = LogLevel.Verbose;
            Assert.AreEqual(LogLevel.Verbose, ChartboostMediation.LogLevel);

            ChartboostMediation.LogLevel = initial;
            Assert.AreEqual(initial, ChartboostMediation.LogLevel);
        }

        [Test]
        public void SDKVersionReturnsExpectedVersion()
        {
            Assert.IsNotNull(ChartboostMediation.SDKVersion);
            Assert.IsNotEmpty(ChartboostMediation.SDKVersion);
            Assert.AreEqual(TestConstants.Versions.ExpectedSDK, ChartboostMediation.SDKVersion);
        }

        [Test]
        public void NativeSDKVersionReturnsValue()
        {
            var version = ChartboostMediation.NativeSDKVersion;
            Assert.IsNotNull(version);
        }

        [Test]
        public void CoreModuleIdReturnsExpectedValue()
        {
            var moduleId = ChartboostMediation.CoreModuleId;
            Assert.IsNotNull(moduleId);
            Assert.IsNotEmpty(moduleId);
            Assert.AreEqual(TestConstants.ChartboostMediation.ExpectedCoreModuleId, moduleId);
        }

        [Test]
        public void TestModeCanBeSetAndRetrieved()
        {
            var initial = ChartboostMediation.TestMode;

            ChartboostMediation.TestMode = true;
            Assert.IsTrue(ChartboostMediation.TestMode);

            ChartboostMediation.TestMode = false;
            Assert.IsFalse(ChartboostMediation.TestMode);

            ChartboostMediation.TestMode = initial;
        }

        [Test]
        public void DiscardOverSizedAdsCanBeSetAndRetrieved()
        {
            var initial = ChartboostMediation.DiscardOverSizedAds;

            ChartboostMediation.DiscardOverSizedAds = true;
            Assert.IsTrue(ChartboostMediation.DiscardOverSizedAds);

            ChartboostMediation.DiscardOverSizedAds = false;
            Assert.IsFalse(ChartboostMediation.DiscardOverSizedAds);

            ChartboostMediation.DiscardOverSizedAds = initial;
        }

        [Test]
        public void AdaptersInfoReturnsNonNullArray()
        {
            var adaptersInfo = ChartboostMediation.AdaptersInfo;
            Assert.IsNotNull(adaptersInfo);
        }

        [Test]
        public void SetPreInitializationConfigurationWithValidConfigurationReturnsNoError()
        {
            var config = new ChartboostMediationPreInitializationConfiguration(new System.Collections.Generic.HashSet<string>());
            var error = ChartboostMediation.SetPreInitializationConfiguration(config);
            Assert.IsNull(error);
        }

        [Test]
        public void SetPreInitializationConfigurationWithSkippablePartnersReturnsNoError()
        {
            var skippablePartners = new System.Collections.Generic.HashSet<string> { TestConstants.Partners.Partner1, TestConstants.Partners.Partner2 };
            var config = new ChartboostMediationPreInitializationConfiguration(skippablePartners);
            var error = ChartboostMediation.SetPreInitializationConfiguration(config);
            Assert.IsNull(error);
        }

        [Test]
        public void SetPreInitializationConfigurationWithNullSkippablePartnersReturnsNoError()
        {
            var config = new ChartboostMediationPreInitializationConfiguration(null);
            var error = ChartboostMediation.SetPreInitializationConfiguration(config);
            Assert.IsNull(error);
        }

        [Test]
        public void GetBannerAdReturnsValidBannerAdInstance()
        {
            var bannerAd = ChartboostMediation.GetBannerAd();
            Assert.IsNotNull(bannerAd);
            Assert.IsInstanceOf<IBannerAd>(bannerAd);
        }

        [Test]
        public void GetFullscreenAdQueueWithValidPlacementReturnsValidQueue()
        {
            var queue = ChartboostMediation.GetFullscreenAdQueue(TestConstants.Placements.Standard);
            Assert.IsNotNull(queue);
            Assert.IsInstanceOf<IFullscreenAdQueue>(queue);
        }

        [Test]
        public void GetFullscreenAdQueueWithSamePlacementReturnsSameInstance()
        {
            var queue1 = ChartboostMediation.GetFullscreenAdQueue(TestConstants.Placements.Singleton);
            var queue2 = ChartboostMediation.GetFullscreenAdQueue(TestConstants.Placements.Singleton);
            Assert.AreSame(queue1, queue2);
        }

        [UnityTest]
        public IEnumerator LoadFullscreenAdWithValidRequestReturnsResult()
        {
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard);
            var task = ChartboostMediation.LoadFullscreenAd(request);

            yield return new WaitUntil(() => task.IsCompleted);

            Assert.IsNotNull(task.Result);
            Assert.IsInstanceOf<FullscreenAdLoadResult>(task.Result);
        }

        [Test]
        public void GetUnityBannerAdWithValidPlacementReturnsValidInstance()
        {
            var unityBannerAd = ChartboostMediation.GetUnityBannerAd(TestConstants.Placements.Standard);
            Assert.IsNotNull(unityBannerAd);
            Assert.IsInstanceOf<UnityBannerAd>(unityBannerAd);
            Assert.AreEqual(TestConstants.Placements.Standard, unityBannerAd.PlacementName);

            if (unityBannerAd != null && unityBannerAd.gameObject != null)
                Object.DestroyImmediate(unityBannerAd.gameObject);
        }

        [Test]
        public void GetUnityBannerAdWithParentCreatesUnderParent()
        {
            var parent = new GameObject(TestConstants.ChartboostMediation.TestParentGameObjectName);
            var unityBannerAd = ChartboostMediation.GetUnityBannerAd(TestConstants.Placements.Standard, parent.transform);

            Assert.IsNotNull(unityBannerAd);
            Assert.AreEqual(parent.transform, unityBannerAd.transform.parent);

            if (unityBannerAd != null && unityBannerAd.gameObject != null)
                Object.DestroyImmediate(unityBannerAd.gameObject);
            if (parent != null)
                Object.DestroyImmediate(parent);
        }

        [Test]
        public void GetBannerVisualElementWithValidPlacementReturnsValidInstance()
        {
            var bannerVisualElement = ChartboostMediation.GetBannerVisualElement(TestConstants.Placements.Standard);
            Assert.IsNotNull(bannerVisualElement);
            Assert.AreEqual(TestConstants.Placements.Standard, bannerVisualElement.PlacementName);
        }

        [UnityTest]
        public IEnumerator OnDidReceiveImpressionLevelRevenueDataWithNullJsonLogsError()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.ILRD));
            ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(null);
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnDidReceiveImpressionLevelRevenueDataWithEmptyJsonLogsError()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.ILRD));
            ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(string.Empty);
            yield return null;
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        public IEnumerator OnDidReceiveImpressionLevelRevenueDataWithInvalidJsonLogsError()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.UnexpectedCharacter));
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.ILRD));
            ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(TestConstants.ChartboostMediation.InvalidJsonString);
            yield return null;
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void DidReceiveImpressionLevelRevenueDataEventSubscriptionWorksCorrectly()
        {
            var eventReceived = false;
            ChartboostMediationImpressionLevelRevenueDataEvent handler = (_, _) =>
            {
                eventReceived = true;
            };

            ChartboostMediation.DidReceiveImpressionLevelRevenueData += handler;
            ChartboostMediation.DidReceiveImpressionLevelRevenueData -= handler;

            Assert.IsFalse(eventReceived);
        }

        [Test]
        public void DidReceivePartnerAdapterInitializationDataEventSubscriptionWorksCorrectly()
        {
            var eventReceived = false;
            ChartboostMediationPartnerAdapterInitializationEvent handler = _ =>
            {
                eventReceived = true;
            };

            ChartboostMediation.DidReceivePartnerAdapterInitializationData += handler;
            ChartboostMediation.DidReceivePartnerAdapterInitializationData -= handler;

            Assert.IsFalse(eventReceived);
        }

        [UnityTest]
        public IEnumerator OnDidReceiveImpressionLevelRevenueDataWithValidJsonInvokesEvent()
        {
            string receivedPlacementName = null;
            Hashtable receivedData = null;
            var eventReceived = false;

            ChartboostMediationImpressionLevelRevenueDataEvent handler = (placementName, data) =>
            {
                receivedPlacementName = placementName;
                receivedData = data;
                eventReceived = true;
            };

            ChartboostMediation.DidReceiveImpressionLevelRevenueData += handler;
            ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(TestConstants.ChartboostMediation.ValidJsonWithPlacement);

            yield return null;

            Assert.IsTrue(eventReceived);
            Assert.AreEqual(TestConstants.Placements.Standard, receivedPlacementName);
            Assert.IsNotNull(receivedData);
            Assert.IsTrue(receivedData.ContainsKey(TestConstants.ChartboostMediation.PlacementKey));
            Assert.AreEqual(TestConstants.Placements.Standard, receivedData[TestConstants.ChartboostMediation.PlacementKey]);

            ChartboostMediation.DidReceiveImpressionLevelRevenueData -= handler;
        }

        [UnityTest]
        public IEnumerator ValidJsonWithoutPlacementInvokesEvent()
        {
            string receivedPlacementName = null;
            Hashtable receivedData = null;
            var eventReceived = false;

            ChartboostMediationImpressionLevelRevenueDataEvent handler = (placementName, data) =>
            {
                receivedPlacementName = placementName;
                receivedData = data;
                eventReceived = true;
            };

            ChartboostMediation.DidReceiveImpressionLevelRevenueData += handler;
            ChartboostMediation.OnDidReceiveImpressionLevelRevenueData(TestConstants.ChartboostMediation.ValidJsonWithoutPlacement);

            yield return null;

            Assert.IsTrue(eventReceived);
            Assert.IsNull(receivedPlacementName);
            Assert.IsNotNull(receivedData);
            Assert.IsFalse(receivedData.ContainsKey(TestConstants.ChartboostMediation.PlacementKey));

            ChartboostMediation.DidReceiveImpressionLevelRevenueData -= handler;
        }

        [UnityTest]
        public IEnumerator OnDidReceivePartnerAdapterInitializationDataInvokesEvent()
        {
            string receivedData = null;
            var eventReceived = false;

            ChartboostMediationPartnerAdapterInitializationEvent handler = data =>
            {
                receivedData = data;
                eventReceived = true;
            };

            ChartboostMediation.DidReceivePartnerAdapterInitializationData += handler;
            ChartboostMediation.OnDidReceivePartnerAdapterInitializationData(TestConstants.ChartboostMediation.TestPartnerData);

            yield return null;

            Assert.IsTrue(eventReceived);
            Assert.AreEqual(TestConstants.ChartboostMediation.TestPartnerData, receivedData);

            ChartboostMediation.DidReceivePartnerAdapterInitializationData -= handler;
        }

        [UnityTest]
        public IEnumerator OnDidReceivePartnerAdapterInitializationDataWithNullInvokesEvent()
        {
            string receivedData = TestConstants.ChartboostMediation.TestPartnerData;
            var eventReceived = false;

            ChartboostMediationPartnerAdapterInitializationEvent handler = data =>
            {
                receivedData = data;
                eventReceived = true;
            };

            ChartboostMediation.DidReceivePartnerAdapterInitializationData += handler;
            ChartboostMediation.OnDidReceivePartnerAdapterInitializationData(null);

            yield return null;

            Assert.IsTrue(eventReceived);
            Assert.IsNull(receivedData);

            ChartboostMediation.DidReceivePartnerAdapterInitializationData -= handler;
        }

        [UnityTest]
        public IEnumerator OnDidReceivePartnerAdapterInitializationDataWithEmptyStringInvokesEvent()
        {
            string receivedData = null;
            var eventReceived = false;

            ChartboostMediationPartnerAdapterInitializationEvent handler = data =>
            {
                receivedData = data;
                eventReceived = true;
            };

            ChartboostMediation.DidReceivePartnerAdapterInitializationData += handler;
            ChartboostMediation.OnDidReceivePartnerAdapterInitializationData(string.Empty);

            yield return null;

            Assert.IsTrue(eventReceived);
            Assert.AreEqual(string.Empty, receivedData);

            ChartboostMediation.DidReceivePartnerAdapterInitializationData -= handler;
        }
    }
}
