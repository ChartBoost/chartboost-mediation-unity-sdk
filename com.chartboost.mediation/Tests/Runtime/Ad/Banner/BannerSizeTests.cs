using System;
using System.Collections.Generic;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class BannerSizeTests
    {
        private const string ExpectedExceptionMessage = "Cannot create fixed size banner for size type Adaptive";
        private const float Width = TestConstants.Dimensions.StandardWidth;
        private const float Height = TestConstants.Dimensions.StandardHeight;

        private static IEnumerable<TestCaseData> SizeCases()
        {
            yield return Fixed(BannerSize.Standard, Width, Height, BannerSizeType.Standard).SetName("StandardSizeHasCorrectDimensions");
            yield return Fixed(BannerSize.MediumRect, TestConstants.Dimensions.MediumWidth, TestConstants.Dimensions.MediumHeight, BannerSizeType.Medium).SetName("MediumRectSizeHasCorrectDimensions");
            yield return Fixed(BannerSize.Leaderboard, TestConstants.Dimensions.LeaderboardWidth, TestConstants.Dimensions.LeaderboardHeight, BannerSizeType.Leaderboard).SetName("LeaderboardSizeHasCorrectDimensions");
            yield return Adaptive(BannerSize.Adaptive(Width), 0f).SetName("AdaptiveWithWidthOnlyHasZeroHeight");
            yield return Adaptive(BannerSize.Adaptive(Width, Height), Height).SetName("AdaptiveWithWidthAndHeightInitializesCorrectly");
            yield return Adaptive(BannerSize.Adaptive2X1(Width), Width / 2f).SetName("Adaptive2X1HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive4X1(Width), Width / 4f).SetName("Adaptive4X1HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive6X1(Width), Width / 6f).SetName("Adaptive6X1HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive8X1(Width), Width / 8f).SetName("Adaptive8X1HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive10X1(Width), Width / 10f).SetName("Adaptive10X1HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive1X2(Width), Width * 2f).SetName("Adaptive1X2HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive1X3(Width), Width * 3f).SetName("Adaptive1X3HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive1X4(Width), Width * 4f).SetName("Adaptive1X4HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive9X16(Width), Width * 16f / 9f).SetName("Adaptive9X16HasCorrectAspectRatio");
            yield return Adaptive(BannerSize.Adaptive1X1(Width), Width).SetName("Adaptive1X1HasCorrectAspectRatio");

            static TestCaseData Fixed(BannerSize size, float width, float height, BannerSizeType sizeType)
                => new(size, width, height, sizeType, BannerType.Fixed);

            static TestCaseData Adaptive(BannerSize size, float height)
                => new(size, Width, height, BannerSizeType.Adaptive, BannerType.Adaptive);
        }

        private static IEnumerable<TestCaseData> AspectRatioCases()
        {
            yield return new TestCaseData(BannerSize.Adaptive(Width, Height), Width / Height).SetName("AspectRatioIsCalculatedCorrectly");
            yield return new TestCaseData(BannerSize.Adaptive(0f, Height), 0f).SetName("AspectRatioIsZeroWhenWidthIsZero");
            yield return new TestCaseData(BannerSize.Adaptive(Width), 0f).SetName("AspectRatioIsZeroWhenHeightIsZero");
        }

        [TestCaseSource(nameof(SizeCases))]
        public void SizeHasExpectedDimensionsAndTypes(BannerSize size, float width, float height, BannerSizeType sizeType, BannerType bannerType)
        {
            Assert.AreEqual(width, size.Width);
            Assert.AreEqual(height, size.Height);
            Assert.AreEqual(sizeType, size.SizeType);
            Assert.AreEqual(bannerType, size.BannerType);
        }

        [TestCaseSource(nameof(AspectRatioCases))]
        public void AspectRatioMatchesDimensions(BannerSize size, float expectedAspectRatio)
        {
            Assert.AreEqual(expectedAspectRatio, size.AspectRatio);
        }

        [Test]
        public void GetFixedTypeAdThrowsExceptionForAdaptiveSizeType()
        {
            var exception = Assert.Throws<Exception>(() => BannerSize.GetFixedTypeAd(BannerSizeType.Adaptive));

            Assert.That(exception.Message, Is.EqualTo(ExpectedExceptionMessage));
        }
    }
}
