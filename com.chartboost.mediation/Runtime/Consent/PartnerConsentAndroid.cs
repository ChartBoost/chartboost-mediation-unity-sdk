#if UNITY_ANDROID
using System.Collections.Generic;
using Chartboost.Platforms.Android;
using Chartboost.Utilities;
using UnityEngine;

namespace Chartboost.Consent
{
    public class PartnerConsentAndroid : IPartnerConsent
    {
        private static AndroidJavaObject GetNativePartnerConsents()
        {
            using var native = ChartboostMediationAndroid.GetNativeSDK();
            return native.CallStatic<AndroidJavaObject>(AndroidConstants.FunGetPartnerConsents);
        }

        /// <inheritdoc cref="GetPartnerIdToConsentGivenDictionaryCopy"/>
        public IReadOnlyDictionary<string, bool> GetPartnerIdToConsentGivenDictionaryCopy()
        {
            using var partnerConsents = GetNativePartnerConsents();
            using var consentGivenMapCopy = partnerConsents.Call<AndroidJavaObject>(AndroidConstants.FunGetPartnerIdToConsentGivenMapCopy);
            return ConsentMapToDictionary(consentGivenMapCopy);
        }

        /// <inheritdoc cref="SetPartnerConsent"/>
        public void SetPartnerConsent(string partnerId, bool consentGiven)
        {
            using var partnerConsents = GetNativePartnerConsents();
            partnerConsents.Call(AndroidConstants.FunSetPartnerConsent, partnerId, consentGiven);
        }
        
        /// <inheritdoc cref="AddPartnerConsents"/>
        public void AddPartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary)
        {
            using var consentMap = DictionaryToConsentMap(partnerIdToConsentGivenDictionary);
            using var partnerConsents = GetNativePartnerConsents();
            partnerConsents.Call(AndroidConstants.FunAddPartnerConsents, consentMap);
        }

        /// <inheritdoc cref="ReplacePartnerConsents"/>
        public void ReplacePartnerConsents(IDictionary<string, bool> partnerIdToConsentGivenDictionary)
        {
            using var newConsentMap = DictionaryToConsentMap(partnerIdToConsentGivenDictionary);
            using var partnerConsents = GetNativePartnerConsents();
            partnerConsents.Call(AndroidConstants.FunReplacePartnerConsents, newConsentMap);
        }

        /// <inheritdoc cref="ClearConsents"/>
        public void ClearConsents()
        {
            using var partnerConsents = GetNativePartnerConsents();
            partnerConsents.Call(AndroidConstants.FunClear);
        }

        /// <inheritdoc cref="RemovePartnerConsent"/>
        public bool? RemovePartnerConsent(string partnerId)
        {
            using var partnerConsents = GetNativePartnerConsents();
            var result = partnerConsents.Call<AndroidJavaObject>(AndroidConstants.FunRemovePartnerConsent, partnerId);
            return result?.Call<bool>(AndroidConstants.FunBooleanValue);
        }
        
        /// <summary>
        /// Converts a <see cref="AndroidJavaObject"/> implementing <a href="https://docs.oracle.com/javase/8/docs/api/java/util/Map.html">Java Map Interface</a>, into a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// </summary>
        private static IReadOnlyDictionary<string, bool> ConsentMapToDictionary(AndroidJavaObject source) {
            if (source == null)
                return null;

            var ret = new Dictionary<string, bool>();

            var size = source.Call<int>(AndroidConstants.FunSize);
            if (size == 0)
                return ret;
            
            var entries = source.Call<AndroidJavaObject>(AndroidConstants.FunEntrySet);
            var iter = entries.Call<AndroidJavaObject>(AndroidConstants.FunIterator);

            do {
                var entry = iter.Call<AndroidJavaObject>(AndroidConstants.FunNext);
                var key = entry.Call<string>(AndroidConstants.FunGetKey);
                var value = entry.Call<AndroidJavaObject>(AndroidConstants.FunGetValue);
                ret[key] = value.Call<bool>(AndroidConstants.FunBooleanValue);
            } while (iter.Call<bool>(AndroidConstants.FunHasNext));
            
            return ret;
        }
        
        /// <summary>
        /// Converts a <see cref="IReadOnlyDictionary{TKey,TValue}"/> into a <see cref="AndroidJavaObject"/> implementing <a href="https://docs.oracle.com/javase/8/docs/api/java/util/HashMap.html">Java HashMap</a>.
        /// </summary>
        private static AndroidJavaObject DictionaryToConsentMap(IDictionary<string, bool> source) {
            var map = new AndroidJavaObject(AndroidConstants.ClassHashMap);

            if (source == null || source.Count == 0)
                return map;
            
            foreach (var kv in source)
            {
                var partnerId = kv.Key;
                if (string.IsNullOrEmpty(partnerId))
                    continue;
                using var key = new AndroidJavaObject( AndroidConstants.ClassString, partnerId);
                using var value = new AndroidJavaObject(AndroidConstants.ClassBoolean, kv.Value);
                map.Call<AndroidJavaClass>(AndroidConstants.FunPut, partnerId, value);
            }
            return map;
        }
    }
}
#endif
