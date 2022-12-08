namespace Helium.FullScreen.Rewarded
{
    public interface IHeliumRewardedAd
    {
        /// <summary>
        /// Specify custom data that can be passed along with the rewarded advertisement.
        /// </summary>
        /// <param name="customData">The custom data (for example: a BASE64 encoded JSON string).</param>
        void SetCustomData(string customData);
    }
}
