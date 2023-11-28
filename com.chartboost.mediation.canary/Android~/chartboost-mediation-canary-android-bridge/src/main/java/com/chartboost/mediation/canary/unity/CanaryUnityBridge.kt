package com.chartboost.mediation.canary.unity

import android.provider.Settings
import android.widget.Toast
import android.preference.PreferenceManager
import com.amazon.device.ads.AdRegistration
import com.applovin.sdk.AppLovinSdk
import com.facebook.ads.AdSettings
import com.google.android.gms.ads.identifier.AdvertisingIdClient
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class CanaryUnityBridge {
    companion object {
        private const val TC_STRING = "IABTCF_TCString"

        @JvmStatic
        fun setSdkDomainName(sdkDomainName: String) {
            CanaryNetworking.setSdkDomainName(sdkDomainName)
        }

        @JvmStatic
        fun setRtbDomainName(rtbDomainName: String) {
            CanaryNetworking.setRtbDomainName(rtbDomainName)
        }

        @JvmStatic
        fun setAmazonPublisherServicesTestMode(value: Boolean) {
            AdRegistration.enableTesting(value);
        }

        @JvmStatic
        fun setMetaAudienceNetworkTestMode(value: Boolean) {
            AdSettings.setTestMode(value)
        }

        @JvmStatic
        fun setAppLovinTestMode(value: Boolean) {
            UnityPlayer.currentActivity?.let {
                if (value) {
                    // Getting Advertising Id Info needs to be launched in the background.
                    CoroutineScope(Dispatchers.IO).launch {
                        val adInfo = try {
                            AdvertisingIdClient.getAdvertisingIdInfo(it).id
                        } catch (e: Exception) {
                            withContext(Dispatchers.Main) {
                                Toast.makeText(
                                    it,
                                    "Using alternative ad ID for AppLovin's Test Mode.",
                                    Toast.LENGTH_SHORT
                                ).show()
                            }
                            it.contentResolver.let { contentResolver ->
                                Settings.Secure.getString(contentResolver, "advertising_id")
                            }
                        }

                        adInfo?.let { adId ->
                            withContext(Dispatchers.Main) {
                                // AppLovin's testDeviceAdvertisingIds needs a list.
                                AppLovinSdk.getInstance(it).settings.testDeviceAdvertisingIds = listOf(adId)
                            }
                        }
                    }
                }

                // AppLovin's testDeviceAdvertisingIds needs a list.
                else
                    AppLovinSdk.getInstance(it).settings.testDeviceAdvertisingIds = listOf()
            }
        }

        @JvmStatic
        fun setTcString(value: String) {
            UnityPlayer.currentActivity?.let {
                val defaultSharedPreferences = PreferenceManager.getDefaultSharedPreferences(it)
                defaultSharedPreferences.edit().putString(TC_STRING, value)
                        .apply()
            }
        }
    }
}
