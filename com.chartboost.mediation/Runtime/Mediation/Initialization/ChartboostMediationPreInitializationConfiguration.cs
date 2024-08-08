using System;
using System.Collections.Generic;

namespace Chartboost.Mediation.Initialization
{
    /// <summary>
    /// Options for Chartboost Mediation initialization.
    /// </summary>
    [Serializable]
    public struct ChartboostMediationPreInitializationConfiguration
    {
        /// Set of Partner adapters to skip during Chartboost Mediation SDK initialization.
        public readonly HashSet<string> SkippablePartnerIds;

        public ChartboostMediationPreInitializationConfiguration(HashSet<string> skippablePartnerIds)
        {
            SkippablePartnerIds = skippablePartnerIds ?? new HashSet<string>();
        }
    }
}
