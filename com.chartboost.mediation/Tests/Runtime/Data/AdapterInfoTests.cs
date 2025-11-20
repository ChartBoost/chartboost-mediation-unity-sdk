using Chartboost.Mediation.Data;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Data
{
    public class AdapterInfoTests
    {
        [Test]
        public void ConstructorInitializesAllFields()
        {
            var adapterInfo = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, TestConstants.Partners.Generic);

            Assert.AreEqual(TestConstants.Versions.Adapter, adapterInfo.AdapterVersion);
            Assert.AreEqual(TestConstants.Versions.Partner, adapterInfo.PartnerVersion);
            Assert.AreEqual(TestConstants.Partners.Standard, adapterInfo.PartnerIdentifier);
            Assert.AreEqual(TestConstants.Partners.Generic, adapterInfo.PartnerDisplayName);
        }

        [Test]
        public void ConstructorHandlesNullValues()
        {
            var adapterInfo = new AdapterInfo(null, null, null, null);

            Assert.IsNull(adapterInfo.AdapterVersion);
            Assert.IsNull(adapterInfo.PartnerVersion);
            Assert.IsNull(adapterInfo.PartnerIdentifier);
            Assert.IsNull(adapterInfo.PartnerDisplayName);
        }

        [Test]
        public void ConstructorHandlesEmptyStrings()
        {
            var adapterInfo = new AdapterInfo(string.Empty, string.Empty, string.Empty, string.Empty);

            Assert.AreEqual(string.Empty, adapterInfo.AdapterVersion);
            Assert.AreEqual(string.Empty, adapterInfo.PartnerVersion);
            Assert.AreEqual(string.Empty, adapterInfo.PartnerIdentifier);
            Assert.AreEqual(string.Empty, adapterInfo.PartnerDisplayName);
        }

        [Test]
        public void StructsWithSameValuesAreEqual()
        {
            var info1 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, TestConstants.Partners.Generic);
            var info2 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, TestConstants.Partners.Generic);

            Assert.AreEqual(info1, info2);
        }

        [Test]
        public void StructsWithDifferentAdapterVersionsAreNotEqual()
        {
            var info1 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, TestConstants.Partners.Generic);
            var info2 = new AdapterInfo(TestConstants.Versions.PartnerAdapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, TestConstants.Partners.Generic);

            Assert.AreNotEqual(info1, info2);
        }

        [Test]
        public void StructsWithDifferentPartnerVersionsAreNotEqual()
        {
            var info1 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, TestConstants.Partners.Generic);
            var info2 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.PartnerAdapter, TestConstants.Partners.Standard, TestConstants.Partners.Generic);

            Assert.AreNotEqual(info1, info2);
        }

        [Test]
        public void StructsWithDifferentPartnerIdentifiersAreNotEqual()
        {
            var info1 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Partner1, TestConstants.Partners.Generic);
            var info2 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Partner2, TestConstants.Partners.Generic);

            Assert.AreNotEqual(info1, info2);
        }

        [Test]
        public void StructsWithDifferentPartnerDisplayNamesAreNotEqual()
        {
            var info1 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, "Partner Name 1");
            var info2 = new AdapterInfo(TestConstants.Versions.Adapter, TestConstants.Versions.Partner, TestConstants.Partners.Standard, "Partner Name 2");

            Assert.AreNotEqual(info1, info2);
        }
    }
}
