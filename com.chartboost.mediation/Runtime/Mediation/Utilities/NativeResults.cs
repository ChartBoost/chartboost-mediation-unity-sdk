using System.Globalization;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Utilities
{
    /// <summary>Pure mappings of values the native bridges pass back, shared so they can be tested in the Editor.</summary>
    internal static class NativeResults
    {
        /// <summary>An error when native sent a code or a message; null on success (HB-12131).</summary>
        internal static ChartboostMediationError? ToError(string code, string message)
            => string.IsNullOrEmpty(code) && string.IsNullOrEmpty(message) ? null : new ChartboostMediationError(code, message);

        /// <summary>Parses a native price independently of the device locale (HB-12132).</summary>
        internal static bool TryParsePrice(string price, out double value)
            => double.TryParse(price, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
