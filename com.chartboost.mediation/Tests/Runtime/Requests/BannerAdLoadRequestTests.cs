using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Requests;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Requests
{
    public class BannerAdLoadRequestTests
    {
        [Test]
        public void ConstructorInitializesCorrectly()
        {
            var size = BannerSize.Standard;
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, size);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.AreEqual(size, request.Size);
        }

        [Test]
        public void ConstructorWithAdaptiveSizeInitializesCorrectly()
        {
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth);
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, size);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.AreEqual(size, request.Size);
        }

        [Test]
        public void ConstructorWithAdaptiveSizeAndHeightInitializesCorrectly()
        {
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth, TestConstants.Dimensions.StandardHeight);
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, size);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.AreEqual(size.Width, request.Size.Width);
            Assert.AreEqual(size.Height, request.Size.Height);
        }

        [Test]
        public void ConstructorWithNullPlacementHandlesCorrectly()
        {
            var size = BannerSize.Standard;
            var request = new BannerAdLoadRequest(null, size);

            Assert.IsNull(request.PlacementName);
            Assert.AreEqual(size, request.Size);
        }

        [Test]
        public void ConstructorWithEmptyPlacementHandlesCorrectly()
        {
            var size = BannerSize.Standard;
            var request = new BannerAdLoadRequest(string.Empty, size);

            Assert.AreEqual(string.Empty, request.PlacementName);
            Assert.AreEqual(size, request.Size);
        }

        [Test]
        public void SizeCanBeModified()
        {
            var initialSize = BannerSize.Standard;
            var newSize = BannerSize.MediumRect;
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, initialSize)
            {
                Size = newSize
            };

            Assert.AreEqual(newSize, request.Size);
        }

        [Test]
        public void ConstructorWithStandardSizeInitializesCorrectly()
        {
            var size = BannerSize.Standard;
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, size);

            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, request.Size.Width);
            Assert.AreEqual(TestConstants.Dimensions.StandardHeight, request.Size.Height);
        }

        [Test]
        public void ConstructorWithMediumRectSizeInitializesCorrectly()
        {
            var size = BannerSize.MediumRect;
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, size);

            Assert.AreEqual(TestConstants.Dimensions.MediumWidth, request.Size.Width);
            Assert.AreEqual(TestConstants.Dimensions.MediumHeight, request.Size.Height);
        }

        [Test]
        public void ConstructorWithLeaderboardSizeInitializesCorrectly()
        {
            var size = BannerSize.Leaderboard;
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, size);

            Assert.AreEqual(TestConstants.Dimensions.LeaderboardWidth, request.Size.Width);
            Assert.AreEqual(TestConstants.Dimensions.LeaderboardHeight, request.Size.Height);
        }
    }
}
