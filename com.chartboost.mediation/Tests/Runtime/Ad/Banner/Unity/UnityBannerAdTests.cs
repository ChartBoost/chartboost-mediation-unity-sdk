using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Tests.Runtime.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner.Unity
{
    public class UnityBannerAdTests
    {
        private GameObject _gameObject;
        private UnityBannerAd _unityBannerAd;
        private TestBannerAd _testBannerAd;
        private IntPtr _testUniqueId;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(TestConstants.Unity.TestUnityBannerAdName);
            _unityBannerAd = _gameObject.AddComponent<UnityBannerAd>();

            _testUniqueId = new IntPtr(TestConstants.UniqueIds.Id11111);
            _testBannerAd = new TestBannerAd(_testUniqueId);
        }

        [TearDown]
        public void TearDown()
        {
            _testBannerAd?.Dispose();
            AdCache.ReleaseAd(_testUniqueId.ToInt64());

            if (_gameObject != null)
                UnityEngine.Object.DestroyImmediate(_gameObject);
        }

        /// <summary>
        /// Helper method to inject a TestBannerAd into UnityBannerAd using reflection.
        /// Also subscribes UnityBannerAd event handlers to the TestBannerAd events.
        /// </summary>
        private void InjectBannerAd(TestBannerAd testBannerAd)
        {
            // Inject the TestBannerAd into the _bannerAd field
            var bannerAdField = typeof(UnityBannerAd).GetField(TestConstants.Reflection.BannerAdField, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(bannerAdField, TestConstants.Reflection.BannerAdFieldNotFound);
            bannerAdField.SetValue(_unityBannerAd, testBannerAd);

            // Subscribe UnityBannerAd's event handlers to TestBannerAd's events
            // These are the private event handler methods in UnityBannerAd
            var onWillAppearMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnWillAppearMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onClickMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnClickMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onRecordImpressionMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnRecordImpressionMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onDragBeginMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnDragBeginMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onDragMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnDragMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onDragEndMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnDragEndMethod, BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(onWillAppearMethod, TestConstants.Reflection.OnWillAppearMethodNotFound);
            Assert.IsNotNull(onClickMethod, TestConstants.Reflection.OnClickMethodNotFound);
            Assert.IsNotNull(onRecordImpressionMethod, TestConstants.Reflection.OnRecordImpressionMethodNotFound);
            Assert.IsNotNull(onDragBeginMethod, TestConstants.Reflection.OnDragBeginMethodNotFound);
            Assert.IsNotNull(onDragMethod, TestConstants.Reflection.OnDragMethodNotFound);
            Assert.IsNotNull(onDragEndMethod, TestConstants.Reflection.OnDragEndMethodNotFound);

            // Create delegates and subscribe to TestBannerAd events
            testBannerAd.WillAppear += (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), _unityBannerAd, onWillAppearMethod);
            testBannerAd.DidClick += (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), _unityBannerAd, onClickMethod);
            testBannerAd.DidRecordImpression += (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), _unityBannerAd, onRecordImpressionMethod);
            testBannerAd.DidBeginDrag += (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), _unityBannerAd, onDragBeginMethod);
            testBannerAd.DidDrag += (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), _unityBannerAd, onDragMethod);
            testBannerAd.DidEndDrag += (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), _unityBannerAd, onDragEndMethod);
        }

        /// <summary>
        /// Helper method to unsubscribe UnityBannerAd event handlers from TestBannerAd events.
        /// Must be called after InjectBannerAd to prevent memory leaks.
        /// </summary>
        private void UnsubscribeBannerAd(TestBannerAd testBannerAd)
        {
            // Get the same event handler methods
            var onWillAppearMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnWillAppearMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onClickMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnClickMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onRecordImpressionMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnRecordImpressionMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onDragBeginMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnDragBeginMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onDragMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnDragMethod, BindingFlags.NonPublic | BindingFlags.Instance);
            var onDragEndMethod = typeof(UnityBannerAd).GetMethod(TestConstants.Reflection.OnDragEndMethod, BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(onWillAppearMethod, TestConstants.Reflection.OnWillAppearMethodNotFound);
            Assert.IsNotNull(onClickMethod, TestConstants.Reflection.OnClickMethodNotFound);
            Assert.IsNotNull(onRecordImpressionMethod, TestConstants.Reflection.OnRecordImpressionMethodNotFound);
            Assert.IsNotNull(onDragBeginMethod, TestConstants.Reflection.OnDragBeginMethodNotFound);
            Assert.IsNotNull(onDragMethod, TestConstants.Reflection.OnDragMethodNotFound);
            Assert.IsNotNull(onDragEndMethod, TestConstants.Reflection.OnDragEndMethodNotFound);

            // Unsubscribe the delegates
            testBannerAd.WillAppear -= (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), _unityBannerAd, onWillAppearMethod);
            testBannerAd.DidClick -= (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), _unityBannerAd, onClickMethod);
            testBannerAd.DidRecordImpression -= (BannerAdEvent)Delegate.CreateDelegate(typeof(BannerAdEvent), _unityBannerAd, onRecordImpressionMethod);
            testBannerAd.DidBeginDrag -= (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), _unityBannerAd, onDragBeginMethod);
            testBannerAd.DidDrag -= (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), _unityBannerAd, onDragMethod);
            testBannerAd.DidEndDrag -= (BannerAdDragEvent)Delegate.CreateDelegate(typeof(BannerAdDragEvent), _unityBannerAd, onDragEndMethod);
        }

        [Test]
        public void ComponentIsCreated()
        {
            Assert.IsNotNull(_unityBannerAd);
        }

        [Test]
        public void PlacementNameCanBeSetAndRetrieved()
        {
            _unityBannerAd.PlacementName = TestConstants.Placements.Standard;

            Assert.AreEqual(TestConstants.Placements.Standard, _unityBannerAd.PlacementName);
        }

        [Test]
        public void DraggableCanBeSetAndRetrieved()
        {
            _unityBannerAd.Draggable = true;

            Assert.IsTrue(_unityBannerAd.Draggable);

            _unityBannerAd.Draggable = false;

            Assert.IsFalse(_unityBannerAd.Draggable);
        }

        [Test]
        public void HorizontalAlignmentCanBeSetAndRetrieved()
        {
            _unityBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Left;

            Assert.AreEqual(BannerHorizontalAlignment.Left, _unityBannerAd.HorizontalAlignment);

            _unityBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;

            Assert.AreEqual(BannerHorizontalAlignment.Center, _unityBannerAd.HorizontalAlignment);

            _unityBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;

            Assert.AreEqual(BannerHorizontalAlignment.Right, _unityBannerAd.HorizontalAlignment);
        }

        [Test]
        public void VerticalAlignmentCanBeSetAndRetrieved()
        {
            _unityBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;

            Assert.AreEqual(BannerVerticalAlignment.Top, _unityBannerAd.VerticalAlignment);

            _unityBannerAd.VerticalAlignment = BannerVerticalAlignment.Center;

            Assert.AreEqual(BannerVerticalAlignment.Center, _unityBannerAd.VerticalAlignment);

            _unityBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;

            Assert.AreEqual(BannerVerticalAlignment.Bottom, _unityBannerAd.VerticalAlignment);
        }

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _unityBannerAd.Keywords = keywords;

            var retrieved = _unityBannerAd.Keywords;
            Assert.IsNotNull(retrieved);
        }

        [Test]
        public void PartnerSettingsCanBeSetAndRetrieved()
        {
            var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _unityBannerAd.PartnerSettings = settings;

            var retrieved = _unityBannerAd.PartnerSettings;
            Assert.IsNotNull(retrieved);
        }

        [UnityTest]
        public IEnumerator LoadWithNullPlacementNameReturnsError()
        {
            _unityBannerAd.PlacementName = null;

            var loadTask = _unityBannerAd.Load();
            yield return new WaitUntil(() => loadTask.IsCompleted);

            Assert.IsNotNull(loadTask.Result);
            Assert.IsNotNull(loadTask.Result.Error);
            Assert.AreEqual(Errors.ErrorNotReady, loadTask.Result.Error.Value.Message);
        }

        [UnityTest]
        public IEnumerator LoadWithEmptyPlacementNameReturnsError()
        {
            _unityBannerAd.PlacementName = string.Empty;

            var loadTask = _unityBannerAd.Load();
            yield return new WaitUntil(() => loadTask.IsCompleted);

            Assert.IsNotNull(loadTask.Result);
            Assert.IsNotNull(loadTask.Result.Error);
            Assert.AreEqual(Errors.ErrorNotReady, loadTask.Result.Error.Value.Message);
        }

        [UnityTest]
        public IEnumerator LoadWithValidPlacementNameReturnsResult()
        {
            _unityBannerAd.PlacementName = TestConstants.Placements.Standard;

            var loadTask = _unityBannerAd.Load();
            yield return new WaitUntil(() => loadTask.IsCompleted);

            Assert.IsNotNull(loadTask.Result);
        }

        [UnityTest]
        public IEnumerator LoadWithRequestSetsPlacementName()
        {
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);

            var loadTask = _unityBannerAd.Load(request);
            yield return new WaitUntil(() => loadTask.IsCompleted);

            Assert.AreEqual(TestConstants.Placements.Standard, _unityBannerAd.PlacementName);
        }

        [Test]
        public void RequestReturnsNullBeforeLoad()
        {
            Assert.IsNull(_unityBannerAd.Request);
        }

        [Test]
        public void LoadIdReturnsNullBeforeLoad()
        {
            var loadId = _unityBannerAd.LoadId;
            Assert.IsTrue(string.IsNullOrEmpty(loadId));
        }

        [Test]
        public void LoadMetricsReturnsNullBeforeLoad()
        {
            var metrics = _unityBannerAd.LoadMetrics;
            Assert.IsNull(metrics);
        }

        [Test]
        public void BannerSizeReturnsNullBeforeLoad()
        {
            var bannerSize = _unityBannerAd.BannerSize;
            Assert.IsNull(bannerSize);
        }

        [Test]
        public void WinningBidInfoReturnsNullBeforeLoad()
        {
            var bidInfo = _unityBannerAd.WinningBidInfo;
            Assert.IsNull(bidInfo);
        }

        [Test]
        public void ResetDoesNotThrow()
        {
            Assert.DoesNotThrow(() => _unityBannerAd.Reset());
        }

        [Test]
        public void ToStringReturnsValidJson()
        {
            var jsonString = _unityBannerAd.ToString();

            Assert.IsNotNull(jsonString);
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.OpenBrace));
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.CloseBrace));
        }

        [UnityTest]
        public IEnumerator WillAppearEventIsInvoked()
        {
            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdEvent handler = unityBanner =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.WillAppear += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnWillAppear(_unityBannerAd, _testBannerAd);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
            }
            finally
            {
                _unityBannerAd.WillAppear -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidClickEventIsInvoked()
        {
            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdEvent handler = unityBanner =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.DidClick += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnClick(_unityBannerAd, _testBannerAd);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
            }
            finally
            {
                _unityBannerAd.DidClick -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidRecordImpressionEventIsInvoked()
        {
            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdEvent handler = unityBanner =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.DidRecordImpression += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnRecordImpression(_unityBannerAd, _testBannerAd);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
            }
            finally
            {
                _unityBannerAd.DidRecordImpression -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidBeginDragEventIsInvoked()
        {
            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;
            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (unityBanner, x, y) =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidBeginDrag += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _unityBannerAd.DidBeginDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidDragEventIsInvoked()
        {
            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdDragEvent handler = (unityBanner, _, _) =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.DidDrag += handler;

                // Must call TriggerOnDragBegin first to set _isDragging = true
                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator DidEndDragEventIsInvoked()
        {
            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;
            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (unityBanner, x, y) =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidEndDrag += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _unityBannerAd.DidEndDrag -= handler;
            }
        }

        [Test]
        public void GameObjectHasRectTransformComponent()
        {
            var rectTransform = _gameObject.GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform);
        }

        [UnityTest]
        public IEnumerator OnDestroyDisposesAd()
        {
            _unityBannerAd.PlacementName = TestConstants.Placements.Standard;

            // Trigger BannerAd creation by accessing a property
            var _ = _unityBannerAd.Keywords;

            yield return null;

            // Destroy will be called in TearDown
            // We just verify it doesn't throw
            Assert.DoesNotThrow(() => UnityEngine.Object.DestroyImmediate(_gameObject));
            _gameObject = null; // Prevent double destroy in TearDown
        }

        [Test]
        public void InitialHorizontalAlignmentIsCenter()
        {
            Assert.AreEqual(BannerHorizontalAlignment.Center, _unityBannerAd.HorizontalAlignment);
        }

        [Test]
        public void InitialVerticalAlignmentIsCenter()
        {
            Assert.AreEqual(BannerVerticalAlignment.Center, _unityBannerAd.VerticalAlignment);
        }

        #region Propagation Tests with Injected TestBannerAd

        [Test]
        public void DraggablePropagatesToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                _unityBannerAd.Draggable = true;

                Assert.IsTrue(_testBannerAd.Draggable);

                _unityBannerAd.Draggable = false;

                Assert.IsFalse(_testBannerAd.Draggable);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [Test]
        public void KeywordsPropagatesToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };
                _unityBannerAd.Keywords = keywords;

                Assert.AreSame(keywords, _testBannerAd.Keywords);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [Test]
        public void PartnerSettingsPropagatesToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };
                _unityBannerAd.PartnerSettings = settings;

                Assert.AreSame(settings, _testBannerAd.PartnerSettings);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [Test]
        public void HorizontalAlignmentPropagatesToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                _unityBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Left;

                Assert.AreEqual(BannerHorizontalAlignment.Left, _testBannerAd.HorizontalAlignment);

                _unityBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;

                Assert.AreEqual(BannerHorizontalAlignment.Right, _testBannerAd.HorizontalAlignment);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [Test]
        public void VerticalAlignmentPropagatesToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                _unityBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;

                Assert.AreEqual(BannerVerticalAlignment.Top, _testBannerAd.VerticalAlignment);

                _unityBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;

                Assert.AreEqual(BannerVerticalAlignment.Bottom, _testBannerAd.VerticalAlignment);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator WillAppearEventPropagatesFromBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdEvent handler = unityBanner =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.WillAppear += handler;

                _testBannerAd.TestOnWillAppear();

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
            }
            finally
            {
                _unityBannerAd.WillAppear -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator DidClickEventPropagatesFromBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdEvent handler = unityBanner =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.DidClick += handler;

                _testBannerAd.TestOnClick();

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
            }
            finally
            {
                _unityBannerAd.DidClick -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator DidRecordImpressionEventPropagatesFromBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;

            UnityBannerAdEvent handler = unityBanner =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
            };

            try
            {
                _unityBannerAd.DidRecordImpression += handler;

                _testBannerAd.TestOnRecordImpression();

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
            }
            finally
            {
                _unityBannerAd.DidRecordImpression -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator DidBeginDragEventPropagatesFromBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;
            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (unityBanner, x, y) =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidBeginDrag += handler;

                _testBannerAd.TestOnDragBegin(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _unityBannerAd.DidBeginDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator DidEndDragEventPropagatesFromBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            var eventCalled = false;
            UnityBannerAd receivedUnityBannerAd = null;
            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (unityBanner, x, y) =>
            {
                eventCalled = true;
                receivedUnityBannerAd = unityBanner;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidEndDrag += handler;

                _testBannerAd.TestOnDragEnd(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_unityBannerAd, receivedUnityBannerAd);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _unityBannerAd.DidEndDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [Test]
        public void ResetPropagatesCallToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                Assert.DoesNotThrow(() => _unityBannerAd.Reset());
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [Test]
        public void DisposePropagatesCallToBannerAd()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                UnityEngine.Object.DestroyImmediate(_gameObject);
                _gameObject = null; // Prevent double destroy in TearDown

                Assert.IsTrue(_testBannerAd.DisposeCalled);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        #endregion

        #region OnDrag Coordinate Transformation Tests

        [UnityTest]
        public IEnumerator OnDragTransformsScreenCoordinatesCorrectly()
        {
            InjectBannerAd(_testBannerAd);

            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (_, x, y) =>
            {
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidDrag += handler;

                // Set up the initial drag state
                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                // Get current layout params
                var layoutParams = _unityBannerAd.GetComponent<RectTransform>().LayoutParams();
                var pivot = _unityBannerAd.GetComponent<RectTransform>().pivot;

                // Test coordinates from a native (top-left corner)
                const float nativeX = TestConstants.Dimensions.TestNativeX1;
                const float nativeY = TestConstants.Dimensions.TestNativeY1;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, nativeX, nativeY);
                yield return null;

                // Expected transformation:
                // y = Screen.height - nativeY
                // x += widthInPixels * pivot.x
                // y -= heightInPixels * pivot.y
                var expectedY = Screen.height - nativeY;
                var expectedX = nativeX + layoutParams.width * pivot.x;
                expectedY -= layoutParams.height * pivot.y;

                Assert.AreEqual(expectedX, receivedX, TestConstants.Tolerance.Assertion, "X coordinate transformation incorrect");
                Assert.AreEqual(expectedY, receivedY, TestConstants.Tolerance.Assertion, "Y coordinate transformation incorrect");

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator OnDragUpdatesTransformPosition()
        {
            InjectBannerAd(_testBannerAd);

            try
            {
                var initialPosition = _unityBannerAd.transform.position;

                // Set up the initial drag state
                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                const float nativeX = TestConstants.Dimensions.TestNativeX2;
                const float nativeY = TestConstants.Dimensions.TestNativeY2;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, nativeX, nativeY);
                yield return null;

                var newPosition = _unityBannerAd.transform.position;

                // Position should have changed
                Assert.AreNotEqual(initialPosition, newPosition, "Transform position was not updated");

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            }
            finally
            {
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator OnDragWithoutDragBeginDoesNotInvokeEvent()
        {
            InjectBannerAd(_testBannerAd);

            var eventCalled = false;

            UnityBannerAdDragEvent handler = (_, _, _) => eventCalled = true;

            try
            {
                _unityBannerAd.DidDrag += handler;

                // Call OnDrag without calling OnDragBegin first
                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.TestNativeX1, TestConstants.Dimensions.TestNativeY1);
                yield return null;

                Assert.IsFalse(eventCalled, "DidDrag event should not be invoked without preceding DidBeginDrag");
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator OnDragWithCenterPivotAdjustsCorrectly()
        {
            InjectBannerAd(_testBannerAd);

            // Set pivot to center (0.5, 0.5) - this is the default for RectTransform
            var rectTransform = _unityBannerAd.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Center);

            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (_, x, y) =>
            {
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidDrag += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                var layoutParams = rectTransform.LayoutParams();
                const float nativeX = TestConstants.Dimensions.TestNativeX6;
                const float nativeY = TestConstants.Dimensions.TestNativeY6;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, nativeX, nativeY);
                yield return null;

                // With center pivot (0.5, 0.5), we expect:
                // x += width * 0.5
                // y -= height * 0.5
                var expectedX = nativeX + layoutParams.width * TestConstants.Pivots.Center;
                var expectedY = (Screen.height - nativeY) - layoutParams.height * TestConstants.Pivots.Center;

                Assert.AreEqual(expectedX, receivedX, TestConstants.Tolerance.Assertion, "X coordinate with center pivot incorrect");
                Assert.AreEqual(expectedY, receivedY, TestConstants.Tolerance.Assertion, "Y coordinate with center pivot incorrect");

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator OnDragWithTopLeftPivotAdjustsCorrectly()
        {
            InjectBannerAd(_testBannerAd);

            // Set the pivot to top-left (0, 1)
            var rectTransform = _unityBannerAd.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(TestConstants.Pivots.Min, TestConstants.Pivots.Max);

            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (_, x, y) =>
            {
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidDrag += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                var layoutParams = rectTransform.LayoutParams();
                const float nativeX = TestConstants.Dimensions.TestNativeX3;
                const float nativeY = TestConstants.Dimensions.TestNativeY3;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, nativeX, nativeY);
                yield return null;

                // With the top-left pivot (0, 1), we expect:
                // x += width * 0 = x (no change)
                // y -= height * 1 = y - height
                var expectedX = nativeX;
                var expectedY = (Screen.height - nativeY) - layoutParams.height;

                Assert.AreEqual(expectedX, receivedX, TestConstants.Tolerance.Assertion, "X coordinate with top-left pivot incorrect");
                Assert.AreEqual(expectedY, receivedY, TestConstants.Tolerance.Assertion, "Y coordinate with top-left pivot incorrect");

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator OnDragWithBottomRightPivotAdjustsCorrectly()
        {
            InjectBannerAd(_testBannerAd);

            // Set the pivot to bottom-right (1, 0)
            var rectTransform = _unityBannerAd.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(TestConstants.Pivots.Max, TestConstants.Pivots.Min);

            var receivedX = 0f;
            var receivedY = 0f;

            UnityBannerAdDragEvent handler = (_, x, y) =>
            {
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _unityBannerAd.DidDrag += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;

                var layoutParams = rectTransform.LayoutParams();
                const float nativeX = TestConstants.Dimensions.TestNativeX4;
                const float nativeY = TestConstants.Dimensions.TestNativeY4;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, nativeX, nativeY);
                yield return null;

                // With the bottom-right pivot (1, 0), we expect:
                // x += width * 1 = x + width
                // y -= height * 0 = y (no change)
                var expectedX = nativeX + layoutParams.width;
                var expectedY = Screen.height - nativeY;

                Assert.AreEqual(expectedX, receivedX, TestConstants.Tolerance.Assertion, "X coordinate with bottom-right pivot incorrect");
                Assert.AreEqual(expectedY, receivedY, TestConstants.Tolerance.Assertion, "Y coordinate with bottom-right pivot incorrect");

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        [UnityTest]
        public IEnumerator OnDragSequenceTracksMultiplePositions()
        {
            InjectBannerAd(_testBannerAd);

            var dragEventCount = 0;
            var lastX = 0f;
            var lastY = 0f;

            UnityBannerAdDragEvent handler = (_, x, y) =>
            {
                dragEventCount++;
                lastX = x;
                lastY = y;
            };

            try
            {
                _unityBannerAd.DidDrag += handler;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragBegin(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.TestNativeX5, TestConstants.Dimensions.TestNativeY5);
                yield return null;

                // Simulate drag sequence
                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.TestNativeX1, TestConstants.Dimensions.TestNativeY1);
                yield return null;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.TestNativeX2, TestConstants.Dimensions.TestNativeY2);
                yield return null;

                BannerEventTrigger<UnityBannerAd>.TriggerOnDrag(_unityBannerAd, _testBannerAd, TestConstants.Dimensions.TestNativeX6, TestConstants.Dimensions.TestNativeY6);
                yield return null;

                Assert.AreEqual(TestConstants.Counts.ExpectedDragEventCount, dragEventCount, "Expected 3 drag events");

                BannerEventTrigger<UnityBannerAd>.TriggerOnDragEnd(_unityBannerAd, _testBannerAd, lastX, lastY);
                yield return null;
            }
            finally
            {
                _unityBannerAd.DidDrag -= handler;
                UnsubscribeBannerAd(_testBannerAd);
            }
        }

        #endregion

        #region GetTransformSize and LayoutGroup Tests

        [UnityTest]
        public IEnumerator LoadWithoutLayoutGroupDoesNotYield()
        {
            _unityBannerAd.PlacementName = TestConstants.Placements.Standard;

            // GameObject is not part of a LayoutGroup, so GetTransformSize should not yield
            var loadTask = _unityBannerAd.Load();
            yield return new WaitUntil(() => loadTask.IsCompleted);

            Assert.IsNotNull(loadTask.Result);
        }

        // NOTE: LayoutGroup detection logic in UnityBannerAd.cs (lines 326-331) CANNOT be reliably unit tested.
        //
        // The following code cannot be tested:
        //
        //     if (UnityBannerTransform.GetComponentInParent<LayoutGroup>())
        //     {
        //         // Wait a couple of frames
        //         await Task.Yield();
        //         await Task.Yield();
        //         layoutParams = UnityBannerTransform.LayoutParams();
        //     }
        //
        // TECHNICAL REASONS:
        //
        // 1. ASYNC VOID IN UPDATE LIFECYCLE
        //    - SyncWithNativeContainer() is declared as 'async void' and called from Update() (line 300-318)
        //    - When LayoutGroup is present, GetTransformSize() calls await Task.Yield() twice
        //    - This creates an infinite async loop that cannot be awaited or canceled
        //
        // 2. UNCANCELLABLE OPERATIONS
        //    - 'async void' methods cannot be awaited (unlike 'async Task')
        //    - Unity's Update() cannot await async operations
        //    - Once started, the async operation continues even after GameObject.SetActive(false)
        //    - Even using reflection to set _isDragging = true doesn't prevent the lock-up
        //
        // 3. EDITOR LOCK-UP
        //    - All attempted test approaches cause the Unity Editor to freeze
        //    - Attempted solutions:
        //      a) SetActive(false) before Load() - FAILED (async operations continue)
        //      b) Partial class with SkipLayoutGroupDetection flag - FAILED (can't cross assembly boundaries)
        //      c) Reflection to set _isDragging = true - FAILED (still causes lock-up)
        //      d) Only test Load(BannerAdLoadRequest) which doesn't call GetTransformSize() - Works but doesn't test the logic
        //
        // ALTERNATIVE VERIFICATION METHODS:
        //
        // 1. Code Inspection: The logic is straightforward (detect LayoutGroup → wait 2 frames → refresh layout)
        // 2. Manual Testing: Create test scene with UnityBannerAd as child of LayoutGroup and verify behavior
        // 3. Integration Tests: Use PlayMode tests in actual Unity scenes with proper lifecycle management
        // 4. Runtime Testing: Test in actual builds on target platforms
        //
        // ARCHITECTURE RECOMMENDATION:
        //
        // To make this testable, SyncWithNativeContainer() would need to be refactored to return Task instead of void,
        // but this would require significant changes to Unity's MonoBehaviour lifecycle integration.

        #endregion
    }
}
