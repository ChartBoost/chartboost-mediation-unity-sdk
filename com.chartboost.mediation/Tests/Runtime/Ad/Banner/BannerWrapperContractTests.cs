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
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    /// <summary>
    /// The contract every banner wrapper (UnityBannerAd, BannerVisualElement) keeps with its native <see cref="IBannerAd"/>.
    /// </summary>
    public abstract class BannerWrapperContractTests<TWrapper> where TWrapper : class
    {
        private const long NativeBannerId = TestConstants.UniqueIds.Id11111;

        protected TWrapper Wrapper { get; private set; }

        // A fake native banner, created per test; Inject it to back the wrapper.
        private protected TestBannerAd NativeBanner { get; private set; }

        #region Hooks

        protected abstract TWrapper CreateWrapper();

        // Destroys/disposes the wrapper; must be safe to call twice.
        protected abstract void DestroyWrapper();

        // Makes the banner the wrapper's native banner, subscribed like a lazily created one.
        private protected abstract void Inject(TestBannerAd bannerAd);

        // The wrapper's native banner, without creating one.
        protected abstract IBannerAd NativeBannerAd { get; }

        // Records every wrapper event into events until the returned handle is disposed.
        protected abstract IDisposable RecordEvents(List<WrapperEvent> events);

        protected abstract string PlacementName { get; set; }
        protected abstract bool Draggable { get; set; }
        protected abstract BannerHorizontalAlignment HorizontalAlignment { get; set; }
        protected abstract BannerVerticalAlignment VerticalAlignment { get; set; }
        protected abstract IReadOnlyDictionary<string, string> Keywords { get; set; }
        protected abstract IReadOnlyDictionary<string, string> PartnerSettings { get; set; }
        protected abstract BannerAdLoadRequest Request { get; }
        protected abstract string LoadId { get; }
        protected abstract Metrics? LoadMetrics { get; }
        protected abstract BannerSize? WrapperBannerSize { get; }
        protected abstract BidInfo? WinningBidInfo { get; }
        protected abstract Task<BannerAdLoadResult> Load();
        protected abstract Task<BannerAdLoadResult> Load(BannerAdLoadRequest request);
        protected abstract void ResetWrapper();

        #endregion

        [SetUp]
        public void SetUpWrapperContract()
        {
            Wrapper = CreateWrapper();
            NativeBanner = new TestBannerAd(new IntPtr(NativeBannerId));
        }

        [TearDown]
        public void TearDownWrapperContract()
        {
            DestroyWrapper();
            NativeBanner?.Dispose();
            AdCache<IAd>.ReleaseAd(NativeBannerId);
        }

        #region Properties

        [Test]
        public void PlacementNameCanBeSetAndRetrieved()
        {
            PlacementName = TestConstants.Placements.New;

            Assert.AreEqual(TestConstants.Placements.New, PlacementName);
        }

        [Test]
        public void DraggableCanBeSetAndRetrieved()
        {
            Draggable = true;
            Assert.IsTrue(Draggable);

            Draggable = false;
            Assert.IsFalse(Draggable);
        }

        [Test]
        public void HorizontalAlignmentCanBeSetAndRetrieved()
        {
            foreach (var alignment in new[] { BannerHorizontalAlignment.Left, BannerHorizontalAlignment.Center, BannerHorizontalAlignment.Right })
            {
                HorizontalAlignment = alignment;
                Assert.AreEqual(alignment, HorizontalAlignment);
            }
        }

        [Test]
        public void VerticalAlignmentCanBeSetAndRetrieved()
        {
            foreach (var alignment in new[] { BannerVerticalAlignment.Top, BannerVerticalAlignment.Center, BannerVerticalAlignment.Bottom })
            {
                VerticalAlignment = alignment;
                Assert.AreEqual(alignment, VerticalAlignment);
            }
        }

        [Test]
        public void InitialHorizontalAlignmentIsCenter() => Assert.AreEqual(BannerHorizontalAlignment.Center, HorizontalAlignment);

        [Test]
        public void InitialVerticalAlignmentIsCenter() => Assert.AreEqual(BannerVerticalAlignment.Center, VerticalAlignment);

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            Keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            Assert.IsNotNull(Keywords);
        }

        [Test]
        public void PartnerSettingsCanBeSetAndRetrieved()
        {
            PartnerSettings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            Assert.IsNotNull(PartnerSettings);
        }

        [Test]
        public void RequestReturnsNullBeforeLoad() => Assert.IsNull(Request);

        [Test]
        public void LoadIdReturnsNullBeforeLoad() => Assert.IsTrue(string.IsNullOrEmpty(LoadId));

        [Test]
        public void LoadMetricsReturnsNullBeforeLoad() => Assert.IsNull(LoadMetrics);

        [Test]
        public void BannerSizeReturnsNullBeforeLoad() => Assert.IsNull(WrapperBannerSize);

        [Test]
        public void WinningBidInfoReturnsNullBeforeLoad() => Assert.IsNull(WinningBidInfo);

        #endregion

        #region Propagation to the native banner

        [Test]
        public void DraggablePropagatesToBannerAd()
        {
            Inject(NativeBanner);

            Draggable = true;
            Assert.IsTrue(NativeBanner.Draggable);

            Draggable = false;
            Assert.IsFalse(NativeBanner.Draggable);
        }

        [Test]
        public void KeywordsPropagatesToBannerAd()
        {
            Inject(NativeBanner);
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            Keywords = keywords;

            Assert.AreSame(keywords, NativeBanner.Keywords);
        }

        [Test]
        public void PartnerSettingsPropagatesToBannerAd()
        {
            Inject(NativeBanner);
            var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            PartnerSettings = settings;

            Assert.AreSame(settings, NativeBanner.PartnerSettings);
        }

        [Test]
        public void HorizontalAlignmentPropagatesToBannerAd()
        {
            Inject(NativeBanner);

            HorizontalAlignment = BannerHorizontalAlignment.Left;
            Assert.AreEqual(BannerHorizontalAlignment.Left, NativeBanner.HorizontalAlignment);

            HorizontalAlignment = BannerHorizontalAlignment.Right;
            Assert.AreEqual(BannerHorizontalAlignment.Right, NativeBanner.HorizontalAlignment);
        }

        [Test]
        public void VerticalAlignmentPropagatesToBannerAd()
        {
            Inject(NativeBanner);

            VerticalAlignment = BannerVerticalAlignment.Top;
            Assert.AreEqual(BannerVerticalAlignment.Top, NativeBanner.VerticalAlignment);

            VerticalAlignment = BannerVerticalAlignment.Bottom;
            Assert.AreEqual(BannerVerticalAlignment.Bottom, NativeBanner.VerticalAlignment);
        }

        [Test]
        public void ResetDoesNotThrow() => Assert.DoesNotThrow(ResetWrapper);

        [Test]
        public void ResetPropagatesCallToBannerAd()
        {
            Inject(NativeBanner);

            ResetWrapper();

            Assert.IsTrue(NativeBanner.ResetCalled);
        }

        [Test]
        public void DisposePropagatesCallToBannerAd()
        {
            Inject(NativeBanner);
            Assert.IsFalse(NativeBanner.DisposeCalled);

            DestroyWrapper();

            Assert.IsTrue(NativeBanner.DisposeCalled);
        }

        [Test]
        public void DestroyBeforeTheBannerExistsDoesNotThrow() => Assert.DoesNotThrow(DestroyWrapper);

        [Test]
        public void DestroyAfterTheBannerIsCreatedDoesNotThrow()
        {
            PlacementName = TestConstants.Placements.Standard;
            _ = Keywords;

            Assert.DoesNotThrow(DestroyWrapper);
        }

        [Test]
        public void ToStringReturnsJsonRepresentation()
        {
            NativeBanner.Keywords = new Dictionary<string, string> { { TestConstants.BannerVisualElement.JsonKey, TestConstants.BannerVisualElement.JsonValue } };
            NativeBanner.PartnerSettings = new Dictionary<string, string> { { TestConstants.BannerVisualElement.PartnerKey, TestConstants.BannerVisualElement.PartnerValue } };
            NativeBanner.Draggable = true;
            Inject(NativeBanner);

            var json = Wrapper.ToString();

            Assert.IsFalse(string.IsNullOrEmpty(json));
            StringAssert.StartsWith(TestConstants.Json.OpenBrace, json);
            StringAssert.EndsWith(TestConstants.Json.CloseBrace, json);
            StringAssert.Contains(TestConstants.BannerVisualElement.JsonKey, json);
            StringAssert.Contains(TestConstants.BannerVisualElement.PartnerKey, json);
            StringAssert.Contains(TestConstants.Json.DraggableProperty, json);
        }

        #endregion

        #region Load

        [UnityTest]
        public IEnumerator LoadWithNullPlacementNameReturnsError()
        {
            PlacementName = null;
            yield return AssertLoadReturnsNotReady();
        }

        [UnityTest]
        public IEnumerator LoadWithEmptyPlacementNameReturnsError()
        {
            PlacementName = string.Empty;
            yield return AssertLoadReturnsNotReady();
        }

        [UnityTest]
        public IEnumerator LoadWithRequestSetsPlacementName()
        {
            Inject(NativeBanner);
            var request = new BannerAdLoadRequest(TestConstants.Placements.Load, BannerSize.Standard);

            var loadTask = Load(request);
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

            Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
            Assert.AreEqual(TestConstants.Placements.Load, PlacementName);
        }

        [UnityTest]
        public IEnumerator RequestReturnsLoadRequestAfterLoad()
        {
            Inject(NativeBanner);
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);

            var loadTask = Load(request);
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

            Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
            Assert.AreSame(request, NativeBanner.Request, "the request didn't reach the native banner");
            Assert.AreSame(request, Request);
        }

        private IEnumerator AssertLoadReturnsNotReady()
        {
            var loadTask = Load();
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

            Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
            Assert.IsNotNull(loadTask.Result.Error);
            Assert.AreEqual(Errors.ErrorNotReady, loadTask.Result.Error.Value.Message);
        }

        #endregion

        #region Events

        [Test]
        public void WillAppearEventPropagatesFromBannerAd() => AssertForwarded(nameof(IBannerAd.WillAppear), NativeBanner.TestOnWillAppear);

        [Test]
        public void DidClickEventPropagatesFromBannerAd() => AssertForwarded(nameof(IBannerAd.DidClick), NativeBanner.TestOnClick);

        [Test]
        public void DidRecordImpressionEventPropagatesFromBannerAd() => AssertForwarded(nameof(IBannerAd.DidRecordImpression), NativeBanner.TestOnRecordImpression);

        [Test]
        public void DidBeginDragEventPropagatesFromBannerAd()
        {
            var fired = AssertForwarded(nameof(IBannerAd.DidBeginDrag),
                () => NativeBanner.TestOnDragBegin(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise));

            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, fired.X);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, fired.Y);
        }

        // Each wrapper maps the drag coordinates its own way; its fixture asserts them.
        [Test]
        public void DidDragEventPropagatesFromBannerAd()
        {
            AssertForwarded(nameof(IBannerAd.DidDrag), () =>
            {
                NativeBanner.TestOnDragBegin(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                NativeBanner.TestOnDrag(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
            });
            NativeBanner.TestOnDragEnd(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
        }

        [Test]
        public void DidEndDragEventPropagatesFromBannerAd()
        {
            var fired = AssertForwarded(nameof(IBannerAd.DidEndDrag),
                () => NativeBanner.TestOnDragEnd(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise));

            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, fired.X);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, fired.Y);
        }

        [Test]
        public void DidDragWithoutDragBeginIsDroppedAndLogged()
        {
            var initialLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;
            var events = new List<WrapperEvent>();
            try
            {
                Inject(NativeBanner);
                LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.DragWithoutBegin));

                using (RecordEvents(events))
                    NativeBanner.TestOnDrag(TestConstants.Dimensions.TestNativeX1, TestConstants.Dimensions.TestNativeY1);

                Assert.IsEmpty(events, "DidDrag fired without a preceding DidBeginDrag");
            }
            finally
            {
                LogController.LoggingLevel = initialLevel;
            }
        }

        // The banner the wrapper creates on first use is subscribed like an injected one.
        [Test]
        public void TheLazilyCreatedBannerForwardsItsEvents()
        {
            _ = Request;
            var banner = NativeBannerAd as BannerAdBase;
            Assert.IsNotNull(banner, "reading the request didn't create a banner");
            var events = new List<WrapperEvent>();

            using (RecordEvents(events))
            {
                banner.OnWillAppear();
                banner.OnClick();
                banner.OnRecordImpression();
                banner.OnDragBegin(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
                banner.OnDrag(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);
                banner.OnDragEnd(TestConstants.Dimensions.Zero, TestConstants.Dimensions.Zero);
            }

            CollectionAssert.AreEqual(
                new[] { nameof(IBannerAd.WillAppear), nameof(IBannerAd.DidClick), nameof(IBannerAd.DidRecordImpression), nameof(IBannerAd.DidBeginDrag), nameof(IBannerAd.DidDrag), nameof(IBannerAd.DidEndDrag) },
                events.Select(e => e.Name));
            Assert.IsTrue(events.All(e => ReferenceEquals(e.Sender, Wrapper)), "an event was sent by something other than the wrapper");
        }

        // Injects the native banner, fires through it, and asserts the wrapper re-raised exactly that event as itself.
        private WrapperEvent AssertForwarded(string eventName, Action fire)
        {
            Inject(NativeBanner);
            var events = new List<WrapperEvent>();

            using (RecordEvents(events))
                fire();

            var fired = events.Where(e => e.Name == eventName).ToList();
            Assert.AreEqual(1, fired.Count, $"{eventName} wasn't forwarded exactly once");
            Assert.AreSame(Wrapper, fired[0].Sender);
            return fired[0];
        }

        #endregion

        /// <summary>
        /// One wrapper event, named after its <see cref="IBannerAd"/> counterpart.
        /// </summary>
        protected sealed class WrapperEvent
        {
            public readonly string Name;
            public readonly object Sender;
            public readonly float X;
            public readonly float Y;

            public WrapperEvent(string name, object sender, float x = 0, float y = 0)
            {
                Name = name;
                Sender = sender;
                X = x;
                Y = y;
            }
        }

        /// <summary>
        /// Runs an action on dispose; RecordEvents returns one to unsubscribe.
        /// </summary>
        protected sealed class Subscription : IDisposable
        {
            private Action _unsubscribe;

            public Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;

            public void Dispose()
            {
                _unsubscribe?.Invoke();
                _unsubscribe = null;
            }
        }
    }
}
