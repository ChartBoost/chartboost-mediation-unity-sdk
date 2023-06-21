using System;

namespace Chartboost.FullScreen.Rewarded
{
	/// <summary>
	/// Chartboost Mediation rewarded ad object.
	/// </summary>
	[Obsolete("ChartboostMediationRewardedAd has been deprecated, use the new fullscreen API instead.")]
	public class ChartboostMediationRewardedAd : ChartboostMediationRewardedBase {
		private readonly ChartboostMediationRewardedBase _platformRewarded;

		public ChartboostMediationRewardedAd(string placementName) : base(placementName)
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

		/// <inheritdoc cref="ChartboostMediationRewardedBase.SetKeyword"/>>
		public override bool SetKeyword(string keyword, string value)
			=> _platformRewarded.IsValid && _platformRewarded.SetKeyword(keyword, value);

		/// <inheritdoc cref="ChartboostMediationRewardedBase.RemoveKeyword"/>>
		public override string RemoveKeyword(string keyword)
			=> _platformRewarded.IsValid ? _platformRewarded.RemoveKeyword(keyword) : null;

		/// <inheritdoc cref="ChartboostMediationRewardedBase.Destroy"/>>
		public override void Destroy()
		{
			if (!_platformRewarded.IsValid)
				return;
			_platformRewarded.Destroy();
			base.Destroy();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.Load"/>>
		public override void Load()
		{
			if (_platformRewarded.IsValid)
				_platformRewarded.Load();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.Show"/>>
		public override void Show()
		{
			if (_platformRewarded.IsValid)
				_platformRewarded.Show();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.ReadyToShow"/>>
		public override bool ReadyToShow() 
			=> _platformRewarded.IsValid && _platformRewarded.ReadyToShow();

		/// <inheritdoc cref="ChartboostMediationRewardedBase.ClearLoaded"/>>
		public override void ClearLoaded()
		{
			if (_platformRewarded.IsValid)
				_platformRewarded.ClearLoaded();
		}

		/// <inheritdoc cref="ChartboostMediationRewardedBase.SetCustomData"/>>
		public override void SetCustomData(string customData)
		{
			if (_platformRewarded.IsValid)
				_platformRewarded.SetCustomData(customData);
		}

		~ChartboostMediationRewardedAd() => Destroy();
	}
}
