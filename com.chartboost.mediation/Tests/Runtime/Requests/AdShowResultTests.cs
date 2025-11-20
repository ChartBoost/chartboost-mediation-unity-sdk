using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Requests
{
    public class AdShowResultTests
    {

        [Test]
        public void ConstructorForSuccessfulShowInitializesCorrectly()
        {
            var metrics = new Metrics();
            var result = new AdShowResult(metrics);

            Assert.IsNotNull(result.Metrics);
            Assert.IsNull(result.Error);
        }

        [Test]
        public void ConstructorForSuccessfulShowWithErrorInitializesCorrectly()
        {
            var metrics = new Metrics();
            var error = new ChartboostMediationError(TestConstants.Errors.CodeShow, TestConstants.Errors.Message);
            var result = new AdShowResult(metrics, error);

            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeShow, result.Error.Value.Code);
            Assert.AreEqual(TestConstants.Errors.Message, result.Error.Value.Message);
        }

        [Test]
        public void ConstructorForFailedShowInitializesCorrectly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.CodeShow, TestConstants.Errors.Message);
            var result = new AdShowResult(error);

            Assert.IsNull(result.Metrics);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeShow, result.Error.Value.Code);
            Assert.AreEqual(TestConstants.Errors.Message, result.Error.Value.Message);
        }

        [Test]
        public void ConstructorWithNullMetricsInitializesCorrectly()
        {
            var result = new AdShowResult(null);

            Assert.IsNull(result.Metrics);
            Assert.IsNull(result.Error);
        }

        [Test]
        public void ConstructorWithNullMetricsAndErrorInitializesCorrectly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.CodeShow, TestConstants.Errors.Message);
            var result = new AdShowResult(null, error);

            Assert.IsNull(result.Metrics);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeShow, result.Error.Value.Code);
        }

        [Test]
        public void ConstructorForFailedShowWithEmptyErrorMessageInitializesCorrectly()
        {
            var error = new ChartboostMediationError(string.Empty);
            var result = new AdShowResult(error);

            Assert.IsNull(result.Metrics);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(string.Empty, result.Error.Value.Message);
        }

        [Test]
        public void MetricsPropertyIsReadOnly()
        {
            var metrics = new Metrics();
            var result = new AdShowResult(metrics);

            var retrievedMetrics = result.Metrics;
            Assert.IsNotNull(retrievedMetrics);
        }

        [Test]
        public void ErrorPropertyIsReadOnly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.Message);
            var result = new AdShowResult(error);

            var retrievedError = result.Error;
            Assert.IsNotNull(retrievedError);
        }
    }
}
