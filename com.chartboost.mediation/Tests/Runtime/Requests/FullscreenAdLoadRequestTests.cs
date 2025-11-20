using System.Collections.Generic;
using Chartboost.Mediation.Requests;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Requests
{
    public class FullscreenAdLoadRequestTests
    {

        [Test]
        public void ConstructorWithPlacementOnlyInitializesCorrectly()
        {
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.IsNotNull(request.Keywords);
            Assert.AreEqual(0, request.Keywords.Count);
            Assert.IsNull(request.PartnerSettings);
        }

        [Test]
        public void ConstructorWithKeywordsInitializesCorrectly()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.KeyAlt, TestConstants.Keywords.ValueAlt } };
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, keywords);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.IsNotNull(request.Keywords);
            Assert.AreEqual(1, request.Keywords.Count);
            Assert.AreEqual(TestConstants.Keywords.ValueAlt, request.Keywords[TestConstants.Keywords.KeyAlt]);
        }

        [Test]
        public void ConstructorWithPartnerSettingsInitializesCorrectly()
        {
            var partnerSettings = new Dictionary<string, string> { { TestConstants.Keywords.SettingKey, TestConstants.Keywords.SettingValue } };
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, null, partnerSettings);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.IsNotNull(request.Keywords);
            Assert.AreEqual(0, request.Keywords.Count);
            Assert.IsNotNull(request.PartnerSettings);
            Assert.AreEqual(1, request.PartnerSettings.Count);
            Assert.AreEqual(TestConstants.Keywords.SettingValue, request.PartnerSettings[TestConstants.Keywords.SettingKey]);
        }

        [Test]
        public void ConstructorWithAllParametersInitializesCorrectly()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.KeyAlt, TestConstants.Keywords.ValueAlt } };
            var partnerSettings = new Dictionary<string, string> { { TestConstants.Keywords.SettingKey, TestConstants.Keywords.SettingValue } };
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, keywords, partnerSettings);

            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
            Assert.AreEqual(1, request.Keywords.Count);
            Assert.AreEqual(TestConstants.Keywords.ValueAlt, request.Keywords[TestConstants.Keywords.KeyAlt]);
            Assert.AreEqual(1, request.PartnerSettings.Count);
            Assert.AreEqual(TestConstants.Keywords.SettingValue, request.PartnerSettings[TestConstants.Keywords.SettingKey]);
        }

        [Test]
        public void ConstructorWithNullPlacementHandlesCorrectly()
        {
            var request = new FullscreenAdLoadRequest(null);

            Assert.IsNull(request.PlacementName);
            Assert.IsNotNull(request.Keywords);
            Assert.IsNull(request.PartnerSettings);
        }

        [Test]
        public void ConstructorWithEmptyPlacementHandlesCorrectly()
        {
            var request = new FullscreenAdLoadRequest(string.Empty);

            Assert.AreEqual(string.Empty, request.PlacementName);
            Assert.IsNotNull(request.Keywords);
        }

        [Test]
        public void ConstructorWithNullKeywordsInitializesEmptyDictionary()
        {
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard);

            Assert.IsNotNull(request.Keywords);
            Assert.AreEqual(0, request.Keywords.Count);
        }

        [Test]
        public void ConstructorWithEmptyKeywordsDictionaryInitializesCorrectly()
        {
            var keywords = new Dictionary<string, string>();
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, keywords);

            Assert.IsNotNull(request.Keywords);
            Assert.AreEqual(0, request.Keywords.Count);
        }

        [Test]
        public void ConstructorWithNullPartnerSettingsInitializesCorrectly()
        {
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard);

            Assert.IsNull(request.PartnerSettings);
        }

        [Test]
        public void KeywordsAreReadOnly()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.KeyAlt, TestConstants.Keywords.ValueAlt } };
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, keywords);

            Assert.IsInstanceOf<IReadOnlyDictionary<string, string>>(request.Keywords);
        }

        [Test]
        public void PartnerSettingsAreReadOnly()
        {
            var partnerSettings = new Dictionary<string, string> { { TestConstants.Keywords.SettingKey, TestConstants.Keywords.SettingValue } };
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, null, partnerSettings);

            Assert.IsInstanceOf<IReadOnlyDictionary<string, string>>(request.PartnerSettings);
        }
    }
}
