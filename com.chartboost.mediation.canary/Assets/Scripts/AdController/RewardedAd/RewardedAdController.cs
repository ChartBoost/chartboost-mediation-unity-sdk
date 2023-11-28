using Chartboost;
using Chartboost.FullScreen.Rewarded;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The controller for a specific rewarded ad placement.
/// </summary>
public class RewardedAdController : CanaryAdController
{
    /// <summary>
    /// An input field for specifying custom data.
    /// </summary>
    public InputField customDataInputField;
    
    /// <summary>
    /// The rewarded ad that is being managed by this controller.
    /// </summary>
    private ChartboostMediationRewardedAd _rewardedAd;

    /// <inheritdoc cref="CanaryAdController.Awake"/>
    protected override void Awake()
    {
        base.Awake();
        ChartboostMediation.DidLoadRewarded += DidLoad;
        ChartboostMediation.DidShowRewarded += DidShow;
        ChartboostMediation.DidCloseRewarded += DidClose;
        ChartboostMediation.DidClickRewarded += DidClick;
        ChartboostMediation.DidReceiveReward += DidReceiveReward;
        ChartboostMediation.DidRecordImpressionRewarded += DidRecordImpression;

        var callbacks = new[] {
            CanaryConstants.Callbacks.DidLoad,
            CanaryConstants.Callbacks.DidShow,
            CanaryConstants.Callbacks.DidClose,
            CanaryConstants.Callbacks.DidClick,
            CanaryConstants.Callbacks.DidReceiveReward,
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
        base.OnDestroy();
        // release ad resources
        _rewardedAd?.Destroy();
        ChartboostMediation.DidLoadRewarded -= DidLoad;
        ChartboostMediation.DidShowRewarded -= DidShow;
        ChartboostMediation.DidCloseRewarded -= DidClose;
        ChartboostMediation.DidClickRewarded -= DidClick;
        ChartboostMediation.DidReceiveReward -= DidReceiveReward;
        ChartboostMediation.DidRecordImpressionRewarded -= DidRecordImpression;
    }

    /// <inheritdoc cref="CanaryAdController.OnLoadButtonPushed"/>
    public override void OnLoadButtonPushed()
    {
        base.OnLoadButtonPushed();

        // First load
        if (_rewardedAd == null)
        {
            _rewardedAd = ChartboostMediation.GetRewardedAd(controllerConfiguration.placementName);
        }
        else    // subsequent loads
        {
            if (controllerConfiguration.loadType == AdLoadType.Reuse)
                Log(AdReUse);
            else
            {
                Log(AdReplacement);
                _rewardedAd = ChartboostMediation.GetRewardedAd(controllerConfiguration.placementName);
            }
        }

        if (_rewardedAd == null)
        {
            Log(NotFound, null, LogType.Error);
            return;
        }

        foreach (var keyword in keywordsDataSource.Keywords)
            _rewardedAd.SetKeyword(keyword.name, keyword.value);
        
        Log(RequestingLoad);
        _rewardedAd.SetCustomData(customDataInputField.text);
        _rewardedAd.Load();
    }

    /// <inheritdoc cref="CanaryAdController.OnShowButtonPushed"/>
    public override void OnShowButtonPushed()
    {
        if (_rewardedAd == null)
        {
            Log(HasNotBeenLoaded);
            return;
        }

        if (_rewardedAd.ReadyToShow())
        {
            Log(AttemptingToShow);
            _rewardedAd.Show();
        }
        else 
            Log(NotReadyToShow);
    }

    /// <inheritdoc cref="CanaryAdController.OnClearButtonPushed"/>
    public override void OnClearButtonPushed()
    {
        base.OnClearButtonPushed();

        if (_rewardedAd == null)
        {
            Log(HasNotBeenLoaded);
            return;
        }

        _rewardedAd.ClearLoaded();
        Log(HasBeenCleared);
    }

    public override void OnDestroyButtonPushed()
    {
        base.OnDestroyButtonPushed();

        if (_rewardedAd == null)
        {
            Log(HasNotBeenLoaded, null, LogType.Error);
            return;
        }
        _rewardedAd.Destroy();
        Log(HasBeenDestroyed);
    }

    /// <summary>
    /// Handler for when the input of custom data has completed.
    /// </summary>
    public void OnCustomDataEndEdit()
    {
        if (_rewardedAd == null)
        {
            Log("failed to set custom data, Ad does not exist", null, LogType.Error);
            return;
        }
        
        Log($"setting custom data: {customDataInputField.text}");
        _rewardedAd.SetCustomData(customDataInputField.text);
    }
}
