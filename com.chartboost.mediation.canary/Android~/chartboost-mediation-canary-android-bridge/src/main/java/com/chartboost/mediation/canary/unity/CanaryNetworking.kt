package com.chartboost.mediation.canary.unity

import android.widget.Toast
import com.unity3d.player.UnityPlayer

class CanaryNetworking {
    /*
    * An enum class that has information in regards with helium endpoints that will be updated from
    * within the canary app. Any new domains introduced can be added here for simplicity.
    */
    private enum class Domains(val classPath: String) {
        RTB_DOMAIN("com.chartboost.heliumsdk.network.Endpoints"),
        SDK_DOMAIN("com.chartboost.heliumsdk.network.Endpoints"),
    }

    companion object {
        @JvmStatic
        fun setSdkDomainName(sdkDomainName: String) {
            reflectUpdateEndpoint(Domains.SDK_DOMAIN.classPath, Domains.SDK_DOMAIN.name, sdkDomainName)
        }

        @JvmStatic
        fun setRtbDomainName(rtbDomainName: String) {
            reflectUpdateEndpoint(Domains.RTB_DOMAIN.classPath, Domains.RTB_DOMAIN.name, rtbDomainName)
        }

        private fun reflectUpdateEndpoint(className: String, endpointFieldName: String, customDomain: String) {
            val context = UnityPlayer.currentActivity
            try {
                val reflectClass = Class.forName(className)
                val endpointField = reflectClass.getDeclaredField(endpointFieldName).also {
                    it.isAccessible = true
                }

                endpointField.set(endpointField, customDomain)
                endpointField.isAccessible = false
            }
            catch (e: ClassNotFoundException) {
                Toast.makeText(
                    context,
                    "Failed to set endpoint due to not finding class for $className endpoint.",
                    Toast.LENGTH_SHORT
                ).show()
            } catch (e: Exception) {
                Toast.makeText(
                    context,
                    "Failed to set $className endpoint due to an exception found.",
                    Toast.LENGTH_SHORT
                ).show()
            }
        }
    }
}
