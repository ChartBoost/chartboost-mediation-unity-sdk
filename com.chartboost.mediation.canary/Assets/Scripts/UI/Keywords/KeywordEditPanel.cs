using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Objects used for the keyword editor.
/// </summary>
[System.Serializable]
public struct KeywordEditPanel
{
    /// <summary>
    /// The keyword edit panel game object.
    /// </summary>
    public GameObject gameObject;

    /// <summary>
    /// Input field for the keyword name.
    /// </summary>
    public InputField nameInputField;

    /// <summary>
    /// Input field for the keyword value.
    /// </summary>
    public InputField valueInputField;

    /// <summary>
    /// Button to save the information in the panel.
    /// </summary>
    public Button confirmationButton;

    /// <summary>
    /// Handler to call to enable/show the panel.
    /// </summary>
    public void OnEnable()
    {
        nameInputField.text = null;
        valueInputField.text = null;
        gameObject.SetActive(true);
        confirmationButton.interactable = false;
    }

    /// <summary>
    /// Handler to call to disable/hide the panel.
    /// </summary>
    public void OnDisable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Populate the panel with the keyword information.
    /// </summary>
    /// <param name="keyword"></param>
    public void PopulateWithKeyword(Keyword keyword)
    {
        nameInputField.text = keyword.name;
        valueInputField.text = keyword.value;
        confirmationButton.interactable = true;
    }

    /// <summary>
    /// Handler to call when the value in the name input field changes.
    /// </summary>
    public void OnNameInputChanged()
    {
        ValidateInputFields();
    }

    /// <summary>
    /// Handler to call when the value in the value input field changes.
    /// </summary>
    public void OnValueInputChanged()
    {
        ValidateInputFields();
    }

    private void ValidateInputFields()
    {
        var name = nameInputField.text ?? "";
        var value = valueInputField.text ?? "";
        confirmationButton.interactable = name.Length > 0 && value.Length > 0;
    }
}
