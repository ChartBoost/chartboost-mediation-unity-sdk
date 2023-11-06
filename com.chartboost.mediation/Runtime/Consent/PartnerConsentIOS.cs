#if UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Chartboost.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Chartboost.Consent
{
    public class PartnerConsentIOS : IPartnerConsent
    {
        /// <inheritdoc cref="GetPartnerIdToConsentGivenDictionaryCopy"/>
        public IReadOnlyDictionary<string, bool> GetPartnerIdToConsentGivenDictionaryCopy()
        {
            var jsonDictionary = _chartboostMediationGetPartnerConsentDictionary();
            return JsonConvert.DeserializeObject<Dictionary<string, bool>>(jsonDictionary);
        }

        /// <inheritdoc cref="SetPartnerConsent"/>
        public void SetPartnerConsent(string partnerId, bool consentGiven)
        {
            _chartboostMediationSetPartnerConsent(partnerId, consentGiven);
        }

        /// <inheritdoc cref="SetPartnerConsents"/>
        public void AddPartnerConsents([CanBeNull] IDictionary<string, bool> partnerIdToConsentGivenDictionary)
        {
            partnerIdToConsentGivenDictionary ??= new Dictionary<string, bool>();
            var jsonConversion = JsonConvert.SerializeObject(partnerIdToConsentGivenDictionary);
            _chartboostMediationAddPartnerConsents(jsonConversion);
        }

        public void ReplacePartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary)
        {
            partnerIdToConsentGivenDictionary ??= new Dictionary<string, bool>();
            var jsonConversion = JsonConvert.SerializeObject(partnerIdToConsentGivenDictionary);
            _chartboostMediationReplacePartnerConsents(jsonConversion);
        }

        public void ClearConsents()
        {
            _chartboostMediationClearConsents();
        }

        /// <inheritdoc cref="RemovePartnerConsent"/>
        public bool? RemovePartnerConsent(string partnerId)
        {
            var value = _chartboostMediationRemovePartnerConsent(partnerId);
            if (value == -1)
                return null;
            return value != 0;
        }

        [DllImport(IOSConstants.Internal)] private static extern string _chartboostMediationGetPartnerConsentDictionary();
        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationSetPartnerConsent(string partnerIdentifier, bool consentGiven);
        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationAddPartnerConsents(string partnerConsentJson);
        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationReplacePartnerConsents(string partnerConsentJson);
        [DllImport(IOSConstants.Internal)] private static extern void _chartboostMediationClearConsents();
        [DllImport(IOSConstants.Internal)] private static extern int _chartboostMediationRemovePartnerConsent(string partnerIdentifier);
    }
}
#endif
