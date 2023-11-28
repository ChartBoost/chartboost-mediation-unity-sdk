/// <summary>
/// A structure used to conveniently pass configuration data into an
/// ad controller.
/// </summary>
public struct AdControllerConfiguration
{
    /// <summary>
    /// The placement name for the ad that will be controlled.
    /// </summary>
    public string placementName;

    /// <summary>
    /// The parent list controller that may be communicated with.
    /// </summary>
    public PlacementListController parentListController;

    /// <summary>
    /// The AdLoadType to control reusability of Ad.
    /// </summary>
    public AdLoadType loadType;

    /// <summary>
    /// The Chartboost Mediation API used by this controller 
    /// </summary>
    public ChartboostMediationAPI chartboostMediationAPI;
}
