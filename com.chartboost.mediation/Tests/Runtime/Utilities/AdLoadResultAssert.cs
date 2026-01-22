using System.Collections.Generic;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using NUnit.Framework;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Utility class for asserting ad load results with special handling for ad inventory errors.
    /// Ad inventory errors (no fill, waterfall exhausted, etc.) are marked as Inconclusive rather than Failed
    /// since they indicate a lack of ad inventory rather than a code/SDK issue.
    /// </summary>
    public static class AdLoadResultAssert
    {
        /// <summary>
        /// Error codes that indicate ad inventory issues.
        /// These errors should mark tests as Inconclusive, not Failed.
        /// </summary>
        private static readonly HashSet<string> InconclusiveErrorCodes = new()
        {
            "CM_313", // No ad inventory at this time
            "CM_335", // All waterfall entries have resulted in an error or no fill
            "CM_411", // No ad inventory at this time (show)
            "CM_317"  // Rate limited
        };

        /// <summary>
        /// Asserts that the fullscreen ad load result succeeded.
        /// If the load failed due to ad inventory issues, the test is marked as Inconclusive.
        /// </summary>
        /// <param name="result">The fullscreen ad load result to validate.</param>
        /// <param name="message">Optional assertion message.</param>
        public static void AssertSuccess(FullscreenAdLoadResult result, string message = null)
        {
            Assert.IsNotNull(result, message ?? "Load result should not be null");

            if (!result.Error.HasValue)
            {
                // Load succeeded
                Assert.IsNotNull(result.Ad, message ?? "Ad should not be null after successful load");
                return;
            }

            // Load failed - check if it's an ad inventory error
            var error = result.Error.Value;
            HandleLoadError(error, message ?? "Fullscreen ad load failed");
        }

        /// <summary>
        /// Asserts that the banner ad load result succeeded.
        /// If the load failed due to ad inventory issues, the test is marked as Inconclusive.
        /// </summary>
        /// <param name="result">The banner ad load result to validate.</param>
        /// <param name="message">Optional assertion message.</param>
        public static void AssertSuccess(BannerAdLoadResult result, string message = null)
        {
            Assert.IsNotNull(result, message ?? "Load result should not be null");

            if (!result.Error.HasValue)
            {
                // Load succeeded
                return;
            }

            // Load failed - check if it's an ad inventory error
            var error = result.Error.Value;
            HandleLoadError(error, message ?? "Banner ad load failed");
        }

        /// <summary>
        /// Checks if an error code indicates an ad inventory issue.
        /// </summary>
        /// <param name="errorCode">The error code to check.</param>
        /// <returns>True if the error indicates ad inventory issues, false otherwise.</returns>
        public static bool IsAdInventoryError(string errorCode)
        {
            return !string.IsNullOrEmpty(errorCode) && InconclusiveErrorCodes.Contains(errorCode);
        }

        /// <summary>
        /// Checks if an error indicates an ad inventory issue.
        /// </summary>
        /// <param name="error">The error to check.</param>
        /// <returns>True if the error indicates ad inventory issues, false otherwise.</returns>
        public static bool IsAdInventoryError(ChartboostMediationError error)
        {
            return IsAdInventoryError(error.Code);
        }

        /// <summary>
        /// Handles a load error by either marking the test as Inconclusive (for ad inventory errors)
        /// or failing the test (for other errors).
        /// </summary>
        /// <param name="error">The error that occurred.</param>
        /// <param name="baseMessage">Base message for the assertion.</param>
        private static void HandleLoadError(ChartboostMediationError error, string baseMessage)
        {
            var errorCode = error.Code ?? "unknown";
            var errorMessage = error.Message ?? "No message";

            Debug.LogWarning($"[AdLoadResultAssert] Load error: {errorCode} - {errorMessage}");

            if (IsAdInventoryError(errorCode))
            {
                // Ad inventory error - mark as Inconclusive
                var inconclusiveMessage = $"[Ad Inventory - {errorCode}] {baseMessage}: {errorMessage}. " +
                                          "This error indicates a lack of ad inventory, not a code issue.";
                Debug.Log($"[AdLoadResultAssert] Marking test as Inconclusive: {inconclusiveMessage}");
                Assert.Inconclusive(inconclusiveMessage);
            }
            else
            {
                // Other error - fail the test
                Assert.Fail($"{baseMessage}: {errorCode} - {errorMessage}");
            }
        }
    }
}
