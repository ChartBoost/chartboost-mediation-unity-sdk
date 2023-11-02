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
        public const string MetaAudienceNetwork = "facebook";
        public const string DigitalTurbineExchange = "fyber";
        public const string GoogleBidding = "google_googlebidding";
        public const string InMobi = "inmobi";
        public const string IronSource = "ironsource";
        public const string Mintegral = "mintegral";
        public const string Pangle = "pangle";
        public const string Tapjoy = "tapjoy";
        public const string Unity = "unity";
        public const string Vungle = "vungle";
        public const string Yahoo = "yahoo";
        public const string MobileFuse = "mobilefuse";
        public const string Verve = "verve";
        public const string HyprMX = "hyprmx";
    }
    
    [Obsolete("Adapters has been deprecated and will be replaced by Partners in the future.")]
    public static class Adapters
    {
        public const string AdColony = Partners.AdColony;
        public const string AdMob = Partners.AdMob;
        public const string AmazonPublisherServices = Partners.AmazonPublisherServices;
        public const string AppLovin = Partners.AppLovin;
        public const string MetaAudienceNetwork = Partners.MetaAudienceNetwork;
        public const string DigitalTurbineExchange = Partners.DigitalTurbineExchange;
        public const string GoogleBidding = Partners.GoogleBidding;
        public const string InMobi = Partners.InMobi;
        public const string IronSource = Partners.IronSource;
        public const string Mintegral = Partners.Mintegral;
        public const string Pangle = Partners.Pangle;
        public const string Tapjoy = Partners.Tapjoy;
        public const string Unity = Partners.Unity;
        public const string Vungle = Partners.Vungle;
        public const string Yahoo = Partners.Yahoo;
        public const string MobileFuse = Partners.MobileFuse;
        public const string Verve = Partners.Verve;
        public const string HyprMX = Partners.HyprMX;
    }
}
