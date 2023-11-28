using System;
using System.Collections.Generic;
using Chartboost;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class PartnerConsentHandler : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;

    [SerializeField] private Toggle toggleNotSet;
    [SerializeField] private Toggle toggleGiven;
    [SerializeField] private Toggle toggleDenied;

    [SerializeField] private Text label;

    private string _partnerIdentifier;
    
    public void Initialize(string partnerId)
    {
        _partnerIdentifier = partnerId;
        toggleNotSet.onValueChanged.AddListener(RemoveConsent);
        toggleGiven.onValueChanged.AddListener(GiveConsent);
        toggleDenied.onValueChanged.AddListener(DenyConsent);

        var currentConsents = ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy();

        if (currentConsents.TryGetValue(partnerId, out var value))
        {
            if (value)
                toggleGiven.isOn = true;
            else
                toggleDenied.isOn = true;
        }
        else
            toggleNotSet.isOn = true;
        
        toggleGroup.RegisterToggle(toggleNotSet);
        toggleGroup.RegisterToggle(toggleGiven);
        toggleGroup.RegisterToggle(toggleDenied);

        label.text = _partnerIdentifier;
    }

    private void RemoveConsent(bool currentValue)
    {
        if (!currentValue)
            return;
        ChartboostMediation.PartnerConsents.RemovePartnerConsent(_partnerIdentifier);
        LogPartnerConsent();
    }

    private void GiveConsent(bool currentValue) 
        => SetConsent(currentValue, ChartboostMediation.PartnerConsents.SetPartnerConsent, true);

    private void DenyConsent(bool currentValue) 
        => SetConsent(currentValue, ChartboostMediation.PartnerConsents.SetPartnerConsent, false);

    private void SetConsent(bool isToggled, Action<string, bool> setPartnerConsent, bool value)
    {
        if (!isToggled)
            return;
        setPartnerConsent(_partnerIdentifier, value);
        LogPartnerConsent();
    }

    private void LogPartnerConsent()
    {
       Debug.Log($"Per Partner Consent: {JsonConvert.SerializeObject(ChartboostMediation.PartnerConsents.GetPartnerIdToConsentGivenDictionaryCopy())}"); 
    }
}
