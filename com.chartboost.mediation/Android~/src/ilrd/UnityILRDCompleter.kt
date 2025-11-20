@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.ilrd

/**
 * Callback interface for notifying completion of ILRD (Impression Level Revenue Data) processing.
 *
 * This interface is used by the Unity layer to signal when it has finished processing
 * an impression event, allowing the native layer to remove the cached data.
 *
 * @see UnityILRDConsumer
 * @see UnityILRDObserver
 */
interface UnityILRDCompleter {
    /**
     * Called when Unity has successfully processed an ILRD impression event.
     *
     * This callback signals that the impression data can be safely removed from
     * the cache as it has been consumed by the Unity layer.
     *
     * @param uniqueId The unique identifier (hash code) of the processed impression data
     */
    fun completed(uniqueId: Int)
}
