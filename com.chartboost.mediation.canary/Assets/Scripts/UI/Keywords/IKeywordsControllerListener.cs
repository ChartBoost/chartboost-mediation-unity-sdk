/// <summary>
/// The interface for objects that instantiate a keywords controller.
/// </summary>
public interface IKeywordsControllerListener
{
    /// <summary>
    /// The keywords controller wants to be destroyed.
    /// </summary>
    void KeywordsControllerDidRequestDestroy();

    /// <summary>
    /// Keywords within the controller have been updated.
    /// </summary>
    /// <param name="keywords">The updated list of keywords.</param>
    void KeywordsControllerDidUpdateKeywords(Keyword[] keywords);
}
