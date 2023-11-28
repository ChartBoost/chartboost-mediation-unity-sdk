using Chartboost;
using Chartboost.FullScreen.Interstitial;
using UnityEngine;

/// <summary>
/// The controller for a specific interstitial ad placement.
/// </summary>
public class InterstitialAdController : CanaryAdController
{
    /// <summary>
    /// The interstitial ad that is being managed by this controller.
    /// </summary>
    private ChartboostMediationInterstitialAd _interstitialAd;

    /// <inheritdoc cref="CanaryAdController.Awake"/>
    protected override void Awake()
    {
        base.Awake();
        ChartboostMediation.DidLoadInterstitial += DidLoad;
        ChartboostMediation.DidShowInterstitial += DidShow;
        ChartboostMediation.DidCloseInterstitial += DidClose;
        ChartboostMediation.DidClickInterstitial += DidClick;
        ChartboostMediation.DidRecordImpressionInterstitial += DidRecordImpression;


        // Note : This may need an update once Interstitial and Rewarded are combined
        // into FullscreenAd which will lead to callbacks coming from either of them
        string[] callbacks = new string[] {
            CanaryConstants.Callbacks.DidLoad,
            CanaryConstants.Callbacks.DidShow,
            CanaryConstants.Callbacks.DidClose,
            CanaryConstants.Callbacks.DidClick,
            CanaryConstants.Callbacks.DidRecordImpression,
            CanaryConstants.Callbacks.DidReceiveILRD
        };
        callbackPanel.AddCallbacks(callbacks);
    }

    /// <summary>
    /// Standard Unity Destroy handler.
    /// </summary>
    protected override void OnDestroy()
    {
        _interstitialAd?.Destroy();
        ChartboostMediation.DidLoadInterstitial -= DidLoad;
        ChartboostMediation.DidShowInterstitial -= DidShow;
        ChartboostMediation.DidCloseInterstitial -= DidClose;
        ChartboostMediation.DidClickInterstitial -= DidClick;
        ChartboostMediation.DidRecordImpressionInterstitial -= DidRecordImpression;
    }

    /// <inheritdoc cref="CanaryAdController.OnLoadButtonPushed"/>
    public override void OnLoadButtonPushed()
    {
        base.OnLoadButtonPushed();

        // First load
        if (_interstitialAd == null)
        {
            _interstitialAd = ChartboostMediation.GetInterstitialAd(controllerConfiguration.placementName);
        }
        else    // subsequent loads
        {
            if (controllerConfiguration.loadType == AdLoadType.Reuse)
                Log(AdReUse);
            else
            {
                Log(AdReplacement);
                _interstitialAd = ChartboostMediation.GetInterstitialAd(controllerConfiguration.placementName);
            }
        }

        if (_interstitialAd == null)
        {
            Log(NotFound, null, LogType.Error);
            return;
        }

        foreach (var keyword in keywordsDataSource.Keywords)
            _interstitialAd.SetKeyword(keyword.name, keyword.value);
        
        Log(RequestingLoad);
        _interstitialAd.Load();
    }

    /// <inheritdoc cref="CanaryAdController.OnShowButtonPushed"/>
    public override void OnShowButtonPushed()
    {
        if (_interstitialAd == null)
        {
            Log(HasNotBeenLoaded);
            return;
        }

        if (_interstitialAd.ReadyToShow())
        {
            Log(AttemptingToShow);
            _interstitialAd.Show();
        }
        else
            Log(NotReadyToShow);
    }

    /// <inheritdoc cref="CanaryAdController.OnClearButtonPushed"/>
    public override void OnClearButtonPushed()
    {
        base.OnClearButtonPushed();

        if (_interstitialAd == null)
        {
            Log(HasNotBeenLoaded);
            return;
        }

        _interstitialAd.ClearLoaded();
        Log(HasBeenCleared);
    }
    
    public override void OnDestroyButtonPushed()
    {
        base.OnDestroyButtonPushed();

        if (_interstitialAd == null)
        {
            Log(HasNotBeenLoaded, null, LogType.Error);
            return;
        }
        _interstitialAd.Destroy();
        Log(HasBeenDestroyed);
    }
}
