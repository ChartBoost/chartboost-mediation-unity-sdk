namespace Chartboost.Mediation.Error
{
    internal static class Errors 
    {
        public const string ErrorNotReady = "Chartboost Mediation is not ready or placement is invalid.";
        public const string UnsupportedPlatform = "Unsupported platform, unable to load ads.";
        public const string InvalidAdError = " Ad placement is not valid, reference should be disposed.";
        public const string InitializationError = "Chartboost Mediation is only supported in Android & iOS platforms.";
    }
}
