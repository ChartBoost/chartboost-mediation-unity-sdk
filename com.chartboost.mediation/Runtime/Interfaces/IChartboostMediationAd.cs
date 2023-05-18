namespace Chartboost.Interfaces
{
    #nullable enable
    public interface IChartboostMediationAd
    {
        /// <summary>
        /// Set a keyword/value pair on the advertisement. If the keyword has previously been
        /// set, then the value will be replaced with the new value.  These values will be
        /// used upon the loading of the advertisement.
        /// </summary>
        /// <param name="keyword">The keyword (maximum of 64 characters)</param>
        /// <param name="value">The value (maximum of 256 characters)</param>
        /// <returns>true if the keyword was successfully set, else false</returns>
        bool SetKeyword(string keyword, string value);

        /// <summary>
        /// Remove a keyword from the advertisement.
        /// </summary>
        /// <param name="keyword">The keyword to remove.</param>
        /// <returns>The currently set value, else null</returns>
        string RemoveKeyword(string keyword);
        
        /// <summary>
        /// Destroy the advertisement to free up memory resources.
        /// </summary>
        void Destroy();
    }
    #nullable disable
}
