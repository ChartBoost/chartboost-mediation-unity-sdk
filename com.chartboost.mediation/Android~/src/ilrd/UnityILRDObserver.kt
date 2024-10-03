@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.ilrd

import com.chartboost.chartboostmediationsdk.ChartboostMediationIlrdObserver
import com.chartboost.chartboostmediationsdk.ChartboostMediationImpressionData
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import org.json.JSONObject
import java.io.File
import java.io.IOException

@Suppress("unused")
class UnityILRDObserver : ChartboostMediationIlrdObserver {

    private val keyPlacement: String = "placement"
    private val keyILRD: String = "ilrd"

    override fun onImpression(impData: ChartboostMediationImpressionData) {
        val unityILRD = JSONObject()
        unityILRD.put(keyPlacement, impData.placementId)
        unityILRD.put(keyILRD, impData.ilrdInfo)

        val unityILRDJson = unityILRD.toString()
        cacheImpressionData(unityILRDJson)
    }

    companion object {

        private val fileWriteMutex = Mutex()

        private var TAG = UnityILRDObserver::class.simpleName

        private const val CACHE_FILE_NAME = "ilrd_cache.json"

        @JvmStatic
        private var ilrdCache : MutableMap<Int, String> =  mutableMapOf()

        private var unityILRDProxy : UnityILRDConsumer? = null

        private var consumeILRDOnRetrieval = true

        @JvmStatic
        fun setUnityILRDProxy(unityILRDConsumer: UnityILRDConsumer)
        {
            unityILRDProxy = unityILRDConsumer
            UnityLoggingBridge.log(TAG, "Set UnityILRD Consumer", LogLevel.VERBOSE)
        }

        @JvmStatic
        fun setConsumeILRDOnRetrieval(value: Boolean) {
            consumeILRDOnRetrieval = value
            UnityLoggingBridge.log(TAG, "ILRD Consumed on Cached Retrieval Set to: $value", LogLevel.VERBOSE)
        }

        @JvmStatic
        fun retrieveImpressionData() {
            CoroutineScope(Dispatchers.IO).launch {
                fileWriteMutex.withLock {
                    UnityPlayer.currentActivity.let {
                        UnityLoggingBridge.log(TAG,"Attempting to retrieve impression data", LogLevel.VERBOSE)
                        val file = File(it.filesDir, CACHE_FILE_NAME)
                        if (file.exists()) {
                            val ilrdCacheJson = file.readText()
                            UnityLoggingBridge.log(TAG, "Read file at ${file.absolutePath} with contents: $ilrdCacheJson", LogLevel.VERBOSE)
                            ilrdCache = Json.decodeFromString(ilrdCacheJson)
                            if (consumeILRDOnRetrieval) {
                                ilrdCache.forEach { entry ->
                                    requestUnityILRDConsumption(entry.value)
                                }
                            }
                        }
                        else {
                            UnityLoggingBridge.log(TAG, "Nothing to retrieve. Cache is clean.", LogLevel.VERBOSE)
                        }
                    }
                }
            }
        }

        private fun requestUnityILRDConsumption(unityILRD: String) {
            UnityLoggingBridge.log(TAG, "Requesting Unity consumption", LogLevel.VERBOSE)
            unityILRDProxy?.onImpression(unityILRD.hashCode(), unityILRD, object : UnityILRDCompleter {
                override fun completed(uniqueId: Int) {
                    UnityLoggingBridge.log(TAG, "Unity consumption completed", LogLevel.VERBOSE)
                    removeImpressionData(uniqueId)
                }
            })
        }

        private fun cacheImpressionData(unityILRD: String) {

            val hashCode = unityILRD.hashCode()
            if (ilrdCache.containsKey(hashCode))
                return

            ilrdCache[hashCode] = unityILRD
            ilrdCache.forEach{ entry ->
                requestUnityILRDConsumption(entry.value)
            }
            saveCacheToFile()
        }

        private fun removeImpressionData(uniqueId: Int)
        {
            if (!ilrdCache.containsKey(uniqueId))
            {
                UnityLoggingBridge.log(TAG, "Requested ID $uniqueId to be removed from ILRD Cache but no such key is present", LogLevel.WARNING)
                return
            }

            ilrdCache.remove(uniqueId)
            saveCacheToFile()
        }

        private fun saveCacheToFile() {
            CoroutineScope(Dispatchers.IO).launch {
                fileWriteMutex.withLock {
                    try {
                        UnityPlayer.currentActivity.let {
                            val file = File(it.filesDir, CACHE_FILE_NAME)

                            if (ilrdCache.isEmpty()) {
                                if (file.exists()) {
                                    file.delete()
                                    UnityLoggingBridge.log(TAG, "Cache empty file at ${file.absolutePath} deleted", LogLevel.VERBOSE)
                                    return@withLock
                                }
                                UnityLoggingBridge.log(TAG, "Cache empty and no file at ${file.absolutePath}", LogLevel.VERBOSE)
                                return@withLock
                            }

                            val ilrdCacheJson = Json.encodeToString(ilrdCache)
                            file.writeText(ilrdCacheJson)
                            UnityLoggingBridge.log(TAG, "Saved file at ${file.absolutePath} with ilrd: $ilrdCacheJson", LogLevel.VERBOSE)
                        }
                    }
                    catch (e: IOException) {
                        UnityLoggingBridge.logException(TAG, "Failed to save ILRD Cache with Exception: $e")
                    }
                }
            }
        }
    }
}
