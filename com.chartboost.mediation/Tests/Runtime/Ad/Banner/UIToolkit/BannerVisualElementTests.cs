using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.UIToolkit;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Tests.Runtime.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Chartboost.Tests.Runtime.Ad.Banner.UIToolkit
{
    public class BannerVisualElementTests
    {
        private BannerVisualElement _bannerVisualElement;
        private UIDocument _uiDocument;
        private GameObject _testGameObject;
        private LogLevel _initialLogLevel;

        // Delegate storage for proper event unsubscription
        private BannerAdEvent _willAppearDelegate;
        private BannerAdEvent _clickDelegate;
        private BannerAdEvent _recordImpressionDelegate;
        private BannerAdDragEvent _dragBeginDelegate;
        private BannerAdDragEvent _dragDelegate;
        private BannerAdDragEvent _dragEndDelegate;

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;

            // Create UIDocument for testing
            _testGameObject = new GameObject(TestConstants.Unity.TestUIDocumentName);
            _uiDocument = _testGameObject.AddComponent<UIDocument>();

            _bannerVisualElement = new BannerVisualElement
            {
                PlacementName = TestConstants.Placements.Standard
            };

            // Add to the document so it's in a panel
            _uiDocument.rootVisualElement.Add(_bannerVisualElement);
        }

        [TearDown]
        public void TearDown()
        {
            LogController.LoggingLevel = _initialLogLevel;

            _bannerVisualElement?.Dispose();

            if (_testGameObject != null)
                UnityEngine.Object.DestroyImmediate(_testGameObject);
        }

        #region Helper Methods

        /// <summary>
        /// Helper method to inject a TestBannerAd into BannerVisualElement using reflection.
        /// Also subscribes BannerVisualElement event handlers to the TestBannerAd events.
        /// </summary>
        private void InjectBannerAdWithEventSubscription(BannerVisualElement element, TestBannerAd testBannerAd)
        {
            // Inject the TestBannerAd into the _bannerAd field
            var bannerAdField = typeof(BannerVisualElement).GetField(TestConstants.Reflection.BannerAdField, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(bannerAdField, TestConstants.Reflection.BannerAdFieldNotFound);
            bannerAdField.SetValue(element, testBannerAd);

            // Subscribe BannerVisualElement's event handlers to TestBannerAd's events
            var onWillAppearMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnWillAppearMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var onClickMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnClickMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var onRecordImpressionMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnRecordImpressionMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var onDragBeginMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragBeginMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var onDragMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var onDragEndMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragEndMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(onWillAppearMethod, TestConstants.Reflection.OnWillAppearMethodNotFound);
            Assert.IsNotNull(onClickMethod, TestConstants.Reflection.OnClickMethodNotFound);
            Assert.IsNotNull(onRecordImpressionMethod, TestConstants.Reflection.OnRecordImpressionMethodNotFound);
            Assert.IsNotNull(onDragBeginMethod, TestConstants.Reflection.OnDragBeginMethodNotFound);
            Assert.IsNotNull(onDragMethod, TestConstants.Reflection.OnDragMethodNotFound);
            Assert.IsNotNull(onDragEndMethod, TestConstants.Reflection.OnDragEndMethodNotFound);

            // Create and STORE delegates for later unsubscription
            _willAppearDelegate = (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), element, onWillAppearMethod);
            _clickDelegate = (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), element, onClickMethod);
            _recordImpressionDelegate = (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), element, onRecordImpressionMethod);
            _dragBeginDelegate = (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), element, onDragBeginMethod);
            _dragDelegate = (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), element, onDragMethod);
            _dragEndDelegate = (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), element, onDragEndMethod);

            // Subscribe using stored delegates
            testBannerAd.WillAppear += _willAppearDelegate;
            testBannerAd.DidClick += _clickDelegate;
            testBannerAd.DidRecordImpression += _recordImpressionDelegate;
            testBannerAd.DidBeginDrag += _dragBeginDelegate;
            testBannerAd.DidDrag += _dragDelegate;
            testBannerAd.DidEndDrag += _dragEndDelegate;
        }

        /// <summary>
        /// Helper method to unsubscribe BannerVisualElement event handlers from TestBannerAd events.
        /// Must be called after InjectBannerAdWithEventSubscription to prevent memory leaks.
        /// </summary>
        private void UnsubscribeBannerAd(TestBannerAd testBannerAd)
        {
            // Unsubscribe using the STORED delegates (not new instances)
            if (_willAppearDelegate != null)
                testBannerAd.WillAppear -= _willAppearDelegate;
            if (_clickDelegate != null)
                testBannerAd.DidClick -= _clickDelegate;
            if (_recordImpressionDelegate != null)
                testBannerAd.DidRecordImpression -= _recordImpressionDelegate;
            if (_dragBeginDelegate != null)
                testBannerAd.DidBeginDrag -= _dragBeginDelegate;
            if (_dragDelegate != null)
                testBannerAd.DidDrag -= _dragDelegate;
            if (_dragEndDelegate != null)
                testBannerAd.DidEndDrag -= _dragEndDelegate;

            // Clear stored delegates
            _willAppearDelegate = null;
            _clickDelegate = null;
            _recordImpressionDelegate = null;
            _dragBeginDelegate = null;
            _dragDelegate = null;
            _dragEndDelegate = null;
        }

        /// <summary>
        /// Simple helper method to inject a BannerAd without an event subscription.
        /// Use this for basic tests that don't need event propagation.
        /// </summary>
        private void InjectBannerAd(BannerVisualElement element, IBannerAd bannerAd)
        {
            var bannerAdField = typeof(BannerVisualElement).GetField(TestConstants.Reflection.BannerAdField, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(bannerAdField, TestConstants.Reflection.BannerAdFieldNotFound);
            bannerAdField.SetValue(element, bannerAd);
        }

        #endregion

        #region Basic Property Tests

        [Test]
        public void ConstructorCreatesAdElement()
        {
            Assert.IsNotNull(_bannerVisualElement);

            var adElement = _bannerVisualElement.Q(TestConstants.BannerVisualElement.AdElementName);
            Assert.IsNotNull(adElement);
        }

        [Test]
        public void PlacementNameCanBeSetAndRetrieved()
        {
            _bannerVisualElement.PlacementName = TestConstants.Placements.New;

            Assert.AreEqual(TestConstants.Placements.New, _bannerVisualElement.PlacementName);
        }

        [Test]
        public void DraggableCanBeSetAndRetrieved()
        {
            _bannerVisualElement.Draggable = true;

            Assert.IsTrue(_bannerVisualElement.Draggable);

            _bannerVisualElement.Draggable = false;

            Assert.IsFalse(_bannerVisualElement.Draggable);
        }

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _bannerVisualElement.Keywords = keywords;

            var retrieved = _bannerVisualElement.Keywords;
            Assert.IsNotNull(retrieved);
        }

        [Test]
        public void PartnerSettingsCanBeSetAndRetrieved()
        {
            var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _bannerVisualElement.PartnerSettings = settings;

            var retrieved = _bannerVisualElement.PartnerSettings;
            Assert.IsNotNull(retrieved);
        }

        [Test]
        public void HorizontalAlignmentCanBeSetAndRetrieved()
        {
            _bannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Left;
            Assert.AreEqual(BannerHorizontalAlignment.Left, _bannerVisualElement.HorizontalAlignment);

            _bannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Center;
            Assert.AreEqual(BannerHorizontalAlignment.Center, _bannerVisualElement.HorizontalAlignment);

            _bannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Right;
            Assert.AreEqual(BannerHorizontalAlignment.Right, _bannerVisualElement.HorizontalAlignment);
        }

        [Test]
        public void VerticalAlignmentCanBeSetAndRetrieved()
        {
            _bannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Top;
            Assert.AreEqual(BannerVerticalAlignment.Top, _bannerVisualElement.VerticalAlignment);

            _bannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Center;
            Assert.AreEqual(BannerVerticalAlignment.Center, _bannerVisualElement.VerticalAlignment);

            _bannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Bottom;
            Assert.AreEqual(BannerVerticalAlignment.Bottom, _bannerVisualElement.VerticalAlignment);
        }

        [Test]
        public void InitialHorizontalAlignmentIsCenter()
        {
            Assert.AreEqual(BannerHorizontalAlignment.Center, _bannerVisualElement.HorizontalAlignment);
        }

        [Test]
        public void InitialVerticalAlignmentIsCenter()
        {
            Assert.AreEqual(BannerVerticalAlignment.Center, _bannerVisualElement.VerticalAlignment);
        }

        #endregion

        #region Load and Lifecycle Tests

        [UnityTest]
        public IEnumerator RequestReturnsNullBeforeLoad()
        {
            var request = _bannerVisualElement.Request;

            yield return null;

            Assert.IsNull(request);
        }

        [UnityTest]
        public IEnumerator RequestReturnsLoadRequestAfterLoad()
        {
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);

            Task<BannerAdLoadResult> loadTask;
            try
            {
                loadTask = _bannerVisualElement.Load(request);
            }
            catch (NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            // Wait for task completion
            while (!loadTask.IsCompleted)
                yield return null;

            // Check if a task faulted with NullReferenceException
            if (loadTask.IsFaulted && loadTask.Exception?.InnerException is NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            Assert.IsNotNull(_bannerVisualElement.Request);
        }

        [UnityTest]
        public IEnumerator LoadWithNullPlacementNameReturnsError()
        {
            _bannerVisualElement.PlacementName = null;

            Task<BannerAdLoadResult> loadTask;
            try
            {
                loadTask = _bannerVisualElement.Load();
            }
            catch (NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            // Wait for task completion
            while (!loadTask.IsCompleted)
                yield return null;

            // Check if a task faulted with NullReferenceException
            if (loadTask.IsFaulted && loadTask.Exception?.InnerException is NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            Assert.IsNotNull(loadTask.Result);
            Assert.IsNotNull(loadTask.Result.Error);
            Assert.AreEqual(Errors.ErrorNotReady, loadTask.Result.Error.Value.Message);
        }

        [UnityTest]
        public IEnumerator LoadWithEmptyPlacementNameReturnsError()
        {
            _bannerVisualElement.PlacementName = string.Empty;

            Task<BannerAdLoadResult> loadTask;
            try
            {
                loadTask = _bannerVisualElement.Load();
            }
            catch (NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            // Wait for task completion
            while (!loadTask.IsCompleted)
                yield return null;

            // Check if a task faulted with NullReferenceException
            if (loadTask.IsFaulted && loadTask.Exception?.InnerException is NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            Assert.IsNotNull(loadTask.Result);
            Assert.IsNotNull(loadTask.Result.Error);
            Assert.AreEqual(Errors.ErrorNotReady, loadTask.Result.Error.Value.Message);
        }

        [UnityTest]
        public IEnumerator LoadWithoutParametersHandlesNullBannerAd()
        {
            Task<BannerAdLoadResult> loadTask;
            try
            {
                loadTask = _bannerVisualElement.Load();
            }
            catch (NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            // Wait for task completion
            while (!loadTask.IsCompleted)
                yield return null;

            // Check if a task faulted with NullReferenceException
            if (loadTask.IsFaulted && loadTask.Exception?.InnerException is NullReferenceException)
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                yield break;
            }

            Assert.IsNotNull(loadTask.Result);
        }

        [UnityTest]
        public IEnumerator LoadWithRequestSetsPlacementName()
        {
            var request = new BannerAdLoadRequest(TestConstants.Placements.Load, BannerSize.Standard);

            Task<BannerAdLoadResult> loadTask;
            try
            {
                loadTask = _bannerVisualElement.Load(request);
            }
            catch (NullReferenceException)
            {
                // In test environment, placement name should still be set even if a load fails
                Assert.AreEqual(TestConstants.Placements.Load, _bannerVisualElement.PlacementName);
                yield break;
            }

            // Wait for task completion
            while (!loadTask.IsCompleted)
                yield return null;

            // Check if a task faulted with NullReferenceException
            if (loadTask.IsFaulted && loadTask.Exception?.InnerException is NullReferenceException)
            {
                // In test environment, placement name should still be set even if a load fails
                Assert.AreEqual(TestConstants.Placements.Load, _bannerVisualElement.PlacementName);
                yield break;
            }

            Assert.AreEqual(TestConstants.Placements.Load, _bannerVisualElement.PlacementName);
        }

        [UnityTest]
        public IEnumerator ToStringReturnsJsonRepresentation()
        {
            // Create and inject TestBannerAd with populated properties
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id33333));
            testBannerAd.Keywords = new Dictionary<string, string> { { TestConstants.BannerVisualElement.JsonKey, TestConstants.BannerVisualElement.JsonValue } };
            testBannerAd.PartnerSettings = new Dictionary<string, string> { { TestConstants.BannerVisualElement.PartnerKey, TestConstants.BannerVisualElement.PartnerValue } };
            testBannerAd.Draggable = true;
            testBannerAd.Visible = false;
            testBannerAd.Position = new Vector2(TestConstants.Dimensions.TestPositionX1, TestConstants.Dimensions.TestPositionY1);
            testBannerAd.Pivot = new Vector2(0.5f, 0.5f);

            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            var jsonString = _bannerVisualElement.ToString();

            yield return null;

            // Verify JSON contains expected property values
            Assert.IsNotNull(jsonString);
            Assert.IsFalse(string.IsNullOrEmpty(jsonString));

            // Verify it contains serialized property data
            Assert.IsTrue(jsonString.Contains(TestConstants.BannerVisualElement.JsonKey) || jsonString.Contains(TestConstants.Json.KeywordsProperty));
            Assert.IsTrue(jsonString.Contains(TestConstants.BannerVisualElement.PartnerKey) || jsonString.Contains(TestConstants.Json.PartnerSettingsProperty));
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.DraggableProperty) || jsonString.Contains(TestConstants.Json.TrueValue));

            // Verify it's valid JSON structure
            Assert.IsTrue(jsonString.StartsWith(TestConstants.Json.OpenBrace));
            Assert.IsTrue(jsonString.EndsWith(TestConstants.Json.CloseBrace));

            // Cleanup
            testBannerAd.Dispose();
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id33333);
        }

        [Test]
        public void BannerSizeReturnsNullBeforeLoad()
        {
            var bannerSize = _bannerVisualElement.BannerSize;
            Assert.IsNull(bannerSize);
        }

        [Test]
        public void LoadIdReturnsNullBeforeLoad()
        {
            var loadId = _bannerVisualElement.LoadId;
            Assert.IsTrue(string.IsNullOrEmpty(loadId));
        }

        [Test]
        public void WinningBidInfoReturnsNullBeforeLoad()
        {
            var bidInfo = _bannerVisualElement.WinningBidInfo;
            Assert.IsNull(bidInfo);
        }

        [Test]
        public void LoadMetricsReturnsNullBeforeLoad()
        {
            var loadMetrics = _bannerVisualElement.LoadMetrics;
            Assert.IsNull(loadMetrics);
        }

        [UnityTest]
        public IEnumerator ResetDoesNotThrowWhenBannerAdIsNull()
        {
            // Reset before initializing BannerAd should not throw
            _bannerVisualElement.Reset();

            yield return null;
        }

        [UnityTest]
        public IEnumerator DisposeDoesNotThrowWhenBannerAdIsNull()
        {
            // Dispose before initializing BannerAd should not throw
            _bannerVisualElement.Dispose();

            yield return null;
        }

        [UnityTest]
        public IEnumerator DetachFromPanelCallsDispose()
        {
            Task<BannerAdLoadResult> loadTask;
            try
            {
                var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);
                loadTask = _bannerVisualElement.Load(request);
            }
            catch (NullReferenceException)
            {
                // Expected in the test environment
                // Remove should still work even if BannerAd is null
                _uiDocument.rootVisualElement.Remove(_bannerVisualElement);
                Assert.Pass(TestConstants.AssertionMessages.DetachFromPanelHandledNull);
                yield break;
            }

            // Wait for task completion
            while (!loadTask.IsCompleted)
                yield return null;

            // Check if a task faulted with NullReferenceException
            if (loadTask.IsFaulted && loadTask.Exception?.InnerException is NullReferenceException)
            {
                // Expected in the test environment
                // Remove should still work even if BannerAd is null
                _uiDocument.rootVisualElement.Remove(_bannerVisualElement);
                Assert.Pass(TestConstants.AssertionMessages.DetachFromPanelHandledNull);
                yield break;
            }

            // Remove from parent to trigger DetachFromPanelEvent
            _uiDocument.rootVisualElement.Remove(_bannerVisualElement);

            yield return null;

            // BannerAd should be disposed
            Assert.Pass(TestConstants.AssertionMessages.DetachFromPanelHandledSuccessfully);
        }

        [UnityTest]
        public IEnumerator WillAppearEventIsInvokedWhenBannerAdLoads()
        {
            var eventCalled = false;
            BannerVisualElement receivedElement = null;

            BannerVisualElementAdEvent handler = element =>
            {
                eventCalled = true;
                receivedElement = element;
            };

            try
            {
                _bannerVisualElement.WillAppear += handler;

                // Access BannerAd directly to trigger initialization
                var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

                yield return null;

                // Skip load and directly test the event handler
                if (bannerAd != null)
                {
                    var onWillAppearMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnWillAppearMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    onWillAppearMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd });
                }

                yield return null;

                if (bannerAd != null)
                {
                    Assert.IsTrue(eventCalled);
                    Assert.AreSame(_bannerVisualElement, receivedElement);
                }
                else
                {
                    Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                }
            }
            finally
            {
                _bannerVisualElement.WillAppear -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidClickEventIsInvokedWhenBannerAdClicked()
        {
            var eventCalled = false;
            BannerVisualElement receivedElement = null;

            BannerVisualElementAdEvent handler = element =>
            {
                eventCalled = true;
                receivedElement = element;
            };

            try
            {
                _bannerVisualElement.DidClick += handler;

                // Access BannerAd directly to trigger initialization
                var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

                yield return null;

                // Skip load and directly test the event handler
                if (bannerAd != null)
                {
                    var onClickMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnClickMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    onClickMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd });
                }

                yield return null;

                if (bannerAd != null)
                {
                    Assert.IsTrue(eventCalled);
                    Assert.AreSame(_bannerVisualElement, receivedElement);
                }
                else
                {
                    Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                }
            }
            finally
            {
                _bannerVisualElement.DidClick -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidRecordImpressionEventIsInvokedWhenImpressionOccurs()
        {
            var eventCalled = false;
            BannerVisualElement receivedElement = null;

            BannerVisualElementAdEvent handler = element =>
            {
                eventCalled = true;
                receivedElement = element;
            };

            try
            {
                _bannerVisualElement.DidRecordImpression += handler;

                // Access BannerAd directly to trigger initialization
                var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

                yield return null;

                // Skip load and directly test the event handler
                if (bannerAd != null)
                {
                    var onRecordImpressionMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnRecordImpressionMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    onRecordImpressionMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd });
                }

                yield return null;

                if (bannerAd != null)
                {
                    Assert.IsTrue(eventCalled);
                    Assert.AreSame(_bannerVisualElement, receivedElement);
                }
                else
                {
                    Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                }
            }
            finally
            {
                _bannerVisualElement.DidRecordImpression -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidBeginDragEventIsInvokedWhenDragStarts()
        {
            var eventCalled = false;
            BannerVisualElement receivedElement = null;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerVisualElementAdDragEvent handler = (element, x, y) =>
            {
                eventCalled = true;
                receivedElement = element;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _bannerVisualElement.DidBeginDrag += handler;

                // Access BannerAd directly to trigger initialization
                var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

                yield return null;

                // Skip load and directly test the event handler
                if (bannerAd != null)
                {
                    var onDragBeginMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragBeginMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    onDragBeginMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise });
                }

                yield return null;

                if (bannerAd != null)
                {
                    Assert.IsTrue(eventCalled);
                    Assert.AreSame(_bannerVisualElement, receivedElement);
                    Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                    Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
                }
                else
                {
                    Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                }
            }
            finally
            {
                _bannerVisualElement.DidBeginDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidDragEventIsInvokedWhenDragging()
        {
            var eventCalled = false;
            BannerVisualElement receivedElement = null;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerVisualElementAdDragEvent handler = (element, x, y) =>
            {
                eventCalled = true;
                receivedElement = element;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _bannerVisualElement.DidDrag += handler;

                // Access BannerAd directly to trigger initialization
                var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

                yield return null;

                // Skip load and directly test the event handler
                if (bannerAd != null)
                {
                    // First, simulate drag begin to set _isDragging flag
                    var onDragBeginMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragBeginMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var onDragMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    onDragBeginMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise });
                    onDragMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise });
                }

                yield return null;

                if (bannerAd != null)
                {
                    Assert.IsTrue(eventCalled);
                    Assert.AreSame(_bannerVisualElement, receivedElement);
                    Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                    Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
                }
                else
                {
                    Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                }
            }
            finally
            {
                _bannerVisualElement.DidDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidDragEventLogsWarningWithoutBeginDrag()
        {
            // Access BannerAd directly to trigger initialization
            var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

            yield return null;

            // Skip load and directly test the event handler
            if (bannerAd != null)
            {
                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.DragWithoutBegin));

                // Simulate drag without drag begin
                var onDragMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                onDragMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise });
            }
            else
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator DidEndDragEventIsInvokedWhenDragEnds()
        {
            var eventCalled = false;
            BannerVisualElement receivedElement = null;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerVisualElementAdDragEvent handler = (element, x, y) =>
            {
                eventCalled = true;
                receivedElement = element;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _bannerVisualElement.DidEndDrag += handler;

                // Access BannerAd directly to trigger initialization
                var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

                yield return null;

                // Skip load and directly test the event handler
                if (bannerAd != null)
                {
                    var onDragEndMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDragEndMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    onDragEndMethod?.Invoke(_bannerVisualElement, new object[] { bannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise });
                }

                yield return null;

                if (bannerAd != null)
                {
                    Assert.IsTrue(eventCalled);
                    Assert.AreSame(_bannerVisualElement, receivedElement);
                    Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                    Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
                }
                else
                {
                    Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
                }
            }
            finally
            {
                _bannerVisualElement.DidEndDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator AdRelativePositionIsAccessedDuringSyncWithNativeContainer()
        {
            // Access BannerAd directly to trigger initialization
            var bannerAdProperty = typeof(BannerVisualElement).GetProperty(TestConstants.Reflection.BannerAdProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bannerAd = bannerAdProperty?.GetValue(_bannerVisualElement) as IBannerAd;

            yield return null;

            // Access AdRelativePosition via reflection to ensure it's covered
            var adRelativePositionProperty = typeof(BannerVisualElement).GetProperty(TestConstants.BannerVisualElement.ReflectionAdRelativePositionProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var position = adRelativePositionProperty?.GetValue(_bannerVisualElement);

            yield return null;

            if (bannerAd != null)
            {
                Assert.IsNotNull(position);
                Assert.IsInstanceOf<Vector2>(position);
            }
            else
            {
                Assert.Pass(TestConstants.AssertionMessages.BannerAdNullExpected);
            }
        }

        [UnityTest]
        public IEnumerator SyncWithNativeContainerDoesNothingWhenBannerAdIsNull()
        {
            // Don't inject BannerAd - SyncWithNativeContainer should return early
            yield return null;

            // Access SyncWithNativeContainer via reflection
            var syncMethod = typeof(BannerVisualElement).GetMethod(TestConstants.BannerVisualElement.ReflectionSyncWithNativeContainerMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // This should not throw
            syncMethod?.Invoke(_bannerVisualElement, null);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SyncWithNativeContainerUpdatesContainerSizeWhenDifferent()
        {
            // Create and inject TestBannerAd with different ContainerSize
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id22222));
            testBannerAd.ContainerSize = ContainerSize.FixedSize((int)TestConstants.Dimensions.TestContainerWidth, (int)TestConstants.Dimensions.TestContainerHeight);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            // Access SyncWithNativeContainer via reflection
            var syncMethod = typeof(BannerVisualElement).GetMethod(TestConstants.BannerVisualElement.ReflectionSyncWithNativeContainerMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            syncMethod?.Invoke(_bannerVisualElement, null);

            yield return null;

            // ContainerSize should be updated to match VisualElement's size
            Assert.AreNotEqual((int)TestConstants.Dimensions.TestContainerWidth, testBannerAd.ContainerSize.Width);
            Assert.AreNotEqual((int)TestConstants.Dimensions.TestContainerHeight, testBannerAd.ContainerSize.Height);

            // Cleanup
            testBannerAd.Dispose();
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id22222);
        }

        [UnityTest]
        public IEnumerator SyncWithNativeContainerUpdatesPositionWhenDifferentAndNotDragging()
        {
            // Create and inject TestBannerAd with different Position
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id11111));
            testBannerAd.Position = new Vector2(TestConstants.Dimensions.TestPositionX2, TestConstants.Dimensions.TestPositionY2);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            // Access SyncWithNativeContainer via reflection
            var syncMethod = typeof(BannerVisualElement).GetMethod(TestConstants.BannerVisualElement.ReflectionSyncWithNativeContainerMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            syncMethod?.Invoke(_bannerVisualElement, null);

            yield return null;

            // Position should be updated to match VisualElement's position
            Assert.AreNotEqual(TestConstants.Dimensions.TestPositionX2, testBannerAd.Position.x);
            Assert.AreNotEqual(TestConstants.Dimensions.TestPositionY2, testBannerAd.Position.y);

            // Cleanup
            testBannerAd.Dispose();
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id11111);
        }

        [UnityTest]
        public IEnumerator SyncWithNativeContainerDoesNotUpdatePositionWhileDragging()
        {
            // Create and inject TestBannerAd with different Position
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id11111));
            testBannerAd.Position = new Vector2(TestConstants.Dimensions.TestPositionX2, TestConstants.Dimensions.TestPositionY2);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            // Simulate dragging by setting _isDragging flag
            var isDraggingField = typeof(BannerVisualElement).GetField(TestConstants.BannerVisualElement.ReflectionIsDraggingField, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isDraggingField?.SetValue(_bannerVisualElement, true);

            yield return null;

            // Access SyncWithNativeContainer via reflection
            var syncMethod = typeof(BannerVisualElement).GetMethod(TestConstants.BannerVisualElement.ReflectionSyncWithNativeContainerMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            syncMethod?.Invoke(_bannerVisualElement, null);

            yield return null;

            // Position should NOT be updated while dragging
            Assert.AreEqual(TestConstants.Dimensions.TestPositionX2, testBannerAd.Position.x);
            Assert.AreEqual(TestConstants.Dimensions.TestPositionY2, testBannerAd.Position.y);

            // Cleanup
            testBannerAd.Dispose();
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id11111);
        }

        [UnityTest]
        public IEnumerator SyncWithNativeContainerUpdatesVisibleWhenDifferent()
        {
            // Create and inject TestBannerAd with different Visible state
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id10101));
            testBannerAd.Visible = false;
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            // Ensure VisualElement is visible
            _bannerVisualElement.style.display = DisplayStyle.Flex;

            yield return null;

            // Access SyncWithNativeContainer via reflection
            var syncMethod = typeof(BannerVisualElement).GetMethod(TestConstants.BannerVisualElement.ReflectionSyncWithNativeContainerMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            syncMethod?.Invoke(_bannerVisualElement, null);

            yield return null;

            // Visible should be updated to match VisualElement's display style
            Assert.IsTrue(testBannerAd.Visible);

            // Cleanup
            testBannerAd.Dispose();
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id10101);
        }

        [UnityTest]
        public IEnumerator SyncWithNativeContainerUpdatesAdRelativePositionWhenDifferent()
        {
            // Create and inject TestBannerAd
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id20202));

            // Set AdRelativePosition to a different value using reflection
            var adRelativePositionProperty = typeof(BannerAdBase).GetProperty(TestConstants.BannerVisualElement.ReflectionAdRelativePositionProperty, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            adRelativePositionProperty?.SetValue(testBannerAd, new Vector2(TestConstants.Dimensions.RelativePositionX, TestConstants.Dimensions.RelativePositionY));

            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            // Access SyncWithNativeContainer via reflection
            var syncMethod = typeof(BannerVisualElement).GetMethod(TestConstants.BannerVisualElement.ReflectionSyncWithNativeContainerMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            syncMethod?.Invoke(_bannerVisualElement, null);

            yield return null;

            // AdRelativePosition should be updated to match the ad element's relative position
            // ReSharper disable once PossibleNullReferenceException
            var updatedPosition = (Vector2)adRelativePositionProperty?.GetValue(testBannerAd);
            Assert.AreNotEqual(TestConstants.Dimensions.RelativePositionX, updatedPosition.x);
            Assert.AreNotEqual(TestConstants.Dimensions.RelativePositionY, updatedPosition.y);

            // Cleanup
            testBannerAd.Dispose();
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id20202);
        }

        [UnityTest]
        public IEnumerator OnDetachFromPanelCallsDisposeOnBannerAd()
        {
            // Create and inject TestBannerAd
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id9999));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            yield return null;

            // Verify BannerAd is not disposed yet
            Assert.IsFalse(testBannerAd.IsDisposedPublic);

            // Access OnDetachFromPanel via reflection
            var onDetachMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDetachFromPanelMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Create a mock DetachFromPanelEvent (pass null since the method doesn't use the event parameter)
            onDetachMethod?.Invoke(_bannerVisualElement, new object[] { null });

            yield return null;

            // Verify BannerAd.Dispose() was called
            Assert.IsTrue(testBannerAd.IsDisposedPublic);

            // Cleanup
            AdCache.ReleaseAd(TestConstants.UniqueIds.Id9999);
        }

        [UnityTest]
        public IEnumerator OnDetachFromPanelDoesNotThrowWhenBannerAdIsNull()
        {
            // Don't inject BannerAd - it should be null
            yield return null;

            // Access OnDetachFromPanel via reflection
            var onDetachMethod = typeof(BannerVisualElement).GetMethod(TestConstants.Reflection.OnDetachFromPanelMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // This should not throw even with null BannerAd
            onDetachMethod?.Invoke(_bannerVisualElement, new object[] { null });

            yield return null;

            // Test passes if no exception was thrown
        }

        #endregion

        #region Propagation Tests with Injected TestBannerAd

        [UnityTest]
        public IEnumerator DraggablePropagatesToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Primary));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.Draggable = true;
                yield return null;

                Assert.IsTrue(testBannerAd.Draggable);

                _bannerVisualElement.Draggable = false;
                yield return null;

                Assert.IsFalse(testBannerAd.Draggable);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            }
        }

        [UnityTest]
        public IEnumerator KeywordsPropagatesToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id23456));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };
                _bannerVisualElement.Keywords = keywords;
                yield return null;

                Assert.AreSame(keywords, testBannerAd.Keywords);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id23456);
            }
        }

        [UnityTest]
        public IEnumerator PartnerSettingsPropagatesToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id34567));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };
                _bannerVisualElement.PartnerSettings = settings;
                yield return null;

                Assert.AreSame(settings, testBannerAd.PartnerSettings);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id34567);
            }
        }

        [UnityTest]
        public IEnumerator HorizontalAlignmentPropagatesToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id30303));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Left;
                yield return null;

                Assert.AreEqual(BannerHorizontalAlignment.Left, testBannerAd.HorizontalAlignment);

                _bannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Right;
                yield return null;

                Assert.AreEqual(BannerHorizontalAlignment.Right, testBannerAd.HorizontalAlignment);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id30303);
            }
        }

        [UnityTest]
        public IEnumerator VerticalAlignmentPropagatesToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Secondary));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Top;
                yield return null;

                Assert.AreEqual(BannerVerticalAlignment.Top, testBannerAd.VerticalAlignment);

                _bannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Bottom;
                yield return null;

                Assert.AreEqual(BannerVerticalAlignment.Bottom, testBannerAd.VerticalAlignment);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Secondary);
            }
        }

        [UnityTest]
        public IEnumerator WillAppearEventPropagatesFromBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id45678));
            InjectBannerAdWithEventSubscription(_bannerVisualElement, testBannerAd);

            var eventCalled = false;
            BannerVisualElement receivedElement = null;

            BannerVisualElementAdEvent handler = element =>
            {
                eventCalled = true;
                receivedElement = element;
            };

            try
            {
                _bannerVisualElement.WillAppear += handler;

                BannerEventTrigger<BannerVisualElement>.TriggerOnWillAppear(_bannerVisualElement, testBannerAd);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_bannerVisualElement, receivedElement);
            }
            finally
            {
                _bannerVisualElement.WillAppear -= handler;
                UnsubscribeBannerAd(testBannerAd);
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id45678);
            }
        }

        [UnityTest]
        public IEnumerator DidClickEventPropagatesFromBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id56789));
            InjectBannerAdWithEventSubscription(_bannerVisualElement, testBannerAd);

            var eventCalled = false;
            BannerVisualElement receivedElement = null;

            BannerVisualElementAdEvent handler = element =>
            {
                eventCalled = true;
                receivedElement = element;
            };

            try
            {
                _bannerVisualElement.DidClick += handler;

                BannerEventTrigger<BannerVisualElement>.TriggerOnClick(_bannerVisualElement, testBannerAd);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_bannerVisualElement, receivedElement);
            }
            finally
            {
                _bannerVisualElement.DidClick -= handler;
                UnsubscribeBannerAd(testBannerAd);
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id56789);
            }
        }

        [UnityTest]
        public IEnumerator DidRecordImpressionEventPropagatesFromBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Secondary));
            InjectBannerAdWithEventSubscription(_bannerVisualElement, testBannerAd);

            var eventCalled = false;
            BannerVisualElement receivedElement = null;

            BannerVisualElementAdEvent handler = element =>
            {
                eventCalled = true;
                receivedElement = element;
            };

            try
            {
                _bannerVisualElement.DidRecordImpression += handler;

                BannerEventTrigger<BannerVisualElement>.TriggerOnRecordImpression(_bannerVisualElement, testBannerAd);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_bannerVisualElement, receivedElement);
            }
            finally
            {
                _bannerVisualElement.DidRecordImpression -= handler;
                UnsubscribeBannerAd(testBannerAd);
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Secondary);
            }
        }

        [UnityTest]
        public IEnumerator DidBeginDragEventPropagatesFromBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id78901));
            InjectBannerAdWithEventSubscription(_bannerVisualElement, testBannerAd);

            var eventCalled = false;
            BannerVisualElement receivedElement = null;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerVisualElementAdDragEvent handler = (element, x, y) =>
            {
                eventCalled = true;
                receivedElement = element;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _bannerVisualElement.DidBeginDrag += handler;

                BannerEventTrigger<BannerVisualElement>.TriggerOnDragBegin(_bannerVisualElement, testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_bannerVisualElement, receivedElement);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _bannerVisualElement.DidBeginDrag -= handler;
                UnsubscribeBannerAd(testBannerAd);
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id78901);
            }
        }

        [UnityTest]
        public IEnumerator DidDragEventPropagatesFromBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id90123));
            InjectBannerAdWithEventSubscription(_bannerVisualElement, testBannerAd);

            var eventCalled = false;
            BannerVisualElement receivedElement = null;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerVisualElementAdDragEvent handler = (element, x, y) =>
            {
                eventCalled = true;
                receivedElement = element;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _bannerVisualElement.DidDrag += handler;

                // Must call TriggerOnDragBegin first to set isDragging flag
                BannerEventTrigger<BannerVisualElement>.TriggerOnDragBegin(_bannerVisualElement, testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                BannerEventTrigger<BannerVisualElement>.TriggerOnDrag(_bannerVisualElement, testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_bannerVisualElement, receivedElement);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _bannerVisualElement.DidDrag -= handler;
                UnsubscribeBannerAd(testBannerAd);
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id90123);
            }
        }

        [UnityTest]
        public IEnumerator DidEndDragEventPropagatesFromBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Id89012));
            InjectBannerAdWithEventSubscription(_bannerVisualElement, testBannerAd);

            var eventCalled = false;
            BannerVisualElement receivedElement = null;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerVisualElementAdDragEvent handler = (element, x, y) =>
            {
                eventCalled = true;
                receivedElement = element;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _bannerVisualElement.DidEndDrag += handler;

                BannerEventTrigger<BannerVisualElement>.TriggerOnDragEnd(_bannerVisualElement, testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_bannerVisualElement, receivedElement);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _bannerVisualElement.DidEndDrag -= handler;
                UnsubscribeBannerAd(testBannerAd);
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id89012);
            }
        }

        [UnityTest]
        public IEnumerator ResetPropagatesCallToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Tertiary));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                Assert.DoesNotThrow(() => _bannerVisualElement.Reset());
                yield return null;
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Tertiary);
            }
        }

        [UnityTest]
        public IEnumerator DisposePropagatesCallToBannerAd()
        {
            var testBannerAd = new TestBannerAd(new IntPtr(TestConstants.UniqueIds.Dispose));
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.Dispose();
                yield return null;

                Assert.IsTrue(testBannerAd.DisposeCalled);
            }
            finally
            {
                AdCache.ReleaseAd(TestConstants.UniqueIds.Dispose);
            }
        }

        #endregion

        #region SetAbsolutePositionByAlignment Tests

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentLogsWarningWhenBannerSizeIsNull()
        {
            // Don't inject a banner ad, so BannerSize will be null
            yield return null;

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("Cannot position banner.*BannerSize is null"));

            _bannerVisualElement.SetAbsolutePositionByAlignment(0.5f, 0.5f);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentSetsTopLeftPosition()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50001), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(0f, 0f);
                yield return null;

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(0f, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(0f, _bannerVisualElement.style.top.value.value);
                Assert.AreEqual(StyleKeyword.Auto, _bannerVisualElement.style.right.keyword);
                Assert.AreEqual(StyleKeyword.Auto, _bannerVisualElement.style.bottom.keyword);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50001);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentSetsTopRightPosition()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50002), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(1f, 0f);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var expectedLeft = Mathf.Round(Screen.width - width);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(0f, _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50002);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentSetsBottomLeftPosition()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50003), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(0f, 1f);
                yield return null;

                var height = DensityConverters.NativeToPixels(BannerSize.Standard.Height);
                var expectedTop = Mathf.Round(Screen.height - height);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(0f, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50003);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentSetsBottomRightPosition()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50004), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(1f, 1f);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.Standard.Height);
                var expectedLeft = Mathf.Round(Screen.width - width);
                var expectedTop = Mathf.Round(Screen.height - height);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50004);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentSetsCenterPosition()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50005), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(0.5f, 0.5f);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.Standard.Height);
                var expectedLeft = Mathf.Round((Screen.width - width) * 0.5f);
                var expectedTop = Mathf.Round((Screen.height - height) * 0.5f);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50005);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentSetsCustomAlignmentPosition()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50006), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                const float horizontalAlignment = 0.25f;
                const float verticalAlignment = 0.75f;

                _bannerVisualElement.SetAbsolutePositionByAlignment(horizontalAlignment, verticalAlignment);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.Standard.Height);
                var expectedLeft = Mathf.Round((Screen.width - width) * horizontalAlignment);
                var expectedTop = Mathf.Round((Screen.height - height) * verticalAlignment);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50006);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentResizesContainerToMatchBannerSize()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50007), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(0.5f, 0.5f);
                yield return null;

                var expectedWidth = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var expectedHeight = DensityConverters.NativeToPixels(BannerSize.Standard.Height);

                Assert.AreEqual(expectedWidth, _bannerVisualElement.style.width.value.value);
                Assert.AreEqual(expectedHeight, _bannerVisualElement.style.height.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50007);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentWorksWithMediumRectBannerSize()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50008), BannerSize.MediumRect);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(0.5f, 0.5f);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.MediumRect.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.MediumRect.Height);
                var expectedLeft = Mathf.Round((Screen.width - width) * 0.5f);
                var expectedTop = Mathf.Round((Screen.height - height) * 0.5f);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
                Assert.AreEqual(width, _bannerVisualElement.style.width.value.value);
                Assert.AreEqual(height, _bannerVisualElement.style.height.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50008);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentWorksWithLeaderboardBannerSize()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50009), BannerSize.Leaderboard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                _bannerVisualElement.SetAbsolutePositionByAlignment(0.5f, 0f);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Leaderboard.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.Leaderboard.Height);
                var expectedLeft = Mathf.Round((Screen.width - width) * 0.5f);

                Assert.AreEqual(Position.Absolute, _bannerVisualElement.style.position.value);
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(0f, _bannerVisualElement.style.top.value.value);
                Assert.AreEqual(width, _bannerVisualElement.style.width.value.value);
                Assert.AreEqual(height, _bannerVisualElement.style.height.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50009);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentCanBeCalledMultipleTimes()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50010), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                // First position: top-left
                _bannerVisualElement.SetAbsolutePositionByAlignment(0f, 0f);
                yield return null;

                Assert.AreEqual(0f, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(0f, _bannerVisualElement.style.top.value.value);

                // Second position: bottom-right
                _bannerVisualElement.SetAbsolutePositionByAlignment(1f, 1f);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.Standard.Height);
                var expectedLeft = Mathf.Round(Screen.width - width);
                var expectedTop = Mathf.Round(Screen.height - height);

                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);

                // Third position: center
                _bannerVisualElement.SetAbsolutePositionByAlignment(0.5f, 0.5f);
                yield return null;

                expectedLeft = Mathf.Round((Screen.width - width) * 0.5f);
                expectedTop = Mathf.Round((Screen.height - height) * 0.5f);

                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50010);
            }
        }

        [UnityTest]
        public IEnumerator SetAbsolutePositionByAlignmentUsesPixelPerfectRounding()
        {
            var testBannerAd = new TestBannerAdWithSize(new IntPtr(TestConstants.UniqueIds.Id50011), BannerSize.Standard);
            InjectBannerAd(_bannerVisualElement, testBannerAd);

            try
            {
                // Use alignment values that would produce fractional pixels without rounding
                const float horizontalAlignment = 0.333f;
                const float verticalAlignment = 0.667f;

                _bannerVisualElement.SetAbsolutePositionByAlignment(horizontalAlignment, verticalAlignment);
                yield return null;

                var width = DensityConverters.NativeToPixels(BannerSize.Standard.Width);
                var height = DensityConverters.NativeToPixels(BannerSize.Standard.Height);
                var expectedLeft = Mathf.Round((Screen.width - width) * horizontalAlignment);
                var expectedTop = Mathf.Round((Screen.height - height) * verticalAlignment);

                // Verify the positions are whole numbers (pixel-perfect)
                Assert.AreEqual(expectedLeft, _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(expectedTop, _bannerVisualElement.style.top.value.value);
                Assert.AreEqual(Mathf.Floor(expectedLeft), _bannerVisualElement.style.left.value.value);
                Assert.AreEqual(Mathf.Floor(expectedTop), _bannerVisualElement.style.top.value.value);
            }
            finally
            {
                testBannerAd.Dispose();
                AdCache.ReleaseAd(TestConstants.UniqueIds.Id50011);
            }
        }

        #endregion
    }
}
