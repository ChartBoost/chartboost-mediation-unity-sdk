using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Serialization;
using UnityEngine.Video;

/// <summary>
/// A structure that represents a single placement, as defined by a placement
/// data source coming from the Helium backend.
/// </summary>
[Serializable]
public struct Placement
{
    /// <summary>
    /// The string that defines the placement name.
    /// </summary>
    [JsonProperty("helium_placement")]
    public string placement;

    /// <summary>
    /// The string that defines the placement type.
    /// </summary>
    public string type;

    /// <summary>
    /// Banners may have an automatic refresh rate defined.
    /// </summary>
    [JsonProperty("auto_refresh_rate")]
    public int autoRefreshRate;

    /// <summary>
    /// A strongly typed enumeration for the string that stores the
    /// `type` value provided by the backend.
    /// </summary>
    public PlacementType TypeAsEnum
    {
        get
        {
            return type switch
            {
                "banner" => PlacementType.Banner,
                "adaptive_banner" => PlacementType.Banner,
                "interstitial" => PlacementType.Interstitial,
                "rewarded" => PlacementType.Rewarded,
                "rewarded_interstitial" => PlacementType.Fullscreen,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected placement type value: {type}")
            };
        }
    }

    public string PlacementLowerCase => placement.ToLower();
}

/// <summary>
/// A comparer for Placements in order to sort a list of placements by
/// the `helium_placement` value.
/// </summary>
public class PlacementComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (new CaseInsensitiveComparer()).Compare(((Placement)x).placement, ((Placement)y).placement);
    }
}
