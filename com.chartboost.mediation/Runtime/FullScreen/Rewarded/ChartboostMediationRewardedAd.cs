namespace Chartboost.FullScreen.Rewarded
{
	/// <summary>
	/// Chartboost Mediation rewarded ad object.
	/// </summary>
	public class ChartboostMediationRewardedAd : ChartboostMediationRewardedBaseOld {
		private readonly ChartboostMediationRewardedBaseOld _platformRewarded;
		
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

		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.SetKeyword"/>>
		public override bool SetKeyword(string keyword, string value)
			=> _platformRewarded.SetKeyword(keyword, value);

		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.RemoveKeyword"/>>
		public override string RemoveKeyword(string keyword)
			=> _platformRewarded.RemoveKeyword(keyword);

		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.Destroy"/>>
		public override void Destroy()
			=> _platformRewarded.Destroy();
		
		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.Load"/>>
		public override void Load()
			=> _platformRewarded.Load();

		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.Show"/>>
		public override void Show()
			=> _platformRewarded.Show();

		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.ReadyToShow"/>>
		public override bool ReadyToShow()
			=> _platformRewarded.ReadyToShow();
		
		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.ClearLoaded"/>>
		public override void ClearLoaded()
			=> _platformRewarded.ClearLoaded();

		/// <inheritdoc cref="ChartboostMediationRewardedBaseOld.SetCustomData"/>>
		public override void SetCustomData(string customData)
			=> _platformRewarded.SetCustomData(customData);
	}
}
