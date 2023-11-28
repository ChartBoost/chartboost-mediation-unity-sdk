using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ButtonPushedDelegate(int id);

/// <summary>
/// The UI for a row that contains a label with a button.
/// </summary>
public class SettingsButtonItem : MonoBehaviour
{
    /// <summary>
    /// An numeric identifier for the button.
    /// </summary>
    public int id = 1;

    public Text titleLabel;

    public Button button;

    public ButtonPushedDelegate pushedDelegate;

    public void OnButtonPushed()
    {
        pushedDelegate(id);
    }
}
