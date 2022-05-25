using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Helium
{
	public enum HeliumBannerAdSize : int
    {
        /// 320 x 50
        Standard = 0,
        /// 300 x 250
        MediumRect = 1,
		/// 720 x 90
        Leaderboard = 2
    };

	public enum HeliumBannerAdScreenLocation : int
    {
        TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		Center = 3,
		BottomLeft = 4,
		BottomCenter = 5,
		BottomRight = 6
    };

	public class HeliumBannerAd {

		#if UNITY_IPHONE
		// Extern functions
		[DllImport("__Internal")]
		private static extern bool _heliumSdkBannerSetKeyword(IntPtr uniqueID, string keyword, string value);
		[DllImport("__Internal")]
		private static extern string _heliumSdkBannerRemoveKeyword(IntPtr uniqueID, string keyword);
		[DllImport("__Internal")]
		private static extern void _heliumSdkBannerAdLoad(IntPtr uniqueID);
		[DllImport("__Internal")]
		private static extern bool _heliumSdkBannerClearLoaded(IntPtr uniqueID);
		[DllImport("__Internal")]
		private static extern void _heliumSdkBannerAdShow(IntPtr uniqueID, int screenLocation);
		[DllImport("__Internal")]
		private static extern bool _heliumSdkBannerAdReadyToShow(IntPtr uniqueID);
		[DllImport("__Internal")]
		private static extern bool _heliumSdkBannerRemove(IntPtr uniqueID);
		[DllImport("__Internal")]
		private static extern bool _heliumSdkBannerSetVisibility(IntPtr uniqueID, bool isVisible);
		[DllImport("__Internal")]
		private static extern void _heliumSdkFreeBannerAdObject(IntPtr uniqueID);
		#endif

		// Class variables
		private readonly IntPtr uniqueId;

		#if UNITY_IPHONE
		public HeliumBannerAd(IntPtr uniqueId)
		{
			this.uniqueId = uniqueId;
		}
		#elif UNITY_ANDROID
		private AndroidJavaObject androidAd;
		public HeliumBannerAd(AndroidJavaObject ad)
		{
			androidAd = ad;
		}
		#endif

		// Class functions

		/// <summary>
		/// Set a keyword/value pair on the advertisement. If the keyword has previously been
		/// set, then the value will be replaced with the new value.  These values will be
		/// used upon the loading of the advertisement.
		/// </summary>
		/// <param name="keyword">The keyword (maximum of 64 characters)</param>
		/// <param name="value">The value (maximum of 256 characters)</param>
		/// <returns>true if the keyword was successfully set, else false</returns>
		public bool SetKeyword(string keyword, string value)
		{
			#if UNITY_IPHONE
			return _heliumSdkBannerSetKeyword(uniqueId, keyword, value);
			#elif UNITY_ANDROID
			return androidAd.Call<bool>("setKeyword", keyword, value);
			#else
			return false;
			#endif
		}

		/// <summary>
		/// Remove a keyword from the advertisement.
		/// </summary>
		/// <param name="keyword">The keyword to remove.</param>
		/// <returns>The currently set value, else null</returns>
		public string RemoveKeyword(string keyword)
		{
			#if UNITY_IPHONE
			return _heliumSdkBannerRemoveKeyword(uniqueId, keyword);
			#elif UNITY_ANDROID
			return androidAd.Call<string>("removeKeyword", keyword);
			#else
			return null;
			#endif
		}

		/// <summary>
		/// Load the advertisement.
		/// </summary>
		public void Load() {
			#if UNITY_IPHONE
			_heliumSdkBannerAdLoad(uniqueId);
			#elif UNITY_ANDROID
			androidAd.Call("load");
			#endif
		}

		/// <summary>
		/// If an advertisement has been loaded, clear it. Once cleared, a new
		/// load can be performed.
		/// </summary>
		/// <returns>true if successfully cleared</returns>
		public bool ClearLoaded() {
			#if UNITY_IPHONE
			return _heliumSdkBannerClearLoaded(uniqueId);
			#elif UNITY_ANDROID
			return androidAd.Call<bool>("clearLoaded");
			#else
			return false;
			#endif
		}

		/// <summary>
		/// Show a previously loaded advertisement at a specific screen location.
		/// </summary>
		/// <param name="screenLocation">The screen location to show the banner at.</param>
		public void Show(HeliumBannerAdScreenLocation screenLocation)
		{
			#if UNITY_IPHONE
			System.GC.Collect(); // make sure previous banner ads get destructed if necessary
			_heliumSdkBannerAdShow(uniqueId, (int) screenLocation);
			#elif UNITY_ANDROID
            androidAd.Call("show", (int) screenLocation);
			#endif
		}

		/// <summary>
		/// Remove the banner.
		/// </summary>
		public void Remove()
		{
			#if UNITY_IPHONE
			_heliumSdkBannerRemove(uniqueId);
			#elif UNITY_ANDROID
			//android doesn't have a remove method. Instead, calling destroy
			destroy();
			#endif
		}

		/// <summary>
		/// Indicates if an advertisement is ready to show.
		/// </summary>
		/// <returns>True if ready to show.</returns>
		public bool ReadyToShow()
		{
			#if UNITY_IPHONE
			return _heliumSdkBannerAdReadyToShow(uniqueId);
			#elif UNITY_ANDROID
			return androidAd.Call<bool>("readyToShow");
			#else
			return false;
			#endif
		}

		/// <summary>This method changes the visibility of the banner ad.</summary>
		/// <param name="isVisible">Specify if the banner should be visible.</param>
		public void SetVisibility(bool isVisible)
		{
			#if UNITY_IPHONE
			_heliumSdkBannerSetVisibility(uniqueId, isVisible);
			#elif UNITY_ANDROID
			androidAd.Call("setBannerVisibility", isVisible);
			#endif
		}

		/// <summary>
		/// Destroy the advertisement to free up memory resources.
		/// </summary>
		public void Destroy()
		{
			#if UNITY_ANDROID
			androidAd.Call("destroy");
			#endif
		}

		~HeliumBannerAd()
		{
			#if UNITY_IPHONE
			_heliumSdkFreeBannerAdObject(uniqueId);
			#endif
		}
	}
}


