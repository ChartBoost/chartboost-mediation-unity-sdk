using Chartboost.Mediation.Error;
using NUnit.Framework;
// ReSharper disable AssignNullToNotNullAttribute

namespace Chartboost.Tests.Runtime.Error
{
    public class ChartboostMediationErrorTests
    {
        private const string AlternativeCode = "CODE";
        private const string AlternativeMessage = "Message";

        [Test]
        public void ConstructorWithMessageOnlyInitializesCorrectly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.Message);

            Assert.AreEqual(TestConstants.Errors.Message, error.Message);
            Assert.IsNull(error.Code);
        }

        [Test]
        public void ConstructorWithCodeAndMessageInitializesCorrectly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.Code, TestConstants.Errors.Message);

            Assert.AreEqual(TestConstants.Errors.Code, error.Code);
            Assert.AreEqual(TestConstants.Errors.Message, error.Message);
        }

        [Test]
        public void ConstructorWithNullMessageHandlesCorrectly()
        {
            var error = new ChartboostMediationError(null);

            Assert.IsNull(error.Message);
        }

        [Test]
        public void ConstructorWithNullCodeAndMessageHandlesCorrectly()
        {
            var error = new ChartboostMediationError(null, null);

            Assert.IsNull(error.Code);
            Assert.IsNull(error.Message);
        }

        [Test]
        public void ConstructorWithEmptyStringsHandlesCorrectly()
        {
            var error = new ChartboostMediationError(string.Empty, string.Empty);

            Assert.AreEqual(string.Empty, error.Code);
            Assert.AreEqual(string.Empty, error.Message);
        }

        [Test]
        public void ErrorsWithSameValuesAreEqual()
        {
            var error1 = new ChartboostMediationError(AlternativeCode, AlternativeMessage);
            var error2 = new ChartboostMediationError(AlternativeCode, AlternativeMessage);

            Assert.AreEqual(error1, error2);
        }

        [Test]
        public void ErrorsWithDifferentCodesAreNotEqual()
        {
            var error1 = new ChartboostMediationError("CODE1", AlternativeMessage);
            var error2 = new ChartboostMediationError("CODE2", AlternativeMessage);

            Assert.AreNotEqual(error1, error2);
        }

        [Test]
        public void ErrorsWithDifferentMessagesAreNotEqual()
        {
            var error1 = new ChartboostMediationError(AlternativeCode, "Message1");
            var error2 = new ChartboostMediationError(AlternativeCode, "Message2");

            Assert.AreNotEqual(error1, error2);
        }

        [Test]
        public void ErrorWithCodeVsErrorWithoutCodeAreNotEqual()
        {
            var error1 = new ChartboostMediationError(AlternativeMessage);
            var error2 = new ChartboostMediationError(AlternativeCode, AlternativeMessage);

            Assert.AreNotEqual(error1, error2);
        }
    }
}
