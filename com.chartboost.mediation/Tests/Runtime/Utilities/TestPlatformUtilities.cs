using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Utility methods for platform detection in tests.
    /// </summary>
    public static class TestPlatformUtilities
    {
        /// <summary>
        /// Checks if the current platform is a native mobile platform (Android or iOS).
        /// Used to determine if native banner ad tests should run.
        /// </summary>
        /// <returns>True if running on Android or iOS, false otherwise.</returns>
        public static bool IsNativePlatform()
        {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }

        /// <summary>
        /// Checks if the current platform is Android.
        /// </summary>
        /// <returns>True if running on Android, false otherwise.</returns>
        public static bool IsAndroid()
        {
            return Application.platform == RuntimePlatform.Android;
        }

        /// <summary>
        /// Checks if the current platform is iOS.
        /// </summary>
        /// <returns>True if running on iOS, false otherwise.</returns>
        public static bool IsIOS()
        {
            return Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }
}
