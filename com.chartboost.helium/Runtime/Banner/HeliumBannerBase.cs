using Helium.Interfaces;

namespace Helium.Banner
{
    /// <summary>
    /// Helium banner base structure.
    /// </summary>
    public abstract class HeliumBannerBase : IHeliumAd, IHeliumBannerAd
    {
        protected static string LOGTag = "HeliumBanner (Base)";
        private readonly string _placementName;
        private readonly HeliumBannerAdSize _size;

        protected HeliumBannerBase(string placementName, HeliumBannerAdSize size)
        {
            _placementName = placementName;
            _size = size;
        }

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public virtual bool SetKeyword(string keyword, string value)
        {
            HeliumLogger.Log(LOGTag, $"banner: {_placementName}, setting keyword {keyword} with value: {value}");
            return false;
        }

        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public virtual string RemoveKeyword(string keyword)
        {
            HeliumLogger.Log(LOGTag, $"banner: {_placementName}, removing keyword {keyword}");
            return keyword;
        }

        /// <inheritdoc cref="IHeliumAd.Destroy"/>>
        public virtual void Destroy()
            => HeliumLogger.Log(LOGTag, $"destroying banner: {_placementName}");

        /// <inheritdoc cref="IHeliumBannerAd.Load"/>>
        public virtual void Load(HeliumBannerAdScreenLocation location)
            => HeliumLogger.Log(LOGTag, $"loading banner: {_placementName} with size: {_size} at {location}");

        /// <inheritdoc cref="IHeliumBannerAd.SetVisibility"/>>
        public virtual void SetVisibility(bool isVisible)
            => HeliumLogger.Log(LOGTag, $"setting visibility: {isVisible} for banner: {_placementName}");

        /// <inheritdoc cref="IHeliumBannerAd.ClearLoaded"/>>
        public virtual void ClearLoaded()
            => HeliumLogger.Log(LOGTag, $"clearing banner: {_placementName}");

        /// <inheritdoc cref="IHeliumBannerAd.Remove"/>>
        public virtual void Remove()
            => HeliumLogger.Log(LOGTag, $"removing banner: {_placementName}");
    }

    /// <summary>
    /// Helium banner object for unsupported platforms.
    /// </summary>
    public class HeliumBannerUnsupported : HeliumBannerBase
    {
        public HeliumBannerUnsupported(string placementName, HeliumBannerAdSize size) : base(placementName, size)
        {
            LOGTag = "HeliumBanner (Unsupported)";
        }
    }
}
