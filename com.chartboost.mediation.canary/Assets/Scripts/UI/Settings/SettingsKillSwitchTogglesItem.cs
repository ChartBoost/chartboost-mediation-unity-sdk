using UnityEngine;
using UnityEngine.UI;

public class SettingsKillSwitchTogglesItem : MonoBehaviour
{
    public Toggle noneToggle;
    public Toggle[] partnerToggles;

    private void Start()
    {
        noneToggle.onValueChanged.AddListener(delegate {
            OnNoneChanged(noneToggle);
        });
        
        foreach (var partnerToggle in partnerToggles)
        {
            var isOn = ChartboostMediationPartnerSkipper.SkippedPartners.Contains(partnerToggle.name);
            partnerToggle.SetIsOnWithoutNotify(isOn);
            partnerToggle.onValueChanged.AddListener(delegate {
                OnPartnerChanged(partnerToggle);
            });

            if (isOn)
                noneToggle.isOn = false;
        }
    }

    private void OnNoneChanged(Toggle myToggle)
    {
        if (!myToggle.isOn)
            return;
        
        foreach (var toggle in partnerToggles)
            toggle.isOn = false;

        ChartboostMediationPartnerSkipper.SkippedPartners.Clear();
    }

    private void OnPartnerChanged(Toggle toggle)
    {
        var partnerName = toggle.name;
        if (toggle.isOn)
            noneToggle.isOn = false;
        
        ChartboostMediationPartnerSkipper.SkipPartnerInitialization(partnerName, toggle.isOn);
    }

    
}
