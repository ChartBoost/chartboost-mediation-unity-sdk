using UnityEngine;
using UnityEngine.UI;

public class SettingsKillSwitchTogglesItem : MonoBehaviour
{
    private const string PartnerKillPpKeyBase = "com.chartboost.mediation.canary.partnerKill";
    
    public Toggle noneToggle;

    public Toggle[] partnerToggles;

    private void Awake()
    {
        ChartboostMediationInitializer.Instance.InitializationOptions.Clear();
        
        // Note : If this is done in `Start()` then there is a chance that these toggles
        // may not be applied during automatic initialization since automatic initialization 
        // is also called from Start
        foreach (var partnerToggle in partnerToggles)
        {
            partnerToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt($"{PartnerKillPpKeyBase}.{partnerToggle.name}", 0) == 1);
            SetInitializationOptions(partnerToggle.name, partnerToggle.isOn);
        }
    }

    private void Start()
    {
        noneToggle.onValueChanged.AddListener(delegate {
            OnNoneChanged(noneToggle);
        });
        
        foreach (var partnerToggle in partnerToggles)
        {
            partnerToggle.onValueChanged.AddListener(delegate {
                OnPartnerChanged(partnerToggle);
            });
        }
    }

    private void OnNoneChanged(Toggle myToggle)
    {
        if (!myToggle.isOn)
            return;
        
        foreach (var toggle in partnerToggles)
            toggle.isOn = false;

        ChartboostMediationInitializer.Instance.InitializationOptions.Clear();
    }

    private void OnPartnerChanged(Toggle toggle)
    {
        var partnerName = toggle.name;
        SetInitializationOptions(partnerName, toggle.isOn);
    }

    private void SetInitializationOptions(string partnerName, bool shouldKill)
    {
        var initializationOptions = ChartboostMediationInitializer.Instance.InitializationOptions;
        if (shouldKill)
        {
            if (!initializationOptions.Contains(partnerName))
                initializationOptions.Add(partnerName);
            noneToggle.isOn = false;
        }
        else
        {
            if (initializationOptions.Contains(partnerName))
                initializationOptions.Remove(partnerName);
        }

        PlayerPrefs.SetInt($"{PartnerKillPpKeyBase}.{partnerName}", shouldKill ? 1 : 0);
    }
}
