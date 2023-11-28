using System;
using System.Collections;

/// <summary>
/// A single keyword that can be defined for a placement.
/// </summary>
[Serializable]
public struct Keyword
{
    /// <summary>
    /// The name of the keyword.
    /// </summary>
    public string name;

    /// <summary>
    /// The value for the keyword.
    /// </summary>
    public string value;
}

/// <summary>
/// A comparer for keywords in order to sort a list of keywords by
/// the name value.
/// </summary>
public class KeywordComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer()).Compare(((Keyword)x).name, ((Keyword)y).name);
    }
}
