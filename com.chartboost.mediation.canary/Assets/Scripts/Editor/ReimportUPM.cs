using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

[InitializeOnLoad]
public class ReimportUPM
{
    private const string HasFetch = "upm_refresh";
    private static AddRequest _request;
    
    static ReimportUPM()
    {
        // This should not run in batchmode
        if (Application.isBatchMode)
            return;
        
        // We only want this to happen once, not every time we compile code.
        if (HasShown) 
            return;
        
        UpdateGitPackage();
        HasShown = true;
    }
    
    private static bool HasShown
    {
        get => PlayerPrefs.HasKey(HasFetch);
        set => PlayerPrefs.SetString(HasFetch, value.ToString());
    }

    // Delete the key when exiting the Editor.
    ~ReimportUPM()
    {
        PlayerPrefs.DeleteKey(HasFetch);
        _request = null;
    }
    
    /// <summary>
    /// Request Package Manager to Update Chartboost Mediation Git Package Integration on Startup.
    /// </summary>
    [MenuItem("Chartboost Mediation/Canary/Update Git Package")]
    private static void UpdateGitPackage()
    {
        switch (_request)
        {
            case null:
                _request = Client.Add("https://github.com/ChartBoost/helium-unity-sdk.git?path=/com.chartboost.mediation");
                Debug.Log("[Chartboost Mediation] Updating Chartboost Mediation Unity SDK Git Package.");
                break;
            case { IsCompleted: true }:
                switch (_request.Status)
                {
                    case StatusCode.Success:
                        Debug.Log("[Chartboost Mediation] Existing Request Succeeded to Update Package, More Requests Can be Done Now.");
                        _request = null;
                        break;
                    case StatusCode.Failure:
                        Debug.LogError($"[Chartboost Mediation] Existing Request Failed to Update Package, More Request Can be Done Now. Error: {_request.Error}");
                        _request = null;
                        break;
                    case StatusCode.InProgress:
                        Debug.LogWarning("[Chartboost Mediation] Existing Request is in Progress. Can't Request Right Now.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
        }
    }
}
