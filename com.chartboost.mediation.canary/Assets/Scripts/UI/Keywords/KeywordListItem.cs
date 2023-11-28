using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An item for the placement keyword list scroll view. Each item represents
/// a single keyword for a particular placement.
/// </summary>
public class KeywordListItem : MonoBehaviour
{
    /// <summary>
    /// The name of the keyword.
    /// </summary>
    public Text keywordName;

    /// <summary>
    /// The value for the keyword.
    /// </summary>
    public Text keywordValue;

    private IKeywordListItemListener _listener;
    private Keyword _keyword;

    /// <summary>
    /// Configure this keyword list item.
    /// </summary>
    /// <param name="keyword">The keyword that it represents.</param>
    /// <param name="listener">The action listener.</param>
    public void Configure(Keyword keyword, IKeywordListItemListener listener)
    {
        _keyword = keyword;
        _listener = listener;

        keywordName.text = keyword.name;
        keywordName.gameObject.name = keyword.name;
        keywordValue.text = keyword.value;
    }

    /// <summary>
    /// Handler to respond to the pushing of the delete button.
    /// </summary>
    public void OnDeleteButtonPushed()
    {
        _listener.DidRequestKeywordDeletion(_keyword);
    }

    /// <summary>
    /// Handler to respond to the pushing of the edit button.
    /// </summary>
    public void OnEditButtonPushed()
    {
        _listener.DidRequestKeywordEdit(_keyword);
    }
}
