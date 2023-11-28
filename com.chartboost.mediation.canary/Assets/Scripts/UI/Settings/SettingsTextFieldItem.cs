using UnityEngine;
using UnityEngine.UI;

public delegate void TextFieldChangedDelegate(string newValue);

/// <summary>
/// The UI for a row that contains a label and a modifiable text field.
/// </summary>
public class SettingsTextFieldItem : MonoBehaviour
{
    public Text titleLabel;

    public InputField inputField;

    public TextFieldChangedDelegate changedDelegate;

    public void OnInputFieldEndEdit()
    {
        changedDelegate(inputField.text);
    }
}
