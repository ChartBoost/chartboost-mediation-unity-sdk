using System.Globalization;
using System.Threading;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Utilities
{
    public class NativeResultsTests
    {
        // HB-12131: iOS needed both a code and a message, so a code alone was treated as success.
        [TestCase("CM_LOAD_FAILURE", "message")]
        [TestCase("CM_LOAD_FAILURE", "")]
        [TestCase("CM_LOAD_FAILURE", null)]
        [TestCase("", "message")]
        public void ACodeOrAMessageIsAnError(string code, string message)
        {
            var error = NativeResults.ToError(code, message);

            Assert.IsTrue(error.HasValue);
            Assert.AreEqual(code, error.Value.Code);
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        public void NoCodeAndNoMessageIsSuccess(string code, string message)
            => Assert.IsNull(NativeResults.ToError(code, message));

        // HB-12132: parsing used the device locale, so "1.5" failed under a decimal-comma locale.
        [TestCase("de-DE")]
        [TestCase("en-US")]
        public void PriceParsesTheSameUnderAnyLocale(string culture)
        {
            var original = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            try
            {
                Assert.IsTrue(NativeResults.TryParsePrice("1.5", out var price));
                Assert.AreEqual(1.5, price);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("not-a-price")]
        public void UnparseablePriceFails(string price)
            => Assert.IsFalse(NativeResults.TryParsePrice(price, out _));
    }
}
