using System.Text.RegularExpressions;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Android.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Android
{
    /// <summary>
    /// The pure name/width/height mapping behind the Android bridge's BannerSize; Editor-only.
    /// </summary>
    public class AndroidBannerSizeMappingTests
    {
        private LogLevel _initialLogLevel;

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Warning;
        }

        [TearDown]
        public void TearDown() => LogController.LoggingLevel = _initialLogLevel;

        [TestCase("STANDARD", BannerSizeType.Standard)]
        [TestCase("MEDIUM", BannerSizeType.Medium)]
        [TestCase("LEADERBOARD", BannerSizeType.Leaderboard)]
        public void FixedNamesMapToTheirSizes(string name, BannerSizeType expected)
            => Assert.AreEqual(expected, AndroidExtensions.ToBannerSize(name, 320, 50)?.SizeType);

        [Test]
        public void AdaptiveKeepsItsDimensions()
        {
            var size = AndroidExtensions.ToBannerSize("ADAPTIVE", 400, 100);

            Assert.AreEqual(BannerSizeType.Adaptive, size?.SizeType);
            Assert.AreEqual(400, size?.Width);
            Assert.AreEqual(100, size?.Height);
        }

        // A size type added by a newer native SDK must not crash the BannerSize getter.
        [Test]
        public void AnUnknownNameIsReportedAsUnknownInsteadOfThrowing()
        {
            LogAssert.Expect(LogType.Warning, new Regex("Unknown native banner size 'SKYSCRAPER'"));

            var size = AndroidExtensions.ToBannerSize("SKYSCRAPER", 160, 600);

            Assert.AreEqual(BannerSizeType.Unknown, size?.SizeType);
            Assert.AreEqual(160, size?.Width);
            Assert.AreEqual(600, size?.Height);
        }

        // Pins today's behavior: a 0x0 size is "no size", whatever its name.
        [TestCase("ADAPTIVE")]
        [TestCase("STANDARD")]
        public void AZeroSizeIsNoSize(string name)
            => Assert.IsNull(AndroidExtensions.ToBannerSize(name, 0, 0));
    }
}
