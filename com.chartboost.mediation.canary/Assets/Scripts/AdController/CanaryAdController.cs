using System;
using System.Collections;
using System.IO;
using AdController;
using Chartboost;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class CanaryAdController : MonoBehaviour, IAdController, IKeywordsControllerListener
{
    /// <summary>
    /// Label for the banner placement name.
    /// </summary>
    public Text placementNameLabel;
    
    /// <summary>
    /// The log panel
    /// </summary>
    public ExpandableLogger logger;
    
    /// <summary>
    /// The button text that is used to present the keywords user interface.
    /// </summary>
    public Text keywordsText;
    
    /// <summary>
    /// The configuration for keywords UI.
    /// </summary>
    [SerializeField]
    public AdControllerKeywordsConfiguration keywords;

    /// <summary>
    /// The panel that holds all the callbacks associated with this controller
    /// </summary>
    public CallbackPanel callbackPanel;
    
    /// <summary>
    /// The controller's configuration.
    /// </summary>
    protected AdControllerConfiguration controllerConfiguration;
    
    /// <summary>
    /// keywords data source for placement
    /// </summary>
    protected IKeywordsDataSource keywordsDataSource;

    /// <summary>
    /// Configure this ad controller.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public virtual void Configure(AdControllerConfiguration configuration)
    {
        placementNameLabel.text = configuration.placementName;
        controllerConfiguration = configuration;
    }

    /// <summary>
    /// Standard Unity Start handler.
    /// </summary>
    protected virtual void Awake()
    {
        ChartboostMediation.DidReceiveImpressionLevelRevenueData += DidReceiveILRD;
        ChartboostMediation.UnexpectedSystemErrorDidOccur += UnexpectedError;
    }

    /// <summary>
    /// Standard Unity Start handler.
    /// </summary>
    protected virtual void Start()
    {
        InitializeKeywords();
    }

    /// <summary>
    /// Standard Unity OnEnable Event
    /// </summary>
    protected virtual void OnEnable()
    {
        TabPanelController.Instance.ToggleSearchBarVisibility(false);
        UpdateKeywordsButton();
    }

    /// <summary>
    /// Standard Unity Update Event
    /// </summary>
    protected virtual void Update()
    {
        // Responds to device back button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackButtonPushed();
        }
    }

    /// <summary>
    /// Standard Unity OnDisable Event
    /// </summary>
    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {
        ChartboostMediation.DidReceiveImpressionLevelRevenueData -= DidReceiveILRD;
        ChartboostMediation.UnexpectedSystemErrorDidOccur -= UnexpectedError;
    }

    /// <inheritdoc cref="IAdController.OnBackButtonPushed"/>
    public virtual void OnBackButtonPushed()
    {
        controllerConfiguration.parentListController.PopAdController(gameObject);
    }

    /// <inheritdoc cref="IAdController.OnClearLogButtonPushed"/>
    public virtual void OnClearLogButtonPushed()
    {
        logger.Clear();
    }

    /// <inheritdoc cref="IAdController.OnLoadButtonPushed"/>
    public virtual void OnLoadButtonPushed()
    {
        callbackPanel.Reset();
    }

    /// <inheritdoc cref="IAdController.OnShowButtonPushed"/>
    public virtual void OnShowButtonPushed() { }

    /// <inheritdoc cref="IAdController.OnClearButtonPushed"/>
    public virtual void OnClearButtonPushed()
    {
        callbackPanel.Reset();
    }

    /// <inheritdoc cref="IAdController.OnDestroyButtonPushed"/>
    public virtual void OnDestroyButtonPushed()
    {
        callbackPanel.Reset();
        GC.Collect();
    }

    /// <inheritdoc cref="IAdController.OnToggleVisibilityButtonPushed"/>
    public virtual void OnToggleVisibilityButtonPushed() { }

    /// <inheritdoc cref="IAdController.OnKeywordsButtonPushed"/>
    public void OnKeywordsButtonPushed()
    {
        if (keywords.ShowKeywordsController(controllerConfiguration.placementName, UIHelper.Instance.MainCanvas, this, keywordsDataSource))
            gameObject.SetActive(false);
    }

    /// <inheritdoc cref="IKeywordsControllerListener.KeywordsControllerDidRequestDestroy"/>
    public void KeywordsControllerDidRequestDestroy()
    {
        keywords.DestroyKeywordsController();
        gameObject.SetActive(true);
    }

    /// <inheritdoc cref="IKeywordsControllerListener.KeywordsControllerDidUpdateKeywords"/>
    public void KeywordsControllerDidUpdateKeywords(Keyword[] keywords)
    {
        UpdateKeywordsButton();
    }
    
    protected void InitializeKeywords()
    {
        IKeywordsDataSource dataSource = new KeywordsPlayerPrefsDataSource(controllerConfiguration.placementName);
        keywordsDataSource = dataSource;
        dataSource.FetchKeywords(delegate (string error)
        {
            if (error != null)
                Console.Write($"Error with fetching keywords: {error}");
            else
            {
                Console.Write($"Parsed {dataSource.Keywords.Length} keywords");
                UpdateKeywordsButton();
            }
        });
    }

    private void UpdateKeywordsButton()
    {
        var length = keywordsDataSource != null ?  keywordsDataSource.Keywords.Length : 0;
        keywordsText.text = $"Keywords ({length})";
    }
    
    protected void Log(string text, string detailedInfo = null, LogType logType = LogType.Log)
    {
        Console.Out.WriteLine(text);
        logger.Log(text, detailedInfo, logType);
    }
    
    #region Callbacks
    protected virtual void DidLoad(string placementName, string loadId, BidInfo info, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidLoad Info", $"Price ${info.Price:F4}, \nAuction Id: {info.AuctionId}, \nPartner Id: {info.PartnerId}");
        else
            Log($"{placementName} DidLoad Error", error, LogType.Error);
        SetCallbackState(CanaryConstants.Callbacks.DidLoad, error);
    }

    protected virtual void DidLoad(AdLoadDetails adLoadDetails)
    {
        if (!adLoadDetails.error.HasValue)
        {
            Log($"{adLoadDetails.placementName} DidLoad Info",
            JsonConvert.SerializeObject(adLoadDetails, Formatting.Indented));
        }
        else
        {
            Log($"{adLoadDetails.placementName} DidLoad Error", 
                $"Code : {adLoadDetails.error?.Code}\nMessage : {adLoadDetails.error?.Message}",
                LogType.Error);
        }

        SetCallbackState(CanaryConstants.Callbacks.DidLoad, adLoadDetails.error?.Message);
    }

    protected virtual void DidShow(string placementName, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidShow");
        else
            Log($"{placementName} DidShow Error", error, LogType.Error);
        SetCallbackState(CanaryConstants.Callbacks.DidShow, error);
    }

    protected virtual void DidShow(string placementName, Metrics? metrics, ChartboostMediationError? error)
    {
        if (!error.HasValue)
        {
            Log($"{placementName} DidShow",
                $"Metrics : {JsonConvert.SerializeObject(metrics, Formatting.Indented)}");
        }
        else
        {
            Log($"{placementName} DidShow Error", 
                $"Code : {error?.Code}\nMessage : {error?.Message}",
                LogType.Error);
        }
        
        SetCallbackState(CanaryConstants.Callbacks.DidShow, error?.Message);
    }
    
    protected virtual void DidClose(string placementName, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidClose");
        else
            Log($"{placementName} DidClose Error", error, LogType.Error);
        SetCallbackState(CanaryConstants.Callbacks.DidClose, error);
    }

    protected virtual void DidClick(string placementName, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidClick");
        else
            Log($"{placementName} DidClick Error", error, LogType.Error);
        SetCallbackState(CanaryConstants.Callbacks.DidClick, error);
    }

    protected virtual void DidRecordImpression(string placementName, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidRecordImpression");
        else
            Log($"{placementName} DidRecordImpression Error", error, LogType.Error);
        SetCallbackState(CanaryConstants.Callbacks.DidRecordImpression, error);
    }

    protected virtual void DidReceiveILRD(string placement, Hashtable impressionData)
    {
        var json =  JsonTools.Serialize(impressionData);
        Log($"{placement} ILRD",$"{JsonPrettify(json)}");
        SetCallbackState(CanaryConstants.Callbacks.DidReceiveILRD, null);        
    }

    protected virtual void UnexpectedError(string error)
    {
        Log(error, null, LogType.Warning);
    }

    protected void DidReceiveReward(string placementName, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidReceiveReward");
        else
            Log($"{placementName} DidReceiveReward Error", error, LogType.Error);

        SetCallbackState(CanaryConstants.Callbacks.DidReceiveReward, error);
    }

    protected void DidExpire(string placementName, string error)
    {
        if (string.IsNullOrEmpty(error))
            Log($"{placementName} DidExpire");
        else
            Log($"{placementName} DidExpire Error", error, LogType.Error);

        SetCallbackState(CanaryConstants.Callbacks.DidExpire, error);
    }
    
    #endregion
    
    protected void SetCallbackState(string callback, string error = null)
    {
        try
        {
            if (callbackPanel.Contains(callback))
            {
                if (string.IsNullOrEmpty(error))
                {
                    callbackPanel.SetCallbackSuccess(callback);
                }
                else
                {
                    callbackPanel.SetCallbackError(callback);
                }
            }
        }
        catch(Exception e)
        {
            Log($"ERROR => Error while setting callback state for {callback}.",$" Error : {e.Message}", LogType.Error);
        }
    }
    
    public static string JsonPrettify(string json)
    {
        using var stringReader = new StringReader(json);
        using var stringWriter = new StringWriter();
        var jsonReader = new JsonTextReader(stringReader);
        var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
        jsonWriter.WriteToken(jsonReader);
        return stringWriter.ToString();
    }

    protected const string NotFound = "not found.";
    protected const string RequestingLoad = "requesting load.";
    protected const string AdReplacement = "replacing ad.";
    protected const string AdReUse = "reusing ad.";
    protected const string HasNotBeenLoaded = "has not been loaded.";
    protected const string AttemptingToShow = "attempting to show.";
    protected const string NotReadyToShow = "not ready to show.";
    protected const string HasBeenCleared = "has been cleared.";
    protected const string HasBeenDestroyed = "has been destroyed, resources released.";
    protected const string ToggleVisibilityFailed = "does not exist, unable to toggle visibility.";
    protected const string ToggleVisibility = "visibility toggled.";
    protected const string AutoLoadingNextAd = "automatically loading next ad...";
}
