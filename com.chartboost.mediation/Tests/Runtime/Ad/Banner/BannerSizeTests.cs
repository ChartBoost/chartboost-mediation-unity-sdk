using System;
using System.Reflection;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class BannerSizeTests
    {
        private const string ExpectedExceptionMessage = "Cannot create fixed size banner for size type Adaptive";

        [Test]
        public void StandardSizeHasCorrectDimensions()
        {
            var size = BannerSize.Standard;

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, size.Height);
            Assert.AreEqual(BannerSizeType.Standard, size.SizeType);
            Assert.AreEqual(BannerType.Fixed, size.BannerType);
        }

        [Test]
        public void MediumRectSizeHasCorrectDimensions()
        {
            var size = BannerSize.MediumRect;

            Assert.AreEqual(TestConstants.Dimensions.MediumWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.MediumHeight, size.Height);
            Assert.AreEqual(BannerSizeType.Medium, size.SizeType);
            Assert.AreEqual(BannerType.Fixed, size.BannerType);
        }

        [Test]
        public void LeaderboardSizeHasCorrectDimensions()
        {
            var size = BannerSize.Leaderboard;

            Assert.AreEqual(TestConstants.Dimensions.LeaderboardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.LeaderboardHeight, size.Height);
            Assert.AreEqual(BannerSizeType.Leaderboard, size.SizeType);
            Assert.AreEqual(BannerType.Fixed, size.BannerType);
        }

        [Test]
        public void AdaptiveWithWidthOnlyHasZeroHeight()
        {
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(0f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
            Assert.AreEqual(BannerType.Adaptive, size.BannerType);
        }

        [Test]
        public void AdaptiveWithWidthAndHeightInitializesCorrectly()
        {
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth, TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
            Assert.AreEqual(BannerType.Adaptive, size.BannerType);
        }

        [Test]
        public void Adaptive2X1HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive2X1(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth / 2f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive4X1HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive4X1(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth / 4f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive6X1HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive6X1(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth / 6f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive8X1HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive8X1(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth / 8f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive10X1HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive10X1(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth / 10f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive1X2HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive1X2(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth * 2f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive1X3HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive1X3(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth * 3f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive1X4HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive1X4(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth * 4f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive9X16HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive9X16(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual((TestConstants.Dimensions.StandardWidth * 16f) / 9f, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void Adaptive1X1HasCorrectAspectRatio()
        {
            var size = BannerSize.Adaptive1X1(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, size.Height);
            Assert.AreEqual(BannerSizeType.Adaptive, size.SizeType);
        }

        [Test]
        public void AspectRatioIsCalculatedCorrectly()
        {
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth, TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth / TestConstants.Dimensions.StandardHeight, size.AspectRatio);
        }

        [Test]
        public void AspectRatioIsZeroWhenWidthIsZero()
        {
            var size = BannerSize.Adaptive(0f, TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(0f, size.AspectRatio);
        }

        [Test]
        public void AspectRatioIsZeroWhenHeightIsZero()
        {
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth);

            Assert.AreEqual(0f, size.AspectRatio);
        }

        [Test]
        public void FixedSizesHaveCorrectBannerType()
        {
            Assert.AreEqual(BannerType.Fixed, BannerSize.Standard.BannerType);
            Assert.AreEqual(BannerType.Fixed, BannerSize.MediumRect.BannerType);
            Assert.AreEqual(BannerType.Fixed, BannerSize.Leaderboard.BannerType);
        }

        [Test]
        public void AdaptiveSizesHaveCorrectBannerType()
        {
            var adaptive = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth);
            var adaptiveWithHeight = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth, TestConstants.Dimensions.StandardHeight);

            Assert.AreEqual(BannerType.Adaptive, adaptive.BannerType);
            Assert.AreEqual(BannerType.Adaptive, adaptiveWithHeight.BannerType);
        }

        [Test]
        public void GetFixedTypeAdThrowsExceptionForAdaptiveSizeType()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
            {
                var type = typeof(BannerSize);
                var method = type.GetMethod("GetFixedTypeAd", BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                    method.Invoke(null, new object[] { BannerSizeType.Adaptive });
            });

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOf<Exception>(exception.InnerException);
            Assert.That(exception.InnerException.Message, Is.EqualTo(ExpectedExceptionMessage));
        }
    }
}
