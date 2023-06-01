using UnityEngine;

namespace Chartboost.Utilities
{
    /// <summary>
    /// Chartboost Mediation utility logger which accounts for ChartboostMediationSettings.
    /// </summary>
    public static class Logger
    {
        public static void Log(string tag, string message)
        {
            if (ChartboostMediationSettings.IsLoggingEnabled)
                Debug.Log( $"{tag}/{message}");
        }

        public static void LogError(string tag, string error)
        {
            if (ChartboostMediationSettings.IsLoggingEnabled)
                Debug.Log( $"{tag}/{error}");
        }
    }
}
