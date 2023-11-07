using System.Collections.Generic;

namespace Chartboost.Consent
{
    public interface IPartnerConsent
    {
        /// <summary>
        /// Gets a copy of the partner ID to consent given <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        IReadOnlyDictionary<string, bool> GetPartnerIdToConsentGivenDictionaryCopy();
        
        /// <summary>
        /// Set a single partner's consent. This value is persisted between app launches.
        /// </summary>
        /// <param name="partnerId">The partner Identifier.</param>
        /// <param name="consentGiven">True if there is consent for this partner, false otherwise.</param>
        void SetPartnerConsent(string partnerId, bool consentGiven);
        
        /// <summary>
        /// Adds a map of partner identifiers to consent given. These values are persisted between app launches.
        /// This is only additive. If you'd like to remove a consent for a particular partner, please use <see cref="RemovePartnerConsent"/>.
        /// </summary>
        /// <param name="partnerIdToConsentGivenDictionary">The <see cref="Dictionary{TKey,TValue}"/> of partner identifiers to consent given.</param>
        void AddPartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary);

        /// <summary>
        /// Clears and adds a <see cref="Dictionary{TKey,TValue}"/> of partner identifiers to consent given. These values are persisted between app launches.
        /// </summary>
        /// <param name="partnerIdToConsentGivenDictionary"></param>
        void ReplacePartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary);

        /// <summary>
        /// Clears all partner consents. This change will persist to disk.
        /// </summary>
        void ClearConsents();
        
        /// <summary>
        /// Remove a partner consent.
        /// </summary>
        /// <param name="partnerId">The partner Identifier.</param>
        /// <returns>The previous consent state of this partner. Null if not set.</returns>
        bool? RemovePartnerConsent(string partnerId);
    }
}
