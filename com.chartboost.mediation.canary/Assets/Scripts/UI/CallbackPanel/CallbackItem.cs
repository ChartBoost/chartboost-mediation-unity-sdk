using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Callback UI element that can be either in Inactive, Successful or Error state
/// </summary>
public class CallbackItem : MonoBehaviour
{
    [Header("Config")]
    public Image Background;
    public Text Text;
    public Image Icon;
    public Sprite SuccessIcon;
    public Sprite ErrorIcon;

    [SerializeField]
    private Color _successColor;
    [SerializeField]
    private Color _errorColor;
    [SerializeField]
    private Color _inActiveColor;

    /// <summary>
    /// Sets this callback in inactive state
    /// </summary>
    public void SetInactive()
    {
        Text.color = _inActiveColor;
        Background.color = _inActiveColor;
        Icon.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets this callback in successful state
    /// </summary>
    public void SetSuccessful()
    {
        SetSuccess(true);
    }

    /// <summary>
    /// Sets this callback in error state
    /// </summary>
    public void SetInError()
    {
        SetSuccess(false);
    }

    private void SetSuccess(bool success)
    {
        Text.color = success ? _successColor : _errorColor;
        Background.color = success ? _successColor : _errorColor;

        Icon.gameObject.SetActive(true);
        Icon.sprite = success ? SuccessIcon : ErrorIcon;
    }
}
