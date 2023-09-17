namespace Chartboost.Utilities
{
    public static class Constants
    {
        /// <summary>
        /// Multiplication factor that can be applied to native unit (dips in Android and points in iOS) to get the value in pixels.
        /// </summary>
        public static readonly float EditorUIScaleFactor = 2.5f;
        
        public static class BannerSize
        {
            public static readonly (float, float) STANDARD = (320, 50);
            public static readonly (float, float) MEDIUM = (300, 250);
            public static readonly (float, float) LEADERBOARD = (728, 90);
        }
    }
}
