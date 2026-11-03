namespace Chartboost.Mediation.Error
{
    internal static class Errors
    {
        public const string ErrorNotReady = "Chartboost Mediation is not ready or placement is invalid.";
        public const string UnsupportedPlatform = "Unsupported platform, unable to load ads.";
        public const string InvalidAdError = " Ad placement is not valid, reference should be disposed.";
        public const string InitializationError = "Chartboost Mediation is only supported in Android & iOS platforms.";
        public const string ShowFailureUnknown = "Failed to show ad for unknown reason.";
        public const string ShowFailureAdNotReady = "Failed to show ad, ad is not ready.";
        public const string BannerNotLaidOut = "Banner has no size yet: load it once the element is laid out.";
        public const string InvalidBannerSize = "Banner size width and height must be numbers.";
    }
}
