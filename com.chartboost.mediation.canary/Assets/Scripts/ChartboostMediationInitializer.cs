using System;
using System.Collections.Generic;
using Chartboost;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

/// <summary>
/// The script that will perform initialization of the Helium SDK.
/// </summary>
public class ChartboostMediationInitializer : SimpleSingleton<ChartboostMediationInitializer>
{
    /// <summary>
    /// 
    /// </summary>
    public DataVisualizer partnerInitDataPrefab;
    
    /// <summary>
    /// Environment settings.
    /// </summary>
    private readonly Lazy<Environment> _environment = new Lazy<Environment>(() => Environment.Shared);
    private Environment Environment => _environment.Value;

    /// <summary>
    /// Helium Initialization Status
    /// </summary>
    public bool Initialized { get; private set; }
    

    /// <summary>
    /// Standard Unity Awake handler.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        Application.targetFrameRate = 120;
    }

    /// <summary>
    /// Standard Unity Start handler.
    /// </summary>
    private void Start()
    {
        ChartboostMediation.DidStart += MediationStart;
        ChartboostMediation.DidReceivePartnerInitializationData += ReceivePartnerInitializationData;

        if (Environment.AutomaticInitialization)
            Initialize();
    }

    /// <summary>
    /// Standard Unity Start handler.
    /// </summary>
    public void Initialize()
    {
        if (Initialized) 
            return;
        
        var appId = Environment.AppIdentifier;
        ChartboostMediationPlacementDataSource.Instance.LoadPlacementCache(appId);
        
        ChartboostMediationPartnerSkipper.Initialize();
        var initializationOptions = ChartboostMediationPartnerSkipper.SkippedPartners.ToArray();
        ChartboostMediation.StartWithOptions(Environment.AppIdentifier, Environment.AppSignature, initializationOptions);
    }

    /// <summary>
    /// Handler that is called when the Helium SDK starts.
    /// </summary>
    /// <param name="error"></param>
    private void MediationStart(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            Console.Out.WriteLine($"MediationStart with error: {error}");
            ToastManager.ShowMessage($"Chartboost Mediation SDK Failed to Initialize with Error Code: {error}");
            return;
        }

        ToastManager.ShowMessage("Chartboost Mediation Unity SDK Initialized");
        Console.Out.WriteLine("DidStartHelium");

        // Will be doing this later.
        var userIdentifier = Environment.UserIdentifier;
        if (userIdentifier != null)
            ChartboostMediation.SetUserIdentifier(userIdentifier);
        
        Initialized = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="partnerInitializationEventData"></param>
    private void ReceivePartnerInitializationData(string partnerInitializationEventData)
    {
        Console.Out.WriteLine($"ReceivePartnerInitializationData: {partnerInitializationEventData}");
        SettingsController.PartnerInitializationData = partnerInitializationEventData;
        DataVisualizer.Instance.UpdateContents("Partner Initialization Data", partnerInitializationEventData);
        DataVisualizer.Instance.OpenVisualizer();
    }

}
