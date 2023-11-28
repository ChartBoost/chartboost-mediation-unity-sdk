using System;
using System.Collections.Generic;

/// <summary>
/// Async delegate definition for server placement fetch completion.
/// </summary>
/// <param name="error">A string containing a description of an error
/// that occurred during fetching, if any.</param>
public delegate void FetchCompletionDelegate(string error);

/// <summary>
/// Contract for implementation of a data source for ad placements.
/// </summary>
public interface IPlacementDataSource
{
    /// <summary>
    /// The placements that were fetched.
    /// </summary>
    List<Placement> Placements { get; }
    
    event Action<List<Placement>> DidUpdateDataSource;

    /// <summary>
    /// Attempt to fetch placements for a specific appId and cache if needed.
    /// </summary>
    /// <param name="appId">Target app id.</param>
    void LoadPlacementCache(string appId);

    /// <summary>
    /// Expires a set of cached placement for an specific appId. Cannot expire default cache.
    /// </summary>
    /// <param name="appId">Target app id.</param>
    void ExpirePlacementCache(string appId);
}
