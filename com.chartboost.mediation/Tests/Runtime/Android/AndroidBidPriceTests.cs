using System.Text.RegularExpressions;
using Chartboost.Logging;
using Chartboost.Mediation.Android.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Android
{
    /// <summary>
    /// The winning-bid price parsing behind the Android bridge's BidInfo; Editor-only.
    /// </summary>
    public class AndroidBidPriceTests
    {
        private int _errors;

        private void CountErrors(string message, LogLevel level)
        {
            if (level == LogLevel.Error)
                _errors++;
        }

        [SetUp]
        public void SetUp()
        {
            _errors = 0;
            LogController.MessageLogged += CountErrors;
        }

        [TearDown]
        public void TearDown() => LogController.MessageLogged -= CountErrors;

        [TestCase("1.25", 1.25)]
        [TestCase("0", 0d)]
        public void APriceIsParsedWithoutAnError(string price, double expected)
        {
            Assert.AreEqual(expected, AndroidExtensions.ParseBidPrice(price));
            Assert.AreEqual(0, _errors);
        }

        // A failed load has no winning bid, so no price: that's expected, not an error.
        [TestCase(null)]
        [TestCase("")]
        public void NoPriceIsZeroWithoutAnError(string price)
        {
            Assert.AreEqual(0d, AndroidExtensions.ParseBidPrice(price));
            Assert.AreEqual(0, _errors, "a missing price logged an error");
        }

        [Test]
        public void AnUnreadablePriceIsZeroAndLogsAnError()
        {
            Assert.AreEqual(0d, AndroidExtensions.ParseBidPrice("not-a-price"));
            Assert.AreEqual(1, _errors);
            LogAssert.Expect(LogType.Error, new Regex("not-a-price"));
        }
    }
}
