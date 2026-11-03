using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chartboost.Caching;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner.Unity
{
    public class UnityBannerAdTests : BannerWrapperContractTests<UnityBannerAd>
    {
        private GameObject _gameObject;
        private TestBannerAd _extraBannerAd;

        #region Contract hooks

        protected override UnityBannerAd CreateWrapper()
        {
            _gameObject = new GameObject(TestConstants.Unity.TestUnityBannerAdName);
            return _gameObject.AddComponent<UnityBannerAd>();
        }

        protected override void DestroyWrapper()
        {
            if (_gameObject != null)
                UnityEngine.Object.DestroyImmediate(_gameObject);
            _gameObject = null;
        }

        private protected override void Inject(TestBannerAd bannerAd)
        {
            Wrapper.AttachNativeBannerAd(bannerAd);
        }

        protected override IBannerAd NativeBannerAd => Wrapper.NativeBannerAd;

        protected override IDisposable RecordEvents(List<WrapperEvent> events)
        {
            var wrapper = Wrapper;
            UnityBannerAdEvent willAppear = b => events.Add(new WrapperEvent(nameof(IBannerAd.WillAppear), b));
            UnityBannerAdEvent click = b => events.Add(new WrapperEvent(nameof(IBannerAd.DidClick), b));
            UnityBannerAdEvent impression = b => events.Add(new WrapperEvent(nameof(IBannerAd.DidRecordImpression), b));
            UnityBannerAdDragEvent beginDrag = (b, x, y) => events.Add(new WrapperEvent(nameof(IBannerAd.DidBeginDrag), b, x, y));
            UnityBannerAdDragEvent drag = (b, x, y) => events.Add(new WrapperEvent(nameof(IBannerAd.DidDrag), b, x, y));
            UnityBannerAdDragEvent endDrag = (b, x, y) => events.Add(new WrapperEvent(nameof(IBannerAd.DidEndDrag), b, x, y));
            wrapper.WillAppear += willAppear;
            wrapper.DidClick += click;
            wrapper.DidRecordImpression += impression;
            wrapper.DidBeginDrag += beginDrag;
            wrapper.DidDrag += drag;
            wrapper.DidEndDrag += endDrag;
            return new Subscription(() =>
            {
                wrapper.WillAppear -= willAppear;
                wrapper.DidClick -= click;
                wrapper.DidRecordImpression -= impression;
                wrapper.DidBeginDrag -= beginDrag;
                wrapper.DidDrag -= drag;
                wrapper.DidEndDrag -= endDrag;
            });
        }

        protected override string PlacementName { get => Wrapper.PlacementName; set => Wrapper.PlacementName = value; }
        protected override bool Draggable { get => Wrapper.Draggable; set => Wrapper.Draggable = value; }
        protected override BannerHorizontalAlignment HorizontalAlignment { get => Wrapper.HorizontalAlignment; set => Wrapper.HorizontalAlignment = value; }
        protected override BannerVerticalAlignment VerticalAlignment { get => Wrapper.VerticalAlignment; set => Wrapper.VerticalAlignment = value; }
        protected override IReadOnlyDictionary<string, string> Keywords { get => Wrapper.Keywords; set => Wrapper.Keywords = value; }
        protected override IReadOnlyDictionary<string, string> PartnerSettings { get => Wrapper.PartnerSettings; set => Wrapper.PartnerSettings = value; }
        protected override BannerAdLoadRequest Request => Wrapper.Request;
        protected override string LoadId => Wrapper.LoadId;
        protected override Metrics? LoadMetrics => Wrapper.LoadMetrics;
        protected override BannerSize? WrapperBannerSize => Wrapper.BannerSize;
        protected override BidInfo? WinningBidInfo => Wrapper.WinningBidInfo;
        protected override Task<BannerAdLoadResult> Load() => Wrapper.Load();
        protected override Task<BannerAdLoadResult> Load(BannerAdLoadRequest request) => Wrapper.Load(request);
        protected override void ResetWrapper() => Wrapper.Reset();

        #endregion

        [TearDown]
        public void TearDown()
        {
            _extraBannerAd?.Dispose();
            _extraBannerAd = null;
            AdCache<IAd>.ReleaseAd(TestConstants.UniqueIds.Id22222);
        }

        [Test]
        public void GameObjectHasRectTransformComponent() => Assert.IsNotNull(_gameObject.GetComponent<RectTransform>());

        [Test]
        public void EnablingTheComponentShowsTheNativeBanner()
        {
            Inject(NativeBanner);
            _gameObject.SetActive(false);
            NativeBanner.Visible = false;

            _gameObject.SetActive(true);

            Assert.IsTrue(NativeBanner.Visible);
        }

        #region OnDrag Coordinate Transformation Tests

        // Native drags report the top-left corner; DidDrag reports the pivot in Unity screen space.
        [UnityTest]
        public IEnumerator OnDragTransformsScreenCoordinatesCorrectly()
        {
            var rect = (RectTransform)_gameObject.transform;
            yield return AssertDragReportsPivotPosition(rect.pivot, TestConstants.Dimensions.TestNativeX1, TestConstants.Dimensions.TestNativeY1, "default pivot");
        }

        [UnityTest]
        public IEnumerator OnDragWithCenterPivotAdjustsCorrectly()
        {
            yield return AssertDragReportsPivotPosition(new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Center),
                TestConstants.Dimensions.TestNativeX6, TestConstants.Dimensions.TestNativeY6, "center pivot");
        }

        [UnityTest]
        public IEnumerator OnDragWithTopLeftPivotAdjustsCorrectly()
        {
            yield return AssertDragReportsPivotPosition(new Vector2(TestConstants.Pivots.Min, TestConstants.Pivots.Max),
                TestConstants.Dimensions.TestNativeX3, TestConstants.Dimensions.TestNativeY3, "top-left pivot");
        }

        [UnityTest]
        public IEnumerator OnDragWithBottomRightPivotAdjustsCorrectly()
        {
            yield return AssertDragReportsPivotPosition(new Vector2(TestConstants.Pivots.Max, TestConstants.Pivots.Min),
                TestConstants.Dimensions.TestNativeX4, TestConstants.Dimensions.TestNativeY4, "bottom-right pivot");
        }

        [UnityTest]
        public IEnumerator OnDragUpdatesTransformPosition()
        {
            Inject(NativeBanner);
            var initialPosition = Wrapper.transform.position;

            NativeBanner.TestOnDragBegin(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            yield return null;
            NativeBanner.TestOnDrag(TestConstants.Dimensions.TestNativeX2, TestConstants.Dimensions.TestNativeY2);
            yield return null;

            Assert.AreNotEqual(initialPosition, Wrapper.transform.position, "Transform position was not updated");
            NativeBanner.TestOnDragEnd(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
        }

        [UnityTest]
        public IEnumerator OnDragSequenceTracksMultiplePositions()
        {
            Inject(NativeBanner);
            var events = new List<WrapperEvent>();
            using (RecordEvents(events))
            {
                NativeBanner.TestOnDragBegin(TestConstants.Dimensions.TestNativeX5, TestConstants.Dimensions.TestNativeY5);
                yield return null;
                NativeBanner.TestOnDrag(TestConstants.Dimensions.TestNativeX1, TestConstants.Dimensions.TestNativeY1);
                yield return null;
                NativeBanner.TestOnDrag(TestConstants.Dimensions.TestNativeX2, TestConstants.Dimensions.TestNativeY2);
                yield return null;
                NativeBanner.TestOnDrag(TestConstants.Dimensions.TestNativeX6, TestConstants.Dimensions.TestNativeY6);
                yield return null;
            }

            var drags = events.Where(e => e.Name == nameof(IBannerAd.DidDrag)).ToList();
            Assert.AreEqual(TestConstants.Counts.ExpectedDragEventCount, drags.Count, "Expected 3 drag events");
            var last = drags[drags.Count - 1];
            NativeBanner.TestOnDragEnd(last.X, last.Y);
        }

        // x moves right by width * pivot.x; y flips to bottom-up, then moves down by height * pivot.y.
        private IEnumerator AssertDragReportsPivotPosition(Vector2 pivot, float nativeX, float nativeY, string label)
        {
            Inject(NativeBanner);
            var rect = (RectTransform)_gameObject.transform;
            rect.pivot = pivot;
            var events = new List<WrapperEvent>();
            using (RecordEvents(events))
            {
                NativeBanner.TestOnDragBegin(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                yield return null;
                var layoutParams = rect.LayoutParams();
                NativeBanner.TestOnDrag(nativeX, nativeY);
                yield return null;
                NativeBanner.TestOnDragEnd(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);

                var drag = events.Single(e => e.Name == nameof(IBannerAd.DidDrag));
                var expectedX = nativeX + layoutParams.width * pivot.x;
                var expectedY = Screen.height - nativeY - layoutParams.height * pivot.y;
                Assert.AreEqual(expectedX, drag.X, TestConstants.Tolerance.Assertion, $"X coordinate with {label} incorrect");
                Assert.AreEqual(expectedY, drag.Y, TestConstants.Tolerance.Assertion, $"Y coordinate with {label} incorrect");
            }
        }

        #endregion

        #region GetTransformSize and LayoutGroup Tests

        [UnityTest]
        public IEnumerator LoadWithoutLayoutGroupDoesNotYield()
        {
            Wrapper.PlacementName = TestConstants.Placements.Standard;

            // Not in a LayoutGroup, so GetTransformSize shouldn't wait for layout.
            var loadTask = Wrapper.Load();
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

            Assert.IsNotNull(loadTask.Result);
        }

        // Inside a LayoutGroup the size is re-read after layout settles: a resize during that wait wins.
        [UnityTest]
        public IEnumerator InsideALayoutGroupTheSizeIsReadAfterLayoutSettles()
        {
            var layout = ParentInLayoutGroup(new Vector2(100, 50));
            var recording = ExtraBannerAd(id => new RecordingBannerAd(id));
            try
            {
                var rect = (RectTransform)_gameObject.transform;
                Inject(recording);

                // This frame's sync read 100x50 and is waiting for layout; resize before it re-reads.
                yield return null;
                rect.sizeDelta = new Vector2(320, 50);

                yield return TestWait.Until(() => recording.ContainerSizes.Count > 0, TestConstants.Timeouts.EditorTask);

                Assert.IsNotEmpty(recording.ContainerSizes, "the container size never reached the banner");
                // Rounded like ContainerSize.FixedSize; truncating fails off whole-number densities (e.g. 2.625 → 121.9).
                Assert.AreEqual(Mathf.RoundToInt(DensityConverters.PixelsToNative(320)), recording.ContainerSizes[0].Width, "the first size wasn't re-read after layout");
            }
            finally
            {
                _gameObject.transform.SetParent(null, false);
                UnityEngine.Object.DestroyImmediate(layout);
            }
        }

        [UnityTest]
        public IEnumerator DestroyingTheBannerWhileItWaitsForLayoutLogsNothing()
        {
            var layout = ParentInLayoutGroup(new Vector2(320, 50));
            var recording = ExtraBannerAd(id => new RecordingBannerAd(id));
            var errors = 0;
            void CountErrors(string message, LogLevel level)
            {
                if (level == LogLevel.Error)
                    errors++;
            }

            // Errors counted through LogController: other fixtures' log levels let harmless lifecycle logs through.
            LogController.MessageLogged += CountErrors;
            try
            {
                Inject(recording);

                // The sync is now waiting for layout; destroy the banner under it.
                yield return null;
                Assert.IsTrue(Wrapper.SyncInProgress, "no sync started");
                Assert.IsEmpty(recording.ContainerSizes, "the sync didn't wait for layout");
                DestroyWrapper();
                for (var frame = 0; frame < 4; frame++)
                    yield return null;

                Assert.AreEqual(0, errors, "the sync errored on the destroyed banner");
                Assert.IsEmpty(recording.ContainerSizes, "the sync wrote to the destroyed banner");
            }
            finally
            {
                LogController.MessageLogged -= CountErrors;
                UnityEngine.Object.DestroyImmediate(layout);
            }
        }

        [UnityTest]
        public IEnumerator LoadingWhileTheBannerIsDestroyedReturnsNotReady()
        {
            var layout = ParentInLayoutGroup(new Vector2(320, 50));
            var recording = ExtraBannerAd(id => new RecordingBannerAd(id));
            try
            {
                Inject(recording);
                Wrapper.PlacementName = TestConstants.Placements.Standard;
                var loadTask = Wrapper.Load();

                // Load is waiting for layout; destroy the banner under it.
                Assert.IsFalse(loadTask.IsCompleted, "Load didn't wait for layout");
                DestroyWrapper();
                yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

                Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
                Assert.AreEqual(Errors.ErrorNotReady, loadTask.Result.Error?.Message);
                Assert.AreEqual(0, recording.LoadCalls, "the destroyed banner still loaded");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(layout);
            }
        }

        #endregion

        #region Automation Slot Overrides

        [UnityTest]
        public IEnumerator AutomationOverridesDriveTheNativeContainerOnlyWhenTheyChange()
        {
            var recording = ExtraBannerAd(id => new RecordingBannerAd(id));
            Inject(recording);
            Assert.AreSame(recording, Wrapper.NativeBannerAd);
            Wrapper.ContainerSizeOverride = new Vector2(300, 250);
            Wrapper.ContainerPositionOverride = new Vector2(10, 20);

            yield return TestWait.Until(() => recording.ContainerSizes.Count > 0, TestConstants.Timeouts.EditorTask);
            CollectionAssert.AreEqual(new[] { ContainerSize.FixedSize(300, 250) }, recording.ContainerSizes);
            Assert.AreEqual(new Vector2(10, 20), recording.Position);

            // Several sync cycles with unchanged overrides: nothing is applied again.
            for (var frame = 0; frame < 4; frame++)
                yield return null;
            Assert.AreEqual(1, recording.ContainerSizes.Count, "an unchanged override was applied again");

            Wrapper.ContainerSizeOverride = new Vector2(320, 50);
            yield return TestWait.Until(() => recording.ContainerSizes.Count > 1, TestConstants.Timeouts.EditorTask);
            Assert.AreEqual(ContainerSize.FixedSize(320, 50), recording.ContainerSizes[1]);
        }

        // A failing native write is logged, not thrown, and the next frame's sync tries again.
        [UnityTest]
        public IEnumerator ASyncFailureIsLoggedAndRetriedNextFrame()
        {
            var throwing = ExtraBannerAd(id => new ThrowingPositionBannerAd(id));
            var failures = 0;
            void Count(string message, LogLevel level)
            {
                if (level == LogLevel.Error && message.Contains("Error syncing banner with native container: position rejected"))
                    failures++;
            }

            // Logged every frame while it fails, so counted here rather than through one LogAssert.Expect.
            LogAssert.ignoreFailingMessages = true;
            LogController.MessageLogged += Count;
            try
            {
                Inject(throwing);
                Wrapper.ContainerPositionOverride = new Vector2(1, 2);

                yield return TestWait.Until(() => failures >= 2, TestConstants.Timeouts.EditorTask);

                Assert.GreaterOrEqual(failures, 2, "the sync didn't log the failure and retry");
                Wrapper.ContainerPositionOverride = null;
            }
            finally
            {
                LogController.MessageLogged -= Count;
                LogAssert.ignoreFailingMessages = false;
            }
        }

        #endregion

        #region Helpers

        // A second test banner, disposed and released in TearDown.
        private T ExtraBannerAd<T>(Func<IntPtr, T> create) where T : TestBannerAd
        {
            var bannerAd = create(new IntPtr(TestConstants.UniqueIds.Id22222));
            _extraBannerAd = bannerAd;
            return bannerAd;
        }

        // Parents the banner under a LayoutGroup that keeps the banner's own size (by default it shrinks children to 0x0).
        private GameObject ParentInLayoutGroup(Vector2 size)
        {
            var layout = new GameObject("LayoutGroupParent", typeof(RectTransform), typeof(UnityEngine.UI.VerticalLayoutGroup));
            var group = layout.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
            group.childControlWidth = false;
            group.childControlHeight = false;
            _gameObject.transform.SetParent(layout.transform, false);
            ((RectTransform)_gameObject.transform).sizeDelta = size;
            return layout;
        }

        // Records every ContainerSize write and Load call, so a test can tell how often and in which order they happened.
        private sealed class RecordingBannerAd : TestBannerAd
        {
            public RecordingBannerAd(IntPtr uniqueId) : base(uniqueId) { }

            public readonly List<ContainerSize> ContainerSizes = new();
            public int LoadCalls { get; private set; }

            public override Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
            {
                LoadCalls++;
                return base.Load(request);
            }

            public override ContainerSize ContainerSize
            {
                get => base.ContainerSize;
                set
                {
                    ContainerSizes.Add(value);
                    base.ContainerSize = value;
                }
            }
        }

        private sealed class ThrowingPositionBannerAd : TestBannerAd
        {
            public ThrowingPositionBannerAd(IntPtr uniqueId) : base(uniqueId) { }

            public override Vector2 Position
            {
                get => base.Position;
                set => throw new InvalidOperationException("position rejected");
            }
        }

        #endregion
    }
}
