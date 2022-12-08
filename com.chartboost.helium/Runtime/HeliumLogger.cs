using UnityEngine;

namespace Helium
{
    /// <summary>
    /// Helium utility logger which accounts for HeliumSettings.
    /// </summary>
    public static class HeliumLogger
    {
        public static void Log(string tag, string message)
        {
            if (HeliumSettings.IsLoggingEnabled)
                Debug.Log( $"{tag}/{message}");
        }

        public static void LogError(string tag, string error)
        {
            if (HeliumSettings.IsLoggingEnabled)
                Debug.Log( $"{tag}/{error}");
        }
    }
}
