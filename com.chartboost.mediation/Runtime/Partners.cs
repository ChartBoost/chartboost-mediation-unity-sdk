using System;
using Chartboost.Consent;

namespace Chartboost
{
    /// <summary>
    /// List of Adapter identifiers to be utilized when calling <see cref="ChartboostMediation.StartWithOptions"/> and <see cref="IPartnerConsent"/>
    /// </summary>
    public static class Partners
    {
        public const string AdColony = "adcolony";
        public const string AdMob = "admob";
        public const string AmazonPublisherServices = "amazon_aps";
        public const string AppLovin = "applovin";
        public const string DigitalTurbineExchange = "fyber";
        public const string GoogleBidding = "google_googlebidding";
        public const string HyprMX = "hyprmx";
        public const string InMobi = "inmobi";
        public const string IronSource = "ironsource";
        public const string MetaAudienceNetwork = "facebook";
        public const string Mintegral = "mintegral";
        public const string MobileFuse = "mobilefuse";
        public const string Pangle = "pangle";
        public const string Tapjoy = "tapjoy";
        public const string Unity = "unity";
        public const string Verve = "verve";
        public const string Vungle = "vungle";
        public const string Yahoo = "yahoo";
    }
    
    [Obsolete("Adapters has been deprecated and will be replaced by Partners in the future.")]
    public static class Adapters
    {
        public const string AdColony = Partners.AdColony;
        public const string AdMob = Partners.AdColony;
        public const string AmazonPublisherServices = Partners.AdColony;
        public const string AppLovin = Partners.AdColony;
        public const string DigitalTurbineExchange = Partners.AdColony;
        public const string GoogleBidding = Partners.AdColony;
        public const string HyprMX = Partners.AdColony;
        public const string InMobi = Partners.AdColony;
        public const string IronSource = Partners.AdColony;
        public const string MetaAudienceNetwork = Partners.AdColony;
        public const string Mintegral = Partners.AdColony;
        public const string MobileFuse = Partners.AdColony;
        public const string Pangle = Partners.AdColony;
        public const string Tapjoy = Partners.AdColony;
        public const string Unity = Partners.AdColony;
        public const string Verve = Partners.AdColony;
        public const string Vungle = Partners.AdColony;
        public const string Yahoo = Partners.AdColony;
    }
}
