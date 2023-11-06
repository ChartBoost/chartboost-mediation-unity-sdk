using System.Collections.Generic;

namespace Chartboost.Consent
{
    public class PartnerConsentUnsupported : IPartnerConsent
    {
        private Dictionary<string, bool> _partnerConsent = new Dictionary<string, bool>();

        public IReadOnlyDictionary<string, bool> GetPartnerIdToConsentGivenDictionaryCopy()
        {
            return new Dictionary<string, bool>(_partnerConsent);
        }

        public void SetPartnerConsent(string partnerId, bool consentGiven)
        {
            if (string.IsNullOrEmpty(partnerId))
                return;
            
            _partnerConsent[partnerId] = consentGiven;
        }

        public void AddPartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary)
        {
            foreach (var kvp in partnerIdToConsentGivenDictionary)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    continue;
                _partnerConsent[kvp.Key] = kvp.Value;
            }
        }

        public void ReplacePartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary)
        {
            _partnerConsent = (Dictionary<string, bool>)partnerIdToConsentGivenDictionary;
        }

        public void ClearConsents()
        {
            _partnerConsent.Clear();
        }

        public bool? RemovePartnerConsent(string partnerId)
        {
            if (!_partnerConsent.ContainsKey(partnerId))
                return null;

            var value = _partnerConsent[partnerId];
            _partnerConsent.Remove(partnerId);
            return value;
        }
    }
}
