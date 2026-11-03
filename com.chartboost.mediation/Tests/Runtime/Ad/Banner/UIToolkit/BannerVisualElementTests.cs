using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chartboost.Caching;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.UIToolkit;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Chartboost.Tests.Runtime.Ad.Banner.UIToolkit
{
    public class BannerVisualElementTests : BannerWrapperContractTests<BannerVisualElement>
    {
        private const long SizedBannerId = TestConstants.UniqueIds.Id50001;

        private UIDocument _uiDocument;
        private GameObject _testGameObject;
        private LogLevel _initialLogLevel;
        private PanelSettings _panelSettings;
        private TestBannerAdWithSize _sizedBanner;

        #region Contract hooks

        protected override BannerVisualElement CreateWrapper()
        {
            _testGameObject = new GameObject(TestConstants.Unity.TestUIDocumentName);
            _uiDocument = _testGameObject.AddComponent<UIDocument>();
            var element = new BannerVisualElement { PlacementName = TestConstants.Placements.Standard };
            // On the document, but with no PanelSettings it's never laid out.
            _uiDocument.rootVisualElement.Add(element);
            return element;
        }

        protected override void DestroyWrapper() => Wrapper.Dispose();

        private protected override void Inject(TestBannerAd bannerAd)
        {
            Wrapper.AttachNativeBannerAd(bannerAd);
        }

        protected override IBannerAd NativeBannerAd => Wrapper.NativeBannerAd;

        protected override IDisposable RecordEvents(List<WrapperEvent> events)
        {
            var wrapper = Wrapper;
            BannerVisualElementAdEvent willAppear = b => events.Add(new WrapperEvent(nameof(IBannerAd.WillAppear), b));
            BannerVisualElementAdEvent click = b => events.Add(new WrapperEvent(nameof(IBannerAd.DidClick), b));
            BannerVisualElementAdEvent impression = b => events.Add(new WrapperEvent(nameof(IBannerAd.DidRecordImpression), b));
            BannerVisualElementAdDragEvent beginDrag = (b, x, y) => events.Add(new WrapperEvent(nameof(IBannerAd.DidBeginDrag), b, x, y));
            BannerVisualElementAdDragEvent drag = (b, x, y) => events.Add(new WrapperEvent(nameof(IBannerAd.DidDrag), b, x, y));
            BannerVisualElementAdDragEvent endDrag = (b, x, y) => events.Add(new WrapperEvent(nameof(IBannerAd.DidEndDrag), b, x, y));
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

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;
        }

        [TearDown]
        public void TearDown()
        {
            LogController.LoggingLevel = _initialLogLevel;

            _sizedBanner?.Dispose();
            _sizedBanner = null;
            AdCache<IAd>.ReleaseAd(SizedBannerId);

            if (_testGameObject != null)
                UnityEngine.Object.DestroyImmediate(_testGameObject);
            if (_panelSettings != null)
                UnityEngine.Object.DestroyImmediate(_panelSettings);
        }

        // Puts the banner element on a real panel; the fixture's document has none, so it's never laid out.
        private IEnumerator AttachToPanel()
        {
            _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            _uiDocument.panelSettings = _panelSettings;
            yield return null;
            _uiDocument.rootVisualElement.Add(Wrapper);
            Assert.IsNotNull(Wrapper.panel, "the banner element isn't on a panel");
        }

        [Test]
        public void ConstructorCreatesAdElement() => Assert.IsNotNull(Wrapper.Q(TestConstants.BannerVisualElement.AdElementName));

        #region Load and Lifecycle Tests

        // The fixture's document has no PanelSettings, so the element is never laid out.
        [UnityTest]
        public IEnumerator LoadBeforeLayoutReturnsErrorWithoutLoadingTheBanner()
        {
            Inject(NativeBanner);
            Assert.IsTrue(float.IsNaN(Wrapper.worldBound.width), "expected an element that isn't laid out");

            var loadTask = Wrapper.Load();
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

            Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
            Assert.AreEqual(Errors.BannerNotLaidOut, loadTask.Result.Error?.Message);
            Assert.IsNull(NativeBanner.Request, "the banner ad was loaded with a NaN size");
        }

        [UnityTest]
        public IEnumerator LoadAfterLayoutLoadsTheBannerWithItsSize()
        {
            Wrapper.style.width = TestConstants.Dimensions.StandardWidth;
            Wrapper.style.height = TestConstants.Dimensions.StandardHeight;
            yield return AttachToPanel();
            Inject(NativeBanner);

            // worldBound updates after a layout pass.
            yield return TestWait.Until(() => !float.IsNaN(Wrapper.worldBound.width), TestConstants.Timeouts.EditorTask);
            Assert.IsFalse(float.IsNaN(Wrapper.worldBound.width), "the element was never laid out");

            var loadTask = Wrapper.Load();
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

            Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
            Assert.IsNotNull(NativeBanner.Request, "the banner ad wasn't loaded");
            Assert.AreEqual(TestConstants.Placements.Standard, NativeBanner.Request.PlacementName);
            // Layout snaps to pixels, so compare against the laid-out bounds.
            var bounds = Wrapper.worldBound;
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, bounds.width, 1f);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, bounds.height, 1f);
            Assert.AreEqual(DensityConverters.UIDocToNative(bounds.width), NativeBanner.Request.Size.Width, 0.01f);
            Assert.AreEqual(DensityConverters.UIDocToNative(bounds.height), NativeBanner.Request.Size.Height, 0.01f);
        }

        // Automation slot hosting: the overrides, not the element's own bounds, size and place the new banner.
        [Test]
        public void TheAutomationOverridesSizeAndPlaceTheNewBanner()
        {
            Wrapper.ContainerSizeOverride = new Vector2(300, 250);
            Wrapper.ContainerPositionOverride = new Vector2(10, 20);

            _ = Wrapper.Request;

            var banner = Wrapper.NativeBannerAd;
            Assert.IsNotNull(banner, "reading the request didn't create the banner");
            Assert.AreEqual(ContainerSize.FixedSize(300, 250), banner.ContainerSize);
            Assert.AreEqual(new Vector2(10, 20), banner.Position);
        }

        [UnityTest]
        public IEnumerator DetachFromPanelCallsDispose()
        {
            yield return AttachToPanel();
            Inject(NativeBanner);

            _uiDocument.rootVisualElement.Remove(Wrapper);

            // Disposal is deferred to the panel's scheduler.
            yield return TestWait.Until(() => NativeBanner.DisposeCalled, TestConstants.Timeouts.EditorTask);

            Assert.IsTrue(NativeBanner.DisposeCalled, "detaching from the panel didn't dispose the banner ad");
        }

        // Unlike UnityBannerAd, the element forwards the native drag coordinates unchanged.
        [Test]
        public void DidDragForwardsTheNativeCoordinatesUnchanged()
        {
            Inject(NativeBanner);
            var events = new List<WrapperEvent>();

            using (RecordEvents(events))
            {
                NativeBanner.TestOnDragBegin(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
                NativeBanner.TestOnDrag(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
            }

            var drag = events.Single(e => e.Name == nameof(IBannerAd.DidDrag));
            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, drag.X);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, drag.Y);
        }

        #endregion

        #region SyncWithNativeContainer Tests

        [Test]
        public void SyncWithNativeContainerDoesNothingWhenBannerAdIsNull()
        {
            Assert.DoesNotThrow(Wrapper.SyncWithNativeContainer);

            Assert.IsNull(Wrapper.NativeBannerAd, "the sync created a banner");
        }

        [Test]
        public void SyncWithNativeContainerUpdatesContainerSizeWhenDifferent()
        {
            NativeBanner.ContainerSize = ContainerSize.FixedSize((int)TestConstants.Dimensions.TestContainerWidth, (int)TestConstants.Dimensions.TestContainerHeight);
            Inject(NativeBanner);

            Wrapper.SyncWithNativeContainer();

            // ContainerSize now matches the element's size.
            Assert.AreNotEqual((int)TestConstants.Dimensions.TestContainerWidth, NativeBanner.ContainerSize.Width);
            Assert.AreNotEqual((int)TestConstants.Dimensions.TestContainerHeight, NativeBanner.ContainerSize.Height);
        }

        [Test]
        public void SyncWithNativeContainerUpdatesPositionWhenDifferentAndNotDragging()
        {
            NativeBanner.Position = new Vector2(TestConstants.Dimensions.TestPositionX2, TestConstants.Dimensions.TestPositionY2);
            Inject(NativeBanner);

            Wrapper.SyncWithNativeContainer();

            // Position now matches the element's position.
            Assert.AreNotEqual(TestConstants.Dimensions.TestPositionX2, NativeBanner.Position.x);
            Assert.AreNotEqual(TestConstants.Dimensions.TestPositionY2, NativeBanner.Position.y);
        }

        [Test]
        public void SyncWithNativeContainerDoesNotUpdatePositionWhileDragging()
        {
            Inject(NativeBanner);
            NativeBanner.TestOnDragBegin(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            NativeBanner.Position = new Vector2(TestConstants.Dimensions.TestPositionX2, TestConstants.Dimensions.TestPositionY2);

            Wrapper.SyncWithNativeContainer();

            Assert.AreEqual(TestConstants.Dimensions.TestPositionX2, NativeBanner.Position.x);
            Assert.AreEqual(TestConstants.Dimensions.TestPositionY2, NativeBanner.Position.y);
        }

        [Test]
        public void SyncWithNativeContainerUpdatesVisibleWhenDifferent()
        {
            NativeBanner.Visible = false;
            Inject(NativeBanner);
            Wrapper.style.display = DisplayStyle.Flex;

            Wrapper.SyncWithNativeContainer();

            Assert.IsTrue(NativeBanner.Visible);
        }

        [Test]
        public void SyncWithNativeContainerUpdatesAdRelativePositionWhenDifferent()
        {
            NativeBanner.AdRelativePosition = new Vector2(TestConstants.Dimensions.RelativePositionX, TestConstants.Dimensions.RelativePositionY);
            Inject(NativeBanner);

            Wrapper.SyncWithNativeContainer();

            // AdRelativePosition now matches the ad element's relative position.
            Assert.AreNotEqual(TestConstants.Dimensions.RelativePositionX, NativeBanner.AdRelativePosition.x);
            Assert.AreNotEqual(TestConstants.Dimensions.RelativePositionY, NativeBanner.AdRelativePosition.y);
        }

        #endregion

        #region SetAbsolutePositionByAlignment Tests

        [Test]
        public void SetAbsolutePositionByAlignmentLogsWarningWhenBannerSizeIsNull()
        {
            LogAssert.Expect(LogType.Warning, new Regex("Cannot position banner.*BannerSize is null"));

            Wrapper.SetAbsolutePositionByAlignment(0.5f, 0.5f);
        }

        [Test]
        public void SetAbsolutePositionByAlignmentSetsTopLeftPosition()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0f, 0f);

            AssertAbsolutePosition(0f, 0f);
            Assert.AreEqual(StyleKeyword.Auto, Wrapper.style.right.keyword);
            Assert.AreEqual(StyleKeyword.Auto, Wrapper.style.bottom.keyword);
        }

        [Test]
        public void SetAbsolutePositionByAlignmentSetsTopRightPosition()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(1f, 0f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 1f), 0f);
        }

        [Test]
        public void SetAbsolutePositionByAlignmentSetsBottomLeftPosition()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0f, 1f);

            AssertAbsolutePosition(0f, ExpectedTop(BannerSize.Standard, 1f));
        }

        [Test]
        public void SetAbsolutePositionByAlignmentSetsBottomRightPosition()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(1f, 1f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 1f), ExpectedTop(BannerSize.Standard, 1f));
        }

        [Test]
        public void SetAbsolutePositionByAlignmentSetsCenterPosition()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0.5f, 0.5f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 0.5f), ExpectedTop(BannerSize.Standard, 0.5f));
        }

        [Test]
        public void SetAbsolutePositionByAlignmentSetsCustomAlignmentPosition()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0.25f, 0.75f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 0.25f), ExpectedTop(BannerSize.Standard, 0.75f));
        }

        [Test]
        public void SetAbsolutePositionByAlignmentResizesContainerToMatchBannerSize()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0.5f, 0.5f);

            AssertSize(BannerSize.Standard);
        }

        [Test]
        public void SetAbsolutePositionByAlignmentWorksWithMediumRectBannerSize()
        {
            InjectSized(BannerSize.MediumRect);

            Wrapper.SetAbsolutePositionByAlignment(0.5f, 0.5f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.MediumRect, 0.5f), ExpectedTop(BannerSize.MediumRect, 0.5f));
            AssertSize(BannerSize.MediumRect);
        }

        [Test]
        public void SetAbsolutePositionByAlignmentWorksWithLeaderboardBannerSize()
        {
            InjectSized(BannerSize.Leaderboard);

            Wrapper.SetAbsolutePositionByAlignment(0.5f, 0f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.Leaderboard, 0.5f), 0f);
            AssertSize(BannerSize.Leaderboard);
        }

        [Test]
        public void SetAbsolutePositionByAlignmentCanBeCalledMultipleTimes()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0f, 0f);
            AssertAbsolutePosition(0f, 0f);

            Wrapper.SetAbsolutePositionByAlignment(1f, 1f);
            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 1f), ExpectedTop(BannerSize.Standard, 1f));

            Wrapper.SetAbsolutePositionByAlignment(0.5f, 0.5f);
            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 0.5f), ExpectedTop(BannerSize.Standard, 0.5f));
        }

        // Alignments that would land on fractional pixels without rounding.
        [Test]
        public void SetAbsolutePositionByAlignmentUsesPixelPerfectRounding()
        {
            InjectSized(BannerSize.Standard);

            Wrapper.SetAbsolutePositionByAlignment(0.333f, 0.667f);

            AssertAbsolutePosition(ExpectedLeft(BannerSize.Standard, 0.333f), ExpectedTop(BannerSize.Standard, 0.667f));
            Assert.AreEqual(Mathf.Floor(Wrapper.style.left.value.value), Wrapper.style.left.value.value, "left isn't a whole pixel");
            Assert.AreEqual(Mathf.Floor(Wrapper.style.top.value.value), Wrapper.style.top.value.value, "top isn't a whole pixel");
        }

        // BannerSize is native units; the ad element is laid out in panel units, which change when the device rotates.
        [Test]
        public void TheAdElementFollowsTheCreativeSizeInPanelUnits()
        {
            var originalScale = DensityConverters.ScaleFactor;
            var originalUIDocScale = DensityConverters._uiDocScaleFactor;
            try
            {
                DensityConverters.ScaleFactor = TestConstants.DensityConverters.DefaultScaleFactor;
                DensityConverters._uiDocScaleFactor = TestConstants.DensityConverters.TestUIDocScaleFactor;
                InjectSized(BannerSize.Standard);
                var ad = Wrapper.Q(TestConstants.BannerVisualElement.AdElementName);

                _sizedBanner.TestOnWillAppear();
                AssertAdElementSize(ad, BannerSize.Standard);

                DensityConverters._uiDocScaleFactor = TestConstants.DensityConverters.RotatedUIDocScaleFactor;
                Wrapper.SyncWithNativeContainer();
                AssertAdElementSize(ad, BannerSize.Standard);
            }
            finally
            {
                DensityConverters.ScaleFactor = originalScale;
                DensityConverters._uiDocScaleFactor = originalUIDocScale;
            }
        }

        private static void AssertAdElementSize(VisualElement ad, BannerSize size)
        {
            Assert.AreEqual(DensityConverters.NativeToUIDoc(size.Width), ad.style.width.value.value, 0.01f, "width");
            Assert.AreEqual(DensityConverters.NativeToUIDoc(size.Height), ad.style.height.value.value, 0.01f, "height");
        }

        // A sized test banner, disposed and released in TearDown.
        private void InjectSized(BannerSize size)
        {
            _sizedBanner = new TestBannerAdWithSize(new IntPtr(SizedBannerId), size);
            Inject(_sizedBanner);
        }

        private static float ExpectedLeft(BannerSize size, float alignment)
            => Mathf.Round((Screen.width - DensityConverters.NativeToPixels(size.Width)) * alignment);

        private static float ExpectedTop(BannerSize size, float alignment)
            => Mathf.Round((Screen.height - DensityConverters.NativeToPixels(size.Height)) * alignment);

        private void AssertAbsolutePosition(float left, float top)
        {
            Assert.AreEqual(Position.Absolute, Wrapper.style.position.value);
            Assert.AreEqual(left, Wrapper.style.left.value.value);
            Assert.AreEqual(top, Wrapper.style.top.value.value);
        }

        private void AssertSize(BannerSize size)
        {
            Assert.AreEqual(DensityConverters.NativeToPixels(size.Width), Wrapper.style.width.value.value);
            Assert.AreEqual(DensityConverters.NativeToPixels(size.Height), Wrapper.style.height.value.value);
        }

        #endregion
    }
}
