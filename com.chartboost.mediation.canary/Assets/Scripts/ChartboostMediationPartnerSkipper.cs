using System.Collections.Generic;
using UnityEngine;

public static class ChartboostMediationPartnerSkipper
{
    private const string PartnerKillPpKeyBase = "com.chartboost.mediation.canary.partnerKill";
    
    // https://docs.chartboost.com/en/mediation/integrate/unity/initialize-mediation/#network-adapter-identifiers
    private static readonly string[] Partners =
    {
        "adcolony",
        "admob", 
        "amazon_aps",
        "applovin",
        "facebook",
        "fyber",
        "google_googlebidding",
        "inmobi",
        "ironsource",
        "mintegral",
        "pangle",
        "tapjoy",
        "unity",
        "vungle",
        "yahoo",
        "mobilefuse",
        "verve",
        "hyprmx"
    };
    
    /// <summary>
    /// The list of partners that are disabled/skipped during Chartboost Mediation SDK initialization
    /// </summary>
    public static List<string> SkippedPartners { get; } = new List<string>();
    
    /// <summary>
    /// Initializes skipped partners by restoring the partners skipped from previous session into current session
    /// </summary>
    public static void Initialize()
    {
        SkippedPartners.Clear();
        
        foreach (var partner in Partners)
        {
            var shouldSkip = PlayerPrefs.GetInt($"{PartnerKillPpKeyBase}.{partner}", 0) == 1;
            SkipPartnerInitialization(partner, shouldSkip);
        }
    }
    
    /// <summary>
    /// Skips/Unskips the initialization of provided partner 
    /// </summary>
    /// <param name="partnerIdentifier"></param>
    /// <param name="shouldSkip"></param>
    public static void SkipPartnerInitialization(string partnerIdentifier, bool shouldSkip = true)
    {
        if (shouldSkip)
        {
            if (!SkippedPartners.Contains(partnerIdentifier))
                SkippedPartners.Add(partnerIdentifier);
        }
        else
        {
            if (SkippedPartners.Contains(partnerIdentifier))
                SkippedPartners.Remove(partnerIdentifier);
        }

        PlayerPrefs.SetInt($"{PartnerKillPpKeyBase}.{partnerIdentifier}", shouldSkip ? 1 : 0);
        PlayerPrefs.Save();
    }
}
