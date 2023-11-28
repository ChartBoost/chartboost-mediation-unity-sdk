using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// A keywords data source that is backed by Unity PlayerPrefs.
/// </summary>
public class KeywordsPlayerPrefsDataSource : IKeywordsDataSource
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="placementName">The placement name that this datasource
    /// managed keywords for.</param>
    public KeywordsPlayerPrefsDataSource(string placementName)
    {
        PlacementName = placementName;
    }

    /// <summary>
    /// The placement name bound with this data source.
    /// </summary>
    public string PlacementName { get; private set; }

    /// <summary>
    /// Flag indicating that the data source is currently fetching keyword data.
    /// </summary>
    public bool IsFetching { get; private set; } = false;

    /// <summary>
    /// The keywords associated with the placement.
    /// </summary>
    public Keyword[] Keywords { get; private set; } = new Keyword[0];

    /// <summary>
    /// Fetch the keywords.
    /// If a fetch is already in progress when another fetch is called, nothing
    /// will be done.
    /// </summary>
    public void FetchKeywords(KeywordFetchCompletionDelegate fetchCompletionEvent)
    {
        if (IsFetching)
            return;
        IsFetching = true;
        var keywordsJSONString = PlayerPrefs.GetString($"keywords:{PlacementName}");
        if (!string.IsNullOrEmpty(keywordsJSONString))
        {
            var keywords = JsonConvert.DeserializeObject<Keywords>(keywordsJSONString);
            Keywords = keywords.keywords ?? Array.Empty<Keyword>();
        }
        IsFetching = false;
        fetchCompletionEvent(null);
    }

    /// <summary>
    /// Create a keyword.
    /// </summary>
    /// <param name="name">The name of the keyword.</param>
    /// <param name="value">The value of the keyword.</param>
    /// <returns>Returns the created keyword.</returns>
    public Keyword CreateKeyword(string name, string value)
    {
        // existing
        var keywordsCount = Keywords.Length;
        for (int i = 0; i < keywordsCount; i++)
        {
            var keyword = Keywords[i];
            if (keyword.name != name)
                continue;
            return UpdateExistingKeyword(keyword, value, i);
        }

        // new
        return CreateNewKeyword(name, value);
    }

    private Keyword UpdateExistingKeyword(Keyword keyword, string value, int atIndex)
    {
        keyword.value = value;
        var keywordsArray = new List<Keyword>(Keywords);
        keywordsArray.RemoveAt(atIndex);
        keywordsArray.Add(keyword);
        Keywords = keywordsArray.ToArray();
        PersistKeywords();
        return keyword;
    }

    private Keyword CreateNewKeyword(string name, string value)
    {
        var newKeyword = new Keyword
        {
            name = name,
            value = value
        };
        var keywordsArray = new List<Keyword>(Keywords)
        {
            newKeyword
        };
        Keywords = keywordsArray.ToArray();
        PersistKeywords();
        return newKeyword;
    }

    /// <summary>
    /// Delete an existing keyword.
    /// </summary>
    /// <param name="keyword">The keyword to delete.</param>
    /// <returns>Returns true if successful, otherwise false.</returns>
    public bool Delete(Keyword keyword)
    {
        var result = false;
        var keywordsCount = Keywords.Length;
        for (int i = 0; i < keywordsCount; i++)
        {
            var existingKeyword = Keywords[i];
            if (existingKeyword.name != keyword.name)
                continue;

            var keywordsArray = new List<Keyword>(Keywords);
            keywordsArray.RemoveAt(i);
            Keywords = keywordsArray.ToArray();
            PersistKeywords();
            result = true;
            break;
        }
        return result;
    }

    private void PersistKeywords()
    {
        var keywords = new Keywords
        {
            placementName = PlacementName,
            keywords = Keywords
        };
        var keywordsJSONString = JsonConvert.SerializeObject(keywords);
        PlayerPrefs.SetString($"keywords:{PlacementName}", keywordsJSONString);
    }
}
