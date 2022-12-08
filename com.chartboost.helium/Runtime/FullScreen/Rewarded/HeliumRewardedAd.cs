namespace Helium.FullScreen.Rewarded
{
	public class HeliumRewardedAd : HeliumRewardedBase {
		private readonly HeliumRewardedBase _platformRewarded;
		
		public HeliumRewardedAd(string placementName) : base(placementName)
		{
			#if UNITY_ANDROID
			_platformRewarded = new HeliumRewardedAndroid(placementName);
			#elif UNITY_IOS
			_platformRewarded = new HeliumRewardedIOS(placementName);
			#else
			_platformRewarded = new HeliumRewardedUnsupported(placementName);
			#endif
		}

		/// <inheritdoc cref="HeliumRewardedBase.SetKeyword"/>>
		public override bool SetKeyword(string keyword, string value)
			=> _platformRewarded.SetKeyword(keyword, value);

		/// <inheritdoc cref="HeliumRewardedBase.RemoveKeyword"/>>
		public override string RemoveKeyword(string keyword)
			=> _platformRewarded.RemoveKeyword(keyword);

		/// <inheritdoc cref="HeliumRewardedBase.Destroy"/>>
		public override void Destroy()
			=> _platformRewarded.Destroy();
		
		/// <inheritdoc cref="HeliumRewardedBase.Load"/>>
		public override void Load()
			=> _platformRewarded.Load();

		/// <inheritdoc cref="HeliumRewardedBase.Show"/>>
		public override void Show()
			=> _platformRewarded.Show();

		/// <inheritdoc cref="HeliumRewardedBase.ReadyToShow"/>>
		public override bool ReadyToShow()
			=> _platformRewarded.ReadyToShow();
		
		/// <inheritdoc cref="HeliumRewardedBase.ClearLoaded"/>>
		public override bool ClearLoaded()
			=> _platformRewarded.ClearLoaded();

		/// <inheritdoc cref="HeliumRewardedBase.SetCustomData"/>>
		public override void SetCustomData(string customData)
			=> _platformRewarded.SetCustomData(customData);
	}
}
