using System.Collections;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Default.Ad.Banner;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class BannerAdDefaultTests
    {

        private BannerAdDefault _bannerAdDefault;

        [SetUp]
        public void SetUp()
        {
            _bannerAdDefault = new BannerAdDefault();
        }

        [TearDown]
        public void TearDown()
        {
            _bannerAdDefault?.Dispose();
        }

        [Test]
        public void ConstructorCreatesInstance()
        {
            Assert.IsNotNull(_bannerAdDefault);
        }

        [UnityTest]
        public IEnumerator LoadReturnsUnsupportedPlatformError()
        {
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);
            var bannerLoadTask = _bannerAdDefault.Load(request);
            yield return bannerLoadTask;

            Assert.IsNotNull(bannerLoadTask.Result);
            Assert.IsNotNull(bannerLoadTask.Result.Error);
            Assert.AreEqual(Errors.UnsupportedPlatform, bannerLoadTask.Result.Error.Value.Message);
        }

        [UnityTest]
        public IEnumerator LoadStoresRequest()
        {
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);
            var bannerLoadTask = _bannerAdDefault.Load(request);
            yield return bannerLoadTask;

            Assert.AreEqual(request, _bannerAdDefault.Request);
        }

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            var keywords = new System.Collections.Generic.Dictionary<string, string> { { "key1", "value1" } };

            _bannerAdDefault.Keywords = keywords;

            Assert.AreSame(keywords, _bannerAdDefault.Keywords);
        }

        [Test]
        public void PartnerSettingsCanBeSetAndRetrieved()
        {
            var settings = new System.Collections.Generic.Dictionary<string, string> { { "setting1", "value1" } };

            _bannerAdDefault.PartnerSettings = settings;

            Assert.AreSame(settings, _bannerAdDefault.PartnerSettings);
        }

        [Test]
        public void PositionCanBeSetAndRetrieved()
        {
            var position = new Vector2(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            _bannerAdDefault.Position = position;

            Assert.AreEqual(position, _bannerAdDefault.Position);
        }

        [Test]
        public void PivotCanBeSetAndRetrieved()
        {
            var pivot = new Vector2(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            _bannerAdDefault.Pivot = pivot;

            Assert.AreEqual(pivot, _bannerAdDefault.Pivot);
        }

        [Test]
        public void VisibleCanBeSetAndRetrieved()
        {
            _bannerAdDefault.Visible = true;

            Assert.IsTrue(_bannerAdDefault.Visible);

            _bannerAdDefault.Visible = false;

            Assert.IsFalse(_bannerAdDefault.Visible);
        }

        [Test]
        public void DraggableCanBeSetAndRetrieved()
        {
            _bannerAdDefault.Draggable = true;

            Assert.IsTrue(_bannerAdDefault.Draggable);

            _bannerAdDefault.Draggable = false;

            Assert.IsFalse(_bannerAdDefault.Draggable);
        }

        [Test]
        public void ContainerSizeCanBeSetAndRetrieved()
        {
            var containerSize = new ContainerSize(320, 50);

            _bannerAdDefault.ContainerSize = containerSize;

            Assert.AreEqual(containerSize, _bannerAdDefault.ContainerSize);
        }

        [Test]
        public void HorizontalAlignmentCanBeSetAndRetrieved()
        {
            _bannerAdDefault.HorizontalAlignment = BannerHorizontalAlignment.Center;

            Assert.AreEqual(BannerHorizontalAlignment.Center, _bannerAdDefault.HorizontalAlignment);

            _bannerAdDefault.HorizontalAlignment = BannerHorizontalAlignment.Right;

            Assert.AreEqual(BannerHorizontalAlignment.Right, _bannerAdDefault.HorizontalAlignment);
        }

        [Test]
        public void VerticalAlignmentCanBeSetAndRetrieved()
        {
            _bannerAdDefault.VerticalAlignment = BannerVerticalAlignment.Top;

            Assert.AreEqual(BannerVerticalAlignment.Top, _bannerAdDefault.VerticalAlignment);

            _bannerAdDefault.VerticalAlignment = BannerVerticalAlignment.Bottom;

            Assert.AreEqual(BannerVerticalAlignment.Bottom, _bannerAdDefault.VerticalAlignment);
        }

        [Test]
        public void BannerSizeReturnsNull()
        {
            Assert.IsNull(_bannerAdDefault.BannerSize);
        }

        [Test]
        public void LoadIdReturnsNull()
        {
            Assert.IsNull(_bannerAdDefault.LoadId);
        }

        [Test]
        public void LoadMetricsReturnsNull()
        {
            Assert.IsNull(_bannerAdDefault.LoadMetrics);
        }

        [Test]
        public void WinningBidInfoReturnsNull()
        {
            Assert.IsNull(_bannerAdDefault.WinningBidInfo);
        }

        [Test]
        public void RequestReturnsNullBeforeLoad()
        {
            Assert.IsNull(_bannerAdDefault.Request);
        }

        [Test]
        public void DisposeDoesNotThrow()
        {
            Assert.DoesNotThrow(() => _bannerAdDefault.Dispose());
        }
    }
}
