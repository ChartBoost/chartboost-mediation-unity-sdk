using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An item for the placement list scroll view. Each item represents
/// a single placement for a particular placement type.
/// </summary>
public class PlacementListItem : MonoBehaviour
{
    /// <summary>
    /// The button to be triggered by users
    /// </summary>
    public Button button;
    
    /// <summary>
    /// The headline text for the item.
    /// </summary>
    public Text headline;

    /// <summary>
    /// The subheadline text for the item.
    /// </summary>
    public Text subheadline;

    /// <summary>
    /// Standard game object `Start` handler.
    /// </summary>
    private void Start()
    {
        subheadline.gameObject.SetActive(subheadline.text != "sub headline");
    }

    /// <summary>
    /// Setter for setting the headline text to a specific value.
    /// </summary>
    /// <param name="text">The string to show in the headline.</param>
    public void SetHeadline(string text)
    {
        headline.text = text;
        headline.gameObject.name = $"placement:{text}";
    }

    /// <summary>
    /// Setter for setting the subheadline text to a specific value.
    /// </summary>
    /// <param name="text">The string to show in the subheadline.</param>
    public void SetSubheadline(string text)
    {
        subheadline.text = text;
        subheadline.gameObject.SetActive(text.Length != 0);
    }
}
