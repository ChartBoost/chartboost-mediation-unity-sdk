using System;
using System.Threading.Tasks;
using System.Reflection;
using Chartboost.Core.Error;
using Chartboost.Core.Initialization;
using Chartboost.Logging;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Initialization;
using Chartboost.Mediation.Requests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime
{
    public class ChartboostMediationBaseTests
    {
        private const string WrongModuleId = "wrong_module_id";
        private const string PropertyNameIsInitialized = "IsInitialized";
        private const string MethodNameOnMediationInitialized = "OnMediationInitialized";
        private const int TestErrorCode = 100;
        private const int TestInitializationDurationSeconds = 1;
        private const int ExpectedEmptyArrayLength = 0;

        private TestChartboostMediationBase _testInstance;
        private bool _originalIsInitialized;
        private string _actualCoreModuleId;

        [SetUp]
        public void SetUp()
        {
            _testInstance = new TestChartboostMediationBase();

            _actualCoreModuleId = ChartboostMediation.CoreModuleId;

            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            _originalIsInitialized = (bool)(isInitializedProperty?.GetValue(null) ?? false);
        }

        [TearDown]
        public void TearDown()
        {
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, _originalIsInitialized);
        }

        [Test]
        public void CoreModuleIdReturnsExpectedValue()
        {
            var result = _testInstance.CoreModuleId;

            Assert.AreEqual(_actualCoreModuleId, result);
        }

        [Test]
        public void NativeSDKVersionReturnsExpectedValue()
        {
            var result = _testInstance.NativeSDKVersion;

            Assert.AreEqual(TestConstants.Versions.NativeSDK, result);
        }

        [Test]
        public void TestModeCanBeSet()
        {
            _testInstance.TestMode = true;

            Assert.IsTrue(_testInstance.TestMode);

            _testInstance.TestMode = false;

            Assert.IsFalse(_testInstance.TestMode);
        }

        [Test]
        public void LogLevelCanBeSet()
        {
            var originalLogLevel = LogController.LoggingLevel;

            _testInstance.LogLevel = LogLevel.Debug;

            Assert.AreEqual(LogLevel.Debug, _testInstance.LogLevel);
            Assert.AreEqual(LogLevel.Debug, LogController.LoggingLevel);

            _testInstance.LogLevel = LogLevel.Error;

            Assert.AreEqual(LogLevel.Error, _testInstance.LogLevel);
            Assert.AreEqual(LogLevel.Error, LogController.LoggingLevel);

            // Restore original
            LogController.LoggingLevel = originalLogLevel;
        }

        [Test]
        public void DiscardOverSizedAdsCanBeSet()
        {
            _testInstance.DiscardOverSizedAds = true;

            Assert.IsTrue(_testInstance.DiscardOverSizedAds);

            _testInstance.DiscardOverSizedAds = false;

            Assert.IsFalse(_testInstance.DiscardOverSizedAds);
        }

        [Test]
        public void AdaptersInfoReturnsEmptyArray()
        {
            var result = _testInstance.AdaptersInfo;

            Assert.IsNotNull(result);
            Assert.AreEqual(ExpectedEmptyArrayLength, result.Length);
        }

        [Test]
        public void SetPreInitializationConfigurationLogsInfo()
        {
            var config = new ChartboostMediationPreInitializationConfiguration();

            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.PreInitConfig));

            var result = _testInstance.SetPreInitializationConfiguration(config);

            Assert.IsNull(result);
        }

        [Test]
        public void CanFetchAdReturnsFalseWhenNotInitialized()
        {
            // Set IsInitialized to false
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, false);

            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.NotInitialized));

            var result = TestChartboostMediationBase.TestCanFetchAd(TestConstants.Placements.Standard);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanFetchAdReturnsFalseWhenPlacementIsNull()
        {
            // Set IsInitialized to true
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, true);

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.NullPlacement));

            var result = TestChartboostMediationBase.TestCanFetchAd(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanFetchAdReturnsFalseWhenPlacementIsEmpty()
        {
            // Set IsInitialized to true
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, true);

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.NullPlacement));

            var result = TestChartboostMediationBase.TestCanFetchAd(TestConstants.Placements.Empty);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanFetchAdReturnsTrueWhenInitializedAndPlacementValid()
        {
            // Set IsInitialized to true
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, true);

            var result = TestChartboostMediationBase.TestCanFetchAd(TestConstants.Placements.Standard);

            Assert.IsTrue(result);
        }

        [Test]
        public void OnMediationInitializedSetsIsInitializedWhenSuccessful()
        {
            // Set IsInitialized to false initially
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, false);

            // Create a successful initialization result using the actual CoreModuleId
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddSeconds(TestInitializationDurationSeconds);
            var result = new ModuleInitializationResult(startTime, endTime, _actualCoreModuleId, TestConstants.Versions.Module, null);

            // Invoke the handler directly
            var handlerMethod = typeof(ChartboostMediationBase).GetMethod(MethodNameOnMediationInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            handlerMethod?.Invoke(null, new object[] { result });

            var isInitialized = (bool)(isInitializedProperty?.GetValue(null) ?? false);
            Assert.IsTrue(isInitialized);
        }

        [Test]
        public void OnMediationInitializedDoesNotSetIsInitializedWhenWrongModuleId()
        {
            // Set IsInitialized to false initially
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, false);

            // Create a result with the wrong module ID
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddSeconds(TestInitializationDurationSeconds);
            var result = new ModuleInitializationResult(startTime, endTime, WrongModuleId, TestConstants.Versions.Module, null);

            // Invoke the handler directly
            var handlerMethod = typeof(ChartboostMediationBase).GetMethod(MethodNameOnMediationInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            handlerMethod?.Invoke(null, new object[] { result });

            var isInitialized = (bool)(isInitializedProperty?.GetValue(null) ?? false);
            Assert.IsFalse(isInitialized);
        }

        [Test]
        public void OnMediationInitializedLogsErrorWhenInitializationFails()
        {
            // Set IsInitialized to false initially
            var isInitializedProperty = typeof(ChartboostMediationBase).GetProperty(PropertyNameIsInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            isInitializedProperty?.SetValue(null, false);

            // Create a failed initialization result with ChartboostCoreError using actual CoreModuleId
            var error = new ChartboostCoreError(TestErrorCode, TestConstants.Errors.Message);
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddSeconds(TestInitializationDurationSeconds);
            var result = new ModuleInitializationResult(startTime, endTime, _actualCoreModuleId, TestConstants.Versions.Module, error);

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.InitializationFailed));

            // Invoke the handler directly
            var handlerMethod = typeof(ChartboostMediationBase).GetMethod(MethodNameOnMediationInitialized, BindingFlags.NonPublic | BindingFlags.Static);
            handlerMethod?.Invoke(null, new object[] { result });

            var isInitialized = (bool)(isInitializedProperty?.GetValue(null) ?? false);
            Assert.IsFalse(isInitialized);
        }

        // Test helper class
        private class TestChartboostMediationBase : ChartboostMediationBase
        {
            public override string CoreModuleId => ChartboostMediation.CoreModuleId;
            public override string NativeSDKVersion => TestConstants.Versions.NativeSDK;

            public override bool TestMode { get; set; }

            public override bool DiscardOverSizedAds { get; set; }

            public override AdapterInfo[] AdaptersInfo => Array.Empty<AdapterInfo>();

            public override Task<FullscreenAdLoadResult> LoadFullscreenAd(FullscreenAdLoadRequest request)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                return Task.FromResult(new FullscreenAdLoadResult(null, null, null, null));
                // ReSharper restore AssignNullToNotNullAttribute
            }

            public override IFullscreenAdQueue GetFullscreenAdQueue(string placementName) => null;

            public override IBannerAd GetBannerAd() => null;

            // Expose internal method for testing
            public static bool TestCanFetchAd(string placementName) => CanFetchAd(placementName);
        }
    }
}
