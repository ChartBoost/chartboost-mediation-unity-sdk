using System;

/// <summary>
/// All of the keywords associated with a specific placement.
/// </summary>
[Serializable]
public struct Keywords
{
    /// <summary>
    /// The placement that these keywords belongs to.
    /// </summary>
    public string placementName;

    /// <summary>
    /// The keywords defined for the placement.
    /// </summary>
    public Keyword[] keywords;
}
