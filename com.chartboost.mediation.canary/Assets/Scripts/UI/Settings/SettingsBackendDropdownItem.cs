using UnityEngine;
using UnityEngine.UI;
public delegate void DropdownChangedDelegate(int newIndex);

/// <summary>
/// The UI for a row that contains a dropdown to select the backend endpoint.
/// </summary>
public class SettingsBackendDropdownItem : MonoBehaviour
{
    public Dropdown dropdown;

    public DropdownChangedDelegate changedDelegate;

    public void OnDropdownChanged()
    {
        changedDelegate?.Invoke(dropdown.value);
    }
}
