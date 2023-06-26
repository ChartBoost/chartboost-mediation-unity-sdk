using System;
using Chartboost.Events;

namespace Chartboost.FullScreen.Rewarded
{
	/// <summary>
	/// Chartboost Mediation rewarded ad object.
	/// </summary>
	[Obsolete("ChartboostMediationRewardedAd has been deprecated, use the new fullscreen API instead.")]
	public sealed class ChartboostMediationRewardedAd : ChartboostMediationRewardedBase {
		private readonly ChartboostMediationRewardedBase _platformRewarded;

		internal ChartboostMediationRewardedAd(string placementName) : base(placementName)
		{
			#if UNITY_EDITOR
			_platformRewarded = new ChartboostMediationRewardedUnsupported(placementName);
			#elif UNITY_ANDROID
			_platformRewarded = new ChartboostMediationRewardedAndroid(placementName);
			#elif UNITY_IOS
			_platformRewarded = new ChartboostMediationRewardedIOS(placementName);
			#else
			_platformRewarded = new ChartboostMediationRewardedUnsupported(placementName);
			#endif
		}

		internal override bool IsValid { get => _platformRewarded.IsValid; set => _platformRewarded.IsValid = value; }

		/// <inheritdoc cref="ChartboostMediationRewardedBase.SetKeyword"/>>
		public override bool SetKeyword(string keyword, string value)
			=> IsValid && _platformRewarded.SetKeyword(keyword, value);

		/// <inheritdoc cref="ChartboostMediationRewardedBase.RemoveKeyword"/>>
		public override string RemoveKeyword(string keyword)
			=> IsValid ? _platformRewarded.RemoveKeyword(keyword) : null;

		/// <inheritdoc cref="ChartboostMediationRewardedBase.Destroy"/>>
		public override void Destroy()
		{
			Destroy(false);
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.Load"/>>
		public override void Load()
		{
			if (IsValid)
				_platformRewarded.Load();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.Show"/>>
		public override void Show()
		{
			if (IsValid)
				_platformRewarded.Show();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.ReadyToShow"/>>
		public override bool ReadyToShow() 
			=> IsValid && _platformRewarded.ReadyToShow();

		/// <inheritdoc cref="ChartboostMediationRewardedBase.ClearLoaded"/>>
		public override void ClearLoaded()
		{
			if (IsValid)
				_platformRewarded.ClearLoaded();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.SetCustomData"/>>
		public override void SetCustomData(string customData)
		{
			if (IsValid)
				_platformRewarded.SetCustomData(customData);
		}
		
		private void Destroy(bool isCollected)
		{
			if (!IsValid)
				return;
			_platformRewarded.Destroy();
			base.Destroy();
            
			if (isCollected) 
				EventProcessor.ReportUnexpectedSystemError($"Interstitial Ad with placement: {placementName}, got GC. Make sure to properly dispose of ads utilizing Destroy for the best integration experience.");
		}

		~ChartboostMediationRewardedAd() => Destroy(true);
	}
}
