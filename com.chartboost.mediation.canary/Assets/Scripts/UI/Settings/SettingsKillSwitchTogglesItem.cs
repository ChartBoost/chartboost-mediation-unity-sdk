using UnityEngine;
using UnityEngine.UI;

public class SettingsKillSwitchTogglesItem : MonoBehaviour
{
    public Toggle noneToggle;

    public Toggle[] partnerToggles;

    private void Start()
    {
        ChartboostMediationInitializer.Instance.InitializationOptions.Clear();
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

        if (toggle.isOn)
        {
            ChartboostMediationInitializer.Instance.InitializationOptions.Remove(partnerName);
            noneToggle.isOn = false;
        }

        if (!ChartboostMediationInitializer.Instance.InitializationOptions.Contains(partnerName))
            ChartboostMediationInitializer.Instance.InitializationOptions.Add(partnerName);
    }
}
