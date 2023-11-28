using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The user interface controller that lists out all currently defined keywords
/// for a specific placement.
/// </summary>
public class KeywordsController : MonoBehaviour, IKeywordListItemListener
{
    /// <summary>
    /// The `ScrollRect` game object in the transform hierarchy that is the scroll view for this UI.
    /// </summary>
    public ScrollRect scrollView;

    /// <summary>
    /// The `RectTransform` of the game object where scrollable content will be placed under.
    /// </summary>
    public RectTransform scrollViewContent;

    /// <summary>
    /// The prefab to instantiate for each keyword.
    /// </summary>
    public GameObject keywordListPrefab;

    /// <summary>
    /// The keyword edit panel.
    /// </summary>
    public KeywordEditPanel editPanel;

    /// <summary>
    /// The listener for changes for the controller.
    /// </summary>
    private IKeywordsControllerListener _listener;

    /// <summary>
    /// The keywords data source.
    /// Precondition: the data source has already been fetched.
    /// </summary>
    [System.NonSerialized]
    private IKeywordsDataSource _dataSource;

    public void Configure(IKeywordsControllerListener listener, IKeywordsDataSource dataSource)
    {
        _listener = listener;
        _dataSource = dataSource;
    }

    private void OnEnable()
    {
        TabPanelController.Instance.ToggleSearchBarVisibility(false);
    }

    /// <summary>
    /// Standard game object `Start` handler.
    /// </summary>
    private void Start()
    {
        editPanel.OnDisable();
        if (_dataSource == null)
        {
            System.Console.Write("Error: no data source has been provided.");
            return;
        }
        GenerateListFromKeywords(_dataSource.Keywords);
    }

    /// <summary>
    /// Generate the list of keywords in the UI.
    /// </summary>
    /// <param name="keywords">The list of keywords provided by the data source.</param>
    private void GenerateListFromKeywords(Keyword[] keywords)
    {
        foreach (Transform child in scrollViewContent.transform)
            Destroy(child.gameObject);

        if (keywords.Length == 0)
            return;

        Array.Sort(keywords, new KeywordComparer());

        float yOffset = 0;
        foreach (var keyword in keywords)
        {
            var instance = Instantiate(keywordListPrefab, scrollViewContent, false);
            var rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(x: 0, y: -yOffset, z: 0);
            var rect = rectTransform.rect;
            scrollViewContent.offsetMax += new Vector2(x: 0, y: rect.height);
            yOffset += rect.height;

            KeywordListItem item = instance.GetComponent<KeywordListItem>();
            item.Configure(keyword, this);
        }

        scrollView.normalizedPosition = new Vector2(x: 0, y: 1);
    }

    /// <summary>
    /// Handler for the back button. Communicate to the parent ad
    /// controller to remove this UI from the object hierarchy.
    /// </summary>
    public void OnBackButtonPushed()
    {
        _listener.KeywordsControllerDidRequestDestroy();
    }

    /// <summary>
    /// Handler for responding to pushing the add keyword button.
    /// </summary>
    public void OnAddButtonPushed()
    {
        editPanel.OnEnable();
    }

    /// <summary>
    /// Request that the keyword be deleted.
    /// </summary>
    /// <param name="keyword">The keyword to delete.</param>
    public void DidRequestKeywordDeletion(Keyword keyword)
    {
        _dataSource.Delete(keyword);
        GenerateListFromKeywords(_dataSource.Keywords);
        _listener.KeywordsControllerDidUpdateKeywords(_dataSource.Keywords);
    }

    /// <summary>
    /// Request that the keyword be edited.
    /// </summary>
    /// <param name="keyword">The keyword to edit.</param>
    public void DidRequestKeywordEdit(Keyword keyword)
    {
        editPanel.OnEnable();
        editPanel.PopulateWithKeyword(keyword);
    }

    /// <summary>
    /// Handler for when the cancel button on the editor is pushed.
    /// </summary>
    public void EditPanelCancelButtonPushed()
    {
        editPanel.OnDisable();
    }

    /// <summary>
    /// Handler for when the confirmation button on the editor is pushed.
    /// </summary>
    public void EditPanelConfirmationButtonPushed()
    {
        editPanel.OnDisable();
        var name = editPanel.nameInputField.text ?? "";
        var value = editPanel.valueInputField.text ?? "";
        if (name.Length == 0 || value.Length == 0)
            return;
        _dataSource.CreateKeyword(name, value);
        GenerateListFromKeywords(_dataSource.Keywords);
        _listener.KeywordsControllerDidUpdateKeywords(_dataSource.Keywords);
    }

    /// <summary>
    /// Handler for when the value in the editor name field changes.
    /// </summary>
    public void OnEditPanelNameInputChanged()
    {
        editPanel.OnNameInputChanged();
    }

    /// <summary>
    /// Handler for when the value in the editor value field changes.
    /// </summary>
    public void OnEditPanelValueInputChanged()
    {
        editPanel.OnValueInputChanged();
    }
}
