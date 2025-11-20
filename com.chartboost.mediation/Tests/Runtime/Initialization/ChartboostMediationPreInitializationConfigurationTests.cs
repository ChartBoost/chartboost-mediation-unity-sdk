using System.Collections.Generic;
using Chartboost.Mediation.Initialization;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Initialization
{
    public class ChartboostMediationPreInitializationConfigurationTests
    {

        [Test]
        public void ConstructorWithNullInitializesEmptyHashSet()
        {
            var config = new ChartboostMediationPreInitializationConfiguration(null);

            Assert.IsNotNull(config.SkippablePartnerIds);
            Assert.AreEqual(0, config.SkippablePartnerIds.Count);
        }

        [Test]
        public void ConstructorWithEmptyHashSetInitializesCorrectly()
        {
            var skippablePartners = new HashSet<string>();
            var config = new ChartboostMediationPreInitializationConfiguration(skippablePartners);

            Assert.IsNotNull(config.SkippablePartnerIds);
            Assert.AreEqual(0, config.SkippablePartnerIds.Count);
        }

        [Test]
        public void ConstructorWithSinglePartnerInitializesCorrectly()
        {
            var skippablePartners = new HashSet<string> { TestConstants.Partners.Id1 };
            var config = new ChartboostMediationPreInitializationConfiguration(skippablePartners);

            Assert.IsNotNull(config.SkippablePartnerIds);
            Assert.AreEqual(1, config.SkippablePartnerIds.Count);
            Assert.IsTrue(config.SkippablePartnerIds.Contains(TestConstants.Partners.Id1));
        }

        [Test]
        public void ConstructorWithMultiplePartnersInitializesCorrectly()
        {
            var skippablePartners = new HashSet<string> { TestConstants.Partners.Id1, TestConstants.Partners.Id2, TestConstants.Partners.Id3 };
            var config = new ChartboostMediationPreInitializationConfiguration(skippablePartners);

            Assert.IsNotNull(config.SkippablePartnerIds);
            Assert.AreEqual(3, config.SkippablePartnerIds.Count);
            Assert.IsTrue(config.SkippablePartnerIds.Contains(TestConstants.Partners.Id1));
            Assert.IsTrue(config.SkippablePartnerIds.Contains(TestConstants.Partners.Id2));
            Assert.IsTrue(config.SkippablePartnerIds.Contains(TestConstants.Partners.Id3));
        }

        [Test]
        public void ConstructorPreservesOriginalHashSet()
        {
            var skippablePartners = new HashSet<string> { TestConstants.Partners.Id1, TestConstants.Partners.Id2 };
            var config = new ChartboostMediationPreInitializationConfiguration(skippablePartners);

            Assert.AreEqual(skippablePartners, config.SkippablePartnerIds);
        }

        [Test]
        public void StructsWithSameValuesAreEqual()
        {
            var skippablePartners1 = new HashSet<string> { TestConstants.Partners.Id1, TestConstants.Partners.Id2 };
            var skippablePartners2 = new HashSet<string> { TestConstants.Partners.Id1, TestConstants.Partners.Id2 };

            var config1 = new ChartboostMediationPreInitializationConfiguration(skippablePartners1);
            var config2 = new ChartboostMediationPreInitializationConfiguration(skippablePartners2);

            Assert.AreEqual(config1.SkippablePartnerIds.Count, config2.SkippablePartnerIds.Count);
        }

        [Test]
        public void StructsWithDifferentValuesAreNotEqual()
        {
            var skippablePartners1 = new HashSet<string> { TestConstants.Partners.Id1 };
            var skippablePartners2 = new HashSet<string> { TestConstants.Partners.Id2 };

            var config1 = new ChartboostMediationPreInitializationConfiguration(skippablePartners1);
            var config2 = new ChartboostMediationPreInitializationConfiguration(skippablePartners2);

            Assert.AreNotEqual(config1.SkippablePartnerIds, config2.SkippablePartnerIds);
        }

        [Test]
        public void ConstructorWithNullVsEmptyHashSetBothCreateEmptySet()
        {
            var config1 = new ChartboostMediationPreInitializationConfiguration(null);
            var config2 = new ChartboostMediationPreInitializationConfiguration(new HashSet<string>());

            Assert.AreEqual(config1.SkippablePartnerIds.Count, config2.SkippablePartnerIds.Count);
            Assert.AreEqual(0, config1.SkippablePartnerIds.Count);
        }
    }
}
