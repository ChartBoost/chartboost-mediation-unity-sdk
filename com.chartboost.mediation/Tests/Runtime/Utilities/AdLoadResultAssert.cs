using System.Collections.Generic;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Testing;
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
        /// Asserts that a load succeeded (banners, queue updates); ad inventory errors are Inconclusive.
        /// </summary>
        public static void AssertSuccess(IAdLoadResult result, string message = "Ad load failed")
        {
            Assert.IsNotNull(result, message);
            if (result.Error.HasValue)
                HandleLoadError(result.Error.Value, message);
        }

        /// <summary>Marks a test that needs the fixture's loaded ad Inconclusive when the load got none.</summary>
        public static void RequireLoaded(bool loaded) => Assume.That(loaded, TestConstants.AssertionMessages.RequiresLoadedAd);

        /// <summary>Inconclusive for an ad inventory error, a failed test for any other error.</summary>
        private static void HandleLoadError(ChartboostMediationError error, string baseMessage)
        {
            Debug.LogWarning($"[AdLoadResultAssert] Load error: {error.Code ?? "unknown"} - {error.Message ?? "No message"}");
            AdInventoryAssert.FailOrInconclusive(error.Code, error.Message ?? "No message", InconclusiveErrorCodes, baseMessage);
        }
    }
}
