using System;
using System.Collections;
using System.Collections.Generic;
using Chartboost.Banners;
using Chartboost.Caching;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    /// <summary>How a platform Mediation banner maps its public API onto the shared banner container.</summary>
    public class NativeBannerAdBaseTests
    {
        private RecordingPlatform _platform;
        private TestNativeBanner _banner;

        [SetUp]
        public void SetUp()
        {
            _platform = new RecordingPlatform();
            _banner = new TestNativeBanner(new IntPtr(TestConstants.UniqueIds.NativeBanner));
        }

        [TearDown]
        public void TearDown()
        {
            _banner.Dispose();
            AdCache<IAd>.ReleaseAd(TestConstants.UniqueIds.NativeBanner);
        }

        private void UseContainer()
        {
            _banner.UseContainer(new NativeBannerContainer(_platform));
            _platform.Calls.Clear();
        }

        [Test]
        public void TheContainerReceivesStateSetBeforeIt()
        {
            _banner.Position = new Vector2(10, 20);
            _banner.Pivot = new Vector2(0.5f, 1);
            _banner.Visible = false;
            _banner.Draggable = true;

            _banner.UseContainer(new NativeBannerContainer(_platform));

            CollectionAssert.AreEqual(new[]
            {
                $"SetAbsolute:{new Vector2(10, 20)}/{new Vector2(0.5f, 1)}", "SetVisible:False", "SetDraggable:True"
            }, _platform.Calls);
        }

        [Test]
        public void PositionAndPivotPlaceTheContainerOnlyWhenTheyChange()
        {
            UseContainer();

            _banner.Position = new Vector2(5, 6);
            _banner.Position = new Vector2(5, 6);
            _banner.Pivot = new Vector2(1, 1);

            CollectionAssert.AreEqual(new[]
            {
                $"SetAbsolute:{new Vector2(5, 6)}/{Vector2.zero}", $"SetAbsolute:{new Vector2(5, 6)}/{Vector2.one}"
            }, _platform.Calls);
            Assert.AreEqual(new Vector2(5, 6), _banner.Position);
            Assert.AreEqual(Vector2.one, _banner.Pivot);
        }

        [Test]
        public void MatchAdWrapsAndOverflowsEvenAsTheFirstSize()
        {
            UseContainer();

            _banner.ContainerSize = ContainerSize.WrapContent();

            // The container already wraps by default, so only the overflow is new.
            CollectionAssert.AreEqual(new[] { "SetClipContent:False" }, _platform.Calls);
            Assert.AreEqual(ContainerSize.WrapContent(), _banner.ContentSizes[0]);
        }

        [Test]
        public void AFixedSizeClipsAndSizesTheContent()
        {
            UseContainer();
            _banner.ContainerSize = ContainerSize.WrapHorizontal(50);

            _banner.ContainerSize = ContainerSize.FixedSize(320, 50);

            CollectionAssert.AreEqual(new[] { "SetSize:-1x50", "SetClipContent:False", "SetSize:320x50", "SetClipContent:True" }, _platform.Calls);
            Assert.AreEqual(ContainerSize.FixedSize(320, 50), _banner.ContentSizes[1]);
        }

        [Test]
        public void AWrappingAxisReadsBackAsTheCreativeSize()
        {
            UseContainer();
            _banner.Creative = BannerSize.Adaptive(728, 90);

            _banner.ContainerSize = ContainerSize.WrapVertical(300);

            Assert.AreEqual(new ContainerSize(300, 90), _banner.ContainerSize);
        }

        [Test]
        public void ADropStoresTheDraggedPositionAndReportsPixels()
        {
            UseContainer();
            var drops = new List<(float x, float y)>();
            _banner.DidEndDrag += (_, x, y) => drops.Add((x, y));

            _banner.TestContainer.OnDragEnded(new Vector2(200, 400), new Vector2(100, 200));

            Assert.AreEqual(new Vector2(100, 200), _banner.Position);
            CollectionAssert.AreEqual(new[] { (200f, 400f) }, drops);
        }

        [Test]
        public void LoadAttachesTheContainer()
        {
            UseContainer();

            _ = _banner.Load(new BannerAdLoadRequest("placement", BannerSize.Standard));

            CollectionAssert.Contains(_platform.Calls, "Attach");
        }

        [Test]
        public void HostingGoesToTheContainer()
        {
            UseContainer();

            _banner.SetHost(NativeView.FromPointer(new IntPtr(42)));

            CollectionAssert.AreEqual(new[] { "SetHost:42" }, _platform.Calls);
        }

        [Test]
        public void DisposeReleasesTheContainerBeforeTheNativeAd()
        {
            UseContainer();
            var order = new List<string>();
            _platform.OnDispose = () => order.Add("container");
            _banner.OnDestroyNative = () => order.Add("native");

            _banner.Dispose();

            CollectionAssert.AreEqual(new[] { "container", "native" }, order);
        }

        [UnityTest]
        public IEnumerator TheFinalizerPathReleasesTheContainerOnTheMainThread()
        {
            UseContainer();

            _banner.DisposeFromFinalizer();
            CollectionAssert.DoesNotContain(_platform.Calls, "Dispose", "never on the finalizer thread");
            yield return null;

            CollectionAssert.Contains(_platform.Calls, "Dispose");
        }

        // Android: the container's AndroidJavaObject is finalized with the banner, so only the native destroy runs.
        [UnityTest]
        public IEnumerator TheFinalizerPathSkipsAContainerThatIsNotSafeToFinalize()
        {
            UseContainer();
            _platform.FinalizerSafe = false;
            var destroyed = false;
            _banner.OnDestroyNative = () => destroyed = true;

            _banner.DisposeFromFinalizer();
            yield return null;

            Assert.IsTrue(destroyed);
            CollectionAssert.DoesNotContain(_platform.Calls, "Dispose");
        }

        private sealed class TestNativeBanner : NativeBannerAdBase
        {
            public TestNativeBanner(IntPtr id) : base(id) { }

            public readonly List<ContainerSize> ContentSizes = new();
            public Action OnDestroyNative;
            public BannerSize? Creative;

            public void UseContainer(NativeBannerContainer container) => Container = container;
            public NativeBannerContainer TestContainer => Container;
            public void DisposeFromFinalizer() => Dispose(false);

            protected override void SetContentSize(ContainerSize size) => ContentSizes.Add(size);
            protected override void DestroyNative(bool disposing) => OnDestroyNative?.Invoke();

            public override IReadOnlyDictionary<string, string> Keywords { get; set; }
            public override IReadOnlyDictionary<string, string> PartnerSettings { get; set; }
            public override BannerSize? BannerSize => Creative;
            public override BannerAdLoadRequest Request => _request;
            public override BidInfo? WinningBidInfo => null;
            public override string LoadId => null;
            public override Metrics? LoadMetrics => null;
            public override Chartboost.Mediation.Ad.Banner.Enums.BannerHorizontalAlignment HorizontalAlignment { get; set; }
            public override Chartboost.Mediation.Ad.Banner.Enums.BannerVerticalAlignment VerticalAlignment { get; set; }
        }

        private sealed class RecordingPlatform : IBannerContainerPlatform
        {
            private static int _nextId = 9000;
            public int Id { get; } = ++_nextId;
            public bool FinalizerSafe = true;
            public bool CanDisposeFromFinalizer => FinalizerSafe;
            public List<string> Calls { get; } = new();
            public Action OnDispose;

            public void SetAnchor(BannerAnchor anchor, bool useSafeArea) => Calls.Add($"SetAnchor:{anchor}");
            public void SetAbsolute(Vector2 position, Vector2 pivot) => Calls.Add($"SetAbsolute:{position}/{pivot}");
            public void SetSize(float width, float height) => Calls.Add($"SetSize:{width}x{height}");
            public void SetAlignment(BannerHorizontalAlignment horizontal, BannerVerticalAlignment vertical) => Calls.Add("SetAlignment");
            public void SetBackgroundColor(Color color) => Calls.Add("SetBackgroundColor");
            public void SetClipContent(bool clip) => Calls.Add($"SetClipContent:{clip}");
            public void SetVisible(bool visible) => Calls.Add($"SetVisible:{visible}");
            public void SetInteractionEnabled(bool enabled) => Calls.Add($"SetInteractionEnabled:{enabled}");
            public void SetDraggable(bool draggable) => Calls.Add($"SetDraggable:{draggable}");
            public void SetHost(NativeView host) => Calls.Add(host.IsNone ? "SetHost:none" : $"SetHost:{host.Pointer}");
            public void Attach() => Calls.Add("Attach");
            public void Detach() => Calls.Add("Detach");
            public void Dispose() { Calls.Add("Dispose"); OnDispose?.Invoke(); }
        }
    }
}
