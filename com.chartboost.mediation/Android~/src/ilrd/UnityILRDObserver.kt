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
import java.util.concurrent.ConcurrentHashMap

/**
 * Observer implementation for handling ILRD (Impression Level Revenue Data) events.
 *
 * This class implements the Chartboost Mediation SDK's ILRD observer interface and manages
 * a persistent cache of impression data to ensure no impressions are lost if Unity is not
 * ready to process them immediately.
 *
 * Thread Safety: All operations are thread-safe using ConcurrentHashMap and Mutex for file I/O.
 * Memory Management: Cached impressions are automatically persisted to disk and removed once
 * processed by the Unity layer via the [UnityILRDCompleter] callback.
 *
 * @see ChartboostMediationIlrdObserver
 * @see UnityILRDConsumer
 * @see UnityILRDCompleter
 */
@Suppress("unused")
class UnityILRDObserver : ChartboostMediationIlrdObserver {

    /**
     * Called by the Chartboost Mediation SDK when an impression occurs.
     *
     * This method transforms the native impression data into a Unity-compatible JSON format
     * and caches it for processing by the Unity layer.
     *
     * @param impData The impression data from the Chartboost Mediation SDK
     */
    override fun onImpression(impData: ChartboostMediationImpressionData) {
        val unityILRD = JSONObject()
        unityILRD.put(KEY_PLACEMENT, impData.placementId)
        unityILRD.put(KEY_ILRD, impData.ilrdInfo)

        val unityILRDJson = unityILRD.toString()
        cacheImpressionData(unityILRDJson)
    }

    companion object {
        /**
         * Tag used for logging operations related to ILRD.
         */
        private const val TAG = "UnityILRDObserver"

        /**
         * Name of the cache file for persisting impression data.
         */
        private const val CACHE_FILE_NAME = "ilrd_cache.json"

        /**
         * JSON key for the placement identifier.
         */
        private const val KEY_PLACEMENT = "placement"

        /**
         * JSON key for the ILRD information.
         */
        private const val KEY_ILRD = "ilrd"

        /**
         * Maximum number of cached impressions to prevent memory leaks.
         */
        private const val MAX_CACHE_SIZE = 1000

        /**
         * Mutex for synchronizing file I/O operations.
         */
        private val fileWriteMutex = Mutex()

        /**
         * Thread-safe storage for impression data keyed by hash codes.
         * Stores references to prevent data loss while waiting for Unity processing.
         */
        private val ilrdCache: ConcurrentHashMap<Int, String> = ConcurrentHashMap()

        /**
         * Consumer callback provided by the Unity layer to receive impression events.
         * Must be set via [setUnityILRDProxy] before impressions can be delivered to Unity.
         */
        @Volatile
        private var unityILRDProxy: UnityILRDConsumer? = null

        /**
         * Flag indicating whether cached impressions should be automatically consumed when
         * retrieved from disk. Defaults to true.
         */
        @Volatile
        private var consumeILRDOnRetrieval = true

        /**
         * Sets the Unity consumer callback for receiving impression events.
         *
         * This must be called from the Unity C# layer during initialization to enable
         * impression data delivery. Once set, any cached impressions will be delivered
         * immediately.
         *
         * @param unityILRDConsumer The consumer callback to receive impression events
         */
        @JvmStatic
        fun setUnityILRDProxy(unityILRDConsumer: UnityILRDConsumer) {
            unityILRDProxy = unityILRDConsumer
            UnityLoggingBridge.log(TAG, "Set UnityILRD Consumer", LogLevel.VERBOSE)
        }

        /**
         * Configures whether cached impressions should be automatically consumed when loaded from disk.
         *
         * When true (default), calling [retrieveImpressionData] will automatically deliver all
         * cached impressions to the Unity consumer. When false, impressions remain cached until
         * explicitly consumed.
         *
         * @param value True to auto-consume on retrieval, false to keep cached
         */
        @JvmStatic
        fun setConsumeILRDOnRetrieval(value: Boolean) {
            consumeILRDOnRetrieval = value
            UnityLoggingBridge.log(TAG, "ILRD consumed on cached retrieval set to: $value", LogLevel.VERBOSE)
        }

        /**
         * Retrieves cached impression data from disk and optionally delivers it to Unity.
         *
         * This method asynchronously loads the cache file from disk. If [consumeILRDOnRetrieval]
         * is true, all cached impressions will be delivered to the Unity consumer immediately.
         *
         * This method should be called during Unity initialization to recover any impressions
         * that were cached before the previous session ended.
         */
        @JvmStatic
        fun retrieveImpressionData() {
            CoroutineScope(Dispatchers.IO).launch {
                fileWriteMutex.withLock {
                    val activity = UnityPlayer.currentActivity
                    if (activity == null) {
                        UnityLoggingBridge.log(TAG, "Cannot retrieve impression data: Activity not available", LogLevel.ERROR)
                        return@withLock
                    }

                    try {
                        UnityLoggingBridge.log(TAG, "Attempting to retrieve impression data", LogLevel.VERBOSE)
                        val file = File(activity.filesDir, CACHE_FILE_NAME)
                        if (file.exists()) {
                            val ilrdCacheJson = file.readText()
                            UnityLoggingBridge.log(TAG, "Read file at ${file.absolutePath}", LogLevel.VERBOSE)

                            val cachedData = Json.decodeFromString<Map<Int, String>>(ilrdCacheJson)
                            ilrdCache.putAll(cachedData)

                            if (consumeILRDOnRetrieval) {
                                cachedData.forEach { entry ->
                                    requestUnityILRDConsumption(entry.value)
                                }
                            }
                        } else {
                            UnityLoggingBridge.log(TAG, "Nothing to retrieve. Cache is clean", LogLevel.VERBOSE)
                        }
                    } catch (e: Exception) {
                        UnityLoggingBridge.log(TAG, "Failed to retrieve impression data: ${e.message}", LogLevel.ERROR)
                        // Delete corrupted cache file
                        try {
                            File(activity.filesDir, CACHE_FILE_NAME).delete()
                            UnityLoggingBridge.log(TAG, "Deleted corrupted cache file", LogLevel.VERBOSE)
                        } catch (deleteException: Exception) {
                            UnityLoggingBridge.log(TAG, "Failed to delete corrupted cache: ${deleteException.message}", LogLevel.ERROR)
                        }
                    }
                }
            }
        }

        /**
         * Requests Unity to consume an impression event.
         *
         * This method delivers the impression data to the Unity consumer via the [unityILRDProxy]
         * callback. The consumer must invoke the completer callback when processing is finished
         * to allow removal from the cache.
         *
         * @param unityILRD JSON string containing the impression data
         */
        private fun requestUnityILRDConsumption(unityILRD: String) {
            val proxy = unityILRDProxy
            if (proxy == null) {
                UnityLoggingBridge.log(TAG, "Cannot request Unity consumption: Consumer not set", LogLevel.WARNING)
                return
            }

            UnityLoggingBridge.log(TAG, "Requesting Unity consumption", LogLevel.VERBOSE)
            proxy.onImpression(unityILRD.hashCode(), unityILRD, object : UnityILRDCompleter {
                override fun completed(uniqueId: Int) {
                    UnityLoggingBridge.log(TAG, "Unity consumption completed for ID: $uniqueId", LogLevel.VERBOSE)
                    removeImpressionData(uniqueId)
                }
            })
        }

        /**
         * Caches a new impression event and attempts to deliver it to Unity.
         *
         * This method stores the impression data in memory and persists it to disk. If the
         * impression is already cached (based on hash code), it is ignored. New impressions
         * are immediately delivered to Unity if a consumer is registered.
         *
         * @param unityILRD JSON string containing the impression data
         */
        private fun cacheImpressionData(unityILRD: String) {
            if (ilrdCache.size >= MAX_CACHE_SIZE) {
                UnityLoggingBridge.log(TAG, "ILRD cache exceeded maximum size ($MAX_CACHE_SIZE). Possible memory leak", LogLevel.ERROR)
            }

            val hashCode = unityILRD.hashCode()

            // Only process if not already cached (thread-safe check and put)
            if (ilrdCache.putIfAbsent(hashCode, unityILRD) != null) {
                UnityLoggingBridge.log(TAG, "Impression already cached with ID: $hashCode", LogLevel.VERBOSE)
                return
            }

            UnityLoggingBridge.log(TAG, "Cached new impression with ID: $hashCode (Total cached: ${ilrdCache.size})", LogLevel.VERBOSE)

            // Only request consumption for the newly added impression
            requestUnityILRDConsumption(unityILRD)
            saveCacheToFile()
        }

        /**
         * Removes a processed impression from the cache.
         *
         * This method is called by the completer callback when Unity has finished processing
         * an impression. The impression is removed from memory and the cache file is updated.
         *
         * @param uniqueId The unique identifier (hash code) of the impression to remove
         */
        private fun removeImpressionData(uniqueId: Int) {
            val removed = ilrdCache.remove(uniqueId)
            if (removed == null) {
                UnityLoggingBridge.log(TAG, "Attempted to remove non-existent impression with ID: $uniqueId", LogLevel.WARNING)
                return
            }

            UnityLoggingBridge.log(TAG, "Removed impression with ID: $uniqueId (Remaining cached: ${ilrdCache.size})", LogLevel.VERBOSE)
            saveCacheToFile()
        }

        /**
         * Persists the current cache to disk asynchronously.
         *
         * This method saves the in-memory cache to a JSON file for recovery after app restart.
         * If the cache is empty, the file is deleted. Uses atomic write (temp file + rename)
         * to prevent corruption during crashes.
         */
        private fun saveCacheToFile() {
            CoroutineScope(Dispatchers.IO).launch {
                fileWriteMutex.withLock {
                    val activity = UnityPlayer.currentActivity
                    if (activity == null) {
                        UnityLoggingBridge.log(TAG, "Cannot save cache: Activity not available", LogLevel.ERROR)
                        return@withLock
                    }

                    try {
                        val file = File(activity.filesDir, CACHE_FILE_NAME)
                        val tempFile = File(activity.filesDir, "$CACHE_FILE_NAME.tmp")

                        if (ilrdCache.isEmpty()) {
                            // Delete both cache and temp files if cache is empty
                            if (file.exists()) {
                                file.delete()
                                UnityLoggingBridge.log(TAG, "Cache empty, file at ${file.absolutePath} deleted", LogLevel.VERBOSE)
                            }
                            if (tempFile.exists()) {
                                tempFile.delete()
                            }
                            return@withLock
                        }

                        // Atomic write: write to temp file first, then rename
                        val ilrdCacheJson = Json.encodeToString(ilrdCache.toMap())
                        tempFile.writeText(ilrdCacheJson)
                        tempFile.renameTo(file)
                        UnityLoggingBridge.log(TAG, "Saved cache to ${file.absolutePath} with ${ilrdCache.size} impressions", LogLevel.VERBOSE)
                    } catch (e: IOException) {
                        UnityLoggingBridge.log(TAG, "Failed to save ILRD cache: ${e.message}", LogLevel.ERROR)
                    }
                }
            }
        }

        /**
         * Returns diagnostic information about the current state of the ILRD cache.
         *
         * Provides count of cached impressions for debugging and monitoring.
         *
         * @return A formatted string containing cache statistics
         */
        @JvmStatic
        fun getCacheInfo(): String {
            return "$TAG - Cached Impressions: ${ilrdCache.size}, Consumer Set: ${unityILRDProxy != null}"
        }
    }
}
