/// <summary>
/// Async delegate definition for keyword fetch completion.
/// </summary>
/// <param name="error">A string containing a description of an error
/// that occurred during fetching, if any.</param>
public delegate void KeywordFetchCompletionDelegate(string error);

/// <summary>
/// Interface to define the contract for a data source for keywords that
/// belong to a particular placement.
/// </summary>
public interface IKeywordsDataSource
{
    /// <summary>
    /// The placement name bound with this data source.
    /// </summary>
    string PlacementName { get; }

    /// <summary>
    /// Flag indicating that the data source is currently fetching keyword data.
    /// </summary>
    bool IsFetching { get; }

    /// <summary>
    /// The keywords associated with the placement.
    /// </summary>
    Keyword[] Keywords { get; }

    /// <summary>
    /// Fetch the keywords.
    /// If a fetch is already in progress when another fetch is called, nothing
    /// will be done.
    /// </summary>
    void FetchKeywords(KeywordFetchCompletionDelegate fetchCompletionEvent);

    /// <summary>
    /// Create a keyword.
    /// </summary>
    /// <param name="name">The name of the keyword.</param>
    /// <param name="value">The value of the keyword.</param>
    /// <returns>Returns the created keyword.</returns>
    Keyword CreateKeyword(string name, string value);

    /// <summary>
    /// Delete an existing keyword.
    /// </summary>
    /// <param name="keyword">The keyword to delete.</param>
    /// <returns>Returns true if successful, otherwise false.</returns>
    bool Delete(Keyword keyword);
}
