using System.Collections.Generic;

namespace Chartboost.Consent
{
    public interface IPartnerConsent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, bool> GetPartnerIdToConsentGivenDictionaryCopy();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="consentGiven"></param>
        void SetPartnerConsent(string partnerId, bool consentGiven);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partnerIdToConsentGivenDictionary"></param>
        void SetPartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        bool? RemovePartnerConsent(string partnerId);
    }
}
