namespace Chartboost.Mediation.iOS
{
    /// <summary>
    /// Extern delegate definition for simple <see cref="string"/> data events.
    /// </summary>
    internal delegate void ExternChartboostMediationDataEvent(string data);

    /// <summary>
    /// Extern delegate definition for ILRD events.
    /// </summary>
    internal delegate void ExternChartboostMediationImpressionLevelRevenueDataEvent(int hashCode, string impressionDataJson);
}
