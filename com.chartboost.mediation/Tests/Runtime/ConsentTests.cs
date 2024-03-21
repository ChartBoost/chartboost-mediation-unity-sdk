using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Chartboost.Tests.Runtime
{
    public class ConsentTests
    {
        private readonly Dictionary<string, bool> _emptyDictionary = new Dictionary<string, bool>();

        [Test, Order(0)]
        public void ResetConsent()
        {
            ClearConsent();
        }

        [Test, Order(1)]
        public void SetPartnerConsent()
        {
            const bool unityConsentGiven = true;
            ChartboostMediation.PartnerConsents.SetPartnerConsent(Partners.Unity, unityConsentGiven);
            Debug.Log($"Setting Unity Consent to : {unityConsentGiven}");
            var consentCopy = ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy();
            if (consentCopy.TryGetValue(Partners.Unity, out var value))
            {
                Debug.Log($"Found Unity Consent value : {value}");
                Assert.AreEqual(value,unityConsentGiven);
            }
            else
                Assert.Fail();
        }

        [Test, Order(2)]
        public void AddPartnerConsent()
        {
            var testConsents = new Dictionary<string, bool>
            {
                { Partners.AdColony, true },
                { Partners.Vungle, false },
                { Partners.Unity, true }
            };
            ChartboostMediation.PartnerConsents.AddPartnerConsents(testConsents);
            
            var consentCopy = ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy();
            
            Assert.AreEqual(testConsents.Count, consentCopy.Count);
            foreach (var kvp in testConsents)
            {
                Debug.Log($"Found Consent Key: {kvp.Key} with Value: {kvp.Value}");
                if (consentCopy.TryGetValue(kvp.Key, out var consentGiven)) 
                    Assert.AreEqual(consentGiven, kvp.Value);
            }
        }

        [Test, Order(3)]
        public void RemovePartnerConsent()
        {
            var unityConsentValue = ChartboostMediation.PartnerConsents.RemovePartnerConsent(Partners.Unity);
            Assert.AreEqual(unityConsentValue, true);

            var vungleConsentValue = ChartboostMediation.PartnerConsents.RemovePartnerConsent(Partners.Vungle);
            Assert.AreEqual(vungleConsentValue, false);

            var metaConsentValue = ChartboostMediation.PartnerConsents.RemovePartnerConsent(Partners.MetaAudienceNetwork);
            Assert.IsNull(metaConsentValue);
        }
        
        [Test, Order(4)]
        public void ClearConsent()
        {
            ChartboostMediation.PartnerConsents.ClearConsents();
            Assert.AreEqual(_emptyDictionary, ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy());
        }
    }
}
