/// <summary>
/// An interface that defines actions that the keyword list item can
/// request.
/// </summary>
public interface IKeywordListItemListener
{
    /// <summary>
    /// Request that the keyword be deleted.
    /// </summary>
    /// <param name="keyword">The keyword to delete.</param>
    void DidRequestKeywordDeletion(Keyword keyword);

    /// <summary>
    /// Request that the keyword be edited.
    /// </summary>
    /// <param name="keyword">The keyword to edit.</param>
    void DidRequestKeywordEdit(Keyword keyword);
}
