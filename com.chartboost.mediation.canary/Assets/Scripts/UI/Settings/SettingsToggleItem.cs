using UnityEngine;
using UnityEngine.UI;

public delegate void ToggleChangedDelegate(bool isOn);

/// <summary>
/// The UI for a row that contains a toggle.
/// </summary>
public class SettingsToggleItem : MonoBehaviour
{
    public Toggle toggle;

    public ToggleChangedDelegate changedDelegate;

    public void OnToggleChanged()
    {
        changedDelegate(toggle.isOn);
    }
}
