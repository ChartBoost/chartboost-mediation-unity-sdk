using UnityEngine;
#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
#endif

namespace Helium
{
	public enum HeliumBannerAdSize
    {
        /// 320 x 50
        Standard = 0,
        /// 300 x 250
        MediumRect = 1,
		/// 720 x 90
        Leaderboard = 2
    }

	public enum HeliumBannerAdScreenLocation
    {
        TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		Center = 3,
		BottomLeft = 4,
		BottomCenter = 5,
		BottomRight = 6
    }

	public class HeliumBannerAd {

		#if UNITY_IPHONE
		// Extern functions
		[DllImport("__Internal")]
		private static extern bool _heliumSdkBannerSetKeyword(IntPtr uniqueId, string keyword, string value);
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
		#if UNITY_IPHONE
		private readonly IntPtr _uniqueId;
		#endif

		#if UNITY_IPHONE
		public HeliumBannerAd(IntPtr uniqueId)
		{
			_uniqueId = uniqueId;
		}
		#elif UNITY_ANDROID
		private readonly AndroidJavaObject _androidAd;
		public HeliumBannerAd(AndroidJavaObject ad)
		{
			_androidAd = ad;
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
			return _heliumSdkBannerSetKeyword(_uniqueId, keyword, value);
			#elif UNITY_ANDROID
			return _androidAd.Call<bool>("setKeyword", keyword, value);
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
			return _heliumSdkBannerRemoveKeyword(_uniqueId, keyword);
			#elif UNITY_ANDROID
			return _androidAd.Call<string>("removeKeyword", keyword);
			#else
			return null;
			#endif
		}

		/// <summary>
		/// Load the advertisement.
		/// </summary>
		public void Load(HeliumBannerAdScreenLocation screenLocation) 
		{
			#if UNITY_IPHONE
			_heliumSdkBannerAdLoad(_uniqueId);
			#elif UNITY_ANDROID
			_androidAd.Call("load", (int) screenLocation);
			#endif
		}

		/// <summary>
		/// If an advertisement has been loaded, clear it. Once cleared, a new
		/// load can be performed.
		/// </summary>
		/// <returns>true if successfully cleared</returns>
		public bool ClearLoaded() 
		{
			#if UNITY_IPHONE
			return _heliumSdkBannerClearLoaded(_uniqueId);
			#elif UNITY_ANDROID
			return _androidAd.Call<bool>("clearLoaded");
			#else
			return false;
			#endif
		}

		/// <summary>
		/// Remove the banner.
		/// </summary>
		public void Remove()
		{
			#if UNITY_IPHONE
			_heliumSdkBannerRemove(_uniqueId);
			#elif UNITY_ANDROID
			//android doesn't have a remove method. Instead, calling destroy
			Destroy();
			#endif
		}

		/// <summary>This method changes the visibility of the banner ad.</summary>
		/// <param name="isVisible">Specify if the banner should be visible.</param>
		public void SetVisibility(bool isVisible)
		{
			#if UNITY_IPHONE
			_heliumSdkBannerSetVisibility(_uniqueId, isVisible);
			#elif UNITY_ANDROID
			_androidAd.Call("setBannerVisibility", isVisible);
			#endif
		}

		/// <summary>
		/// Destroy the advertisement to free up memory resources.
		/// </summary>
		public void Destroy()
		{
			#if UNITY_ANDROID
			_androidAd.Call("destroy");
			#endif
		}

		~HeliumBannerAd()
		{
			#if UNITY_IPHONE
			_heliumSdkFreeBannerAdObject(_uniqueId);
			#endif
		}
	}
}


